using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Logic.Services;
using Logic.Shows;
using Logic.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Logic.Tests.Shows
{
    public class AddShowHandlerTests : IDisposable
    {
        private IUniqueIdService IdService { get; set; }

        private TestEventPublisher EventPublisher { get; set; }

        private IDateTimeService DateTimeService { get; set; }

        private IDbContextFactory DbContextFactory { get; set; }

        private InMemoryDb InMemoryDb { get; set; }

        public AddShowHandlerTests()
        {
            IdService = new TestUniqueIdService();

            EventPublisher = new TestEventPublisher();

            DateTimeService = new TestDateTimeService();

            InMemoryDb = new InMemoryDb();
            DbContextFactory = InMemoryDb;
        }

        public void Dispose()
        {
            InMemoryDb.Dispose();
        }

        public AddShowHandler CreateHandler()
        {
            return new AddShowHandler()
            {
                IdService = IdService,
                EventPublisher = EventPublisher,
                DateTimeService = DateTimeService,
                DbFactory = DbContextFactory
            };
        }

        [Fact(DisplayName = nameof(AddShowHandler) + ": First show should be correctly added and event should be raised.")]
        public async Task Adding_First_Show()
        {
            // arrange
            var command = new AddShowCommand
            {
                ShowId = await IdService.GenerateId(),
                Title = "Title",
                Description = "Description",
                Subtitle = "Subtitle",
                UserName = "testuser",
                Properties = new Dictionary<string, string>
                {
                    ["Prop1"] = "Val1",
                    ["Prop2"] = "Val2",
                }
            };
            var handler = CreateHandler();

            // act
            await handler.Handle(command);

            // assert
            var db = InMemoryDb.Create();
            var actualShow = await db.Shows.SingleAsync();
            var actualProperties = await db.ShowProperties.ToListAsync();

            actualShow.Id.Should().Be(command.ShowId);
            actualShow.Title.Should().Be(command.Title);
            actualShow.Description.Should().Be(command.Description);
            actualShow.Subtitle.Should().Be(command.Subtitle);
            actualShow.CreatedBy.Should().Be(command.UserName);
            actualShow.CreatedUtc.Should().Be(DateTimeService.UtcNow);
            actualShow.ModifiedBy.Should().Be(command.UserName);
            actualShow.ModifiedUtc.Should().Be(DateTimeService.UtcNow);
            actualShow.IsDeleted.Should().Be(false);

            var expectedProperties = command.Properties.Select(x => new { Name = x.Key, Value = x.Value });
            actualProperties
                .Where(x => x.IsDeleted == false)
                .Select(x => new { x.Name, x.Value })
                .Should().BeEquivalentTo(expectedProperties);
            actualProperties.Select(x => x.Id).Distinct()
                .Should().HaveSameCount(actualProperties, "Show property IDs should be unique.")
                .And.Should().NotBe(command.ShowId);

            var actualEvent = EventPublisher.PublishedEvents.Cast<ShowAddedEvent>().Single();
            actualEvent.ShowId.Should().Be(command.ShowId);
        }
    }
}
