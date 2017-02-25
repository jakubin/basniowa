using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.FileContainers;

namespace Testing.FileContainers
{
    /// <summary>
    /// In memory implementation of <see cref="IFileContainer"/>
    /// mainly for unit testing purposes.
    /// </summary>
    public class InMemoryFileContainer : IFileContainer
    {
        private readonly ConcurrentDictionary<string, byte[]> _files
            = new ConcurrentDictionary<string, byte[]>();

        /// <inheritdoc/>
        public virtual Task<string[]> GetAllFiles()
        {
            return Task.FromResult(_files.Keys.ToArray());
        }

        /// <inheritdoc/>
        public virtual Task<Stream> ReadFile(string path)
        {
            Guard.NotNull(path, nameof(path));
            ValidateContainerPath(path, nameof(path));

            byte[] contentBytes;
            bool exists = _files.TryGetValue(path, out contentBytes);

            if (!exists)
            {
                throw new FileNotFoundInContainerException(path);
            }

            return Task.FromResult<Stream>(new MemoryStream(contentBytes, false));
        }

        /// <inheritdoc/>
        public Task<bool> Exists(string path)
        {
            Guard.NotNull(path, nameof(path));
            ValidateContainerPath(path, nameof(path));

            return Task.FromResult(_files.ContainsKey(path));
        }

        /// <inheritdoc/>
        public virtual async Task AddFile(string path, Stream contentStream)
        {
            Guard.NotNull(path, nameof(path));
            Guard.NotNull(contentStream, nameof(contentStream));
            ValidateContainerPath(path, nameof(path));

            byte[] contentBytes;
            using (var memoryStream = new MemoryStream())
            {
                await contentStream.CopyToAsync(memoryStream);
                contentBytes = memoryStream.ToArray();
            }

            bool added = _files.TryAdd(path, contentBytes);

            if (!added)
            {
                throw new FileContainerException(
                    $"File with path \"{path}\" already exists in container.");
            }
        }

        /// <inheritdoc/>
        public virtual Task RemoveFile(string path)
        {
            Guard.NotNull(path, nameof(path));
            ValidateContainerPath(path, nameof(path));

            byte[] contentBytes;
            bool removed = _files.TryRemove(path, out contentBytes);

            if (!removed)
            {
                throw new FileNotFoundInContainerException(path);
            }

            return Task.CompletedTask;
        }

        private static void ValidateContainerPath(string path, string paramName)
        {
            if (!FileContainerPath.IsValid(path))
            {
                throw new ArgumentException(
                    $"Path ({path}) is not valid for file container.",
                    paramName);
            }
        }
    }
}