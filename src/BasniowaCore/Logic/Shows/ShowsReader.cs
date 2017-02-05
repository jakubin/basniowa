using System.Collections.Generic;
using System.Linq;
using DataAccess;
using Logic.Common;
using Logic.Services;
using Microsoft.EntityFrameworkCore;

namespace Logic.Shows
{
    /// <summary>
    /// Implementation of <see cref="IShowsReader"/>.
    /// </summary>
    public class ShowsReader : IShowsReader
    {
        /// <summary>
        /// Gets or sets the database context factory.
        /// </summary>
        public IDbContextFactory DbFactory { get; set; }

        private TheaterDb CreateContext()
        {
            return DbFactory.Create(trackEntities: false);
        }

        /// <inheritdoc/>
        public IList<ShowHeader> GetAllShows()
        {
            using (var db = CreateContext())
            {
                var shows = db.Shows
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .Select(x => new ShowHeader
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Subtitle = x.Subtitle
                    })
                    .ToList();

                return shows;
            }
        }

        /// <inheritdoc/>
        public ShowWithDetails GetShowById(long showId)
        {
            using (var db = CreateContext())
            {
                var show = db.Shows.Include(x => x.ShowProperties)
                .Where(x => !x.IsDeleted)
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
}
