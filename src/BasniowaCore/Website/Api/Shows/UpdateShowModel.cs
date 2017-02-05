using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Website.Api.Shows
{
    /// <summary>
    /// Model for command updating show.
    /// </summary>
    public class UpdateShowModel
    {
        /// <summary>
        /// Identifier of show
        /// </summary>
        [Required]
        public long ShowId { get; set; }

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
