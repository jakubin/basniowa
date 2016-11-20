using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Tests.HttpAssert
{
    public static class HttpHelper
    {
        private static readonly string JsonContentType = "application/json";

        public static HttpResponseMessageWrapper GetAsJson(this HttpClient client, string uri)
        {
            return client.SendAsJson(HttpMethod.Get, uri);
        }

        public static HttpResponseMessageWrapper PostAsJson(this HttpClient client, string uri, string content)
        {
            return client.SendAsJson(HttpMethod.Post, uri, content);
        }

        public static HttpResponseMessageWrapper PostAsJsonFromFile(this HttpClient client, string uri, string contentPath)
        {
            return client.SendAsJsonFromFile(HttpMethod.Post, uri, contentPath);
        }

        public static HttpResponseMessageWrapper SendAsJsonFromFile(this HttpClient client, HttpMethod method, string uri, string contentPath)
        {
            var content = File.ReadAllText(contentPath);
            return SendAsJson(client, method, uri, content);
        }

        public static HttpResponseMessageWrapper SendAsJson(this HttpClient client, HttpMethod method, string uri, string content = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonContentType));
            if (content != null)
            {
                request.Content = new StringContent(content, Encoding.UTF8, JsonContentType);
            }

            var response = client.SendAsync(request).Result;
            return new HttpResponseMessageWrapper(response);
        }
    }
}
