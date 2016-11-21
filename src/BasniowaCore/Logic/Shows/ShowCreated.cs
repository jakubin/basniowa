using Common.Cqrs;

namespace Logic.Shows
{
    /// <summary>
    /// Event raised after a show has been created.
    /// </summary>
    public class ShowCreated : IEvent
    {
        /// <summary>
        /// Gets or sets the identifier of new event.
        /// </summary>
        public long Id { get; set; }
    }
}
