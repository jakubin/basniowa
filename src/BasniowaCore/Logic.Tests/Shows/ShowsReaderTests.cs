using System.Collections.Generic;
using System.Linq;
using DataAccess;
using DataAccess.Shows;
using FluentAssertions;
using Logic.Common;
using Logic.Shows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Logic.Tests.Shows
{
    public class ShowsReaderTests
    {
        private readonly DbContextOptions<TheaterDb> _options;

        public ShowsReaderTests()
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

        [Fact(DisplayName = nameof(ShowsReader) + ": GetAllShows() returns correctly empty list")]
        public void GetAllShowsEmpty()
        {
            using (var db = GetDbContext())
            {
                var provider = new ShowsReader(db);

                var all = provider.GetAllShows();
                all.Should().HaveCount(0);
            }
        }

        [Fact(DisplayName = nameof(ShowsReader) + ": GetAllShows() returns correctly more shows.")]
        public void GetShowsHasData()
        {
            using (var db = GetDbContext())
            {
                db.Shows.Add(new Show
                {
                    Id = 1,
                    Title = "Show",
                    Description = "Description1",
                    Subtitle = "Subtitle"
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
                var provider = new ShowsReader(db);

                var all = provider.GetAllShows();
                all.Should().HaveCount(1);
                all.Select(x => new { x.Id, x.Title, x.Subtitle })
                    .Should().BeEquivalentTo(new[]
                    {
                        new { Id = 1L, Title = "Show", Subtitle = "Subtitle" }
                    });
            }
        }

        [Fact(DisplayName = nameof(ShowsReader) + ": GetShowById() should throw when no show was not found.")]
        public void GetShowByIdNotFound()
        {
            using (var db = GetDbContext())
            {
                db.Shows.Add(new Show
                {
                    Id = 1,
                    Title = "Show",
                    Description = "Description1",
                    Subtitle = "Subtitle"
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
                var provider = new ShowsReader(db);

                var ex = Assert.Throws<EntityNotFoundException<ShowWithDetails>>(() =>
                {
                    provider.GetShowById(2);
                });

                ex.EntityKey.Should().Be("Id=2");
            }
        }

        [Fact(DisplayName = nameof(ShowsReader) + ": GetShowById() should correctly return an existing show.")]
        public void GetShowByIdCorrect()
        {
            using (var db = GetDbContext())
            {
                db.Shows.Add(new Show
                {
                    Id = 1,
                    Title = "Show1",
                    Description = "Description1",
                    Subtitle = "Subtitle1"
                });
                db.ShowProperties.Add(new ShowProperty
                {
                    Id = 100,
                    ShowId = 1,
                    Name = "Prop1",
                    Value = "Value1"
                });

                db.Shows.Add(new Show
                {
                    Id = 2,
                    Title = "Show2",
                    Description = "Description2",
                    Subtitle = "Subtitle2"
                });
                db.ShowProperties.Add(new ShowProperty
                {
                    Id = 200,
                    ShowId = 2,
                    Name = "Prop2",
                    Value = "Value2"
                });

                db.SaveChanges();
            }

            using (var db = GetDbContext())
            {
                var provider = new ShowsReader(db);

                var actual = provider.GetShowById(1);

                new { actual.Id, actual.Title, actual.Subtitle, actual.Description }.Should()
                    .Be(new { Id = 1L, Title = "Show1", Subtitle = "Subtitle1", Description = "Description1" });

                actual.Properties.ShouldBeEquivalentTo(new Dictionary<string, string> { ["Prop1"] = "Value1" });
            }
        }
    }
}
