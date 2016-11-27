using System;
using System.Threading.Tasks;

namespace Common.Cqrs
{
    /// <summary>
    /// The context of processing a message (command or event).
    /// </summary>
    public class MessageProcessingContext
    {
        private readonly Func<Task> _handleMessage;

        /// <summary>
        /// Gets the message.
        /// </summary>
        public IMessage Message { get; }

        /// <summary>
        /// Gets the message handler.
        /// </summary>
        /// <remarks>
        /// May be <c>null</c> if used when event is published, but not dispatched yet to particular handlers.
        /// </remarks>
        public IHandler Handler { get; }

        private MessageProcessingContext(IMessage message, IHandler handler, Func<Task> handleMessage)
        {
            Message = message;
            Handler = handler;
            _handleMessage = handleMessage;
        }

        /// <summary>
        /// Causes handler to handle the message.
        /// </summary>
        /// <returns>Task representing async operation.</returns>
        public async Task HandleMessage()
        {
            await _handleMessage();
        }

        /// <summary>
        /// Creates a new instance of <see cref="MessageProcessingContext"/>
        /// based on strongly typed handler and message.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="message"/> or <paramref name="handler"/> is <c>null</c>.</exception>
        public static MessageProcessingContext Create<T>(T message, IHandler<T> handler)
            where T : IMessage
        {
            Guard.NotNull(message, nameof(message));
            Guard.NotNull(handler, nameof(handler));

            return new MessageProcessingContext(
                message,
                handler,
                () => handler.Handle(message));
        }

        /// <summary>
        /// Creates a new instance of <see cref="MessageProcessingContext" />
        /// based on message and a function to handle it.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="handleMessage">The message handler delegate.</param>
        /// <returns>
        /// The created instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">When <paramref name="message" /> or <paramref name="handleMessage" /> is <c>null</c>.</exception>
        public static MessageProcessingContext Create(IMessage message, Func<Task> handleMessage)
        {
            Guard.NotNull(message, nameof(message));
            Guard.NotNull(handleMessage, nameof(handleMessage));

            return new MessageProcessingContext(
                message,
                null,
                handleMessage);
        }
    }
}
