using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script
{
    [ProtoInclude(100, typeof(Actions.ArithmeticComplex))]
    [ProtoInclude(101, typeof(Actions.ArithmeticSimple))]
    [ProtoInclude(102, typeof(Actions.ArithmeticSimpleSingle))]
    [ProtoInclude(103, typeof(Actions.BlockGetProperty))]
    [ProtoInclude(104, typeof(Actions.BlockGetState))]
    [ProtoInclude(105, typeof(Actions.BlockRunAction))]
    [ProtoInclude(106, typeof(Actions.BlockSetProperty))]
    [ProtoInclude(107, typeof(Actions.Choose))]
    [ProtoInclude(108, typeof(Actions.ConditionAction))]
    [ProtoInclude(109, typeof(Actions.Delay))]
    [ProtoInclude(110, typeof(Actions.RepeatTimes))]
    [ProtoInclude(111, typeof(Actions.RepeatUntil))]
    [ProtoInclude(112, typeof(Actions.RepeatWhile))]
    [ProtoInclude(113, typeof(Actions.SetVariables))]
    [ProtoInclude(114, typeof(Actions.StorePermanentVariables))]
    [ProtoInclude(115, typeof(Actions.WaitTrigger))]
    [ProtoContract]
    [XmlInclude(typeof(Actions.ArithmeticComplex))]
    [XmlInclude(typeof(Actions.ArithmeticSimple))]
    [XmlInclude(typeof(Actions.ArithmeticSimpleSingle))]
    [XmlInclude(typeof(Actions.BlockGetProperty))]
    [XmlInclude(typeof(Actions.BlockGetState))]
    [XmlInclude(typeof(Actions.BlockRunAction))]
    [XmlInclude(typeof(Actions.BlockSetProperty))]
    [XmlInclude(typeof(Actions.Choose))]
    [XmlInclude(typeof(Actions.ConditionAction))]
    [XmlInclude(typeof(Actions.Delay))]
    [XmlInclude(typeof(Actions.RepeatTimes))]
    [XmlInclude(typeof(Actions.RepeatUntil))]
    [XmlInclude(typeof(Actions.RepeatWhile))]
    [XmlInclude(typeof(Actions.SetVariables))]
    [XmlInclude(typeof(Actions.StorePermanentVariables))]
    [XmlInclude(typeof(Actions.WaitTrigger))]
    // Abstract class instead of interface to make XML serialization possible
    public abstract class ScriptAction : IScriptPiece
    {
        public abstract bool IsValid { get; }
    }
}
