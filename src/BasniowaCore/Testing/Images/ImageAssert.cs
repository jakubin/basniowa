using FluentAssertions;
using ImageMagick;

namespace Testing.Images
{
    /// <summary>
    /// Assertions for comparing images
    /// </summary>
    public static class ImageAssert
    {
        /// <summary>
        /// Asserts that an image (represented by byte array) has particular size.
        /// </summary>
        /// <param name="imageBytes">The image bytes.</param>
        /// <param name="expectedWidth">The expected width.</param>
        /// <param name="expectedHeight">The expected height.</param>
        public static void HasSize(byte[] imageBytes, int expectedWidth, int expectedHeight)
        {
            var image = new MagickImageInfo(imageBytes);
            image.Width.Should().Be(expectedWidth);
            image.Height.Should().Be(expectedHeight);
        }

        /// <summary>
        /// Asserts that two images are practically identical.
        /// </summary>
        /// <param name="firstBytes">The first image in binary format.</param>
        /// <param name="secondBytes">The second image in binary format.</param>
        /// <param name="acceptedDifference">
        /// The accepted difference.
        /// Value between 0.0 (identical) and 1.0 (completely different).
        /// </param>
        public static void AreAlmostIdentical(byte[] firstBytes, byte[] secondBytes, double acceptedDifference = 0.01)
        {
            using (var first = new MagickImage(firstBytes))
            using (var second = new MagickImage(secondBytes))
            {
                second.Width.Should().Be(first.Width, "width of both images should be the equal");
                second.Height.Should().Be(first.Height, "height of both images should be the equal");

                var actualDiff = first.Compare(second, ErrorMetric.MeanAbsolute);
                actualDiff.Should().BeLessOrEqualTo(acceptedDifference);
            }
        }

        /// <summary>
        /// Asserts that provided image (in binary format) is JPEG.
        /// </summary>
        /// <param name="imageBytes">The image bytes.</param>
        public static void IsJpeg(byte[] imageBytes)
        {
            var allowedFormats = new[] {MagickFormat.Jpg, MagickFormat.Jpe, MagickFormat.Jpeg};
            var image = new MagickImageInfo(imageBytes);
            allowedFormats.Should().Contain(image.Format);
        }
    }
}