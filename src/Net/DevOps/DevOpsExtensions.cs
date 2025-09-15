using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using Xanadu.Skidbladnir.Net.DevOps.Service;

namespace Xanadu.Skidbladnir.Net.DevOps
{
    /// <summary>
    /// DevOps related extension methods for IServiceCollection.
    /// </summary>
    public static class DevOpsExtensions
    {
        /// <summary>
        /// Adds GitHubRestApiClient to the IServiceCollection with optional HttpClient and HttpClientHandler configuration.
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="httpClientConfig">Custom HttpClient Action.</param>
        /// <param name="handlerConfig">Custom HttpClientHandler Action.</param>
        /// <returns>Service Collection.</returns>
        public static IServiceCollection AddGitHubRestApiClient(this IServiceCollection services, Action<HttpClient>? httpClientConfig = null, Action<HttpClientHandler>? handlerConfig = null)
        {
            services.AddHttpClient<GitHubRestApiClient>(nameof(GitHubRestApiClient), httpClient =>
            {
                GitHubRestApiClient.DefaultHttpClientAction(httpClient);
                httpClientConfig?.Invoke(httpClient);

            }).ConfigurePrimaryHttpMessageHandler(_ => GitHubRestApiClient.DefaultHttpClientHandler(handlerConfig));

            return services;
        }
    }
}
