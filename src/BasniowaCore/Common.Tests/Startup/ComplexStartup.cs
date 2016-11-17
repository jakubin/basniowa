using System;
using System.Collections.Generic;

namespace Common.Tests.Startup
{
    /// <summary>
    /// Test class with startup method using <see cref="ComplexStartupAttribute"/>.
    /// </summary>
    public static class ComplexStartup
    {
        /// <summary>
        /// The calls of <see cref="StartupMethod"/>.
        /// </summary>
        public static readonly List<Tuple<string, int, IComplexUserType>> StartupMethodCalls = new List<Tuple<string, int, IComplexUserType>>();

        /// <summary>
        /// The number of times <see cref="OtherMethod"/> was called.
        /// </summary>
        public static int OtherMethodCalls = 0;

        /// <summary>
        /// Startup method.
        /// </summary>
        [ComplexStartup]
        public static void StartupMethod(string name, int id, IComplexUserType userType)
        {
            StartupMethodCalls.Add(new Tuple<string, int, IComplexUserType>(name, id, userType));
        }

        /// <summary>
        /// Method not being startup logic.
        /// </summary>
        public static void OtherMethod(string name, int id, IComplexUserType userType)
        {
            OtherMethodCalls++;
        }

        /// <summary>
        /// Resets the call counters.
        /// </summary>
        public static void Reset()
        {
            StartupMethodCalls.Clear();
            OtherMethodCalls = 0;
        }
    }
}
