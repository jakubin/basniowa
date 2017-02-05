using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using DataAccess.Shows;
using FluentAssertions;
using Logic.Common;
using Logic.Shows;
using Logic.Tests.Helpers;
using Xunit;

namespace Logic.Tests.Shows
{
    public class ShowsReaderTests : IDisposable
    {
        #region Preparation

        private readonly InMemoryDb _inMemoryDb;

        public ShowsReaderTests()
        {
            _inMemoryDb = new InMemoryDb();
        }

        private TheaterDb GetDbContext()
        {
            return _inMemoryDb.Create();
        }

        private ShowsReader Create()
        {
            return new ShowsReader
            {
                DbFactory = _inMemoryDb
            };
        }

        public void Dispose()
        {
            _inMemoryDb.Dispose();
        }

        #endregion

        #region GetAllShows tests

        [Fact]
        public void GetAllShows_Empty()
        {
            var reader = Create();
            var all = reader.GetAllShows();
            all.Should().HaveCount(0);
        }

        [Fact]
        public void GetAllShows_ReturnsShows()
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

            var reader = Create();

            var all = reader.GetAllShows();
            all.Should().HaveCount(1);
            all.Select(x => new { x.Id, x.Title, x.Subtitle })
                .Should().BeEquivalentTo(new { Id = 1L, Title = "Show", Subtitle = "Subtitle" });
        }

        [Fact]
        public void GetAllShows_SkipsDeleted()
        {
            using (var db = GetDbContext())
            {
                db.Shows.Add(new Show
                {
                    Id = 1,
                    Title = "Show",
                    Description = "Description1",
                    Subtitle = "Subtitle",
                    IsDeleted = true
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

            var reader = Create();

            var all = reader.GetAllShows();
            all.Should().HaveCount(0);
        }

        #endregion

        #region GetShowById tests

        [Fact]
        public void GetShowById_NotFound()
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

            var reader = Create();

            var ex = Assert.Throws<EntityNotFoundException<ShowWithDetails>>(() =>
            {
                reader.GetShowById(2);
            });

            ex.EntityKey.Should().Be("Id=2");
        }

        [Fact]
        public void GetShowById_Existing()
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

            var reader = Create();

            var actual = reader.GetShowById(1);

            new { actual.Id, actual.Title, actual.Subtitle, actual.Description }.Should()
                .Be(new { Id = 1L, Title = "Show1", Subtitle = "Subtitle1", Description = "Description1" });

            actual.Properties.ShouldBeEquivalentTo(new Dictionary<string, string> { ["Prop1"] = "Value1" });
        }

        [Fact]
        public void GetShowById_Deleted()
        {
            using (var db = GetDbContext())
            {
                db.Shows.Add(new Show
                {
                    Id = 2,
                    Title = "Show",
                    Description = "Description1",
                    Subtitle = "Subtitle",
                    IsDeleted = true
                });

                db.SaveChanges();
            }

            var reader = Create();

            var ex = Assert.Throws<EntityNotFoundException<ShowWithDetails>>(() =>
            {
                reader.GetShowById(2);
            });

            ex.EntityKey.Should().Be("Id=2");
        }

        #endregion
    }
}
