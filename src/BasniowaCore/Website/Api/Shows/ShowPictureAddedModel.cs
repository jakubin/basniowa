using System.ComponentModel.DataAnnotations;

namespace Website.Api.Shows
{
    /// <summary>
    /// Model for confirming that picture was added to the show.
    /// </summary>
    public class ShowPictureAddedModel
    {
        /// <summary>
        /// Unique ID of the show's picture.
        /// </summary>
        [Required]
        public long ShowPictureId { get; set; }
    }
}