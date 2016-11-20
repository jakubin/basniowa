using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Website.Infrastructure;

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
        /// <returns>Object with token.</returns>
        /// <response code="201">Returns logon result with JWT token.</response>
        /// <response code="400">When request is invalid.</response>
        [Route("authenticate")]
        [HttpPost]
        [ProducesResponseType(typeof(LoggedOnModel), (int)HttpStatusCode.Created)]
        public IActionResult Authenticate([FromBody]UserLogonModel logon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = TokenProvider.CreateToken(logon.UserName);
            var model = new LoggedOnModel
            {
                UserName = logon.UserName,
                Token = token
            };

            return new ObjectResult(model) { StatusCode = StatusCodes.Status201Created };
        }
    }
}
