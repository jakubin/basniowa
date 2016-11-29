using Microsoft.AspNetCore.Mvc.Filters;

namespace Website.Infrastructure.ErrorHandling
{
    /// <summary>
    /// Exception filter replacing thrown <see cref="HttpErrorException"/> with
    /// action result within the exception.
    /// </summary>
    public class HttpErrorFilter : IExceptionFilter, IFilterMetadata
    {
        /// <inheritdoc/>
        public void OnException(ExceptionContext context)
        {
            var httpError = context.Exception as HttpErrorException;
            if (httpError == null)
            {
                return;
            }

            context.Result = httpError.ActionResult;
        }
    }
}
