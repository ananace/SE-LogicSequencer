using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicSequencer.Script;
using LogicSequencer.Util;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;

namespace LogicSequencer.Blocks
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_UpgradeModule), false, "LogicSequencerLarge", "LogicSequencerSmall")]
    public class LogicSequencer : MyGameLogicComponent
    {
        [Flags]
        public enum ChangedSettingsFlags
        {
            Settings = 0x01,
            Script = 0x02
        }

        public delegate void OnStartedEvent(LogicProgramRun vm);
        public event OnStartedEvent OnStarted;

        Serialization.LogicSequencerBlockSettings Settings { get; set; } = new Serialization.LogicSequencerBlockSettings();

        ScriptSequence _Script = new ScriptSequence();
        public ScriptSequence Script { get { return _Script ?? new ScriptSequence(); } set { _Script = value ?? new ScriptSequence(); ReloadScript(); SettingsChanged(ChangedSettingsFlags.Script); } }
        public IMyUpgradeModule Block { get; private set; }

        public int MaxRuns { get { return Settings.MaxRuns; } set { Settings.MaxRuns = value; SettingsChanged(); } }
        public ProgramStartMode StartMode { get { return Settings.StartMode; } set { Settings.StartMode = value; SettingsChanged(); } }
        bool _EditingScript = false;
        public bool EditingScript { get { return _EditingScript; } set { if (_EditingScript == false && value) { CopyScriptToCustomData(); } else if (_EditingScript == true && !value) { Block.CustomData = _PreEditCustomData; } _EditingScript = value; } }
        string _PreEditCustomData = null;

        public DateTime LastStarted { get; private set; }

        public bool IsRunning => CurrentExecutions.Any();
        public bool SupportsMaxRuns => StartMode == ProgramStartMode.Multiple || StartMode == ProgramStartMode.Queue;

        public IEnumerable<LogicProgramRun> CurrentExecutions => Session.Instance.RunningScripts.Where(r => r.LogicSequencer == this);

        public void DoTrigger(ScriptTrigger trigger)
        {
            // TODO:
            // if (!HasTrigger(trigger))
            //   return;

            SequenceStart(trigger);
        }

        public void SequenceStart()
        {
            SequenceStart(new Script.Triggers.External());
        }

        public void SequenceStart(ScriptTrigger trigger)
        {
            try
            {
                if (_Script == null)
                    return;

                if (IsRunning)
                {
                    if (StartMode == ProgramStartMode.Single)
                    {
                        Log.Debug($"{Block.EntityId} attempted to start multiple with single start mode, ignoring");
                        return;
                    }
                    if (StartMode != ProgramStartMode.Restart && CurrentExecutions.Count() > MaxRuns)
                    {
                        Log.Debug($"{Block.EntityId} attempted to start more than {MaxRuns} executions, ignoring");
                        return;
                    }
                }

                LogicProgramRun vm = new LogicProgramRun(this)
                {
                    TriggeredBy = trigger
                };

                Session.Instance.RunningScripts.Add(vm);
                if (StartMode == ProgramStartMode.Restart)
                    SequenceStopAll(spare: vm);

                OnStarted?.Invoke(vm);
                LastStarted = DateTime.Now;

                Entity.Components.Get<MyResourceSinkComponent>()?.Update();
            }
            catch (Exception ex)
            {
                Log.Error("SequenceStart()", ex, GetType());
            }
        }
        public void SequenceStop(LogicProgramRun vm)
        {
            try
            {
                Session.Instance.RunningScripts.Remove(vm);

                Entity.Components.Get<MyResourceSinkComponent>()?.Update();
            }
            catch (Exception ex)
            {
                Log.Error("SequenceStop()", ex, GetType());
            }
        }
        public void SequenceStopAll(LogicProgramRun spare = null)
        {
            try
            {
                var toStop = CurrentExecutions.Where(vm => vm != spare).ToArray();
                foreach (var vm in toStop)
                    vm.Stop();

                Entity.Components.Get<MyResourceSinkComponent>()?.Update();
            }
            catch (Exception ex)
            {
                Log.Error("SequenceStop()", ex, GetType());
            }
        }

        public void SequenceTick()
        {
            try
            {
                if (!Block.IsWorking)
                    return;

                var executions = CurrentExecutions.ToArray();
                foreach (var vm in executions)
                {
                    if (!vm.IsActive && StartMode != ProgramStartMode.Queue)
                        continue;

                    vm.RunStep();

                    if (vm.IsCompleted)
                        SequenceStop(vm);
                    else if (StartMode == ProgramStartMode.Queue)
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error("SequenceTick()", ex, GetType());
            }
        }

        public void CopyScriptToCustomData()
        {
            _PreEditCustomData = Block.CustomData;
            Block.CustomData = MyAPIGateway.Utilities.SerializeToXML(_Script);
        }

        void ReloadScript()
        {
            if (!MyAPIGateway.Multiplayer.IsServer)
                return;

            try
            {
                Log.Debug("ReloadScript()");
                // Set up triggers

            }
            catch (Exception ex)
            {
                Log.Error("ReloadScript()", ex, GetType());
            }
        }

        void SequenceAppendCustomInfo(IMyTerminalBlock block, StringBuilder builder)
        {
            try
            {
                if (IsRunning)
                {
                    builder.AppendLine("Logic Sequencer: Running");
                    builder.AppendLine("Current executions:");
                    foreach (var execution in CurrentExecutions)
                        builder.AppendLine($" - For {DateTime.Now - execution.StartedAt}: {execution.StateName} at step {execution.CurrentExecutionStep}@{execution.CurrentExecutionTag}");
                }
                else
                {
                    builder.AppendLine("Logic Sequencer: Idle");
                }
            }
            catch (Exception ex)
            {
                Log.Error("SequenceAppendCustomInfo()", ex, GetType());
            }
        }


        #region Entity stuff

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            Log.Debug("Blocks.LogicSequencer.Init()");

            base.Init(objectBuilder);

            try
            {
                Block = Entity as IMyUpgradeModule;
                if (Block == null)
                    return;

                // IsServer = MyAPIGateway.Multiplayer.IsServer;
                // IsDedicated = MyAPIGateway.Utilities.IsDedicated;

                NeedsUpdate = MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
            }
            catch (Exception ex)
            {
                Log.Error("Init()", ex, GetType());
            }
        }

        public override void Close()
        {
            base.Close();

            try
            {
                if (Block == null)
                    return;

                Block.AppendingCustomInfo -= SequenceAppendCustomInfo;

                SequenceStopAll();

                Block = null;
                NeedsUpdate = MyEntityUpdateEnum.NONE;
            }
            catch (Exception ex)
            {
                Log.Error("Close()", ex, GetType());
            }
        }

        public override void UpdateOnceBeforeFrame()
        {
            base.UpdateOnceBeforeFrame();

            try
            {
                Log.Debug("Blocks.LogicSequencer.UpdateOnceBeforeFrame()");

                // Don't update if block isn't fully loaded
                // Might be a projection
                if (Block.CubeGrid?.Physics == null)
                    return;

                Block.AppendingCustomInfo += SequenceAppendCustomInfo;

                Settings.MaxRuns = 10;
                Settings.StartMode = ProgramStartMode.Single;

                var sink = Entity.Components.Get<MyResourceSinkComponent>();
                if(sink != null)
                {
                    sink.SetRequiredInputFuncByType(MyResourceDistributorComponent.ElectricityId, ComputePowerRequired);
                    sink.Update();
                }

                LoadSettings();
                LoadScript();

                SaveSettings();
                SaveScript();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME | MyEntityUpdateEnum.EACH_100TH_FRAME;
        }

        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            try
            {
                SyncSettings();

                if (!Block.IsFunctional)
                 return;

                if (!MyAPIGateway.Multiplayer.IsServer)
                    return;

                SequenceTick();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public override void UpdateBeforeSimulation100()
        {
            base.UpdateBeforeSimulation100();

            try
            {
                Block.RefreshCustomInfo();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        const float POWER_REQUIRED_MW = 0.01f;
        const float POWER_REQUIRED_PER_EXECUTION_MW = 0.1f;

        private float ComputePowerRequired()
        {
            if(!Block.Enabled || !Block.IsFunctional)
                return 0f;

            // You can of course add some more complicated logic here.
            // However you need to call sink.Update() whenever you think you need the power to update.
            // Updating sink will call sink.SetRequiredInputByType(<ThisMethod>) for every resource type.
            // One way to keep it topped up at a reasonable rate is to use Update100.
            // The game will call Update() when it feels like it too so do some tests.

            return POWER_REQUIRED_MW + CurrentExecutions.Count() * POWER_REQUIRED_PER_EXECUTION_MW;
        }

        #endregion


        #region Storage

        static readonly Guid SETTINGS_GUID = Guid.Parse("6158c8ef-ae66-4c45-b04c-383e326931d0");
        static readonly Guid SCRIPT_GUID = Guid.Parse("6158c8ef-ae66-4c45-b04c-383e326931d1");
        public const int STORAGE_CHANGED_COUNTDOWN = 60;

        int syncCountdown = 0;

        // =================
        // @Digi's FUNCTIONS
        // =================

        // @Digi storage and serialization
        bool LoadSettings()
        {
            try
            {
                if (Block.Storage == null)
                    return false;

                string rawData;
                if (!Block.Storage.TryGetValue(SETTINGS_GUID, out rawData))
                    return false;

                var loadedSettings = MyAPIGateway.Utilities.SerializeFromBinary<Serialization.LogicSequencerBlockSettings>(Convert.FromBase64String(rawData));

                if (loadedSettings == null)
                    return false;

                //_CurrentExecutions.Clear();
                StartMode = loadedSettings.StartMode;
                MaxRuns = loadedSettings.MaxRuns;
                //foreach (var execution in loadedSettings.CurrentExecutions)
                //{
                //    _CurrentExecutions.Add(new LogicProgramRun(this, execution.Variables));
                //    // TODO: "Rewind" script to execution location
                //}
            }
            catch (Exception e)
            {
                Log.Error("LoadSettings()", e, GetType());
            }

            return false;
        }

        bool LoadScript()
        {
            try
            {
                if (Block.Storage == null)
                    return false;

                string rawData;
                if (!Block.Storage.TryGetValue(SCRIPT_GUID, out rawData))
                    return false;

                var loadedScript = MyAPIGateway.Utilities.SerializeFromBinary<ScriptSequence>(Convert.FromBase64String(rawData));

                if (loadedScript == null)
                    return false;

                Script = loadedScript;
            }
            catch (Exception e)
            {
                Log.Error("LoadScript()", e, GetType());
            }

            return false;
        }

        // @Digi storage and serialization
        void SaveSettings()
        {
            try
            {
                if (Block == null)
                    return; // called too soon or after it was already closed, ignore

                if (MyAPIGateway.Utilities == null)
                    throw new NullReferenceException($"MyAPIGateway.Utilities == null; entId={Entity?.EntityId}");

                if (Block.Storage == null)
                    Block.Storage = new MyModStorageComponent();

                Block.Storage.SetValue(SETTINGS_GUID, Convert.ToBase64String(MyAPIGateway.Utilities.SerializeToBinary(Settings)));
            }
            catch (Exception ex)
            {
                Log.Error("SaveSettings()", ex, GetType());
            }
        }

        void SaveScript()
        {
            try
            {
                if (Block == null)
                    return; // called too soon or after it was already closed, ignore

                if (MyAPIGateway.Utilities == null)
                    throw new NullReferenceException($"MyAPIGateway.Utilities == null; entId={Entity?.EntityId}");

                if (Block.Storage == null)
                    Block.Storage = new MyModStorageComponent();

                Block.Storage.SetValue(SCRIPT_GUID, Convert.ToBase64String(MyAPIGateway.Utilities.SerializeToBinary(_Script)));
            }
            catch (Exception ex)
            {
                Log.Error("SaveScript()", ex, GetType());
            }
        }

        ChangedSettingsFlags _changed = 0;

        // @Digi storage and serialization
        public void SettingsChanged(ChangedSettingsFlags flags = ChangedSettingsFlags.Settings)
        {
            _changed |= flags;
            if (syncCountdown == 0)
                syncCountdown = STORAGE_CHANGED_COUNTDOWN;
        }

        // @Digi storage and serialization
        void SyncSettings()
        {
            try
            {
                if (syncCountdown > 0 && --syncCountdown <= 0)
                {
                    if (_changed.HasFlag(ChangedSettingsFlags.Settings))
                    {
                        SaveSettings();
                        var packet = new Serialization.Packets.LogicSequencerBlockSettingsChanged(Settings);
                        Networking.Instance.Send(packet);
                    }
                    if (_changed.HasFlag(ChangedSettingsFlags.Script))
                    {
                        SaveScript();
                        var packet = new Serialization.Packets.LogicSequencerBlockScriptChanged(_Script);
                        Networking.Instance.Send(packet);
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error("SyncSettings()", ex, GetType());
            }
        }

        // @Digi storage and serialization
        public override bool IsSerialized()
        {
            // called when the game iterates components to check if they should be serialized, before they're actually serialized.
            // this does not only include saving but also streaming and blueprinting.
            // NOTE for this to work reliably the MyModStorageComponent needs to already exist in this block with at least one element.

            try
            {
                SaveSettings();
                SaveScript();
            }
            catch (Exception e)
            {
                Log.Error("IsSerialized()", e, GetType());
            }

            return base.IsSerialized();
        }

        #endregion
    }
}
