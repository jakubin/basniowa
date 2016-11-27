namespace Common.Cqrs
{
    /// <summary>
    /// Service allowing to send commands, which are processed by an extendable chain of processors.
    /// </summary>
    /// <seealso cref="ICommandSender" />
    public interface ICommandBus : ICommandSender
    {
        /// <summary>
        /// Adds the command handler processor to command processing chain.
        /// </summary>
        /// <param name="processor">The processor.</param>
        void AddCommandHandlerProcessor(IMessageProcessor processor);
    }
}