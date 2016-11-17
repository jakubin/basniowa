using Logic.Services;
using System;

namespace Logic.Tests.Helpers
{
    public class TestDateTimeService : IDateTimeService
    {
        public DateTimeOffset UtcNow { get; set; } = new DateTimeOffset(2016, 02, 01, 12, 00, 00, TimeSpan.Zero);
    }
}
