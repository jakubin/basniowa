namespace Common.Cqrs
{
    /// <summary>
    /// Base interface for CQRS events.
    /// </summary>
    /// <seealso cref="Common.Cqrs.IMessage" />
    public interface IEvent : IMessage
    {
    }
}