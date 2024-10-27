using System;
using System.IO;

namespace Xanadu.Skidbladnir.IO.File.Cache
{
    /// <summary>
    /// File cache which used to manage file cache.
    /// </summary>
    /// <param name="pool">The file cache pool which the instance bind.</param>
    /// <param name="fileName">The file name of the file cache.</param>
    /// <param name="subFolder">The subfolder of the file cache in pool.</param>
    public class FileCache(FileCachePool pool, string fileName, string subFolder = "") : IDisposable
    {
        /// <summary>
        /// The switcher of the dispose status.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The file name of the file cache.
        /// </summary>
        public string FileName => Path.GetFileName(this.FullPath);

        /// <summary>
        /// The relative path of the file cache.
        /// </summary>
        public string RelativePath => Path.GetRelativePath(this.Pool.BasePath, this.FullPath);

        /// <summary>
        /// The full path of the file cache.
        /// </summary>
        public string FullPath { get; } = Path.Combine(pool.BasePath, subFolder, fileName);

        /// <summary>
        /// The existence of the file cache.
        /// </summary>
        public bool Exists => System.IO.File.Exists(this.FullPath);

        /// <summary>
        /// The file cache pool which the instance bind.
        /// </summary>
        public FileCachePool Pool => pool;

        /// <summary>
        /// Create the file cache.
        /// </summary>
        public void Create()
        {
            System.IO.File.Create(this.FullPath).Close();
        }

        /// <summary>
        /// Delete the file cache.
        /// </summary>
        public void Delete()
        {
            IOExtension.DeleteFile(this.FullPath);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is not FileCache cacheFile)
            {
                return false;
            }

            return this.Pool.Equals(cacheFile.Pool) && this.FullPath.Equals(cacheFile.FullPath);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Pool, this.FullPath);
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
                if (!this.Pool.UnRegister(this))
                {
                    this.Delete();
                }
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
        ~FileCache()
        {
            this.Dispose(false);
        }

        #endregion
    }
}
