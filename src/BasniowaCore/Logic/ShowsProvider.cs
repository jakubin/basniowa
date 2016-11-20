using System.Collections.Generic;
using System.Linq;
using DataAccess;
using DataAccess.Shows;
using Logic.Common;
using Microsoft.EntityFrameworkCore;

namespace Logic
{
    /// <summary>
    /// Implementation of <see cref="IShowsProvider"/>.
    /// </summary>
    /// <seealso cref="Logic.IShowsProvider" />
    public class ShowsProvider : IShowsProvider
    {
        private TheaterDb _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowsProvider"/> class.
        /// </summary>
        /// <param name="db">The database.</param>
        public ShowsProvider(TheaterDb db)
        {
            _db = db;
        }

        /// <inheritdoc/>
        public IList<ShowWithDetails> GetAllShows()
        {
            var shows = _db.Shows.Include(x => x.ShowProperties)
                .ToList();

            return shows.Select(x => new ShowWithDetails
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Subtitle = x.Subtitle,
                Properties = x.ShowProperties.ToDictionary(p => p.Name, p => p.Value)
            })
            .ToList();
        }

        /// <inheritdoc/>
        public ShowWithDetails GetShowById(long showId)
        {
            var show = _db.Shows.Include(x => x.ShowProperties)
                .FirstOrDefault(x => x.Id == showId);

            if (show == null)
            {
                throw new EntityNotFoundException<ShowWithDetails>($"Id={showId}");
            }

            return new ShowWithDetails
            {
                Id = show.Id,
                Title = show.Title,
                Description = show.Description,
                Subtitle = show.Subtitle,
                Properties = show.ShowProperties.ToDictionary(p => p.Name, p => p.Value)
            };
        }
    }
}
