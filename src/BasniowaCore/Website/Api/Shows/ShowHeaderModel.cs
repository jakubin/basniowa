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
        public long Id { get; set; }

        /// <summary>
        /// Show title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Show subtitle (usually author or inspiration)
        /// </summary>
        public string Subtitle { get; set; }
    }
}
