using Microsoft.Extensions.Logging;
using RebacExperiments.Blazor.Shared.Http;
using RebacExperiments.Blazor.Shared.Logging;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace RebacExperiments.Blazor.Shared.Extensions
{
    public class ODataServiceOptions
    {
        public required string ServiceUri { get; set; }
    }

    public class ODataService
    {
        private readonly ILogger<ODataService> _logger;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ODataResponseParser _parser;

        public ODataService(ILogger<ODataService> logger, HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions, ODataResponseParser parser) 
        {
            _logger = logger;
            _httpClient = httpClient;
            _jsonSerializerOptions = jsonSerializerOptions;
            _parser = parser;
        }

        public async Task<ODataEntityResponse<TEntityType>> GetEntity<TEntityType>(string url, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            // Builds the GET Request Message
            var httpRequestMessage = new HttpRequestMessageBuilder(url, HttpMethod.Get)
                .SetHeader("Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;")
                .Build();

            // Sends the HttpRequestMessage to the OData Service
            var httpResponseMessage = await _httpClient
                .SendAsync(httpRequestMessage, cancellationToken)
                .ConfigureAwait(false);

            // Parses the OData Result
            var response = await _parser.ParseEntityAsync<TEntityType>(httpResponseMessage, cancellationToken)
                .ConfigureAwait(false);

            return response;
        }

        public async Task<ODataEntitiesResponse<TEntityType>> GetEntities<TEntityType>(string url, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            // Builds the GET Request Message
            var httpRequestMessage = new HttpRequestMessageBuilder(url, HttpMethod.Get)
                .SetHeader("Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;")
                .Build();

            // Sends the HttpRequestMessage to the OData Service
            var httpResponseMessage = await _httpClient
                .SendAsync(httpRequestMessage, cancellationToken)
                .ConfigureAwait(false);

            // Parses the OData Result
            var response = await _parser
                .ParseEntitiesAsync<TEntityType>(httpResponseMessage, cancellationToken)
                .ConfigureAwait(false);

            return response;
        }

        public async Task<ODataEntityResponse<TEntityType>> UpdateAsync<TEntityType>(string url, TEntityType entity, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            // Serialize to JSON
            var json = JsonSerializer.Serialize(entity, _jsonSerializerOptions);

            var httpRequestMessage = new HttpRequestMessageBuilder(url, HttpMethod.Patch)
                .SetStringContent(json, Encoding.UTF8, "application/json")
                .SetHeader("Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;")
                .SetHeader("Prefer", "return=representation")
            .Build();

            var httpResponseMessage = await _httpClient
                .SendAsync(httpRequestMessage, cancellationToken)
                .ConfigureAwait(false);

            var response = await _parser.ParseEntityAsync<TEntityType>(httpResponseMessage, cancellationToken)
                .ConfigureAwait(false);


            return response;
        }

    
    }
}
