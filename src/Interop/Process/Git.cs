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
        /// <param name="url"></param>
        /// <param name="branch"></param>
        /// <param name="single"></param>
        /// <param name="destination"></param>
        /// <param name="workFolder"></param>
        /// <param name="args"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<ProcessExitInfo> Clone(string url, string branch = "", bool single = false, string destination = "", string workFolder = "", string[]? args = null, CancellationToken cancellationToken = default)
        {
            var info = new ProcessStartInfo
            {
                FileName = "git.exe",
                StandardErrorEncoding = Encoding.UTF8,
                StandardInputEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
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
        /// <param name="workFolder"></param>
        /// <param name="args"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<ProcessExitInfo> Push(string workFolder = "", string[]? args = null, CancellationToken cancellationToken = default)
        {
            var info = new ProcessStartInfo
            {
                FileName = "git.exe",
                StandardErrorEncoding = Encoding.UTF8,
                StandardInputEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
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