using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Skidbladnir.CommonTest.File
{
    [TestClass]
    public class Checksum
    {
        private TestContext TestContext { get; set; }

        private CancellationTokenSource _cancelToken = new();

        [TestMethod]
        public async Task TestGetSHA256()
        {
            TestContext.WriteLine("Test started!");

            var cancelTask = Task.Run((() =>
            {
                while (Console.ReadKey().Key != ConsoleKey.Enter)
                {
                    Console.WriteLine("Press the ENTER key to cancel...");
                }

                Console.WriteLine("\nENTER key pressed: cancelling downloads.\n");
                _cancelToken.Cancel();
            }));
            var watch = new Stopwatch();
            watch.Start();
            const string expect = @"1B0EQw++DTFwcSaEYa9m8qQS8lgzvlMSwNOYJbGZTiY=";
            var checksumTask = Common.File.Checksum.GetSHA256Async(@"E:\Captura\2021-01-01-20-01-58.mp4", _cancelToken.Token);

            await Task.WhenAny(new[] { cancelTask, checksumTask });

            var result = checksumTask.IsCompletedSuccessfully ? checksumTask.Result : string.Empty;

            Assert.AreEqual(expect, result);
            TestContext.WriteLine($"Test Finished!\nRun Time: {watch.ElapsedMilliseconds}");
        }
    }
}
