using Common.Cqrs;

namespace Logic.Shows
{
    /// <summary>
    /// Event raised after a show has been updated.
    /// </summary>
    public class ShowUpdated : IEvent
    {
        /// <summary>
        /// Gets or sets the identifier of updated show.
        /// </summary>
        public long ShowId { get; set; }
    }
}
