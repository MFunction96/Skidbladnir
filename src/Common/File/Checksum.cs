using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
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

    public class ChecksumEventArg
    {
        public long Position { get; set; }
        public long Count { get; set; }
        public FileInfo FileInfo { get; set; }
    }

    public sealed class Checksum
    {
        private const int DefaultBufferSize = 1 << 26;

        public event EventHandler<ChecksumEventArg> CalculatePositionEvent;

        public event EventHandler<ChecksumEventArg> FileInfoEvent;

        private SHAAlgorithm Algorithm { get; }

        public Checksum(SHAAlgorithm shaAlgorithm)
        {
            Algorithm = shaAlgorithm;
        }

        #region GetFileHashAsync

        public async Task<string> GetFileHashAsync(string filePath, SHAFormatting shaFormatting, CancellationToken cancellationToken)
        {
            return await GetFileHashAsync(filePath, shaFormatting, DefaultBufferSize, cancellationToken);
        }

        public async Task<string> GetFileHashAsync(string filePath, SHAFormatting shaFormatting, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            var hash = await GetFileHashAsync(filePath, bufferSize, cancellationToken);
            return shaFormatting == SHAFormatting.Base64 ?
                Convert.ToBase64String(hash) :
                hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        public async Task<byte[]> GetFileHashAsync(string filePath, CancellationToken cancellationToken, int bufferSize = DefaultBufferSize)
        {
            return await GetFileHashAsync(filePath, bufferSize, cancellationToken);
        }

        public async Task<byte[]> GetFileHashAsync(string filePath, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            if (!System.IO.File.Exists(filePath)) throw new FileNotFoundException(filePath);
            FileInfoEvent?.Invoke(this, new ChecksumEventArg { FileInfo = new FileInfo(filePath) });
            await using var fs = System.IO.File.OpenRead(filePath);
            await using var bs = new BufferedStream(fs, bufferSize);
            return await GetStreamHashAsync(bs, bufferSize, cancellationToken);
        }

        #endregion

        #region GetBinaryHashAsync

        public async Task<string> GetBinaryHashAsync(byte[] binary, SHAFormatting shaFormatting, CancellationToken cancellationToken)
        {
            return await GetBinaryHashAsync(binary, shaFormatting, DefaultBufferSize, cancellationToken);
        }

        public async Task<string> GetBinaryHashAsync(byte[] binary, SHAFormatting shaFormatting, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            var hash = await GetBinaryHashAsync(binary, bufferSize, cancellationToken);
            return shaFormatting == SHAFormatting.Base64 ?
                Convert.ToBase64String(hash) :
                hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        public async Task<byte[]> GetBinaryHashAsync(byte[] binary, CancellationToken cancellationToken)
        {
            return await GetStreamHashAsync(new MemoryStream(binary), DefaultBufferSize, cancellationToken);
        }

        public async Task<byte[]> GetBinaryHashAsync(byte[] binary, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            return await GetStreamHashAsync(new MemoryStream(binary), bufferSize, cancellationToken);
        }

        #endregion

        #region GetObjectHashAsync

        public async Task<string> GetObjectHashAsync(object obj, SHAFormatting shaFormatting, CancellationToken cancellationToken)
        {
            return await GetObjectHashAsync(obj, shaFormatting, DefaultBufferSize, cancellationToken);
        }

        public async Task<string> GetObjectHashAsync(object obj, SHAFormatting shaFormatting, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            var hash = await GetObjectHashAsync(obj, bufferSize, cancellationToken);
            return shaFormatting == SHAFormatting.Base64 ?
                Convert.ToBase64String(hash) :
                hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        public async Task<byte[]> GetObjectHashAsync(object obj, CancellationToken cancellationToken)
        {
            return await GetObjectHashAsync(obj, DefaultBufferSize, cancellationToken);
        }

        public async Task<byte[]> GetObjectHashAsync(object obj, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            if (obj == null) throw new ArgumentNullException(@"obj can not be null!");
            var binary = JsonSerializer.SerializeToUtf8Bytes(obj, new JsonSerializerOptions { IgnoreNullValues = true });
            return await GetBinaryHashAsync(binary, bufferSize, cancellationToken);
        }

        #endregion

        #region GetStreamHashAsync

        public async Task<string> GetStreamHashAsync(Stream stream, SHAFormatting shaFormatting, CancellationToken cancellationToken)
        {
            return await GetStreamHashAsync(stream, shaFormatting, DefaultBufferSize, cancellationToken);
        }

        public async Task<string> GetStreamHashAsync(Stream stream, SHAFormatting shaFormatting, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            var hash = await GetStreamHashAsync(stream, bufferSize, cancellationToken);
            return shaFormatting == SHAFormatting.Base64 ?
                Convert.ToBase64String(hash) :
                hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        public async Task<byte[]> GetStreamHashAsync(Stream stream, CancellationToken cancellationToken)
        {
            return await GetStreamHashAsync(stream, DefaultBufferSize, cancellationToken);
        }

        public async Task<byte[]> GetStreamHashAsync(Stream stream, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            using HashAlgorithm sha = Algorithm switch
            {
                SHAAlgorithm.SHA1 => SHA1.Create(),
                SHAAlgorithm.SHA256 => SHA256.Create(),
                _ => SHA512.Create()
            };
            var buffer = new byte[bufferSize];
            var totalRead = 0L;

            while (stream.Length - totalRead >= bufferSize)
            {
                var readCount = await stream.ReadAsync(buffer, 0, bufferSize, cancellationToken);
                if (readCount != bufferSize) throw new DataException(@"Read Count is not equal to expected!\r\n");
                totalRead += sha.TransformBlock(buffer, 0, bufferSize, null, 0);
                CalculatePositionEvent?.Invoke(this, new ChecksumEventArg { Position = totalRead, Count = stream.Length });
            }

            var finalSize = (int)(stream.Length - totalRead);
            var readCountFinal = await stream.ReadAsync(buffer, 0, finalSize, cancellationToken);
            if (readCountFinal != finalSize) throw new DataException(@"Read Count is not equal to expected!\r\n");
            sha.TransformFinalBlock(buffer, 0, finalSize);
            return sha.Hash;
        }

        #endregion
    }
}
