using System;
using Microsoft.AspNetCore.Mvc;

namespace Website.Infrastructure.ErrorHandling
{
    /// <summary>
    /// Exception thrown from controller when a non-successful response should be returned.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class HttpErrorException : Exception
    {
        /// <summary>
        /// Gets the action result.
        /// </summary>
        public IActionResult ActionResult { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpErrorException"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public HttpErrorException(IActionResult result)
        {
            ActionResult = result;
        }
    }
}
