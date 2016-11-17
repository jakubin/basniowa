using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Website.Infrastructure
{
    public class JwtBearerAuthenticationOptions
    {
        public string IssuerSigningKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public TimeSpan ExpireTimeSpan { get; set; }

        public SymmetricSecurityKey GetSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(IssuerSigningKey));
        }
    }
}
