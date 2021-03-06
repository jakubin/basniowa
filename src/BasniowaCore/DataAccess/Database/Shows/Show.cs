﻿using System;
using System.Collections.Generic;

namespace DataAccess.Database.Shows
{
    /// <summary>
    /// Represents a show.
    /// </summary>
    public partial class Show
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
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the subtitle.
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// Gets or sets the ID of the main picture for the show.
        /// </summary>
        public long? MainShowPictureId { get; set; }

        /// <summary>
        /// Gets or sets the created UTC.
        /// </summary>
        public DateTimeOffset CreatedUtc { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the modified UTC.
        /// </summary>
        public DateTimeOffset ModifiedUtc { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the show properties.
        /// </summary>
        public virtual ICollection<ShowProperty> ShowProperties { get; set; } = new List<ShowProperty>();

        /// <summary>
        /// Gets or sets the show pictures.
        /// </summary>
        public virtual ICollection<ShowPicture> ShowPictures { get; set; } = new List<ShowPicture>();

        /// <summary>
        /// Gets or sets the main show picture.
        /// </summary>
        public virtual ShowPicture MainShowPicture { get; set; }
    }
}
