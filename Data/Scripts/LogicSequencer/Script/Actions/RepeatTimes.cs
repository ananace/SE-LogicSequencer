using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class RepeatTimes : ScriptAction, IRepeat
    {
        [ProtoMember(1)]
        public int Times { get; set; }
        [ProtoMember(2)]
        public List<ScriptAction> Actions { get; set; } = new List<ScriptAction>();

        public override bool IsValid => Actions.Any() && Actions.All(a => a.IsValid) && Times >= 0;
    }
}
