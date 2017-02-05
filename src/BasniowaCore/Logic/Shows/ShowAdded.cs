using Common.Cqrs;

namespace Logic.Shows
{
    /// <summary>
    /// Event raised after a show has been added.
    /// </summary>
    public class ShowAdded : IEvent
    {
        /// <summary>
        /// Gets or sets the identifier of new show.
        /// </summary>
        public long ShowId { get; set; }
    }
}
