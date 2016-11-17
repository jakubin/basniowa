using Website;
using Microsoft.Extensions.DependencyInjection;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace Tests.Helpers
{
    public class InMemoryStartup : Startup
    {
        public static readonly string DbNameSetting = "InMemoryDbName";

        public InMemoryStartup(IHostingEnvironment env) : base(env)
        {
        }

        protected override void ConfigureDbOptions(DbContextOptionsBuilder<TheaterDb> options)
        {
            var dbName = Configuration[DbNameSetting];
            options.UseInMemoryDatabase(dbName);
        }
    }
}
