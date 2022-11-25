using System.Text.RegularExpressions;

namespace Skidbladnir.Net.DevOps.Azure
{
    public class AzureArtifactsFeeds
    {
        public static Regex OrganizationFeedsUrlRegex =>
            new(@"^https:\/\/dev\.azure\.com\/(?<Organization>(.+?))\/_artifacts\/feed\/(?<FeedsName>(.+?))$", RegexOptions.ExplicitCapture);

        public static Regex ProjectFeedsUrlRegex =>
            new(@"^https:\/\/dev\.azure\.com\/(?<Organization>(.+?))\/(?<Project>(.+?))\/_artifacts\/feed\/(?<FeedsName>(.+?))$", RegexOptions.ExplicitCapture);

        public string Organization { get; set; }

        public string Project { get; set; }

        public string FeedsName { get; set; }

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
                if (!AzureArtifactsFeeds.ProjectFeedsUrlRegex.IsMatch(value) &&
                    !AzureArtifactsFeeds.OrganizationFeedsUrlRegex.IsMatch(value))
                {
                    return;
                }

                if (AzureArtifactsFeeds.ProjectFeedsUrlRegex.IsMatch(value))
                {
                    var m = AzureArtifactsFeeds.ProjectFeedsUrlRegex.Match(value);
                    this.Organization = m.Groups["Organization"].Value;
                    this.Project = m.Groups["Project"].Value;
                    this.FeedsName = m.Groups["FeedsName"].Value;
                    return;
                }

                var match = AzureArtifactsFeeds.OrganizationFeedsUrlRegex.Match(value);
                this.Organization = match.Groups["Organization"].Value;
                this.FeedsName = match.Groups["FeedsName"].Value;
            }
        }

        public bool IsAvailable => !string.IsNullOrWhiteSpace(this.Organization) && !string.IsNullOrEmpty(this.FeedsName);

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
