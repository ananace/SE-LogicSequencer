namespace LogicSequencer.UI
{
    public interface ISequenceContainer
    {
        void Load(Script.ScriptSequence script);
        void Save(Script.ScriptSequence script);
    }
}
