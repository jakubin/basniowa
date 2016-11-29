using System.Security.Claims;

namespace Website.Infrastructure.Jwt
{
    /// <summary>
    /// Creates JWT bearer tokens.
    /// </summary>
    public interface IJwtBearerTokenProvider
    {
        /// <summary>
        /// Creates the token.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="additionalClaims">The additional claims.</param>
        /// <returns>Encoded token.</returns>
        string CreateToken(string userName, params Claim[] additionalClaims);
    }
}
