using System;
using System.IO;

namespace Xanadu.Skidbladnir.IO.File.Cache
{
    public class CacheFile(CachePool pool, string fileName, string subFolder = "")
    {
        public string FileName => Path.GetFileName(this.FullPath);

        public string RelativePath => Path.GetRelativePath(this.Pool.BasePath, this.FullPath);

        public string FullPath { get; } = Path.Combine(pool.BasePath, subFolder, fileName);

        public CachePool Pool => pool;

        public override bool Equals(object obj)
        {
            if (obj is not CacheFile cacheFile)
            {
                return false;
            }
            
            return this.Pool.Equals(cacheFile.Pool) && this.FullPath.Equals(cacheFile.FullPath);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Pool, this.FullPath);
        }
    }
}
