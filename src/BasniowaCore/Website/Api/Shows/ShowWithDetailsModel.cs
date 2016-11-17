using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Website.Api.Shows
{
    /// <summary>
    /// Show with additional information.
    /// </summary>
    public class ShowWithDetailsModel
    {
        /// <summary>
        /// Unique ID of the show.
        /// </summary>
        [Required]
        public long Id { get; set; }

        /// <summary>
        /// Show title.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Show subtitle (usually author or inspiration)
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// Description of the show.
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Properties of the show (director, etc.).
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }
    }
}
