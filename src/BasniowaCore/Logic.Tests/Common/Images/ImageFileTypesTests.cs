using System.Linq;
using System.Reflection;
using FluentAssertions;
using Logic.Common.Images;
using Xunit;

namespace Logic.Tests.Common.Images
{
    public class ImageFileTypesTests
    {
        [Fact]
        public void AllPublicFieldsShouldNotBeNullOrEmpty()
        {
            var fields = typeof(ImageFileTypes).GetTypeInfo()
                .GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.FieldType == typeof(string));

            foreach (var field in fields)
            {
                var value = (string)field.GetValue(null);
                value.Should().NotBeNullOrEmpty(
                    "field with file type extension should not contain empty or null value (field: {0})", field.Name);
            }
        }

        [Theory]
        [InlineData(".jpg")]
        [InlineData(".JPG")]
        [InlineData(".jpeg")]
        [InlineData(".png")]
        [InlineData(".gif")]
        [InlineData(".bmp")]
        [InlineData(".tiff")]
        [InlineData(".tif")]
        public void IsImageFileExtension_True(string extension)
        {
            var actual = ImageFileTypes.ImageFileExtensions.Contains(extension);
            actual.Should().BeTrue();
        }

        [Theory]
        [InlineData(".docx")]
        [InlineData(".txt")]
        public void IsImageFileExtension_False(string extension)
        {
            var actual = ImageFileTypes.ImageFileExtensions.Contains(extension);
            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData(".jpg")]
        [InlineData(".JPG")]
        [InlineData(".jpeg")]
        [InlineData(".png")]
        [InlineData(".gif")]
        public void IsCompressedImageFileExtension_True(string extension)
        {
            var actual = ImageFileTypes.CompressedImageFileExtensions.Contains(extension);
            actual.Should().BeTrue();
        }

        [Theory]
        [InlineData(".bmp")]
        [InlineData(".tiff")]
        [InlineData(".tif")]
        [InlineData(".txt")]
        public void IsCompressedImageFileExtension_False(string extension)
        {
            var actual = ImageFileTypes.CompressedImageFileExtensions.Contains(extension);
            actual.Should().BeFalse();
        }
    }
}