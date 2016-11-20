namespace Tests.Helpers
{
    /// <summary>
    /// Test fixture setting up an in-memory environment pre-initialized using initializer <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Class that will perform the environment initialization.</typeparam>
    public class EnvironmentFixture<T> : EnvironmentFixture
        where T : IEnvironmentInitializer, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentFixture{T}"/> class.
        /// </summary>
        public EnvironmentFixture()
        {
            var initializer = new T();
            initializer.Initialize(this);
        }
    }
}
