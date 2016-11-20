namespace Common.Tests.Startup
{
    public static class SimpleStartup
    {
        public static int StartupMethodCalls { get; set; } = 0;

        public static int OtherMethodCalls { get; set; } = 0;

        [SimpleStartup]
        public static void StartupMethod()
        {
            StartupMethodCalls++;
        }

        public static void OtherMethod()
        {
            OtherMethodCalls++;
        }

        public static void Reset()
        {
            StartupMethodCalls = 0;
            OtherMethodCalls = 0;
        }
    }
}
