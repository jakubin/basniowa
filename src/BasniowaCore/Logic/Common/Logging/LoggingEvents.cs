using Microsoft.Extensions.Logging;

namespace Logic.Common.Logging
{
    /// <summary>
    /// Provides <see cref="EventId"/> for most common logging scenarios in current module.
    /// </summary>
    public static class LoggingEvents
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public static int BaseId { get; } = 1000;

        public static EventId CommandHandling => new EventId(BaseId + 0);

        public static EventId CommandHandled => new EventId(BaseId + 1);

        public static EventId CommandHandlingException => new EventId(BaseId + 2);

        public static EventId EventHandling => new EventId(BaseId + 3);

        public static EventId EventHandled => new EventId(BaseId + 4);

        public static EventId EventHandlingException => new EventId(BaseId + 5);

        public static EventId EventPublishing => new EventId(BaseId + 6);

        public static EventId EventPublished => new EventId(BaseId + 7);

        public static EventId EventPublishingException => new EventId(BaseId + 8);

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
