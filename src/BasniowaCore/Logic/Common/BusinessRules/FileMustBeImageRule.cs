namespace Logic.Common.BusinessRules
{
    /// <summary>
    /// Business rule ensuring that file is an image.
    /// </summary>
    public sealed class FileMustBeImageRule : IBusinessRule
    {
        /// <inheritdoc/>
        public string RuleId => "Common.FileMustBeImage";

        /// <inheritdoc/>
        public string GetUserMessage() => "File is not a valid image.";
    }
}