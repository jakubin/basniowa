using DataAccess;
using DataAccess.Shows;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Logic.Tests
{
    public class ShowsProviderTests
    {
        private readonly DbContextOptions<TheaterDb> _options;

        public ShowsProviderTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<TheaterDb>()
                .UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider);

            _options = optionsBuilder.Options;
        }

        private TheaterDb GetDbContext()
        {
            return new TheaterDb(_options);
        }

        [Fact]
        public void Returns_Empty_When_No_Shows()
        {
            using (var db = GetDbContext())
            {
                var provider = new ShowsProvider(db);

                var all = provider.GetAllShows();
                all.Should().HaveCount(0);
            }
        }

        [Fact]
        public void Returns_All_Shows()
        {
            using (var db = GetDbContext())
            {
                db.Shows.Add(new Show
                {
                    Id = 1,
                    Title = "Show",
                    Description = "Description1"
                });
                db.ShowProperties.Add(new ShowProperty
                {
                    Id = 100,
                    ShowId = 1,
                    Name = "Prop1",
                    Value = "Value1"
                });

                db.SaveChanges();
            }

            using (var db = GetDbContext())
            {
                var provider = new ShowsProvider(db);

                var all = provider.GetAllShows();
                all.Should().HaveCount(1);
            }
        }
    }
}
