using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skidbladnir.Common.File;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Skidbladnir.CommonTest.File
{
    [TestClass]
    public class Checksum
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task TestGetSHA256()
        {
            TestContext.WriteLine("Test started!");
            const string expect = @"X9m2LqN3Bv+yBgZmQ2PstvVky9BPGZHOSOEUfW2ABTtfWelLA5sQgwH81w9YHHxJmyEvIOkIuSr07mcqZnLuDw==";
            var watch = new Stopwatch();
            var checksum = new Common.File.Checksum(SHAAlgorithm.SHA512);
            watch.Start();

            var result = checksum.GetFileHashAsync(@"E:\Captura\2021-01-01-20-01-58.mp4", SHAFormatting.Base64);

            watch.Stop();
            Assert.AreEqual(expect, await result);
            TestContext.WriteLine($"Test Finished!\nRun Time: {watch.ElapsedMilliseconds}");
        }
    }
}
