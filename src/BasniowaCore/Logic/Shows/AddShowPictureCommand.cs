using System.IO;
using Common.Cqrs;

namespace Logic.Shows
{
    /// <summary>
    /// Command for adding a new picture to a show.
    /// </summary>
    public class AddShowPictureCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the show picture identifier.
        /// </summary>
        public long ShowPictureId { get; set; }

        /// <summary>
        /// Gets or sets the show identifier.
        /// </summary>
        public long ShowId { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file stream.
        /// </summary>
        /// <remarks>
        /// It is the command caller responsibility to close the stream.
        /// </remarks>
        public Stream FileStream { get; set; }

        /// <summary>
        /// Gets or sets if the piture should be the default for the show.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets short title/description for the picture.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the name of the user executing this command.
        /// </summary>
        public string UserName { get; set; }
    }
}