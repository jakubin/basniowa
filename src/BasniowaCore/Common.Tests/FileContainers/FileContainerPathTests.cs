using System;
using Common.FileContainers;
using FluentAssertions;
using Xunit;

namespace Common.Tests.FileContainers
{
    public class FileContainerPathTests
    {
        #region IsValid

        [Fact]
        public void IsValid_ThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => FileContainerPath.IsValid(null));
        }

        [Theory]
        [InlineData("a")]
        [InlineData(".git")]
        [InlineData("folder/file")]
        [InlineData("folder/file.txt")]
        [InlineData("0123456789_-.QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm")]
        public void IsValid_ValidPaths(string path)
        {
            var actual = FileContainerPath.IsValid(path);
            actual.Should().Be(true);
        }

        [Theory]
        [InlineData("")]
        [InlineData("invąlid-charactęrs")]
        [InlineData("/cannot-start-with-separator")]
        [InlineData("cannot-end-with-dot.")]
        [InlineData("any-part-cannot-end./with-dot")]
        [InlineData("plus+forbidden")]
        [InlineData("space forbidden")]
        [InlineData("double//separator")]
        [InlineData("separator-at-the-end/")]
        public void IsValid_InvalidPaths(string path)
        {
            var actual = FileContainerPath.IsValid(path);
            actual.Should().Be(false);
        }

        #endregion

        #region Combine

        [Fact]
        public void Combine_ThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => FileContainerPath.Combine(null));
        }

        [Fact]
        public void Combine_EmptyList()
        {
            var actual = FileContainerPath.Combine();
            actual.Should().BeEmpty();
        }

        [Fact]
        public void Combine_SingleItem()
        {
            var actual = FileContainerPath.Combine("part");
            actual.Should().Be("part");
        }

        [Fact]
        public void Combine_MultipleItems()
        {
            var actual = FileContainerPath.Combine("part1", "part2", "part3");
            actual.Should().Be("part1/part2/part3");
        }

        [Fact]
        public void Combine_NullsAreTreatedLikeEmptyStrings()
        {
            var actual = FileContainerPath.Combine("part1", null, "part3");
            actual.Should().Be("part1//part3");
        }

        #endregion

        #region Normalize

        [Fact]
        public void Normalize_ThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => FileContainerPath.Normalize(null));
        }

        [Theory]
        [InlineData("file.txt")]
        [InlineData("other_file-type1234")]
        [InlineData(".git")]
        public void Normalize_ValidPart(string part)
        {
            var actual = FileContainerPath.Normalize(part);
            actual.Should().Be(part);
        }

        [Theory]
        [InlineData("", "_")]
        [InlineData("file+type", "file_type")]
        [InlineData("ąĘĆżź", "aECzz")]
        [InlineData("ÇÜĥŢ", "CUhT")]
        [InlineData("file..", "file")]
        [InlineData("..", "_")]
        public void Normalize_RequiredNormalization(string part, string expected)
        {
            var actual = FileContainerPath.Normalize(part);
            actual.Should().Be(expected);
        }

        #endregion

        #region GetExtension

        [Theory]
        [InlineData("file.jpg", ".jpg")]
        [InlineData("file.new.jpg", ".jpg")]
        [InlineData("parent/sub/file.jpg", ".jpg")]
        [InlineData("file", "")]
        [InlineData(".jpg", ".jpg")]
        [InlineData("parent.sth/file.new.jpg", ".jpg")]
        [InlineData("parent.sth/file", "")]
        [InlineData("parent.sth/.jpg", ".jpg")]
        public void GetExtensionTests(string input, string expected)
        {
            var actual = FileContainerPath.GetExtension(input);
            actual.Should().Be(expected);
        }

        [Fact]
        public void GetExtension_ShouldThrowOnNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => FileContainerPath.GetExtension(null));
        }

        #endregion
    }
}