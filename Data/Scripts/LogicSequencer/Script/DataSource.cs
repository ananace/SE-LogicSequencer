using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script
{
    [ProtoContract]
    public class DataSource
    {
        [ProtoMember(1)]
        public string VariableName { get; set; }
        [ProtoMember(2)]
        public ScriptValue Value { get; set; }
        [ProtoMember(3)]
        public ArithmeticComponent Arithmetic { get; set; }

        [XmlIgnore]
        public bool HasVariable => !string.IsNullOrEmpty(VariableName);
        [XmlIgnore]
        public bool HasValue => Value != null;
        [XmlIgnore]
        public bool HasArithmetic => Arithmetic != null;

        public bool IsValid => HasVariable || HasValue || HasArithmetic;
    }
}
