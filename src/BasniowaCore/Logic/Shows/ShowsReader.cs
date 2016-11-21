using System.Collections.Generic;
using System.Linq;
using DataAccess;
using Logic.Common;
using Microsoft.EntityFrameworkCore;

namespace Logic.Shows
{
    /// <summary>
    /// Implementation of <see cref="IShowsReader"/>.
    /// </summary>
    public class ShowsReader : IShowsReader
    {
        private TheaterDb _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowsReader"/> class.
        /// </summary>
        /// <param name="db">The database.</param>
        public ShowsReader(TheaterDb db)
        {
            _db = db;
        }

        /// <inheritdoc/>
        public IList<ShowHeader> GetAllShows()
        {
            var shows = _db.Shows.Select(x => new ShowHeader
                {
                    Id = x.Id,
                    Title = x.Title,
                    Subtitle = x.Subtitle
                })
                .ToList();

            return shows;
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
