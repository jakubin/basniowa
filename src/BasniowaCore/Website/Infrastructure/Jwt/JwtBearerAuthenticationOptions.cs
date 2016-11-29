using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Website.Infrastructure.Jwt
{
    /// <summary>
    /// JWT bearer authentication options.
    /// </summary>
    public class JwtBearerAuthenticationOptions
    {
        /// <summary>
        /// Gets or sets the issuer signing key.
        /// </summary>
        public string IssuerSigningKey { get; set; }

        /// <summary>
        /// Gets or sets the issuer.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the audience.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the expire time span.
        /// </summary>
        public TimeSpan ExpireTimeSpan { get; set; }

        /// <summary>
        /// Gets the security key.
        /// </summary>
        /// <returns>The security key.</returns>
        public SymmetricSecurityKey GetSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(IssuerSigningKey));
        }
    }
}
