﻿namespace Common.Tests.Startup
{
    public static class MultipleStartup1
    {
        public static int StartupMethodCalls { get; set; } = 0;

        [MultipleStartup]
        public static void StartupMethod()
        {
            StartupMethodCalls++;
        }

        public static void Reset()
        {
            StartupMethodCalls = 0;
        }
    }
}
