using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script
{
    [ProtoInclude(100, typeof(Actions.BlockGetProperty))]
    [ProtoInclude(101, typeof(Actions.BlockGetState))]
    [ProtoInclude(102, typeof(Actions.BlockRunAction))]
    [ProtoInclude(103, typeof(Actions.BlockSetProperty))]
    [ProtoInclude(104, typeof(Actions.CallService))]
    [ProtoInclude(105, typeof(Actions.Choose))]
    [ProtoInclude(106, typeof(Actions.ConditionAction))]
    [ProtoInclude(107, typeof(Actions.Delay))]
    [ProtoInclude(108, typeof(Actions.RepeatTimes))]
    [ProtoInclude(109, typeof(Actions.RepeatUntil))]
    [ProtoInclude(110, typeof(Actions.RepeatWhile))]
    [ProtoInclude(111, typeof(Actions.SetVariables))]
    [ProtoInclude(112, typeof(Actions.StorePermanentVariables))]
    [ProtoInclude(113, typeof(Actions.WaitTrigger))]
    [ProtoContract]
    [XmlInclude(typeof(Actions.BlockGetProperty))]
    [XmlInclude(typeof(Actions.BlockGetState))]
    [XmlInclude(typeof(Actions.BlockRunAction))]
    [XmlInclude(typeof(Actions.BlockSetProperty))]
    [XmlInclude(typeof(Actions.CallService))]
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
