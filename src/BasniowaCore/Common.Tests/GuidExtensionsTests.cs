using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Common.Tests
{
    public class GuidExtensionsTests
    {
        public static IEnumerable<object[]> TwoWayEncodingTestData
        {
            get
            {
                return new[]
                    {
                        Guid.Empty,
                        new Guid("e5fdba50-0dd0-4964-b112-49971d451dcd"),
                        new Guid("f9e446ef-2010-47a4-a9e6-0793e763eb02"),
                        new Guid("64a41e26-9629-4c87-a931-baac04b67929"),
                        new Guid("63c0124c-f064-4881-aa31-e2db70a21f7b"),
                    }
                    .Select(x => new object[] {x});
            }
        }

        [Theory]
        [MemberData(nameof(TwoWayEncodingTestData))]
        public void TwoWayEncoding(Guid input)
        {
            var encoded = input.ToShortString();
            var decoded = GuidEncoder.FromShortString(encoded);

            decoded.Should().Be(input);
        }

        [Fact]
        public void FromShortString_ShouldThrowOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => GuidEncoder.FromShortString(null));
        }

        [Fact]
        public void FromShortString_ShouldThrowOnInvalidString()
        {
            Assert.Throws<ArgumentException>(() => GuidEncoder.FromShortString("ABC"));
        }
    }
}