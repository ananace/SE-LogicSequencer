using ProtoBuf;

namespace LogicSequencer.Script.Conditions
{
    [ProtoContract]
    public class VariableIsTruthy : ScriptCondition
    {
        [ProtoMember(1)]
        public string Variable { get; set; }

        public override bool IsValid => !string.IsNullOrEmpty(Variable);
    }
}
