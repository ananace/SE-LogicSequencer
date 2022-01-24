using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;

namespace LogicSequencer
{
    partial class Session
    {
        readonly List<IMyTerminalControl> SequencerControls = new List<IMyTerminalControl>();

        void CustomControlGetter(IMyTerminalBlock block, List<IMyTerminalControl> controls)
        {
            Util.Log.Debug("Checking custom controls...");
            if (!block.BlockDefinition.SubtypeName.StartsWith("LogicSequencer"))
                return;

            PrepareCustomControls();

            Util.Log.Debug("Adding custom controls.");
            foreach (var control in SequencerControls)
            {
                controls.Add(control);
            }

            // foreach (var control in SequenceEditControls)
            //   controls.Add(control);
        }

        bool ControlsLoaded = false;

        const string PROPERTY_PREFIX = "LogicSequencer.";
        const string CONTROL_PREFIX = "LogicSequencer.Terminal.";
        void PrepareCustomControls()
        {
            if (ControlsLoaded)
                return;

            Util.Log.Info("Preparing controls...");

            Func<IMyTerminalBlock, bool> isSequencer = (block) => block?.GameLogic?.GetAs<Blocks.LogicSequencer>() != null;

            ControlsLoaded = true;

            var TC = MyAPIGateway.TerminalControls;

            {
                var control = TC.CreateControl<IMyTerminalControlLabel, IMyUpgradeModule>(String.Empty);
                control.Label = VRage.Utils.MyStringId.GetOrCompute("Logic Sequencer");
                control.Enabled = isSequencer;
                control.Visible = isSequencer;
                // TC.AddControl<IMyUpgradeModule>(control);
                SequencerControls.Add(control);
            }

            {
                var control = TC.CreateControl<IMyTerminalControlSeparator, IMyUpgradeModule>(String.Empty);
                control.Enabled = isSequencer;
                control.Visible = isSequencer;
                // TC.AddControl<IMyUpgradeModule>(control);
                SequencerControls.Add(control);
            }

            {
                var control = TC.CreateControl<IMyTerminalControlCombobox, IMyUpgradeModule>(CONTROL_PREFIX+"ProgramStartMode");
                control.Title = VRage.Utils.MyStringId.GetOrCompute("Start Mode");
                control.Tooltip = VRage.Utils.MyStringId.GetOrCompute("How the sequencer should handle multiple attempts to run at the same time.");
                control.SupportsMultipleBlocks = true;
                control.Enabled = isSequencer;
                control.Visible = isSequencer;
                control.ComboBoxContent = (list) => {
                    list.Add(new VRage.ModAPI.MyTerminalControlComboBoxItem { Key = (long)ProgramStartMode.Single, Value = VRage.Utils.MyStringId.GetOrCompute("Single (ignore additional)") });
                    list.Add(new VRage.ModAPI.MyTerminalControlComboBoxItem { Key = (long)ProgramStartMode.Restart, Value = VRage.Utils.MyStringId.GetOrCompute("Restart") });
                    list.Add(new VRage.ModAPI.MyTerminalControlComboBoxItem { Key = (long)ProgramStartMode.Queue, Value = VRage.Utils.MyStringId.GetOrCompute("Queue") });
                    list.Add(new VRage.ModAPI.MyTerminalControlComboBoxItem { Key = (long)ProgramStartMode.Multiple, Value = VRage.Utils.MyStringId.GetOrCompute("Multiple") });
                };
                control.Getter = (block) => {
                    var logic = block?.GameLogic as Blocks.LogicSequencer;
                    if (logic == null)
                        return 0;
                    return (long)logic.StartMode;
                };
                control.Setter = (block, value) => {
                    var logic = block?.GameLogic as Blocks.LogicSequencer;
                    if (logic == null)
                        return;
                    logic.StartMode = (ProgramStartMode)value;
                };
                // TC.AddControl<IMyUpgradeModule>(control);
                SequencerControls.Add(control);

                var property = TC.CreateProperty<string, IMyUpgradeModule>(PROPERTY_PREFIX+"ProgramStartMode");
                property.Enabled = control.Enabled;
                property.Visible = control.Visible;
                property.SupportsMultipleBlocks = control.SupportsMultipleBlocks;
                property.Getter = (block) => Enum.GetName(typeof(ProgramStartMode), (block?.GameLogic as Blocks.LogicSequencer)?.StartMode ?? ProgramStartMode.Single);
                property.Setter = (block, value) => {
                    if (block?.GameLogic as Blocks.LogicSequencer == null)
                        return;

                    ProgramStartMode mode = ProgramStartMode.Single;
                    if (Enum.TryParse(value, true, out mode))
                        (block.GameLogic as Blocks.LogicSequencer).StartMode = mode;
                };
                TC.AddControl<IMyUpgradeModule>(property);
            }

            {
                var control = TC.CreateControl<IMyTerminalControlTextbox, IMyUpgradeModule>(CONTROL_PREFIX+"MaxRuns");
                control.Title = VRage.Utils.MyStringId.GetOrCompute("Max Runs");
                control.Tooltip = VRage.Utils.MyStringId.GetOrCompute("The maximum number of active runs. (Either in the queue or actively executing)");
                control.SupportsMultipleBlocks = true;
                control.Enabled = isSequencer;
                control.Visible = (block) => {
                    var logic = block?.GameLogic as Blocks.LogicSequencer;
                    if (logic == null)
                        return false;

                    return logic.StartMode == ProgramStartMode.Queue || logic.StartMode == ProgramStartMode.Restart;
                };
                control.Getter = (block) => {
                    var logic = block?.GameLogic as Blocks.LogicSequencer;
                    StringBuilder sb = new StringBuilder(logic?.MaxRuns.ToString());
                    return sb;
                };
                control.Setter = (block, sb) => {
                    var logic = block?.GameLogic as Blocks.LogicSequencer;
                    if (logic == null)
                        return;

                    int max = 0;
                    if (int.TryParse(sb.ToString(), out max))
                        logic.MaxRuns = max;
                };
                // TC.AddControl<IMyUpgradeModule>(control);
                SequencerControls.Add(control);

                var property = TC.CreateProperty<int, IMyUpgradeModule>(PROPERTY_PREFIX+"MaxRuns");
                property.Enabled = control.Enabled;
                property.Visible = control.Visible;
                property.SupportsMultipleBlocks = control.SupportsMultipleBlocks;
                property.Getter = (block) => (block?.GameLogic as Blocks.LogicSequencer)?.MaxRuns ?? 0;
                property.Setter = (block, value) => {
                    var logic = block?.GameLogic as Blocks.LogicSequencer;
                    if (logic == null)
                        return;

                    logic.MaxRuns = value;
                };
                TC.AddControl<IMyUpgradeModule>(property);
            }

            {
                var control = TC.CreateControl<IMyTerminalControlButton, IMyUpgradeModule>(CONTROL_PREFIX+"Run");
                control.Title = VRage.Utils.MyStringId.GetOrCompute("Run");
                control.Tooltip = VRage.Utils.MyStringId.GetOrCompute("Manually start a new execution of the contained logic sequence");
                control.Enabled = isSequencer;
                control.Visible = isSequencer;
                control.SupportsMultipleBlocks = true;
                control.Action = (block) => {
                    var logic = block?.GameLogic as Blocks.LogicSequencer;
                    logic?.SequenceStart();
                };
                // TC.AddControl<IMyUpgradeModule>(control);
                SequencerControls.Add(control);

                var action = TC.CreateAction<IMyUpgradeModule>(CONTROL_PREFIX+"RunAction");
                action.Enabled = control.Enabled;
                action.ValidForGroups = control.SupportsMultipleBlocks;
                action.Name = new StringBuilder("Run");
                action.Icon = @"Textures\GUI\Icons\Actions\SwitchOn.dds";
                action.Action = (block) => {
                    var logic = block?.GameLogic as Blocks.LogicSequencer;
                    logic?.DoTrigger(new Script.Triggers.Action());
                };
                TC.AddAction<IMyUpgradeModule>(action);
            }

            {
                var control = TC.CreateControl<IMyTerminalControlSeparator, IMyUpgradeModule>(String.Empty);
                control.Enabled = isSequencer;
                control.Visible = isSequencer;
                // TC.AddControl<IMyUpgradeModule>(control);
                SequencerControls.Add(control);
            }

            Script.ScriptSequence selectedScript = null;
            {
                var control = TC.CreateControl<IMyTerminalControlListbox, IMyUpgradeModule>(CONTROL_PREFIX+"ScriptList");
                control.Title = VRage.Utils.MyStringId.GetOrCompute("Script");
                control.Enabled = isSequencer;
                control.Visible = isSequencer;
                control.ListContent = (block, list, selected) => {
                    foreach (var script in AvailableScripts())
                        list.Add(new VRage.ModAPI.MyTerminalControlListBoxItem(VRage.Utils.MyStringId.GetOrCompute(script.Name), VRage.Utils.MyStringId.GetOrCompute(script.Description), script));
                };
                control.ItemSelected = (block, selected) => {
                    selectedScript = selected.FirstOrDefault().UserData as Script.ScriptSequence;
                };
                // TC.AddControl<IMyUpgradeModule>(control);
                SequencerControls.Add(control);
            }

            {
                var control = TC.CreateControl<IMyTerminalControlButton, IMyUpgradeModule>(CONTROL_PREFIX+"ScriptLoad");
                control.Title = VRage.Utils.MyStringId.GetOrCompute("Load");
                control.Tooltip = VRage.Utils.MyStringId.GetOrCompute("Load the selected script into the sequencer");
                control.Enabled = isSequencer;
                control.Visible = isSequencer;
                control.Action = (block) => {
                    var logic = block?.GameLogic?.GetAs<Blocks.LogicSequencer>();
                    if (logic == null)
                        return;

                    if (selectedScript == null)
                        return;

                    logic.Script = selectedScript;
                };
                // TC.AddControl<IMyUpgradeModule>(control);
                SequencerControls.Add(control);
            }

            {
                var control = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSeparator, IMyUpgradeModule>(CONTROL_PREFIX+"ScriptSeparator");
                control.Enabled = isSequencer;
                control.Visible = isSequencer;
                // TC.AddControl<IMyUpgradeModule>(control);
                SequencerControls.Add(control);
            }

            {
                var control = TC.CreateControl<IMyTerminalControlButton, IMyUpgradeModule>(CONTROL_PREFIX+"ClearScript");
                control.Title = VRage.Utils.MyStringId.GetOrCompute("Clear");
                control.Tooltip = VRage.Utils.MyStringId.GetOrCompute("Clear the currently loaded script");
                control.Enabled = isSequencer;
                control.Visible = isSequencer;
                control.Action = (block) => {
                    var logic = block?.GameLogic?.GetAs<Blocks.LogicSequencer>();
                    if (logic == null)
                        return;

                    logic.Script = null;
                };
                // TC.AddControl<IMyUpgradeModule>(control);
                SequencerControls.Add(control);
            }

            {
                var control = TC.CreateControl<IMyTerminalControlButton, IMyUpgradeModule>(CONTROL_PREFIX+"EditScript");
                control.Title = VRage.Utils.MyStringId.GetOrCompute("Edit");
                control.Tooltip = VRage.Utils.MyStringId.GetOrCompute("Edit the currently loaded script");
                control.Enabled = isSequencer;
                control.Visible = isSequencer;
                control.Action = (block) => {
                    var logic = block?.GameLogic?.GetAs<Blocks.LogicSequencer>();
                    if (logic == null)
                        return;

                    ShowEditor(logic);
                };
                // TC.AddControl<IMyUpgradeModule>(control);
                SequencerControls.Add(control);
            }
        }
    }
}
