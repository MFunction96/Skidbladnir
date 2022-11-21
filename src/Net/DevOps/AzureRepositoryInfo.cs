using System.Security;
using System.Text.RegularExpressions;

namespace Skidbladnir.Net.DevOps
{
    public class AzureRepositoryInfo : RepositoryInfo
    {
        public string Organization { get; set; }

        public string Project { get; set; }

        public static readonly Regex RepositoryRegex = new(@"https:\/\/dev\.azure\.com\/(?<Organization>(.+?))\/(?<Project>(.+?))\/(?<Repository>(.+?))", RegexOptions.ExplicitCapture);

        public override string RepositoryUrl
        {
            get
            {
                return $"https://dev.azure.com/{Organization}/{Project}/{Repository}";
            }
            set
            {
                if (!RepositoryRegex.IsMatch(value))
                {
                    return;
                }
                var match = RepositoryRegex.Match(value);
                Organization = match.Groups["Organization"].Value;
                Project = match.Groups["Project"].Value;
                Repository = match.Groups["Repository"].Value;
            }
        }

        public override string OriginUrl(SecureString pat)
        {
            return $"https://{pat}@dev.azure.com/{Organization}/{Project}/_git/{Repository}";
        }
    }
}