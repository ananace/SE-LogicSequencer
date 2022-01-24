using System;
using System.Text;

namespace LogicSequencer.Script.Helper
{
    public static class ScriptValueExtensions
    {
        public static void SetFromObject(this ScriptValue obj, object value)
        {
            if (value is bool)
                obj.Boolean = (bool)value;
            else if (value is float)
                obj.Real = (float)value;
            else if (value is double)
                obj.Real = (double)value;
            else if (value is VRage.MyFixedPoint)
                obj.Real = (double)(VRage.MyFixedPoint)value;
            else if (value is decimal)
                obj.Real = (double)(decimal)value;
            else if (value is char)
                obj.Integer = (char)value;
            else if (value is short)
                obj.Integer = (short)value;
            else if (value is int)
                obj.Integer = (int)value;
            else if (value is long)
                obj.Integer = (long)value;
            else if (value is byte)
                obj.Integer = (byte)value;
            else if (value is ushort)
                obj.Integer = (ushort)value;
            else if (value is uint)
                obj.Integer = (uint)value;
            else if (value is ulong)
                obj.Integer = (long)(ulong)value;
            else if (value is string)
                obj.String = (string)value;
            else if (value is StringBuilder)
                obj.String = ((StringBuilder)value).ToString();
            else
                throw new ArgumentException($"Unable to convert {value.GetType().Name} to a script value");
        }

        public static object GetAsObject(this ScriptValue obj, Type type)
        {
            if (type == typeof(bool))
                return obj.ConvertToBoolean().Boolean;
            else if (type == typeof(float))
                return (float)obj.ConvertToReal().Real;
            else if (type == typeof(double))
                return obj.ConvertToReal().Real;
            else if (type == typeof(VRage.MyFixedPoint))
                return (VRage.MyFixedPoint)obj.ConvertToReal().Real;
            else if (type == typeof(decimal))
                return (decimal)obj.ConvertToReal().Real;
            else if (type == typeof(char))
                return (char)obj.ConvertToInteger().Integer;
            else if (type == typeof(short))
                return (short)obj.ConvertToInteger().Integer;
            else if (type == typeof(int))
                return (int)obj.ConvertToInteger().Integer;
            else if (type == typeof(long))
                return obj.ConvertToInteger().Integer;
            else if (type == typeof(byte))
                return (byte)obj.ConvertToInteger().Integer;
            else if (type == typeof(ushort))
                return (ushort)obj.ConvertToInteger().Integer;
            else if (type == typeof(uint))
                return (uint)obj.ConvertToInteger().Integer;
            else if (type == typeof(ulong))
                return (ulong)obj.ConvertToInteger().Integer;
            else if (type == typeof(string))
                return obj.ConvertToString().String;
            else if (type == typeof(StringBuilder))
                return new StringBuilder(obj.ConvertToString().String);
            else
                throw new ArgumentException($"Unable to convert script value to {type.Name}");
        }

        public static T GetAsObject<T>(this ScriptValue obj)
        {
            return (T)GetAsObject(obj, typeof(T));
        }

        public static ScriptValue ConvertToBoolean(this ScriptValue obj)
        {
            if (obj.BooleanSpecified)
                return obj;
            else if (obj.IntegerSpecified)
                return new ScriptValue { Boolean = obj.Integer != 0 };
            else if (obj.RealSpecified)
                return new ScriptValue { Boolean = obj.Real != 0 };
            else if (obj.StringSpecified)
                return new ScriptValue { Boolean = !string.IsNullOrEmpty(obj.String) };
            throw new ArgumentException($"{obj.TypeName} can't be handled as a bool", "obj");
        }

        public static ScriptValue ConvertToInteger(this ScriptValue obj)
        {
            if (obj.BooleanSpecified)
                return new ScriptValue { Integer = obj.Boolean ? 1 : 0 };
            else if (obj.IntegerSpecified)
                return obj;
            else if (obj.RealSpecified)
                return new ScriptValue { Integer = (long)obj.Real };
            else if (obj.StringSpecified)
                return new ScriptValue { Integer = int.Parse(obj.String) };
            throw new ArgumentException($"{obj.TypeName} can't be handled as a long", "obj");
        }

        public static ScriptValue ConvertToReal(this ScriptValue obj)
        {
            if (obj.BooleanSpecified)
                return new ScriptValue { Real = obj.Boolean ? 1 : 0 };
            else if (obj.IntegerSpecified)
                return new ScriptValue { Real = obj.Integer };
            else if (obj.RealSpecified)
                return obj;
            else if (obj.StringSpecified)
                return new ScriptValue { Real = double.Parse(obj.String) };
            throw new ArgumentException($"{obj.TypeName} can't be handled as a double", "obj");
        }

        public static ScriptValue ConvertToString(this ScriptValue obj)
        {
            if (obj.BooleanSpecified)
                return new ScriptValue { String = obj.Boolean.ToString() };
            else if (obj.IntegerSpecified)
                return new ScriptValue { String = obj.Integer.ToString() };
            else if (obj.RealSpecified)
                return new ScriptValue { String = obj.Real.ToString() };
            else if (obj.StringSpecified)
                return obj;
            throw new ArgumentException($"{obj.TypeName} can't be handled as a string", "obj");
        }

        public static ScriptValue ConvertToScriptType(this ScriptValue obj, VariableType type)
        {
            switch (type)
            {
            case VariableType.Boolean: return obj.ConvertToBoolean();
            case VariableType.Integer: return obj.ConvertToInteger();
            case VariableType.Real: return obj.ConvertToReal();
            case VariableType.String: return obj.ConvertToString();
            }
            throw new ArgumentException($"Unknown conversion {type}");
        }
    }
}
