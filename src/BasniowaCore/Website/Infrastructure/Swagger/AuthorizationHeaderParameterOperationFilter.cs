using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Authorization;
using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Generator;

namespace Website.Infrastructure.Swagger
{
    /// <summary>
    /// Swashbuckle operation filter that adds information that Authorization header is required
    /// for methods that require authorization.
    /// </summary>
    /// <seealso cref="Swashbuckle.SwaggerGen.Generator.IOperationFilter" />
    public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        /// <inheritdoc/>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter).OfType<AuthorizeFilter>().Any();
            var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);

            if (isAuthorized && !allowAnonymous)
            {
                if (operation.Parameters == null)
                {
                    operation.Parameters = new List<IParameter>();
                }

                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "Authorization",
                    In = "header",
                    Description = "JWT Bearer access token",
                    Required = true,
                    Type = "string"
                });
            }
        }
    }
}
