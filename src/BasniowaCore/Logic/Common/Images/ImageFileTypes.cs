using System;
using System.Collections.Immutable;

namespace Logic.Common.Images
{
    /// <summary>
    /// Well-know image file extensions.
    /// </summary>
    public static class ImageFileTypes
    {
        /// <summary>
        /// "jpeg" file extension.
        /// </summary>
        public static readonly string Jpeg = "jpeg";

        /// <summary>
        /// "jpg" file extension.
        /// </summary>
        public static readonly string Jpg = "jpg";

        /// <summary>
        /// "png" file extension.
        /// </summary>
        public static readonly string Png = "png";

        /// <summary>
        /// "gif" file extension.
        /// </summary>
        public static readonly string Gif = "gif";

        /// <summary>
        /// "bmp" file extension.
        /// </summary>
        public static readonly string Bmp = "bmp";

        /// <summary>
        /// "tiff" file extension.
        /// </summary>
        public static readonly string Tiff = "tiff";

        /// <summary>
        /// Initializes the <see cref="ImageFileTypes"/> class.
        /// </summary>
        static ImageFileTypes()
        {
            ImageFileExtensions = new[] {Jpeg, Jpg, Png, Gif, Bmp, Tiff}
                .ToImmutableSortedSet(StringComparer.OrdinalIgnoreCase);
            CompressedImageFileExtensions = new[] {Jpeg, Jpg, Png, Gif}
                .ToImmutableSortedSet(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the file extensions representing an image type.
        /// </summary>
        public static ImmutableSortedSet<string> ImageFileExtensions { get; }

        /// <summary>
        /// Gets the file extensions representing a compressed image type.
        /// </summary>
        public static ImmutableSortedSet<string> CompressedImageFileExtensions { get; }
    }
}