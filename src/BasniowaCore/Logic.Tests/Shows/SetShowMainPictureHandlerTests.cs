using System;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Database.Shows;
using FluentAssertions;
using Logic.Common;
using Logic.Common.BusinessRules;
using Logic.Services;
using Logic.Shows;
using Logic.Tests.Helpers;
using Xunit;

namespace Logic.Tests.Shows
{
    public class SetShowMainPictureHandlerTests
    {
        #region Setup

        private TestEventPublisher EventPublisher { get; set; }

        private IDateTimeService DateTimeService { get; set; }

        private IDbContextFactory DbContextFactory { get; set; }

        private InMemoryDb InMemoryDb { get; set; }

        public SetShowMainPictureHandlerTests()
        {
            EventPublisher = new TestEventPublisher();

            DateTimeService = new TestDateTimeService();

            InMemoryDb = new InMemoryDb();
            DbContextFactory = InMemoryDb;
        }

        public void Dispose()
        {
            InMemoryDb.Dispose();
        }

        public SetShowMainPictureHandler CreateHandler()
        {
            return new SetShowMainPictureHandler()
            {
                EventPublisher = EventPublisher,
                DateTimeService = DateTimeService,
                DbFactory = DbContextFactory,
            };
        }

        #endregion

        #region Tests

        [Fact]
        public async Task ShowHasNoMainPicture()
        {
            // arrange
            var originalShow = await PrepareShow(1, x => x.MainShowPictureId = null);
            await PrepareShowPicture(10, 1);
            await PrepareShowPicture(12, 1);

            var command = new SetShowMainPictureCommand {ShowId = 1, ShowPictureId = 10, UserName = "user"};
            var handler = CreateHandler();

            // act
            await handler.Handle(command);

            // assert
            var actualShow = GetShowById(1);
            actualShow.MainShowPictureId.Should().Be(10);
            actualShow.IsDeleted.Should().Be(false);
            actualShow.CreatedBy.Should().Be(originalShow.CreatedBy);
            actualShow.CreatedUtc.Should().Be(originalShow.CreatedUtc);
            actualShow.ModifiedBy.Should().Be("user");
            actualShow.ModifiedUtc.Should().Be(DateTimeService.UtcNow);

            EventPublisher.PublishedEvents.Should().HaveCount(1);
            var actualEvent = EventPublisher.PublishedEvents.OfType<ShowMainPictureSetEvent>().Single();
            actualEvent.ShowId.Should().Be(1);
            actualEvent.ShowPictureId.Should().Be(10);
        }

        [Fact]
        public async Task ShowHasDifferentMainPicture()
        {
            // arrange
            var originalShow = await PrepareShow(1);
            await PrepareShowPicture(10, 1);
            await PrepareShowPicture(11, 1);
            originalShow.MainShowPictureId = 11;
            await UpdateShow(originalShow);

            var command = new SetShowMainPictureCommand {ShowId = 1, ShowPictureId = 10, UserName = "user"};
            var handler = CreateHandler();

            // act
            await handler.Handle(command);

            // assert
            var actualShow = GetShowById(1);
            actualShow.MainShowPictureId.Should().Be(10);
            actualShow.IsDeleted.Should().Be(false);
            actualShow.CreatedBy.Should().Be(originalShow.CreatedBy);
            actualShow.CreatedUtc.Should().Be(originalShow.CreatedUtc);
            actualShow.ModifiedBy.Should().Be("user");
            actualShow.ModifiedUtc.Should().Be(DateTimeService.UtcNow);

            EventPublisher.PublishedEvents.Should().HaveCount(1);
            var actualEvent = EventPublisher.PublishedEvents.OfType<ShowMainPictureSetEvent>().Single();
            actualEvent.ShowId.Should().Be(1);
            actualEvent.ShowPictureId.Should().Be(10);
        }

