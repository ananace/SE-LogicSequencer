using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI;

namespace LogicSequencer.Script.Services
{
    public class FunctionalOffService : ScriptService
    {
        public override string ID => "functional.turn_off";
        public override string Name => "Functional - Turn off";
        public override string Description => "Turn off all blocks in the block list";

        public override bool CanApplyTo(IEnumerable<IMyTerminalBlock> blocks)
        {
            return blocks.OfType<IMyFunctionalBlock>().Any();
        }
        public override void Apply(IEnumerable<IMyTerminalBlock> blocks, Dictionary<string, ScriptValue> parameters)
        {
            foreach (var functional in blocks.OfType<IMyFunctionalBlock>())
                functional.Enabled = false;
        }
    }

    public class FunctionalOnService : ScriptService
    {
        public override string ID => "functional.turn_on";
        public override string Name => "Functional - Turn on";
        public override string Description => "Turn on all blocks in the block list";

        public override bool CanApplyTo(IEnumerable<IMyTerminalBlock> blocks)
        {
            return blocks.OfType<IMyLightingBlock>().Any();
        }
        public override void Apply(IEnumerable<IMyTerminalBlock> blocks, Dictionary<string, ScriptValue> parameters)
        {
            foreach (var functional in blocks.OfType<IMyFunctionalBlock>())
                functional.Enabled = true;
        }
    }

    public class FunctionalOnOffService : ScriptService
    {
        public override string ID => "functional.toggle";
        public override string Name => "Functional - Toggle on/off";
        public override string Description => "Toggle on/off all blocks in the block list";

        public override bool CanApplyTo(IEnumerable<IMyTerminalBlock> blocks)
        {
            return blocks.OfType<IMyFunctionalBlock>().Any();
        }
        public override void Apply(IEnumerable<IMyTerminalBlock> blocks, Dictionary<string, ScriptValue> parameters)
        {
            foreach (var functional in blocks.OfType<IMyFunctionalBlock>())
                functional.Enabled = !functional.Enabled;
        }
    }
}
