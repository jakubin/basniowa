using System.ComponentModel.DataAnnotations;

namespace Website.Api.Shows
{
    /// <summary>
    /// Model for deleting show.
    /// </summary>
    public class DeleteShowModel
    {
        /// <summary>
        /// Identifier of show to delete
        /// </summary>
        [Required]
        public long ShowId { get; set; }
    }
}