namespace Logic.Shows
{
    /// <summary>
    /// Main information about show.
    /// </summary>
    public class ShowHeader
    {
        /// <summary>
        /// Unique ID of the show.
        /// </summary>
        public long ShowId { get; set; }

        /// <summary>
        /// Show title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Show subtitle (usually author or inspiration)
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// The path in picture container of the main show picture.
        /// </summary>
        public string MainShowPicturePath { get; set; }

        /// <summary>
        /// The path in picture container of the main show picture thumbnail.
        /// </summary>
        public string MainShowPictureThumbPath { get; set; }
    }
}
