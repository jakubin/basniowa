using System;
using System.IO;
using System.Threading.Tasks;

namespace Common.FileContainers
{
    /// <summary>
    /// A container that can store files.
    /// </summary>
    public interface IFileContainer
    {
        /// <summary>
        /// Adds the file to the container under specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path, which the file should be saved under.</param>
        /// <param name="contentStream">The stream containing the file content to be saved.</param>
        /// <returns>Task representing the async operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// When <paramref name="path"/> or <paramref name="contentStream"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// When <paramref name="path"/> is invalid.
        /// </exception>
        Task AddFile(string path, Stream contentStream);

        /// <summary>
        /// Removes the file for specified location.
        /// Does not throw exception is file does not exist.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task representing the async operation.</returns>
        Task RemoveFile(string path);
    }
}