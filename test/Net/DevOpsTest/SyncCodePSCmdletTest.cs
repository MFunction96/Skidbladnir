using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Skidbladnir.Interop.Extension;
using Skidbladnir.Net.DevOps;

namespace Skidbladnir.Test.Net.DevOpsTest
{
    [TestClass]
    public class SyncCodePSCmdletTest
    {
        private static readonly object SyncRoot = new();

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Monitor.Enter(SyncCodePSCmdletTest.SyncRoot);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Monitor.Exit(SyncCodePSCmdletTest.SyncRoot);
        }

        [TestMethod]
        public void SyncCodePs()
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

            using var ps = PowerShell.Create()
                .AddCommand("Import-Module").AddArgument(Assembly.GetAssembly(typeof(SyncCodePSCmdlet))!.Location);
            ps.Invoke();

            ps.AddCommand("Sync-Code")
                .AddParameter("AzureUrl", config["Azure:RepositoryUrl"]!)
                .AddParameter("AzurePAT", config["Azure:PAT"]!.ToSecureString())
                .AddParameter("GithubUrl", config["Github:RepositoryUrl"]!)
                .AddParameter("GithubPAT", config["Github:PAT"]!.ToSecureString());

            var objs = ps.Invoke();
            foreach (var obj in objs)
            {
                this.TestContext.WriteLine(obj.ToString());
            }

        }
    }
}
