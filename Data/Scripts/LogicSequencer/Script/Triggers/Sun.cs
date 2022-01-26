using System;
using ProtoBuf;

namespace LogicSequencer.Script.Triggers
{
    [ProtoContract]
    public class Sun : ScriptTrigger
    {
        public enum SunState
        {
            Sunrise,
            Sunset
        }

        [ProtoMember(2)]
        public SunState Event { get; set; }
        [ProtoMember(3, IsRequired = false)]
        public TimeSpan? Offset { get; set; } = null;

        public override bool IsValid => true;
    }
}
