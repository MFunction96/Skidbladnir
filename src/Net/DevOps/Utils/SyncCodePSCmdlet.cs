using System.Management.Automation;
using System.Security;
using Xanadu.Skidbladnir.Interop.Process;
using Xanadu.Skidbladnir.Net.DevOps.Azure;
using Xanadu.Skidbladnir.Net.DevOps.Github;

namespace Xanadu.Skidbladnir.Net.DevOps.Utils
{
    [Cmdlet(VerbsData.Sync, "Code")]
    [OutputType(typeof(ProcessExitInfo))]
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

            WriteObject(exitInfo);
        }
    }
}
