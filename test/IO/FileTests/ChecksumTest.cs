using Skidbladnir.IO.File;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Skidbladnir.Test.IO.FileTests
{
    [TestClass]
    public class ChecksumTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task GetSHA256Test()
        {
            TestContext.WriteLine("Test Started!");
            const string expect = @"eace7ca4f91eff4da627ba7cda1346fa9600656f9331496f9b64e5cffa171bd7";
            var filePath = "1225.pdf";
            var watch = new Stopwatch();
            var checksum = new Checksum(SHAAlgorithm.SHA256);
            watch.Start();
            var fileResult = await checksum.GetFileHashAsync(filePath, SHAFormatting.Hexadecimal);
            Assert.AreEqual(expect, fileResult.ToLower());
            var binary = await File.ReadAllBytesAsync(filePath);
            var binResult = await checksum.GetBinaryHashAsync(binary, SHAFormatting.Hexadecimal);
            Assert.AreEqual(expect, binResult.ToLower());
            watch.Stop();
            TestContext.WriteLine($"Test Finished!\r\nExpected: {expect}\r\nActual: {fileResult}\r\nRun Time: {watch.ElapsedMilliseconds}ms\r\n");
        }
    }
}