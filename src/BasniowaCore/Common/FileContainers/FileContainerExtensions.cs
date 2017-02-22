using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Common.FileContainers
{
    /// <summary>
    /// Extension methods for <see cref="IFileContainer"/> and <see cref="IFileContainerReader"/>.
    /// </summary>
    public static class FileContainerExtensions
    {
        #region IFileContainer.AddFile extensions

        /// <summary>
        /// Adds the binary file to the container under specified <paramref name="path" />.
        /// </summary>
        /// <param name="container">The file container.</param>
        /// <param name="path">The path, which the file should be saved under.</param>
        /// <param name="fileBytes">The file bytes.</param>
        /// <returns>Task representing the async operation.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="container"/> or <paramref name="path" /> or <paramref name="fileBytes" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">When <paramref name="path" /> is invalid.</exception>
        /// <exception cref="FileContainerException">If a problem occured while creating the file.</exception>
        public static async Task AddFile(this IFileContainer container, string path, byte[] fileBytes)
        {
            Guard.NotNull(container, nameof(container));
            Guard.NotNull(fileBytes, nameof(fileBytes));

            using (var memoryStream = new MemoryStream(fileBytes, false))
            {
                await container.AddFile(path, memoryStream);
            }
        }

        /// <summary>
        /// Adds the text file to the container under specified <paramref name="path" />.
        /// </summary>
        /// <param name="container">The file container.</param>
        /// <param name="path">The path, which the file should be saved under.</param>
        /// <param name="fileText">The file text.</param>
        /// <param name="encoding">The encoding to use to persist the file. Use <c>null</c> for default (<see cref="Encoding.UTF8"/>).</param>
        /// <returns>Task representing the async operation.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="container"/> or <paramref name="path" /> or <paramref name="fileText" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">When <paramref name="path" /> is invalid.</exception>
        /// <exception cref="FileContainerException">If a problem occured while creating the file.</exception>
        public static async Task AddFile(this IFileContainer container, string path, string fileText, Encoding encoding = null)
        {
            Guard.NotNull(container, nameof(container));
            Guard.NotNull(fileText, nameof(fileText));

            encoding = encoding ?? Encoding.UTF8;

            using (var memoryStream = new MemoryStream(encoding.GetBytes(fileText), false))
            {
                await container.AddFile(path, memoryStream);
            }
        }

        #endregion

        #region IFileContainerReader.ReadFile extensions

        /// <summary>
        /// Adds the binary file to the container under specified <paramref name="path" />.
        /// </summary>
        /// <param name="container">The file container.</param>
        /// <param name="path">The path, which the file should be saved under.</param>
        /// <returns>Byte array with file content.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="container"/> or <paramref name="path" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">When <paramref name="path" /> is invalid.</exception>
        /// <exception cref="FileContainerException">If a problem occured while creating the file.</exception>
        public static async Task<byte[]> ReadAllBytes(this IFileContainerReader container, string path)
        {
            Guard.NotNull(container, nameof(container));

            using (var stream = await container.ReadFile(path))
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Adds the binary file to the container under specified <paramref name="path" />.
        /// </summary>
        /// <param name="container">The file container.</param>
        /// <param name="path">The path, which the file should be saved under.</param>
        /// <param name="encoding">The encoding to use to read the file. Use <c>null</c> for default (<see cref="Encoding.UTF8"/>).</param>
        /// <returns>String with the file content.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="container"/> or <paramref name="path" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">When <paramref name="path" /> is invalid.</exception>
        /// <exception cref="FileContainerException">If a problem occured while creating the file.</exception>
        public static async Task<string> ReadAllText(this IFileContainerReader container, string path, Encoding encoding = null)
        {
            Guard.NotNull(container, nameof(container));

            encoding = encoding ?? Encoding.UTF8;

            using (var stream = await container.ReadFile(path))
            using (var streamReader = new StreamReader(stream, encoding))
            {
                return await streamReader.ReadToEndAsync();
            }
        }

        #endregion
    }
}