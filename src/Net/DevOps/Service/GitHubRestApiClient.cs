using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xanadu.Skidbladnir.Net.DevOps.Model.GitHub.Basic;
using Xanadu.Skidbladnir.Net.DevOps.Model.GitHub.Release;

namespace Xanadu.Skidbladnir.Net.DevOps.Service
{
    /// <summary>
    /// GitHub REST API client.
    /// </summary>
    /// <param name="httpClient">Http Client</param>
    public class GitHubRestApiClient(HttpClient httpClient)
    {
        /// <summary>
        /// GitHub API base URL.
        /// </summary>
        public static Uri BaseUrl => new("https://api.github.com");

        /// <summary>
        /// GitHub default headers.
        /// </summary>
        public static IReadOnlyDictionary<string, string> GitHubDefaultHeaders => new Dictionary<string, string>
        {
            { "Accept", "application/vnd.GitHub.v3+json" },
            { "User-Agent", "Xanadu.Skidbladnir" },
            { "X-GitHub-Api-Version", "2022-11-28" }
        };

        /// <summary>
        /// Default HttpClient with common settings.
        /// </summary>
        /// <param name="httpClient">Http Client instance.</param>
        public static void DefaultHttpClientAction(HttpClient httpClient)
        {
            RestApiClient.DefaultHttpClientAction(httpClient);
            httpClient.BaseAddress = GitHubRestApiClient.BaseUrl;
            foreach (var gitHubDefaultHeader in GitHubRestApiClient.GitHubDefaultHeaders)
            {
                httpClient.DefaultRequestHeaders.Add(gitHubDefaultHeader.Key, gitHubDefaultHeader.Value);
            }
        }

        /// <summary>
        /// Default HttpClient with common settings, can be configured via Action. Default HttpClientHandler is configured. BaseAddress is GitHub API base URL, DefaultRequestVersion is 3.0, DefaultVersionPolicy is RequestVersionOrLower.
        /// </summary>
        /// <param name="handlerConfigure">Configuration Action for HttpClientHandler.</param>
        /// <param name="clientConfigure">Configuration Action for HttpClient.</param>
        /// <returns>HttpClient Instance.</returns>
        public static HttpClient DefaultHttpClient(Action<HttpClientHandler>? handlerConfigure = null, Action<HttpClient>? clientConfigure = null)
        {
            var httpClient = RestApiClient.DefaultHttpClient(handlerConfigure, clientConfigure);
            GitHubRestApiClient.DefaultHttpClientAction(httpClient);
            return httpClient;
        }

        /// <summary>
        /// Gets the list of release assets for a specific release in a GitHub repository. See detail: https://docs.github.com/en/rest/releases/assets?apiVersion=2022-11-28#list-release-assets
        /// </summary>
        /// <param name="gitHubRepositoryInfoModel">GitHub repository info.</param>
        /// <param name="releaseId">Release ID</param>
        /// <returns>Release Assets Array, NULL if not existed or failed.</returns>
        public async Task<ReleaseAssetModel[]?> ListAssets(GitHubRepositoryInfoModel gitHubRepositoryInfoModel, long releaseId)
        {
            return await httpClient.GetFromJsonAsync<ReleaseAssetModel[]>(gitHubRepositoryInfoModel.ListReleaseAssetApi(releaseId));
        }

        /// <summary>
        /// Gets the list of releases in a GitHub repository. See detail: https://docs.github.com/en/rest/releases/releases?apiVersion=2022-11-28#list-releases
        /// </summary>
        /// <param name="gitHubRepositoryInfoModel">GitHub repository info.</param>
        /// <returns>Release Array, NULL if not existed or failed.</returns>
        public async Task<ReleaseModel[]?> ListReleases(GitHubRepositoryInfoModel gitHubRepositoryInfoModel)
        {
            return await httpClient.GetFromJsonAsync<ReleaseModel[]>(gitHubRepositoryInfoModel.ListReleasesApi);
        }
    }
}
