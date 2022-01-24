using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicSequencer.Script;
using LogicSequencer.Script.Helper;
using LogicSequencer.Util;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using VRage.Game.ModAPI;

namespace LogicSequencer
{
    public partial class LogicProgramRun
    {
        readonly Dictionary<Type, Action<ScriptAction>> _ActionHandlers = new Dictionary<Type, Action<ScriptAction>>();

        void InitializeActionHandlers()
        {
            _ActionHandlers.Add(typeof(Script.Actions.ArithmeticComplex), HandleArithmeticComplexAction);
            _ActionHandlers.Add(typeof(Script.Actions.ArithmeticSimple), HandleArithmeticSimpleAction);
            _ActionHandlers.Add(typeof(Script.Actions.ArithmeticSimpleSingle), HandleArithmeticSimpleSingleAction);
            _ActionHandlers.Add(typeof(Script.Actions.BlockGetProperty), HandleBlockGetPropertyAction);
            _ActionHandlers.Add(typeof(Script.Actions.BlockSetProperty), HandleBlockSetPropertyAction);
            _ActionHandlers.Add(typeof(Script.Actions.BlockRunAction), HandleBlockRunActionAction);
            _ActionHandlers.Add(typeof(Script.Actions.CallService), HandleCallServiceAction);
            _ActionHandlers.Add(typeof(Script.Actions.Choose), HandleChooseAction);
            _ActionHandlers.Add(typeof(Script.Actions.ConditionAction), HandleConditionAction);
            _ActionHandlers.Add(typeof(Script.Actions.Delay), HandleDelayAction);
            _ActionHandlers.Add(typeof(Script.Actions.RepeatTimes), HandleRepeatAction);
            _ActionHandlers.Add(typeof(Script.Actions.RepeatUntil), HandleRepeatAction);
            _ActionHandlers.Add(typeof(Script.Actions.RepeatWhile), HandleRepeatAction);
            _ActionHandlers.Add(typeof(Script.Actions.SetVariables), HandleSetVariablesAction);
            _ActionHandlers.Add(typeof(Script.Actions.StorePermanentVariables), HandleStorePermanentVariablesAction);
            _ActionHandlers.Add(typeof(Script.Actions.WaitTrigger), HandleWaitTriggerAction);
        }

        void RunAction(ScriptAction action)
        {
            var handler = _ActionHandlers[action.GetType()];
            if (handler == null)
                return;

            Log.Debug($"Invoking handler for {action}");
            handler.Invoke(action);
        }

        void HandleArithmeticComplexAction(ScriptAction action)
        {
            Log.Debug("HandleArithmeticComplexAction()");
            var realAction = action as Script.Actions.ArithmeticComplex;

            Variables[realAction.TargetVariable] = MathHelper.ResolveArithmeticPart(realAction.Part, this);
            Log.Debug($"Stored {realAction.TargetVariable} as {Variables[realAction.TargetVariable]} after {realAction}");
        }

        void HandleArithmeticSimpleAction(ScriptAction action)
        {
            var realAction = action as Script.Actions.ArithmeticSimple;

            Variables[realAction.TargetVariable] = MathHelper.DoOperation(realAction.OperatorType, realAction.Operands.Select(d => ResolveDataSource(d)));
            Log.Debug($"Stored {realAction.TargetVariable} as {Variables[realAction.TargetVariable]} after {realAction}");
        }

        void HandleArithmeticSimpleSingleAction(ScriptAction action)
        {
            var realAction = action as Script.Actions.ArithmeticSimpleSingle;

            Variables[realAction.TargetVariable] = MathHelper.DoOperation(realAction.SingleOperatorType, ResolveDataSource(realAction.Operand));
            Log.Debug($"Stored {realAction.TargetVariable} as {Variables[realAction.TargetVariable]} after {realAction}");
        }

        void HandleBlockGetPropertyAction(ScriptAction action)
        {
            var realAction = action as Script.Actions.BlockGetProperty;

            var gts = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(LogicSequencer.Block.CubeGrid);

            var block = ResolveBlockSelector(realAction.Block);
            if (block == null)
                throw new ArgumentException($"Failed to find a block with the selector {realAction.Block}");

            var prop = block.GetProperty(realAction.Property);
            if (prop == null)
                throw new ArgumentException($"Failed to find a property with the name {realAction.Property}");

            ScriptValue value = prop.GetScriptValue(block);

            if (realAction.Type.HasValue)
                value = value.ConvertToScriptType(realAction.Type.Value);

            Variables[realAction.IntoVariable] = value;
            Log.Debug($"Stored {realAction.IntoVariable} as {value} after {realAction}");
        }

        void HandleBlockSetPropertyAction(ScriptAction action)
        {
            var realAction = action as Script.Actions.BlockSetProperty;

            var gts = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(LogicSequencer.Block.CubeGrid);

            var block = ResolveBlockSelector(realAction.Block);
            if (block == null)
                throw new ArgumentException($"Failed to find a block with the selector {realAction.Block}");

            var prop = block.GetProperty(realAction.Property);
            if (prop == null)
                throw new ArgumentException($"Failed to find a property with the name {realAction.Property}");

            var value = ResolveDataSource(realAction.Source);
            prop.SetScriptValue(block, value);

            Log.Debug($"Set property {realAction.Property} to {value} in {realAction}");
        }

