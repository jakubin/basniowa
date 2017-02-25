using System.Threading.Tasks;

namespace Common.Cqrs
{
    /// <summary>
    /// Interface for message processor - object that is a part of message (command or event)
    /// processing chain and can perform additional actions before or after the actual handler
    /// is executed.
    /// </summary>
    public interface IMessageProcessor
    {
        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <remarks>
        /// The implementation should call <paramref name="next"/> exactly once, so that actual handling can occur.
        /// </remarks>
        /// <param name="context">The context message processing.</param>
        /// <param name="next">The delegate to continue execution.</param>
        /// <returns>Task representing the async operation.</returns>
        Task Process(MessageProcessingContext context, MessageHandlerDelegate next);
    }
}
