using System;
using System.Collections.Generic;

namespace LogicSequencer.Script.Helper
{
    public static class DataSourceExtensions
    {
        public static ScriptValue Resolve(this DataSource dataSource, IReadOnlyDictionary<string, ScriptValue> variables)
        {
            if (dataSource.HasVariable)
            {
                if (variables.ContainsKey(dataSource.VariableName))
                    return variables[dataSource.VariableName];
                else if (dataSource.HasValue)
                    return dataSource.Value;
                else
                    throw new ArgumentException("Unable to resolve datasource");
            }
            else
                return dataSource.Value;
        }
    }
}
