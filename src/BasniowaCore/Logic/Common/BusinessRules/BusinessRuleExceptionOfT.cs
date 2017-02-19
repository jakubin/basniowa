using System;
using Common;

namespace Logic.Common.BusinessRules
{
    /// <summary>
    /// Exception thrown when business rule is violated.
    /// </summary>
    /// <typeparam name="T">Business rule type</typeparam>
    public class BusinessRuleException<T> : BusinessRuleException
        where T : IBusinessRule
    {
        /// <summary>
        /// Gets the business rule.
        /// </summary>
        public T BusinessRule { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleException{T}"/> class.
        /// </summary>
        /// <param name="businessRule">The business rule.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="businessRule"/> is <c>null</c>.</exception>
        public BusinessRuleException(T businessRule) 
            : this(businessRule, $"Business rule of type {typeof(T).FullName} was violated.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleException{T}"/> class.
        /// </summary>
        /// <param name="businessRule">The business rule.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="businessRule"/> is <c>null</c>.</exception>
        public BusinessRuleException(T businessRule, string message) 
            : base(message)
        {
            Guard.NotNull(businessRule, nameof(businessRule));
            BusinessRule = businessRule;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleException{T}"/> class.
        /// </summary>
        /// <param name="businessRule">The business rule.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="businessRule"/> is <c>null</c>.</exception>
        public BusinessRuleException(T businessRule, string message, Exception inner) 
            : base(message, inner)
        {
            Guard.NotNull(businessRule, nameof(businessRule));
            BusinessRule = businessRule;
        }

        /// <summary>
        /// Gets the business rule.
        /// </summary>
        /// <returns>Business rule.</returns>
        public override IBusinessRule GetBusinessRule()
        {
            return BusinessRule;
        }
    }
}