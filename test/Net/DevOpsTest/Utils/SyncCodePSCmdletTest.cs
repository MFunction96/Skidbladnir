﻿using Microsoft.Extensions.Configuration;
using Xanadu.Skidbladnir.Core.Extension;
using Xanadu.Skidbladnir.Net.DevOps.Utils;
using System.Management.Automation;
using System.Reflection;
using System.Threading;

namespace Xanadu.Skidbladnir.Test.Net.DevOpsTest.Utils
{
    [TestClass]
    public class SyncCodePSCmdletTest
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

        //[TestMethod]
        public void SyncCodePs()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddUserSecrets<SyncCodeTest>()
                .Build();

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
                TestContext.WriteLine(obj.ToString());
            }

        }
    }
}
