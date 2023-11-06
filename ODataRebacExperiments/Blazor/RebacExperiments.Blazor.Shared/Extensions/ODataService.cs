﻿using Microsoft.Extensions.Logging;
using RebacExperiments.Blazor.Shared.Http;
using RebacExperiments.Blazor.Shared.Logging;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace RebacExperiments.Blazor.Shared.Extensions
{
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

        public async Task<ODataEntityResponse<TEntityType>> GetEntityAsync<TEntityType>(string url, ODataQueryParameters parameters, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            // Create the HttpRequestMessageBuilder for the Request
            var httpRequestMessageBuilder = new HttpRequestMessageBuilder(url, HttpMethod.Get)
                .SetHeader("Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;");

            // Apply OData Parameters
            ApplyODataParameters(httpRequestMessageBuilder, parameters);

            // Builds the GET Request Message
            var httpRequestMessage = httpRequestMessageBuilder.Build();

            // Sends the HttpRequestMessage to the OData Service
            var httpResponseMessage = await _httpClient
                .SendAsync(httpRequestMessage, cancellationToken)
                .ConfigureAwait(false);

            // Parses the OData Result
            var response = await _parser.ParseEntityAsync<TEntityType>(httpResponseMessage, cancellationToken)
                .ConfigureAwait(false);

            return response;
        }

        public async Task<ODataEntitiesResponse<TEntityType>> GetEntitiesAsync<TEntityType>(string url, ODataQueryParameters parameters, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            // Create the HttpRequestMessageBuilder for the Request
            var httpRequestMessageBuilder = new HttpRequestMessageBuilder(url, HttpMethod.Get)
                .SetHeader("Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;");

            // Apply OData Parameters
            ApplyODataParameters(httpRequestMessageBuilder, parameters);

            // Builds the GET Request Message
            var httpRequestMessage = httpRequestMessageBuilder.Build();

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

            // Builds the PATCH Request Message
            var httpRequestMessage = new HttpRequestMessageBuilder(url, HttpMethod.Patch)
                .SetStringContent(json, Encoding.UTF8, "application/json")
                .SetHeader("Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;")
                .SetHeader("Prefer", "return=representation")
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

        public async Task<ODataEntityResponse<TEntityType>> CreateAsync<TEntityType>(string url, TEntityType entity, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            // Serialize to JSON
            var json = JsonSerializer.Serialize(entity, _jsonSerializerOptions);

            // Builds the POST Request Message
            var httpRequestMessage = new HttpRequestMessageBuilder(url, HttpMethod.Post)
                .SetStringContent(json, Encoding.UTF8, "application/json")
                .SetHeader("Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;")
                .SetHeader("Prefer", "return=representation")
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


        public async Task<ODataResponse> DeleteAsync(string url, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            // Builds the DELETE Request Message
            var httpRequestMessage = new HttpRequestMessageBuilder(url, HttpMethod.Delete).Build();

            // Sends the HttpRequestMessage to the OData Service
            var httpResponseMessage = await _httpClient
                .SendAsync(httpRequestMessage, cancellationToken)
                .ConfigureAwait(false);

            // Parses the OData Result
            var response = await _parser.ParseAsync(httpResponseMessage, cancellationToken)
                .ConfigureAwait(false);

            return response;
        }

        private void ApplyODataParameters(HttpRequestMessageBuilder httpRequestMessageBuilder, ODataQueryParameters parameters)
        {
            if(parameters.Skip != null)
            {
                httpRequestMessageBuilder.SetQueryString("$skip", parameters.Skip.ToString()!);
            }

            if (parameters.Top != null)
            {
                httpRequestMessageBuilder.SetQueryString("$top", parameters.Top.ToString()!);
            }

            if (parameters.Filter != null)
            {
                httpRequestMessageBuilder.SetQueryString("$filter", parameters.Filter);
            }
            if (parameters.OrderBy != null)
            {
                httpRequestMessageBuilder.SetQueryString("$orderby", parameters.OrderBy);
            }
        } 
    }
}
