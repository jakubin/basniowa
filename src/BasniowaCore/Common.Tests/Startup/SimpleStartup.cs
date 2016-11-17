namespace Common.Tests.Startup
{
    /// <summary>
    /// Simple class with startup method.
    /// </summary>
    public static class SimpleStartup
    {
        /// <summary>
        /// The number of times <see cref="StartupMethod"/> was called.
        /// </summary>
        public static int StartupMethodCalls = 0;

        /// <summary>
        /// The number of times <see cref="OtherMethod"/> was called.
        /// </summary>
        public static int OtherMethodCalls = 0;

        /// <summary>
        /// Startup method.
        /// </summary>
        [SimpleStartup]
        public static void StartupMethod()
        {
            StartupMethodCalls++;
        }

        /// <summary>
        /// Method not being startup logic.
        /// </summary>
        public static void OtherMethod()
        {
            OtherMethodCalls++;
        }

        /// <summary>
        /// Resets the call counters.
        /// </summary>
        public static void Reset()
        {
            StartupMethodCalls = 0;
            OtherMethodCalls = 0;
        }
    }
}
