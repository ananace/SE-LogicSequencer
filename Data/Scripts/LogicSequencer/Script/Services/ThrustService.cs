using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI;

namespace LogicSequencer.Script.Services
{
    public class ThrustOffService : ScriptService
    {
        public override string ID => "thrust.turn_off";
        public override string Name => "Thrust - Turn off";
        public override string Description => "Turn off any thrust blocks in the block list";

        public override bool CanApplyTo(IEnumerable<IMyTerminalBlock> blocks)
        {
            return blocks.OfType<IMyThrust>().Any();
        }
        public override void Apply(IEnumerable<IMyTerminalBlock> blocks, Dictionary<string, ScriptValue> parameters)
        {
            foreach (var thrust in blocks.OfType<IMyThrust>())
                thrust.Enabled = false;
        }
    }

    public class ThrustOnService : ScriptService
    {
        public override string ID => "thrust.turn_on";
        public override string Name => "Thrust - Turn on";
        public override string Description => "Turn on any thrust blocks in the block list";

        public override bool CanApplyTo(IEnumerable<IMyTerminalBlock> blocks)
        {
            return blocks.OfType<IMyThrust>().Any();
        }
        public override void Apply(IEnumerable<IMyTerminalBlock> blocks, Dictionary<string, ScriptValue> parameters)
        {
            foreach (var thrust in blocks.OfType<IMyThrust>())
                thrust.Enabled = false;
        }
    }

    public class ThrustOnOffService : ScriptService
    {
        public override string ID => "thrust.toggle";
        public override string Name => "Thrust - Toggle on/off";
        public override string Description => "Toggle on/off any thrust blocks in the block list";

        public override bool CanApplyTo(IEnumerable<IMyTerminalBlock> blocks)
        {
            return blocks.OfType<IMyThrust>().Any();
        }
        public override void Apply(IEnumerable<IMyTerminalBlock> blocks, Dictionary<string, ScriptValue> parameters)
        {
            foreach (var thrust in blocks.OfType<IMyThrust>())
                thrust.Enabled = !thrust.Enabled;
        }
    }

    public class ThrustOverrideService : ScriptService
    {
        public override string ID => "thrust.override";
        public override string Name => "Thrust - Set override";
        public override string Description => "Set the overried of any thrust blocks in the block list";

        public override IEnumerable<Parameter> AvailableParameters { get {
            yield return new Parameter { Name = "Override", Description = "Override in newtons", Type = VariableType.Real };
            yield return new Parameter { Name = "OverridePercent", Description = "Override in percent (0.0 - 1.0)", Type = VariableType.Real };
        } }

        public override bool CanApplyTo(IEnumerable<IMyTerminalBlock> blocks)
        {
            return blocks.OfType<IMyThrust>().Any();
        }
        public override void Apply(IEnumerable<IMyTerminalBlock> blocks, Dictionary<string, ScriptValue> parameters)
        {
            if (!(parameters.ContainsKey("Override") || parameters.ContainsKey("OverridePercent")))
                throw new ArgumentException("Must provide either Override or OverridePercent");

            foreach (var thrust in blocks.OfType<IMyThrust>())
            {
                if (parameters.ContainsKey("Override"))
                    thrust.ThrustOverride = (float)parameters["Override"].Real;
                else if (parameters.ContainsKey("OverridePercent"))
                    thrust.ThrustOverridePercentage = (float)parameters["OverridePercent"].Real;
            }
        }
    }
}
