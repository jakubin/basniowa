using System;
using System.Linq;
using System.Threading.Tasks;
using Common.FileContainers;
using DataAccess.Database.Shows;
using FluentAssertions;
using Logic.Common;
using Logic.Services;
using Logic.Shows;
using Logic.Tests.Helpers;
using Moq;
using Testing.FileContainers;
using Xunit;

namespace Logic.Tests.Shows
{
    public class DeleteShowPictureHandlerTests : IDisposable
    {
        #region Setup

        private IUniqueIdService IdService { get; set; }

        private TestEventPublisher EventPublisher { get; set; }

        private IDateTimeService DateTimeService { get; set; }

        private IDbContextFactory DbContextFactory { get; set; }

        private InMemoryDb InMemoryDb { get; set; }

        private TestFileContainer FileContainer { get; set; }

        public DeleteShowPictureHandlerTests()
        {
            IdService = new TestUniqueIdService();

            EventPublisher = new TestEventPublisher();

            DateTimeService = new TestDateTimeService();

            InMemoryDb = new InMemoryDb();
            DbContextFactory = InMemoryDb;

            FileContainer = new TestFileContainer();
        }

        public void Dispose()
        {
            InMemoryDb.Dispose();
        }

        public DeleteShowPictureHandler CreateHandler()
        {
            return new DeleteShowPictureHandler()
            {
                EventPublisher = EventPublisher,
                DateTimeService = DateTimeService,
                DbFactory = DbContextFactory,
                ShowPictures = new ShowPicturesFileContainer(FileContainer)
            };
        }

        #endregion

        #region Tests

        [Fact]
        public async Task CorrectlyDeletesShowPicture()
        {
            var originalShow = await PrepareShow(1);
            var originalPicture = await PrepareShowPicture(10, 1);

            var handler = CreateHandler();
            var command = new DeleteShowPictureCommand {ShowPictureId = 10, UserName = "user"};
            await handler.Handle(command);

            var actualPicture = GetShowPictureById(10);
            actualPicture.IsDeleted.Should().Be(true);
            actualPicture.ModifiedBy.Should().Be("user");
            actualPicture.ModifiedUtc.Should().Be(DateTimeService.UtcNow);

            var imageExists = await FileContainer.Exists(originalPicture.ImagePath);
            var thumbExists = await FileContainer.Exists(originalPicture.ThumbPath);
            imageExists.Should().BeFalse();
            thumbExists.Should().BeFalse();

            EventPublisher.PublishedEvents.Should().AllBeOfType<ShowPictureDeletedEvent>().And.HaveCount(1);
            var actualEvent = EventPublisher.PublishedEvents.OfType<ShowPictureDeletedEvent>().Single();
            actualEvent.ShowId.Should().Be(1);
            actualEvent.ShowPictureId.Should().Be(10);

            var actualShow = GetShowById(1);
            actualShow.ModifiedUtc.Should().Be(originalShow.ModifiedUtc, "show should not be changed");
        }

        [Fact]
        public async Task CorrectlyDeletesShowMainPicture()
        {
            var originalShow = await PrepareShow(1);
            var originalPicture = await PrepareShowPicture(10, 1);
            originalShow.MainShowPictureId = 10;
            await UpdateShow(originalShow);

            var handler = CreateHandler();
            var command = new DeleteShowPictureCommand { ShowPictureId = 10, UserName = "user" };
            await handler.Handle(command);

            var actualPicture = GetShowPictureById(10);
            actualPicture.IsDeleted.Should().Be(true);
            actualPicture.ModifiedBy.Should().Be("user");
            actualPicture.ModifiedUtc.Should().Be(DateTimeService.UtcNow);

            var imageExists = await FileContainer.Exists(originalPicture.ImagePath);
            var thumbExists = await FileContainer.Exists(originalPicture.ThumbPath);
            imageExists.Should().BeFalse();
            thumbExists.Should().BeFalse();

            EventPublisher.PublishedEvents.Should().HaveCount(2);
            var actualEvent = EventPublisher.PublishedEvents.OfType<ShowPictureDeletedEvent>().Single();
            actualEvent.ShowId.Should().Be(1);
            actualEvent.ShowPictureId.Should().Be(10);
            var actualShowEvent = EventPublisher.PublishedEvents.OfType<ShowMainPictureSetEvent>().Single();
            actualShowEvent.ShowId.Should().Be(1);
            actualShowEvent.ShowPictureId.Should().Be(null);

            var actualShow = GetShowById(1);
            actualShow.MainShowPictureId.Should().Be(null);
            actualShow.ModifiedUtc.Should().Be(DateTimeService.UtcNow);
            actualShow.ModifiedBy.Should().Be("user");
        }

