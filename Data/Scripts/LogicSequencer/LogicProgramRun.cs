using System;
using System.Collections.Generic;
using System.Linq;
using LogicSequencer.Script;
using ProtoBuf;

namespace LogicSequencer
{
    public partial class LogicProgramRun
    {
        [ProtoContract(UseProtoMembersOnly = true)]
        public class Execution
        {
            [ProtoMember(1)]
            public string Tag;
            [ProtoMember(2)]
            public IEnumerable<ScriptAction> ScriptCode;

            public IEnumerator<ScriptAction> ScriptCounter = null;

            [ProtoMember(3)]
            public int Step = 0;
        }

        public Blocks.LogicSequencer LogicSequencer { get; private set; }
        public Dictionary<string, ScriptValue> Variables { get; private set; }

        public ScriptTrigger TriggeredBy { get; set; }

        public TimeSpan? WaitingForDuration { get; set; }
        public ScriptTrigger WaitingForTrigger { get; set; }

        public ScriptAction LastAction { get; private set; }

        Execution _CurrentExecution = null;
        readonly Stack<Execution> _CurrentExecutionStack = new Stack<Execution>();
        public string CurrentExecutionTag => string.Join(" / ", new[] { _CurrentExecution }.Concat(_CurrentExecutionStack).Where(e => e != null).Select(e => e.Tag));
        public int? CurrentExecutionStep => _CurrentExecution?.Step;

        public Exception Exception { get; private set; } = null;

        public bool IsActive => !IsPaused && !IsCompleted && !IsFaulted;
        public bool IsPaused => WaitingForDuration != null || WaitingForTrigger != null;
        public bool IsCompleted => _CurrentExecution == null && !_CurrentExecutionStack.Any();
        public bool IsFaulted => Exception != null;
        public string StateName => IsFaulted ? "faulted" : (IsPaused ? "paused" : (IsActive ? "running" : "completed"));
        public DateTime StartedAt { get; private set; }


        public LogicProgramRun(Blocks.LogicSequencer logicSequencer)
        {
            LogicSequencer = logicSequencer;
            Variables = new Dictionary<string, ScriptValue>(LogicSequencer.Script.Variables.Dictionary);

            _CurrentExecutionStack.Push(new Execution { Tag = "script", ScriptCode = logicSequencer.Script.Actions });

            InitializeActionHandlers();
            InitializeConditionHandlers();

            StartedAt = DateTime.Now;
        }

        public bool RunStep()
        {
            if (!IsActive)
                return true;

            Util.Log.Debug("RunStep()");

            try
            {
                if (_CurrentExecution == null)
                {
                    Util.Log.Debug("  No current execution");
                    while (_CurrentExecutionStack.Any())
                    {
                        _CurrentExecution = _CurrentExecutionStack.Pop();
                        Util.Log.Debug($"    Popped {_CurrentExecution.Tag}@{_CurrentExecution.Step} from the stack");
                        if (_CurrentExecution.ScriptCounter == null)
                        {
                            Util.Log.Debug($"    No script counter, starting new");
                            _CurrentExecution.ScriptCounter = _CurrentExecution.ScriptCode.GetEnumerator();
                            if (!_CurrentExecution.ScriptCounter.MoveNext())
                            {
                                Util.Log.Debug($"      Failed to MoveNext(), ignoring");
                                continue;
                            }

                            if (_CurrentExecution.Step > 0)
                                Util.Log.Debug($"    Replaying state");
                            bool valid = true;
                            for (int i = 0; i < _CurrentExecution.Step; ++i)
                                if (!_CurrentExecution.ScriptCounter.MoveNext())
                                {
                                    Util.Log.Debug($"      Failed to replay state at step {i}");
                                    valid = false;
                                    break;
                                }

                            if (!valid)
                                continue;
                            break;
                        }
                    }
                }

                if (_CurrentExecution == null)
                {
                    Util.Log.Debug("  No valid execution available, stopping");
                    // Ran out of script
                    Stop();

                    return false;
                }

                var execution = _CurrentExecution.ScriptCounter;
                var action = execution.Current;
                if (action == null)
                {
                    Util.Log.Debug("  Retrieved null action from script, moving one");
                    if (!execution.MoveNext())
                    {
                        Util.Log.Debug("    Active execution ran to end");
                        _CurrentExecution = null;
                        return true;
                    }

                    action = execution.Current;
                    if (action == null)
                    {
                        Util.Log.Debug("    Action still null, waiting one");
                        return true;
                    }
                }

                if (!execution.MoveNext())
                {
                    Util.Log.Debug("  Active execution ending after action");
                    _CurrentExecution = null;
                }

                if (!action.IsValid)
                {
                    Util.Log.Info("Action in script was invalid, aborting.");
                    Util.Log.Info(Sandbox.ModAPI.MyAPIGateway.Utilities.SerializeToXML(action));
                    Stop();

                    return false;
                }

                Util.Log.Debug($"Running {action.GetType()}");
                if (Util.Log.DebugEnabled)
                    Util.Log.Debug(Sandbox.ModAPI.MyAPIGateway.Utilities.SerializeToXML(action));

                RunAction(action);
                LastAction = action;
                if (_CurrentExecution != null)
                    _CurrentExecution.Step++;
            }
            catch (Exception ex)
            {
                Util.Log.Error(ex, GetType());
                Exception = ex;
                Stop();

                return false;
            }
            finally
            {
            }

            return true;
        }

        public void Stop()
        {
            Finish();
        }

        void Finish()
        {
            Util.Log.Debug("Removing running instance");
            LogicSequencer.SequenceStop(this);
        }

        void AnnotateCurrentStep(string annotation)
        {
            if (_CurrentExecution == null)
                return;

            string tag = _CurrentExecution.Tag;

            var index = tag.IndexOf(':');
            if (index > 0)
                tag = $"{tag.Substring(0, index)}:{annotation}";

            _CurrentExecution.Tag = tag;
        }

        public ScriptValue ResolveDataSource(DataSource dataSource)
        {
            if (dataSource.HasVariable)
            {
                if (Variables.ContainsKey(dataSource.VariableName))
                    return Variables[dataSource.VariableName];
                else if (dataSource.HasValue)
                    return dataSource.Value;
                else
                    throw new ArgumentException("Unable to resolve datasource");
            }
            else
                return dataSource.Value;
        }
    }
}
