using DataAccess;
using DataAccess.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Website;

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