        void HandleBlockRunActionAction(ScriptAction action)
        {
            var realAction = action as Script.Actions.BlockRunAction;

            var gts = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(LogicSequencer.Block.CubeGrid);

            var block = ResolveBlockSelector(realAction.Block);
            if (block == null)
                throw new ArgumentException($"Failed to find a block with the selector {realAction.Block}");

            var blockAction = block.GetActionWithName(realAction.Action);
            if (blockAction == null)
                throw new ArgumentException($"Failed to find an action with the name {realAction.Action}");

            blockAction.Apply(block);

            Log.Debug($"Ran action {realAction.Action} in {realAction}");
        }

        void HandleCallServiceAction(ScriptAction action)
        {
            var realAction = action as Script.Actions.CallService;

            var service = Session.Instance.RegisteredServices.FirstOrDefault(s => s.Name == realAction.Name);
            if (service == null)
                throw new ArgumentException($"Failed to find a service with the name {realAction.Name}");

            var blocks = ResolveMultiBlockSelector(realAction.Blocks);

            // Filter out only the paramters wanted by the service
            var providedParameters = realAction.Parameters.Dictionary
                .Where(kv => service.AvailableParameters.Any(p => p.Name == kv.Key))
                .Select(kv => new KeyValuePair<string,ScriptValue>(kv.Key, ResolveDataSource(kv.Value)))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            // Ensure all required parameters are provided
            if (!service.AvailableParameters.Where(param => param.IsRequired).All(req => providedParameters.ContainsKey(req.Name)))
                throw new ArgumentException($"Not all required parameters are provided");

            // Convert provided data to the correct types
            foreach (var wanted in service.AvailableParameters)
            {
                var provided = providedParameters[wanted.Name];
                if (provided.TypeEnum != wanted.Type)
                    providedParameters[wanted.Name] = provided.ConvertToScriptType(wanted.Type);
            }

            // Apply the result
            service.Apply(blocks, providedParameters);

            Log.Debug($"Applied service {service.Name} to {blocks.Count()} blocks");
        }

        void HandleChooseAction(ScriptAction action)
        {
            var realAction = action as Script.Actions.Choose;

            _CurrentExecutionStack.Push(_CurrentExecution);
            _CurrentExecution = null;

            bool found = false;
            int i = 0;
            foreach (var choice in realAction.Choices)
            {
                if (FulfillsAllConditions(choice.Conditions))
                {
                    _CurrentExecutionStack.Push(new Execution { Tag = $"choose:{i}", ScriptCode = choice.Actions });
                    found = true;
                    break;
                }
                ++i;
            }

            if (!found)
                _CurrentExecutionStack.Push(new Execution { Tag = "choose:default", ScriptCode = realAction.DefaultActions });
        }

        void HandleConditionAction(ScriptAction action)
        {
            var realAction = action as Script.Actions.ConditionAction;
            if (!FulfillsAllConditions(new[] { realAction.Condition }))
            {
                Stop();
            }
        }

        void HandleDelayAction(ScriptAction action)
        {
            var realAction = action as Script.Actions.Delay;
            WaitingForDuration = realAction.Time;
        }

        IEnumerable<ScriptAction> HandleRepeatActionEnumerable(Script.Actions.IRepeat repeat)
        {
            if (repeat is Script.Actions.RepeatTimes)
            {
                var repeatTimes = repeat as Script.Actions.RepeatTimes;
                for (int i = 0; i < repeatTimes.Times; ++i)
                {
                    AnnotateCurrentStep(i.ToString());

                    foreach (var action in repeat.Actions)
                        yield return action;
                }
            }
            else if (repeat is Script.Actions.RepeatWhile)
            {
                var repeatUntil = repeat as Script.Actions.RepeatUntil;
                AnnotateCurrentStep("until");
                while (!FulfillsAllConditions(repeatUntil.Conditions))
                    foreach (var action in repeat.Actions)
                        yield return action;
            }
            else if (repeat is Script.Actions.RepeatWhile)
            {
                var repeatWhile = repeat as Script.Actions.RepeatWhile;
                AnnotateCurrentStep("while");
                while (FulfillsAllConditions(repeatWhile.Conditions))
                    foreach (var action in repeat.Actions)
                        yield return action;
            }
        }

        void HandleRepeatAction(ScriptAction action)
        {
            var realAction = action as Script.Actions.IRepeat;

            _CurrentExecutionStack.Push(_CurrentExecution);
            _CurrentExecution = null;

            var actions = HandleRepeatActionEnumerable(realAction);
            _CurrentExecutionStack.Push(new Execution { Tag = "repeat", ScriptCode = actions });
        }

        void HandleSetVariablesAction(ScriptAction action)
        {
            var realAction = action as Script.Actions.SetVariables;
            foreach (var variable in realAction.Variables.Dictionary)
            {
                Variables[variable.Key] = ResolveDataSource(variable.Value);
                Log.Debug($"Stored {variable.Key} as {Variables[variable.Key]} in {realAction}");
            }
        }

        void HandleStorePermanentVariablesAction(ScriptAction action)
        {
            var realAction = action as Script.Actions.SetVariables;
            foreach (var variable in realAction.Variables.Dictionary)
            {
                LogicSequencer.Script.Variables[variable.Key] = ResolveDataSource(variable.Value);
                Log.Debug($"Permanently stored {variable.Key} as {LogicSequencer.Script.Variables[variable.Key]} in {realAction}");
            }
            LogicSequencer.SettingsChanged(Blocks.LogicSequencer.ChangedSettingsFlags.Script);
        }

        void HandleWaitTriggerAction(ScriptAction action)
        {
            var realAction = action as Script.Actions.WaitTrigger;
            WaitingForTrigger = realAction.TriggerToWait;
            WaitingForDuration = realAction.Timeout;
        }
    }
}
