using System;
using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class ArithmeticComplexPart
    {
        [ProtoContract(UseProtoMembersOnly = true)]
        public class Part
        {
            [ProtoMember(1)]
            public DataSource DataSource { get; set; }
            [ProtoMember(2)]
            public ArithmeticComplexPart Arithmetic { get; set; }

            public bool IsData => DataSource != null;
            public bool IsArithmetic => Arithmetic != null;

            /*
                if (!IsData && !IsArithmetic)
                    return false;
                if (IsData && IsArithmetic)
                    return false;
                if (IsData && !DataSource.IsValid)
                    return false;
                if (IsArithmetic && !Arithmetic.IsValid)
                    return false;
                return true;
            */
            public bool IsValid => (IsData || IsArithmetic) && !(IsData && IsArithmetic) && (!IsData || DataSource.IsValid) && (!IsArithmetic || Arithmetic.IsValid);
        }

        // Serialize operators as string to avoid changes if the operator list is updated
        [ProtoMember(1, IsRequired = false)]
        public string Operator { get; set; }
        [XmlIgnore]
        public Helper.MathHelper.OperationType OperatorType => (Helper.MathHelper.OperationType)Enum.Parse(typeof(Helper.MathHelper.OperationType), Operator, true);
        [ProtoMember(2, IsRequired = false)]
        public string SingleOperator { get; set; }
        [XmlIgnore]
        public Helper.MathHelper.SingleObjectOperationType SingleOperatorType => (Helper.MathHelper.SingleObjectOperationType)Enum.Parse(typeof(Helper.MathHelper.SingleObjectOperationType), SingleOperator, true);

        [ProtoMember(3)]
        public Part LHS { get; set; }
        [ProtoMember(4)]
        public Part RHS { get; set; }

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

    [ProtoContract]
    public class ArithmeticComplex : ScriptAction
    {
        [ProtoMember(1)]
        public string TargetVariable { get; set; }

        [ProtoMember(2)]
        public ArithmeticComplexPart Part { get; set; }

        public override bool IsValid => !string.IsNullOrEmpty(TargetVariable) && Part.IsValid;
    }
}
