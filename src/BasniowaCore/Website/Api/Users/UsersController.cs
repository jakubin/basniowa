using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Website.Infrastructure;
using Website.Infrastructure.Helpers;
using Website.Infrastructure.Jwt;

namespace Website.Api.Users
{
    /// <summary>
    /// API controller for user account management and authentication.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/users")]
    [Produces(ContentTypes.ApplicationJson)]
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
        /// <response code="200">When authentication succeeds, returns logon result with JWT token.</response>
        /// <response code="400">When request is invalid.</response>
        [Route("authenticate")]
        [HttpPost]
        public Task<LoggedOnModel> Authenticate([FromBody]UserLogonModel logon)
        {
            ModelState.ThrowIfNotValid();

            // TODO: provide real authentication
            var token = TokenProvider.CreateToken(logon.UserName);
            var model = new LoggedOnModel
            {
                UserName = logon.UserName,
                Token = token
            };

            return Task.FromResult(model);
        }
    }
}
