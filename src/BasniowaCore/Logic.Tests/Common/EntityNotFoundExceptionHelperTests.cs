using FluentAssertions;
using Logic.Common;
using Xunit;

namespace Logic.Tests.Common
{
    public class EntityNotFoundExceptionHelperTests
    {
        [Fact(DisplayName = "ThrowIfNull should not throw on not null value.")]
        public void ThrowIfNull_NotNull()
        {
            var obj = new Entity();
            obj.ThrowIfNull("xyz");
        }

        [Fact(DisplayName = "ThrowIfNull should throw on not value.")]
        public void ThrowIfNull_Null()
        {
            Entity obj = null;
            var ex = Assert.Throws<EntityNotFoundException<Entity>>(() => obj.ThrowIfNull("key"));
            ex.EntityKey.Should().Be("key");
        }

        private class Entity
        {
        }
    }
}