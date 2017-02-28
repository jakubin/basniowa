using Common.Cqrs;

namespace Logic.Shows
{
    /// <summary>
    /// Event published when show picture is deleted.
    /// </summary>
    public class ShowPictureDeletedEvent : IEvent
    {
        /// <summary>
        /// Gets or sets the show identifier.
        /// </summary>
        public long ShowId { get; set; }

        /// <summary>
        /// Gets or sets the show picture identifier, which has been deleted.
        /// </summary>
        public long ShowPictureId { get; set; }
    }
}