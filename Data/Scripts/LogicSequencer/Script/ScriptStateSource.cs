using System.Collections.Generic;
using System.Xml.Serialization;
using ProtoBuf;
using Sandbox.ModAPI;

namespace LogicSequencer.Script
{
    public abstract class ScriptStateSource
    {
        public abstract string ID { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract VariableType ResultType { get; }
        public virtual bool SelfOnly { get { return false; } }

        public virtual bool IsEnabled { get; protected set; }
        public virtual bool IsVisible { get; protected set; }
        public virtual bool IsValid { get { return true; } }

        public abstract bool CanReadFrom(IMyTerminalBlock block);
        public abstract ScriptValue Read(IMyTerminalBlock block);
    }
}
