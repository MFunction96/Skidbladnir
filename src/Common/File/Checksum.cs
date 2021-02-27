using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Skidbladnir.Common.File
{
    public enum SHAAlgorithm
    {
        SHA1,
        SHA256,
        SHA512
    }

    public enum SHAFormatting
    {
        Base64,
        Hexadecimal
    }

    public static class Checksum
    {
        public static Task<string> GetFileHash(string filePath, SHAAlgorithm shaAlgorithm, SHAFormatting shaFormatting, int bufferSize = 1 << 26)
        {
            return GetFileHash(filePath, shaAlgorithm, shaFormatting, bufferSize, CancellationToken.None);
        }

        public static Task<string> GetFileHash(string filePath, SHAAlgorithm shaAlgorithm, SHAFormatting shaFormatting, CancellationToken cancellationToken, int bufferSize = 1 << 26)
        {
            return GetFileHash(filePath, shaAlgorithm, shaFormatting, bufferSize, cancellationToken);
        }

        public static async Task<string> GetFileHash(string filePath, SHAAlgorithm shaAlgorithm, SHAFormatting shaFormatting, int bufferSize, CancellationToken cancellationToken)
        {
            var hash = await GetFileHash(filePath, shaAlgorithm, bufferSize, cancellationToken);
            return shaFormatting == SHAFormatting.Base64 ? Convert.ToBase64String(hash) : hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        public static Task<byte[]> GetFileHash(string filePath, SHAAlgorithm shaAlgorithm, int bufferSize = 1 << 26)
        {
            return GetFileHash(filePath, shaAlgorithm, bufferSize, CancellationToken.None);
        }

        public static Task<byte[]> GetFileHash(string filePath, SHAAlgorithm shaAlgorithm, CancellationToken cancellationToken, int bufferSize = 1 << 26)
        {
            return GetFileHash(filePath, shaAlgorithm, bufferSize, cancellationToken);
        }

        public static Task<byte[]> GetFileHash(string filePath, SHAAlgorithm shaAlgorithm, int bufferSize, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (!System.IO.File.Exists(filePath)) throw new FileNotFoundException(filePath);
                using var fs = System.IO.File.OpenRead(filePath);
                using var bs = new BufferedStream(fs, bufferSize);
                HashAlgorithm sha = shaAlgorithm switch
                {
                    SHAAlgorithm.SHA1 => SHA1.Create(),
                    SHAAlgorithm.SHA256 => SHA256.Create(),
                    _ => SHA512.Create()
                };
                var hash = sha.ComputeHash(bs);
                sha.Dispose();
                return hash;
            }, cancellationToken);
        }
    }
}
