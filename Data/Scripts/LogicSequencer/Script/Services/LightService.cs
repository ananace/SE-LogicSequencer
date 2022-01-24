using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI;

namespace LogicSequencer.Script.Services
{
    public class LightOffService : ScriptService
    {
        public override string ID => "light.turn_off";
        public override string Name => "Light - Turn off";
        public override string Description => "Turn off any lights in the block list";

        public override bool CanApplyTo(IEnumerable<IMyTerminalBlock> blocks)
        {
            return blocks.OfType<IMyLightingBlock>().Any();
        }
        public override void Apply(IEnumerable<IMyTerminalBlock> blocks, Dictionary<string, ScriptValue> parameters)
        {
            foreach (var light in blocks.OfType<IMyLightingBlock>())
                light.Enabled = false;
        }
    }

    public class LightOnService : ScriptService
    {
        public override string ID => "light.turn_on";
        public override string Name => "Light - Turn on";
        public override string Description => "Turn on any lights in the block list";

        public override bool CanApplyTo(IEnumerable<IMyTerminalBlock> blocks)
        {
            return blocks.OfType<IMyLightingBlock>().Any();
        }
        public override void Apply(IEnumerable<IMyTerminalBlock> blocks, Dictionary<string, ScriptValue> parameters)
        {
            foreach (var light in blocks.OfType<IMyLightingBlock>())
                light.Enabled = false;
        }
    }

    public class LightOnOffService : ScriptService
    {
        public override string ID => "light.toggle";
        public override string Name => "Light - Toggle on/off";
        public override string Description => "Toggle on/off any lights in the block list";

        public override bool CanApplyTo(IEnumerable<IMyTerminalBlock> blocks)
        {
            return blocks.OfType<IMyLightingBlock>().Any();
        }
        public override void Apply(IEnumerable<IMyTerminalBlock> blocks, Dictionary<string, ScriptValue> parameters)
        {
            foreach (var light in blocks.OfType<IMyLightingBlock>())
                light.Enabled = !light.Enabled;
        }
    }

    public class LightColorService : ScriptService
    {
        public override string ID => "light.color";
        public override string Name => "Light - Change color";
        public override string Description => "Change the color of any lights in the block list";

        public override IEnumerable<Parameter> AvailableParameters { get {
            yield return new Parameter { Name = "Red", Description = "Red colour", Type = VariableType.Integer, IsRequired = true };
            yield return new Parameter { Name = "Green", Description = "Green colour", Type = VariableType.Integer, IsRequired = true };
            yield return new Parameter { Name = "Blue", Description = "Blue colour", Type = VariableType.Integer, IsRequired = true };
        } }

        public override bool CanApplyTo(IEnumerable<IMyTerminalBlock> blocks)
        {
            return blocks.OfType<IMyLightingBlock>().Any();
        }
        public override void Apply(IEnumerable<IMyTerminalBlock> blocks, Dictionary<string, ScriptValue> parameters)
        {
            foreach (var light in blocks.OfType<IMyLightingBlock>())
                light.Color = new VRageMath.Color(parameters["Red"].Integer, parameters["Green"].Integer, parameters["Blue"].Integer);
        }
    }
}
