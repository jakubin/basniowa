using DataAccess.UniqueId;
using FluentAssertions;
using Logic.Services;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Logic.Tests.Services
{
    public class BufferingUniqueIdServiceTests
    {
        public IUniqueIdProvider UniqueIdProvider { get; set; }

        public BufferingUniqueIdServiceTests()
        {
            UniqueIdProvider = new Mock<IUniqueIdProvider>().Object;
            long tmp = 100;
            Mock.Get(UniqueIdProvider)
                .Setup(x => x.GetNextIds(It.IsAny<long>()))
                .Returns((long count) => {
                    var task = Task.FromResult(tmp);
                    tmp += count;
                    return task;
                })
                .Verifiable();
        }

        public BufferingUniqueIdService Create(int prefetchCount)
        {
            return new BufferingUniqueIdService(UniqueIdProvider, prefetchCount);
        }

        [Fact(DisplayName = nameof(BufferingUniqueIdService) + ": GenerateId() should provide ID from source when pre-fetching is off.")]
        public async Task SingleIdGenerationWithoutPrefetch()
        {
            // arrange
            var service = Create(0);

            // act
            var actualId1 = await service.GenerateId();
            var actualId2 = await service.GenerateId();

            // assert
            actualId1.Should().Be(100L);
            actualId2.Should().Be(101L);
            Mock.Get(UniqueIdProvider).Verify(x => x.GetNextIds(1), Times.Exactly(2));
        }

        [Fact(DisplayName = nameof(BufferingUniqueIdService) + ": GenerateIds() should provide ID from source when pre-fetching is off.")]
        public async Task MultipleIdGenerationWithoutPrefetch()
        {
            // arrange
            var service = Create(0);

            // act
            var actualId1 = await service.GenerateIds(2);
            var actualId2 = await service.GenerateIds(3);

            // assert
            actualId1.Should().BeEquivalentTo(new[] { 100L, 101L });
            actualId2.Should().BeEquivalentTo(new[] { 102L, 103L, 104L });
            Mock.Get(UniqueIdProvider).Verify(x => x.GetNextIds(2), Times.Once());
            Mock.Get(UniqueIdProvider).Verify(x => x.GetNextIds(3), Times.Once());
        }

        [Fact(DisplayName = nameof(BufferingUniqueIdService) + ": GenerateId() should provide buffered ID when pre-fetching is on.")]
        public async Task GenerateIdWithPrefetch()
        {
            // arrange
            var service = Create(1);

            // act
            var actualIds = new[] {
                await service.GenerateId(),
                await service.GenerateId(),
                await service.GenerateId(),
                await service.GenerateId()
            };

            // assert
            actualIds.Should().BeEquivalentTo(new[] { 100L, 101L, 102L, 103L });
            Mock.Get(UniqueIdProvider).Verify(x => x.GetNextIds(2), Times.Exactly(2));
        }

        [Fact(DisplayName = nameof(BufferingUniqueIdService) + ": GenerateIds() should provide buffered ID when pre-fetching is on.")]
        public async Task GenerateIsdWithPrefetch()
        {
            // arrange
            var service = Create(5);

            // act
            await service.GenerateId();
            var actualIds = await service.GenerateIds(5);

            // assert
            actualIds.Should().BeEquivalentTo(new[] { 101L, 102L, 103L, 104L, 105L });
            Mock.Get(UniqueIdProvider).Verify(x => x.GetNextIds(6), Times.Exactly(1));
        }
    }
}
