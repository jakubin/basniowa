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
    public class PhysicalFileContainer : IFileContainer, IPhysicalFileContainerReader
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
        /// <exception cref="ArgumentNullException">When <paramref name="rootPath"/> is <c>null</c>.</exception>
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
            catch (Exception exception)
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
            ValidateContainerPath(path, nameof(path));

            try
            {
                var physicalPath = GetPhysicalPath(path);
                return Task.FromResult<Stream>(File.OpenRead(physicalPath));
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundInContainerException(path, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new FileNotFoundInContainerException(path, ex);
            }
            catch (Exception ex)
            {
                throw new FileContainerException(
                    $"An error occured while reading file in the container under path \"{path}\". See inner exception for details.",
                    ex);
            }
        }

        /// <inheritdoc/>
        public Task<bool> Exists(string path)
        {
            Guard.NotNull(path, nameof(path));
            ValidateContainerPath(path, nameof(path));

            try
            {
                var physicalPath = GetPhysicalPath(path);
                return Task.FromResult(File.Exists(physicalPath));
            }
            catch (Exception ex)
            {
                throw new FileContainerException(
                    $"An error occured while checking for file existance in the container under path \"{path}\". See inner exception for details.",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task AddFile(string path, Stream contentStream)
        {
            Guard.NotNull(path, nameof(path));
            Guard.NotNull(contentStream, nameof(contentStream));
            ValidateContainerPath(path, nameof(path));

            try
            {
                var physicalPath = GetPhysicalPath(path);

                // ensure directory exist
                var physicalDirectoryPath = Path.GetDirectoryName(physicalPath);
                Directory.CreateDirectory(physicalDirectoryPath);

                using (FileStream fileStream = File.Open(physicalPath, FileMode.CreateNew, FileAccess.Write))
                {
                    await contentStream.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                throw new FileContainerException(
                    $"An error occured while creating file within the container under path \"{path}\". See inner exception for details.",
                    ex);
            }
        }

        /// <inheritdoc/>
        public Task RemoveFile(string path)
        {
            Guard.NotNull(path, nameof(path));
            ValidateContainerPath(path, nameof(path));

            try
            {
                var physicalPath = GetPhysicalPath(path);

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

        /// <inheritdoc/>
        public string GetFilePhysicalPath(string path)
        {
            Guard.NotNull(path, nameof(path));
            ValidateContainerPath(path, nameof(path));

            return GetPhysicalPath(path);
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
    }
}