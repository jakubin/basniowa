using System;

namespace Logic.Common
{
    /// <summary>
    /// Exception thrown when entity of given key has not been found.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class EntityNotFoundException : Exception
    {
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        public Type EntityType { get; private set; }

        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        public string EntityKey { get; set; }

        private static string FormatMessage(Type entityType, string entityKey)
        {
            return $"Entity {entityType.FullName} with key {entityKey} was not found.";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="entityKey">The entity key.</param>
        public EntityNotFoundException(Type entityType, string entityKey) :
            base(FormatMessage(entityType, entityKey))
        {
            EntityType = entityType;
            EntityKey = entityKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="entityKey">The entity key.</param>
        /// <param name="innerException">The inner exception.</param>
        public EntityNotFoundException(Type entityType, string entityKey, Exception innerException)
            : base(FormatMessage(entityType, entityKey), innerException)
        {
            EntityType = entityType;
            EntityKey = entityKey;
        }
    }
}
