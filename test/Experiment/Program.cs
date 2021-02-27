using Skidbladnir.Common.File;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Skidbladnir.Experiment
{
    internal static class Program
    {
        private static readonly CancellationTokenSource SCts = new();

        private static readonly ConcurrentQueue<long> ConcurrentQueue = new();

        private static object _statisticsThreadStatus = new();

        private static FileInfo fileInfo = new(@"E:\Captura\2021-01-01-20-01-58.mp4");

        private static async Task Main(string[] args)
        {
            Console.WriteLine("Application started.");
            Console.WriteLine("Press the ENTER key to cancel...");

            var cancelTask = Task.Run(() =>
            {
                while (Console.ReadKey().Key != ConsoleKey.Enter)
                {
                    Console.WriteLine("Press the ENTER key to cancel...");
                }

                Console.WriteLine("ENTER key pressed: cancelling calculating file.");
                SCts.Cancel();
            });
            var watch = new Stopwatch();

            var checksum = new Checksum(SHAAlgorithm.SHA512);
            checksum.CalculatePositionEvent += ChecksumOnCalculatePositionEvent;
            checksum.FileInfoEvent += ChecksumFileInfoEvent;

            var statistics = new Thread(StatisticsThread);
            _statisticsThreadStatus = true;
            statistics.Start();
            watch.Start();

            var sumPageSizesTask = checksum.GetFileHashAsync(@"E:\Captura\2021-01-01-20-01-58.mp4", SHAFormatting.Base64, 1 << 20, SCts.Token);

            await Task.WhenAny(new[] { cancelTask, sumPageSizesTask });
            watch.Stop();

            lock (_statisticsThreadStatus)
            {
                _statisticsThreadStatus = false;
            }
            if (sumPageSizesTask.IsCompletedSuccessfully)
                Console.WriteLine($"File Hash is {sumPageSizesTask.Result} in {watch.ElapsedMilliseconds}ms.");
            else if (cancelTask.IsCompletedSuccessfully) Console.WriteLine("Task Cancelled!");
            else if (sumPageSizesTask.Exception != null) throw sumPageSizesTask.Exception;
            else throw new SystemException("Fatal error!");

            Console.WriteLine("Application ending.");
        }

        private static void StatisticsThread()
        {
            while ((bool) _statisticsThreadStatus)
            {
                FileInfo file;
                lock (fileInfo)
                {
                    file = fileInfo;
                }
                var peek = ConcurrentQueue.TryDequeue(out var start);
                if (peek)
                {
                    var end = start;
                    while (ConcurrentQueue.TryDequeue(out var temp)) end = temp;
                    Console.WriteLine($"Speed: {end - start >> 20} MB/s. {file.Length - end >> 20} MB Remain.");
                }
                Thread.Sleep(1000);
            }
        }

        private static void ChecksumFileInfoEvent(object sender, ChecksumEventArg e)
        {
            lock (fileInfo)
            {
                fileInfo = e.FileInfo;
            }
        }

        private static void ChecksumOnCalculatePositionEvent(object sender, ChecksumEventArg e)
        {
            ConcurrentQueue.Enqueue(e.Position);
            //Console.WriteLine($"Position : {e.Position}. Total : {e.Count}");
        }
    }
}
