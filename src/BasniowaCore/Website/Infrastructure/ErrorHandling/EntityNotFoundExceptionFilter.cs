using System.Net.Http;
using Logic.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Website.Infrastructure.ErrorHandling
{
    /// <summary>
    /// Exception filter, which checks if <see cref="EntityNotFoundException"/> was thrown
    /// and transforms it to <see cref="ErrorModel"/> response with HTTP status code depending on
    /// HTTP method:
    /// - 404 (Not Found) for GET requests 
    /// - 422 (Unprocessable Entity) for other request types
    /// </summary>
    public class EntityNotFoundExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// The type of error set in <see cref="ErrorModel.Type"/>.
        /// </summary>
        public const string BusinessRuleErrorType = "EntityNotFound";

        /// <summary>
        /// The user message set in <see cref="ErrorModel.Message"/>.
        /// </summary>
        public const string UserMessage = "Entity was not found.";

        /// <inheritdoc/>
        public void OnException(ExceptionContext context)
        {
            var entityNotFoundException = context.Exception as EntityNotFoundException;
            if (entityNotFoundException == null)
            {
                return;
            }

            var details = new EntityNotFoundModel
            {
                EntityType = entityNotFoundException.EntityType.Name,
                EntityKey = entityNotFoundException.EntityKey
            };
            var errorModel = new ErrorModel
            {
                Type = BusinessRuleErrorType,
                Message = UserMessage,
                Details = details
            };

            var statusCode = context.HttpContext.Request.Method == HttpMethod.Get.Method
                ? 404  // Not Found
                : 422; // Unprocessable Entity
            context.Result = new ObjectResult(errorModel)
            {
                StatusCode = statusCode 
            };
        }
    }
}