        [Fact]
        public async Task ClearingShowMainPicture()
        {
            // arrange
            var originalShow = await PrepareShow(1);
            await PrepareShowPicture(10, 1);
            await PrepareShowPicture(11, 1);
            originalShow.MainShowPictureId = 11;
            await UpdateShow(originalShow);

            var command = new SetShowMainPictureCommand { ShowId = 1, ShowPictureId = null, UserName = "user" };
            var handler = CreateHandler();

            // act
            await handler.Handle(command);

            // assert
            var actualShow = GetShowById(1);
            actualShow.MainShowPictureId.Should().Be(null);
            actualShow.IsDeleted.Should().Be(false);
            actualShow.CreatedBy.Should().Be(originalShow.CreatedBy);
            actualShow.CreatedUtc.Should().Be(originalShow.CreatedUtc);
            actualShow.ModifiedBy.Should().Be("user");
            actualShow.ModifiedUtc.Should().Be(DateTimeService.UtcNow);

            EventPublisher.PublishedEvents.Should().HaveCount(1);
            var actualEvent = EventPublisher.PublishedEvents.OfType<ShowMainPictureSetEvent>().Single();
            actualEvent.ShowId.Should().Be(1);
            actualEvent.ShowPictureId.Should().Be(null);
        }

        [Fact]
        public async Task SettingSameMainPicture()
        {
            // arrange
            var originalShow = await PrepareShow(1);
            await PrepareShowPicture(10, 1);
            await PrepareShowPicture(11, 1);
            originalShow.MainShowPictureId = 10;
            await UpdateShow(originalShow);

            var command = new SetShowMainPictureCommand {ShowId = 1, ShowPictureId = 10, UserName = "user"};
            var handler = CreateHandler();

            // act
            await handler.Handle(command);

            // assert
            var actualShow = GetShowById(1);
            actualShow.MainShowPictureId.Should().Be(10);
            actualShow.IsDeleted.Should().Be(false);
            actualShow.CreatedBy.Should().Be(originalShow.CreatedBy);
            actualShow.CreatedUtc.Should().Be(originalShow.CreatedUtc);
            actualShow.ModifiedBy.Should().Be(originalShow.ModifiedBy);
            actualShow.ModifiedUtc.Should().Be(originalShow.ModifiedUtc);

            EventPublisher.PublishedEvents.Should().HaveCount(0);
        }

        [Fact]
        public async Task ClearingMainPictureWhenItWasNotSetBefore()
        {
            // arrange
            var originalShow = await PrepareShow(1);

            var command = new SetShowMainPictureCommand { ShowId = 1, ShowPictureId = null, UserName = "user" };
            var handler = CreateHandler();

            // act
            await handler.Handle(command);

            // assert
            var actualShow = GetShowById(1);
            actualShow.MainShowPictureId.Should().Be(null);
            actualShow.IsDeleted.Should().Be(false);
            actualShow.CreatedBy.Should().Be(originalShow.CreatedBy);
            actualShow.CreatedUtc.Should().Be(originalShow.CreatedUtc);
            actualShow.ModifiedBy.Should().Be(originalShow.ModifiedBy);
            actualShow.ModifiedUtc.Should().Be(originalShow.ModifiedUtc);

            EventPublisher.PublishedEvents.Should().HaveCount(0);
        }

        [Fact]
        public async Task ShowNotFound()
        {
            // arrange
            var originalShow = await PrepareShow(1);

            var command = new SetShowMainPictureCommand { ShowId = 2, ShowPictureId = null, UserName = "user" };
            var handler = CreateHandler();

            // act
            await Assert.ThrowsAsync<EntityNotFoundException<Show>>(
                async () => await handler.Handle(command));

            // assert
            var actualShow = GetShowById(1);
            actualShow.ModifiedUtc.Should().Be(originalShow.ModifiedUtc);

            EventPublisher.PublishedEvents.Should().HaveCount(0);
        }

