using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DataAccess.Database.Shows;

namespace Logic.Tests.Helpers
{
    public static class TestData
    {
        public static Show CreateShow(long id = 1, long[] propertyIds = null)
        {
            propertyIds = propertyIds ?? new[] {id + 1, id + 2};
            return new Show
            {
                Id = id,
                Title = "Title " + id,
                Subtitle = "Subtitle " + id,
                Description = "Description " + id,
                CreatedBy = "test",
                CreatedUtc = new DateTimeOffset(2016, 1, 1, 15, 23, 23, TimeSpan.Zero),
                ModifiedBy = "test",
                ModifiedUtc = new DateTimeOffset(2016, 1, 1, 15, 23, 23, TimeSpan.Zero),
                IsDeleted = false,
                ShowProperties = propertyIds
                    .Select(x => new ShowProperty {Id = x, Name = $"Name{x}", Value = $"Value{x}", IsDeleted = false})
                    .ToList()
            };
        }

        internal static byte[] GetResourceFileBytes(string relativeName)
        {
            var assembly = typeof(TestData).GetTypeInfo().Assembly;
            using (var memoryStream = new MemoryStream())
            using (var resourceStream = assembly.GetManifestResourceStream($"Logic.Tests.Resources.{relativeName}"))
            {
                resourceStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}