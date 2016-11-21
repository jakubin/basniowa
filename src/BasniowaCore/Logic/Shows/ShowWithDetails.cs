using System.Collections.Generic;

namespace Logic.Shows
{
    /// <summary>
    /// The show with all details.
    /// </summary>
    public class ShowWithDetails
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

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
    }
}
