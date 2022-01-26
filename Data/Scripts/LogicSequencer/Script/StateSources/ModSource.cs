using System;
using System.Collections.Generic;
using System.Linq;
using LogicSequencer.Script.Helper;
using Sandbox.ModAPI;
using VRage;

namespace LogicSequencer.Script.StateSources
{
    public class ModSource : ScriptStateSource
    {
        public string ModName { get; set; }
        public string ModProvidedID { get; set; }
        public string ModProvidedName { get; set; }
        public string ModProvidedDescription { get; set; }
        public Type ModProvidedOutputType { get; set; }

        bool ModIsValid { get; set; } = true;
        public override bool IsValid => ModIsValid;

        public Func<IMyTerminalBlock, object> ModProvidedTryRead { get; set; }
        public Func<IMyTerminalBlock, bool> ModProvidedCanReadFrom { get; set; }

        public override string ID => ModProvidedID;
        public override string Name => ModProvidedName;
        public override string Description => ModProvidedDescription;
        public override VariableType ResultType => VariableTypeExtensions.GetTypeFor(ModProvidedOutputType);

        public override ScriptValue Read(IMyTerminalBlock block)
        {
            var resultObject = ModProvidedTryRead.Invoke(block);
            var result = new ScriptValue();
            result.SetFromObject(resultObject);
            return result;
        }

        public override bool CanReadFrom(IMyTerminalBlock block)
        {
            if (!ModIsValid)
                return false;

            try
            {
                return ModProvidedCanReadFrom.Invoke(block);
            }
            catch (Exception ex)
            {
                Util.Log.Error($"Error in CanReadFrom() for state source {ID} from mod {ModName}, disabling", ex, GetType(), false);
                ModIsValid = false;
                return false;
            }
        }
    }
}
