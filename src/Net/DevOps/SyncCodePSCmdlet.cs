using System.Management.Automation;
using System.Security;

namespace Skidbladnir.Net.DevOps
{
    [Cmdlet(VerbsData.Sync, "Code")]
    public class SyncCodePSCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string AzureUrl { get; set; }

        [Parameter(Mandatory = true)]
        public SecureString AzurePAT { get; set; }

        [Parameter(Mandatory = true)]
        public string GithubUrl { get; set; }

        [Parameter(Mandatory = true)]
        public SecureString GithubPAT { get; set; }

        [Parameter(Mandatory = false)]
        public string Branch { get; set; }

        [Parameter(Mandatory = false)]
        public int Retry { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            var azureRepo = new AzureRepositoryInfo
            {
                RepositoryUrl = this.AzureUrl
            };

            var githubRepo = new GithubRepositoryInfo
            {
                RepositoryUrl = this.GithubUrl
            };

            if (this.Retry < 1)
            {
                this.Retry = 5;
            }

            var exitInfo = SyncCode.SyncAzureToGithub(azureRepo, this.AzurePAT, githubRepo,
                this.GithubPAT, this.Branch, this.Retry).ConfigureAwait(false).GetAwaiter().GetResult();

            base.WriteObject(exitInfo);
        }
    }
}
