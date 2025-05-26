using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ReSharper disable ClassNeverInstantiated.Global

namespace Xanadu.Skidbladnir.IO.File.Cache
{
    /// <summary>
    /// File cache pool which used to manage file cache. Support Dependency Injection.
    /// </summary>
    public class FileCachePool : IDisposable
    {
        /// <summary>
        /// The flag of the dispose status.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The base path of the file cache pool. Path is %TMP%/{RandomPath}, where {RandomPath} is a random file name.
        /// </summary>
        public string BasePath { get; } = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        /// <summary>
        /// The current cache files in the pool.
        /// </summary>
        public ICollection<FileCache> CacheFiles => this.InternalCacheFiles.Values;

        /// <summary>
        /// 
        /// </summary>
        protected ConcurrentDictionary<string, FileCache> InternalCacheFiles { get; } = new();

        /// <summary>
        /// Clean the cache files in the pool and delete them all.
        /// </summary>
        public virtual void Clean()
        {
            foreach (var fileCache in this.InternalCacheFiles.Values.ToArray())
            {
                fileCache.Dispose();
            }

            IOExtension.DeleteDirectory(this.BasePath, allowNotFound: true, force: true);
            this.InternalCacheFiles.Clear();
        }

        /// <summary>
        /// Register a file cache in the pool.
        /// </summary>
        /// <param name="fileName">The cache file name. If leaves it empty, it will be filled by random file name.</param>
        /// <param name="subFolder">The subfolder for the storage.</param>
        /// <param name="create">Create file when register or not.</param>
        /// <returns>The file cache instance.</returns>
        public virtual FileCache Register(string fileName = "", string subFolder = "", bool create = true)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Path.GetRandomFileName();
            }

            var file = new FileCache(this, fileName, subFolder);
            var folder = Path.Combine(this.BasePath, subFolder);
            if (create && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var result = this.InternalCacheFiles.TryAdd(file.FullPath, file);
            if (!result)
            {
                return file;
            }

            if (create)
            {
                file.Create();
            }

            return file;
        }

        /// <summary>
        /// Unregister a file cache from the pool.
        /// </summary>
        /// <param name="fileCache">The file cache instance.</param>
        /// <param name="delete">Delete file when unregister or not. Highly recommended set to TRUE or leave default.</param>
        /// <returns>Success or not. Only if failed to delete the cache file but still is existed, return false.</returns>
        public virtual bool UnRegister(FileCache fileCache, bool delete = true)
        {
            var result = this.InternalCacheFiles.TryRemove(fileCache.FullPath, out _);
            if (!result)
            {
                result = !this.InternalCacheFiles.ContainsKey(fileCache.FullPath);
            }

            if (result && delete)
            {
                fileCache.Delete();
            }

            return result;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is not FileCachePool cachePool)
            {
                return false;
            }

            return this.BasePath.Equals(cachePool.BasePath) && this.InternalCacheFiles.Equals(cachePool.InternalCacheFiles);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.BasePath);
        }

        #region Disposing

        /// <summary>
        /// Implement IDisposable. Do not make this method virtual. A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// </summary>
        /// <param name="disposing">If disposing equals true, the method has been called directly, or indirectly by a user's code. Managed and unmanaged resources can be disposed. If disposing equals false, the method has been called by the runtime from inside the finalizer and you should not reference other objects. Only unmanaged resources can be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (this._disposed)
            {
                return;
            }

            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources.
                this.Clean();

            }
            // Call the appropriate methods to clean up
            // unmanaged resources here.
            // If disposing is false,
            // only the following code is executed.

            this._disposed = true;

        }

        /// <summary>
        /// The finalize method.
        /// </summary>
        ~FileCachePool()
        {
            this.Dispose(false);
        }

        #endregion
    }
}
