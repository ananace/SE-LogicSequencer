using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class Choose : ScriptAction
    {
        [ProtoContract]
        public class Choice
        {
            [ProtoMember(1)]
            public List<ScriptCondition> Conditions { get; set; } = new List<ScriptCondition>();
            [ProtoMember(2)]
            public List<ScriptAction> Actions { get; set; } = new List<ScriptAction>();

            [XmlIgnore]
            public bool IsValid => Conditions.Any() && Conditions.All(c => c.IsValid) &&
                Actions.Any() && Actions.All(a => a.IsValid);
        }

        [ProtoMember(3)]
        public List<Choice> Choices { get; set; } = new List<Choice>();
        [ProtoMember(4, IsRequired = false)]
        public List<ScriptAction> DefaultActions { get; set; } = new List<ScriptAction>();

        public override bool IsValid => Choices.Any() && Choices.All(c => c.IsValid) &&
            (!DefaultActions.Any() || DefaultActions.All(a => a.IsValid));
    }
}
