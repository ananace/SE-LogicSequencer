using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class ArithmeticSimple : ScriptAction
    {
        [ProtoMember(1)]
        public List<DataSource> Operands { get; set; } = new List<DataSource>();
        [ProtoMember(2)]
        public Helper.MathHelper.OperationType Operator { get; set; }

        [ProtoMember(3)]
        public string TargetVariable { get; set; }

        public override bool IsValid => Operands.Any() && !string.IsNullOrEmpty(TargetVariable);
    }
}
