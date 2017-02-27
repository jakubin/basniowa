using Common.Cqrs;

namespace Logic.Shows
{
    /// <summary>
    /// Event published when main show picture changes.
    /// </summary>
    public class ShowMainPictureSetEvent : IEvent
    {
        /// <summary>
        /// Gets or sets the show identifier.
        /// </summary>
        public long ShowId { get; set; }

        /// <summary>
        /// Gets or sets the show picture identifier, which was set as main.
        /// </summary>
        public long? ShowPictureId { get; set; }
    }
}