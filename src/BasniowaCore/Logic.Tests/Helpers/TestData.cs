using System;
using System.Collections.Generic;
using DataAccess.Shows;

namespace Logic.Tests.Helpers
{
    public static class TestData
    {
        public static Show CreateShow(long id = 1)
        {
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
                ShowProperties = new List<ShowProperty>
                {
                    new ShowProperty { Id = id + 1, Name = "Name1", Value = "Value1", IsDeleted = false },
                    new ShowProperty { Id = id + 2, Name = "Name2", Value = "Value2", IsDeleted = false },
                }
            };
        }
    }
}