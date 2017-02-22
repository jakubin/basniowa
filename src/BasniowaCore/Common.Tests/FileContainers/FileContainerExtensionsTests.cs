using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Common.FileContainers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Common.Tests.FileContainers
{
    public class FileContainerExtensionsTests
    {
        #region Setup

        private IFileContainer _fileContainer;

        public FileContainerExtensionsTests()
        {
            _fileContainer = Mock.Of<IFileContainer>();
        }

        #endregion

        #region AddFile (byte array) tests

        [Fact]
        public async Task AddFileByteArray_ThowsWhenFileContainerIsNull()
        {
            _fileContainer = null;
            var bytes = new byte[0];

            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _fileContainer.AddFile("file", bytes));
        }

        [Fact]
        public async Task AddFileByteArray_ThowsWhenFileBytesIsNull()
        {
            byte[] bytes = null;

            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _fileContainer.AddFile("file", bytes));
        }

        [Fact]
        public async Task AddFileByteArray_EmptyArray()
        {
            const string path = "file.txt";
            var bytes = new byte[0];

            var copiedStream = new MemoryStream();

            Mock.Get(_fileContainer)
                .Setup(x => x.AddFile(path, It.IsAny<Stream>()))
                .Callback<string,Stream>((p, s) => s.CopyTo(copiedStream))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _fileContainer.AddFile(path, bytes);

            Mock.Get(_fileContainer).Verify();
            var actualBytes = copiedStream.ToArray();

            actualBytes.Should().BeEmpty();
        }

        [Fact]
        public async Task AddFileByteArray_WithData()
        {
            const string path = "file.txt";
            var bytes = new byte[] {1, 2, 3, 4, 5, 6, 7};

            var copiedStream = new MemoryStream();

            Mock.Get(_fileContainer)
                .Setup(x => x.AddFile(path, It.IsAny<Stream>()))
                .Callback<string, Stream>((p, s) => s.CopyTo(copiedStream))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _fileContainer.AddFile(path, bytes);

            Mock.Get(_fileContainer).Verify();
            var actualBytes = copiedStream.ToArray();

            actualBytes.Should().Equal(bytes);
        }

        #endregion

        #region AddFile (text) tests

        [Fact]
        public async Task AddFileText_ThowsWhenFileContainerIsNull()
        {
            _fileContainer = null;
            var text = "content";

            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _fileContainer.AddFile("file", text));
        }

        [Fact]
        public async Task AddFileText_ThowsWhenTextIsNull()
        {
            string text = null;

            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _fileContainer.AddFile("file", text));
        }

        [Fact]
        public async Task AddFileText_Empty()
        {
            const string path = "file.txt";
            var text = "";

            var copiedStream = new MemoryStream();

            Mock.Get(_fileContainer)
                .Setup(x => x.AddFile(path, It.IsAny<Stream>()))
                .Callback<string, Stream>((p, s) => s.CopyTo(copiedStream))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _fileContainer.AddFile(path, text);

            Mock.Get(_fileContainer).Verify();
            var actualBytes = copiedStream.ToArray();
            var actualText = Encoding.UTF8.GetString(actualBytes);

            actualText.Should().Be(text);
        }

        [Fact]
        public async Task AddFileText()
        {
            const string path = "file.txt";
            var text = "some content";

            var copiedStream = new MemoryStream();

            Mock.Get(_fileContainer)
                .Setup(x => x.AddFile(path, It.IsAny<Stream>()))
                .Callback<string, Stream>((p, s) => s.CopyTo(copiedStream))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _fileContainer.AddFile(path, text);

            Mock.Get(_fileContainer).Verify();
            var actualBytes = copiedStream.ToArray();
            var actualText = Encoding.UTF8.GetString(actualBytes);

            actualText.Should().Be(text);
        }

        [Fact]
        public async Task AddFileText_CustomEncoding()
        {
            const string path = "file.txt";
            var text = "some content";

            var copiedStream = new MemoryStream();

            Mock.Get(_fileContainer)
                .Setup(x => x.AddFile(path, It.IsAny<Stream>()))
                .Callback<string, Stream>((p, s) => s.CopyTo(copiedStream))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _fileContainer.AddFile(path, text, Encoding.UTF32);

            Mock.Get(_fileContainer).Verify();
            var actualBytes = copiedStream.ToArray();
            var actualText = Encoding.UTF32.GetString(actualBytes);

            actualText.Should().Be(text);
        }

        #endregion

        #region ReadAllBytes tests

        [Fact]
        public async Task ReadAllBytes_ThowsWhenFileContainerIsNull()
        {
            _fileContainer = null;

            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _fileContainer.ReadAllBytes("file"));
        }

        [Fact]
        public async Task ReadAllBytes_Empty()
        {
            const string path = "file.txt";
            var bytes = new byte[0];

            Stream stream = new MemoryStream(bytes);

            Mock.Get(_fileContainer)
                .Setup(x => x.ReadFile(path))
                .Returns(Task.FromResult(stream))
                .Verifiable();

            var actualBytes = await _fileContainer.ReadAllBytes(path);

            Mock.Get(_fileContainer).Verify();

            actualBytes.Should().Equal(bytes);
        }

        [Fact]
        public async Task ReadAllBytes_WithData()
        {
            const string path = "file.txt";
            var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7 };

            Stream stream = new MemoryStream(bytes);

            Mock.Get(_fileContainer)
                .Setup(x => x.ReadFile(path))
                .Returns(Task.FromResult(stream))
                .Verifiable();

            var actualBytes = await _fileContainer.ReadAllBytes(path);

            Mock.Get(_fileContainer).Verify();

            actualBytes.Should().Equal(bytes);
        }

        #endregion

        #region ReadAllText tests

        [Fact]
        public async Task ReadAllText_ThowsWhenFileContainerIsNull()
        {
            _fileContainer = null;

            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _fileContainer.ReadAllText("file"));
        }

        [Fact]
        public async Task ReadAllText_Empty()
        {
            const string path = "file.txt";
            var text = "";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

            Mock.Get(_fileContainer)
                .Setup(x => x.ReadFile(path))
                .Returns(Task.FromResult(stream))
                .Verifiable();

            var actualText = await _fileContainer.ReadAllText(path);

            actualText.Should().Be(text);
        }

        [Fact]
        public async Task ReadAllText()
        {
            const string path = "file.txt";
            var text = "some content";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

            Mock.Get(_fileContainer)
                .Setup(x => x.ReadFile(path))
                .Returns(Task.FromResult(stream))
                .Verifiable();

            var actualText = await _fileContainer.ReadAllText(path);

            actualText.Should().Be(text);
        }

        [Fact]
        public async Task ReadAllText_CustomEncoding()
        {
            const string path = "file.txt";
            var text = "some content";

            Stream stream = new MemoryStream(Encoding.UTF32.GetBytes(text));

            Mock.Get(_fileContainer)
                .Setup(x => x.ReadFile(path))
                .Returns(Task.FromResult(stream))
                .Verifiable();

            var actualText = await _fileContainer.ReadAllText(path, Encoding.UTF32);

            actualText.Should().Be(text);
        }

        #endregion
    }
}