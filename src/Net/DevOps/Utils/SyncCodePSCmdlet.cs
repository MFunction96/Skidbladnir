using Skidbladnir.Net.DevOps.Azure;
using Skidbladnir.Net.DevOps.Github;
using System.Management.Automation;
using System.Security;

namespace Skidbladnir.Net.DevOps.Utils
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
                RepositoryUrl = AzureUrl
            };

            var githubRepo = new GithubRepositoryInfo
            {
                RepositoryUrl = GithubUrl
            };

            if (Retry < 1)
            {
                Retry = 5;
            }

            var exitInfo = SyncCode.SyncAzureToGithub(azureRepo, AzurePAT, githubRepo,
                GithubPAT, Branch, Retry).ConfigureAwait(false).GetAwaiter().GetResult();

            WriteObject(exitInfo);
        }
    }
}
