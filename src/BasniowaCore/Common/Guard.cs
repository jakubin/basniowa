using System;

namespace Common
{
    /// <summary>
    /// Helpers for validating method arguments.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Ensures that specified <paramref name="obj"/> is not <c>null</c>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentNullException">When <paramref name="obj"/> is <c>null</c>.</exception>
        public static void NotNull(object obj, string parameterName)
        {
            if (ReferenceEquals(obj, null))
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Ensures that specified <paramref name="value"/> is greater of equal to <paramref name="minValue"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">When check condition is not met.</exception>
        public static void GreaterOrEqual(int value, int minValue, string parameterName)
        {
            if (value < minValue)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    $"Parameter {parameterName} must be greater or equal to {minValue}.");
            }
        }
    }
}
