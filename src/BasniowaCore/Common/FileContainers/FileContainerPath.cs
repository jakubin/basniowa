using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.FileContainers
{
    /// <summary>
    /// Helper methods for managing paths in implementations of <see cref="IFileContainer"/>.
    /// </summary>
    public static class FileContainerPath
    {
        private const string PartExpr = @"[A-Za-z0-9\-_.]*[A-Za-z0-9\-_]";

        private static readonly Regex ContainerNameRegex =
            new Regex($"^{PartExpr}(/{PartExpr})*$", RegexOptions.Compiled);

        private static readonly Regex NormalizeRegex =
            new Regex(@"[^A-Za-z0-9\-_./]", RegexOptions.Compiled);

        /// <summary>
        /// The path separator for file containers.
        /// </summary>
        public const string PathSeparator = "/";

        /// <summary>
        /// Returns true if specified <paramref name="path"/> is a well formed container path.
        /// Actual existance of the path is not checked.
        /// </summary>
        /// <param name="path">The path to validate.</param>
        /// <returns>
        ///   <c>true</c> if the specified path is valid; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">When <paramref name="path"/> is <c>null</c>.</exception>
        public static bool IsValid(string path)
        {
            Guard.NotNull(path, nameof(path));
            return ContainerNameRegex.IsMatch(path);
        }

        /// <summary>
        /// Combines the specified parts of the path.
        /// Parts validity is not checked.
        /// </summary>
        /// <param name="parts">The parts.</param>
        /// <returns>Parts of the path combined with the <see cref="PathSeparator"/>.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="parts"/> is <c>null</c>.</exception>
        public static string Combine(params string[] parts)
        {
            Guard.NotNull(parts, nameof(parts));
            return string.Join(PathSeparator, parts);
        }

        /// <summary>
        /// Normalizes the specified part of the path, so it is valid.
        /// </summary>
        /// <remarks>
        /// Normalization consists of 3 steps:
        /// 1. Replacement of diacritics (aka accents) with corresponding latin letters (<see cref="NormalizationForm.FormD"/>).
        /// 2. Replacement of all invalid characters with underscore (_).
        /// 3. Removal of trailing dot (.) characters.
        /// If the resulting name is an empty string, underscore (_) is returned.
        /// </remarks>
        /// <param name="part">The part of the path to normalize.</param>
        /// <returns>Normalized part of path.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="part"/> is <c>null</c>.</exception>
        public static string Normalize(string part)
        {
            Guard.NotNull(part, nameof(part));

            part = new string(
                part.Normalize(NormalizationForm.FormD)
                    .ToCharArray()
                    .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    .ToArray());
            part = NormalizeRegex.Replace(part, "_");
            part = part.TrimEnd('.');
            if (part.Length == 0)
            {
                part = "_";
            }

            return part;
        }
    }
}