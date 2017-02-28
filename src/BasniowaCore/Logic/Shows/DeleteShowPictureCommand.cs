using Common.Cqrs;

namespace Logic.Shows
{
    /// <summary>
    /// Command for deleting show picture.
    /// </summary>
    public class DeleteShowPictureCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the ID of show picture to remove.
        /// </summary>
        public long ShowPictureId { get; set; }

        /// <summary>
        /// Gets or sets the name of the current user.
        /// </summary>
        public string UserName { get; set; }
    }
}