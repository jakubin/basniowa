using Microsoft.Extensions.Logging;

namespace Logic.Common.Logging
{
    /// <summary>
    /// Provides <see cref="EventId"/> for most common logging scenarios in current module.
    /// </summary>
    public static class LoggingEvents
    {
        /// <summary>
        /// Gets the base ID.
        /// </summary>
        public static int BaseId { get; } = 1000;

        /// <summary>
        /// Gets the command handling event ID.
        /// </summary>
        public static EventId CommandHandling => new EventId(BaseId + 0);

        /// <summary>
        /// Gets the command handled event ID.
        /// </summary>
        public static EventId CommandHandled => new EventId(BaseId + 1);

        /// <summary>
        /// Gets the command handling exception event ID.
        /// </summary>
        public static EventId CommandHandlingException => new EventId(BaseId + 2);
    }
}
