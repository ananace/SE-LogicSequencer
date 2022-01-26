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
        public DataSource Name { get; set; }
        [ProtoMember(3, IsRequired = false)]
        public bool? Self { get; set; }

        [XmlIgnore]
        public bool IDSpecified => ID.HasValue;
        [XmlIgnore]
        public bool SelfSpecified => Self.HasValue;

        public bool IsValid => IDSpecified || SelfSpecified || (Name != null && Name.IsValid);
    }
}
