namespace Website.Infrastructure.ErrorHandling
{
    /// <summary>
    /// Generic model representing information about a problem.
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// The type of the problem, which occured.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The message explaining the problem.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Details about the problem (content depends on <see cref="Type"/>).
        /// </summary>
        public object Details { get; set; }
    }
}