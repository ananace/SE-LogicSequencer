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
            UnregisterServices();
            UnloadModAPI();

            MyAPIGateway.TerminalControls.CustomControlGetter -= CustomControlGetter;
            Util.Networking.Instance.Unregister();
        }

        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

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
                Name = "Simple arithmetic",
                Description = "A simple arithmetic test",

                Variables = new VRage.Serialization.SerializableDictionary<string, Script.ScriptValue> {
                    Dictionary = new Dictionary<string, Script.ScriptValue> {
                        { "input", new Script.ScriptValue { Integer = 10 } }
                    }
                },

                Actions = new List<Script.ScriptAction> {
                    new Script.Actions.ArithmeticComplex {
                        TargetVariable = "output",
                        Part = new Script.Actions.ArithmeticComplexPart {
                            LHS = new Script.Actions.ArithmeticComplexPart.Part {
                                DataSource = new Script.DataSource { VariableName = "input" },
                            },
                            RHS = new Script.Actions.ArithmeticComplexPart.Part {
                                DataSource = new Script.DataSource { Value = new Script.ScriptValue { Integer = 2 } }
                            },
                            Operator = "Divide"
                        }
                    },
                    new Script.Actions.ConditionAction {
                        Condition = new Script.Conditions.Comparison {
                            SourceData = new Script.DataSource { VariableName = "output" },
                            ComparisonData = new Script.DataSource { Value = new Script.ScriptValue { Integer = 5 } },
                            Operation = "CompareEqual"
                        }
                    },
                    new Script.Actions.Delay {
                        Time = TimeSpan.FromSeconds(10)
                    },
                    new Script.Actions.ArithmeticSimpleSingle {
                        Operand = new Script.DataSource { VariableName = "output" },
                        SingleOperator = "Negate",
                        TargetVariable = "output"
                    },
                    new Script.Actions.StorePermanentVariables {
                        Variables = new VRage.Serialization.SerializableDictionary<string, Script.DataSource> {
                            Dictionary = new Dictionary<string, Script.DataSource> {
                                { "result", new Script.DataSource { VariableName = "output "} }
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
