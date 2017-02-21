using System;

namespace Common.FileContainers
{
    /// <summary>
    /// Exception thrown when file under specified path was not found in file container.
    /// </summary>
    /// <seealso cref="Common.FileContainers.FileContainerException" />
    public class FileNotFoundInContainerException : FileContainerException
    {
        /// <summary>
        /// Gets the relative path to the file within the container.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileNotFoundInContainerException"/> class.
        /// </summary>
        /// <param name="path">The relative path to the file within the container.</param>
        public FileNotFoundInContainerException(string path)
            : base(GetMessage(path))
        {
            Path = path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileNotFoundInContainerException"/> class.
        /// </summary>
        /// <param name="path">The relative path to the file within the container.</param>
        /// <param name="innerException">The inner exception.</param>
        public FileNotFoundInContainerException(string path, Exception innerException)
            : base(GetMessage(path), innerException)
        {
            Path = path;
        }

        private static string GetMessage(string path)
            => $"File is not present in the container under path \"{path}\".";
    }
}