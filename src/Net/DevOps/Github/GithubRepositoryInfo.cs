using System.Security;
using System.Text.RegularExpressions;
using Skidbladnir.Core.Extension;

namespace Skidbladnir.Net.DevOps.Github
{
    public class GithubRepositoryInfo : RepositoryInfo
    {
        public string Username { get; set; }

        public static readonly Regex RepositoryRegex = new(@"^https:\/\/github\.com\/(?<Username>(.+?))\/(?<Repository>(.+?))$", RegexOptions.ExplicitCapture);

        public override string RepositoryUrl
        {
            get
            {
                return IsAvailable ? $"https://github.com/{Username}/{Repository}" : string.Empty;
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

        public override bool IsAvailable => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Repository);

        public override string OriginUrl(SecureString pat)
        {
            return IsAvailable ? $"https://{Username}:{pat.ToStr()}@github.com/{Username}/{Repository}.git" : string.Empty;
        }

        public string MetadataUrl => IsAvailable ? $"https://api.github.com/repos/{Username}/{Repository}/tags" : string.Empty;
    }
}