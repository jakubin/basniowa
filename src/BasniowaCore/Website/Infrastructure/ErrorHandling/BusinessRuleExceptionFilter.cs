using Logic.Common.BusinessRules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Website.Infrastructure.ErrorHandling
{
    /// <summary>
    /// Exception filter, which checks if <see cref="BusinessRuleException"/> was thrown
    /// and transforms it to <see cref="ErrorModel"/> response with HTTP status code 422 (Unprocessable Entity).
    /// </summary>
    public class BusinessRuleExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// The type of error set in <see cref="ErrorModel.Type"/>.
        /// </summary>
        public const string BusinessRuleErrorType = "BusinessRuleViolation";

        /// <inheritdoc/>
        public void OnException(ExceptionContext context)
        {
            var businessRuleException = context.Exception as BusinessRuleException;
            if (businessRuleException == null)
            {
                return;
            }

            var businessRule = businessRuleException.GetBusinessRule();
            var errorModel = new ErrorModel
            {
                Type = BusinessRuleErrorType,
                Message = businessRule.GetUserMessage(),
                Details = businessRule
            };

            context.Result = new ObjectResult(errorModel)
            {
                StatusCode = 422 // Unprocessable Entity
            };
        }
    }
}