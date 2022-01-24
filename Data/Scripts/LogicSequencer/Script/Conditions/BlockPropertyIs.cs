using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script.Conditions
{
    [ProtoContract]
    public class BlockPropertyIs : ScriptCondition
    {
        [ProtoMember(1)]
        public long BlockID { get; set; }

        [ProtoMember(2)]
        public string Property { get; set; }
        [ProtoMember(3, IsRequired = false)]
        public VariableType? Type { get; set; } = null;
        [ProtoMember(4)]
        public DataSource ToCompare { get; set; }
        [ProtoMember(5, IsRequired = false)]
        public string StoreInVariable { get; set; }
        [XmlIgnore]
        public bool TypeSpecified => Type.HasValue;

        public override bool IsValid => BlockID != 0 && !string.IsNullOrEmpty(Property) && !string.IsNullOrEmpty(StoreInVariable);
    }
}
