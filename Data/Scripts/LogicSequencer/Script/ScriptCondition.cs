using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script
{
    [ProtoInclude(100, typeof(Conditions.And))]
    [ProtoInclude(101, typeof(Conditions.BlockPropertyIs))]
    [ProtoInclude(102, typeof(Conditions.Comparison))]
    [ProtoInclude(103, typeof(Conditions.HasVariable))]
    [ProtoInclude(104, typeof(Conditions.Not))]
    [ProtoInclude(105, typeof(Conditions.Or))]
    [ProtoInclude(106, typeof(Conditions.VariableIsTruthy))]
    [ProtoInclude(107, typeof(Conditions.VariableIsType))]
    [ProtoContract]
    [XmlInclude(typeof(Conditions.And))]
    [XmlInclude(typeof(Conditions.BlockPropertyIs))]
    [XmlInclude(typeof(Conditions.Comparison))]
    [XmlInclude(typeof(Conditions.HasVariable))]
    [XmlInclude(typeof(Conditions.Not))]
    [XmlInclude(typeof(Conditions.Or))]
    [XmlInclude(typeof(Conditions.VariableIsTruthy))]
    [XmlInclude(typeof(Conditions.VariableIsType))]
    // Abstract class instead of interface to make XML serialization possible
    public abstract class ScriptCondition : IScriptPiece
    {
        public abstract bool IsValid { get; }
    }
}
