using System.Collections.Generic;
using ProtoBuf;

namespace LogicSequencer.Serialization
{
    [ProtoContract(UseProtoMembersOnly = true)]
    public class LogicSequencerBlockSettings
    {
        [ProtoMember(1)]
        public ProgramStartMode StartMode;
        [ProtoMember(2)]
        public int MaxRuns;

        //[ProtoMember(3)]
        //public List<LogicProgramRunStorage> CurrentExecutions;
    }
}
