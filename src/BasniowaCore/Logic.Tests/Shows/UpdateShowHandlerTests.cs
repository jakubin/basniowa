using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Logic.Common;
using Logic.Shows;
using Logic.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Logic.Tests.Shows
{
    public class UpdateShowHandlerTests : IDisposable
    {
        private TestUniqueIdService IdService { get; set; }

        private TestEventPublisher EventPublisher { get; set; }

        private TestDateTimeService DateTimeService { get; set; }

        private InMemoryDb InMemoryDb { get; set; }

        public UpdateShowHandlerTests()
        {
            IdService = new TestUniqueIdService();

            EventPublisher = new TestEventPublisher();

            DateTimeService = new TestDateTimeService();

            InMemoryDb = new InMemoryDb();
        }

        public void Dispose()
        {
            InMemoryDb.Dispose();
        }

        public UpdateShowHandler CreateHandler()
        {
            return new UpdateShowHandler
            {
                IdService = IdService,
                EventPublisher = EventPublisher,
                DateTimeService = DateTimeService,
                DbFactory = InMemoryDb
            };
        }

        [Fact]
        public async Task UpdateExisting()
        {
            // arrange
            var originalShow = TestData.CreateShow(10, propertyIds: new long[] {11, 12, 13});
            originalShow.ShowProperties.First(x => x.Id == 12).IsDeleted = true;
            using (var db = InMemoryDb.Create())
            {
                db.Add(originalShow);
                db.SaveChanges();
            }
            IdService.NextId = 14;
            var property11 = originalShow.ShowProperties.First(x => x.Id == 11);
            var property12 = originalShow.ShowProperties.First(x => x.Id == 12);
            var command = new UpdateShowCommand
            {
                ShowId = 10,
                Title = "NewTitle",
                Description = "NewDescription",
                Subtitle = "NewSubtitle",
                UserName = "updator",
                Properties = new Dictionary<string, string>
                {
                    [property11.Name] = "NewValue11",
                    [property12.Name] = "NewValue12",
                    ["NewProperty"] = "NewValue2"
                }
            };
            var handler = CreateHandler();

            // act
            await handler.Handle(command);

            // assert
            using (var db = InMemoryDb.Create())
            {
                var actual = db.Shows.Include(x => x.ShowProperties).Single(x => x.Id == 10);

                actual.Id.Should().Be(command.ShowId);
                actual.Title.Should().Be(command.Title);
                actual.Description.Should().Be(command.Description);
                actual.Subtitle.Should().Be(command.Subtitle);
                actual.CreatedBy.Should().Be(originalShow.CreatedBy);
                actual.CreatedUtc.Should().Be(originalShow.ModifiedUtc);
                actual.ModifiedBy.Should().Be(command.UserName);
                actual.ModifiedUtc.Should().Be(DateTimeService.UtcNow);
                actual.IsDeleted.Should().Be(false);

                var expectedProperties = command.Properties.Select(x => new { Name = x.Key, Value = x.Value });
                actual.ShowProperties
                    .Where(x => x.IsDeleted == false)
                    .Select(x => new { x.Name, x.Value })
                    .Should().BeEquivalentTo(expectedProperties);
                actual.ShowProperties.Select(x => new {x.Id, x.IsDeleted})
                    .Should().BeEquivalentTo(
                        new {Id = 11L, IsDeleted = false},
                        new {Id = 12L, IsDeleted = false},
                        new {Id = 13L, IsDeleted = true},
                        new {Id = 14L, IsDeleted = false}
                    );
            }

            var actualEvent = EventPublisher.PublishedEvents.Cast<ShowUpdated>().Single();
            actualEvent.ShowId.Should().Be(command.ShowId);
        }

        [Fact]
        public async Task NotExisting()
        {
            // arrange
            var command = new UpdateShowCommand
            {
                ShowId = 100,
                Title = "NewTitle",
                Description = "NewDescription",
                Subtitle = "NewSubtitle",
                UserName = "updator",
                Properties = new Dictionary<string, string>
                {
                    ["NewProperty"] = "NewValue"
                }
            };
            var handler = CreateHandler();

            // act
            await Assert.ThrowsAsync<EntityNotFoundException<DataAccess.Shows.Show>>(
                () => handler.Handle(command));

            EventPublisher.PublishedEvents.OfType<ShowUpdated>().Should().BeEmpty();
        }

        [Fact]
        public async Task Deleted()
        {
            // arrange
            var originalShow = TestData.CreateShow(10);
            originalShow.IsDeleted = true;
            using (var db = InMemoryDb.Create())
            {
                db.Add(originalShow);
                db.SaveChanges();
            }
            var command = new UpdateShowCommand
            {
                ShowId = 10,
                Title = "NewTitle",
                Description = "NewDescription",
                Subtitle = "NewSubtitle",
                UserName = "updator",
                Properties = new Dictionary<string, string>()
            };
            var handler = CreateHandler();

            // act
            await Assert.ThrowsAsync<EntityNotFoundException<DataAccess.Shows.Show>>(
                () => handler.Handle(command));

            // assert
            using (var db = InMemoryDb.Create())
            {
                var actual = db.Shows.Include(x => x.ShowProperties).Single(x => x.Id == 10);

                actual.Id.Should().Be(originalShow.Id);
                actual.Title.Should().Be(originalShow.Title);
                actual.Description.Should().Be(originalShow.Description);
                actual.Subtitle.Should().Be(originalShow.Subtitle);
                actual.CreatedBy.Should().Be(originalShow.CreatedBy);
                actual.CreatedUtc.Should().Be(originalShow.CreatedUtc);
                actual.ModifiedBy.Should().Be(originalShow.ModifiedBy);
                actual.ModifiedUtc.Should().Be(originalShow.ModifiedUtc);
                actual.IsDeleted.Should().Be(true);

                var expectedProperties = originalShow.ShowProperties
                    .Select(x => new {x.Id, x.Name, x.Value, x.IsDeleted})
                    .ToList();
                actual.ShowProperties.Select(x => new {x.Id, x.Name, x.Value, x.IsDeleted})
                    .Should().BeEquivalentTo(expectedProperties);
            }

            EventPublisher.PublishedEvents.OfType<ShowUpdated>().Should().BeEmpty();
        }
    }
}
