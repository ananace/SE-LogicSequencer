using System.Collections.Generic;

namespace LogicSequencer.Script.Helper
{
    public static class ArithmeticComponentExtensions
    {
        public static ScriptValue Resolve(this ArithmeticComponent part, IReadOnlyDictionary<string, ScriptValue> variables)
        {
            if (part.IsSingle)
            {
                var data = part.RHS.Resolve(variables);
                return MathHelper.PerformSingleOperation(part.SingleOperatorType, data);
            }
            else
            {
                var lhs = part.LHS.Resolve(variables);
                var rhs = part.RHS.Resolve(variables);

                return MathHelper.PerformOperation(part.OperatorType, lhs, rhs);
            }
        }
    }
}
