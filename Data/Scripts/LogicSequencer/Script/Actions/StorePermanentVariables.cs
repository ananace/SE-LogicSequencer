using System.Linq;
using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class StorePermanentVariables : ScriptAction
    {
        [ProtoMember(1)]
        public VRage.Serialization.SerializableDictionary<string, DataSource> Variables { get; set; } = new VRage.Serialization.SerializableDictionary<string, DataSource>();

        public override bool IsValid => Variables.Dictionary.Any() && Variables.Dictionary.All(v => !string.IsNullOrEmpty(v.Key) && v.Value.IsValid);
    }
}
