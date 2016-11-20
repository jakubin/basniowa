using Tests.Helpers;
using Xunit;

namespace Tests.DataSets
{
    /// <summary>
    /// This class has no code, and is never created. Its purpose is simply
    /// to be the place to apply [CollectionDefinition] and all the
    /// <see cref="ICollectionFixture{TFixture}"/> interfaces.
    /// </summary>
    /// <seealso cref="Xunit.ICollectionFixture{EnvironmentFixture}" />
    [CollectionDefinition(Name)]
    public class EmptyEnvironmentReadOnlyCollection : ICollectionFixture<EnvironmentFixture>
    {
        /// <summary>
        /// The name of collection.
        /// </summary>
        public const string Name = nameof(EmptyEnvironmentReadOnlyCollection);
    }
}
