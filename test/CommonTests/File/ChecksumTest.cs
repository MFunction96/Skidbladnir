using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skidbladnir.Common.File;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Skidbladnir.CommonTests.File
{
    [TestClass]
    public class ChecksumTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task GetSHA256Test()
        {
            TestContext.WriteLine("Test Started!");
            const string expect = @"VT60uG92jXAm9PNhoEZ5u4+2PZlZT4zgkqtsfapgkgUW+9q+1aHcOBmQ/zbbffEZ/VhPQI7tYk3+RMqT5cmp0w==";
            var watch = new Stopwatch();
            var checksum = new Checksum(SHAAlgorithm.SHA512, "Hello World!");
            watch.Start();

            var result = await checksum.GetObjectHashAsync("Hello World!", SHAFormatting.Base64);
            result = await checksum.GetObjectHashAsync("Hello World!", SHAFormatting.Base64);

            watch.Stop();
            Assert.AreEqual(expect, result);
            TestContext.WriteLine($"Test Finished!\r\nExpected: {expect}\r\nActual: {result}\r\nRun Time: {watch.ElapsedMilliseconds}ms\r\n");
        }
    }
}
