using Microsoft.Extensions.Configuration;
using Skidbladnir.Net.DevOps.Azure;

namespace Skidbladnir.Test.Net.DevOpsTest.Azure
{
    [TestClass]
    public class AzureArtifactsFeedsTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void FeedsUrlTest()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .Build();
            var feedsUrl = config["Azure:FeedsUrl"]!;
            var feeds = new AzureArtifactsFeeds
            {
                FeedsUrl = feedsUrl
            };

            Assert.AreEqual(feedsUrl, feeds.FeedsUrl);
        }
    }
}
