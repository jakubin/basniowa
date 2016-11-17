using System.Threading.Tasks;

namespace Common.Cqrs
{
    /// <summary>
    /// Interface for CQRS message handler (command or event).
    /// </summary>
    /// <typeparam name="T">Type of message that can be handled.</typeparam>
    public interface IHandler<T> where T: IMessage
    {
        /// <summary>
        /// Handles the message (asynchronously).
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task representing the async operation.</returns>
        Task Handle(T message);
    }
}
