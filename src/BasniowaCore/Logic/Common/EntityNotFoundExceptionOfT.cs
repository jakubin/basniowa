using System;

namespace Logic.Common
{
    /// <summary>
    /// Exception thrown when entity of type <typeparamref name="T"/> of given key has not been found.
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    /// <seealso cref="System.Exception" />
    public class EntityNotFoundException<T> : EntityNotFoundException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException{T}"/> class.
        /// </summary>
        /// <param name="entityKey">The entity key.</param>
        public EntityNotFoundException(string entityKey) :
            base(typeof(T), entityKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException{T}"/> class.
        /// </summary>
        /// <param name="entityKey">The entity key.</param>
        /// <param name="innerException">The inner exception.</param>
        public EntityNotFoundException(string entityKey, Exception innerException)
            : base(typeof(T), entityKey, innerException)
        {
        }
    }
}
