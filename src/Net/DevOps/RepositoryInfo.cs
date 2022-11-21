using System.Security;

namespace Skidbladnir.Net.DevOps
{
    public abstract class RepositoryInfo
    {
        public string Repository { get; set; }

        public abstract string RepositoryUrl { get; set; }

        public abstract string OriginUrl(SecureString pat);
    }
}