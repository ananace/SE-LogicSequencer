using ProtoBuf;

namespace LogicSequencer.Script.Conditions
{
    [ProtoContract]
    public class VariableIsType : ScriptCondition
    {
        [ProtoMember(1)]
        public string Variable { get; set; }
        [ProtoMember(2)]
        public VariableType Type { get; set; }

        public override bool IsValid => !string.IsNullOrEmpty(Variable);
    }
}
