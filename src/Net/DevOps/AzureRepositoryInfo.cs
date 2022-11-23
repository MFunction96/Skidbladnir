using System.Security;
using System.Text.RegularExpressions;

namespace Skidbladnir.Net.DevOps
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
                return this.IsAvailable ? $"https://dev.azure.com/{this.Organization}/{this.Project}/{base.Repository}" : string.Empty;
            }
            set
            {
                if (!AzureRepositoryInfo.RepositoryRegex.IsMatch(value))
                {
                    return;
                }
                var match = AzureRepositoryInfo.RepositoryRegex.Match(value);
                this.Organization = match.Groups["Organization"].Value;
                this.Project = match.Groups["Project"].Value;
                base.Repository = match.Groups["Repository"].Value;
            }
        }

        public override bool IsAvailable => !string.IsNullOrEmpty(this.Organization) &&
                                            !string.IsNullOrEmpty(this.Project) &&
                                            !string.IsNullOrEmpty(base.Repository);

        /// <summary>
        /// https://learn.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=Windows#use-a-pat
        /// </summary>
        /// <param name="pat"></param>
        /// <returns></returns>
        public override string OriginUrl(string pat)
        {
            return this.IsAvailable ? $"https://{pat}@dev.azure.com/{this.Organization}/{this.Project}/_git/{base.Repository}" : string.Empty;
        }
    }
}