using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Website.Api.Shows;
using Website.Infrastructure;

namespace Website.Api.Pictures
{
    /// <summary>
    /// API controller for managing pictures
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/pictures")]
    [Produces(ContentTypes.ApplicationJson)]
    public class PicturesCommandController : Controller
    {
        private const string GroupName = "Pictures";

        [HttpPost]
        [Route("commands/add")]
        //[Authorize]
        [Consumes(ContentTypes.MultipartFormData, ContentTypes.ApplicationXWwwFormUrlEncoded)]
        [ApiExplorerSettings(GroupName = GroupName)]
        public void Add([FromForm] SomeModel model, IFormFile file2)
        {
        }
    }

    public class SomeModel
    {
        /// <summary>
        /// THE TITLE
        /// </summary>
        public string Title { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// THE FILE.
        /// </summary>
        public IFormFile File { get; set; }
    }
}