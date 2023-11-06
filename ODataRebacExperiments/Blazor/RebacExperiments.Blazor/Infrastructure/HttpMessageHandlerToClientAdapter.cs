using System.Reflection;

namespace RebacExperiments.Blazor.Shared.Infrastructure
{

    /// <summary>
    /// Adapts the <see cref="HttpMessageHandler"/> to work with the <see cref="Microsoft.OData.Client.DataServiceContext"/>
    /// since it only accepts an <see cref="HttpClientHandler"/>.
    /// 
    /// This has been written by @Finickyflame (https://github.com/Finickyflame):
    /// 
    ///     * https://github.com/OData/odata.net/issues/2485
    ///     
    /// </summary>
    internal sealed class HttpMessageHandlerToClientAdapter : HttpClientHandler
    {
        private static readonly MethodInfo SendInfo = typeof(HttpMessageHandler).GetMethod(nameof(Send), BindingFlags.Instance | BindingFlags.NonPublic)!;
        private static readonly MethodInfo SendAsyncInfo = typeof(HttpMessageHandler).GetMethod(nameof(SendAsync), BindingFlags.Instance | BindingFlags.NonPublic)!;

        private readonly HttpMessageHandler _messageHandler;

        public HttpMessageHandlerToClientAdapter(HttpMessageHandler messageHandler)
        {
            this._messageHandler = messageHandler;
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
            => (HttpResponseMessage)SendInfo.Invoke(_messageHandler, new object[] { request, cancellationToken })!;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => (Task<HttpResponseMessage>)SendAsyncInfo.Invoke(_messageHandler, new object[] { request, cancellationToken })!;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._messageHandler.Dispose();
            }
        }
    }
}
