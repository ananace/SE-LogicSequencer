using ProtoBuf;

namespace LogicSequencer.Script.Conditions
{
    [ProtoContract]
    public class Not : ScriptCondition
    {
        [ProtoMember(1)]
        public ScriptCondition Condition { get; set; }

        public override bool IsValid => Condition != null && Condition.IsValid;
    }
}
