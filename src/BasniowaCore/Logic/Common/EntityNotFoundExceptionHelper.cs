namespace Logic.Common
{
    /// <summary>
    /// Helper methods for <see cref="EntityNotFoundException{T}"/>.
    /// </summary>
    public static class EntityNotFoundExceptionHelper
    {
        /// <summary>
        /// Throws <see cref="EntityNotFoundException{T}"/> if <paramref name="item"/> is <c>null</c>.
        /// </summary>
        /// <typeparam name="T">Type of entity.</typeparam>
        /// <param name="item">The item to check for <c>null</c>.</param>
        /// <param name="key">The key of entity.</param>
        /// <exception cref="EntityNotFoundException{T}">When <paramref name="item"/> is <c>null</c>.</exception>
        public static void ThrowIfNull<T>(this T item, string key)
            where T : class
        {
            if (item == null)
            {
                throw new EntityNotFoundException<T>(key);
            }
        }
    }
}