using System;
using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script.Conditions
{
    [ProtoContract]
    public class Comparison : ScriptCondition
    {
        [ProtoMember(1)]
        public DataSource SourceData { get; set; }
        [ProtoMember(2)]
        public DataSource ComparisonData { get; set; }
        [ProtoMember(3)]
        public string Operation { get; set; }

        [XmlIgnore]
        public Helper.MathHelper.OperationType OperationType => (Helper.MathHelper.OperationType)Enum.Parse(typeof(Helper.MathHelper.OperationType), Operation, true);

        public override bool IsValid { get {
            Helper.MathHelper.OperationType op;
            return Operation != null && !Operation.StartsWith("_") && Enum.TryParse(Operation, true, out op) &&
                SourceData != null && ComparisonData != null &&
                Helper.MathHelper.OperationType._ComparisonStart < op && op < Helper.MathHelper.OperationType._ComparisonEnd;
        }}
    }
}
