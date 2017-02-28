using System.ComponentModel.DataAnnotations;

namespace Website.Api.Shows
{
    /// <summary>
    /// Main information about show.
    /// </summary>
    public class ShowHeaderModel
    {
        /// <summary>
        /// Unique ID of the show.
        /// </summary>
        [Required]
        public long ShowId { get; set; }

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
        /// The URL of the main show picture.
        /// </summary>
        public string MainShowPictureUrl { get; set; }

        /// <summary>
        /// The URL of the main show picture thumbnail.
        /// </summary>
        public string MainShowPictureThumbUrl { get; set; }
    }
}
