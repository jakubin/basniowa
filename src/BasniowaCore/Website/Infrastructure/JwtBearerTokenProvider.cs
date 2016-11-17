using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Website.Infrastructure
{
    public class JwtBearerTokenProvider : IJwtBearerTokenProvider
    {
        private JwtBearerAuthenticationOptions _options;

        public JwtBearerTokenProvider(IOptions<JwtBearerAuthenticationOptions> options)
        {
            _options = options.Value;
        }

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