        [Fact]
        public async Task ShowDeleted()
        {
            // arrange
            var originalShow = await PrepareShow(1, x => x.IsDeleted = true);

            var command = new SetShowMainPictureCommand { ShowId = 1, ShowPictureId = null, UserName = "user" };
            var handler = CreateHandler();

            // act
            await Assert.ThrowsAsync<EntityNotFoundException<Show>>(
                async () => await handler.Handle(command));

            // assert
            var actualShow = GetShowById(1);
            actualShow.ModifiedUtc.Should().Be(originalShow.ModifiedUtc);

            EventPublisher.PublishedEvents.Should().HaveCount(0);
        }

        [Fact]
        public async Task ShowPictureNotFound()
        {
            // arrange
            var originalShow = await PrepareShow(1);
            await PrepareShowPicture(10, 1);

            var command = new SetShowMainPictureCommand { ShowId = 1, ShowPictureId = 11, UserName = "user" };
            var handler = CreateHandler();

            // act
            await Assert.ThrowsAsync<EntityNotFoundException<ShowPicture>>(
                async () => await handler.Handle(command));

            // assert
            var actualShow = GetShowById(1);
            actualShow.ModifiedUtc.Should().Be(originalShow.ModifiedUtc);

            EventPublisher.PublishedEvents.Should().HaveCount(0);
        }

        [Fact]
        public async Task ShowPictureDeleted()
        {
            // arrange
            var originalShow = await PrepareShow(1);
            await PrepareShowPicture(10, 1, x => x.IsDeleted = true);

            var command = new SetShowMainPictureCommand { ShowId = 1, ShowPictureId = 10, UserName = "user" };
            var handler = CreateHandler();

            // act
            await Assert.ThrowsAsync<EntityNotFoundException<ShowPicture>>(
                async () => await handler.Handle(command));

            // assert
            var actualShow = GetShowById(1);
            actualShow.ModifiedUtc.Should().Be(originalShow.ModifiedUtc);

            EventPublisher.PublishedEvents.Should().HaveCount(0);
        }

        [Fact]
        public async Task ShowPictureBelongsToDifferentShow()
        {
            // arrange
            var originalShow = await PrepareShow(1);
            var otherShow = await PrepareShow(2);
            await PrepareShowPicture(10, 2);

            var command = new SetShowMainPictureCommand { ShowId = 1, ShowPictureId = 10, UserName = "user" };
            var handler = CreateHandler();

            // act
            var actualException = await Assert.ThrowsAsync<BusinessRuleException<ShowMainPictureMustBelongToShowRule>>(
                async () => await handler.Handle(command));

            // assert
            actualException.BusinessRule.ShowId.Should().Be(1);
            actualException.BusinessRule.ActualShowId.Should().Be(2);
            actualException.BusinessRule.ShowPictureId.Should().Be(10);
            var actualShow1 = GetShowById(1);
            actualShow1.ModifiedUtc.Should().Be(originalShow.ModifiedUtc);
            var actualShow2 = GetShowById(2);
            actualShow2.ModifiedUtc.Should().Be(otherShow.ModifiedUtc);

            EventPublisher.PublishedEvents.Should().HaveCount(0);
        }

        #endregion

        #region Helpers

        private async Task<Show> PrepareShow(long showId, Action<Show> customActions = null)
        {
            var db = InMemoryDb.Create();
            var show = TestData.CreateShow(showId, new long[0]);
            customActions?.Invoke(show);

            db.Shows.Add(show);

            db.SaveChanges();

            return show;
        }

        private async Task UpdateShow(Show show)
        {
            var db = InMemoryDb.Create();

            db.Shows.Update(show);

            db.SaveChanges();
        }

        private async Task PrepareShowPicture(long showPictureId, long showId, Action<ShowPicture> customActions = null)
        {
            var db = InMemoryDb.Create();
            var showPicture = TestData.CreateShowPicture(showPictureId, showId);
            customActions?.Invoke(showPicture);

            db.ShowPictures.Add(showPicture);

            db.SaveChanges();
        }

        private Show GetShowById(long showId)
        {
            var db = InMemoryDb.Create(false);
            return db.Shows.FirstOrDefault(x => x.Id == showId);
        }

        #endregion
    }
}