using Microsoft.AspNetCore.Mvc;
using Website.Infrastructure;
using System.Net;

namespace Website.Api.Users
{
    /// <summary>
    /// API controller for user account management and authentication.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        /// <summary>
        /// Gets or sets the JWT bearer token provider.
        /// </summary>
        [InjectService]
        public IJwtBearerTokenProvider TokenProvider { get; set; }

        /// <summary>
        /// Validates provided credentials and generates a JWT Bearer token that can be user for authentication against other API services.
        /// </summary>
        /// <param name="logon">The logon credentials.</param>
        /// <returns>Token</returns>
        [Route("authenticate")]
        [HttpPost]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public IActionResult Authenticate([FromBody]UserLogon logon)
        {
            var token = TokenProvider.CreateToken(logon.UserName);
            return Content(token);
        }
    }
}
