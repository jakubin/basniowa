namespace Logic.Common.BusinessRules
{
    /// <summary>
    /// Business rule ensuring that provided file must have a file extension image.
    /// </summary>
    public sealed class FileMustHaveImageExtensionRule : IBusinessRule
    {
        /// <inheritdoc/>
        public string RuleId => "Common.FileMustHaveImageExtension";

        /// <summary>
        /// Gets the file provided by the user extension.
        /// </summary>
        public string FileExtension { get; }

        /// <summary>
        /// Gets the allowed file extensions.
        /// </summary>
        public string[] AllowedFileExtensions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMustHaveImageExtensionRule"/> class.
        /// </summary>
        /// <param name="fileExtension">The file extension.</param>
        /// <param name="allowedFileExtensions">The allowed file extensions.</param>
        public FileMustHaveImageExtensionRule(string fileExtension, string[] allowedFileExtensions)
        {
            FileExtension = fileExtension;
            AllowedFileExtensions = allowedFileExtensions;
        }

        /// <inheritdoc/>
        public string GetUserMessage() =>
            $"File extension \"{FileExtension}\" is not an image. Allowed extensions are: {string.Join(", ", AllowedFileExtensions)}.";
    }
}