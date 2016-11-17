namespace DataAccess
{
    /// <summary>
    /// Output parameters from dbo.GetNextIds stored procedure.
    /// </summary>
    public class GetNextIdsOutput
    {
        /// <summary>
        /// Gets the starting ID of generated range.
        /// </summary>
        public long RangeFrom { get; }

        /// <summary>
        /// Gets the ending ID of generated range.
        /// </summary>
        public long RangeTo { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetNextIdsOutput"/> class.
        /// </summary>
        /// <param name="rangeFrom">The starting ID of generated range.</param>
        /// <param name="rangeTo">The ending ID of generated range.</param>
        public GetNextIdsOutput(long rangeFrom, long rangeTo)
        {
            RangeFrom = rangeFrom;
            RangeTo = rangeTo;
        }
    }
}
