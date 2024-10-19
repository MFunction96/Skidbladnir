using System;
using System.IO;

namespace Xanadu.Skidbladnir.IO.File.Cache
{
    public class CacheFile(CachePool pool, string fileName, string subFolder = "")
    {
        public string FileName => Path.GetFileName(this.FullPath);

        public string RelativePath => Path.GetRelativePath(this.Pool.BasePath, this.FullPath);

        public string FullPath { get; } = Path.Combine(pool.BasePath, subFolder, fileName);

        public bool Exists => System.IO.File.Exists(this.FullPath);

        public CachePool Pool => pool;

        public void Create()
        {
            if (this.Exists)
            {
                return;
            }

            System.IO.File.Create(this.FullPath).Close();
        }

        public void Delete()
        {
            if (!this.Exists)
            {
                return;
            }

            System.IO.File.Delete(this.FullPath);
        }

        public override bool Equals(object? obj)
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
