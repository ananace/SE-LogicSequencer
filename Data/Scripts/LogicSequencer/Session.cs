using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RichHudFramework.Client;
using RichHudFramework.UI;
using RichHudFramework.UI.Client;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRageMath;

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
                            Operator = Script.Helper.MathHelper.OperationType.Divide
                        }
                    },
                    new Script.Actions.ConditionAction {
                        Condition = new Script.Conditions.Comparison {
                            SourceData = new Script.DataSource { VariableName = "output" },
                            ComparisonData = new Script.DataSource { Value = new Script.ScriptValue { Integer = 5 } },
                            Operation = Script.Helper.MathHelper.OperationType.CompareEqual
                        }
                    },
                    new Script.Actions.Delay {
                        Time = TimeSpan.FromSeconds(10)
                    },
                    new Script.Actions.ArithmeticSimpleSingle {
                        Operand = new Script.DataSource { VariableName = "output" },
                        SingleOperator = Script.Helper.MathHelper.SingleObjectOperationType.Negate,
                        TargetVariable = "output"
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

        IBindGroup editorBinds;
        void HudInit()
        {
            SequenceEditor = new UI.SequenceEditor(HudMain.HighDpiRoot) {
                Visible = false
            };

            editorBinds = BindManager.GetOrCreateGroup("editorBinds");
            editorBinds.RegisterBinds(new BindGroupInitializer()
            {
                { "editorToggle", VRage.Input.MyKeys.Home }
            });

            editorBinds[0].NewPressed += () => {
                SequenceEditor.Visible = !SequenceEditor.Visible;
            };
        }

        void ClientReset()
        {

        }

        void TestSerialize()
        {
            var variables = new VRage.Serialization.SerializableDictionary<string, Script.ScriptValue>();
            variables["value"] = new Script.ScriptValue { Integer = 2 };

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

                    new Script.Conditions.Or {
                        Conditions = new List<Script.ScriptCondition> {
                            new Script.Conditions.HasVariable {
                                Variable = "potatis"
                            },
                            new Script.Conditions.Comparison {
                                SourceData = new Script.DataSource { Value = new Script.ScriptValue { Integer = 5 } },
                                ComparisonData =  new Script.DataSource { Value = new Script.ScriptValue { Real = 4 } },
                                Operation = Script.Helper.MathHelper.OperationType.CompareLesserEqual
                            }
                        }
                    }
                },
                Actions = new List<Script.ScriptAction> {
                    new Script.Actions.ArithmeticComplex { },
                    new Script.Actions.ArithmeticSimple { },
                    new Script.Actions.ArithmeticSimpleSingle { },
                    new Script.Actions.BlockGetProperty { },
                    new Script.Actions.BlockRunAction { },
                    new Script.Actions.BlockSetProperty { },
                    new Script.Actions.Choose { },
                    new Script.Actions.ConditionAction { },
                    new Script.Actions.Delay { },
                    new Script.Actions.RepeatTimes { },
                    new Script.Actions.RepeatUntil { },
                    new Script.Actions.RepeatWhile { },
                    new Script.Actions.SetVariables { },
                    new Script.Actions.WaitTrigger { },

                    new Script.Actions.ArithmeticComplex {
                        Part = new Script.Actions.ArithmeticComplexPart {
                            LHS = new Script.Actions.ArithmeticComplexPart.Part {
                                DataSource = new Script.DataSource {
                                    VariableName = "value"
                                }
                            },
                            RHS = new Script.Actions.ArithmeticComplexPart.Part {
                                Arithmetic = new Script.Actions.ArithmeticComplexPart {
                                    LHS = new Script.Actions.ArithmeticComplexPart.Part {
                                        DataSource = new Script.DataSource {
                                            Value = new Script.ScriptValue {
                                                Integer = 5
                                            }
                                        }
                                    },
                                    RHS = new Script.Actions.ArithmeticComplexPart.Part {
                                        DataSource = new Script.DataSource {
                                            Value = new Script.ScriptValue {
                                                Real = 2
                                            }
                                        }
                                    },
                                    Operator = Script.Helper.MathHelper.OperationType.Divide
                                }
                            },
                            Operator = Script.Helper.MathHelper.OperationType.Multiply,
                        },
                        TargetVariable = "value2"
                    }
                }
            };

            var serialized = Convert.ToBase64String(MyAPIGateway.Utilities.SerializeToBinary(testScript));
            var unserialized = MyAPIGateway.Utilities.SerializeFromBinary<Script.ScriptSequence>(Convert.FromBase64String(serialized));

            Util.Log.Debug(MyAPIGateway.Utilities.SerializeToXML(testScript));
            Util.Log.Debug(MyAPIGateway.Utilities.SerializeToXML(unserialized));
        }
    }
}
