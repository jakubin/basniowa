namespace DataAccess.Shows
{
    public partial class ShowProperty
    {
        public long Id { get; set; }
        public long ShowId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Show Show { get; set; }
    }
}
