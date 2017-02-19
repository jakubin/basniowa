using Logic.Common.BusinessRules;

namespace Logic.Shows
{
    /// <summary>
    /// Business rule ensuring that provided file must be an image.
    /// </summary>
    public sealed class FileMustBeImageRule : IBusinessRule
    {
        /// <inheritdoc/>
        public string RuleId => "FileMustBeImage";

        /// <summary>
        /// Gets the file provided by the user extension.
        /// </summary>
        public string FileExtension { get; }

        /// <summary>
        /// Gets the allowed file extensions.
        /// </summary>
        public string[] AllowedFileExtensions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMustBeImageRule"/> class.
        /// </summary>
        /// <param name="fileExtension">The file extension.</param>
        /// <param name="allowedFileExtensions">The allowed file extensions.</param>
        public FileMustBeImageRule(string fileExtension, string[] allowedFileExtensions)
        {
            FileExtension = fileExtension;
            AllowedFileExtensions = allowedFileExtensions;
        }

        /// <inheritdoc/>
        public string GetUserMessage() =>
            $"File must be an image. Allowed extensions are: {string.Join(", ", AllowedFileExtensions)}.";
    }
}