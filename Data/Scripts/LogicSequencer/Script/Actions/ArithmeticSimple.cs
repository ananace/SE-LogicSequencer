using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script.Actions
{
    [ProtoContract]
    public class ArithmeticSimple : ScriptAction
    {
        [ProtoMember(1)]
        public List<DataSource> Operands { get; set; } = new List<DataSource>();
        [ProtoMember(2)]
        public string Operator { get; set; }
        [XmlIgnore]
        public Helper.MathHelper.OperationType OperatorType => (Helper.MathHelper.OperationType)Enum.Parse(typeof(Helper.MathHelper.OperationType), Operator, true);

        [ProtoMember(3)]
        public string TargetVariable { get; set; }

        public override bool IsValid { get {
            Helper.MathHelper.OperationType op;
            return !Operator.StartsWith("_") && Enum.TryParse(Operator, true, out op) && Operands.Any() && !string.IsNullOrEmpty(TargetVariable);
        } }
    }
}
