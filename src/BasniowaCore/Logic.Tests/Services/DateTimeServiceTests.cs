using System;
using FluentAssertions;
using Logic.Services;
using Xunit;

namespace Logic.Tests.Services
{
    public class DateTimeServiceTests
    {
        [Fact(DisplayName = nameof(DateTimeService) + ": UtcNow() should return current date and time in UTC time zone.")]
        public void UtcNowShouldReturnCurrentDateAndTimeInUtc()
        {
            // arrange
            var service = new DateTimeService();

            // act
            var now = service.UtcNow;

            // assert
            now.Should().BeCloseTo(DateTimeOffset.UtcNow, 100 /*ms*/);
        }
    }
}
