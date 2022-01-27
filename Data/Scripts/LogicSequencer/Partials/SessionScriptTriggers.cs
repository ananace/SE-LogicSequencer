using System;
using System.Collections.Generic;
using System.Linq;
using LogicSequencer.Script;
using LogicSequencer.Script.Helper;

namespace LogicSequencer
{
    public partial class Session
    {
        Dictionary<ScriptTrigger, Blocks.LogicSequencer> _RegisteredTriggers = new Dictionary<ScriptTrigger, Blocks.LogicSequencer>();
        public IReadOnlyDictionary<ScriptTrigger, Blocks.LogicSequencer> RegisteredTriggers => _RegisteredTriggers;

        List<LogicProgramRun> _PausedScripts = new List<LogicProgramRun>();
        public IReadOnlyList<LogicProgramRun> PausedScripts => _PausedScripts;

        public void RegisterTrigger(ScriptTrigger trigger, Blocks.LogicSequencer block)
        {
            _RegisteredTriggers[trigger] = block;
        }

        public void UnregisterTrigger(ScriptTrigger trigger)
        {
            _RegisteredTriggers.Remove(trigger);
        }


        void HandleScriptTriggers()
        {
            handleTriggersForPaused();
            handleTriggersForBlocks();
        }

        void handleTriggersForPaused()
        {
            List<LogicProgramRun> toUnpause = new List<LogicProgramRun>();
            foreach (var paused in _PausedScripts)
            {
                if (toUnpause.Contains(paused))
                    continue;

                if (paused.WaitingForTrigger is Script.Triggers.BlockState)
                {
                    var state = paused.WaitingForTrigger as Script.Triggers.BlockState;
                    var source = RegisteredStateSources.FirstOrDefault(s => s.Name == state.StateSource);
                    var block = state.Block.Resolve(paused);

                    var value = source.Read(block);
                    var result = MathHelper.PerformOperation(state.OperationType, value, state.Comparison);

                    if (result.Boolean)
                        toUnpause.Add(paused);
                }
                else if (paused.WaitingForTrigger is Script.Triggers.Time)
                {
                    var time = paused.WaitingForTrigger as Script.Triggers.Time;
                    if ((time.MSRemaining ?? 0) <= 0)
                    {
                        time.MSRemaining = (long)time.Every.TotalMilliseconds;
                        toUnpause.Add(paused);
                    }
                }
            }

            _PausedScripts.RemoveAll(p => toUnpause.Contains(p));
            foreach (var paused in toUnpause)
            {
                paused.WaitingForDuration = null;
                paused.WaitingForTrigger = null;
            }
        }

        void handleTriggersForBlocks()
        {
            // Triggers.Action is handled in SessionTerminalControls
            // Triggers.BlockState is handled here
            // Triggers.External is handled in SessionTerminalControls
            // Triggers.GridChange is handled in Blocks.LogicSequencer
            // Triggers.IGC not yet handled
            // Triggers.Sun not yet handled
            // Triggers.Time is handled here

            foreach (var registered in _RegisteredTriggers)
            {
                if (registered.Key is Script.Triggers.BlockState)
                {
                    var state = registered.Key as Script.Triggers.BlockState;
                    var source = RegisteredStateSources.FirstOrDefault(s => s.Name == state.StateSource);
                    var block = registered.Value.Block;

                    var value = source.Read(block);
                    var result = MathHelper.PerformOperation(state.OperationType, value, state.Comparison);

                    if (result.Boolean)
                        registered.Value.DoTrigger(state);
                }
                else if (registered.Key is Script.Triggers.Time)
                {
                    var time = registered.Key as Script.Triggers.Time;
                    if ((time.MSRemaining ?? 0) <= 0)
                    {
                        time.MSRemaining = (long)time.Every.TotalMilliseconds;
                        registered.Value.DoTrigger(time);
                    }
                }
            }
        }
    }
}
