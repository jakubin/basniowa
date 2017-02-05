using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Website.Api.Shows
{
    /// <summary>
    /// Model for command adding show.
    /// </summary>
    public class AddShowModel
    {
        /// <summary>
        /// Show's title
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// Show's subtitle (usually author or inspiration)
        /// </summary>
        [MaxLength(500)]
        public string Subtitle { get; set; }

        /// <summary>
        /// Show's description
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Show's properties (director, actors, etc.)
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }
    }
}
