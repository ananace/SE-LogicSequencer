using System;
using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script
{
    [ProtoContract]
    public class ArithmeticComponent
    {
        // Serialize operators as string to avoid issues if the operator list is updated
        // As a bonus also makes more sense in XML
        [ProtoMember(1, IsRequired = false)]
        public string Operator { get; set; }
        [XmlIgnore]
        public Helper.MathHelper.OperationType OperatorType => (Helper.MathHelper.OperationType)Enum.Parse(typeof(Helper.MathHelper.OperationType), Operator, true);
        [ProtoMember(2, IsRequired = false)]
        public string SingleOperator { get; set; }
        [XmlIgnore]
        public Helper.MathHelper.SingleObjectOperationType SingleOperatorType => (Helper.MathHelper.SingleObjectOperationType)Enum.Parse(typeof(Helper.MathHelper.SingleObjectOperationType), SingleOperator, true);

        [ProtoMember(3)]
        public DataSource LHS { get; set; }
        [ProtoMember(4)]
        public DataSource RHS { get; set; }

        [XmlIgnore]
        public bool IsSingle => SingleOperator != null;
        [XmlIgnore]
        public bool IsDouble => Operator != null;

        public virtual bool IsValid { get {
            if (IsSingle)
            {
                Helper.MathHelper.SingleObjectOperationType op;
                return Operator == null && SingleOperator != null && !SingleOperator.StartsWith("_") && Enum.TryParse(SingleOperator, true, out op) && LHS == null && RHS != null && RHS.IsValid;
            }
            else
            {
                Helper.MathHelper.OperationType op;
                return SingleOperator == null && Operator != null && !Operator.StartsWith("_") && Enum.TryParse(Operator, true, out op) && LHS != null && LHS.IsValid && RHS != null && RHS.IsValid;
            }
        } }
    }
}
