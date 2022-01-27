using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicSequencer.Script.Helper
{
    public static class MathHelper
    {
        public enum OperationType
        {
            _ArithmeticStart,

            // Arithmetic
            Add,
            Subtract,
            Multiply,
            Divide,
            Modulo,

            Comparison, // Spaceship <=> operator

            _ArithmeticEnd,
            _ComparisonStart = _ArithmeticEnd,

            // Comparison
            CompareEqual,
            CompareNotEqual,
            CompareGreaterThan,
            CompareGreaterEqual,
            CompareLesserThan,
            CompareLesserEqual,
            CompareContains,

            _ComparisonEnd,
            _StringStart = _ComparisonEnd,

            StringAdd,
            StringRemove,

            _StringEnd,
            _BooleanStart = _StringEnd,

            // Boolean math
            BooleanAnd,
            BooleanEquivalent,
            BooleanImplication,
            BooleanOr,
            BooleanXor,

            _BooleanEnd,
        }

        public enum SingleObjectOperationType
        {
            _ArithmeticStart,

            // Arithmetic
            Sign,
            Sin,
            Cos,
            Negate,
            Sqrt,

            _ArithmeticEnd,
            _BooleanStart = _ArithmeticEnd,

            // Boolean math
            BooleanNot,

            _BooleanEnd,
            _StringStart = _BooleanEnd,

            StringReverse,

            _StringEnd,
            _TypeCastStart = _StringEnd,

            // Casts
            AsBoolean,
            AsInteger,
            AsReal,
            AsString,

            _TypeCastEnd,
        }

        public static ScriptValue ResolveArithmeticPart(Actions.ArithmeticComplexPart part, LogicProgramRun run)
        {
            if (part.IsSingle)
            {
                var data = part.RHS.DataSource.Resolve(run.Variables);
                return DoOperation(part.SingleOperatorType, data);
            }

            ScriptValue lhs, rhs;

            if (part.LHS.IsData)
                lhs = part.LHS.DataSource.Resolve(run.Variables);
            else
                lhs = ResolveArithmeticPart(part.LHS.Arithmetic, run);

            if (part.RHS.IsData)
                rhs = part.RHS.DataSource.Resolve(run.Variables);
            else
                rhs = ResolveArithmeticPart(part.RHS.Arithmetic, run);

            return PerformOperation(part.OperatorType, lhs, rhs);
        }

        public static ScriptValue DoOperation(OperationType op, IEnumerable<ScriptValue> objects)
        {
            var result = objects.First();
            foreach (var other in objects.Skip(1))
                result = PerformOperation(op, result, other);
            return result;
        }

        public static ScriptValue DoOperation(SingleObjectOperationType op, ScriptValue a)
        {
            return PerformSingleOperation(op, a);
        }

        #region Operation handling

        public static ScriptValue PerformSingleOperation(SingleObjectOperationType op, ScriptValue obj)
        {
            Util.Log.Debug($"Math - Running {op} on {obj}");

            switch (op)
            {
            case SingleObjectOperationType.AsBoolean: return obj.ConvertToBoolean();
            case SingleObjectOperationType.AsInteger: return obj.ConvertToInteger();
            case SingleObjectOperationType.AsReal:    return obj.ConvertToReal();
            case SingleObjectOperationType.AsString:  return obj.ConvertToString();
            }

            if (obj.BooleanSpecified)
                return PerformSingleBooleanOperation(op, obj);
            if (obj.IntegerSpecified)
                return PerformSingleIntegerOperation(op, obj);
            if (obj.RealSpecified)
                return PerformSingleRealOperation(op, obj);
            if (obj.StringSpecified)
                return PerformSingleStringOperation(op, obj);
            throw new ArgumentException($"No single-operation available for {obj.TypeName}");
        }

        public static ScriptValue PerformOperation(OperationType op, ScriptValue objA, ScriptValue objB)
        {
            Util.Log.Debug($"Math - Running {op} on {objA} and {objB}");

            switch (op)
            {
            case OperationType.CompareEqual: return new ScriptValue { Boolean = objA.AsObject == objB.AsObject };
            case OperationType.CompareNotEqual: return new ScriptValue { Boolean = objA.AsObject != objB.AsObject };
            }

            if (objA.BooleanSpecified)
                return PerformBooleanBooleanOperation(op, objA, objB.ConvertToBoolean());
            if (objA.IntegerSpecified)
            {
                if (objB.RealSpecified)
                    return PerformIntegerRealOperation(op, objA, objB);
                return PerformIntegerIntegerOperation(op, objA, objB.ConvertToInteger());
            }
            if (objA.RealSpecified)
                return PerformRealRealOperation(op, objA, objB.ConvertToReal());
            if (objA.StringSpecified)
                return PerformStringStringOperation(op, objA, objB.ConvertToString());
            throw new ArgumentException($"No multi-operation available for {objA.TypeName} and {objB.TypeName}");
        }

        public static ScriptValue PerformSingleBooleanOperation(SingleObjectOperationType op, ScriptValue obj)
        {
            var value = obj.Boolean;
            switch (op)
            {
            case SingleObjectOperationType.BooleanNot: return new ScriptValue { Boolean = !value };
            }
            throw new ArgumentException($"{op} is not a valid operation on a {obj.TypeName}", "op");
        }

        public static ScriptValue PerformSingleIntegerOperation(SingleObjectOperationType op, ScriptValue obj)
        {
            var value = obj.Integer;
            switch (op)
            {
            case SingleObjectOperationType.Cos: return new ScriptValue { Real = Math.Cos(value) };
            case SingleObjectOperationType.Negate: return new ScriptValue { Integer = -value };
            case SingleObjectOperationType.Sign: return new ScriptValue { Integer = value < 0 ? -1 : (value > 0 ? 1 : 0) };
            case SingleObjectOperationType.Sin: return new ScriptValue { Real = Math.Sin(value) };
            case SingleObjectOperationType.Sqrt: return new ScriptValue { Real = Math.Sqrt(value) };
            }
            throw new ArgumentException($"{op} is not a valid operation on a {obj.TypeName}", "op");
        }

        public static ScriptValue PerformSingleRealOperation(SingleObjectOperationType op, ScriptValue obj)
        {
            var value = obj.Real;
            switch (op)
            {
            case SingleObjectOperationType.Cos: return new ScriptValue { Real = Math.Cos(value) };
            case SingleObjectOperationType.Negate: return new ScriptValue { Real = -value };
            case SingleObjectOperationType.Sign: return new ScriptValue { Integer = value < 0 ? -1 : (value > 0 ? 1 : 0) };
            case SingleObjectOperationType.Sin: return new ScriptValue { Real = Math.Sin(value) };
            case SingleObjectOperationType.Sqrt: return new ScriptValue { Real = Math.Sqrt(value) };
            }
            throw new ArgumentException($"{op} is not a valid operation on a {obj.TypeName}", "op");
        }

        public static ScriptValue PerformSingleStringOperation(SingleObjectOperationType op, ScriptValue obj)
        {
            var value = obj.String;
            switch (op)
            {
            case SingleObjectOperationType.StringReverse: return new ScriptValue { String = string.Join("", value.Reverse()) };
            }
            throw new ArgumentException($"{op} is not a valid operation on a {obj.TypeName}", "op");
        }

        // public static ScriptValue PerformBooleanOperation(OperationType op, ScriptValue objA, ScriptValue objB)

        public static ScriptValue PerformBooleanBooleanOperation(OperationType op, ScriptValue objA, ScriptValue objB)
        {
            var valueA = objA.Boolean;
            var valueB = objB.Boolean;
            switch (op)
            {
            case OperationType.BooleanAnd: return new ScriptValue { Boolean = valueA && valueB };
            case OperationType.BooleanEquivalent: return new ScriptValue { Boolean = valueA == valueB };
            case OperationType.BooleanImplication: return new ScriptValue { Boolean = !valueA || valueB };
            case OperationType.BooleanOr: return new ScriptValue { Boolean = valueA || valueB };
            case OperationType.BooleanXor: return new ScriptValue { Boolean = !(valueA == valueB) };
            }
            throw new ArgumentException($"{op} is not a valid operation on a bool and a bool", "op");
        }
        public static ScriptValue PerformIntegerIntegerOperation(OperationType op, ScriptValue objA, ScriptValue objB)
        {
            var valueA = objA.Integer;
            var valueB = objB.Integer;
            switch (op)
            {
            case OperationType.Add: return new ScriptValue { Integer = valueA + valueB };
            case OperationType.CompareGreaterEqual: return new ScriptValue { Boolean = valueA >= valueB };
            case OperationType.CompareGreaterThan: return new ScriptValue { Boolean = valueA > valueB };
            case OperationType.CompareLesserEqual: return new ScriptValue { Boolean = valueA <= valueB };
            case OperationType.CompareLesserThan: return new ScriptValue { Boolean = valueA < valueB };
            case OperationType.CompareNotEqual: return new ScriptValue { Boolean = valueA != valueB };
            case OperationType.Comparison: return new ScriptValue { Integer = valueA.CompareTo(valueB) };
            case OperationType.Divide: return new ScriptValue { Integer = valueA / valueB };
            case OperationType.Modulo: return new ScriptValue { Integer = valueA % valueB };
            case OperationType.Multiply: return new ScriptValue { Integer = valueA * valueB };
            case OperationType.Subtract: return new ScriptValue { Integer = valueA - valueB };
            }
            throw new ArgumentException($"{op} is not a valid operation on a(n) {objA.TypeName} and a(n) {objB.TypeName}", "op");
        }

        public static ScriptValue PerformIntegerRealOperation(OperationType op, ScriptValue objA, ScriptValue objB)
        {
            var valueA = objA.Integer;
            var valueB = objB.Real;
            switch (op)
            {
            case OperationType.Add: return new ScriptValue { Real = valueA + valueB };
            case OperationType.CompareGreaterEqual: return new ScriptValue { Boolean = valueA >= valueB };
            case OperationType.CompareGreaterThan: return new ScriptValue { Boolean = valueA > valueB };
            case OperationType.CompareLesserEqual: return new ScriptValue { Boolean = valueA <= valueB };
            case OperationType.CompareLesserThan: return new ScriptValue { Boolean = valueA < valueB };
            case OperationType.CompareNotEqual: return new ScriptValue { Boolean = valueA != valueB };
            case OperationType.Comparison: return new ScriptValue { Integer = valueA.CompareTo(valueB) };
            case OperationType.Divide: return new ScriptValue { Real = valueA / valueB };
            case OperationType.Modulo: return new ScriptValue { Real = valueA % valueB };
            case OperationType.Multiply: return new ScriptValue { Real = valueA * valueB };
            case OperationType.Subtract: return new ScriptValue { Real = valueA - valueB };
            }
            throw new ArgumentException($"{op} is not a valid operation on a(n) {objA.TypeName} and a(n) {objB.TypeName}", "op");
        }

        public static ScriptValue PerformRealRealOperation(OperationType op, ScriptValue objA, ScriptValue objB)
        {
            var valueA = objA.Real;
            var valueB = objB.Real;
            switch (op)
            {
            case OperationType.Add: return new ScriptValue { Real = valueA + valueB };
            case OperationType.CompareGreaterEqual: return new ScriptValue { Boolean = valueA >= valueB };
            case OperationType.CompareGreaterThan: return new ScriptValue { Boolean = valueA > valueB };
            case OperationType.CompareLesserEqual: return new ScriptValue { Boolean = valueA <= valueB };
            case OperationType.CompareLesserThan: return new ScriptValue { Boolean = valueA < valueB };
            case OperationType.CompareNotEqual: return new ScriptValue { Boolean = valueA != valueB };
            case OperationType.Comparison: return new ScriptValue { Integer = valueA.CompareTo(valueB) };
            case OperationType.Divide: return new ScriptValue { Real = valueA / valueB };
            case OperationType.Modulo: return new ScriptValue { Real = valueA % valueB };
            case OperationType.Multiply: return new ScriptValue { Real = valueA * valueB };
            case OperationType.Subtract: return new ScriptValue { Real = valueA - valueB };
            }
            throw new ArgumentException($"{op} is not a valid operation on a(n) {objA.TypeName} and a(n) {objB.TypeName}", "op");
        }

        public static ScriptValue PerformStringStringOperation(OperationType op, ScriptValue objA, ScriptValue objB)
        {
            var valueA = objA.String;
            var valueB = objB.String;
            switch (op)
            {
            case OperationType.CompareContains: return new ScriptValue { Boolean = valueA.Contains(valueB) };
            case OperationType.StringAdd: return new ScriptValue { String = valueA + valueB };
            case OperationType.StringRemove: return new ScriptValue { String = valueA.Replace(valueB, "") };
            }
            throw new ArgumentException($"{op} is not a valid operation on a(n) {objA.TypeName} and a(n) {objB.TypeName}", "op");
        }

        #endregion
    }
}
