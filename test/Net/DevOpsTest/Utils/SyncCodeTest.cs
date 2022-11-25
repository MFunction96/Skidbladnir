using Microsoft.Extensions.Configuration;
using Skidbladnir.Interop.Extension;
using Skidbladnir.Net.DevOps.Azure;
using Skidbladnir.Net.DevOps.Github;
using Skidbladnir.Net.DevOps.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace Skidbladnir.Test.Net.DevOpsTest.Utils
{
    [TestClass]
    public class SyncCodeTest
    {
        private static readonly object SyncRoot = new();

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Monitor.Enter(SyncRoot);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Monitor.Exit(SyncRoot);
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
            TestContext.WriteLine(result.Output);
            TestContext.WriteLine(result.Error);
            Assert.AreEqual(0, result.ExitCode);
        }
    }
}