using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
// ReSharper disable ClassNeverInstantiated.Global

namespace Xanadu.Skidbladnir.IO.File.Cache
{
    /// <summary>
    /// The implementation of the file cache pool.
    /// </summary>
    /// <param name="logger">The logger in System.</param>
    public class FileCachePool(ILogger<FileCachePool> logger) : IFileCachePool
    {
        /// <summary>
        /// The flag of the dispose status.
        /// </summary>
        private bool _disposed;

        /// <inheritdoc />
        public string BasePath { get; private set; } = Path.Combine(Path.GetTempPath(), Process.GetCurrentProcess().ProcessName);

        /// <inheritdoc />
        public ICollection<FileCache> CacheFiles => this.InternalCacheFiles.Values;
        
        /// <summary>
        /// 
        /// </summary>
        protected ConcurrentDictionary<string, FileCache> InternalCacheFiles { get; } = new();

        /// <inheritdoc />
        public void SetCustomBasePath(string basePath)
        {
            this.Clean();
            this.BasePath = basePath;
        }

        /// <inheritdoc />
        public virtual void Clean(bool force = false)
        {
            var msg = $"Clean {this.BasePath}...";
            logger.LogInformation(msg, string.Empty);
            this.InternalCacheFiles.Clear();
        }

        /// <inheritdoc />
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
            if (result)
            {
                if (create)
                {
                    file.Create();
                }

                return file;
            }

            var msg = $"{file.FullPath} is already at current cache pool. Keep previous creation state: {file.Exists}";
            logger.LogWarning(msg, string.Empty);
            return file;
        }

        /// <inheritdoc />
        public virtual bool UnRegister(FileCache fileCache, bool detete = true)
        {
            var result = this.InternalCacheFiles.TryRemove(fileCache.FullPath, out _);
            if (!result)
            {
                result = !this.InternalCacheFiles.ContainsKey(fileCache.FullPath);
            }

            if (result && detete)
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

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
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
                this.Clean(true);
                logger.LogInformation($"{this.GetType().Name} disposing", string.Empty);

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
