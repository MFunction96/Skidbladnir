using System.Security;

namespace Xanadu.Skidbladnir.Net.DevOps
{
    public abstract class RepositoryInfo
    {
        public string Repository { get; set; }

        public abstract string RepositoryUrl { get; set; }

        public abstract string OriginUrl(SecureString pat);

        public abstract bool IsAvailable { get; }
    }
}