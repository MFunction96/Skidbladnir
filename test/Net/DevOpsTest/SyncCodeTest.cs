using Microsoft.Extensions.Configuration;
using Skidbladnir.Interop.Process;
using Skidbladnir.Net.DevOps;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Skidbladnir.Interop.Extension;

namespace Skidbladnir.Test.Net.DevOpsTest
{
    [TestClass]
    public class SyncCodeTest
    {
        private static readonly object SyncRoot = new();

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
            var azurePAT = config["Azure:PAT"]!.ToSecureString();
            var githubPAT = config["Github:PAT"]!.ToSecureString();
            var azureRepo = new AzureRepositoryInfo
            {
                RepositoryUrl = config["Azure:RepositoryUrl"]!
            };
            var githubRepo = new GithubRepositoryInfo
            {
                RepositoryUrl = config["Github:RepositoryUrl"]!
            };

            var result = await SyncCode.SyncAzureToGithub(azureRepo, azurePAT, githubRepo, githubPAT, "main");
            this.TestContext.WriteLine(result.Output);
            this.TestContext.WriteLine(result.Error);
            Assert.AreEqual(0, result.ExitCode);
        }
    }
}