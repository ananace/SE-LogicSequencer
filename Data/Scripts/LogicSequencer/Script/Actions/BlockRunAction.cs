using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class BlockRunAction : ScriptAction
    {
        [ProtoMember(1)]
        public long BlockID { get; set; }

        [ProtoMember(2)]
        public string Action { get; set; }

        public override bool IsValid => BlockID != 0 && !string.IsNullOrEmpty(Action);
    }
}
