using System;
using System.Threading.Tasks;

namespace Common.Cqrs
{
    /// <summary>
    /// Interface for command publisher.
    /// </summary>
    public interface ICommandPublisher
    {
        /// <summary>
        /// Publishes the specified command.
        /// </summary>
        /// <typeparam name="T">Type of command.</typeparam>
        /// <param name="command">The command.</param>
        /// <returns>Task representing the async operation.</returns>
        Task PublishCommand<T>(T command) where T : ICommand;
    }
}
