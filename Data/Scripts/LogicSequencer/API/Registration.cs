using System;
using Sandbox.ModAPI;
using VRage;

using ModServiceDefinition = VRage.MyTuple<
    string, string, string,
    System.Func<System.Collections.Generic.IEnumerable<VRage.MyTuple<string, string, System.Type, bool, object>>>,
    System.Func<System.Collections.Generic.IEnumerable<Sandbox.ModAPI.IMyTerminalBlock>, bool>,
    System.Action<System.Collections.Generic.IEnumerable<Sandbox.ModAPI.IMyTerminalBlock>, System.Collections.Generic.Dictionary<string, object>>
>;

namespace LogicSequencer.API
{
    using ModRegistrationDefinition = MyTuple<
        string,
        Action<
            Action<ModServiceDefinition>,
            Action<string>
        >
    >;

    public class Registration : IDisposable
    {
        const long registerId = 1337, waitForRegistrationId = 9000000000000000000 + registerId;

        public bool IsRegistered { get; private set; }

        public static Registration Instance { get; private set; }
        public static bool IsReadyForUse => Instance?.IsRegistered ?? false;

        public string Name { get; private set; }
        readonly Action RegistrationFinished;

        bool IsDisposed { get; set; }
        bool InQueue { get; set; }
        Action<ModServiceDefinition> RegisterServiceFunction;
        Action<string> UnregisterServiceFunction;

        public static void Init(string name, Action onRegistered)
        {
            if (Instance != null)
                return;

            Instance = new Registration(name, onRegistered);
        }

        public static void Unload()
        {
            if (Instance == null)
                return;

            Instance.Dispose();
            Instance = null;
        }

        public static void RegisterService(ServiceRegistration service)
        {
            if (!IsReadyForUse)
                throw new ArgumentException("Trying to register service before ready");

            Instance.Register(service);
        }

        public static void UnregisterService(string name)
        {
            if (!IsReadyForUse)
                throw new ArgumentException("Trying to unregister service before ready");

            Instance.Unregister(name);
        }

        Registration(string name, Action onRegistered)
        {
            Name = name;
            RegistrationFinished = onRegistered;

            RequestRegistration();
            if (!IsRegistered)
            {
                InQueue = true;
                MyAPIUtilities.Static.RegisterMessageHandler(waitForRegistrationId, QueueHandler);
            }
        }

        public void Dispose()
        {
            try
            {
                IsDisposed = true;
                IsRegistered = false;
                RegisterServiceFunction = null;
                UnregisterServiceFunction = null;
                if (InQueue)
                    ExitQueue();
            }
            catch (Exception)
            {}
        }

        void RequestRegistration()
        {
            MyAPIUtilities.Static.SendModMessage(registerId, new ModRegistrationDefinition(
                Name,
                (register, unregister) => {
                    RegisterServiceFunction = register;
                    UnregisterServiceFunction = unregister;

                    FinishRegistration();
                }
            ));
        }

        void ExitQueue()
        {
            if (!InQueue)
                return;

            InQueue = false;
            MyAPIUtilities.Static.UnregisterMessageHandler(waitForRegistrationId, QueueHandler);
        }

        void QueueHandler(object message)
        {
            if (IsDisposed)
                return;

            if (IsRegistered)
            {
                ExitQueue();
                return;
            }

            RequestRegistration();
        }

        void FinishRegistration()
        {
            if (IsDisposed)
                return;

            ExitQueue();
            IsRegistered = true;

            try
            {
                RegistrationFinished?.Invoke();
            }
            catch (Exception ex)
            {
                VRage.Utils.MyLog.Default.WriteLineAndConsole($"[LogicSequencer.API] Failed to finish registration - {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                MyAPIGateway.Utilities?.ShowNotification($"[LogicSequencer.API] {Name} failed to finish registration.", 10000, VRage.Game.MyFontEnum.Red);
            }
        }

        public void Unregister(string name)
        {
            UnregisterServiceFunction?.Invoke(name);
        }

        public void Register(ServiceRegistration service)
        {
            RegisterServiceFunction?.Invoke(service);
        }
    }
}
