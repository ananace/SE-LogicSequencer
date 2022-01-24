using ProtoBuf;

namespace LogicSequencer.Script.Triggers
{
    [ProtoContract]
    public class Action : ScriptTrigger
    {
        public override bool IsValid => true;
    }
}
