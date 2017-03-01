using System;
using System.Linq;
using DataAccess.Database.Shows;

namespace Logic.Tests.Helpers
{
    public static class ShowsDataHelper
    {
        public static Show CreateShow(long id, long[] propertyIds = null, Action<Show> customAction = null)
        {
            propertyIds = propertyIds ?? new long[0];
            var show = new Show
            {
                Id = id,
                Title = "Title " + id,
                Subtitle = "Subtitle " + id,
                Description = "Description " + id,
                CreatedBy = "init",
                CreatedUtc = new DateTimeOffset(2016, 1, 1, 15, 23, 23, TimeSpan.Zero),
                ModifiedBy = "init",
                ModifiedUtc = new DateTimeOffset(2016, 1, 1, 15, 23, 23, TimeSpan.Zero),
                IsDeleted = false,
                ShowProperties = propertyIds
                    .Select(x => new ShowProperty { Id = x, Name = $"Name{x}", Value = $"Value{x}", IsDeleted = false })
                    .ToList()
            };
            customAction?.Invoke(show);
            return show;
        }

        public static void AddShow(this IDbTest context, Show show)
        {
            using (var db = context.InMemoryDb.Create())
            {
                db.Add(show);
                db.SaveChanges();
            }
        }

        public static Show AddShow(this IDbTest context, long id, long[] propertyIds = null, Action<Show> customAction = null)
        {
            using (var db = context.InMemoryDb.Create())
            {
                var show = CreateShow(id, propertyIds, customAction);
                db.Add(show);
                db.SaveChanges();

                return show;
            }
        }

        public static void UpdateShow(this IDbTest context, Show show)
        {
            using (var db = context.InMemoryDb.Create())
            {
                db.Update(show);
                db.SaveChanges();
            }
        }

        public static ShowPicture CreateShowPicture(long id, long showId, Action<ShowPicture> customAction = null)
        {
            var showPicture = new ShowPicture
            {
                Id = id,
                ShowId = showId,
                Title = $"picture{id}",
                ImagePath = $"{showId}/{id}/full/picture.jpg",
                ThumbPath = $"{showId}/{id}/thumb-200/picture.jpg",
                CreatedBy = "test",
                CreatedUtc = new DateTimeOffset(2016, 1, 1, 15, 23, 23, TimeSpan.Zero),
                ModifiedBy = "test",
                ModifiedUtc = new DateTimeOffset(2016, 1, 1, 15, 23, 23, TimeSpan.Zero),
                IsDeleted = false,
            };

            customAction?.Invoke(showPicture);

            return showPicture;
        }

        public static ShowPicture AddShowPicture(this IDbTest context, long id, long showId, Action<ShowPicture> customAction = null)
        {
            using (var db = context.InMemoryDb.Create())
            {
                var showPicture = CreateShowPicture(id, showId, customAction);
                db.Add(showPicture);
                db.SaveChanges();

                return showPicture;
            }
        }
    }
}