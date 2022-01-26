using System;
using ProtoBuf;

namespace LogicSequencer.Script.Triggers
{
    [ProtoContract]
    public class Time : ScriptTrigger
    {
        [ProtoMember(2)]
        public TimeSpan Every { get; set; }

        public override bool IsValid => Every.TotalSeconds >= 1;
    }
}
