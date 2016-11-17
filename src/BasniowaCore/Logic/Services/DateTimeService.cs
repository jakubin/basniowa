using System;

namespace Logic.Services
{
    /// <summary>
    /// Implementation of <see cref="IDateTimeService"/> using current UTC time.
    /// </summary>
    public class DateTimeService : IDateTimeService
    {
        /// <inheritdoc/>
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
