using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Website.Api.Shows
{
    /// <summary>
    /// Model for adding a new show picture.
    /// </summary>
    public class AddShowPictureModel
    {
        /// <summary>
        /// Show ID.
        /// </summary>
        [Required]
        public long ShowId { get; set; }

        /// <summary>
        /// File with show's picture.
        /// Accepted file types: jpg/jpeg, png, gif, bmp, tiff.
        /// </summary>
        [Required]
        public IFormFile Picture { get; set; }

        /// <summary>
        /// Short title/description for the picture.
        /// </summary>
        public string Title { get; set; }
    }
}