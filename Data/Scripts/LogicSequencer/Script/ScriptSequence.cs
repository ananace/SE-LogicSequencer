using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script
{
    [ProtoContract(UseProtoMembersOnly = true)]
    [XmlRoot]
    public class ScriptSequence
    {
        [ProtoMember(1)]
        [XmlElement]
        public string Name { get; set; }
        [ProtoMember(2, IsRequired = false)]
        [XmlElement]
        public string Description { get; set; } = null;

        [ProtoMember(3, IsRequired = false)]
        [XmlElement]
        public VRage.Serialization.SerializableDictionary<string, ScriptValue> Variables { get; set; } = new VRage.Serialization.SerializableDictionary<string, ScriptValue>();

        [ProtoMember(4)]
        public List<ScriptTrigger> Triggers { get; set; } = new List<ScriptTrigger>();
        [ProtoMember(5, IsRequired = false)]
        public List<ScriptCondition> Conditions { get; set; } = new List<ScriptCondition>();
        [ProtoMember(6)]
        public List<ScriptAction> Actions { get; set; } = new List<ScriptAction>();

        public bool IsValid => !string.IsNullOrEmpty(Name) &&
            Triggers.Any() && Triggers.All(t => t.IsValid) &&
            Actions.Any() && Actions.All(a => a.IsValid);
    }
}
