using System;

namespace Tests.Helpers
{
    /// <summary>
    /// Attribute allowing to order steps within test scenario.
    /// Steps without the attribute are always executed last.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Method)]
    public class StepOrderAttribute : Attribute
    {
        /// <summary>
        /// Gets the order number to sort steps to execute.
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StepOrderAttribute"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        public StepOrderAttribute(int order)
        {
            Order = order;
        }
    }
}
