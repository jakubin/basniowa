using System;

namespace DataAccess.Database.Shows
{
    /// <summary>
    /// A picture of a show.
    /// </summary>
    public class ShowPicture
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the show identifier.
        /// </summary>
        public long ShowId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the path to the full image in image container.
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Gets or sets the path to the image minature in image container.
        /// </summary>
        public string ThumbPath { get; set; }

        /// <summary>
        /// Gets or sets the created time (UTC).
        /// </summary>
        public DateTimeOffset CreatedUtc { get; set; }

        /// <summary>
        /// Gets or sets the user who created this entity.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the show owning the image.
        /// </summary>
        public virtual Show Show { get; set; }
    }
}
