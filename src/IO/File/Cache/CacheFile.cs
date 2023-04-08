using System;
using System.IO;

namespace Skidbladnir.IO.File.Cache
{
    public class CacheFile
    {
        public string FileName => Path.GetFileName(this.FullPath);

        public string RelativePath => Path.GetRelativePath(this.Pool.BasePath, this.FullPath);

        public string FullPath { get; }

        public readonly CachePool Pool;

        public CacheFile(CachePool pool, string fileName, string subFolder = "")
        {
            this.Pool = pool;
            this.FullPath = Path.Combine(pool.BasePath, subFolder, fileName);
        }

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
