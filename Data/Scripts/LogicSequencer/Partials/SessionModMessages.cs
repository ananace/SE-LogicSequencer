using System;
using Sandbox.ModAPI;
using VRage;

using ModServiceDefinition = VRage.MyTuple<
    string, string, string,
    System.Func<System.Collections.Generic.IEnumerable<VRage.MyTuple<string, string, System.Type, bool, object>>>,
    System.Func<System.Collections.Generic.IEnumerable<Sandbox.ModAPI.IMyTerminalBlock>, bool>,
    System.Action<System.Collections.Generic.IEnumerable<Sandbox.ModAPI.IMyTerminalBlock>, System.Collections.Generic.Dictionary<string, object>>
>;

namespace LogicSequencer
{
    using ModRegistrationDefinition = MyTuple<
        string,
        Action<
            Action<ModServiceDefinition>,
            Action<string>
        >
    >;

    public partial class Session
    {
        const long modRegisterId = 1337, modWaitForRegistrationId = 9000000000000000000 + modRegisterId;

        void SetupModAPI()
        {
            MyAPIUtilities.Static.RegisterMessageHandler(modRegisterId, ModAPIRequest);
            MyAPIUtilities.Static.SendModMessage(modWaitForRegistrationId, null);
        }

        void UnloadModAPI()
        {
            MyAPIUtilities.Static.UnregisterMessageHandler(modRegisterId, ModAPIRequest);
        }

        void ModAPIRequest(object data)
        {
            if (!(data is ModRegistrationDefinition))
                return;

            var definition = (ModRegistrationDefinition)data;
            var modName = definition.Item1;
            var modMethods = definition.Item2;

            try
            {
                modMethods.Invoke((modDef) => RegisterModService(modName, modDef), UnregisterService);
            }
            catch (Exception ex)
            {
                Util.Log.Error($"ModAPIRequest() for {modName}", ex, GetType(), false);
            }
        }

        void RegisterModService(string modName, ModServiceDefinition definition)
        {
            var service = new Script.Services.ModService {
                ModName = modName,
                ModProvidedID = definition.Item1,
                ModProvidedName = definition.Item2,
                ModProvidedDescription = definition.Item3,
                ModProvidedParameters = definition.Item4,
                ModProvidedCanApply = definition.Item5,
                ModProvidedApply = definition.Item6
            };

            RegisterService(service);
        }
    }
}
