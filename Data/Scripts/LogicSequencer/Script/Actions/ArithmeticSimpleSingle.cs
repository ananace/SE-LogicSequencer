using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class ArithmeticSimpleSingle : ScriptAction
    {
        [ProtoMember(1)]
        public DataSource Operand { get; set; }
        [ProtoMember(2)]
        public Helper.MathHelper.SingleObjectOperationType SingleOperator { get; set; }

        [ProtoMember(3)]
        public string TargetVariable { get; set; }

        public override bool IsValid => Operand != null && !string.IsNullOrEmpty(TargetVariable);
    }
}
