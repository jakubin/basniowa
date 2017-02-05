using System.ComponentModel.DataAnnotations;

namespace Website.Api.Shows
{
    /// <summary>
    /// Contains details about added show.
    /// </summary>
    public class ShowAddedModel
    {
        /// <summary>
        /// Identifier of added show
        /// </summary>
        [Required]
        public long ShowId { get; set; }  
    }
}