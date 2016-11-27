using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Cqrs
{
    /// <summary>
    /// Represents an invocation chain that executes a message handler
    /// running it through a list of defined message processors.
    /// </summary>
    public class MessageProcessingChain
    {
        private readonly object _sync = new object();

        private List<IMessageProcessor> _processors = new List<IMessageProcessor>();

        private MessageHandlerDelegate _handler = null;

        /// <summary>
        /// Adds a processor to processing chain.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <remarks>
        /// This method is thread-safe.
        /// </remarks>
        public void AddProcessor(IMessageProcessor processor)
        {
            lock (_sync)
            {
                _processors.Add(processor);
                _handler = null;
            }
        }

        /// <summary>
        /// Processes a message through current processing chain.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Task representing the async operation.</returns>
        /// <remarks>
        /// This method is thread-safe.
        /// </remarks>
        public async Task Process(MessageProcessingContext context)
        {
            MessageHandlerDelegate handler;
            lock (_sync)
            {
                if (_handler == null)
                {
                    _handler = CreateChain(_processors);
                }

                handler = _handler;
            }

            await handler(context);
        }

        private static MessageHandlerDelegate CreateChain(IEnumerable<IMessageProcessor> processors)
        {
            MessageHandlerDelegate next = context => context.HandleMessage();

            foreach (var processor in processors.Reverse())
            {
                var previous = next;
                next = context => processor.Process(context, previous);
            }

            return next;
        }
    }
}
