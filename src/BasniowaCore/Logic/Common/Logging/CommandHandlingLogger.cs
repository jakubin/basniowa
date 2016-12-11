using System;
using System.Threading.Tasks;
using Common;
using Common.Cqrs;
using Microsoft.Extensions.Logging;

namespace Logic.Common.Logging
{
    /// <summary>
    /// Message processor, which is responsible for logging the commands being handled.
    /// </summary>
    public class CommandHandlingLogger : IMessageProcessor
    {
        private readonly ILogger<CommandHandlingLogger> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlingLogger"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="logger"/> is <c>null</c>.</exception>
        public CommandHandlingLogger(ILogger<CommandHandlingLogger> logger)
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
                    LoggingEvents.CommandHandling,
                    "Command {MessageType} will be handled by {HandlerType}.",
                    context.Message.GetType().Name,
                    context.Handler.GetType().Name);

                await next(context);

                _logger.LogInformation(
                    LoggingEvents.CommandHandled,
                    "Command {MessageType} was successfully handled by {HandlerType}.",
                    context.Message.GetType().Name,
                    context.Handler.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    LoggingEvents.CommandHandlingException,
                    ex,
                    "Processing of command {MessageType} by handler {HandlerType} resulted with an exception.",
                    context.Message.GetType().Name,
                    context.Handler.GetType().Name);
                throw;
            }
        }
    }
}
