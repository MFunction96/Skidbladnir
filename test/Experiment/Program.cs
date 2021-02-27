using Skidbladnir.Common.File;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Skidbladnir.Experiment
{
    internal static class Program
    {
        private static readonly CancellationTokenSource SCts = new();

        private static async Task Main(string[] args)
        {
            Console.WriteLine("Application started.");
            Console.WriteLine("Press the ENTER key to cancel...\n");

            var cancelTask = Task.Run(() =>
            {
                while (Console.ReadKey().Key != ConsoleKey.Enter)
                {
                    Console.WriteLine("Press the ENTER key to cancel...");
                }

                Console.WriteLine("\nENTER key pressed: cancelling calculating file.\n");
                SCts.Cancel();
            });
            var watch = new Stopwatch();
            watch.Start();

            var sumPageSizesTask = Checksum.GetFileHash(@"E:\Captura\2021-01-01-20-01-58.mp4", SHAAlgorithm.SHA512, SHAFormatting.Base64, SCts.Token);

            await Task.WhenAny(new[] { cancelTask, sumPageSizesTask });
            watch.Stop();

            if (sumPageSizesTask.IsCompletedSuccessfully) Console.WriteLine($"File Hash is {sumPageSizesTask.Result} in {watch.ElapsedMilliseconds}ms.");

            Console.WriteLine("Application ending.");
        }
    }
}
