using System;
using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script
{
    [ProtoInclude(100, typeof(Triggers.Action))]
    [ProtoInclude(101, typeof(Triggers.External))]
    [ProtoInclude(102, typeof(Triggers.IGC))]
    [ProtoInclude(103, typeof(Triggers.Sun))]
    [ProtoInclude(104, typeof(Triggers.Time))]
    [ProtoContract]
    [XmlInclude(typeof(Triggers.Action))]
    [XmlInclude(typeof(Triggers.External))]
    [XmlInclude(typeof(Triggers.IGC))]
    [XmlInclude(typeof(Triggers.Sun))]
    [XmlInclude(typeof(Triggers.Time))]
    // Abstract class instead of interface to make XML serialization possible
    public abstract class ScriptTrigger : IScriptPiece, IEquatable<ScriptTrigger>
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        public abstract bool IsValid { get; }
        public virtual bool Equals(ScriptTrigger other)
        {
            return GetType() == other.GetType() && Name == other.Name;
        }
    }
}
