using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Testing.Logging
{
    /// <summary>
    /// Log entry.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Gets the log level.
        /// </summary>
        public LogLevel LogLevel { get; }

        /// <summary>
        /// Gets the event identifier.
        /// </summary>
        public EventId EventId { get; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public object State { get; }

        /// <summary>
        /// Gets the log message arguments.
        /// </summary>
        public IReadOnlyList<KeyValuePair<string, object>> Arguments
        {
            get
            {
                return (IReadOnlyList<KeyValuePair<string, object>>)State;
            }
        }

        /// <summary>
        /// Gets the argument by name.
        /// </summary>
        /// <typeparam name="T">Expected argument type.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns>The value of given argument.</returns>
        public T GetArgument<T>(string name)
        {
            return (T)Arguments.First(x => x.Key == name).Value;
        }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="state">The state.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        public LogEntry(LogLevel logLevel, EventId eventId, object state, Exception exception, string message)
        {
            LogLevel = logLevel;
            EventId = eventId;
            State = state;
            Exception = exception;
            Message = message;
        }
    }
}