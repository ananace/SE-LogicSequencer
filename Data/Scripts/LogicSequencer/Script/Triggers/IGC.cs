using ProtoBuf;

namespace LogicSequencer.Script.Triggers
{
    [ProtoContract]
    public class IGC : ScriptTrigger
    {
        public enum IGCSource
        {
            Broadcast,
            Unicast
        }

        [ProtoMember(2)]
        public string Tag { get; set; }

        [ProtoMember(3)]
        public IGCSource Source { get; set; }
        [ProtoMember(4, IsRequired = false)]
        public string DataVariable { get; set; }
        [ProtoMember(5, IsRequired = false)]
        public string SourceVariable { get; set; }

        public override bool IsValid => !string.IsNullOrEmpty(Tag);
    }
}
