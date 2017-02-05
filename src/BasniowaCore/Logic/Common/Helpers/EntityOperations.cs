using System.Collections.Generic;

namespace Logic.Common.Helpers
{
    /// <summary>
    /// Set of operations on entity set.
    /// </summary>
    /// <typeparam name="T">Type of entity.</typeparam>
    public class EntityOperations<T>
    {
        /// <summary>
        /// Gets or sets the list of entities that should be updated.
        /// </summary>
        public List<UpdatedEntities> ToUpdate { get; set; }

        /// <summary>
        /// Gets or sets the entities missing in source collection that must be added.
        /// </summary>
        public List<T> ToAdd { get; set; }

        /// <summary>
        /// Gets or sets the entities missing in target collection that must be removed.
        /// </summary>
        public List<T> ToRemove { get; set; }

        /// <summary>
        /// Pair of entities for update operation.
        /// </summary>
        public class UpdatedEntities
        {
            /// <summary>
            /// Gets or sets the source entity that will be updated.
            /// </summary>
            public T Source { get; }

            /// <summary>
            /// Gets or sets the target entity that contains updates to apply.
            /// </summary>
            public T Target { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="UpdatedEntities"/> class.
            /// </summary>
            /// <param name="source">The source entity.</param>
            /// <param name="target">The target entity.</param>
            public UpdatedEntities(T source, T target)
            {
                Source = source;
                Target = target;
            }
        }
    }
}