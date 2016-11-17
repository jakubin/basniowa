using System;
using System.Collections.Generic;

namespace Tests.Helpers
{
    /// <summary>
    /// Stores shared context between steps of a test scenario.
    /// </summary>
    public class TestScenarioContext
    {
        private Dictionary<Type, object> _data;

        /// <summary>
        /// Gets (if exists) or creates a new container for specific data.
        /// </summary>
        /// <typeparam name="T">Type storing specific scenario context data.</typeparam>
        /// <returns>Data container.</returns>
        public T GetOrCreate<T>() 
            where T : new()
        {
            var type = typeof(T);
            object value;
            if (!_data.TryGetValue(type, out value))
            {
                value = new T();
                _data[type] = value;
            }

            return (T)value;
        }
    }
}
