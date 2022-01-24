using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script
{
    [ProtoContract]
    public class BlockSelector
    {
        [ProtoMember(1, IsRequired = false)]
        public long? ID { get; set; }
        [ProtoMember(2, IsRequired = false)]
        public string Name { get; set; }

        [XmlIgnore]
        public bool IDSpecified => ID.HasValue;

        public bool IsValid => IDSpecified || !string.IsNullOrEmpty(Name);
    }
}
