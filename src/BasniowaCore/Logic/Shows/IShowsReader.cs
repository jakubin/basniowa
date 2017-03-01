using System.Collections.Generic;
using System.Threading.Tasks;
using Logic.Common;

namespace Logic.Shows
{
    /// <summary>
    /// Reads show information.
    /// </summary>
    public interface IShowsReader
    {
        /// <summary>
        /// Gets all shows.
        /// </summary>
        /// <returns>All shows.</returns>
        IList<ShowHeader> GetAllShows();

        /// <summary>
        /// Gets the show by identifier.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns>Show with specified ID.</returns>
        /// <exception cref="EntityNotFoundException{Show}">When show doesn't exist.</exception>
        ShowWithDetails GetShowById(long showId);

        /// <summary>
        /// Gets the show pictures.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns>List of show pictures</returns>
        /// <exception cref="EntityNotFoundException{Show}">When show doesn't exist.</exception>
        Task<IList<ShowPictureDetails>> GetShowPictures(long showId);
    }
}