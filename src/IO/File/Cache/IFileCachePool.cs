using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xanadu.Skidbladnir.IO.File.Cache
{
    /// <summary>
    /// File cache pool which used to manage file cache. Support Dependency Injection.
    /// </summary>
    public interface IFileCachePool : IDisposable
    {
        /// <summary>
        /// The base path of the file cache pool. Default is the temp path of the current process.
        /// </summary>
        public string BasePath { get; }

        /// <summary>
        /// The current cache files in the pool.
        /// </summary>
        public ICollection<FileCache> CacheFiles { get; }

        /// <summary>
        /// Set custom base path for the file cache pool.
        /// </summary>
        /// <param name="basePath">The base path of the file cache pool.</param>
        public void SetCustomBasePath(string basePath);

        /// <summary>
        /// Clean the cache files in the pool and delete them all.
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        public Task CleanAsync(bool force = false);

        /// <summary>
        /// Register a file cache in the pool.
        /// </summary>
        /// <param name="fileName">The cache file name. If leaves it empty, it will be filled by random file name.</param>
        /// <param name="subFolder">The subfolder for the storage.</param>
        /// <param name="create">Create file when register or not.</param>
        /// <returns>The file cache instance.</returns>
        public FileCache Register(string fileName = "", string subFolder = "", bool create = true);

        /// <summary>
        /// Unregister a file cache from the pool.
        /// </summary>
        /// <param name="fileCache">The file cache instance.</param>
        /// <param name="delete">Delete file when unregister or not. Highly recommended set to TRUE or leave default.</param>
        /// <returns>Success or not. Only if failed to delete the cache file but still is existed, return false.</returns>
        public bool UnRegister(FileCache fileCache, bool delete = true);
    }
}
