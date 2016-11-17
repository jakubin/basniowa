using System;
using System.Threading.Tasks;

namespace Logic.Services
{
    /// <summary>
    /// Provides unique IDs.
    /// </summary>
    public interface IUniqueIdService : IDisposable
    {
        /// <summary>
        /// Generates a single ID.
        /// </summary>
        /// <returns>Task with generated ID.</returns>
        Task<long> GenerateId();

        /// <summary>
        /// Generates multiple IDs.
        /// </summary>
        /// <param name="count">The number of IDs to generate.</param>
        /// <returns>Task with array of generated IDs.</returns>
        Task<long[]> GenerateIds(int count);
    }
}
