using System;
using System.Text;
using Sandbox.ModAPI.Interfaces;

namespace LogicSequencer.Script.Helper
{
    public static class ITerminalPropertyExtensions
    {
        public static ScriptValue GetScriptValue(this ITerminalProperty prop, Sandbox.ModAPI.Ingame.IMyTerminalBlock block)
        {
            if (prop.Is<bool>())
                return new ScriptValue { Boolean = prop.Cast<bool>().GetValue(block) };
            else if (prop.Is<float>())
                return new ScriptValue { Real = prop.Cast<float>().GetValue(block) };
            else if (prop.Is<double>())
                return new ScriptValue { Real = prop.Cast<double>().GetValue(block) };
            else if (prop.Is<VRage.MyFixedPoint>())
                return new ScriptValue { Real = (double)prop.Cast<VRage.MyFixedPoint>().GetValue(block) };
            else if (prop.Is<decimal>())
                return new ScriptValue { Real = (double)prop.Cast<decimal>().GetValue(block) };
            else if (prop.Is<char>())
                return new ScriptValue { Integer = prop.Cast<char>().GetValue(block) };
            else if (prop.Is<short>())
                return new ScriptValue { Integer = prop.Cast<short>().GetValue(block) };
            else if (prop.Is<int>())
                return new ScriptValue { Integer = prop.Cast<int>().GetValue(block) };
            else if (prop.Is<long>())
                return new ScriptValue { Integer = prop.Cast<long>().GetValue(block) };
            else if (prop.Is<byte>())
                return new ScriptValue { Integer = prop.Cast<byte>().GetValue(block) };
            else if (prop.Is<ushort>())
                return new ScriptValue { Integer = prop.Cast<ushort>().GetValue(block) };
            else if (prop.Is<uint>())
                return new ScriptValue { Integer = prop.Cast<uint>().GetValue(block) };
            else if (prop.Is<ulong>())
                return new ScriptValue { Integer = (long)prop.Cast<ulong>().GetValue(block) };
            else if (prop.Is<string>())
                return new ScriptValue { String = prop.Cast<string>().GetValue(block) };
            else if (prop.Is<StringBuilder>())
                return new ScriptValue { String =  prop.Cast<StringBuilder>().GetValue(block).ToString() };
            else
                throw new ArgumentException($"Unable to handle property of type {prop.TypeName}");
        }

        public static void SetScriptValue(this ITerminalProperty prop, Sandbox.ModAPI.Ingame.IMyTerminalBlock block, ScriptValue value)
        {
            if (prop.Is<bool>())
                prop.Cast<bool>().SetValue(block, value.ConvertToBoolean().Boolean);
            else if (prop.Is<float>())
                prop.Cast<float>().SetValue(block, (float)value.ConvertToReal().Real);
            else if (prop.Is<double>())
                prop.Cast<double>().SetValue(block, value.ConvertToReal().Real);
            else if (prop.Is<VRage.MyFixedPoint>())
                prop.Cast<VRage.MyFixedPoint>().SetValue(block, (VRage.MyFixedPoint)value.ConvertToReal().Real);
            else if (prop.Is<decimal>())
                prop.Cast<decimal>().SetValue(block, (decimal)value.ConvertToReal().Real);
            else if (prop.Is<char>())
                prop.Cast<char>().SetValue(block, (char)value.ConvertToInteger().Integer);
            else if (prop.Is<short>())
                prop.Cast<short>().SetValue(block, (short)value.ConvertToInteger().Integer);
            else if (prop.Is<int>())
                prop.Cast<int>().SetValue(block, (int)value.ConvertToInteger().Integer);
            else if (prop.Is<long>())
                prop.Cast<long>().SetValue(block, value.ConvertToInteger().Integer);
            else if (prop.Is<byte>())
                prop.Cast<byte>().SetValue(block, (byte)value.ConvertToInteger().Integer);
            else if (prop.Is<ushort>())
                prop.Cast<ushort>().SetValue(block, (ushort)value.ConvertToInteger().Integer);
            else if (prop.Is<uint>())
                prop.Cast<uint>().SetValue(block, (uint)value.ConvertToInteger().Integer);
            else if (prop.Is<ulong>())
                prop.Cast<ulong>().SetValue(block, (ulong)value.ConvertToInteger().Integer);
            else if (prop.Is<string>())
                prop.Cast<string>().SetValue(block, value.ConvertToString().String);
            else if (prop.Is<StringBuilder>())
                prop.Cast<StringBuilder>().SetValue(block, new StringBuilder(value.ConvertToString().String));
            else
                throw new ArgumentException($"Unable to handle property of type {prop.TypeName}");
        }
    }
}
