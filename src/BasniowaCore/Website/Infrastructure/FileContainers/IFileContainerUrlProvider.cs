using System;
using Common.FileContainers;

namespace Website.Infrastructure.FileContainers
{
    /// <summary>
    /// Download URL provider for file container.
    /// </summary>
    public interface IFileContainerUrlProvider
    {
        /// <summary>
        /// Gets the download URL for specified file.
        /// </summary>
        /// <param name="path">The file path within container.</param>
        /// <returns>The file download URL.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="path"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">When <paramref name="path"/> is invalid.</exception>
        string GetDownloadUrl(string path);
    }
}