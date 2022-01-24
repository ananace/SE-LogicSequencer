using System;
using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class WaitTrigger : ScriptAction
    {
        [ProtoMember(1)]
        public ScriptTrigger TriggerToWait { get; set; }
        [ProtoMember(2, IsRequired = false)]
        public TimeSpan? Timeout { get; set; } = null;

        public override bool IsValid => TriggerToWait.IsValid && (!Timeout.HasValue || Timeout.Value.TotalMilliseconds >= 0);
    }
}
