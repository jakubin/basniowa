using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Tests.HttpAssert;

namespace Tests.Common
{
    /// <summary>
    /// Helper class for app authentication.
    /// </summary>
    public static class AuthenticateExtensions
    {
        /// <summary>
        /// Authenticates as default admin user against the application.
        /// Sets the bearer token as default request token and returns it.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns>Bearer token returned from the service.</returns>
        public static string AuthenticateAsDefaultAdmin(this HttpClient client)
        {
            return Authenticate(client, "admin", "");
        }

        /// <summary>
        /// Authenticates as provided user against the application.
        /// Sets the bearer token as default request token and returns it.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>Bearer token returned from the service.</returns>
        public static string Authenticate(this HttpClient client, string userName, string password)
        {
            var model = new
            {
                userName = userName,
                password = password
            };

            string token = null;
            client
                .PostAsJson("/api/users/authenticate", JsonConvert.SerializeObject(model))
                .ExpectStatusCode(HttpStatusCode.OK)
                .ProcessTextContent(x => { token = x; });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return token;
        }
    }
}
