using Microsoft.OData.Client;

namespace RebacExperiments.Blazor.Shared.Infrastructure
{
    /// <summary>
    /// This has been written by @Finickyflame (https://github.com/Finickyflame):
    /// 
    ///     * https://github.com/OData/odata.net/issues/2485
    ///     
    /// </summary>
    public class HttpClientHandlerProvider : IHttpClientHandlerProvider
    {
        private readonly string _name;
        private readonly IHttpMessageHandlerFactory _factory;

        public HttpClientHandlerProvider(string name, IHttpMessageHandlerFactory factory)
        {
            _name = name;
            _factory = factory;
        }

        public HttpClientHandler GetHttpClientHandler() => new HttpMessageHandlerToClientAdapter(_factory.CreateHandler(_name));
    }
}
