using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Cqrs;
using DataAccess;
using FluentAssertions;
using Logic.Services;
using Logic.Shows;
using Logic.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Logic.Tests.Shows
{
    public class AddShowHandlerTests : IDisposable
    {
        private IUniqueIdService IdService { get; set; }

        private IEventPublisher EventPublisher { get; set; }

        private IDateTimeService DateTimeService { get; set; }

        private IDbContextFactory DbContextFactory { get; set; }

        private volatile string _dbName;

        public AddShowHandlerTests()
        {
            IdService = new TestUniqueIdService();

            EventPublisher = Mock.Of<IEventPublisher>();

            DateTimeService = new TestDateTimeService();

            DbContextFactory = Mock.Of<IDbContextFactory>();
            Mock.Get(DbContextFactory).Setup(x => x.Create()).Returns(CreateInMemoryDb);
        }

        public void Dispose()
        {
            if (_dbName != null)
            {
                var db = CreateInMemoryDb();
                db.Database.EnsureDeleted();
            }
        }

        public TheaterDb CreateInMemoryDb()
        {
            if (_dbName == null)
            {
                _dbName = Guid.NewGuid().ToString();
            }

            var builder = new DbContextOptionsBuilder<TheaterDb>()
                .UseInMemoryDatabase(_dbName);

            return new TheaterDb(builder.Options);
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
                Id = await IdService.GenerateId(),
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
            ShowCreated actualEvent = null;
            Mock.Get(EventPublisher)
                .Setup(x => x.PublishEvent(It.IsAny<ShowCreated>()))
                .Returns(Task.CompletedTask)
                .Callback((ShowCreated @event) => 
                {
                    actualEvent = @event;
                })
                .Verifiable();
            var handler = CreateHandler();

            // act
            await handler.Handle(command);

            // assert
            var db = CreateInMemoryDb();
            var actualShow = await db.Shows.SingleAsync();
            var actualProperties = await db.ShowProperties.ToListAsync();

            actualShow.Id.Should().Be(command.Id);
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
                .And.Should().NotBe(command.Id);

            actualEvent.Should().NotBeNull();
            actualEvent.Id.Should().Be(command.Id);
        }
    }
}
