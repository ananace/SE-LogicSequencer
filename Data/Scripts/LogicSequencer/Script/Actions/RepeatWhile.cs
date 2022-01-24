using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class RepeatWhile : ScriptAction, IRepeat
    {
        [ProtoMember(1)]
        public List<ScriptCondition> Conditions { get; set; } = new List<ScriptCondition>();
        [ProtoMember(2)]
        public List<ScriptAction> Actions { get; set; } = new List<ScriptAction>();

        public override bool IsValid => Actions.Any() && Actions.All(a => a.IsValid) && Conditions.Any() && Conditions.All(c => c.IsValid);
    }
}
