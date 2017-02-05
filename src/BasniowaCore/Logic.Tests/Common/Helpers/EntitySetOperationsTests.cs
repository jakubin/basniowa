using System.Linq;
using FluentAssertions;
using Logic.Common.Helpers;
using Xunit;

namespace Logic.Tests.Common.Helpers
{
    public class EntitySetOperationsTests
    {
        [Fact]
        public void GetRequiredOperations_EmptySourceAndTarget()
        {
            var source = new Entity[0];
            var target = new Entity[0];

            var operations = source.GetRequiredOperations(target, x => x.Key);

            operations.ToAdd.Should().BeEmpty();
            operations.ToRemove.Should().BeEmpty();
            operations.ToUpdate.Should().BeEmpty();
        }

        [Fact]
        public void GetRequiredOperations_EmptySource()
        {
            var source = new Entity[0];
            var target = new []
            {
                new Entity(1), 
                new Entity(2),
            };

            var operations = source.GetRequiredOperations(target, x => x.Key);

            operations.ToAdd.Should().BeEquivalentTo(target);
            operations.ToRemove.Should().BeEmpty();
            operations.ToUpdate.Should().BeEmpty();
        }

        [Fact]
        public void GetRequiredOperations_EmptyTarget()
        {
            var source = new[]
            {
                new Entity(1),
                new Entity(2),
            };
            var target = new Entity[0];

            var operations = source.GetRequiredOperations(target, x => x.Key);

            operations.ToAdd.Should().BeEmpty();
            operations.ToRemove.Should().BeEquivalentTo(source);
            operations.ToUpdate.Should().BeEmpty();
        }

        [Fact]
        public void GetRequiredOperations_SourceAndTargetTheSame()
        {
            var source = new[]
            {
                new Entity(1),
                new Entity(2)
            };
            var target = new[]
            {
                new Entity(1),
                new Entity(2)
            };

            var operations = source.GetRequiredOperations(target, x => x.Key);

            operations.ToAdd.Should().BeEmpty();
            operations.ToRemove.Should().BeEmpty();
            operations.ToUpdate.Select(x => new {x.Source, x.Target})
                .Should().BeEquivalentTo(
                    new {Source = source[0], Target = target[0]},
                    new {Source = source[1], Target = target[1]}
                );
        }

        [Fact]
        public void GetRequiredOperations_MultipleUpdates()
        {
            var source = new[]
            {
                new Entity(1),
                new Entity(2)
            };
            var target = new[]
            {
                new Entity(1),
                new Entity(3)
            };

            var operations = source.GetRequiredOperations(target, x => x.Key);

            operations.ToAdd.Should().BeEquivalentTo(target[1]);
            operations.ToRemove.Should().BeEquivalentTo(source[1]);
            operations.ToUpdate.Select(x => new {x.Source, x.Target})
                .Should().BeEquivalentTo(
                    new {Source = source[0], Target = target[0]}
                );
        }

        class Entity
        {
            public int Key { get; }

            public Entity(int key)
            {
                Key = key;
            }

            private bool Equals(Entity other)
            {
                return Key == other.Key;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Entity) obj);
            }

            public override int GetHashCode()
            {
                return Key;
            }
        }
    }
}