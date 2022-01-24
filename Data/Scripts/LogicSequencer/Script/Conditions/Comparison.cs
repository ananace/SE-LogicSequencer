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
        public Helper.MathHelper.OperationType Operation { get; set; }

        public override bool IsValid => SourceData != null && ComparisonData != null &&
            Helper.MathHelper.OperationType._ComparisonStart < Operation && Operation < Helper.MathHelper.OperationType._ComparisonEnd;
    }
}
