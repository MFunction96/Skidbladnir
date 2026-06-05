using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xanadu.Skidbladnir.Interop.Process
{
    /// <summary>
    /// Git command line.
    /// </summary>
    public static class Git
    {
        /// <summary>
        /// Enable git terminal prompt.
        /// </summary>
        public static void EnableGitTerminalPrompt()
        {
            Environment.SetEnvironmentVariable("GIT_TERMINAL_PROMPT", "1", EnvironmentVariableTarget.Process);
        }

        /// <summary>
        /// Clone a repository.
        /// </summary>
        /// <param name="url">The URL of the repository to clone.</param>
        /// <param name="branch">The branch to clone.</param>
        /// <param name="single">Whether to clone a single branch.</param>
        /// <param name="destination">The destination folder for the clone.</param>
        /// <param name="workFolder">The working directory for the git command.</param>
        /// <param name="args">Additional arguments for the git command.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the process exit information.</returns>
        public static async Task<ProcessExitInfo> Clone(string url, string branch = "", bool single = false, string destination = "", string workFolder = "", string[]? args = null, CancellationToken cancellationToken = default)
        {
            var info = new ProcessStartInfo
            {
                FileName = "git.exe",
                StandardErrorEncoding = new UTF8Encoding(false),
                StandardInputEncoding = new UTF8Encoding(false),
                StandardOutputEncoding = new UTF8Encoding(false),
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                ArgumentList =
                {
                    "clone",
                    url
                },
                CreateNoWindow = true,
                UseShellExecute = false
            };

            if (!string.IsNullOrEmpty(destination))
            {
                info.ArgumentList.Add(destination);
            }

            if (!string.IsNullOrEmpty(branch))
            {
                info.ArgumentList.Add("--branch");
                info.ArgumentList.Add(branch);
            }

            if (!string.IsNullOrEmpty(workFolder))
            {
                info.WorkingDirectory = workFolder;
            }

            if (single)
            {
                info.ArgumentList.Add("--single-branch");
            }

            if (args != null)
            {
                foreach (var arg in args)
                {
                    info.ArgumentList.Add(arg);
                }
            }

            using var process = new System.Diagnostics.Process();
            process.StartInfo = info;

            process.Start();
            var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);
            return new ProcessExitInfo
            {
                ExitCode = process.ExitCode,
                Output = await outputTask,
                Error = await errorTask
            };
        }

        /// <summary>
        /// Push changes to remote.
        /// </summary>
        /// <param name="workFolder">The working directory for the git command.</param>
        /// <param name="args">Additional arguments for the git command.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the process exit information.</returns>
        public static async Task<ProcessExitInfo> Push(string workFolder = "", string[]? args = null, CancellationToken cancellationToken = default)
        {
            var info = new ProcessStartInfo
            {
                FileName = "git.exe",
                StandardErrorEncoding = new UTF8Encoding(false),
                StandardInputEncoding = new UTF8Encoding(false),
                StandardOutputEncoding = new UTF8Encoding(false),
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                ArgumentList =
                {
                    "push"
                },
                CreateNoWindow = true,
                UseShellExecute = false
            };

            if (!string.IsNullOrEmpty(workFolder))
            {
                info.WorkingDirectory = workFolder;
            }

            if (args != null)
            {
                foreach (var arg in args)
                {
                    info.ArgumentList.Add(arg);
                }
            }

            using var process = new System.Diagnostics.Process();
            process.StartInfo = info;

            process.Start();
            var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);
            return new ProcessExitInfo
            {
                ExitCode = process.ExitCode,
                Output = await outputTask,
                Error = await errorTask
            };
        }
    }
}