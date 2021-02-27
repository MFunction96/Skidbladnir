using System;
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
            const string expect = @"QIeCsyx57gF3axoYuTUnP5v+h61oPGZqVLb3VzoHfo6MFzTNiaqUb3HSeZoCyWLtrZM75jQ570O30U0Ecr4dwg==";
            var watch = new Stopwatch();
            var checksum = new Common.File.Checksum(SHAAlgorithm.SHA512);
            watch.Start();

            var result = checksum.GetFileHashAsync($"{Environment.CurrentDirectory}\\Microsoft.TestPlatform.CoreUtilities.dll", SHAFormatting.Base64);

            watch.Stop();
            Assert.AreEqual(expect, await result);
            TestContext.WriteLine($"Test Finished!\nRun Time: {watch.ElapsedMilliseconds}");
        }
    }
}
