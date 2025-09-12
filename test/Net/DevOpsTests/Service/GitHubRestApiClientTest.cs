using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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
            services.AddHttpClient("GitHubRestApiClient", httpClient =>
            {
                httpClient.BaseAddress = new Uri(GitHubRestApiClient.BaseUrl);
                httpClient.DefaultRequestVersion = HttpVersion.Version30;
                httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
                foreach (var gitHubDefaultHeader in GitHubRestApiClient.GitHubDefaultHeaders)
                {
                    httpClient.DefaultRequestHeaders.Add(gitHubDefaultHeader.Key, gitHubDefaultHeader.Value);
                }

            }).ConfigurePrimaryHttpMessageHandler(handler => new HttpClientHandler
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.All
            });
            services.AddScoped<GitHubRestApiClient>();
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
            var releaseAssets = await gitHubRestApiClient.ListAssets(gitHubRepositoryInfoModel, 246552604);
            // Assert
            Assert.IsNotNull(releaseAssets);
            Assert.IsGreaterThan(0, releaseAssets.ToArray().Length);
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
            Assert.IsGreaterThan(0, releases.ToArray().Length);
            this.TestContext.WriteLine(JsonSerializer.Serialize(releases, this._jsonSerializerOptions));
        }
    }
}
