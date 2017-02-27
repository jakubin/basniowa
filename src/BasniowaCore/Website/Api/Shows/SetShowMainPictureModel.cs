using System.ComponentModel.DataAnnotations;

namespace Website.Api.Shows
{
    /// <summary>
    /// Command to set a show picture as a show's main picture.
    /// </summary>
    public class SetShowMainPictureModel
    {
        /// <summary>
        /// The show identifier.
        /// </summary>
        [Required]
        public long? ShowId { get; set; }

        /// <summary>
        /// The show picture identifier to be set as show's default (or null to remove).
        /// </summary>
        public long? ShowPictureId { get; set; }
    }
}