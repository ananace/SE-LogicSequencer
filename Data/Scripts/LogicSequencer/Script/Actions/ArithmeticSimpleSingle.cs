using System;
using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class ArithmeticSimpleSingle : ScriptAction
    {
        [ProtoMember(1)]
        public DataSource Operand { get; set; }
        [ProtoMember(2)]
        public string SingleOperator { get; set; }

        public Helper.MathHelper.SingleObjectOperationType SingleOperatorType => (Helper.MathHelper.SingleObjectOperationType)Enum.Parse(typeof(Helper.MathHelper.SingleObjectOperationType), SingleOperator, true);

        [ProtoMember(3)]
        public string TargetVariable { get; set; }

        public override bool IsValid { get {
            Helper.MathHelper.SingleObjectOperationType op;
            return SingleOperator != null && !SingleOperator.StartsWith("_") && Enum.TryParse(SingleOperator, true, out op) && Operand != null && !string.IsNullOrEmpty(TargetVariable);
        } }
    }
}
