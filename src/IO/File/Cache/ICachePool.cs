using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Xanadu.Skidbladnir.IO.File.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICachePool : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public string BasePath { get; }

        /// <summary>
        /// 
        /// </summary>
        public ConcurrentDictionary<string, CacheFile> CacheFiles { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="basePath"></param>
        public void SetCustomBasePath(string basePath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        public Task CleanAsync(bool force = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="subFolder"></param>
        /// <param name="createFolder"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public CacheFile Register(string fileName = "", string subFolder = "", bool createFolder = true, bool create = true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheFile"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        public bool UnRegister(CacheFile cacheFile, bool delete = true);
    }
}
