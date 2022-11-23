using Skidbladnir.Interop.Process;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace Skidbladnir.Net.DevOps
{
    public class SyncCode
    {
        public static async Task<ProcessExitInfo> SyncAzureToGithub(AzureRepositoryInfo azureRepository, string azurePAT,
            GithubRepositoryInfo githubRepository, string githubPAT, string branch = "", CancellationToken cancellationToken = default)
        {
            var cloneUrl = azureRepository.OriginUrl(azurePAT);
            var remoteUrl = githubRepository.OriginUrl(githubPAT);
            var tmpFolder = Path.Combine(Path.GetTempPath(), azureRepository.Repository);
            var exitInfo = await Git.Clone(cloneUrl, branch, true, tmpFolder, string.Empty,null, cancellationToken);
            if (exitInfo.ExitCode != 0)
            {
                throw new ExternalException(exitInfo.Error, exitInfo.ExitCode);
            }

            exitInfo.Append(await Git.Push(tmpFolder, new []{remoteUrl}, cancellationToken));
            return exitInfo;
        }
    }
}
