﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace Common.FileContainers
{
    /// <summary>
    /// Provides read-only access to files in container.
    /// </summary>
    public interface IFileContainerReader
    {
        /// <summary>
        /// Gets all files within the container.
        /// </summary>
        /// <returns>List of container file paths.</returns>
        /// <exception cref="FileContainerException">If any problem occured while accessing the files.</exception>
        Task<string[]> GetAllFiles();

        /// <summary>
        /// Opens specified file within the container for reading.
        /// </summary>
        /// <param name="path">The path to the file in the container.</param>
        /// <returns>The stream with file content. It is the caller responsibility to dispose the stream.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="path"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">When <paramref name="path"/> is invalid.</exception>
        /// <exception cref="FileNotFoundInContainerException">When specified file is not present in the container.</exception>
        /// <exception cref="FileContainerException">If any other problem occured while reading the file.</exception>
        Task<Stream> ReadFile(string path);

        /// <summary>
        /// Checks if the file under specified <paramref name="path"/> exists in container.
        /// </summary>
        /// <param name="path">The path to the file in the container.</param>
        /// <returns>If file at specified path exists.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="path"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">When <paramref name="path"/> is invalid.</exception>
        /// <exception cref="FileContainerException">If any other problem occured while reading the file.</exception>
        Task<bool> Exists(string path);
    }
}