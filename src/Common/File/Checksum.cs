using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Skidbladnir.Common.File
{
    public static class Checksum
    {
        private const int DefaultBufferSize = 1 << 26;

        public static Task<string> GetSHA256Async(string filePath, CancellationToken cancellationToken)
        {
            return GetSHA256Async(filePath, true, DefaultBufferSize, cancellationToken);
        }

        public static Task<string> GetSHA256Async(string filePath, bool base64 = true, int bufferSize = DefaultBufferSize)
        {
            return GetSHA256Async(filePath, base64, bufferSize, CancellationToken.None);
        }

        public static Task<string> GetSHA256Async(string filePath, bool base64, int bufferSize, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (!System.IO.File.Exists(filePath)) throw new FileNotFoundException(filePath);
                using var fs = System.IO.File.OpenRead(filePath);
                using var bs = new BufferedStream(fs, bufferSize);
                using var sha256 = SHA256.Create();
                var hash = sha256.ComputeHash(bs);
                return base64
                    ? Convert.ToBase64String(hash)
                    : hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
            }, cancellationToken);
        }


    }
}
