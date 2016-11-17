using System;

namespace Logic.Services
{
    /// <summary>
    /// Provides date and time for the application.
    /// </summary>
    public interface IDateTimeService
    {
        /// <summary>
        /// Gets the current date and time in UTC.
        /// </summary>
        DateTimeOffset UtcNow { get; }
    }
}