using ProtoBuf;
using Sandbox.ModAPI;

namespace LogicSequencer.Serialization.Packets
{
    [ProtoContract(UseProtoMembersOnly = true)]
    public class LogicSequencerBlockSettingsChanged : EntityPacketBase
    {
        [ProtoMember(2)]
        public LogicSequencerBlockSettings Settings;

        public LogicSequencerBlockSettingsChanged(LogicSequencerBlockSettings settings)
        {
            Settings = settings;
        }

        public override bool Received()
        {
            var logic = AsBlock?.GameLogic?.GetAs<Blocks.LogicSequencer>();

            if (logic == null)
                return false;

            logic.StartMode = Settings.StartMode;
            logic.MaxRuns = Settings.MaxRuns;

            return true;
        }
    }
}
