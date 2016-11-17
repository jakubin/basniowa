using Tests.Common;
using Tests.Helpers;

namespace Tests.DataSets.Sample
{
    /// <summary>
    /// Initializes test environment with sample data set similar to production.
    /// </summary>
    /// <seealso cref="Tests.Helpers.IEnvironmentInitializer" />
    public class SampleInitializer : IEnvironmentInitializer
    {
        private static readonly string BasePath = "DataSets/Sample";

        /// <inheritdoc/>
        public void Initialize(EnvironmentFixture environment)
        {
            using (var client = environment.CreateClient())
            {
                // authenticate
                client.AuthenticateAsDefaultAdmin();
            }
        }
    }
}
