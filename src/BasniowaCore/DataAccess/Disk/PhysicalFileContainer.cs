using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.CodeAnalysis;
using Common.FileContainers;

namespace DataAccess.Disk
{
    /// <summary>
    /// A <see cref="IFileContainer"/> implementation that is based on physical file system.
    /// All files are stored within provided root directory.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PhysicalFileContainer : IFileContainer
    {
        private readonly string _rootPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalFileContainer"/> class.
        /// </summary>
        /// <remarks>
        /// If provided <paramref name="rootPath"/> doesn't exist,
        /// it will be created with first file added.
        /// </remarks>
        /// <param name="rootPath">The root path for the container.</param>
        public PhysicalFileContainer(string rootPath)
        {
            Guard.NotNull(rootPath, nameof(rootPath));

            _rootPath = rootPath;
        }

        /// <inheritdoc/>
        public Task<string[]> GetAllFiles()
        {
            try
            {
                var root = new DirectoryInfo(_rootPath);
                if (!root.Exists)
                {
                    return Task.FromResult(new string[0]);
                }

                var rootFullPath = root.FullName + Path.DirectorySeparatorChar;
                var allFiles = root.GetFiles("*", SearchOption.AllDirectories);
                var relativePaths = allFiles
                    .Where(f => f.FullName.StartsWith(rootFullPath))
                    .Select(f => f.FullName.Substring(rootFullPath.Length));

                var containerPaths = relativePaths
                    .Select(TryConvertToContainerPath)
                    .Where(x => x != null)
                    .ToArray();

                return Task.FromResult(containerPaths);
            }
            catch (IOException exception)
            {
                throw new FileContainerException(
                    "An error occured while accessing files within the container. See inner exception for details.",
                    exception);
            }
        }

        /// <inheritdoc/>
        public Task<Stream> ReadFile(string path)
        {
            Guard.NotNull(path, nameof(path));

            if (!FileContainerPath.IsValid(path))
            {
                throw new ArgumentException(
                    $"Path ({path}) is not valid for file container.");
            }

            try
            {
                var physicalPath = GetPhysicalPath(path);
                return Task.FromResult<Stream>(File.OpenRead(physicalPath));
            }
            catch (IOException ex)
            {
                throw new FileContainerException(
                    $"An error occured while reading file in the container under path \"{path}\". See inner exception for details.",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task AddFile(string path, Stream contentStream)
        {
            Guard.NotNull(path, nameof(path));
            Guard.NotNull(contentStream, nameof(contentStream));

            if (!FileContainerPath.IsValid(path))
            {
                throw new ArgumentException(
                    $"Path ({path}) is not valid for file container.");
            }

            var physicalPath = GetPhysicalPath(path);
            
            // ensure directory exist
            var physicalDirectoryPath = Path.GetDirectoryName(physicalPath);
            Directory.CreateDirectory(physicalDirectoryPath);

            try
            {
                using (FileStream fileStream = File.Open(physicalPath, FileMode.CreateNew, FileAccess.Write))
                {
                    await contentStream.CopyToAsync(fileStream);
                }
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundInContainerException(path, ex);
            }
            catch (IOException ex)
            {
                TryDeleteFile(physicalPath);
                throw new FileContainerException(
                    $"An error occured while creating file within the container under path \"{path}\". See inner exception for details.",
                    ex);
            }
            catch (Exception)
            {
                TryDeleteFile(physicalPath);
            }
        }

        /// <inheritdoc/>
        public Task RemoveFile(string path)
        {
            Guard.NotNull(path, nameof(path));

            if (!FileContainerPath.IsValid(path))
            {
                throw new ArgumentException(
                    $"Path ({path}) is not valid for file container.");
            }

            var physicalPath = GetPhysicalPath(path);

            try
            {
                File.Delete(physicalPath);
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundInContainerException(path, ex);
            }
            catch (Exception ex)
            {
                throw new FileContainerException(
                    $"An error occured while removing file within the container under path \"{path}\". See inner exception for details.",
                    ex);
            }

            return Task.CompletedTask;
        }

        private string GetPhysicalPath(string path)
        {
            var parts = path.Split(new[] {FileContainerPath.PathSeparator}, StringSplitOptions.RemoveEmptyEntries);
            var relativePathToRoot = Path.Combine(parts);
            return Path.Combine(_rootPath, relativePathToRoot);
        }

        private string TryConvertToContainerPath(string physicalPathRelativeToRoot)
        {
            var parts = physicalPathRelativeToRoot.Split(
                new[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar},
                StringSplitOptions.RemoveEmptyEntries);

            var containerPath = FileContainerPath.Combine(parts);

            return FileContainerPath.IsValid(containerPath)
                ? containerPath
                : null;
        }

        private static void TryDeleteFile(string physicalPath)
        {
            try
            {
                File.Delete(physicalPath);
            }
            catch
            {
                // ignore
            }
        }
    }
}