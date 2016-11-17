using System;

namespace Logic.Common
{
    public class EntityNotFoundException : Exception
    {
        public Type EntityType { get; private set; }

        public string EntityKey { get; set; }

        private static string FormatMessage(Type entityType, string entityKey)
        {
            return $"Entity {entityType.FullName} with key {entityKey} was not found.";
        }

        public EntityNotFoundException(Type entityType, string entityKey) :
            base(FormatMessage(entityType, entityKey))
        {
            EntityType = entityType;
            EntityKey = entityKey;
        }

        public EntityNotFoundException(Type entityType, string entityKey, Exception innerException)
            : base(FormatMessage(entityType, entityKey), innerException)
        {
            EntityType = entityType;
            EntityKey = entityKey;
        }
    }

    public class EntityNotFoundException<T> : EntityNotFoundException
    {
        public EntityNotFoundException(string entityKey) :
            base(typeof(T), entityKey)
        {
        }

        public EntityNotFoundException(string entityKey, Exception innerException)
            : base(typeof(T), entityKey, innerException)
        {
        }
    }
}
