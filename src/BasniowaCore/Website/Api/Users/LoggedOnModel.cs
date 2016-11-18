namespace Website.Api.Users
{
    /// <summary>
    /// Represents result of successful authentication.
    /// </summary>
    public class LoggedOnModel
    {
        /// <summary>
        /// The name of the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The authentication token.
        /// </summary>
        public string Token { get; set; }
    }
}
