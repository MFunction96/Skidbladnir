using System.IO;
using Xanadu.Skidbladnir.IO.File;
using Xanadu.Skidbladnir.IO.File.Cache;

namespace Xanadu.Skidbladnir.Test.IO.File
{
    [TestClass]
    public class FileCachePoolTest
    {
        private FileCachePool _pool = null!;

        [TestInitialize]
        public void Setup()
        {
            this._pool = new FileCachePool();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this._pool.Dispose();
            IOExtension.DeleteDirectory(this._pool.BasePath, allowNotFound: true, force: true);
        }

        [TestMethod]
        public void Register_CreatesFile()
        {
            var file = this._pool.Register("test.txt");

            Assert.IsTrue(file.Exists);
            Assert.HasCount(1, this._pool.CacheFiles);
        }

        [TestMethod]
        public void Unregister_DeletesFile()
        {
            var file = this._pool.Register("test.txt");
            var success = this._pool.UnRegister(file);

            Assert.IsTrue(success);
            Assert.IsFalse(System.IO.File.Exists(file.FullPath));
        }

        [TestMethod]
        public void Clean_RemovesAllFiles()
        {
            this._pool.Register("a.txt");
            this._pool.Register("b.txt");

            this._pool.Clean();

            Assert.HasCount(0, this._pool.CacheFiles);
            // 文件可能仍存在，取决于是否调用 Dispose()
        }

        [TestMethod]
        public void Dispose_CleansUpFiles()
        {
            var file = _pool.Register("toDispose.txt");
            var path = file.FullPath;
            var basePath = this._pool.BasePath;
            this._pool.Dispose();

            Assert.IsFalse(Directory.Exists(basePath) || System.IO.File.Exists(path));
        }

        [TestMethod]
        public void FileCache_Delete_DeletesFile()
        {
            var file = this._pool.Register("delete_me.txt");
            file.Delete();

            Assert.IsFalse(System.IO.File.Exists(file.FullPath));
        }

        [TestMethod]
        public void FileCache_Dispose_Unregisters()
        {
            var file = this._pool.Register("dispose.txt");
            file.Dispose();

            Assert.IsEmpty(_pool.CacheFiles);
        }

        [TestMethod]
        public void FileCache_Equals_WorksCorrectly()
        {
            var file1 = this._pool.Register("equal.txt");
            var file2 = new FileCache(this._pool, "equal.txt");

            Assert.AreEqual(file1, file2);
        }

        [TestMethod]
        public void FileCache_GetHashCode_WorksCorrectly()
        {
            var file1 = this._pool.Register("hashcode.txt");
            var file2 = new FileCache(this._pool, "hashcode.txt");

            Assert.AreEqual(file1.GetHashCode(), file2.GetHashCode());
        }
    }
}
