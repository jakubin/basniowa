using System.ComponentModel.DataAnnotations;

namespace Website.Api.Shows
{
    /// <summary>
    /// The model for deleting a show picture.
    /// </summary>
    public class DeleteShowPictureModel
    {
        /// <summary>
        /// The ID of show picture to remove.
        /// </summary>
        [Required]
        public long? ShowPictureId { get; set; }
    }
}