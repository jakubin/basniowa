using Common.Cqrs;

namespace Logic.Shows
{
    /// <summary>
    /// Event raised after a show picture has been added.
    /// </summary>
    public class ShowPictureAddedEvent : IEvent
    {
        /// <summary>
        /// Gets or sets the identifier of new show picture.
        /// </summary>
        public long ShowPictureId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of show.
        /// </summary>
        public long ShowId { get; set; }
    }
}