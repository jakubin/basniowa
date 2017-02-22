﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.FileContainers;
using DataAccess.Disk;
using FluentAssertions;
using Xunit;

namespace Common.Tests.Disk
{
    public class PhysicalFileContainerIntergationTests : IDisposable
    {
        #region Setup

        private string _rootPath;

        public PhysicalFileContainerIntergationTests()
        {
            _rootPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            while (Directory.Exists(_rootPath))
            {
                _rootPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            }
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(_rootPath, true);
            }
            catch
            {
            }
        }

        private PhysicalFileContainer Create()
        {
            return new PhysicalFileContainer(_rootPath);
        }

        #endregion

        [Fact]
        public async Task EmptyContainerHasNoFiles()
        {
            var container = Create();

            var allFiles = await container.GetAllFiles();
            allFiles.Should().BeEmpty();

            await Assert.ThrowsAsync<FileNotFoundInContainerException>(
                async () => await container.ReadAllText("notfound.txt"));
        }

        [Fact]
        public async Task SingleFileScenario()
        {
            const string path = "file.txt";
            const string fileText = "some dummy text";

            var container = Create();

            await container.AddFile(path, fileText);

            var allFiles = await container.GetAllFiles();
            allFiles.Should().BeEquivalentTo(path);

            var fileContent = await container.ReadAllText(path);
            fileContent.Should().Be(fileText);

            await container.RemoveFile(path);
            allFiles = await container.GetAllFiles();
            allFiles.Should().BeEmpty();

            await Assert.ThrowsAsync<FileNotFoundInContainerException>(
                async () => await container.ReadAllText(path));
        }

        [Fact]
        public async Task SingleFileInFolderScenario()
        {
            const string path = "master/sub/file.txt";
            const string fileText = "some dummy text";

            var container = Create();

            await container.AddFile(path, fileText);

            var allFiles = await container.GetAllFiles();
            allFiles.Should().BeEquivalentTo(path);

            var fileContent = await container.ReadAllText(path);
            fileContent.Should().Be(fileText);

            await container.RemoveFile(path);
            allFiles = await container.GetAllFiles();
            allFiles.Should().BeEmpty();

            await Assert.ThrowsAsync<FileNotFoundInContainerException>(
                async () => await container.ReadAllText(path));
        }

        [Fact]
        public async Task MultipleFileScenario()
        {
            var files = new[]
            {
                new { Path = "file1.csv", Text = "File1"},
                new { Path = "dir/file2.txt", Text = "File2"},
                new { Path = "dir/file3.txt", Text = "File3"},
                new { Path = "dir2/file4", Text = "File4"},
            };

            var container = Create();

            foreach (var file in files)
            {
                await container.AddFile(file.Path, file.Text);
            }
            
            var allFiles = await container.GetAllFiles();
            allFiles.Should().BeEquivalentTo(files.Select(x => x.Path));

            foreach (var file in files)
            {
                var fileContent = await container.ReadAllText(file.Path);
                fileContent.Should().Be(file.Text);
            }

            var expectedFiles = files.Select(x => x.Path).ToList();
            foreach (var file in files)
            {
                await container.RemoveFile(file.Path);
                expectedFiles.Remove(file.Path);

                allFiles = await container.GetAllFiles();
                allFiles.Should().BeEquivalentTo(expectedFiles);
            }
        }

        [Fact]
        public async Task InvalidRoot_AddFile_ThrowsFileContainerException()
        {
            _rootPath += Path.GetInvalidFileNameChars()[0];

            var container = Create();

            await Assert.ThrowsAsync<FileContainerException>(
                async () => await container.AddFile("file.txt", "content"));
        }

        [Fact]
        public async Task InvalidRoot_ReadFile_ThrowsFileContainerException()
        {
            _rootPath += Path.GetInvalidFileNameChars()[0];

            var container = Create();

            await Assert.ThrowsAsync<FileContainerException>(
                async () => await container.ReadFile("file.txt"));
        }

        [Fact]
        public async Task InvalidRoot_RemoveFile_ThrowsFileContainerException()
        {
            _rootPath += Path.GetInvalidFileNameChars()[0];

            var container = Create();

            await Assert.ThrowsAsync<FileContainerException>(
                async () => await container.RemoveFile("file.txt"));
        }
    }
}