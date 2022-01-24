using System;
using System.Text;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Utils;

namespace LogicSequencer.Util
{
    public static class Log
    {
        public const string ModName = "LogicSequencer";
        public static bool DebugEnabled { get; set; } = false;

        public static void Debug(string info)
        {
            if (!DebugEnabled)
                return;

            WriteLog($"DEBUG {info}");
        }

        public static void Info(string info)
        {
            WriteLog(info);
        }

        public static void ImportantInfo(string info)
        {
            WriteLog(info);
            if(MyAPIGateway.Session?.Player != null)
                MyAPIGateway.Utilities.ShowNotification($"[{ModName}] {info}", 10000, MyFontEnum.White);
        }

        public static void Error(Exception e, Type caller = null, bool fatal = true)
        {
            Error(null, e, caller: caller, fatal: fatal);
        }

        public static void Error(string msg, Exception e, Type caller = null, bool fatal = true)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("!!Error");
            if (caller != null)
                sb.Append($" from {caller}");
            sb.AppendLine("!!");
            if (!string.IsNullOrEmpty(msg))
                sb.AppendLine(msg);
            if (e != null)
                sb.Append($"{e.GetType().Name}: {e.Message}\n{e.StackTrace}");

            WriteLog(sb.ToString());
            if(fatal && MyAPIGateway.Session?.Player != null)
                MyAPIGateway.Utilities.ShowNotification($"[ ERROR in {caller?.Name ?? "LogicSequencer"}: {e.Message} | Send SpaceEngineers.Log to mod author ]", 10000, MyFontEnum.Red);
        }

        static void WriteLog(string message)
        {
            MyLog.Default.WriteLineAndConsole($"[{ModName}] {message}");
        }
    }

}
