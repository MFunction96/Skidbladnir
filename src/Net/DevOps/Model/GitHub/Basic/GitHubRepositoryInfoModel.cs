using System.Security;
using System.Text.RegularExpressions;
using Xanadu.Skidbladnir.Core.Extension;
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

namespace Xanadu.Skidbladnir.Net.DevOps.Model.GitHub.Basic
{
    /// <summary>
    /// GitHub repository information.
    /// </summary>
    public partial class GitHubRepositoryInfoModel : IRepositoryInfoModel
    {
        /// <summary>
        /// Regular expression to match GitHub repository URLs.
        /// </summary>
        [GeneratedRegex(@"^https:\/\/github\.com\/(?<Owner>(.+?))\/(?<Repository>(.+?))$", RegexOptions.ExplicitCapture)]
        private static partial Regex RepositoryUrlRegex();

        /// <summary>
        /// Regular expression to match GitHub repository URLs.
        /// </summary>
        public static readonly Regex RepositoryRegex = GitHubRepositoryInfoModel.RepositoryUrlRegex();

        /// <summary>
        /// The owner of the GitHub repository.
        /// </summary>
        public string Owner { get; set; } = string.Empty;

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
                if (string.IsNullOrEmpty(value) || !GitHubRepositoryInfoModel.RepositoryRegex.IsMatch(value))
                {
                    return;
                }

                var match = GitHubRepositoryInfoModel.RepositoryRegex.Match(value);
                this.Owner = match.Groups["Owner"].Value;
                this.Repository = match.Groups["Repository"].Value;
            }
        }

        /// <inheritdoc />
        public bool IsAvailable => !string.IsNullOrEmpty(this.Owner) && !string.IsNullOrEmpty(this.Repository);

        /// <inheritdoc />
        public string GitOriginUrl(SecureString pat)
        {
            return this.IsAvailable ? $"https://{this.Owner}:{pat.ToNormalString()}@github.com/{this.Owner}/{this.Repository}.git" : string.Empty;
        }

        /// <summary>
        /// See detail: https://docs.github.com/en/rest/releases/releases?apiVersion=2022-11-28#list-releases
        /// </summary>
        public string ListReleasesApi => this.IsAvailable ? $"/repos/{this.Owner}/{this.Repository}/releases" : string.Empty;

        /// <summary>
        /// See detail: https://docs.github.com/en/rest/releases/assets?apiVersion=2022-11-28#list-release-assets
        /// </summary>
        /// <param name="releaseId">Release ID</param>
        public string ListReleaseAssetApi(long releaseId) => this.IsAvailable ? $"/repos/{this.Owner}/{this.Repository}/releases/{releaseId}/assets" : string.Empty;

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            var model = obj as GitHubRepositoryInfoModel;
            return model is not null && this.Owner == model.Owner && this.Repository == model.Repository;
        }
    }
}