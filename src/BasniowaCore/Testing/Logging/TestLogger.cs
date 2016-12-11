using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Testing.Logging
{
    /// <summary>
    /// Logger that can be used for testing.
    /// Logs are stored in an in-memory collection.
    /// </summary>
    /// <typeparam name="T">Type owning the logger</typeparam>
    public class TestLogger<T> : ILogger<T>
    {
        private List<LogEntry> _entries = new List<LogEntry>();

        /// <summary>
        /// Gets the entries in logger.
        /// </summary>
        /// <returns>Copy of entries in the log.</returns>
        public IList<LogEntry> GetEntries()
        {
            lock (_entries)
            {
                return _entries.ToList();
            }
        }

        /// <inheritdoc/>
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        /// <inheritdoc/>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            lock (_entries)
            {
                _entries.Add(new LogEntry(logLevel, eventId, state, exception, formatter(state, exception)));
            }
        }
    }
}
