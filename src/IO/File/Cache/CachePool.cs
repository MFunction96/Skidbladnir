using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Skidbladnir.IO.File.Cache
{
    public class CachePool : IDisposable
    {
        private bool _disposed;

        private readonly ILogger _logger;

        public string BasePath { get; }

        public ISet<CacheFile> CacheFiles { get; }

        public CachePool(string basePath = "", bool clean = false, ILogger logger = default)
        {
            this._logger = logger;
            this.CacheFiles = new HashSet<CacheFile>();
            this.BasePath = string.IsNullOrEmpty(basePath) ? Path.Combine(Path.GetTempPath(), Assembly.GetAssembly(GetType())!.FullName!) : basePath;
            if (clean)
            {
                this.CleanAsync(true).GetAwaiter().GetResult();
            }

        }

        public async Task CleanAsync(bool force = false)
        {
            var msg = $"Clean {this.BasePath}...";
            this._logger.LogInformation(msg);
            await Deletion.DeleteDirectory(this.BasePath, true, force);
            this.CacheFiles.Clear();
        }

        public string Register(string fileName, string subFolder = "")
        {
            var file = new CacheFile(this, fileName, subFolder);
            var result = this.CacheFiles.Add(file);
            if (result)
            {
                return file.FullPath;
            }

            var msg = $"{file.FullPath} is already at current cache pool.";
            this._logger.LogWarning(msg);
            return file.FullPath;
        }

        public bool UnRegister(CacheFile cacheFile)
        {
            return this.CacheFiles.Remove(cacheFile);
        }

        public override bool Equals(object obj)
        {
            if (obj is not CachePool cachePool)
            {
                return false;
            }

            return this.BasePath.Equals(cachePool.BasePath) && this.CacheFiles.SetEquals(cachePool.CacheFiles);
        }

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
                this.CleanAsync(true).GetAwaiter().GetResult();
                this._logger?.LogInformation($"{this.GetType().Name} disposing");

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
