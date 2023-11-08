// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Text;

namespace RebacExperiments.Blazor.Shared.Http
{
    public class HttpRequestMessageBuilder
    {
        private record UrlSegment
        {
            public readonly string name;
            public readonly string value;

            public UrlSegment(string name, string value)
            {
                this.name = name;
                this.value = value;
            }
        }

        private record Header
        {
            public readonly string Name;
            public readonly string Value;

            public Header(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }

        private readonly string _url;
        private HttpMethod _httpMethod;
        private BrowserRequestMode _browserRequestMode;
        private readonly IDictionary<string, string> _parameters;
        private readonly IList<Header> _headers;
        private readonly IList<UrlSegment> _segments;
        private HttpContent? _content;

        public HttpRequestMessageBuilder(string url, HttpMethod httpMethod)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            _url = url;
            _httpMethod = httpMethod;
            _browserRequestMode = BrowserRequestMode.Cors;
            _headers = new List<Header>();
            _segments = new List<UrlSegment>();
            _content = null;
            _parameters = new Dictionary<string, string>();
        }

        public HttpRequestMessageBuilder HttpMethod(HttpMethod httpMethod)
        {
            _httpMethod = httpMethod;

            return this;
        }

        public HttpRequestMessageBuilder AddHeader(string name, string value)
        {
            _headers.Add(new Header(name, value));

            return this;
        }

        public HttpRequestMessageBuilder SetHeader(string name, string value)
        {
            var header = _headers.FirstOrDefault(x => x.Name == name);

            if (header != null)
            {
                _headers.Remove(header);
            }

            AddHeader(name, value);

            return this;
        }

        public HttpRequestMessageBuilder SetStringContent(string content, Encoding encoding, string mediaType)
        {
            _content = new StringContent(content, encoding, mediaType);

            return this;
        }

        public HttpRequestMessageBuilder SetHttpContent(HttpContent httpContent)
        {
            _content = httpContent;

            return this;
        }

        public HttpRequestMessageBuilder AddUrlSegment(string name, string value)
        {
            _segments.Add(new UrlSegment(name, value));

            return this;
        }

        public HttpRequestMessageBuilder AddQueryString(string key, string value)
        {
            _parameters.Add(key, value);

            return this;
        }

        public HttpRequestMessageBuilder SetQueryString(string key, string value)
        {
            if(!_parameters.TryAdd(key, value))
            {
                _parameters[key] = value;
            }

            return this;
        }

        public HttpRequestMessageBuilder SetBrowserRequestMode(BrowserRequestMode browserRequestMode)
        {
            _browserRequestMode = browserRequestMode;

            return this;
        }

        public HttpRequestMessage Build()
        {
            var baseUrl = ReplaceSegments(_url, _segments);
            var queryString = BuildQueryString(baseUrl, _parameters);
            var unescapedRequestUri = $"{baseUrl}{queryString}";

            HttpRequestMessage httpRequestMessage = new (_httpMethod, unescapedRequestUri);

            foreach (var header in _headers)
            {
                httpRequestMessage.Headers.TryAddWithoutValidation(header.Name, header.Value);
            }

            if (_content != null)
            {
                httpRequestMessage.Content = _content;
            }

            httpRequestMessage.SetBrowserRequestMode(_browserRequestMode);

            return httpRequestMessage;
        }

        private static string ReplaceSegments(string resource, IList<UrlSegment> segments)
        {
            string url = new(resource.ToCharArray());

            foreach (var segment in segments)
            {
                url = url.Replace(segment.name, segment.value);
            }

            return url;
        }

        private static string BuildQueryString(string resource, IDictionary<string, string> parameters)
        {
            var builder = new StringBuilder();

            bool first = true;

            foreach (var parameter in parameters)
            {
                builder.Append(first ? "?" : "&");

                first = false;

                builder.Append(parameter.Key)
                    .Append("=")
                    .Append(parameter.Value);
            }

            return builder.ToString();
        }
    }
}
