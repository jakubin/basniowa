using System;

namespace Logic.Common.BusinessRules
{
    /// <summary>
    /// Exception thrown when business rule is violated.
    /// </summary>
    public abstract class BusinessRuleException : Exception
    {
        /// <summary>
        /// Gets the business rule.
        /// </summary>
        public abstract IBusinessRule GetBusinessRule();

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        protected BusinessRuleException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        protected BusinessRuleException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}