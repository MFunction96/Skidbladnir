using System.IO;
using Xanadu.Skidbladnir.IO.File;
using Xanadu.Skidbladnir.IO.File.Cache;

namespace Xanadu.Skidbladnir.Test.IO.FileTests
{
    [TestClass]
    public class FileCachePoolTest
    {
        private FileCachePool _pool = null!;

        [TestInitialize]
        public void Setup()
        {
            _pool = new FileCachePool();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _pool.Dispose();
            IOExtension.DeleteDirectory(_pool.BasePath, allowNotFound: true, force: true);
        }

        [TestMethod]
        public void Register_CreatesFile()
        {
            var file = _pool.Register("test.txt");

            Assert.IsTrue(file.Exists);
            Assert.AreEqual(1, _pool.CacheFiles.Count);
        }

        [TestMethod]
        public void Unregister_DeletesFile()
        {
            var file = _pool.Register("test.txt");
            var success = _pool.UnRegister(file);

            Assert.IsTrue(success);
            Assert.IsFalse(File.Exists(file.FullPath));
        }

        [TestMethod]
        public void Clean_RemovesAllFiles()
        {
            _pool.Register("a.txt");
            _pool.Register("b.txt");

            _pool.Clean();

            Assert.AreEqual(0, _pool.CacheFiles.Count);
            // 文件可能仍存在，取决于是否调用 Dispose()
        }

        [TestMethod]
        public void Dispose_CleansUpFiles()
        {
            var file = _pool.Register("toDispose.txt");
            var path = file.FullPath;
            var basePath = this._pool.BasePath;
            _pool.Dispose();

            Assert.IsFalse(Directory.Exists(basePath) || File.Exists(path));
        }

        [TestMethod]
        public void FileCache_Delete_DeletesFile()
        {
            var file = _pool.Register("delete_me.txt");
            file.Delete();

            Assert.IsFalse(File.Exists(file.FullPath));
        }

        [TestMethod]
        public void FileCache_Dispose_Unregisters()
        {
            var file = _pool.Register("dispose.txt");
            file.Dispose();

            Assert.AreEqual(0, _pool.CacheFiles.Count);
        }

        [TestMethod]
        public void FileCache_Equals_WorksCorrectly()
        {
            var file1 = _pool.Register("equal.txt");
            var file2 = new FileCache(_pool, "equal.txt");

            Assert.AreEqual(file1, file2);
        }

        [TestMethod]
        public void FileCache_GetHashCode_WorksCorrectly()
        {
            var file1 = _pool.Register("hashcode.txt");
            var file2 = new FileCache(_pool, "hashcode.txt");

            Assert.AreEqual(file1.GetHashCode(), file2.GetHashCode());
        }
    }
}
