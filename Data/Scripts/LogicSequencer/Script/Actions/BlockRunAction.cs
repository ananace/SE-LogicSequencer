using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class BlockRunAction : ScriptAction
    {
        [ProtoMember(1)]
        public BlockSelector Block { get; set; }

        [ProtoMember(2)]
        public string Action { get; set; }

        public override bool IsValid => Block.IsValid && !string.IsNullOrEmpty(Action);
    }
}
