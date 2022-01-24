using System;
using System.Text;

namespace LogicSequencer.Script.Helper
{
    public static class VariableTypeExtensions
    {
        public static VariableType GetTypeFor(Type type)
        {
            if (type == typeof(bool))
                return VariableType.Boolean;
            else if (type == typeof(float))
                return VariableType.Real;
            else if (type == typeof(double))
                return VariableType.Real;
            else if (type == typeof(VRage.MyFixedPoint))
                return VariableType.Real;
            else if (type == typeof(decimal))
                return VariableType.Real;
            else if (type == typeof(char))
                return VariableType.Integer;
            else if (type == typeof(short))
                return VariableType.Integer;
            else if (type == typeof(int))
                return VariableType.Integer;
            else if (type == typeof(long))
                return VariableType.Integer;
            else if (type == typeof(byte))
                return VariableType.Integer;
            else if (type == typeof(ushort))
                return VariableType.Integer;
            else if (type == typeof(uint))
                return VariableType.Integer;
            else if (type == typeof(ulong))
                return VariableType.Integer;
            else if (type == typeof(string))
                return VariableType.String;
            else if (type == typeof(StringBuilder))
                return VariableType.String;
            else
                throw new ArgumentException($"Unable to convert {type.Name} to script value type");
        }
    }
}
