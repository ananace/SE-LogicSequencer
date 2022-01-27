using System;
using System.Collections.Generic;
using System.Text;
using LogicSequencer.Script;
using LogicSequencer.Script.Helper;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;

namespace LogicSequencer
{
    public partial class LogicProgramRun
    {
        readonly Dictionary<Type, Func<ScriptCondition, bool>> _ConditionHandlers = new Dictionary<Type, Func<ScriptCondition, bool>>();

        void InitializeConditionHandlers()
        {
            _ConditionHandlers.Add(typeof(Script.Conditions.And), HandleAndCondition);
            _ConditionHandlers.Add(typeof(Script.Conditions.BlockPropertyIs), HandleBlockPropertyIsCondition);
            _ConditionHandlers.Add(typeof(Script.Conditions.Comparison), HandleComparisonCondition);
            _ConditionHandlers.Add(typeof(Script.Conditions.HasVariable), HandleHasVariableCondition);
            _ConditionHandlers.Add(typeof(Script.Conditions.Not), HandleNotCondition);
            _ConditionHandlers.Add(typeof(Script.Conditions.Or), HandleOrCondition);
            _ConditionHandlers.Add(typeof(Script.Conditions.VariableIsTruthy), HandleVariableIsTruthyCondition);
            _ConditionHandlers.Add(typeof(Script.Conditions.VariableIsType), HandleVariableIsTypeCondition);
        }

        bool FulfillsAllConditions(IEnumerable<ScriptCondition> conditions)
        {
            foreach (var condition in conditions)
            {
                var handler = _ConditionHandlers[condition.GetType()];
                if (!handler.Invoke(condition))
                    return false;
            }

            return true;
        }

        bool FulfillsAnyConditions(IEnumerable<ScriptCondition> conditions)
        {
            foreach (var condition in conditions)
            {
                var handler = _ConditionHandlers[condition.GetType()];
                if (handler.Invoke(condition))
                    return true;
            }

            return false;
        }


        bool HandleAndCondition(ScriptCondition condition)
        {
            var realCondition = condition as Script.Conditions.And;

            return FulfillsAllConditions(realCondition.Conditions);
        }

        bool HandleBlockPropertyIsCondition(ScriptCondition condition)
        {
            var realCondition = condition as Script.Conditions.BlockPropertyIs;

            var block = realCondition.Block.Resolve(this);
            if (block == null)
                throw new ArgumentException($"Failed to find a block with the selector {realCondition.Block}");

            var prop = block.GetProperty(realCondition.Property);
            if (prop == null)
                throw new ArgumentException($"Failed to find a property with the name {realCondition.Property}");

            var value = prop.GetScriptValue(block);

            if (realCondition.Type.HasValue && realCondition.Type.Value != value.TypeEnum)
                return false;

            if (realCondition.ToCompare != null &&
                !MathHelper.PerformOperation(MathHelper.OperationType.CompareEqual, realCondition.ToCompare.Resolve(Variables), value).Boolean)
                return false;

            if (realCondition.StoreInVariable != null)
                Variables[realCondition.StoreInVariable] = value;

            return true;
        }

        bool HandleComparisonCondition(ScriptCondition condition)
        {
            var realCondition = condition as Script.Conditions.Comparison;

            var sourceData = realCondition.SourceData.Resolve(Variables);
            var comparisonData = realCondition.ComparisonData.Resolve(Variables);

            return MathHelper.PerformOperation(realCondition.OperationType, sourceData, comparisonData).ConvertToBoolean().Boolean;
        }

        bool HandleHasVariableCondition(ScriptCondition condition)
        {
            var realCondition = condition as Script.Conditions.HasVariable;

            if (!Variables.ContainsKey(realCondition.Variable))
                return false;
            if (realCondition.OfType.HasValue && Variables[realCondition.Variable].TypeEnum != realCondition.OfType.Value)
                return false;

            return true;
        }

        bool HandleNotCondition(ScriptCondition condition)
        {
            var realCondition = condition as Script.Conditions.Not;

            return !FulfillsAllConditions(new[] { realCondition.Condition });
        }

        bool HandleOrCondition(ScriptCondition condition)
        {
            var realCondition = condition as Script.Conditions.Or;

            return FulfillsAnyConditions(realCondition.Conditions);
        }

        bool HandleVariableIsTruthyCondition(ScriptCondition condition)
        {
            var realCondition = condition as Script.Conditions.VariableIsTruthy;

            if (!Variables.ContainsKey(realCondition.Variable))
                throw new ArgumentException($"{realCondition.Variable}: No such variable");

            return Variables[realCondition.Variable].ConvertToBoolean().Boolean;
        }

        bool HandleVariableIsTypeCondition(ScriptCondition condition)
        {
            var realCondition = condition as Script.Conditions.VariableIsType;

            if (!Variables.ContainsKey(realCondition.Variable))
                throw new ArgumentException($"{realCondition.Variable}: No such variable");

            return Variables[realCondition.Variable].TypeEnum == realCondition.Type;
        }
    }
}
