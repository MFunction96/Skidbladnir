using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xanadu.Skidbladnir.Net.DevOps;
using Xanadu.Skidbladnir.Net.DevOps.Model.GitHub.Basic;
using Xanadu.Skidbladnir.Net.DevOps.Service;

namespace Xanadu.Skidbladnir.Test.Net.DevOps.Service
{
    [TestClass]
    public class GitHubRestApiClientTest
    {
        private IServiceProvider _provider = null!;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public TestContext TestContext { get; set; } = null!;

        [TestInitialize]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddGitHubRestApiClient();
            this._provider = services.BuildServiceProvider();
        }

        [TestCleanup]
        public void Cleanup()
        {

        }

        [TestMethod]
        public async Task GitHubRestApiClient_Creation_Success()
        {
            // Arrange
            using var scope = this._provider.CreateScope();
            var gitHubRestApiClient = scope.ServiceProvider.GetRequiredService<GitHubRestApiClient>();
            var gitHubRepositoryInfoModel = new GitHubRepositoryInfoModel
            {
                Owner = "DDFantasyV",
                Repository = "Korabli_localization_chs"
            };
            // Act
            var releaseAssets = await gitHubRestApiClient.ListAssets(gitHubRepositoryInfoModel, 232881885);
            // Assert
            Assert.IsNotNull(releaseAssets);
            Assert.IsGreaterThan(0, releaseAssets.Length);
            this.TestContext.WriteLine(JsonSerializer.Serialize(releaseAssets, this._jsonSerializerOptions));
        }

        [TestMethod]
        public async Task GitHubRestApiClient_ListReleases_Success()
        {
            // Arrange
            using var scope = this._provider.CreateScope();
            var gitHubRestApiClient = scope.ServiceProvider.GetRequiredService<GitHubRestApiClient>();
            var gitHubRepositoryInfoModel = new GitHubRepositoryInfoModel
            {
                Owner = "DDFantasyV",
                Repository = "Korabli_localization_chs"
            };
            // Act
            var releases = await gitHubRestApiClient.ListReleases(gitHubRepositoryInfoModel);
            // Assert
            Assert.IsNotNull(releases);
            Assert.IsGreaterThan(0, releases.Length);
            this.TestContext.WriteLine(JsonSerializer.Serialize(releases, this._jsonSerializerOptions));
        }

        [TestMethod]
        public void DefaultHttpClient_ShouldReturnConfiguredClient()
        {
            // Arrange
            var handlerConfigured = false;
            var clientConfigured = false;

            // Act
            var httpClient = GitHubRestApiClient.DefaultHttpClient(
                handlerConfigure: handler =>
                {
                    // This is a sample configuration to verify the action is executed.
                    // The default is true, so we set it to false for the test.
                    handler.AllowAutoRedirect = false;
                    handlerConfigured = true;
                },
                clientConfigure: client =>
                {
                    // This is a sample configuration to verify the action is executed.
                    client.Timeout = TimeSpan.FromSeconds(60);
                    clientConfigured = true;
                });

            // Assert
            Assert.IsNotNull(httpClient);
            Assert.AreEqual(GitHubRestApiClient.BaseUrl, httpClient.BaseAddress);
            Assert.IsTrue(handlerConfigured, "Handler configuration action was not called.");
            Assert.IsTrue(clientConfigured, "Client configuration action was not called.");
            Assert.AreEqual(TimeSpan.FromSeconds(60), httpClient.Timeout);

            // Verify default headers
            foreach (var header in GitHubRestApiClient.GitHubDefaultHeaders)
            {
                Assert.IsTrue(httpClient.DefaultRequestHeaders.Contains(header.Key));
                CollectionAssert.Contains(httpClient.DefaultRequestHeaders.GetValues(header.Key).ToList(), header.Value);
            }
        }
    }
}
