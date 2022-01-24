using System;
using ProtoBuf;

namespace LogicSequencer.Script.Triggers
{
    [ProtoContract]
    public class Time : ScriptTrigger
    {
        [ProtoMember(1)]
        public DateTime Target { get; set; }

        public override bool IsValid => Target != null;
    }
}
