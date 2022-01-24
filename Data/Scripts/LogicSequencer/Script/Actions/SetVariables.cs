using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class SetVariables : ScriptAction
    {
        [ProtoMember(1)]
        public VRage.Serialization.SerializableDictionary<string, DataSource> Variables { get; set; } = new VRage.Serialization.SerializableDictionary<string, DataSource>();

        public override bool IsValid => Variables.Dictionary.Any() && Variables.Dictionary.All(v => !string.IsNullOrEmpty(v.Key) && v.Value.IsValid);
    }
}
