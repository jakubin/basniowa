using System;
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
            var show = TestData.CreateShow(10);
            using (var db = GetDbContext())
            {
                db.Shows.Add(show);
                db.SaveChanges();
            }

            var reader = Create();

            var all = reader.GetAllShows();
            all.Should().HaveCount(1);
            all.Select(x => new {Id = x.ShowId, x.Title, x.Subtitle})
                .Should().BeEquivalentTo(new {show.Id, show.Title, show.Subtitle});
        }

        [Fact]
        public void GetAllShows_SkipsDeleted()
        {
            var show = TestData.CreateShow(10);
            show.IsDeleted = true;
            using (var db = GetDbContext())
            {
                db.Shows.Add(show);
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
                db.Shows.Add(TestData.CreateShow(10L));

                db.SaveChanges();
            }

            var reader = Create();

            var ex = Assert.Throws<EntityNotFoundException<Show>>(() =>
            {
                reader.GetShowById(2);
            });

            ex.EntityKey.Should().Be("2");
        }

        [Fact]
        public void GetShowById_Existing()
        {
            var show1 = TestData.CreateShow(10);
            var show2 = TestData.CreateShow(20);
            using (var db = GetDbContext())
            {
                db.Shows.AddRange(show1, show2);
                db.SaveChanges();
            }

            var reader = Create();

            var actual = reader.GetShowById(10);

            actual.ShowId.Should().Be(show1.Id);
            actual.Title.Should().Be(show1.Title);
            actual.Subtitle.Should().Be(show1.Subtitle);
            actual.Description.Should().Be(show1.Description);

            actual.Properties.ShouldBeEquivalentTo(show1.ShowProperties.ToDictionary(x => x.Name, x => x.Value));
        }

        [Fact]
        public void GetShowById_ExistingWithDeletedProperties()
        {
            var show1 = TestData.CreateShow(10);
            show1.ShowProperties.First().IsDeleted = true;
            using (var db = GetDbContext())
            {
                db.Shows.Add(show1);
                db.SaveChanges();
            }

            var reader = Create();

            var actual = reader.GetShowById(10);

            actual.ShowId.Should().Be(show1.Id);
            actual.Title.Should().Be(show1.Title);
            actual.Subtitle.Should().Be(show1.Subtitle);
            actual.Description.Should().Be(show1.Description);

            actual.Properties.ShouldBeEquivalentTo(
                show1.ShowProperties.Where(x => !x.IsDeleted).ToDictionary(x => x.Name, x => x.Value));
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

            var ex = Assert.Throws<EntityNotFoundException<Show>>(() =>
            {
                reader.GetShowById(2);
            });

            ex.EntityKey.Should().Be("2");
        }

        #endregion
    }
}
