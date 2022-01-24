using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class BlockSetProperty : ScriptAction
    {
        [ProtoMember(1)]
        public long BlockID { get; set; }

        [ProtoMember(2)]
        public string Property { get; set; }
        [ProtoMember(3)]
        public DataSource Source { get; set; }

        public override bool IsValid => BlockID != 0 && !string.IsNullOrEmpty(Property) && Source != null && Source.IsValid;
    }
}
