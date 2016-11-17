using System.Security.Claims;

namespace Website.Infrastructure
{
    public interface IJwtBearerTokenProvider
    {
        string CreateToken(string userName, params Claim[] additionalClaims);
    }

}
