using System.Security;
using System.Text.RegularExpressions;
using Xanadu.Skidbladnir.Core.Extension;

namespace Xanadu.Skidbladnir.Net.DevOps.Azure
{
    public class AzureRepositoryInfo : RepositoryInfo
    {
        public string Organization { get; set; }

        public string Project { get; set; }

        public static readonly Regex RepositoryRegex = new(@"^https:\/\/dev\.azure\.com\/(?<Organization>(.+?))\/(?<Project>(.+?))\/_git\/(?<Repository>(.+?))$", RegexOptions.ExplicitCapture);

        public override string RepositoryUrl
        {
            get
            {
                return IsAvailable ? $"https://dev.azure.com/{Organization}/{Project}/{Repository}" : string.Empty;
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

        public override bool IsAvailable => !string.IsNullOrEmpty(Organization) &&
                                            !string.IsNullOrEmpty(Project) &&
                                            !string.IsNullOrEmpty(Repository);

        /// <summary>
        /// https://learn.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=Windows#use-a-pat
        /// </summary>
        /// <param name="pat"></param>
        /// <returns></returns>
        public override string OriginUrl(SecureString pat)
        {
            return IsAvailable ? $"https://{pat.ToStr()}@dev.azure.com/{Organization}/{Project}/_git/{Repository}" : string.Empty;
        }
    }
}