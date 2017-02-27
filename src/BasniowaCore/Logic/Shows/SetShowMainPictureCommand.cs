using Common.Cqrs;

namespace Logic.Shows
{
    /// <summary>
    /// Command to set a show picture as a show default's picture.
    /// </summary>
    public class SetShowMainPictureCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the show identifier.
        /// </summary>
        public long ShowId { get; set; }

        /// <summary>
        /// Gets or sets the show picture identifier to be set as show's default.
        /// </summary>
        public long? ShowPictureId { get; set; }

        /// <summary>
        /// Gets or sets the name of the user executing this command.
        /// </summary>
        public string UserName { get; set; }
    }
}