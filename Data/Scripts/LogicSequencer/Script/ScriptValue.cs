using System;
using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script
{
    [ProtoContract(UseProtoMembersOnly = true)]
    public class ScriptValue
    {
        [ProtoMember(1, IsRequired = false)]
        bool? _Boolean { get; set; }
        public bool Boolean { get { return _Boolean.Value; } set { Reset(); _Boolean = value; } }
        [XmlIgnore]
        public bool BooleanSpecified => _Boolean.HasValue;

        [ProtoMember(2, IsRequired = false)]
        long? _Integer { get; set; }
        public long Integer { get { return _Integer.Value; } set { Reset(); _Integer = value; } }
        [XmlIgnore]
        public bool IntegerSpecified => _Integer.HasValue;

        [ProtoMember(3, IsRequired = false)]
        double? _Real { get; set; }
        public double Real { get { return _Real.Value; } set { Reset(); _Real = value; } }
        [XmlIgnore]
        public bool RealSpecified => _Real.HasValue;

        [ProtoMember(4, IsRequired = false)]
        public string String { get; set; }
        [XmlIgnore]
        public bool StringSpecified => String != null;

        void Reset()
        {
            _Boolean = null;
            _Integer = null;
            _Real = null;
            String = null;
        }

        public override string ToString()
        {
            return $"[{TypeName} => {AsObject}]";
        }

        [XmlIgnore]
        public VariableType TypeEnum { get {
            if (BooleanSpecified)
                return VariableType.Boolean;
            if (IntegerSpecified)
                return VariableType.Integer;
            if (RealSpecified)
                return VariableType.Real;
            if (StringSpecified)
                return VariableType.String;
            throw new ArgumentException("No type for an undefined value");
        } }
        [XmlIgnore]
        public string TypeName { get {
            if (BooleanSpecified)
                return "Boolean";
            if (IntegerSpecified)
                return "Integer";
            if (RealSpecified)
                return "Real";
            if (StringSpecified)
                return "String";
            return "Null";
        } }
        [XmlIgnore]
        public object AsObject { get {
            if (BooleanSpecified)
                return Boolean;
            if (IntegerSpecified)
                return Integer;
            if (RealSpecified)
                return Real;
            if (StringSpecified)
                return String;
            return null;
        } set {
            if (value is bool)
                Boolean = (bool)value;
            else if (value is long)
                Integer = (long)value;
            else if (value is double)
                Real = (double)value;
            else if (value is string)
                String = (string)value;
            throw new ArgumentException($"Unable to handle {value.GetType().Name} as a script value", "value");
        } }
    }
}
