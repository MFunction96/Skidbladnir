using System;
using System.Diagnostics;
using System.IO;
using Xanadu.Skidbladnir.IO.File;

namespace Xanadu.Skidbladnir.Test.IO.FileTests
{
    [TestClass]
    public class IOExtensionTest
    {
        private string _testRoot = null!;

        [TestInitialize]
        public void Setup()
        {
            this._testRoot = Path.Combine(Path.GetTempPath(), "IOExtensionTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(this._testRoot);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (!Directory.Exists(this._testRoot))
            {
                return;
            }

            try
            {
                Directory.Delete(this._testRoot, true);
            }
            catch
            {
                /* ignore */
            }
        }

        [TestMethod]
        public void AppDataFolder_ShouldReturnValidPath()
        {
            var path = IOExtension.AppDataFolder;
            Assert.IsFalse(string.IsNullOrWhiteSpace(path));
            Assert.Contains(Process.GetCurrentProcess().ProcessName, path);
        }

        [TestMethod]
        public void CopyDirectory_ShouldCopyAllFilesAndSubdirectories()
        {
            var source = Path.Combine(this._testRoot, "source");
            var dest = Path.Combine(this._testRoot, "dest");

            Directory.CreateDirectory(source);
            File.WriteAllText(Path.Combine(source, "file.txt"), "Hello");
            var subDir = Path.Combine(source, "subdir");
            Directory.CreateDirectory(subDir);
            File.WriteAllText(Path.Combine(subDir, "subfile.txt"), "World");

            IOExtension.CopyDirectory(source, dest);

            Assert.IsTrue(File.Exists(Path.Combine(dest, "file.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(dest, "subdir", "subfile.txt")));
        }

        [TestMethod]
        public void DeleteFile_ShouldDeleteExistingFile()
        {
            var file = Path.Combine(this._testRoot, "test.txt");
            File.WriteAllText(file, "data");
            File.SetAttributes(file, FileAttributes.ReadOnly);

            IOExtension.DeleteFile(file);
            Assert.IsFalse(File.Exists(file));
        }

        [TestMethod]
        public void DeleteFile_AllowNotFound_ShouldNotThrow()
        {
            var file = Path.Combine(this._testRoot, "nonexistent.txt");
            IOExtension.DeleteFile(file, allowNotFound: true);
        }

        [TestMethod]
        public void DeleteFile_DisallowNotFound_ShouldThrow()
        {
            var file = Path.Combine(this._testRoot, "nonexistent.txt");
            Assert.ThrowsExactly<FileNotFoundException>(() => IOExtension.DeleteFile(file, allowNotFound: false));
        }

        [TestMethod]
        public void DeleteDirectory_Force_ShouldRemoveReadOnlyFiles()
        {
            var dir = Path.Combine(this._testRoot, "readonlydir");
            Directory.CreateDirectory(dir);
            var file = Path.Combine(dir, "ro.txt");
            File.WriteAllText(file, "readonly");
            File.SetAttributes(file, FileAttributes.ReadOnly);

            IOExtension.DeleteDirectory(dir, allowNotFound: false, force: true);

            Assert.IsFalse(Directory.Exists(dir));
        }

        [TestMethod]
        public void DeleteDirectory_AllowNotFound_ShouldNotThrow()
        {
            var dir = Path.Combine(this._testRoot, "missingdir");
            IOExtension.DeleteDirectory(dir, allowNotFound: true);
        }

        [TestMethod]
        public void DeleteDirectory_DisallowNotFound_ShouldThrow()
        {
            var dir = Path.Combine(this._testRoot, "missingdir");
            Assert.ThrowsExactly<DirectoryNotFoundException>(() =>
                IOExtension.DeleteDirectory(dir, allowNotFound: false));
        }
    }
}
