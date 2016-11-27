namespace Common.Cqrs
{
    /// <summary>
    /// Interface for a message bus capable of sending commands and publishing events.
    /// </summary>
    /// <seealso cref="Common.Cqrs.ICommandBus" />
    /// <seealso cref="Common.Cqrs.IEventBus" />
    public interface IMessageBus : ICommandBus, IEventBus
    {
    }
}
