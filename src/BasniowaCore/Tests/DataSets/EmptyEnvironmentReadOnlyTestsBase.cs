using Tests.Helpers;
using Xunit;

namespace Tests.DataSets
{
    /// <summary>
    /// Base class for all read-only tests based on an empty environment.
    /// </summary>
    [Collection(EmptyEnvironmentReadOnlyCollection.Name)]
    public abstract class EmptyEnvironmentReadOnlyTestsBase
    {
        /// <summary>
        /// Gets the environment.
        /// </summary>
        public EnvironmentFixture Environment { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyEnvironmentReadOnlyTestsBase"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        public EmptyEnvironmentReadOnlyTestsBase(EnvironmentFixture environment)
        {
            Environment = environment;
        }
    }
}
