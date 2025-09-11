using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xanadu.Skidbladnir.Net.DevOps.Model.Azure.Basic;

namespace Xanadu.Skidbladnir.Test.Net.DevOpsTest.Model.Azure.Basic
{
    [TestClass]
    public class AzureArtifactsFeedModelTest
    {
        [TestMethod]
        public void FeedsUrlTest()
        {
            const string feedsUrl = "https://dev.azure.com/XanaCN/Lyoko/_artifacts/feed/Subdigitals";
            var feeds = new AzureArtifactsFeedModel
            {
                FeedsUrl = feedsUrl
            };

            Assert.AreEqual(feedsUrl, feeds.FeedsUrl);
        }
    }
}
