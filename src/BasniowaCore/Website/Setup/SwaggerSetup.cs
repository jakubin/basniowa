using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Website.Infrastructure.Swagger;

namespace Website.Setup
{
    /// <summary>
    /// Setup logic for Swagger in application.
    /// </summary>
    public static class SwaggerSetup
    {
        /// <summary>
        /// Configures the services collection for Swagger.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "Basniowa API", Version = "v1" });
                var currentAssemblyPath = typeof(Startup).GetTypeInfo().Assembly.Location;
                var xmlCommentsPath = Path.ChangeExtension(currentAssemblyPath, "xml");

                options.IncludeXmlComments(xmlCommentsPath);
                options.DescribeAllEnumsAsStrings();

                options.TagActionsByNamespaceEnding();

                options.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                options.OperationFilter<FormFileOperationFilter>();
            });
        }

        /// <summary>
        /// Enables swagger generation and UI in the application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The hosting environment.</param>
        public static void UseAppSwagger(this IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }
        }
    }
}