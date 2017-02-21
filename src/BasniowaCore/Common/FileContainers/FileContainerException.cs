using System;

namespace Common.FileContainers
{
    /// <summary>
    /// Exception thrown when there is a problem accessing file in file container.
    /// </summary>
    public class FileContainerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileContainerException"/> class.
        /// </summary>
        public FileContainerException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileContainerException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public FileContainerException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileContainerException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public FileContainerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}