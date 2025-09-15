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
        public const string BaseUrl = "https://api.github.com";

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
