namespace Tests.Helpers
{
    /// <summary>
    /// Interface for classes that can perform environment initialization
    /// (set up data, etc.).
    /// </summary>
    public interface IEnvironmentInitializer
    {
        /// <summary>
        /// Initializes the specified environment.
        /// </summary>
        /// <param name="environment">The environment.</param>
        void Initialize(EnvironmentFixture environment);
    }
}