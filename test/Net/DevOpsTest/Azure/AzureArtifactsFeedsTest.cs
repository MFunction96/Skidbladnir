using Xanadu.Skidbladnir.Net.DevOps.Azure;

namespace Xanadu.Skidbladnir.Test.Net.DevOpsTest.Azure
{
    [TestClass]
    public class AzureArtifactsFeedsTest
    {
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void FeedsUrlTest()
        {
            var feedsUrl = "https://dev.azure.com/XanaCN/Lyoko/_artifacts/feed/Subdigitals";
            var feeds = new AzureArtifactsFeeds
            {
                FeedsUrl = feedsUrl
            };

            Assert.AreEqual(feedsUrl, feeds.FeedsUrl);
        }
    }
}
