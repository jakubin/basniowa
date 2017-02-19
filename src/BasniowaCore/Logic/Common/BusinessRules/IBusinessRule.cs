namespace Logic.Common.BusinessRules
{
    /// <summary>
    /// Represents a business rule.
    /// </summary>
    public interface IBusinessRule
    {
        /// <summary>
        /// Gets the unique identifier of the business rule identifier.
        /// </summary>
        string RuleId { get; }

        /// <summary>
        /// Gets the message that can be presented to the user.
        /// </summary>
        /// <returns>User message</returns>
        string GetUserMessage();
    }
}