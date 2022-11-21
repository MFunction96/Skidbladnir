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
                return $"https://dev.azure.com/{this.Username}/{base.Repository}";
            }
            set
            {
                if (!GithubRepositoryInfo.RepositoryRegex.IsMatch(value))
                {
                    return;
                }
                var match = GithubRepositoryInfo.RepositoryRegex.Match(value);
                this.Username = match.Groups["Username"].Value;
                base.Repository = match.Groups["Repository"].Value;
            }
        }

        public override string OriginUrl(SecureString pat)
        {
            return $"https://{this.Username}:{pat}@github.com/{this.Username}/{base.Repository}.git";
        }

        public string MetadataUrl => $"https://api.github.com/repos/{this.Username}/{base.Repository}/tags";
    }
}