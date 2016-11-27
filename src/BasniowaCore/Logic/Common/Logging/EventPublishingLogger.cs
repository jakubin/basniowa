using System;
using System.Threading.Tasks;
using Common;
using Common.Cqrs;
using Microsoft.Extensions.Logging;

namespace Logic.Common.Logging
{
    /// <summary>
    /// Message processor, which is responsible for logging the events being published.
    /// </summary>
    public class EventPublishingLogger : IMessageProcessor
    {
        private readonly ILogger<EventPublishingLogger> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublishingLogger"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="logger"/> is <c>null</c>.</exception>
        public EventPublishingLogger(ILogger<EventPublishingLogger> logger)
        {
            Guard.NotNull(logger, nameof(logger));

            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task Process(MessageProcessingContext context, MessageHandlerDelegate next)
        {
            try
            {
                _logger.LogInformation(
                    LoggingEvents.EventPublishing,
                    "Event {MessageType} is being published",
                    context.Message.GetType().Name);

                await next(context);

                _logger.LogDebug(
                    LoggingEvents.EventPublished,
                    "Event {MessageType} was published successfully",
                    context.Message.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    LoggingEvents.EventPublishingException,
                    ex,
                    "Publication of event {MessageType} resulted with an exception",
                    context.Message.GetType().Name);
                throw;
            }
        }
    }
}
