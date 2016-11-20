using System;
using System.Collections.Generic;

namespace Common.Tests.Startup
{
    public static class ComplexStartup
    {
        public static List<Tuple<string, int, IComplexUserType>> StartupMethodCalls { get; } = 
            new List<Tuple<string, int, IComplexUserType>>();

        public static int OtherMethodCalls { get; set; } = 0;

        [ComplexStartup]
        public static void StartupMethod(string name, int id, IComplexUserType userType)
        {
            StartupMethodCalls.Add(new Tuple<string, int, IComplexUserType>(name, id, userType));
        }

        public static void OtherMethod(string name, int id, IComplexUserType userType)
        {
            OtherMethodCalls++;
        }

        public static void Reset()
        {
            StartupMethodCalls.Clear();
            OtherMethodCalls = 0;
        }
    }
}
