using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Website.Infrastructure.Swagger
{
    /// <summary>
    /// Operation filter that generates correct Swagger documentation
    /// for operations with <see cref="IFormFile"/>.
    /// </summary>
    public class FormFileOperationFilter : IOperationFilter
    {
        private const string FormDataMimeType = "multipart/form-data";

        private static readonly string FirstFormFileProperty = 
            typeof(IFormFile).GetTypeInfo().GetProperties()
            .Select(x=>x.Name)
            .First();

        /// <inheritdoc/>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                return;
            }
            foreach (var parameter in operation.Parameters)
            {
                if (parameter.In == "form")
                {
                    parameter.In = "formData";
                }
            }

            var formFileParameterNames = new SortedSet<string>(
                context.ApiDescription.ParameterDescriptions
                    .Where(x => x.ModelMetadata.ContainerType == typeof(IFormFile))
                    .Select(x => x.Name));

            if (formFileParameterNames.Count == 0)
            {
                return;
            }

            var directFormFileParameterNames = new Queue<string>(
                context.ApiDescription.ActionDescriptor.Parameters
                    .Where(x => x.ParameterType == typeof(IFormFile))
                    .Select(x => x.Name));

            var newParameters = new List<IParameter>();
            foreach (var parameter in operation.Parameters)
            {
                if (formFileParameterNames.Contains(parameter.Name))
                {
                    var newParameter = new NonBodyParameter
                    {
                        In = "formData",
                        Description = "The file to upload.",
                        Required = true,
                        Type = "file"
                    };

                    var index = parameter.Name.LastIndexOf('.');
                    if (index > 0)
                    {
                        newParameter.Name = parameter.Name.Substring(0, index);
                    }
                    else if (parameter.Name == FirstFormFileProperty && directFormFileParameterNames.Count > 0)
                    {
                        newParameter.Name = directFormFileParameterNames.Dequeue();
                    }

                    if (newParameter.Name != null && newParameters.All(x => x.Name != newParameter.Name))
                    {
                        newParameters.Add(newParameter);
                    }
                }
                else
                {
                    newParameters.Add(parameter);
                }
            }

            operation.Parameters = newParameters;

            if (!operation.Consumes.Contains(FormDataMimeType))
            {
                operation.Consumes.Add(FormDataMimeType);
            }
        }
    }
}