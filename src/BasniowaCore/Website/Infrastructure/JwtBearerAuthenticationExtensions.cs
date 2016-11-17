using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Website.Infrastructure
{
    public static class JwtBearerAuthenticationExtensions
    {
        public static void ConfigureJwtBearerAuthentication(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService<IOptions<JwtBearerAuthenticationOptions>>();
            app.ConfigureJwtBearerAuthentication(options.Value);
        }

        public static void ConfigureJwtBearerAuthentication(this IApplicationBuilder app, JwtBearerAuthenticationOptions config)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.IssuerSigningKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = config.Issuer,
                ValidateAudience = true,
                ValidAudience = config.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });
        }

        public static void AddJwtBearerAuthentication(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<JwtBearerAuthenticationOptions>(configuration.GetSection("JwtBearerAuthentication"));
            services.AddSingleton<IJwtBearerTokenProvider, JwtBearerTokenProvider>();
        }
    }
}
