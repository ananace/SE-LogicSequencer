using System;
using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script
{
    [ProtoInclude(100, typeof(Triggers.Action))]
    [ProtoInclude(101, typeof(Triggers.BlockState))]
    [ProtoInclude(102, typeof(Triggers.External))]
    [ProtoInclude(103, typeof(Triggers.GridChange))]
    [ProtoInclude(104, typeof(Triggers.IGC))]
    [ProtoInclude(105, typeof(Triggers.Sun))]
    [ProtoInclude(106, typeof(Triggers.Time))]
    [ProtoContract]
    [XmlInclude(typeof(Triggers.Action))]
    [XmlInclude(typeof(Triggers.BlockState))]
    [XmlInclude(typeof(Triggers.External))]
    [XmlInclude(typeof(Triggers.GridChange))]
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
