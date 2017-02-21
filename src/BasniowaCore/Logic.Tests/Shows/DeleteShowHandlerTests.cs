using System;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Database.Shows;
using FluentAssertions;
using Logic.Common;
using Logic.Services;
using Logic.Shows;
using Logic.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Logic.Tests.Shows
{
    public class DeleteShowHandlerTests : IDisposable
    {
        #region Preparation

        private TestEventPublisher EventPublisher { get; set; }

        private IDateTimeService DateTimeService { get; set; }

        private IDbContextFactory DbContextFactory { get; set; }

        private InMemoryDb InMemoryDb { get; set; }

        public DeleteShowHandlerTests()
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

        public DeleteShowHandler CreateHandler()
        {
            return new DeleteShowHandler
            {
                EventPublisher = EventPublisher,
                DateTimeService = DateTimeService,
                DbContextFactory = DbContextFactory
            };
        }

        #endregion

        [Fact(DisplayName = "Delete of existing show should succeed and raise an event.")]
        public async Task DeleteExisting()
        {
            // arrange
            const int showId = 5;
            var db = InMemoryDb.Create();
            var show = TestData.CreateShow(id: showId);
            db.Shows.Add(show);
            db.SaveChanges();
            var command = new DeleteShowCommand
            {
                ShowId = showId,
                UserName = "user"
            };

            // act
            var handler = CreateHandler();
            await handler.Handle(command);

            // assert
            db = InMemoryDb.Create();
            var current = db.Shows
                .Include(x => x.ShowProperties)
                .FirstOrDefault(x => x.Id == showId);
            current.Should().NotBeNull();
            current.IsDeleted.Should().BeTrue();
            current.ModifiedBy.Should().Be("user");
            current.ModifiedUtc.Should().Be(DateTimeService.UtcNow);

            current.ShowProperties.Should().HaveCount(2);
            current.ShowProperties.Select(x => x.IsDeleted)
                .ShouldAllBeEquivalentTo(false);

            var actualEvent = EventPublisher.PublishedEvents
                .Cast<ShowDeleted>()
                .Single();
            actualEvent.ShowId.Should().Be(showId);
        }

        [Fact(DisplayName = "Delete of existing show that was deleted before should not result in any action.")]
        public async Task DeleteAlreadyDeleted()
        {
            // arrange
            const int showId = 5;
            var db = InMemoryDb.Create();
            var show = TestData.CreateShow(id: showId);
            show.IsDeleted = true;
            db.Shows.Add(show);
            db.SaveChanges();
            var command = new DeleteShowCommand
            {
                ShowId = showId,
                UserName = "user"
            };

            // act
            var handler = CreateHandler();
            await handler.Handle(command);

            // assert
            db = InMemoryDb.Create();
            var current = db.Shows
                .FirstOrDefault(x => x.Id == showId);
            current.Should().NotBeNull();
            current.IsDeleted.Should().BeTrue();
            current.ModifiedBy.Should().Be(show.ModifiedBy);
            current.ModifiedUtc.Should().Be(show.ModifiedUtc);

            EventPublisher.PublishedEvents.Should().BeEmpty();
        }

        [Fact(DisplayName = "Delete of not existing show should result in an exception")]
        public async Task DeleteNotExisting()
        {
            // arrange
            const int showId = 5;
            var command = new DeleteShowCommand
            {
                ShowId = showId,
                UserName = "user"
            };

            // act & assert
            var handler = CreateHandler();
            var ex = await Assert.ThrowsAsync<EntityNotFoundException<Show>>(
                () => handler.Handle(command));
            ex.EntityKey.Should().Be("5");
            EventPublisher.PublishedEvents.Should().BeEmpty();
        }
    }
}