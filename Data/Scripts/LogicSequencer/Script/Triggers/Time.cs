using System;
using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script.Triggers
{
    [ProtoContract]
    public class Time : ScriptTrigger
    {
        [ProtoMember(2)]
        public TimeSpan Every { get; set; }

        [XmlIgnore]
        [ProtoMember(3)]
        public long? MSRemaining { get; set; }

        [XmlIgnore]
        public bool MSRemainingSpecified => MSRemaining.HasValue;

        public override bool IsValid => Every.TotalSeconds >= 1;
        public override bool Equals(ScriptTrigger other)
        {
            return base.Equals(other) && Every == (other as Time).Every;
        }
    }
}
