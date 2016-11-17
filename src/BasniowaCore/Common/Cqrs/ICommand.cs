namespace Common.Cqrs
{
    /// <summary>
    /// Base interface for CQRS messages.
    /// </summary>
    /// <seealso cref="Common.Cqrs.IMessage" />
    public interface ICommand : IMessage
    {
    }
}