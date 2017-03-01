namespace Logic.Shows
{
    /// <summary>
    /// Information about a show picture.
    /// </summary>
    public class ShowPictureData
    {
        /// <summary>
        /// Gets or sets the show picture identifier.
        /// </summary>
        public long ShowPictureId { get; set; }

        /// <summary>
        /// Gets or sets the full image path in the container.
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Gets or sets the image thumb path in the container.
        /// </summary>
        public string ThumbPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this picture is the main show picture.
        /// </summary>
        public bool IsMainShowPicture { get; set; }
    }
}