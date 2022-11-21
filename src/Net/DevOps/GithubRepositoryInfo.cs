using System.Security;
using System.Text.RegularExpressions;

namespace Skidbladnir.Net.DevOps
{
    public class GithubRepositoryInfo : RepositoryInfo
    {
        public string Username { get; set; }

        public static readonly Regex RepositoryRegex = new(@"https:\/\/github\.com\/(?<Username>(.+?))\/(?<Repository>(.+?))", RegexOptions.ExplicitCapture);

        public override string RepositoryUrl
        {
            get
            {
                return $"https://dev.azure.com/{Username}/{Repository}";
            }
            set
            {
                if (!RepositoryRegex.IsMatch(value))
                {
                    return;
                }
                var match = RepositoryRegex.Match(value);
                Username = match.Groups["Username"].Value;
                Repository = match.Groups["Repository"].Value;
            }
        }

        public override string OriginUrl(SecureString pat)
        {
            return $"https://{Username}:{pat}@github.com/{Username}/{Repository}.git";
        }

        public string MetadataUrl => $"https://api.github.com/repos/{Username}/{Repository}/tags";
    }
}