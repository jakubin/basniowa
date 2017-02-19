using System;

namespace Common
{
    /// <summary>
    /// Extensions for <see cref="Guid"/> type.
    /// </summary>
    public static class GuidEncoder
    {
        /// <summary>
        /// Converts <see cref="Guid"/> to a short, url-friendly string representation.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>The short, url-friendly string representation.</returns>
        public static string ToShortString(this Guid guid)
        {
            return Convert.ToBase64String(guid.ToByteArray())
                .Replace("/", "_")
                .Replace("+", "-")
                .Substring(0, 22);
        }

        /// <summary>
        /// Converts a short string representation to a <see cref="Guid"/>.
        /// </summary>
        /// <param name="encoded">The encoded short string representation of a Guid.</param>
        /// <returns>The <see cref="Guid"/>.</returns>
        public static Guid FromShortString(string encoded)
        {
            Guard.NotNull(encoded, nameof(encoded));
            if (encoded.Length != 22)
            {
                throw new ArgumentException(
                    $"Provided value \"{encoded}\" is not a valid short string representation of a Guid.",
                    nameof(encoded));
            }

            encoded = encoded.Replace("_", "/").Replace("-", "+") + "==";
            var bytes = Convert.FromBase64String(encoded);

            return new Guid(bytes);
        }
    }
}