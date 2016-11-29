using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Website.Infrastructure.Jwt
{
    /// <summary>
    /// Extension methods for registering JWT bearer token authentication.
    /// </summary>
    public static class JwtBearerAuthenticationExtensions
    {
        /// <summary>
        /// Configures the JWT bearer authentication.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void ConfigureJwtBearerAuthentication(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService<IOptions<JwtBearerAuthenticationOptions>>();
            app.ConfigureJwtBearerAuthentication(options.Value);
        }

        /// <summary>
        /// Configures the JWT bearer authentication.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="config">The configuration.</param>
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

        /// <summary>
        /// Adds the JWT bearer authentication.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        public static void AddJwtBearerAuthentication(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<JwtBearerAuthenticationOptions>(configuration.GetSection("JwtBearerAuthentication"));
            services.AddSingleton<IJwtBearerTokenProvider, JwtBearerTokenProvider>();
        }
    }
}
