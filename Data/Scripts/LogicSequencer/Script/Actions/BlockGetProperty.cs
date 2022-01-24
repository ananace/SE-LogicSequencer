using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class BlockGetProperty : ScriptAction
    {
        [ProtoMember(1)]
        public long BlockID { get; set; }

        [ProtoMember(2)]
        public string Property { get; set; }
        [ProtoMember(3, IsRequired = false)]
        public VariableType? Type { get; set; } = null;
        [ProtoMember(4)]
        public string IntoVariable { get; set; }

        public override bool IsValid => BlockID != 0 && !string.IsNullOrEmpty(Property) && !string.IsNullOrEmpty(IntoVariable);
    }
}
