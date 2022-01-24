using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script.Conditions
{
    [ProtoContract]
    public class HasVariable : ScriptCondition
    {
        [ProtoMember(1)]
        public string Variable { get; set; }
        [ProtoMember(2, IsRequired = false)]
        public VariableType? OfType { get; set; } = null;
        [XmlIgnore]
        public bool OfTypeSpecified => OfType.HasValue;

        public override bool IsValid => !string.IsNullOrEmpty(Variable);
    }
}
