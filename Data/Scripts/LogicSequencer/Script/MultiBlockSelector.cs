using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace LogicSequencer.Script
{
    [ProtoContract]
    public class MultiBlockSelector
    {
        [ProtoMember(1, IsRequired = false)]
        public string Name { get; set; }
        [ProtoMember(2, IsRequired = false)]
        public string GroupName { get; set; }
        [ProtoMember(3, IsRequired = false)]
        public List<BlockSelector> Blocks { get; set; }

        public bool IsValid => !string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(GroupName) || (Blocks != null && Blocks.Any() && Blocks.All(b => b.IsValid));
    }
}
