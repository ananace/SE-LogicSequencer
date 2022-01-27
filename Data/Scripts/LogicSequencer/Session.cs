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

        uint atTick = 0;
        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

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

        public IEnumerable<Script.ScriptSequence> AvailableScripts()
        {
            yield return new Script.ScriptSequence {
                Name = "Grid mass warning",
                Description = "Notifies the player if the grid mass exceeds specific values",

                Variables = new VRage.Serialization.SerializableDictionary<string, Script.ScriptValue> {
                    Dictionary = new Dictionary<string, Script.ScriptValue> {
                        { "mass_min", new Script.ScriptValue { Real = 1000 } },
                        { "mass_med", new Script.ScriptValue { Real = 1500 } },
                        { "mass_max", new Script.ScriptValue { Real = 2000 } },
                        { "group_name", new Script.ScriptValue { String = "Mass Warning" } },

                        { "min_color_r", new Script.ScriptValue { Integer = 0 } },
                        { "min_color_g", new Script.ScriptValue { Integer = 255 } },
                        { "min_color_b", new Script.ScriptValue { Integer = 0 } },

                        { "med_color_r", new Script.ScriptValue { Integer = 255 } },
                        { "med_color_g", new Script.ScriptValue { Integer = 255 } },
                        { "med_color_b", new Script.ScriptValue { Integer = 0 } },

                        { "max_color_r", new Script.ScriptValue { Integer = 255 } },
                        { "max_color_g", new Script.ScriptValue { Integer = 0 } },
                        { "max_color_b", new Script.ScriptValue { Integer = 0 } },
                    }
                },

                Triggers = new List<Script.ScriptTrigger> {
                    new Script.Triggers.GridChange(),
                    new Script.Triggers.Time {
                        Every = TimeSpan.FromMinutes(1)
                    }
                },
                Actions = new List<Script.ScriptAction> {
                    new Script.Actions.BlockGetState {
                        Block = new Script.BlockSelector { Self = true },
                        StateSource = "grid.mass",
                        IntoVariable = "grid_mass"
                    },
                    new Script.Actions.Choose {
                        Choices = new List<Script.Actions.Choose.Choice> {
                            new Script.Actions.Choose.Choice {
                                Conditions = new List<Script.ScriptCondition> {
                                    new Script.Conditions.Comparison {
                                        SourceData = new Script.DataSource { VariableName = "grid_mass" },
                                        ComparisonData = new Script.DataSource { VariableName = "mass_max" },
                                        OperationType = Script.Helper.MathHelper.OperationType.CompareGreaterEqual
                                    }
                                },
                                Actions = new List<Script.ScriptAction> {
                                    new Script.Actions.CallService {
                                        Name = "light.turn_on",
                                        Blocks = new Script.MultiBlockSelector {
                                            GroupName = new Script.DataSource { VariableName = "group_name" }
                                        },
                                    },
                                    new Script.Actions.CallService {
                                        Name = "light.color",
                                        Blocks = new Script.MultiBlockSelector {
                                            GroupName = new Script.DataSource { VariableName = "group_name" }
                                        },
                                        Parameters = new VRage.Serialization.SerializableDictionary<string, Script.DataSource> {
                                            Dictionary = new Dictionary<string, Script.DataSource> {
                                                { "Red", new Script.DataSource { VariableName = "color_max_r" } },
                                                { "Green", new Script.DataSource { VariableName = "color_max_g" } },
                                                { "Blue", new Script.DataSource { VariableName = "color_max_b" } },
                                            }
                                        }
                                    },
                                    new Script.Actions.CallService {
                                        Name = "sound.start",
                                        Blocks = new Script.MultiBlockSelector {
                                            GroupName = new Script.DataSource { VariableName = "group_name" }
                                        }
                                    }
                                }
                            },
                            new Script.Actions.Choose.Choice {
                                Conditions = new List<Script.ScriptCondition> {
                                    new Script.Conditions.Comparison {
                                        SourceData = new Script.DataSource { VariableName = "grid_mass" },
                                        ComparisonData = new Script.DataSource { VariableName = "mass_med" },
                                        OperationType = Script.Helper.MathHelper.OperationType.CompareGreaterEqual
                                    }
                                },
                                Actions = new List<Script.ScriptAction> {
                                    new Script.Actions.CallService {
                                        Name = "light.turn_on",
                                        Blocks = new Script.MultiBlockSelector {
                                            GroupName = new Script.DataSource { VariableName = "group_name" }
                                        },
                                    },
                                    new Script.Actions.CallService {
                                        Name = "light.color",
                                        Blocks = new Script.MultiBlockSelector {
                                            GroupName = new Script.DataSource { VariableName = "group_name" }
                                        },
                                        Parameters = new VRage.Serialization.SerializableDictionary<string, Script.DataSource> {
                                            Dictionary = new Dictionary<string, Script.DataSource> {
                                                { "Red", new Script.DataSource { VariableName = "color_med_r" } },
                                                { "Green", new Script.DataSource { VariableName = "color_med_g" } },
                                                { "Blue", new Script.DataSource { VariableName = "color_med_b" } },
                                            }
                                        }
                                    },
                                    new Script.Actions.CallService {
                                        Name = "sound.stop",
                                        Blocks = new Script.MultiBlockSelector {
                                            GroupName = new Script.DataSource { VariableName = "group_name" }
                                        },
                                    }
                                }
                            },
                            new Script.Actions.Choose.Choice {
                                Conditions = new List<Script.ScriptCondition> {
                                    new Script.Conditions.Comparison {
                                        SourceData = new Script.DataSource { VariableName = "grid_mass" },
                                        ComparisonData = new Script.DataSource { VariableName = "mass_min" },
                                        OperationType = Script.Helper.MathHelper.OperationType.CompareGreaterEqual
                                    }
                                },
                                Actions = new List<Script.ScriptAction> {
                                    new Script.Actions.CallService {
                                        Name = "light.turn_on",
                                        Blocks = new Script.MultiBlockSelector {
                                            GroupName = new Script.DataSource { VariableName = "group_name" }
                                        },
                                    },
                                    new Script.Actions.CallService {
                                        Name = "light.color",
                                        Blocks = new Script.MultiBlockSelector {
                                            GroupName = new Script.DataSource { VariableName = "group_name" }
                                        },
                                        Parameters = new VRage.Serialization.SerializableDictionary<string, Script.DataSource> {
                                            Dictionary = new Dictionary<string, Script.DataSource> {
                                                { "Red", new Script.DataSource { VariableName = "color_min_r" } },
                                                { "Green", new Script.DataSource { VariableName = "color_min_g" } },
                                                { "Blue", new Script.DataSource { VariableName = "color_min_b" } },
                                            }
                                        }
                                    },
                                    new Script.Actions.CallService {
                                        Name = "sound.stop",
                                        Blocks = new Script.MultiBlockSelector {
                                            GroupName = new Script.DataSource { VariableName = "group_name" }
                                        },
                                    }
                                }
                            },
                        },

                        DefaultActions = new List<Script.ScriptAction> {
                            new Script.Actions.CallService {
                                Name = "light.turn_off",
                                Blocks = new Script.MultiBlockSelector {
                                    GroupName = new Script.DataSource { VariableName = "group_name" }
                                },
                            },
                            new Script.Actions.CallService {
                                Name = "sound.stop",
                                Blocks = new Script.MultiBlockSelector {
                                    GroupName = new Script.DataSource { VariableName = "group_name" }
                                },
                            }
                        }
                    }
                }
            };
        }

        public void ShowEditor(Blocks.LogicSequencer sequencer)
        {
            if (!RichHudClient.Registered)
                return;

            SequenceEditor.Script = sequencer.Script;

            SequenceEditor.Visible = true;
            HudMain.EnableCursor = true;
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
                    new Script.Triggers.IGC { },
                    new Script.Triggers.Sun { },
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
                    new Script.Actions.ArithmeticComplex { },
                    new Script.Actions.ArithmeticSimple { },
                    new Script.Actions.ArithmeticSimpleSingle { },
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

            try
            {
                var serialized = Convert.ToBase64String(MyAPIGateway.Utilities.SerializeToBinary(testScript));
                var unserialized = MyAPIGateway.Utilities.SerializeFromBinary<Script.ScriptSequence>(Convert.FromBase64String(serialized));
            }
            catch (Exception ex)
            {
                Util.Log.Error("In TestSerialize()", ex, GetType());
            }
        }
    }
}
