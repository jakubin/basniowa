using System.ComponentModel.DataAnnotations;

namespace Website.Api.Users
{
    /// <summary>
    /// User logon data.
    /// </summary>
    public class UserLogonModel
    {
        /// <summary>
        /// The name of the user.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// The password.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
