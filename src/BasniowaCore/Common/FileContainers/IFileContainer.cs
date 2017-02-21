using System;
using System.IO;
using System.Threading.Tasks;

namespace Common.FileContainers
{
    /// <summary>
    /// A container that can store files.
    /// </summary>
    public interface IFileContainer : IFileContainerReader
    {
        /// <summary>
        /// Adds the file to the container under specified <paramref name="path"/>.
        /// </summary>
        /// <remarks>
        /// Caller is expected to dispose <paramref name="contentStream"/> after calling this method.
        /// </remarks>
        /// <param name="path">The path, which the file should be saved under.</param>
        /// <param name="contentStream">The stream containing the file content to be saved.</param>
        /// <returns>Task representing the async operation.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="path"/> or <paramref name="contentStream"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">When <paramref name="path"/> is invalid.</exception>
        /// <exception cref="FileContainerException">If a problem occured while creating the file.</exception>
        Task AddFile(string path, Stream contentStream);

        /// <summary>
        /// Removes the file for specified location.
        /// </summary>
        /// <param name="path">The path to the file in the container.</param>
        /// <returns>Task representing the async operation.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="path"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">When <paramref name="path"/> is invalid.</exception>
        /// <exception cref="FileNotFoundInContainerException">When specified file is not present in the container.</exception>
        /// <exception cref="FileContainerException">If any other problem occured while creating the file.</exception>
        Task RemoveFile(string path);
    }
}