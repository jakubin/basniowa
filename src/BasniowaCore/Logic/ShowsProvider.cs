using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Logic.Common;
using DataAccess.Shows;

namespace Logic
{
    public class ShowsProvider: IShowsProvider
    {
        private TheaterDb _db;

        public ShowsProvider(TheaterDb db)
        {
            _db = db;
        }

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

        public long AddShow(ShowWithDetails showWithDetails)
        {
            var show = new Show()
            {
                Title = showWithDetails.Title,
                Subtitle = showWithDetails.Subtitle,
                Description = showWithDetails.Description
            };

            var properties = showWithDetails.Properties
                .Select(x => new ShowProperty { Name = x.Key, Value = x.Value, Show = show })
                .ToList();

            _db.Add(show);
            _db.AddRange(properties);

            _db.SaveChanges();

            return show.Id;
        }

        public ShowWithDetails GetShowById(long showId)
        {
            var show = _db.Shows.Include(x=>x.ShowProperties)
                .FirstOrDefault(x => x.Id == showId);
            if (show == null)
                throw new EntityNotFoundException<ShowWithDetails>($"Id={showId}");

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
