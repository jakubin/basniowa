using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Helpers
{
    /// <summary>
    /// Test fixture setting up an empty in-memory environment.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class EnvironmentFixture : IDisposable
    {
        private TestServer _server;

        private string _dbName;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentFixture"/> class.
        /// </summary>
        public EnvironmentFixture()
        {
            _dbName = Guid.NewGuid().ToString();

            var webHostBuilder = new WebHostBuilder()
                .UseSetting(InMemoryStartup.DbNameSetting, _dbName)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<InMemoryStartup>();
            _server = new TestServer(webHostBuilder);
        }

        /// <summary>
        /// Cleans up the test environment after tests have been executed.
        /// </summary>
        public void Dispose()
        {
            var serviceProvider = new ServiceCollection()
                 .AddEntityFrameworkInMemoryDatabase()
                 .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(_dbName)
                .UseInternalServiceProvider(serviceProvider);

            using (var context = new DbContext(builder.Options))
            {
                context.Database.EnsureDeleted();
            }
        }

        /// <summary>
        /// Creates the HTTP client, that can send messages to test server.
        /// </summary>
        /// <returns>HTTP client.</returns>
        public HttpClient CreateClient()
        {
            return _server.CreateClient();
        }
    }
}
