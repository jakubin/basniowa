using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Website.Infrastructure.ErrorHandling;

namespace Website.Infrastructure.Helpers
{
    /// <summary>
    /// Helper methods for MVC model validation.
    /// </summary>
    public static class ModelValidationExtensions
    {
        private static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var chars = value.ToCharArray();
            chars[0] = char.ToLowerInvariant(chars[0]);

            return new string(chars);
        }

        private static SerializableError ToCamelCase(this ModelStateDictionary modelState)
        {
            var errorModel = new SerializableError(modelState);
            var camelCaseModel = new SerializableError();

            foreach (var item in errorModel)
            {
                camelCaseModel[item.Key.ToCamelCase()] = item.Value;
            }

            return camelCaseModel;
        }

        /// <summary>
        /// Ensures that <paramref name="modelState"/> is valid; otherwise, an exception is thrown.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <exception cref="HttpErrorException">When model is not valid.</exception>
        public static void ThrowIfNotValid(this ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                var result = new BadRequestObjectResult(modelState.ToCamelCase());
                throw new HttpErrorException(result);
            }
        }
    }
}
