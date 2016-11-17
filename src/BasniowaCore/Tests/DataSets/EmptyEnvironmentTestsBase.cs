using Tests.Helpers;
using Xunit;

namespace Tests.DataSets
{
    /// <summary>
    /// Base class for all tests based on an empty environment.
    /// Environment is recreated for each test.
    /// </summary>
    public abstract class EmptyEnvironmentTestsBase : IClassFixture<EnvironmentFixture>
    {
        /// <summary>
        /// Gets the environment.
        /// </summary>
        public EnvironmentFixture Environment { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyEnvironmentTestsBase"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        public EmptyEnvironmentTestsBase(EnvironmentFixture environment)
        {
            Environment = environment;
        }
    }
}
