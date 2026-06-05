using System;
using System.Net;
using System.Net.Http;

namespace Xanadu.Skidbladnir.Net.DevOps
{
    /// <summary>
    /// REST API client helper.
    /// </summary>
    public static class RestApiClient
    {
        /// <summary>
        /// Default HttpClientHandler with common settings.
        /// </summary>
        /// <param name="handler">SocketsHttpHandler instance.</param>
        public static void DefaultSocketsHttpHandlerAction(SocketsHttpHandler handler)
        {
            handler.AllowAutoRedirect = true;
            handler.AutomaticDecompression = DecompressionMethods.All;
        }

        /// <summary>
        /// Default HttpClient with common settings. Preferred HTTP version is 3.0, but will fallback to lower versions if not supported by server. BaseAddress is null, can be configured via Action.
        /// </summary>
        /// <param name="httpClient">Http Client instance.</param>
        public static void DefaultHttpClientAction(HttpClient httpClient)
        {
            httpClient.DefaultRequestVersion = HttpVersion.Version30;
            httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
        }

        /// <summary>
        /// Default SocketsHttpHandler with common settings, can be configured via Action. AllowAutoRedirect is true, AutomaticDecompression is All.
        /// </summary>
        /// <param name="configure">Configuration Action for SocketsHttpHandler.</param>
        /// <returns>SocketsHttpHandler Instance.</returns>
        public static SocketsHttpHandler DefaultSocketsHttpHandler(Action<SocketsHttpHandler>? configure = null)
        {
            var handler = new SocketsHttpHandler();
            RestApiClient.DefaultSocketsHttpHandlerAction(handler);
            configure?.Invoke(handler);
            return handler;
        }

        /// <summary>
        /// Default HttpClient with common settings, can be configured via Action. Default SocketsHttpHandler is configured. DefaultRequestVersion is 3.0, DefaultVersionPolicy is RequestVersionOrHigher.
        /// </summary>
        /// <param name="handlerConfigure">Configuration Action for SocketsHttpHandler.</param>
        /// <param name="clientConfigure">Configuration Action for HttpClient.</param>
        /// <returns>HttpClient Instance.</returns>
        public static HttpClient DefaultHttpClient(Action<SocketsHttpHandler>? handlerConfigure = null, Action<HttpClient>? clientConfigure = null)
        {
            var httpClient = new HttpClient(RestApiClient.DefaultSocketsHttpHandler(handlerConfigure));
            RestApiClient.DefaultHttpClientAction(httpClient);
            clientConfigure?.Invoke(httpClient);
            return httpClient;
        }
    }
}
