﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Xanadu.Skidbladnir.Interop.Process;
using Xanadu.Skidbladnir.IO.File;
using Xanadu.Skidbladnir.Net.DevOps.Azure;
using Xanadu.Skidbladnir.Net.DevOps.Github;

namespace Xanadu.Skidbladnir.Net.DevOps.Utils
{
    public static class SyncCode
    {
        public static async Task<ProcessExitInfo> SyncAzureToGithub(AzureRepositoryInfo azureRepository, SecureString azurePAT,
            GithubRepositoryInfo githubRepository, SecureString githubPAT, string branch = "", int retry = 5, CancellationToken cancellationToken = default)
        {
            if (!azureRepository.IsAvailable)
            {
                throw new ArgumentException(string.Empty, nameof(azureRepository));
            }

            if (!githubRepository.IsAvailable)
            {
                throw new ArgumentException(string.Empty, nameof(githubRepository));
            }

            var cloneUrl = azureRepository.OriginUrl(azurePAT);
            var remoteUrl = githubRepository.OriginUrl(githubPAT);
            var tmpFolder = Path.Combine(Path.GetTempPath(), azureRepository.Repository);
            Git.EnableGitTerminalPrompt();
            var flag = true;
            var exitInfo = new ProcessExitInfo();
            for (var i = 0; i < retry; ++i)
            {
                await Deletion.DeleteDirectory(tmpFolder, true, true);
                exitInfo.Append(await Git.Clone(cloneUrl, branch, true, string.Empty, Path.GetTempPath(), null, cancellationToken));
                if (exitInfo.ExitCode != 0)
                {
                    continue;
                }

                flag = false;
                break;

            }

            if (flag)
            {
                throw new ExternalException(exitInfo.Error, exitInfo.ExitCode);
            }

            flag = true;
            for (var i = 0; i < retry; ++i)
            {
                exitInfo.Append(await Git.Push(tmpFolder, new[] { remoteUrl }, cancellationToken));
                if (exitInfo.ExitCode != 0)
                {
                    continue;
                }

                flag = false;
                break;

            }

            if (flag)
            {
                throw new ExternalException(exitInfo.Error, exitInfo.ExitCode);
            }

            await Deletion.DeleteDirectory(tmpFolder, true, true);
            return exitInfo;
        }
    }
}
