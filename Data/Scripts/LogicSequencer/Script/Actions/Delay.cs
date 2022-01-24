using System;
using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class Delay : ScriptAction
    {
        [ProtoMember(1)]
        public TimeSpan Time { get; set; }

        public override bool IsValid => Time.TotalMilliseconds >= 0;
    }
}
