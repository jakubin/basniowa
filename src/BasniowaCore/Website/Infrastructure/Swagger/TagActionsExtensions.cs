using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Website.Infrastructure.Swagger
{
    /// <summary>
    /// Extensions for tagging actions in Swagger.
    /// Tags are used to group actions in Swagger UI.
    /// </summary>
    public static class TagActionsExtensions
    {
        /// <summary>
        /// Tags the actions by controller namespace ending.
        /// For instance, if controller is in namespace Api.Shows, 
        /// the tag (i.e. group in Swagger UI) will be "Shows".
        /// </summary>
        /// <param name="options">The Swagger gen options.</param>
        public static void TagActionsByNamespaceEnding(this SwaggerGenOptions options)
        {
            options.TagActionsBy(ExtractNamespaceEnding);
        }

        private static string ExtractNamespaceEnding(ApiDescription api)
        {
            var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
            return controllerActionDescriptor?.ControllerTypeInfo.Namespace
                .Split('.')
                .LastOrDefault();
        }
    }
}