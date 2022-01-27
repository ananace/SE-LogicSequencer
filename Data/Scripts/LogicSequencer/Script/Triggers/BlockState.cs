using System;
using System.ComponentModel;
using System.Xml.Serialization;
using ProtoBuf;

namespace LogicSequencer.Script.Triggers
{
    [ProtoContract]
    class BlockState : ScriptTrigger
    {
        [ProtoMember(1)]
        public string StateSource { get; set; }
        [ProtoMember(2)]
        public BlockSelector Block { get; set; }
        [ProtoMember(3)]
        public ScriptValue Comparison { get; set; }
        [ProtoMember(4)]
        [DefaultValue("CompareEqual")]
        public string Operation { get; set; }

        [XmlIgnore]
        public Helper.MathHelper.OperationType OperationType {
            get { return (Helper.MathHelper.OperationType)Enum.Parse(typeof(Helper.MathHelper.OperationType), Operation, true); }
            set { Operation = value.ToString(); }
        }

        public override bool IsValid { get {
            Helper.MathHelper.OperationType op;
            return !string.IsNullOrEmpty(StateSource) &&
                Block != null && Block.IsValid &&
                Comparison != null &&
                !string.IsNullOrEmpty(Operation) && !Operation.StartsWith("_") && Enum.TryParse(Operation, true, out op) &&
                Helper.MathHelper.OperationType._ComparisonStart < op && op < Helper.MathHelper.OperationType._ComparisonEnd;
        } }
    }
}
