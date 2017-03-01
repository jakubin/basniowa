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
        /// <returns>The file download URL or <c>null</c> if <paramref name="path"/> is <c>null</c>.</returns>
        string GetDownloadUrl(string path);
    }
}