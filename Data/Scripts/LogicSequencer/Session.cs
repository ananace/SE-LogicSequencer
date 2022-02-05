using System;
using System.Collections.Generic;
using RichHudFramework.Client;
using RichHudFramework.UI.Client;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;

namespace LogicSequencer
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    partial class Session : MySessionComponentBase
    {
        public string ModPath;
        public static Session Instance { get; private set; }

        public List<LogicProgramRun> RunningScripts { get; private set; } = new List<LogicProgramRun>();

        UI.SequenceEditor SequenceEditor;

        public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
        {
            Util.Log.Info("ModContext;");
            Util.Log.Info($"ModId: {ModContext.ModId}");
            Util.Log.DebugEnabled = ModContext.ModId == "SE-LogicSequencer";

            Instance = this;
            ModPath = ModContext.ModPath;
            RichHudClient.Init("LogicSequencer", HudInit, ClientReset);

            Util.Log.Debug("In Init()");
            Util.Log.Debug($"IsServer: {MyAPIGateway.Multiplayer.IsServer}");
            Util.Log.Debug($"IsDedicated: {MyAPIGateway.Utilities.IsDedicated}");

            if (MyAPIGateway.Multiplayer.IsServer)
                MyAPIGateway.TerminalControls.CustomControlGetter += CustomControlGetter;

            SetupModAPI();
            RegisterInternalServices();
            RegisterInternalStateSources();

            if (Util.Log.DebugEnabled)
                TestSerialize();
        }

        public override void SaveData()
        {
            // TODO: Dump script states
        }

        public override void LoadData()
        {
            Util.Networking.Instance.Register();

            // TODO: Restore script states
        }

        protected override void UnloadData()
        {
            Instance = null;

            foreach (var vm in RunningScripts)
                vm.Stop();
            RunningScripts.Clear();
            UnregisterStateSources();
            UnregisterServices();
            UnloadModAPI();

            MyAPIGateway.TerminalControls.CustomControlGetter -= CustomControlGetter;
            Util.Networking.Instance.Unregister();
        }

        void UpdateFirstTick()
        {
            // Implicitly loads, and also logs the loading result.
            Util.Log.Info($"Loaded {AvailableScripts.Count} sequences.");

            if (!MyAPIGateway.Utilities.FileExistsInLocalStorage("List.txt", GetType()))
                using (var writer = MyAPIGateway.Utilities.WriteFileInLocalStorage("List.txt", GetType()))
                {
                    writer.WriteLine("# This is a list of logic sequences to load, from this folder");
                    writer.WriteLine("# Can be written with - or without - their .xml extension");
                    writer.WriteLine("# Lines starting with a # will be ignored");
                    writer.WriteLine();
                    writer.WriteLine("#My Sequence.xml");
                }

            try
            {
                if (!MyAPIGateway.Utilities.FileExistsInLocalStorage("Template.xml", GetType()))
                {
                    using (var writer = MyAPIGateway.Utilities.WriteFileInLocalStorage("Template.xml", GetType()))
                    {
                        writer.WriteLine(MyAPIGateway.Utilities.SerializeToXML(new Script.ScriptSequence {
                            Name = "Template",
                            Description = "This is a template for a sequence, copy and rename it in a new file",

                            Variables = new VRage.Serialization.SerializableDictionary<string, Script.ScriptValue> {
                                Dictionary = new Dictionary<string, Script.ScriptValue> {
                                    { "example_bool", new Script.ScriptValue { Boolean = true } },
                                    { "example_int", new Script.ScriptValue { Integer = 9001 } },
                                    { "example_real", new Script.ScriptValue { Real = 1.21 } },
                                    { "example_string", new Script.ScriptValue { String = "Hello world" } }
                                }
                            },

                            Triggers = new List<Script.ScriptTrigger> {
                                { new Script.Triggers.Action { Name = "When Used" } },
                                { new Script.Triggers.GridChange { Name = "When grid changed" } }
                            },
                            Conditions = new List<Script.ScriptCondition> {
                                { new Script.Conditions.Comparison { SourceData = new Script.DataSource { VariableName = "example_int" }, ComparisonData = new Script.DataSource { VariableName = "example_real" }, OperationType = Script.Helper.MathHelper.OperationType.CompareGreaterThan } }
                            },
                            Actions = new List<Script.ScriptAction> {
                                { new Script.Actions.CallService { Name = "functional.turn_on", Blocks = new Script.MultiBlockSelector { GroupName = new Script.DataSource { Value = new Script.ScriptValue { String = "All my blocks" } } } } }
                            }
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Util.Log.Error("When writing sequence template", ex, GetType());

                MyAPIGateway.Utilities.DeleteFileInLocalStorage("Template.xml", GetType());
            }
        }

        uint atTick = 0;
        bool firstTick = false;
        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            if (!firstTick)
            {
                firstTick = true;
                UpdateFirstTick();
            }

            if (atTick++ % 100 == 0)
                HandleScriptTriggers();

            // TODO: Handle script triggers
            foreach (var vm in RunningScripts)
            {
                if (vm.WaitingForDuration != null)
                {
                    vm.WaitingForDuration = vm.WaitingForDuration.Value - TimeSpan.FromMilliseconds(10);
                    if (vm.WaitingForDuration.Value.TotalMilliseconds <= 0)
                    {
                        vm.WaitingForDuration = null;
                        vm.WaitingForTrigger = null;
                        vm.LogicSequencer.ReloadTriggers();
                    }
                }
            }
        }

        public override void Draw()
        {
            if (!RichHudClient.Registered)
                return;

        }

        const string MOD_DATA_PATH = "Data\\Sequences";

        List<Script.ScriptSequence> _sequences = null;
        public IReadOnlyList<Script.ScriptSequence> AvailableScripts { get {
            if (_sequences != null)
                return _sequences;

            _sequences = new List<Script.ScriptSequence>();

            Action<System.IO.TextReader, MyObjectBuilder_Checkpoint.ModItem?> _loadSequencesFrom = (reader, mod) => {
                while (reader.Peek() != -1)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;

                    // Support both listing with exact name and without extension
                    var file = line.Trim().Replace(".xml", "") + ".xml";
                    Util.Log.Debug($"Loading sequence {file}...");
                    if (mod != null)
                    {
                        if (!MyAPIGateway.Utilities.FileExistsInModLocation($"{MOD_DATA_PATH}\\{file}", mod.Value))
                        {
                            Util.Log.Info($"Sequence file \"{file}\" listed in mod {mod.Value.FriendlyName} doesn't exist, ignoring.");
                            continue;
                        }

                        try
                        {
                            using (var sequenceReader = MyAPIGateway.Utilities.ReadFileInModLocation($"{MOD_DATA_PATH}\\{file}", mod.Value))
                            {
                                var script = MyAPIGateway.Utilities.SerializeFromXML<Script.ScriptSequence>(sequenceReader.ReadToEnd());
                                _sequences.Add(script);
                            }
                        }
                        catch (Exception ex)
                        {
                            Util.Log.Error($"When loading sequence \"{file}\" from mod {mod.Value.FriendlyName}, ignoring.", ex, GetType(), false);
                        }
                    }
                    else
                    {
                        if (!MyAPIGateway.Utilities.FileExistsInLocalStorage(file, GetType()))
                        {
                            Util.Log.Info($"Sequence file \"{file}\" listed in local storage doesn't exist, ignoring.");
                            continue;
                        }

                        try
                        {
                            using (var sequenceReader = MyAPIGateway.Utilities.ReadFileInLocalStorage(file, GetType()))
                            {
                                var script = MyAPIGateway.Utilities.SerializeFromXML<Script.ScriptSequence>(sequenceReader.ReadToEnd());
                                _sequences.Add(script);
                            }
                        }
                        catch (Exception ex)
                        {
                            Util.Log.Error($"When loading sequence \"{file}\" from local storage, ignoring.", ex, GetType(), false);
                        }
                    }
                }
            };

            if (MyAPIGateway.Utilities.FileExistsInLocalStorage("List.txt", GetType()))
            {
                Util.Log.Debug("Loading sequences from local storage...");
                using (var reader = MyAPIGateway.Utilities.ReadFileInLocalStorage("List.txt", GetType()))
                    _loadSequencesFrom(reader, null);
            }

            foreach(var mod in MyAPIGateway.Session.Mods)
            {
                if (!MyAPIGateway.Utilities.FileExistsInModLocation($"{MOD_DATA_PATH}\\List.txt", mod))
                    continue;

                Util.Log.Debug($"Loading sequences from mod {mod.FriendlyName}...");
                using (var reader = MyAPIGateway.Utilities.ReadFileInModLocation($"{MOD_DATA_PATH}\\List.txt", mod))
                    _loadSequencesFrom(reader, mod);
            }

            return _sequences;
        } }

        public void ShowEditor(Blocks.LogicSequencer sequencer)
        {
            if (!RichHudClient.Registered)
                return;

            SequenceEditor.Script = sequencer.Script;

            HudMain.EnableCursor = true;
            SequenceEditor.Visible = true;
            SequenceEditor.GetFocus();
        }
        public void HideEditor()
        {
            SequenceEditor.Visible = false;
            HudMain.EnableCursor = false;
        }

        void HudInit()
        {
            SequenceEditor = new UI.SequenceEditor(HudMain.HighDpiRoot) {
                Visible = false
            };
        }

        void ClientReset()
        {

        }

        void TestSerialize()
        {
            var variables = new VRage.Serialization.SerializableDictionary<string, Script.ScriptValue>();
            variables["null"] = new Script.ScriptValue(); // TODO: Handle this in scripts?
            variables["boolean"] = new Script.ScriptValue { Boolean = true };
            variables["integer"] = new Script.ScriptValue { Integer = 2 };
            variables["real"] = new Script.ScriptValue { Real = 3.0 };
            variables["string"] = new Script.ScriptValue { String = "Hello" };

            var testScript = new Script.ScriptSequence {
                Name = "Testing",
                Description = "Testing",

                Variables = variables,

                Triggers = new List<Script.ScriptTrigger> {
                    new Script.Triggers.Action { },
                    new Script.Triggers.BlockState { },
                    new Script.Triggers.External { },
                    new Script.Triggers.GridChange { },
                    new Script.Triggers.IGC { },
                    new Script.Triggers.Time { },
                },
                Conditions = new List<Script.ScriptCondition> {
                    new Script.Conditions.And { },
                    new Script.Conditions.BlockPropertyIs { },
                    new Script.Conditions.Comparison { },
                    new Script.Conditions.HasVariable { },
                    new Script.Conditions.Not { },
                    new Script.Conditions.Or { },
                    new Script.Conditions.VariableIsTruthy { },
                    new Script.Conditions.VariableIsType { },
                },
                Actions = new List<Script.ScriptAction> {
                    new Script.Actions.BlockGetProperty { },
                    new Script.Actions.BlockGetState { },
                    new Script.Actions.BlockRunAction { },
                    new Script.Actions.BlockSetProperty { },
                    new Script.Actions.CallService { },
                    new Script.Actions.Choose { },
                    new Script.Actions.ConditionAction { },
                    new Script.Actions.Delay { },
                    new Script.Actions.RepeatTimes { },
                    new Script.Actions.RepeatUntil { },
                    new Script.Actions.RepeatWhile { },
                    new Script.Actions.SetVariables { },
                    new Script.Actions.StorePermanentVariables { },
                    new Script.Actions.WaitTrigger { },
                }
            };

            var serialized = Convert.ToBase64String(MyAPIGateway.Utilities.SerializeToBinary(testScript));
            var serializedXML = MyAPIGateway.Utilities.SerializeToXML(testScript);
            var unserialized = MyAPIGateway.Utilities.SerializeFromBinary<Script.ScriptSequence>(Convert.FromBase64String(serialized));
            var unserializedXML = MyAPIGateway.Utilities.SerializeFromXML<Script.ScriptSequence>(serializedXML);
        }
    }
}
