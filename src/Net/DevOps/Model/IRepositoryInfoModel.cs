using System.Security;

namespace Xanadu.Skidbladnir.Net.DevOps.Model
{
    /// <summary>
    /// Represents information about a source code repository, including its URL and availability.
    /// </summary>
    /// <remarks>This abstract class provides a base for repository-related operations, such as retrieving the
    /// repository's URL and determining its availability. Derived classes must implement the abstract members to
    /// provide specific repository details and behavior.</remarks>
    public interface IRepositoryInfoModel
    {
        /// <summary>
        /// Represents the name of the repository.
        /// </summary>
        public string Repository { get; set; }

        /// <summary>
        /// Gets or sets the URL of the repository associated with this instance.
        /// </summary>
        public string RepositoryUrl { get; set; }

        /// <summary>
        /// Git origin URL with Personal Access Token (PAT) for authentication.
        /// </summary>
        /// <param name="pat">PAT</param>
        /// <returns>Url with PAT</returns>
        public string GitOriginUrl(SecureString pat);

        /// <summary>
        /// Repository is available.
        /// </summary>
        public bool IsAvailable { get; }
    }
}