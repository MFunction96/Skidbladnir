using System;
using System.Threading;
using System.Threading.Tasks;
using Skidbladnir.Common.File;

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

            var sumPageSizesTask = Checksum.GetSHA256Async(@"E:\Captura\2021-01-01-20-01-58.mp4");

            await Task.WhenAny(new[] { cancelTask, sumPageSizesTask });

            Console.WriteLine("Application ending.");
        }
    }
}
