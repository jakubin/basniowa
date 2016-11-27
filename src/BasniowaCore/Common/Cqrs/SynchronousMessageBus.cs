using System;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Cqrs
{
    /// <summary>
    /// Synchronous, in-process implementation of a message bus.
    /// </summary>
    /// <seealso cref="IMessageBus" />
    public class SynchronousMessageBus : IMessageBus
    {
        private IHandlerResolver _handlerResolver;

        private MessageProcessingChain _commandHandlerChain = new MessageProcessingChain();

        private MessageProcessingChain _eventPublicationChain = new MessageProcessingChain();

        private MessageProcessingChain _eventHandlerChain = new MessageProcessingChain();

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronousMessageBus"/> class.
        /// </summary>
        /// <param name="handlerResolver">The handler resolver.</param>
        public SynchronousMessageBus(IHandlerResolver handlerResolver)
        {
            Guard.NotNull(handlerResolver, nameof(handlerResolver));

            _handlerResolver = handlerResolver;
        }

        #region Sending commands

        /// <inheritdoc/>
        public async Task Send<T>(T command) where T : ICommand
        {
            var handler = ResolveCommandHandler<T>();
            var context = MessageProcessingContext.Create(command, handler);
            await _commandHandlerChain.Process(context);
        }

        private IHandler<T> ResolveCommandHandler<T>() where T : ICommand
        {
            var handlers = _handlerResolver.Resolve<T>();

            if (handlers.Count == 0)
            {
                throw new InvalidOperationException($"Cannot find handler for command of type {typeof(T).FullName}.");
            }

            if (handlers.Count > 1)
            {
                throw new InvalidOperationException(
                    $"{handlers.Count} handler exists for command of type {typeof(T).FullName}. Only 1 handler is allowed.");
            }

            var handler = handlers.Single();
            return handler;
        }

        #endregion

        #region Publishing events

        /// <inheritdoc/>
        public async Task Publish<T>(T @event) where T : IEvent
        {
            var context = MessageProcessingContext.Create(
                @event,
                () => PublishToHandlers(@event));
            await _eventPublicationChain.Process(context);
        }

        private async Task PublishToHandlers<T>(T @event) where T : IEvent
        {
            var handlers = _handlerResolver.Resolve<T>();
            foreach (var handler in handlers)
            {
                try
                {
                    var context = MessageProcessingContext.Create(@event, handler);
                    await _eventHandlerChain.Process(context);
                }
                catch 
                {
                    // ignoring exceptions in handlers by design
                }
            }
        }

        #endregion

        #region Message handling chain

        /// <inheritdoc/>
        public void AddCommandHandlerProcessor(IMessageProcessor processor)
        {
            _commandHandlerChain.AddProcessor(processor);
        }

        /// <inheritdoc/>
        public void AddEventPublicationProcessor(IMessageProcessor processor)
        {
            _eventPublicationChain.AddProcessor(processor);
        }

        /// <inheritdoc/>
        public void AddEventHandlerProcessor(IMessageProcessor processor)
        {
            _eventHandlerChain.AddProcessor(processor);
        }

        #endregion
    }
}
