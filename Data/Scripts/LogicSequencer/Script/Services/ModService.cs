using System;
using System.Collections.Generic;
using System.Linq;
using LogicSequencer.Script.Helper;
using Sandbox.ModAPI;

namespace LogicSequencer.Script.Services
{
    using ModParameter = VRage.MyTuple<string, string, Type, bool, object>;
    public class ModService : ScriptService
    {
        public string ModProvidedID { get; set; }
        public string ModProvidedName { get; set; }
        public string ModProvidedDescription { get; set; }

        public Action<IEnumerable<IMyTerminalBlock>, Dictionary<string, object>> ModProvidedApply { get; set; }
        public Func<IEnumerable<IMyTerminalBlock>, bool> ModProvidedCanApply { get; set; }
        public Func<IEnumerable<ModParameter>> ModProvidedParameters { get; set; }

        List<ModParameter> _AvailableParameters;
        public override IEnumerable<Parameter> AvailableParameters { get {
            if (_AvailableParameters == null)
                _AvailableParameters = ModProvidedParameters.Invoke().ToList();

            foreach (var modEntry in _AvailableParameters)
            {
                ScriptValue value = null;
                if (modEntry.Item5 != null)
                {
                    value = new ScriptValue();
                    value.SetFromObject(modEntry.Item5);
                }
                yield return new Parameter {
                    Name = modEntry.Item1,
                    Description = modEntry.Item2,
                    Type = VariableTypeExtensions.GetTypeFor(modEntry.Item3),
                    IsRequired = modEntry.Item4,
                    DefaultValue = value,
                };
            }
        } }

        public override string ID => ModProvidedID;
        public override string Name => ModProvidedName;
        public override string Description => ModProvidedDescription;

        public override void Apply(IEnumerable<IMyTerminalBlock> blocks, Dictionary<string, ScriptValue> parameters)
        {
            var munged = parameters
                .Where(param => _AvailableParameters.Any(avail => avail.Item1 == param.Key))
                .Select(param => new KeyValuePair<string, object>(param.Key, param.Value.GetAsObject(_AvailableParameters.Find(p => p.Item1 == param.Key).Item3)))
                .ToDictionary(p => p.Key, p => p.Value);

            ModProvidedApply.Invoke(blocks, munged);
        }

        public override bool CanApplyTo(IEnumerable<IMyTerminalBlock> blocks)
        {
            return ModProvidedCanApply.Invoke(blocks);
        }
    }
}
