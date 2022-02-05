using System;
using Sandbox.ModAPI;
using VRage;

using ModServiceV1Definition = VRage.MyTuple<
    string, string, string,
    System.Func<System.Collections.Generic.IEnumerable<VRage.MyTuple<string, string, System.Type, bool, object>>>,
    System.Func<System.Collections.Generic.IEnumerable<Sandbox.ModAPI.IMyTerminalBlock>, bool>,
    System.Action<System.Collections.Generic.IEnumerable<Sandbox.ModAPI.IMyTerminalBlock>, System.Collections.Generic.Dictionary<string, object>>
>;

namespace LogicSequencer
{
    using ModRegistrationV1Definition = MyTuple<
        string,
        string,
        Action<
            Action<ModServiceV1Definition>,
            Action<string>
        >
    >;

    public partial class Session
    {
        const long modRegisterId = 1337, modWaitForRegistrationId = 1000000000000 + modRegisterId;

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
            string version = "",
                   modName = "";

            try
            {
                if (data is ModRegistrationV1Definition)
                {
                    var definition = (ModRegistrationV1Definition)data;
                    version = definition.Item1;
                    modName = definition.Item2;
                    var modMethods = definition.Item3;

                    if (version != "1")
                    {
                        Util.Log.Info($"Received invalid Mod API request with version {version} from {modName}, ignoring.");
                        return;
                    }

                    modMethods.Invoke((modDef) => RegisterModV1Service(modName, modDef), UnregisterService);
                }
            }
            catch (Exception ex)
            {
                Util.Log.Error($"ModAPIRequest() for {modName}, version {version}", ex, GetType(), false);
            }
        }

        void RegisterModV1Service(string modName, ModServiceV1Definition definition)
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
