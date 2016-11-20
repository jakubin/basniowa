using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Website.Infrastructure
{
    /// <summary>
    /// Implementation of <see cref="IJwtBearerTokenProvider"/>.
    /// </summary>
    public class JwtBearerTokenProvider : IJwtBearerTokenProvider
    {
        private JwtBearerAuthenticationOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtBearerTokenProvider"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public JwtBearerTokenProvider(IOptions<JwtBearerAuthenticationOptions> options)
        {
            _options = options.Value;
        }

        /// <inheritdoc/>
        public string CreateToken(string userName, params Claim[] additionalClaims)
        {
            var now = DateTimeOffset.UtcNow;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Name, userName) // this is required so that User.Name is filled in controllers
            };

            if (additionalClaims != null && additionalClaims.Length > 0)
            {
                claims.AddRange(additionalClaims);
            }

            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now.DateTime,
                expires: now.Add(_options.ExpireTimeSpan).DateTime,
                signingCredentials: new SigningCredentials(_options.GetSecurityKey(), SecurityAlgorithms.HmacSha256));
            var handler = new JwtSecurityTokenHandler();
            var encodedJwt = handler.WriteToken(jwt);

            return encodedJwt;
        }
    }
}
