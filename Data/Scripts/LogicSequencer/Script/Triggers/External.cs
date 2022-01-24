using ProtoBuf;

namespace LogicSequencer.Script.Triggers
{
    [ProtoContract]
    public class External : ScriptTrigger
    {
        public override bool IsValid => true;
    }
}
