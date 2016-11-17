using System;
using System.Collections.Generic;

namespace DataAccess.Shows
{
    public partial class Show
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Subtitle { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset ModifiedUtc { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<ShowProperty> ShowProperties { get; set; } = new HashSet<ShowProperty>();
    }
}
