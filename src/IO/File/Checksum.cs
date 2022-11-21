using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Skidbladnir.IO.File
{
    /// <summary>
    /// The SHA family.
    /// </summary>
    public enum SHAAlgorithm
    {
        /// <summary>
        /// SHA1
        /// </summary>
        SHA1,
        /// <summary>
        /// SHA256
        /// </summary>
        SHA256,
        /// <summary>
        /// SHA512
        /// </summary>
        SHA512
    }

    /// <summary>
    /// The format of exportation for SHA.
    /// </summary>
    public enum SHAFormatting
    {
        /// <summary>
        /// Base64 format.
        /// </summary>
        Base64,
        /// <summary>
        /// Binary format.
        /// </summary>
        Binary,
        /// <summary>
        /// Hexadecimal format.
        /// </summary>
        Hexadecimal
    }

    /// <summary>
    /// The info of Checksum event.
    /// </summary>
    public class ChecksumEventArg
    {
        /// <summary>
        /// The position of the stream.
        /// </summary>
        public long Position { get; set; }

        /// <summary>
        /// The length of the stream.
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// The file info of the stream.
        /// </summary>
        public FileInfo FileInfo { get; set; }

    }

    /// <summary>
    /// The class of the checksum using SHA algorithm.
    /// </summary>
    public sealed class Checksum
    {
        /// <summary>
        /// 64MB is the default buffer size.
        /// </summary>
        private const int DefaultBufferSize = 1 << 26;

        /// <summary>
        /// The key for hash algorithm.
        /// </summary>
        private readonly string _key;

        /// <summary>
        /// When the stream position updates, it will raise this event and pass the current status on reading.
        /// </summary>
        public event EventHandler<ChecksumEventArg> CalculatePositionEvent;

        /// <summary>
        /// When the meeting 
        /// </summary>
        public event EventHandler<ChecksumEventArg> FileInfoEvent;

        /// <summary>
        /// The type of hash algorithm.
        /// </summary>
        public SHAAlgorithm Algorithm { get; }

        /// <summary>
        /// The constructor for the checksum.
        /// </summary>
        /// <param name="shaAlgorithm"></param>
        /// <param name="key"></param>
        public Checksum(SHAAlgorithm shaAlgorithm, string key = "")
        {
            Algorithm = shaAlgorithm;
            _key = key;
        }

        #region GetFileHashAsync

        /// <summary>
        /// Get the hash of file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash string of the file.</returns>
        public async Task<string> GetFileHashAsync(string filePath, SHAFormatting shaFormatting, CancellationToken cancellationToken)
        {
            return await GetFileHashAsync(filePath, shaFormatting, DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// Get the hash of file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash string of the file.</returns>
        public async Task<string> GetFileHashAsync(string filePath, SHAFormatting shaFormatting, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            var hash = await GetFileHashAsync(filePath, bufferSize, cancellationToken);
            return shaFormatting == SHAFormatting.Base64
                ? Convert.ToBase64String(hash)
                : hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        /// <summary>
        /// Get the hash of file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]> GetFileHashAsync(string filePath, CancellationToken cancellationToken, int bufferSize = DefaultBufferSize)
        {
            return await GetFileHashAsync(filePath, bufferSize, cancellationToken);
        }

        /// <summary>
        /// Get the hash of file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]> GetFileHashAsync(string filePath, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            FileInfoEvent?.Invoke(this, new ChecksumEventArg { FileInfo = new FileInfo(filePath) });
            await using var fileStream = System.IO.File.OpenRead(filePath);
            return await GetStreamHashAsync(fileStream, bufferSize, cancellationToken);
        }

        #endregion

        #region GetBinaryHashAsync

        /// <summary>
        /// Get the hash of binary.
        /// </summary>
        /// <param name="binary">The binary to compute.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash string of the file.</returns>
        public async Task<string> GetBinaryHashAsync(byte[] binary, SHAFormatting shaFormatting, CancellationToken cancellationToken)
        {
            return await GetBinaryHashAsync(binary, shaFormatting, DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// Get the hash of binary.
        /// </summary>
        /// <param name="binary">The binary to compute.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash string of the file.</returns>
        public async Task<string> GetBinaryHashAsync(byte[] binary, SHAFormatting shaFormatting, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            var hash = await GetBinaryHashAsync(binary, bufferSize, cancellationToken);
            return shaFormatting == SHAFormatting.Base64
                ? Convert.ToBase64String(hash)
                : hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        /// <summary>
        /// Get the hash of binary.
        /// </summary>
        /// <param name="binary">The binary to compute.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]> GetBinaryHashAsync(byte[] binary, CancellationToken cancellationToken)
        {
            return await GetStreamHashAsync(new MemoryStream(binary), DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// Get the hash of binary.
        /// </summary>
        /// <param name="binary">The binary to compute.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns></returns>
        public async Task<byte[]> GetBinaryHashAsync(byte[] binary, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            return await GetStreamHashAsync(new MemoryStream(binary), bufferSize, cancellationToken);
        }

        #endregion

        #region GetObjectHashAsync

        /// <summary>
        /// Get the hash of object. 
        /// </summary>
        /// <param name="obj">The object to compute.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash string of the file.</returns>
        public async Task<string> GetObjectHashAsync(object obj, SHAFormatting shaFormatting, CancellationToken cancellationToken)
        {
            return await GetObjectHashAsync(obj, shaFormatting, DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// Get the hash of object. 
        /// </summary>
        /// <param name="obj">The object to compute.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash string of the file.</returns>
        public async Task<string> GetObjectHashAsync(object obj, SHAFormatting shaFormatting, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            var hash = await GetObjectHashAsync(obj, bufferSize, cancellationToken);
            return shaFormatting == SHAFormatting.Base64
                ? Convert.ToBase64String(hash)
                : hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        /// <summary>
        /// Get the hash of object. 
        /// </summary>
        /// <param name="obj">The object to compute.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]> GetObjectHashAsync(object obj, CancellationToken cancellationToken)
        {
            return await GetObjectHashAsync(obj, DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// Get the hash of object. 
        /// </summary>
        /// <param name="obj">The object to compute.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]> GetObjectHashAsync(object obj, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var binary = JsonSerializer.SerializeToUtf8Bytes(obj, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
            return await GetBinaryHashAsync(binary, bufferSize, cancellationToken);
        }

        #endregion

        #region GetStreamHashAsync

        /// <summary>
        /// Get the hash of stream. 
        /// </summary>
        /// <param name="stream">The stream to compute.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash string of the file.</returns>
        public async Task<string> GetStreamHashAsync(Stream stream, SHAFormatting shaFormatting, CancellationToken cancellationToken)
        {
            return await GetStreamHashAsync(stream, shaFormatting, DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// Get the hash of stream. 
        /// </summary>
        /// <param name="stream">The stream to compute.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash string of the file.</returns>
        public async Task<string> GetStreamHashAsync(Stream stream, SHAFormatting shaFormatting, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            var hash = await GetStreamHashAsync(stream, bufferSize, cancellationToken);
            return shaFormatting == SHAFormatting.Base64
                ? Convert.ToBase64String(hash)
                : hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        /// <summary>
        /// Get the hash of stream. 
        /// </summary>
        /// <param name="stream">The stream to compute.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]> GetStreamHashAsync(Stream stream, CancellationToken cancellationToken)
        {
            return await GetStreamHashAsync(stream, DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// Get the hash of stream. 
        /// </summary>
        /// <param name="stream">The stream to compute.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]> GetStreamHashAsync(Stream stream, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            using HMAC sha = Algorithm switch
            {
                SHAAlgorithm.SHA1 => string.IsNullOrEmpty(_key) ? new HMACSHA1() : new HMACSHA1(Encoding.UTF8.GetBytes(_key)),
                SHAAlgorithm.SHA256 => string.IsNullOrEmpty(_key) ? new HMACSHA256() : new HMACSHA256(Encoding.UTF8.GetBytes(_key)),
                _ => string.IsNullOrEmpty(_key) ? new HMACSHA512() : new HMACSHA512(Encoding.UTF8.GetBytes(_key))
            };

            var buffer = new byte[bufferSize];
            var totalRead = 0L;

            await using var bufferedStream = new BufferedStream(stream, bufferSize);

            while (bufferedStream.Length - totalRead >= bufferSize)
            {
                var readCount = await bufferedStream.ReadAsync(buffer.AsMemory(0, bufferSize), cancellationToken);
                if (readCount != bufferSize)
                {
                    throw new DataException(@"Read Count is not equal to expected!\r\n");
                }

                totalRead += sha.TransformBlock(buffer, 0, bufferSize, null, 0);
                CalculatePositionEvent?.Invoke(this, new ChecksumEventArg { Position = totalRead, Length = bufferedStream.Length });
            }

            var finalSize = (int)(bufferedStream.Length - totalRead);
            var readCountFinal = await bufferedStream.ReadAsync(buffer.AsMemory(0, finalSize), cancellationToken);
            if (readCountFinal != finalSize)
            {
                throw new DataException(@"Read Count is not equal to expected!\r\n");
            }

            sha.TransformFinalBlock(buffer, 0, finalSize);
            return sha.Hash;
        }

        #endregion
    }
}
