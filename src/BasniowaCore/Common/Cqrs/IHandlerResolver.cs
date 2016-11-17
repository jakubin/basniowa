using System.Collections.Generic;

namespace Common.Cqrs
{
    /// <summary>
    /// Resolves command handlers for specific command/event type.
    /// </summary>
    public interface IHandlerResolver
    {
        /// <summary>
        /// Resolves command handlers for specific command/event type.
        /// </summary>
        /// <typeparam name="T">Type of command or message.</typeparam>
        /// <returns>List of matching handlers.</returns>
        IList<IHandler<T>> Resolve<T>() where T : IMessage;
    }
}
