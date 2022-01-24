using System.Xml.Serialization;

namespace LogicSequencer.Script
{
    public interface IScriptPiece
    {
        [XmlIgnore]
        bool IsValid { get; }
    }
}
