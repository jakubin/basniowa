using System;
using System.Threading.Tasks;
using Common;
using Common.Cqrs;
using Microsoft.Extensions.Logging;

namespace Logic.Common.Logging
{
    /// <summary>
    /// Message processor, which is responsible for logging the events being handled.
    /// </summary>
    public class EventHandlingLogger : IMessageProcessor
    {
        private readonly ILogger<EventHandlingLogger> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlingLogger"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="logger"/> is <c>null</c>.</exception>
        public EventHandlingLogger(ILogger<EventHandlingLogger> logger)
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
                    LoggingEvents.EventHandling,
                    "Event {MessageType} will be handled by {HandlerType}.",
                    context.Message.GetType().Name,
                    context.Handler.GetType().Name);

                await next(context);

                _logger.LogInformation(
                    LoggingEvents.EventHandled,
                    "Event {MessageType} was successfully handled by {HandlerType}.",
                    context.Message.GetType().Name,
                    context.Handler.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    LoggingEvents.EventHandlingException,
                    ex,
                    "Processing of event {MessageType} by handler {HandlerType} resulted with an exception.",
                    context.Message.GetType().Name,
                    context.Handler.GetType().Name);
                throw;
            }
        }
    }
}
