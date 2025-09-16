using Xanadu.Skidbladnir.Net.DevOps.Model.Azure.Basic;

namespace Xanadu.Skidbladnir.Test.Net.DevOps.Model.Azure.Basic
{
    [TestClass]
    public class AzureArtifactsFeedModelTest
    {
        [TestMethod]
        [DataRow("https://dev.azure.com/MyOrg/_artifacts/feed/MyFeed", "MyOrg", "", "MyFeed")]
        [DataRow("https://dev.azure.com/MyOrg/MyProject/_artifacts/feed/MyFeed", "MyOrg", "MyProject", "MyFeed")]
        public void FeedsUrl_Set_ShouldParseCorrectly(string url, string expectedOrg, string expectedProject, string expectedFeed)
        {
            // Arrange
            var model = new AzureArtifactsFeedModel();

            // Act
            model.FeedsUrl = url;

            // Assert
            Assert.AreEqual(expectedOrg, model.Organization);
            Assert.AreEqual(expectedProject, model.Project);
            Assert.AreEqual(expectedFeed, model.FeedsName);
        }

        [TestMethod]
        [DataRow("invalid-url")]
        [DataRow("https://example.com")]
        public void FeedsUrl_Set_WithInvalidUrl_ShouldNotChangeProperties(string url)
        {
            // Arrange
            var model = new AzureArtifactsFeedModel
            {
                Organization = "InitialOrg",
                Project = "InitialProject",
                FeedsName = "InitialFeed"
            };

            // Act
            model.FeedsUrl = url;

            // Assert
            Assert.AreEqual("InitialOrg", model.Organization);
            Assert.AreEqual("InitialProject", model.Project);
            Assert.AreEqual("InitialFeed", model.FeedsName);
        }

        [TestMethod]
        public void FeedsUrl_Get_WithProject_ShouldReturnProjectUrl()
        {
            // Arrange
            var model = new AzureArtifactsFeedModel
            {
                Organization = "MyOrg",
                Project = "MyProject",
                FeedsName = "MyFeed"
            };
            var expectedUrl = "https://dev.azure.com/MyOrg/MyProject/_artifacts/feed/MyFeed";

            // Act
            var actualUrl = model.FeedsUrl;

            // Assert
            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [TestMethod]
        public void FeedsUrl_Get_WithoutProject_ShouldReturnOrganizationUrl()
        {
            // Arrange
            var model = new AzureArtifactsFeedModel
            {
                Organization = "MyOrg",
                FeedsName = "MyFeed"
            };
            var expectedUrl = "https://dev.azure.com/MyOrg/_artifacts/feed/MyFeed";

            // Act
            var actualUrl = model.FeedsUrl;

            // Assert
            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [TestMethod]
        public void OriginUrl_Get_WithProject_ShouldReturnProjectUrl()
        {
            // Arrange
            var model = new AzureArtifactsFeedModel
            {
                Organization = "MyOrg",
                Project = "MyProject",
                FeedsName = "MyFeed"
            };
            var expectedUrl = "https://pkgs.dev.azure.com/MyOrg/MyProject/_packaging/MyFeed/nuget/v3/index.json";

            // Act
            var actualUrl = model.OriginUrl;

            // Assert
            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [TestMethod]
        public void OriginUrl_Get_WithoutProject_ShouldReturnOrganizationUrl()
        {
            // Arrange
            var model = new AzureArtifactsFeedModel
            {
                Organization = "MyOrg",
                FeedsName = "MyFeed"
            };
            var expectedUrl = "https://pkgs.dev.azure.com/MyOrg/_packaging/MyFeed/nuget/v3/index.json";

            // Act
            var actualUrl = model.OriginUrl;

            // Assert
            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [TestMethod]
        [DataRow("MyOrg", "MyFeed", true)]
        [DataRow("MyOrg", "", false)]
        [DataRow("", "MyFeed", false)]
        [DataRow("", "", false)]
        [DataRow(" ", "MyFeed", false)]
        [DataRow("MyOrg", " ", false)]
        public void IsAvailable_ShouldReturnCorrectState(string organization, string feedName, bool expected)
        {
            // Arrange
            var model = new AzureArtifactsFeedModel
            {
                Organization = organization,
                FeedsName = feedName
            };

            // Act & Assert
            Assert.AreEqual(expected, model.IsAvailable);
        }

        [TestMethod]
        public void Urls_Get_WhenNotAvailable_ShouldReturnEmpty()
        {
            // Arrange
            var model = new AzureArtifactsFeedModel();

            // Act & Assert
            Assert.IsFalse(model.IsAvailable);
            Assert.AreEqual(string.Empty, model.FeedsUrl);
            Assert.AreEqual(string.Empty, model.OriginUrl);
        }
    }
}