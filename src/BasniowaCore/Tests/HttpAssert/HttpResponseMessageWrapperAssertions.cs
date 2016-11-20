using System;
using System.IO;
using System.Net;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Tests.HttpAssert
{
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
}
