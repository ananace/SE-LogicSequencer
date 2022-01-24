using System.Collections.Generic;
using System.Xml.Serialization;
using ProtoBuf;
using Sandbox.ModAPI;

namespace LogicSequencer.Script
{
    public abstract class ScriptService
    {
        public class Parameter
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public VariableType Type { get; set; }

            public bool IsRequired { get; set; } = false;
            public ScriptValue DefaultValue { get; set; } = null;
        }

        public abstract string ID { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }

        public virtual bool IsEnabled { get; protected set; }
        public virtual bool IsVisible { get; protected set; }
        public virtual bool IsValid { get { return true; } }

        public virtual bool SupportsSingleBlock { get; protected set; } = true;
        public virtual bool SupportsMultiBlock { get; protected set; } = true;

        public virtual IEnumerable<Parameter> AvailableParameters { get { return new Parameter[0]; } }

        public abstract bool CanApplyTo(IEnumerable<IMyTerminalBlock> blocks);
        public abstract void Apply(IEnumerable<IMyTerminalBlock> blocks, Dictionary<string, ScriptValue> parameters);
    }
}
