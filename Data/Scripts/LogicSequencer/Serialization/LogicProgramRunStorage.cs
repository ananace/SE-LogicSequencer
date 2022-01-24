using System.Collections.Generic;
using ProtoBuf;

namespace LogicSequencer.Serialization
{
    [ProtoContract(UseProtoMembersOnly = true)]
    public class LogicProgramRunStorage
    {
        [ProtoMember(0)]
        public Dictionary<string, object> Variables;
    }
}
