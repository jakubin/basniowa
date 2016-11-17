 using System;
using System.Threading.Tasks;
using System.Linq;

namespace Common.Cqrs
{
    /// <summary>
    /// Command publisher implementation that executes the command handler and then
    /// synchronously executes handlers for all raised events.
    /// </summary>
    /// <seealso cref="ICommandPublisher" />
    public class SynchronousMessageBus : ICommandPublisher, IEventPublisher
    {
        private IHandlerResolver _handlerResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronousMessageBus"/> class.
        /// </summary>
        /// <param name="handlerResolver">The handler resolver.</param>
        public SynchronousMessageBus(IHandlerResolver handlerResolver)
        {
            Guard.NotNull(handlerResolver, nameof(handlerResolver));

            _handlerResolver = handlerResolver;
        }

        /// <inheritdoc/>
        public async Task PublishCommand<T>(T command) where T : ICommand
        {
            var handler = ResolveCommandHandler<T>();
            await handler.Handle(command);
        }

        /// <inheritdoc/>
        public async Task PublishEvent<T>(T @event) where T : IEvent
        {
            var handlers = _handlerResolver.Resolve<T>();

            foreach (var handler in handlers)
            {
                try
                {
                    await handler.Handle(@event);
                }
                catch (Exception)
                {
                    // TODO log
                }
            }
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
    }
}
