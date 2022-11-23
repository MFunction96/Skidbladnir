using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Skidbladnir.Interop.Extension;
using Skidbladnir.Interop.Process;
using Skidbladnir.Net.DevOps;

namespace Skidbladnir.Test.Net.DevOpsTest
{
    [TestClass]
    public class SyncCodeTest
    {
        private static readonly object SyncRoot = new ();

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Monitor.Enter(SyncCodeTest.SyncRoot);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Monitor.Exit(SyncCodeTest.SyncRoot);
        }

        [TestMethod]
        public async Task SyncAzureToGithubTest()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddUserSecrets<SyncCodeTest>()
                .Build();
            var azurePAT = config["Azure:PAT"]!;
            var githubPAT = config["Github:PAT"]!;
            var azureRepo = new AzureRepositoryInfo
            {
                RepositoryUrl = config["Azure:RepositoryUrl"]!
            };
            var githubRepo = new GithubRepositoryInfo
            {
                RepositoryUrl = config["Github:RepositoryUrl"]!
            };

            var flag = true;
            Git.EnableGitTerminalPrompt();
            var tmpFolder = Path.GetTempPath();
            for (var i = 0; i < 10; ++i)
            {
                if (Directory.Exists(Path.Combine(tmpFolder, azureRepo.Repository)))
                {
                    Directory.Delete(Path.Combine(tmpFolder, azureRepo.Repository), true);
                }

                var clone = await Git.Clone(azureRepo.OriginUrl(azurePAT), "main", true, azureRepo.Repository, tmpFolder);
                if (clone.ExitCode == 0)
                {
                    flag = false;
                    this.TestContext.WriteLine(clone.Output);
                    break;
                }
                this.TestContext.WriteLine(clone.Error);
            }

            if (flag)
            {
                Assert.Fail();
            }

            flag = true;
            for (var i = 0; i < 10; i++)
            {
                var push = await Git.Push(Path.Combine(tmpFolder, azureRepo.Repository),
                    new[] { githubRepo.OriginUrl(githubPAT) });
                if (push.ExitCode == 0)
                {
                    flag = false;
                    this.TestContext.WriteLine(push.Output);
                    break;
                }
                this.TestContext.WriteLine(push.Error);
            }

            Thread.Sleep(1000);
            try
            {
                Directory.Delete(Path.Combine(tmpFolder, azureRepo.Repository), true);
            }
            catch (Exception e)
            {
                this.TestContext.WriteLine(e.Message);
            }
            
            if (flag)
            {
                Assert.Fail();
            }
            
        }
    }
}