using System;
using Common.FileContainers;

namespace DataAccess.Disk
{
    /// <summary>
    /// Interface for read methods specific for file container based on directory on physical disk.
    /// </summary>
    public interface IPhysicalFileContainerReader : IFileContainerReader
    {
        /// <summary>
        /// Gets the physical path for specific container file.
        /// </summary>
        /// <param name="path">The path to the file in the container.</param>
        /// <returns>The physical location of the file on disk.</returns>
        /// <remarks>
        /// The container does not check for file existance.
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <paramref name="path"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">When <paramref name="path"/> is invalid.</exception>
        string GetFilePhysicalPath(string path);
    }
}