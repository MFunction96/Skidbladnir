using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xanadu.Skidbladnir.IO.File;

namespace Xanadu.Skidbladnir.Test.IO.FileTests
{
    [TestClass]
    public class IOExtensionTest
    {
        private string _testRoot;

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
            string path = IOExtension.AppDataFolder;
            Assert.IsFalse(string.IsNullOrWhiteSpace(path));
            Assert.IsTrue(path.Contains(Process.GetCurrentProcess().ProcessName));
        }

        [TestMethod]
        public void CopyDirectory_ShouldCopyAllFilesAndSubdirectories()
        {
            string source = Path.Combine(_testRoot, "source");
            string dest = Path.Combine(_testRoot, "dest");

            Directory.CreateDirectory(source);
            File.WriteAllText(Path.Combine(source, "file.txt"), "Hello");
            string subDir = Path.Combine(source, "subdir");
            Directory.CreateDirectory(subDir);
            File.WriteAllText(Path.Combine(subDir, "subfile.txt"), "World");

            IOExtension.CopyDirectory(source, dest);

            Assert.IsTrue(File.Exists(Path.Combine(dest, "file.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(dest, "subdir", "subfile.txt")));
        }

        [TestMethod]
        public void DeleteFile_ShouldDeleteExistingFile()
        {
            string file = Path.Combine(this._testRoot, "test.txt");
            File.WriteAllText(file, "data");
            File.SetAttributes(file, FileAttributes.ReadOnly);

            IOExtension.DeleteFile(file);
            Assert.IsFalse(File.Exists(file));
        }

        [TestMethod]
        public void DeleteFile_AllowNotFound_ShouldNotThrow()
        {
            string file = Path.Combine(this._testRoot, "nonexistent.txt");
            IOExtension.DeleteFile(file, allowNotFound: true);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void DeleteFile_DisallowNotFound_ShouldThrow()
        {
            string file = Path.Combine(this._testRoot, "nonexistent.txt");
            IOExtension.DeleteFile(file, allowNotFound: false);
        }

        [TestMethod]
        public void DeleteDirectory_Force_ShouldRemoveReadOnlyFiles()
        {
            string dir = Path.Combine(this._testRoot, "readonlydir");
            Directory.CreateDirectory(dir);
            string file = Path.Combine(dir, "ro.txt");
            File.WriteAllText(file, "readonly");
            File.SetAttributes(file, FileAttributes.ReadOnly);

            IOExtension.DeleteDirectory(dir, allowNotFound: false, force: true);

            Assert.IsFalse(Directory.Exists(dir));
        }

        [TestMethod]
        public void DeleteDirectory_AllowNotFound_ShouldNotThrow()
        {
            string dir = Path.Combine(_testRoot, "missingdir");
            IOExtension.DeleteDirectory(dir, allowNotFound: true);
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void DeleteDirectory_DisallowNotFound_ShouldThrow()
        {
            string dir = Path.Combine(_testRoot, "missingdir");
            IOExtension.DeleteDirectory(dir, allowNotFound: false);
        }
    }
}
