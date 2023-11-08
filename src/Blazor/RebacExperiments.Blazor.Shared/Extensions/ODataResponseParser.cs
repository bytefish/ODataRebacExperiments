// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using RebacExperiments.Blazor.Shared.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace RebacExperiments.Blazor.Shared.Extensions
{
    /// <summary>
    /// The <see cref="ODataResponseParser"/> is used to convert a <see cref="HttpResponseMessage"/> into 
    /// an <see cref="ODataResponse"/>, <see cref="ODataEntitiesResponse{TEntityType}"/>, 
    /// <see cref="ODataEntityResponse{TEntityType}"/>.
    /// </summary>
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

            JsonObject? content;
            try
            {
                content = await httpResponseMessage.Content
                    .ReadFromJsonAsync<JsonObject>(_jsonSerializerOptions, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to deserialize the JSON Payload");

                throw new ODataException("Internal Error during Deserialization", e);
            }

            if (content == null)
            {
                throw new ODataException("Failed to extract JSON from the response");
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

            JsonObject? content = null;

            try
            {
                content = await httpResponseMessage.Content
                    .ReadFromJsonAsync<JsonObject>(_jsonSerializerOptions, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to deserialize the JSON Payload");

                throw new ODataException("Internal Error during Deserialization", e);
            }

            if (content == null)
            {
                throw new ODataException("Failed to extract JSON from the response");
            }

            TEntityType? result;
            try
            {
                result = content.Deserialize<TEntityType>();
            } 
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to deserialize the result entity");

                throw new ODataException("Internal Error during Deserialization", e);
            }            

            return new ODataEntityResponse<TEntityType>
            {
                Status = (int) httpResponseMessage.StatusCode,
                Headers = GetHeaders(httpResponseMessage),
                Metadata = GetMetadata(content),
                Entity = result,
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

            JsonObject? content;

            try
            {
                content = await httpResponseMessage.Content
                    .ReadFromJsonAsync<JsonObject>(_jsonSerializerOptions, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to deserialize the JSON Payload");

                throw new ODataException("Internal Error during Deserialization", e);
            }

            if (content == null)
            {
                throw new ODataException("Failed to extract JSON from the response");
            }

            if (!content.ContainsKey("value"))
            {
                throw new ODataException("Invalid Response. Expected a Key with Name 'value'");
            }

            List<TEntityType>? result;

            try
            {
                result = content["value"].Deserialize<List<TEntityType>>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to deserialize the result entity");

                throw new ODataException("Internal Error during Deserialization", e);
            }

            return new ODataEntitiesResponse<TEntityType>
            {
                Status = (int)httpResponseMessage.StatusCode,
                Headers = GetHeaders(httpResponseMessage),
                Metadata = GetMetadata(content),
                Entities = result
            };
        }

        /// <summary>
        /// Extracts the Headers from a given <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="httpResponseMessage">HTTP Response</param>
        /// <returns></returns>
        private Dictionary<string, List<string>> GetHeaders(HttpResponseMessage httpResponseMessage)
        {
            _logger.TraceMethodEntry();

            return httpResponseMessage.Headers.ToDictionary(header => header.Key, header => header.Value.ToList());
        }

        /// <summary>
        /// Extracts all OData Metadata from a given <see cref="JsonObject"/>, which is the 
        /// keys prefixed with "@odata".
        /// </summary>
        /// <param name="content">Json Content</param>
        /// <returns>A <see cref="Dictionary{string, object?}"/> with the OData Metadata</returns>
        private Dictionary<string, object?> GetMetadata(JsonObject content)
        {
            _logger.TraceMethodEntry();

            return content
                .Where(x => x.Key.ToLowerInvariant().StartsWith("@odata"))
                .GroupBy(x => x.Key).First()
                .ToDictionary(x => x.Key, x => (object?)content[x.Key]);
        }

        /// <summary>
        /// Gets an <see cref="ODataError"/> from a given <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="httpResponseMessage">HTTP Response with JSON Payload</param>
        /// <param name="cancellationToken">Cancellation Token to cancel asynchronous processing</param>
        /// <returns>The <see cref="ODataError"/> of the <see cref="HttpResponseMessage"/> body</returns>
        /// <exception cref="ODataException">If the Error cannot be extracted, an <see cref="ODataException"/> is thrown</exception>
        private async Task<ODataError> GetODataErrorAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                throw new ODataException("Could not extract an ODataError from a successful response");
            }

            ODataError? odataError;

            try
            {

                odataError = await httpResponseMessage.Content
                    .ReadFromJsonAsync<ODataError>(_jsonSerializerOptions, cancellationToken)
                    .ConfigureAwait(false);
            } 
            catch(Exception e)
            {
                _logger.LogError(e, "Deserializing the ODataError from Response failed");

                throw new ODataException($"HTTP Request failed with Status '{httpResponseMessage.StatusCode}'", e);
            }
            
            // If we got no error, just throw an exception ...
            if (odataError == null)
            {
                throw new ODataException($"HTTP Request failed with Status '{httpResponseMessage.StatusCode}'");
            }

            return odataError;
        }
    }
}
