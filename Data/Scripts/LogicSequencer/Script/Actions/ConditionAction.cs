using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class ConditionAction : ScriptAction
    {
        [ProtoMember(1)]
        public ScriptCondition Condition { get; set; }

        public override bool IsValid => Condition != null && Condition.IsValid;
    }
}
