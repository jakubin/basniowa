using System;
using System.Net.Http;

namespace Tests.Helpers
{
    /// <summary>
    /// Interface for testing environment.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ITestEnvironment : IDisposable
    {
        /// <summary>
        /// Creates the HTTP client, that can send messages to test server.
        /// </summary>
        /// <remarks>
        /// The caller is expected to dispose the returned <see cref="HttpClient"/>.
        /// </remarks>
        HttpClient CreateClient();

        /// <summary>
        /// Gets the or creates a container for specific scenario context data,
        /// so that they can be exchanged between scenario steps.
        /// </summary>
        /// <typeparam name="T">Type storing specific scenario context data.</typeparam>
        /// <returns>Ex</returns>
        T GetOrCreateScenarioData<T>() where T : new();
    }
}
