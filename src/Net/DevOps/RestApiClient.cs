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
        /// <param name="handler">HttpClientHandler instance.</param>
        public static void DefaultHttpClientHandlerAction(HttpClientHandler handler)
        {
            handler.AllowAutoRedirect = true;
            handler.AutomaticDecompression = System.Net.DecompressionMethods.All;
        }

        /// <summary>
        /// Default HttpClient with common settings.
        /// </summary>
        /// <param name="httpClient">Http Client instance.</param>
        public static void DefaultHttpClientAction(HttpClient httpClient)
        {
            httpClient.DefaultRequestVersion = HttpVersion.Version30;
            httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
        }

        /// <summary>
        /// Default HttpClientHandler with common settings, can be configured via Action. AllowAutoRedirect is true, AutomaticDecompression is All.
        /// </summary>
        /// <param name="configure">Configuration Action for HttpClientHandler.</param>
        /// <returns>HttpClientHandler Instance.</returns>
        public static HttpClientHandler DefaultHttpClientHandler(Action<HttpClientHandler>? configure = null)
        {
            var handler = new HttpClientHandler();
            RestApiClient.DefaultHttpClientHandlerAction(handler);
            configure?.Invoke(handler);
            return handler;
        }

        /// <summary>
        /// Default HttpClient with common settings, can be configured via Action. Default HttpClientHandler is configured. DefaultRequestVersion is 3.0, DefaultVersionPolicy is RequestVersionOrLower.
        /// </summary>
        /// <param name="handlerConfigure">Configuration Action for HttpClientHandler.</param>
        /// <param name="clientConfigure">Configuration Action for HttpClient.</param>
        /// <returns>HttpClient Instance.</returns>
        public static HttpClient DefaultHttpClient(Action<HttpClientHandler>? handlerConfigure = null, Action<HttpClient>? clientConfigure = null)
        {
            var httpClient = new HttpClient(RestApiClient.DefaultHttpClientHandler(handlerConfigure));
            RestApiClient.DefaultHttpClientAction(httpClient);
            clientConfigure?.Invoke(httpClient);
            return httpClient;
        }
    }
}
