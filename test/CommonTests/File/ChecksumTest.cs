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
            TestContext.WriteLine("Test started!");
            const string expect = @"J8dGcK23UHX60FjVzq97IMTneGyDuuijL2Jvl4KvNMmjPCBG72D9Knh403jin+yFGAa72aZ4ePOp8c2kgwdj/Q==";
            var watch = new Stopwatch();
            var checksum = new Common.File.Checksum(SHAAlgorithm.SHA512);
            watch.Start();

            var result = checksum.GetObjectHashAsync(checksum, SHAFormatting.Base64);

            watch.Stop();
            Assert.AreEqual(expect, await result);
            TestContext.WriteLine($"Test Finished!\r\nExpected: {expect}\r\nActual: {await result}\r\nRun Time: {watch.ElapsedMilliseconds}ms\r\n");
        }
    }
}
