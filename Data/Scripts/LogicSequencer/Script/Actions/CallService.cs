using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class CallService : ScriptAction
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public MultiBlockSelector Blocks { get; set; }
        [ProtoMember(3)]
        public VRage.Serialization.SerializableDictionary<string, DataSource> Parameters { get; set; }

        public override bool IsValid => !string.IsNullOrEmpty(Name) && Blocks.IsValid;
    }
}
