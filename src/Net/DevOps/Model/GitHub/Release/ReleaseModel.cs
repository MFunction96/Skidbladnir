using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Xanadu.Skidbladnir.Net.DevOps.Model.GitHub.User;

namespace Xanadu.Skidbladnir.Net.DevOps.Model.GitHub.Release
{
    public class ReleaseModel
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = string.Empty;

        [JsonPropertyName("assets_url")]
        public string AssetsUrl { get; set; } = string.Empty;

        [JsonPropertyName("upload_url")]
        public string UploadUrl { get; set; } = string.Empty;

        [JsonPropertyName("tarball_url")]
        public string TarballUrl { get; set; } = string.Empty;

        [JsonPropertyName("zipball_url")]
        public string ZipballUrl { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; set; } = string.Empty;

        [JsonPropertyName("tag_name")]
        public string TagName { get; set; } = string.Empty;

        [JsonPropertyName("target_commitish")]
        public string TargetCommitish { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;

        [JsonPropertyName("draft")]
        public bool Draft { get; set; }

        [JsonPropertyName("prerelease")]
        public bool Prerelease { get; set; }

        [JsonPropertyName("immutable")]
        public bool Immutable { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.MinValue;

        [JsonPropertyName("published_at")]
        public DateTimeOffset PublishedAt { get; set; } = DateTimeOffset.MinValue;

        [JsonPropertyName("author")]
        public AuthorModel? Author { get; set; }

        [JsonPropertyName("assets")]
        public ICollection<ReleaseAssetModel> Assets { get; set; } = new List<ReleaseAssetModel>();
    }
}
