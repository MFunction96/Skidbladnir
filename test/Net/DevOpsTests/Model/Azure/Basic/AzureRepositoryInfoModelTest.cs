using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Security;
using Xanadu.Skidbladnir.Net.DevOps.Model.Azure.Basic;

namespace Xanadu.Skidbladnir.Test.Net.DevOpsTest.Model.Azure.Basic
{
    [TestClass]
    public class AzureRepositoryInfoModelTest
    {
        [TestMethod]
        public void RepositoryUrl_SetValidUrl_ParsesPropertiesCorrectly()
        {
            // Arrange
            var model = new AzureRepositoryInfoModel();
            const string expectedOrg = "MyOrg";
            const string expectedProject = "MyProject";
            const string expectedRepo = "MyRepo";

            // Act
            model.RepositoryUrl = $"https://dev.azure.com/{expectedOrg}/{expectedProject}/_git/{expectedRepo}";

            // Assert
            Assert.AreEqual(expectedOrg, model.Organization);
            Assert.AreEqual(expectedProject, model.Project);
            Assert.AreEqual(expectedRepo, model.Repository);
        }

        [TestMethod]
        [DataRow("https://dev.azure.com/MyOrg/MyProject/MyRepo")] // Missing _git
        [DataRow("https://example.com/MyOrg/MyProject/_git/MyRepo")]
        [DataRow("not a url")]
        [DataRow("")]
        [DataRow(null)]
        public void RepositoryUrl_SetInvalidUrl_DoesNotChangeProperties(string invalidUrl)
        {
            // Arrange
            var model = new AzureRepositoryInfoModel
            {
                Organization = "initialOrg",
                Project = "initialProject",
                Repository = "initialRepo",
                // Act
                RepositoryUrl = invalidUrl
            };

            // Assert
            Assert.AreEqual("initialOrg", model.Organization);
            Assert.AreEqual("initialProject", model.Project);
            Assert.AreEqual("initialRepo", model.Repository);
        }

        [TestMethod]
        public void RepositoryUrl_Get_ConstructsCorrectUrl()
        {
            // Arrange
            var model = new AzureRepositoryInfoModel
            {
                Organization = "MyOrg",
                Project = "MyProject",
                Repository = "MyRepo"
            };
            // Note: The getter generates a URL without the "/_git/" part.
            const string expectedUrl = "https://dev.azure.com/MyOrg/MyProject/MyRepo";

            // Act
            var actualUrl = model.RepositoryUrl;

            // Assert
            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [TestMethod]
        public void IsAvailable_WhenAllPropertiesSet_ReturnsTrue()
        {
            // Arrange
            var model = new AzureRepositoryInfoModel
            {
                Organization = "org",
                Project = "project",
                Repository = "repo"
            };

            // Assert
            Assert.IsTrue(model.IsAvailable);
        }

        [TestMethod]
        [DataRow("", "project", "repo")]
        [DataRow("org", "", "repo")]
        [DataRow("org", "project", "")]
        [DataRow("", "", "")]
        public void IsAvailable_WhenAnyPropertyMissing_ReturnsFalse(string org, string project, string repo)
        {
            // Arrange
            var model = new AzureRepositoryInfoModel
            {
                Organization = org,
                Project = project,
                Repository = repo
            };

            // Assert
            Assert.IsFalse(model.IsAvailable);
        }

        [TestMethod]
        public void GitOriginUrl_WhenAvailable_ReturnsCorrectUrl()
        {
            // Arrange
            var model = new AzureRepositoryInfoModel
            {
                Organization = "MyOrg",
                Project = "MyProject",
                Repository = "MyRepo"
            };
            var pat = new SecureString();
            "my-secret-pat".ToList().ForEach(pat.AppendChar);
            pat.MakeReadOnly();
            const string expectedUrl = "https://my-secret-pat@dev.azure.com/MyOrg/MyProject/_git/MyRepo";

            // Act
            var actualUrl = model.GitOriginUrl(pat);

            // Assert
            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [TestMethod]
        public void GitOriginUrl_WhenNotAvailable_ReturnsEmpty()
        {
            // Arrange
            var model = new AzureRepositoryInfoModel();
            var pat = new SecureString();

            // Act
            var actualUrl = model.GitOriginUrl(pat);

            // Assert
            Assert.AreEqual(string.Empty, actualUrl);
        }
    }
}
