using System.Collections.Generic;
using System.Linq;
using DataAccess.Database;
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
                    .Include(x => x.MainShowPicture)
                    .ToList()
                    .Select(x => new ShowHeader
                    {
                        ShowId = x.Id,
                        Title = x.Title,
                        Subtitle = x.Subtitle,
                        MainShowPicturePath = x.MainShowPicture?.ImagePath,
                        MainShowPictureThumbPath = x.MainShowPicture?.ThumbPath
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
                var show = db.Shows
                    .Where(x => !x.IsDeleted)
                    .FirstOrDefault(x => x.Id == showId);
                show.ThrowIfNull(showId.ToString());

                var properties = db.ShowProperties
                    .Where(x => x.ShowId == showId)
                    .Where(x => !x.IsDeleted)
                    .ToDictionary(x => x.Name, x => x.Value);

                return new ShowWithDetails
                {
                    ShowId = show.Id,
                    Title = show.Title,
                    Description = show.Description,
                    Subtitle = show.Subtitle,
                    Properties = properties
                };
            }
        }
    }
}
