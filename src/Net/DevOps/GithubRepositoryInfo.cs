using System.Security;
using System.Text.RegularExpressions;
using Skidbladnir.Interop.Extension;

namespace Skidbladnir.Net.DevOps
{
    public class GithubRepositoryInfo : RepositoryInfo
    {
        public string Username { get; set; }

        public static readonly Regex RepositoryRegex = new(@"^https:\/\/github\.com\/(?<Username>(.+?))\/(?<Repository>(.+?))$", RegexOptions.ExplicitCapture);

        public override string RepositoryUrl
        {
            get
            {
                return this.IsAvailable ? $"https://github.com/{this.Username}/{base.Repository}" : string.Empty;
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

        public override bool IsAvailable => !string.IsNullOrEmpty(this.Username) && !string.IsNullOrEmpty(base.Repository);

        public override string OriginUrl(SecureString pat)
        {
            return this.IsAvailable ? $"https://{this.Username}:{pat.ToStr()}@github.com/{this.Username}/{base.Repository}.git" : string.Empty;
        }

        public string MetadataUrl => this.IsAvailable ? $"https://api.github.com/repos/{this.Username}/{base.Repository}/tags" : string.Empty;
    }
}