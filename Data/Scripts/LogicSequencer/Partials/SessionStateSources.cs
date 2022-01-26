using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI;
using ModParameter = VRage.MyTuple<string, string, System.Type, bool, object>;

namespace LogicSequencer
{
    partial class Session
    {
        readonly List<Script.ScriptStateSource> _RegisteredStateSources = new List<Script.ScriptStateSource>();
        public IReadOnlyList<Script.ScriptStateSource> RegisteredStateSources => _RegisteredStateSources;

        public void RegisterStateSource(Script.ScriptStateSource source)
        {
            if (_RegisteredStateSources.Find((s) => s.ID == source.ID) != null)
                throw new ArgumentException("There's already a source with that ID registered");

            _RegisteredStateSources.Add(source);
        }

        public void UnregisterStateSource(string sourceID)
        {
            UnregisterStateSource(_RegisteredStateSources.Find((s) => s.ID == sourceID));
        }
        public void UnregisterStateSource(Script.ScriptStateSource source)
        {
            if (source == null)
                return;

            _RegisteredStateSources.Remove(source);
        }

        public IEnumerable<Script.ScriptStateSource> FindSourcesFor(IMyTerminalBlock block, IMyTerminalBlock self)
        {
            return _RegisteredStateSources.Where(s => s.CanReadFrom(block) && (!s.SelfOnly || block == self)).OrderBy(s => s.ID);
        }

        void RegisterInternalStateSources()
        {
            // Grid sources
            RegisterStateSource(new Script.StateSources.GridDynamicSource());
            RegisterStateSource(new Script.StateSources.GridLargeSource());
            RegisterStateSource(new Script.StateSources.GridMassSource());
            RegisterStateSource(new Script.StateSources.GridSmallSource());
            RegisterStateSource(new Script.StateSources.GridStaticSource());
        }

        void UnregisterStateSources()
        {
            _RegisteredStateSources.Clear();
        }
    }
}
