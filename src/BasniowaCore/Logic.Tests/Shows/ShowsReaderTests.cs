using System;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Database;
using DataAccess.Database.Shows;
using FluentAssertions;
using Logic.Common;
using Logic.Shows;
using Logic.Tests.Helpers;
using Xunit;

namespace Logic.Tests.Shows
{
    public class ShowsReaderTests : IDisposable, IDbTest
    {
        #region Preparation

        public InMemoryDb InMemoryDb { get; set; }

        public ShowsReaderTests()
        {
            InMemoryDb = new InMemoryDb();
        }

        private TheaterDb GetDbContext()
        {
            return InMemoryDb.Create();
        }

        private ShowsReader Create()
        {
            return new ShowsReader
            {
                DbFactory = InMemoryDb
            };
        }

        public void Dispose()
        {
            InMemoryDb.Dispose();
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
            var show = this.AddShow(10);

            var reader = Create();
            var all = reader.GetAllShows();

            all.Should().HaveCount(1);
            all.Select(x => new {Id = x.ShowId, x.Title, x.Subtitle})
                .Should().BeEquivalentTo(new {show.Id, show.Title, show.Subtitle});
        }

        [Fact]
        public void GetAllShows_SkipsDeleted()
        {
            this.AddShow(10, customAction: x => x.IsDeleted = true);

            var reader = Create();
            var all = reader.GetAllShows();

            all.Should().HaveCount(0);
        }

        #endregion

        #region GetShowById tests

        [Fact]
        public void GetShowById_NotFound()
        {
            this.AddShow(10L);

            var reader = Create();
            var ex = Assert.Throws<EntityNotFoundException<Show>>(() => reader.GetShowById(2));

            ex.EntityKey.Should().Be("2");
        }

        [Fact]
        public void GetShowById_Existing()
        {
            var show1 = this.AddShow(10, new [] {100L, 101L});
            this.AddShow(20, new[] {200L, 201L});

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
            var show1 = this.AddShow(10,
                new long[] {100, 101},
                x => x.ShowProperties.First().IsDeleted = true);

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
            this.AddShow(2, customAction: x => x.IsDeleted = true);

            var reader = Create();

            var ex = Assert.Throws<EntityNotFoundException<Show>>(() =>
            {
                reader.GetShowById(2);
            });

            ex.EntityKey.Should().Be("2");
        }

        #endregion

        #region GetShowPictures

        [Fact]
        public async Task GetShowPictures_Empty()
        {
            this.AddShow(1);

            var reader = Create();
            var actualPictures = await reader.GetShowPictures(1);

            actualPictures.Should().BeEmpty();
        }

        [Fact]
        public async Task GetShowPictures_ReturnValidData()
        {
            this.AddShow(1);
            var p1 = this.AddShowPicture(10, 1);
            var p2 = this.AddShowPicture(11, 1);

            var reader = Create();
            var actualPictures = await reader.GetShowPictures(1);

            actualPictures.Select(x => new {x.ShowPictureId, x.ImagePath, x.ThumbPath, x.Title, x.IsMainShowPicture})
                .Should().Equal(
                    new[] {p1, p2}.Select(
                        x => new {ShowPictureId = x.Id, x.ImagePath, x.ThumbPath, x.Title, IsMainShowPicture = false}));
        }

        [Fact]
        public async Task GetShowPictures_OnlyShowOwnPicturesAreReturned()
        {
            this.AddShow(1);
            this.AddShowPicture(10, 1);
            this.AddShowPicture(11, 1);
            this.AddShow(2);
            this.AddShowPicture(20, 2);
            this.AddShowPicture(21, 2);

            var reader = Create();
            var actualPictures = await reader.GetShowPictures(1);

            actualPictures.Select(x => x.ShowPictureId).Should().Equal(10L, 11L);
        }

        [Fact]
        public async Task GetShowPictures_DeletedPicturesNotReturned()
        {
            this.AddShow(1);
            this.AddShowPicture(10, 1);
            this.AddShowPicture(11, 1);
            this.AddShowPicture(12, 1, x => x.IsDeleted = true);

            var reader = Create();
            var actualPictures = await reader.GetShowPictures(1);

            actualPictures.Select(x => x.ShowPictureId).Should().Equal(10L, 11L);
        }

        [Fact]
        public async Task GetShowPictures_MainShowPictureSetCorrectly()
        {
            var show = this.AddShow(1);
            this.AddShowPicture(10, 1);
            this.AddShowPicture(11, 1);
            show.MainShowPictureId = 11;
            this.UpdateShow(show);

            var reader = Create();
            var actualPictures = await reader.GetShowPictures(1);

            actualPictures.Select(x => new {x.ShowPictureId, x.IsMainShowPicture})
                .Should().Equal(
                    new {ShowPictureId = 10L, IsMainShowPicture = false},
                    new {ShowPictureId = 11L, IsMainShowPicture = true});
        }

        [Fact]
        public async Task GetShowPictures_ThrowsWhenShowIsNotExisting()
        {
            var reader = Create();
            await Assert.ThrowsAsync<EntityNotFoundException<Show>>(
                async () => await reader.GetShowPictures(1));
        }

        [Fact]
        public async Task GetShowPictures_ThrowsWhenShowIsDeleted()
        {
            this.AddShow(1, customAction: x => x.IsDeleted = true);

            var reader = Create();
            await Assert.ThrowsAsync<EntityNotFoundException<Show>>(
                async () => await reader.GetShowPictures(1));
        }

        #endregion
    }
}
