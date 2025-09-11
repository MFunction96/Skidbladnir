using System.Security;
using System.Text.RegularExpressions;
using Xanadu.Skidbladnir.Core.Extension;

namespace Xanadu.Skidbladnir.Net.DevOps.Model.Azure.Basic
{
    /// <summary>
    /// Azure DevOps repository information.
    /// </summary>
    public partial class AzureRepositoryInfoModel : IRepositoryInfoModel
    {
        /// <summary>
        /// Creates a regular expression to match Azure DevOps repository URLs.
        /// </summary>
        /// <returns>A <see cref="Regex"/> instance configured to match Azure DevOps repository URLs.</returns>
        [GeneratedRegex(@"^https:\/\/dev\.azure\.com\/(?<Organization>(.+?))\/(?<Project>(.+?))\/_git\/(?<Repository>(.+?))$", RegexOptions.ExplicitCapture)]
        private static partial Regex RepositoryUrlRegex();

        /// <summary>
        /// A regular expression used to validate and parse repository URLs.
        /// </summary>
        public static readonly Regex RepositoryRegex = AzureRepositoryInfoModel.RepositoryUrlRegex();

        /// <summary>
        /// Organization name.
        /// </summary>
        public string Organization { get; set; } = string.Empty;

        /// <summary>
        /// Project name.
        /// </summary>
        public string Project { get; set; } = string.Empty;

        /// <inheritdoc />
        public string Repository { get; set; } = string.Empty;

        /// <inheritdoc />
        public string RepositoryUrl
        {
            get
            {
                return this.IsAvailable ? $"https://dev.azure.com/{this.Organization}/{this.Project}/{this.Repository}" : string.Empty;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || !AzureRepositoryInfoModel.RepositoryRegex.IsMatch(value))
                {
                    return;
                }

                var match = AzureRepositoryInfoModel.RepositoryRegex.Match(value);
                this.Organization = match.Groups["Organization"].Value;
                this.Project = match.Groups["Project"].Value;
                this.Repository = match.Groups["Repository"].Value;
            }
        }

        /// <inheritdoc />
        public bool IsAvailable => !string.IsNullOrEmpty(this.Organization) &&
                                   !string.IsNullOrEmpty(this.Project) &&
                                   !string.IsNullOrEmpty(this.Repository);

        /// <inheritdoc />
        public string GitOriginUrl(SecureString pat)
        {
            return this.IsAvailable ? $"https://{pat.ToStr()}@dev.azure.com/{this.Organization}/{this.Project}/_git/{this.Repository}" : string.Empty;
        }

    }
}