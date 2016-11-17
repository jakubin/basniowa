namespace Common.Tests.Startup
{
    /// <summary>
    /// Simple class with startup method.
    /// </summary>
    public static class MultipleStartup1
    {
        /// <summary>
        /// The number of times <see cref="StartupMethod"/> was called.
        /// </summary>
        public static int StartupMethodCalls = 0;

        /// <summary>
        /// Startup method.
        /// </summary>
        [MultipleStartup]
        public static void StartupMethod()
        {
            StartupMethodCalls++;
        }

        /// <summary>
        /// Resets the call counters.
        /// </summary>
        public static void Reset()
        {
            StartupMethodCalls = 0;
        }
    }
}
