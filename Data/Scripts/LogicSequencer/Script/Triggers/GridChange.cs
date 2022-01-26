using ProtoBuf;

namespace LogicSequencer.Script.Triggers
{
    [ProtoContract]
    public class GridChange : ScriptTrigger
    {
        public override bool IsValid => true;
    }
}
