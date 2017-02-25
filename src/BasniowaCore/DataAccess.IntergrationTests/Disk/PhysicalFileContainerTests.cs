using System;
using System.IO;
using System.Threading.Tasks;
using DataAccess.Disk;
using FluentAssertions;
using Xunit;

namespace Common.Tests.Disk
{
    public class PhysicalFileContainerTests
    {
        #region Setup

        private string _rootPath;

        public PhysicalFileContainerTests()
        {
            _rootPath = Path.GetRandomFileName();
        }

        private PhysicalFileContainer Create()
        {
            return new PhysicalFileContainer(_rootPath);
        }

        #endregion

        #region Ctor tests

        [Fact]
        public void Ctor_ThrowsOnNullRootPath()
        {
            Assert.Throws<ArgumentNullException>(() => new PhysicalFileContainer(null));
        }

        #endregion

        #region ReadFile tests

        [Fact]
        public async Task ReadFile_ThrowsOnNullPath()
        {
            var container = Create();
            
            await Assert.ThrowsAsync<ArgumentNullException>(() => container.ReadFile(null));
        }

        [Fact]
        public async Task ReadFile_ThrowsOnInvalidPath()
        {
            var container = Create();

            await Assert.ThrowsAsync<ArgumentException>(() => container.ReadFile("%:*"));
        }

        #endregion

        #region GetFilePhysicalPath tests

        [Fact]
        public void GetFilePhysicalPath_ThrowsOnNullPath()
        {
            var container = Create();

            Assert.Throws<ArgumentNullException>(() => container.GetFilePhysicalPath(null));
        }

        [Fact]
        public void GetFilePhysicalPath_ThrowsOnInvalidPath()
        {
            var container = Create();

            Assert.Throws<ArgumentException>(() => container.GetFilePhysicalPath("%:*"));
        }

        [Fact]
        public void GetFilePhysicalPath_MapsCorrectly()
        {
            var container = Create();
            var expected = Path.Combine(_rootPath, "path", "to", "resource.txt");

            var actualPath = container.GetFilePhysicalPath("path/to/resource.txt");

            actualPath.Should().Be(expected);
        }

        #endregion

        #region Exists test

        [Fact]
        public async Task Exists_ThrowsOnNullPath()
        {
            var container = Create();

            await Assert.ThrowsAsync<ArgumentNullException>(() => container.Exists(null));
        }

        [Fact]
        public async Task Exists_ThrowsOnInvalidPath()
        {
            var container = Create();

            await Assert.ThrowsAsync<ArgumentException>(() => container.Exists("%:*"));
        }

        #endregion

        #region AddFile tests

        [Fact]
        public async Task AddFile_ThrowsOnNullPath()
        {
            var container = Create();
            var stream = new MemoryStream();
            
            await Assert.ThrowsAsync<ArgumentNullException>(() => container.AddFile(null, stream));
        }

        [Fact]
        public async Task AddFile_ThrowsOnNullStream()
        {
            var container = Create();

            await Assert.ThrowsAsync<ArgumentNullException>(() => container.AddFile("file", null));
        }

        [Fact]
        public async Task AddFile_ThrowsOnInvalidPath()
        {
            var container = Create();
            var stream = new MemoryStream();

            await Assert.ThrowsAsync<ArgumentException>(() => container.AddFile("%:*", stream));
        }

        #endregion

        #region RemoveFile tests

        [Fact]
        public async Task RemoveFile_ThrowsOnNullPath()
        {
            var container = Create();

            await Assert.ThrowsAsync<ArgumentNullException>(() => container.RemoveFile(null));
        }

        [Fact]
        public async Task RemoveFile_ThrowsOnInvalidPath()
        {
            var container = Create();

            await Assert.ThrowsAsync<ArgumentException>(() => container.RemoveFile("%:*"));
        }

        #endregion
    }
}