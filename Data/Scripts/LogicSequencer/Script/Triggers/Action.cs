using ProtoBuf;

namespace LogicSequencer.Script.Triggers
{
    [ProtoContract]
    public class Action : ScriptTrigger
    {
        [ProtoMember(1, IsRequired = false)]
        public string WithArgument { get; set; }

        public override bool IsValid => true;
    }
}
