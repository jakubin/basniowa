using Common.Cqrs;

namespace Logic.Shows
{
    /// <summary>
    /// Event raised after a show has been added.
    /// </summary>
    public class ShowAddedEvent : IEvent
    {
        /// <summary>
        /// Gets or sets the identifier of new show.
        /// </summary>
        public long ShowId { get; set; }
    }
}
