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
                return $"https://dev.azure.com/{this.Organization}/{this.Project}/{base.Repository}";
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

        /// <summary>
        /// https://learn.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=Windows#use-a-pat
        /// </summary>
        /// <param name="pat"></param>
        /// <returns></returns>
        public override string OriginUrl(SecureString pat)
        {
            return $"https://{pat}@dev.azure.com/{this.Organization}/{this.Project}/_git/{base.Repository}";
        }
    }
}