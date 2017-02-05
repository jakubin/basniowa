using System.Collections.Generic;
using Common.Cqrs;

namespace Logic.Shows
{
    /// <summary>
    /// Command to update an existing show.
    /// </summary>
    public class UpdateShowCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the new show ID.
        /// </summary>
        public long ShowId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the subtitle.
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Gets or sets the name of the user executing this command.
        /// </summary>
        public string UserName { get; set; }
    }
}
