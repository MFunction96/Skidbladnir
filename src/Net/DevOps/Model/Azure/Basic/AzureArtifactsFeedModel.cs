using System.Text.RegularExpressions;

namespace Xanadu.Skidbladnir.Net.DevOps.Model.Azure.Basic
{
    /// <summary>
    /// Azure Artifacts Feed Model.
    /// </summary>
    public partial class AzureArtifactsFeedModel
    {
        /// <summary>
        /// Azure DevOps Organization Level Feeds URL Regex.
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex(@"^https:\/\/dev\.azure\.com\/(?<Organization>(.+?))\/_artifacts\/feed\/(?<FeedsName>(.+?))$", RegexOptions.ExplicitCapture)]
        private static partial Regex AzureOrganizationFeedsUrlRegex();

        /// <summary>
        /// Azure DevOps Project Level Feeds URL Regex.
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex(@"^https:\/\/dev\.azure\.com\/(?<Organization>(.+?))\/(?<Project>(.+?))\/_artifacts\/feed\/(?<FeedsName>(.+?))$", RegexOptions.ExplicitCapture)]
        private static partial Regex AzureProjectFeedsUrlRegex();

        /// <summary>
        /// Organization Name.
        /// </summary>
        private string _organization = string.Empty;

        /// <summary>
        /// Project Name. Optional.
        /// </summary>
        private string _project = string.Empty;

        /// <summary>
        /// Feed Name.
        /// </summary>
        private string _feedName = string.Empty;

        /// <summary>
        /// Azure DevOps Organization Level Feeds URL Regex.
        /// </summary>
        public static Regex OrganizationFeedsUrlRegex => AzureArtifactsFeedModel.AzureOrganizationFeedsUrlRegex();

        /// <summary>
        /// Azure DevOps Project Level Feeds URL Regex.
        /// </summary>
        public static Regex ProjectFeedsUrlRegex => AzureArtifactsFeedModel.AzureProjectFeedsUrlRegex();

        /// <summary>
        /// Organization Name.
        /// </summary>
        public string Organization
        {
            get
            {
                return this._organization;
            }
            set
            {
                this._organization = value.Trim().Trim('/');
            }
        } 

        /// <summary>
        /// Project Name. Optional.
        /// </summary>
        public string Project
        {
            get
            {
                return this._project;
            }
            set
            {
                this._project = value.Trim().Trim('/');
            }
        }

        /// <summary>
        /// Feed Name.
        /// </summary>
        public string FeedsName
        {
            get
            {
                return this._feedName;
            }
            set
            {
                this._feedName = value.Trim().Trim('/');
            }
        }

        /// <summary>
        /// Azure Artifacts Feed URL.
        /// </summary>
        public string FeedsUrl
        {
            get
            {
                if (!this.IsAvailable)
                {
                    return string.Empty;
                }

                return string.IsNullOrEmpty(this.Project)
                    ? $"https://dev.azure.com/{this.Organization}/_artifacts/feed/{this.FeedsName}"
                    : $"https://dev.azure.com/{this.Organization}/{this.Project}/_artifacts/feed/{this.FeedsName}";
            }
            set
            {
                if (!AzureArtifactsFeedModel.ProjectFeedsUrlRegex.IsMatch(value) &&
                    !AzureArtifactsFeedModel.OrganizationFeedsUrlRegex.IsMatch(value))
                {
                    return;
                }

                if (AzureArtifactsFeedModel.ProjectFeedsUrlRegex.IsMatch(value))
                {
                    var m = AzureArtifactsFeedModel.ProjectFeedsUrlRegex.Match(value);
                    this.Organization = m.Groups["Organization"].Value;
                    this.Project = m.Groups["Project"].Value;
                    this.FeedsName = m.Groups["FeedsName"].Value;
                    return;
                }

                var match = AzureArtifactsFeedModel.OrganizationFeedsUrlRegex.Match(value);
                this.Organization = match.Groups["Organization"].Value;
                this.FeedsName = match.Groups["FeedsName"].Value;
            }
        }

        /// <summary>
        /// Azure Artifacts Feed Model is available.
        /// </summary>
        public bool IsAvailable => !string.IsNullOrEmpty(this.Organization) && !string.IsNullOrEmpty(this.FeedsName);

        /// <summary>
        /// Azure Artifacts Feed NuGet Origin URL.
        /// </summary>
        public string OriginUrl
        {
            get
            {
                if (!this.IsAvailable)
                {
                    return string.Empty;
                }

                return string.IsNullOrEmpty(this.Project)
                    ? $"https://pkgs.dev.azure.com/{this.Organization}/_packaging/{this.FeedsName}/nuget/v3/index.json"
                    : $"https://pkgs.dev.azure.com/{this.Organization}/{this.Project}/_packaging/{this.FeedsName}/nuget/v3/index.json";
            }
        }

    }
}
