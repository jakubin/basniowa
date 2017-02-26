using Common.Cqrs;

namespace Logic.Shows
{
    /// <summary>
    /// Event raised when show gets deleted.
    /// </summary>
    public sealed class ShowDeletedEvent : IEvent
    {
        /// <summary>
        /// Gets the show identifier.
        /// </summary>
        public long ShowId { get; set; }
    }
}