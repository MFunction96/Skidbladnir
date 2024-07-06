using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Xanadu.Skidbladnir.IO.File.Cache
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public class CachePool(ILogger<CachePool> logger) : ICachePool
    {
        /// <summary>
        /// 
        /// </summary>
        private bool _disposed;

        /// <inheritdoc />
        public string BasePath { get; protected set; } = Path.Combine(Path.GetTempPath(), Process.GetCurrentProcess().ProcessName);

        /// <inheritdoc />
        public ISet<CacheFile> CacheFiles { get; } = new HashSet<CacheFile>();

        /// <inheritdoc />
        public async void SetCustomBasePath(string basePath)
        {
            await this.CleanAsync();
            this.BasePath = basePath;
        }

        /// <inheritdoc />
        public async Task CleanAsync(bool force = false)
        {
            var msg = $"Clean {this.BasePath}...";
            logger.LogInformation(msg, string.Empty);
            await IOExtension.DeleteDirectory(this.BasePath, true, force);
            this.CacheFiles.Clear();
        }

        /// <inheritdoc />
        public CacheFile Register(string fileName, string subFolder = "", bool createFolder = true)
        {
            var file = new CacheFile(this, fileName, subFolder);
            var folder = Path.Combine(this.BasePath, subFolder);
            if (createFolder && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var result = this.CacheFiles.Add(file);
            if (result)
            {
                return file;
            }

            var msg = $"{file.FullPath} is already at current cache pool.";
            logger.LogWarning(msg, string.Empty);
            return file;
        }

        /// <inheritdoc />
        public bool UnRegister(CacheFile cacheFile)
        {
            return this.CacheFiles.Remove(cacheFile);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is not CachePool cachePool)
            {
                return false;
            }

            return this.BasePath.Equals(cachePool.BasePath) && this.CacheFiles.SetEquals(cachePool.CacheFiles);
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
                this.CleanAsync(true).GetAwaiter().GetResult();
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
        ~CachePool()
        {
            this.Dispose(false);
        }

        #endregion
    }
}
