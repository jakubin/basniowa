using System.Threading.Tasks;

namespace Common.UniqueId
{
    /// <summary>
    /// Provides unique identifiers from data source.
    /// </summary>
    public interface IUniqueIdProvider
    {
        /// <summary>
        /// Gets the next unique IDs.
        /// </summary>
        /// <param name="count">The count of IDs to generate.</param>
        /// <returns>Task with the first generated ID. Next IDs are sequential.</returns>
        Task<long> GetNextIds(long count);
    }
}
