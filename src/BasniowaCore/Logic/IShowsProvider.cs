using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// Reads show information.
    /// </summary>
    public interface IShowsProvider
    {
        /// <summary>
        /// Gets all shows.
        /// </summary>
        /// <returns>All shows.</returns>
        IList<ShowWithDetails> GetAllShows();

        /// <summary>
        /// Gets the show by identifier.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns>Show with specified ID.</returns>
        /// <exception cref="Logic.Common.EntityNotFoundException{ShowWithDetails}">When show doesn't exist.</exception>
        ShowWithDetails GetShowById(long showId);
    }
}