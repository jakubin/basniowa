namespace Website.Api.Shows
{
    /// <summary>
    /// Detailed information about the show.
    /// </summary>
    public class ShowPictureDetailsModel
    {
        /// <summary>
        /// The show picture identifier.
        /// </summary>
        public long ShowPictureId { get; set; }

        /// <summary>
        /// The URL with full image.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// The URL with image thumb.
        /// </summary>
        public string ThumbUrl { get; set; }

        /// <summary>
        /// The picture's title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// If this picture is the main show's picture.
        /// </summary>
        public bool IsMainShowPicture { get; set; }
    }
}