using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Common;

namespace Logic.Common.Helpers
{
    /// <summary>
    /// Operations on set of entities.
    /// </summary>
    public static class EntitySetOperations
    {
        /// <summary>
        /// Gets the required operations to transform one set of entities into another.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source set of entities.</param>
        /// <param name="target">The target set of entities..</param>
        /// <param name="keyResolver">The key resolver.</param>
        /// <returns>Operations to be executed.</returns>
        public static EntityOperations<T> GetRequiredOperations<T, TKey>(
                    this IEnumerable<T> source,
                    IEnumerable<T> target,
                    Func<T, TKey> keyResolver)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(target, nameof(target));

            var sourceByKey = source.ToImmutableSortedDictionary(keyResolver, x => x);
            var targetByKey = target.ToImmutableSortedDictionary(keyResolver, x => x);

            var operations = new EntityOperations<T>
            {
                ToUpdate = sourceByKey.Keys.Intersect(targetByKey.Keys)
                    .Select(x => new EntityOperations<T>.UpdatedEntities(sourceByKey[x], targetByKey[x]))
                    .ToList(),
                ToAdd = targetByKey.Keys.Except(sourceByKey.Keys)
                    .Select(x => targetByKey[x])
                    .ToList(),
                ToRemove = sourceByKey.Keys.Except(targetByKey.Keys)
                    .Select(x => sourceByKey[x])
                    .ToList()
            };

            return operations;
        }
    }
}