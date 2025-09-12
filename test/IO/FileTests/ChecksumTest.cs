using System.Diagnostics;
using System.Threading.Tasks;
using Xanadu.Skidbladnir.Core.Extension;
using Xanadu.Skidbladnir.IO.File;

namespace Xanadu.Skidbladnir.Test.IO.File
{
    [TestClass]
    public class ChecksumTest
    {
        public TestContext TestContext { get; set; } = null!;

        [TestMethod]
        public async Task GetSHA256Test()
        {
            this.TestContext.WriteLine("Test Started!");
            const string expect = "eace7ca4f91eff4da627ba7cda1346fa9600656f9331496f9b64e5cffa171bd7";
            const string filePath = "1225.pdf";
            var watch = new Stopwatch();
            var checksum = new Checksum(SHAAlgorithm.SHA256);
            watch.Start();
            var fileResult = await checksum.GetFileHashAsync(filePath, BinaryFormatting.Hexadecimal);
            Assert.AreEqual(expect, fileResult.ToLower());
            var binary = await System.IO.File.ReadAllBytesAsync(filePath);
            var binResult = await checksum.GetBinaryHashAsync(binary, BinaryFormatting.Hexadecimal);
            Assert.AreEqual(expect, binResult.ToLower());
            watch.Stop();
            this.TestContext.WriteLine($"Test Finished!\r\nExpected: {expect}\r\nActual: {fileResult}\r\nRun Time: {watch.ElapsedMilliseconds}ms\r\n");
        }
    }
}