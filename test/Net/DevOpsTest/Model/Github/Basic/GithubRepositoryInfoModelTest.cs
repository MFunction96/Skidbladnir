using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Security;
using Xanadu.Skidbladnir.Net.DevOps.Model.Github.Basic;

namespace Xanadu.Skidbladnir.Test.Net.DevOpsTest.Model.Github.Basic
{
    [TestClass]
    public class GithubRepositoryInfoModelTest
    {
        [TestMethod]
        public void RepositoryUrl_SetValidUrl_ParsesOwnerAndRepository()
        {
            // Arrange
            var model = new GithubRepositoryInfoModel();
            const string expectedOwner = "microsoft";
            const string expectedRepo = "vscode";

            // Act
            model.RepositoryUrl = $"https://github.com/{expectedOwner}/{expectedRepo}";

            // Assert
            Assert.AreEqual(expectedOwner, model.Owner);
            Assert.AreEqual(expectedRepo, model.Repository);
        }

        [TestMethod]
        [DataRow("https://example.com/owner/repo")]
        [DataRow("not a url")]
        [DataRow("")]
        [DataRow(null)]
        public void RepositoryUrl_SetInvalidUrl_DoesNotChangeProperties(string invalidUrl)
        {
            // Arrange
            var model = new GithubRepositoryInfoModel
            {
                Owner = "initialOwner",
                Repository = "initialRepo",
                // Act
                RepositoryUrl = invalidUrl
            };

            // Assert
            Assert.AreEqual("initialOwner", model.Owner);
            Assert.AreEqual("initialRepo", model.Repository);
        }

        [TestMethod]
        public void RepositoryUrl_Get_ConstructsCorrectUrl()
        {
            // Arrange
            var model = new GithubRepositoryInfoModel
            {
                Owner = "octocat",
                Repository = "Hello-World"
            };
            const string expectedUrl = "https://github.com/octocat/Hello-World";

            // Act
            var actualUrl = model.RepositoryUrl;

            // Assert
            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [TestMethod]
        public void IsAvailable_WhenOwnerAndRepositorySet_ReturnsTrue()
        {
            // Arrange
            var model = new GithubRepositoryInfoModel
            {
                Owner = "owner",
                Repository = "repo"
            };

            // Assert
            Assert.IsTrue(model.IsAvailable);
        }

        [TestMethod]
        [DataRow("", "repo")]
        [DataRow("owner", "")]
        [DataRow("", "")]
        public void IsAvailable_WhenOwnerOrRepositoryMissing_ReturnsFalse(string owner, string repo)
        {
            // Arrange
            var model = new GithubRepositoryInfoModel
            {
                Owner = owner,
                Repository = repo
            };

            // Assert
            Assert.IsFalse(model.IsAvailable);
        }

        [TestMethod]
        public void GitOriginUrl_WhenAvailable_ReturnsCorrectUrl()
        {
            // Arrange
            var model = new GithubRepositoryInfoModel
            {
                Owner = "owner",
                Repository = "repo"
            };
            var pat = new SecureString();
            "my-secret-pat".ToList().ForEach(pat.AppendChar);
            pat.MakeReadOnly();
            const string expectedUrl = "https://owner:my-secret-pat@github.com/owner/repo.git";

            // Act
            var actualUrl = model.GitOriginUrl(pat);

            // Assert
            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [TestMethod]
        public void GitOriginUrl_WhenNotAvailable_ReturnsEmpty()
        {
            // Arrange
            var model = new GithubRepositoryInfoModel();
            var pat = new SecureString();

            // Act
            var actualUrl = model.GitOriginUrl(pat);

            // Assert
            Assert.AreEqual(string.Empty, actualUrl);
        }

        [TestMethod]
        public void ListReleasesApi_WhenAvailable_ReturnsCorrectUrl()
        {
            // Arrange
            var model = new GithubRepositoryInfoModel
            {
                Owner = "owner",
                Repository = "repo"
            };
            const string expectedUrl = "https://api.github.com/repos/owner/repo/releases";

            // Act
            var actualUrl = model.ListReleasesApi;

            // Assert
            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [TestMethod]
        public void ListReleasesApi_WhenNotAvailable_ReturnsEmpty()
        {
            // Arrange
            var model = new GithubRepositoryInfoModel();

            // Act
            var actualUrl = model.ListReleasesApi;

            // Assert
            Assert.AreEqual(string.Empty, actualUrl);
        }
    }
}
