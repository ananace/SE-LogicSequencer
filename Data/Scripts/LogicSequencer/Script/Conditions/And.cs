using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace LogicSequencer.Script.Conditions
{
    [ProtoContract]
    public class And : ScriptCondition
    {
        [ProtoMember(1)]
        public List<ScriptCondition> Conditions { get; set; } = new List<ScriptCondition>();

        public override bool IsValid => Conditions.Any() && Conditions.All(c => c.IsValid);
    }
}
