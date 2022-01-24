using System;

namespace LogicSequencer.Script.Helper
{
    public static class ScriptValueExtensions
    {
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
    }
}
