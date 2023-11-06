// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using Microsoft.OData;
using RebacExperiments.Blazor.Shared.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace RebacExperiments.Blazor.Shared.Extensions
{
    public class ODataResponseParser
    {
        private readonly ILogger<ODataResponseParser> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ODataResponseParser(ILogger<ODataResponseParser> logger, JsonSerializerOptions jsonSerializerOptions) 
        {
            _logger = logger;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        public async Task<ODataResponse> ParseAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var odataError = await GetODataErrorAsync(httpResponseMessage, cancellationToken);

                throw new ODataErrorException($"HTTP Request failed with Status {httpResponseMessage.StatusCode}")
                {
                    ODataError = odataError
                };
            }

            var content = await httpResponseMessage.Content
                .ReadFromJsonAsync<JsonObject>(_jsonSerializerOptions, cancellationToken)
                .ConfigureAwait(false);

            if (content == null)
            {
                throw new InvalidOperationException("");
            }

            return new ODataResponse
            {
                Status = (int)httpResponseMessage.StatusCode,
                Headers = GetHeaders(httpResponseMessage),
                Metadata = GetMetadata(content)
            };
        }

        public async Task<ODataEntityResponse<TEntityType>> ParseEntityAsync<TEntityType>(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var odataError = await GetODataErrorAsync(httpResponseMessage, cancellationToken);

                throw new ODataErrorException($"HTTP Request failed with Status {httpResponseMessage.StatusCode}")
                {
                    ODataError = odataError
                };
            }

            var content = await httpResponseMessage.Content
                .ReadFromJsonAsync<JsonObject>(_jsonSerializerOptions, cancellationToken)
                .ConfigureAwait(false);

            if(content == null)
            {
                throw new InvalidOperationException("");
            }

            return new ODataEntityResponse<TEntityType>
            {
                Status = (int) httpResponseMessage.StatusCode,
                Headers = GetHeaders(httpResponseMessage),
                Metadata = GetMetadata(content),
                Entity = content.Deserialize<TEntityType>(),
            };
        }

        public async Task<ODataEntitiesResponse<TEntityType>> ParseEntitiesAsync<TEntityType>(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var odataError = await GetODataErrorAsync(httpResponseMessage, cancellationToken);

                throw new ODataErrorException($"HTTP Request failed with Status {httpResponseMessage.StatusCode}")
                { 
                    ODataError = odataError 
                };
            }

            var content = await httpResponseMessage.Content
                .ReadFromJsonAsync<JsonObject>(_jsonSerializerOptions, cancellationToken)
                .ConfigureAwait(false);

            if (content == null)
            {
                throw new InvalidOperationException("HTTP Response has no content");
            }

            if (!content.ContainsKey("value"))
            {
                throw new ODataException("Expected a Key with Name 'value'");
            }

            var entities = content["value"].Deserialize<List<TEntityType>>();

            if(entities == null)
            {
                throw new ODataException("Deserializing the Entities failed");
            }

            return new ODataEntitiesResponse<TEntityType>
            {
                Status = (int)httpResponseMessage.StatusCode,
                Headers = GetHeaders(httpResponseMessage),
                Metadata = GetMetadata(content),
                Entities = entities
            };
        }

        private Dictionary<string, List<string>> GetHeaders(HttpResponseMessage httpResponseMessage)
        {
            _logger.TraceMethodEntry();

            return httpResponseMessage.Headers.ToDictionary(x => x.Key, x => x.Value.ToList());
        }

        private Dictionary<string, object?> GetMetadata(JsonObject content)
        {
            _logger.TraceMethodEntry();

            return content
                .Where(x => x.Key.ToLowerInvariant().StartsWith("@odata"))
                .GroupBy(x => x.Key).First()
                .ToDictionary(x => x.Key, x => (object?)content[x.Key]);
        }

        private async Task<ODataError> GetODataErrorAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                throw new ODataException("Could not extract an ODataError from a successful response");
            }

            var odataError = await httpResponseMessage.Content.ReadFromJsonAsync<ODataError>(_jsonSerializerOptions, cancellationToken);

            if(odataError == null)
            {
                throw new ODataException($"HTTP Request failed with Status '{httpResponseMessage.StatusCode}'");
            }

            return odataError;
        }

    }
}
