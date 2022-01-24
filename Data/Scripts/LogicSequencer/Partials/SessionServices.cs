using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI;
using ModParameter = VRage.MyTuple<string, string, System.Type, bool, object>;

namespace LogicSequencer
{
    partial class Session
    {
        readonly List<Script.ScriptService> _RegisteredServices = new List<Script.ScriptService>();
        public IReadOnlyList<Script.ScriptService> RegisteredServices => _RegisteredServices;

        public void RegisterService(Script.ScriptService service)
        {
            if (_RegisteredServices.Find((s) => s.ID == service.ID) != null)
                throw new ArgumentException("There's already a service with that ID registered");

            _RegisteredServices.Add(service);
        }

        public void UnregisterService(string serviceID)
        {
            UnregisterService(_RegisteredServices.Find((s) => s.ID == serviceID));
        }
        public void UnregisterService(Script.ScriptService service)
        {
            if (service == null)
                return;

            _RegisteredServices.Remove(service);
        }

        public IEnumerable<Script.ScriptService> FindServicesFor(IEnumerable<IMyTerminalBlock> blocks)
        {
            return _RegisteredServices.Where(s => s.CanApplyTo(blocks)).OrderBy(s => s.ID);
        }

        void RegisterInternalServices()
        {
            // IMyFunctionalBlock services
            RegisterService(new Script.Services.FunctionalOffService());
            RegisterService(new Script.Services.FunctionalOnService());
            RegisterService(new Script.Services.FunctionalOnOffService());

            // IMyLightingBlock services
            RegisterService(new Script.Services.LightColorService());
            RegisterService(new Script.Services.LightOffService());
            RegisterService(new Script.Services.LightOnService());
            RegisterService(new Script.Services.LightOnOffService());

            // IMyThrust services
            RegisterService(new Script.Services.ThrustOverrideService());
            RegisterService(new Script.Services.ThrustOffService());
            RegisterService(new Script.Services.ThrustOnService());
            RegisterService(new Script.Services.ThrustOnOffService());
        }

        void UnregisterServices()
        {
            _RegisteredServices.Clear();
        }
    }
}
