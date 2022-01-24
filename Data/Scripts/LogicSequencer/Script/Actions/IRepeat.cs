using System.Collections.Generic;

namespace LogicSequencer.Script.Actions
{
    public interface IRepeat
    {
        List<ScriptAction> Actions { get; set; }
    }
}
