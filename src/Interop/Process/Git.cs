using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xanadu.Skidbladnir.Interop.Process
{
    public static class Git
    {
        public static void EnableGitTerminalPrompt()
        {
            Environment.SetEnvironmentVariable("GIT_TERMINAL_PROMPT", "1", EnvironmentVariableTarget.Process);
        }

        public static async Task<ProcessExitInfo> Clone(string url, string branch = "", bool single = false, string destination = "", string workFolder = "", string[] args = null, CancellationToken cancellationToken = default)
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

            using var process = new System.Diagnostics.Process
            {
                StartInfo = info,
            };

            process.Start();
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync(cancellationToken);
            return new ProcessExitInfo
            {
                ExitCode = process.ExitCode,
                Output = await outputTask,
                Error = await errorTask
            };
        }

        public static async Task<ProcessExitInfo> Push(string workFolder = "", string[] args = null, CancellationToken cancellationToken = default)
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

            using var process = new System.Diagnostics.Process
            {
                StartInfo = info,
            };

            process.Start();
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();
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