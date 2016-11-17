using FluentAssertions;
using System;
using Xunit;

namespace Common.Tests
{
    public class GuardTests
    {
        [Fact(DisplayName = nameof(Guard) + ": NotNull() should not throw on not null values.")]
        public void NotNullShouldNotThrowOnNonNull()
        {
            var notNull = new object();
            Guard.NotNull(notNull, "notNull");
        }

        [Fact(DisplayName = nameof(Guard) + ": NotNull() should throw ArgumentNullException on null values.")]
        public void NotNullShouldThrowOnNull()
        {
            var exception = Assert.Throws<ArgumentNullException>("someName", () => {
                Guard.NotNull(null, "someName");
            });

            exception.Message.Should().Contain("someName");
        }

        [Theory(DisplayName = nameof(Guard) + ": GreaterOrEqual() should not throw on valid values.")]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(5, 1)]
        public void GreaterOrEqualShouldNotThrowOnValid(int value, int minValue)
        {
            Guard.GreaterOrEqual(value, minValue, "someName");
        }

        [Fact(DisplayName = nameof(Guard) + ": GreaterOrEqual() should throw ArgumentOutOfRangeException on invalid values.")]
        public void GreaterOrEqualShouldThrowOnInvalid()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>("someName", () => {
                Guard.GreaterOrEqual(1, 2, "someName");
            });
            exception.Message.Should().Contain("someName");
        }
    }
}
