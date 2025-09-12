using System.Security;
using System.Text.RegularExpressions;
using Xanadu.Skidbladnir.Core.Extension;

namespace Xanadu.Skidbladnir.Net.DevOps.Model.Github.Basic
{
    /// <summary>
    /// GitHub repository information.
    /// </summary>
    public partial class GithubRepositoryInfoModel : IRepositoryInfoModel
    {
        /// <summary>
        /// Regular expression to match GitHub repository URLs.
        /// </summary>
        [GeneratedRegex(@"^https:\/\/github\.com\/(?<Owner>(.+?))\/(?<Repository>(.+?))$", RegexOptions.ExplicitCapture)]
        private static partial Regex RepositoryUrlRegex();

        /// <summary>
        /// The owner of the GitHub repository.
        /// </summary>
        public string Owner { get; set; } = string.Empty;

        /// <summary>
        /// Regular expression to match GitHub repository URLs.
        /// </summary>
        public static readonly Regex RepositoryRegex = GithubRepositoryInfoModel.RepositoryUrlRegex();

        /// <inheritdoc />
        public string Repository { get; set; } = string.Empty;

        /// <inheritdoc />
        public string RepositoryUrl
        {
            get
            {
                return this.IsAvailable ? $"https://github.com/{this.Owner}/{this.Repository}" : string.Empty;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || !GithubRepositoryInfoModel.RepositoryRegex.IsMatch(value))
                {
                    return;
                }

                var match = GithubRepositoryInfoModel.RepositoryRegex.Match(value);
                this.Owner = match.Groups["Owner"].Value;
                this.Repository = match.Groups["Repository"].Value;
            }
        }

        /// <inheritdoc />
        public bool IsAvailable => !string.IsNullOrEmpty(this.Owner) && !string.IsNullOrEmpty(this.Repository);

        /// <inheritdoc />
        public string GitOriginUrl(SecureString pat)
        {
            return this.IsAvailable ? $"https://{this.Owner}:{pat.ToStr()}@github.com/{this.Owner}/{this.Repository}.git" : string.Empty;
        }

        /// <summary>
        /// See detail: https://docs.github.com/en/rest/releases/releases?apiVersion=2022-11-28#list-releases
        /// </summary>
        public string ListReleasesApi => this.IsAvailable ? $"https://api.github.com/repos/{this.Owner}/{this.Repository}/releases" : string.Empty;

        /// <summary>
        /// See detail: https://docs.github.com/en/rest/releases/assets?apiVersion=2022-11-28#list-release-assets
        /// </summary>
        /// <param name="releaseId">Release ID</param>
        public string ListReleaseAssetApi(long releaseId) => this.IsAvailable ? $"https://api.github.com/repos/{this.Owner}/{this.Repository}/releases/${releaseId}/assets" : string.Empty;
    }
}