using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Tests.HttpAssert
{
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
