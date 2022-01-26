using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class BlockGetState : ScriptAction
    {
        [ProtoMember(1)]
        public BlockSelector Block { get; set; }

        [ProtoMember(2)]
        public string StateSource { get; set; }
        [ProtoMember(3)]
        public string IntoVariable { get; set; }

        public override bool IsValid => Block.IsValid && !string.IsNullOrEmpty(StateSource) && !string.IsNullOrEmpty(IntoVariable);
    }
}