        [Fact]
        public async Task ThrowsWhenShowPictureDoesNotExist()
        {
            var originalShow = await PrepareShow(1);

            var handler = CreateHandler();
            var command = new DeleteShowPictureCommand { ShowPictureId = 10, UserName = "user" };
            await Assert.ThrowsAsync<EntityNotFoundException<ShowPicture>>(
                async () => await handler.Handle(command));

            EventPublisher.PublishedEvents.Should().BeEmpty();

            var actualShow = GetShowById(1);
            actualShow.ModifiedUtc.Should().Be(originalShow.ModifiedUtc);
        }

        [Fact]
        public async Task IgnoresWhenShowPictureIsAlreadyDeleted()
        {
            await PrepareShow(1);
            var originalPicture = await PrepareShowPicture(10, 1, x => x.IsDeleted = true);

            var handler = CreateHandler();
            var command = new DeleteShowPictureCommand { ShowPictureId = 10, UserName = "user" };
            await handler.Handle(command);

            var actualPicture = GetShowPictureById(10);
            actualPicture.IsDeleted.Should().Be(true);
            actualPicture.ModifiedBy.Should().Be(originalPicture.ModifiedBy);
            actualPicture.ModifiedUtc.Should().Be(originalPicture.ModifiedUtc);

            EventPublisher.PublishedEvents.Should().BeEmpty();
        }

        [Fact]
        public async Task IgnoresFileStoreErrors()
        {
            FileContainer = Mock.Of<TestFileContainer>();
            await PrepareShow(1);
            var originalPicture = await PrepareShowPicture(10, 1);

            Mock.Get(FileContainer)
                .Setup(x => x.RemoveFile(originalPicture.ImagePath))
                .Throws(new FileNotFoundInContainerException(originalPicture.ImagePath));
            Mock.Get(FileContainer)
                .Setup(x => x.RemoveFile(originalPicture.ThumbPath))
                .Throws(new Exception("Kaboom"));

            var handler = CreateHandler();
            var command = new DeleteShowPictureCommand { ShowPictureId = 10, UserName = "user" };
            await handler.Handle(command);

            var actualPicture = GetShowPictureById(10);
            actualPicture.IsDeleted.Should().Be(true);
            actualPicture.ModifiedBy.Should().Be("user");
            actualPicture.ModifiedUtc.Should().Be(DateTimeService.UtcNow);

            EventPublisher.PublishedEvents.Should().AllBeOfType<ShowPictureDeletedEvent>().And.HaveCount(1);
            var actualEvent = EventPublisher.PublishedEvents.OfType<ShowPictureDeletedEvent>().Single();
            actualEvent.ShowId.Should().Be(1);
            actualEvent.ShowPictureId.Should().Be(10);
        }

        #endregion

        #region Helpers

        private async Task<Show> PrepareShow(long showId, Action<Show> customActions = null)
        {
            var db = InMemoryDb.Create();
            var show = TestData.CreateShow(showId, new long[0]);
            customActions?.Invoke(show);

            db.Shows.Add(show);

            await db.SaveChangesAsync();

            return show;
        }

        private async Task<ShowPicture> PrepareShowPicture(long showPictureId, long showId, Action<ShowPicture> customActions = null)
        {
            var db = InMemoryDb.Create();
            var showPicture = TestData.CreateShowPicture(showPictureId, showId);
            customActions?.Invoke(showPicture);

            db.ShowPictures.Add(showPicture);

            await db.SaveChangesAsync();

            await FileContainer.AddFile(showPicture.ImagePath, "File content");
            await FileContainer.AddFile(showPicture.ThumbPath, "File content");

            return showPicture;
        }

        private Show GetShowById(long showId)
        {
            var db = InMemoryDb.Create(false);
            return db.Shows.FirstOrDefault(x => x.Id == showId);
        }

        private ShowPicture GetShowPictureById(long id)
        {
            var db = InMemoryDb.Create(false);
            return db.ShowPictures.FirstOrDefault(x => x.Id == id);
        }

        private async Task UpdateShow(Show show)
        {
            var db = InMemoryDb.Create();

            db.Shows.Update(show);

            await db.SaveChangesAsync();
        }

        #endregion
    }
}