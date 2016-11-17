using FluentAssertions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.IO;
using System.Net;
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

    public static class HttpResponseMessageWrapperAssertions
    {
        public static HttpResponseMessageWrapper ExpectStatusCode(this HttpResponseMessageWrapper wrapper, HttpStatusCode expectedStatusCode)
        {
            wrapper.Response.StatusCode.Should().Be(expectedStatusCode, "Unexpected status code.");
            return wrapper;
        }

        public static HttpResponseMessageWrapper ExpectJsonSchema(this HttpResponseMessageWrapper wrapper, string schemaJson)
        {
            var schema = JSchema.Parse(schemaJson);
            bool isValid = wrapper.ContentJson.IsValid(schema);
            isValid.Should().BeTrue("Response content JSON schema validation failed.");
            return wrapper;
        }

        public static HttpResponseMessageWrapper ExpectJsonSchemaFromFile(this HttpResponseMessageWrapper wrapper, string schemaJsonPath)
        {
            var schemaJson = File.ReadAllText(schemaJsonPath);
            return wrapper.ExpectJsonSchema(schemaJson);
        }

        public static HttpResponseMessageWrapper ExpectJsonContent(
            this HttpResponseMessageWrapper wrapper, 
            string expectedContentJson,
            Action<dynamic> preprocessExpected = null)
        {
            var expectedContent = JToken.Parse(expectedContentJson);
            preprocessExpected?.Invoke(expectedContent);

            bool areEqual = JToken.DeepEquals(expectedContent, wrapper.ContentJson);
            areEqual.Should().BeTrue("Response content doesn't match expected content.");
            return wrapper;
        }

        public static HttpResponseMessageWrapper ExpectJsonContentFromFile(
            this HttpResponseMessageWrapper wrapper, 
            string expectedContentJsonPath,
            Action<dynamic> preprocessExpected = null)
        {
            var expectedContentJson = File.ReadAllText(expectedContentJsonPath);
            return wrapper.ExpectJsonContent(expectedContentJson, preprocessExpected);
        }

        public static HttpResponseMessageWrapper ProcessJsonContent(this HttpResponseMessageWrapper wrapper, Action<dynamic> extract)
        {
            extract(wrapper.ContentJson);
            return wrapper;
        }

        public static HttpResponseMessageWrapper ProcessTextContent(this HttpResponseMessageWrapper wrapper, Action<string> extract)
        {
            extract(wrapper.ContentText);
            return wrapper;
        }
    }

    public class HttpResponseMessageWrapper
    {
        public HttpResponseMessage Response { get; private set; }

        private Lazy<string> _contentText;

        public string ContentText
        {
            get
            {
                return _contentText.Value;
            }
        }

        private Lazy<JToken> _contentJson;

        public JToken ContentJson
        {
            get
            {
                return _contentJson.Value;
            }
        }

        public HttpResponseMessageWrapper(HttpResponseMessage response)
        {
            Response = response;

            Response.Content.LoadIntoBufferAsync();
            _contentText = new Lazy<string>(() => Response.Content.ReadAsStringAsync().Result);
            _contentJson = new Lazy<JToken>(() => JToken.Parse(ContentText));
        }
    }
}
