using ProtoBuf;
using Sandbox.ModAPI;

namespace LogicSequencer.Serialization.Packets
{
    [ProtoContract(UseProtoMembersOnly = true)]
    public class LogicSequencerBlockScriptChanged : EntityPacketBase
    {
        [ProtoMember(2)]
        public Script.ScriptSequence Script { get; set; }

        public LogicSequencerBlockScriptChanged(Script.ScriptSequence script)
        {
            Script = script;
        }

        public override bool Received()
        {
            if (AsBlock == null)
                return false;

            var logic = AsBlock.GameLogic?.GetAs<Blocks.LogicSequencer>();

            if (logic == null)
                return false;

            logic.Script = Script;

            return true;
        }
    }
}
