using DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Tests.Helpers
{
    public class InMemoryEnvironment : IDisposable
    {
        public TestServer Server { get; private set; }

        public InMemoryEnvironment()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<InMemoryStartup>();
            Server = new TestServer(webHostBuilder);
        }

        public TheaterDb CreateTheaterDb()
        {
            return Server.Host.Services.GetService<TheaterDb>();
        }

        public void Dispose()
        {
            CreateTheaterDb().Database.EnsureDeleted();
        }
    }
}
