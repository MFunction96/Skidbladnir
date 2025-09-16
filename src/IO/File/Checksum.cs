using System;
using System.Buffers;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xanadu.Skidbladnir.Core.Extension;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Xanadu.Skidbladnir.IO.File
{
    /// <summary>
    /// The SHA family.
    /// </summary>
    public enum SHAAlgorithm
    {
        /// <summary>
        /// SHA256
        /// </summary>
        SHA256,
        /// <summary>
        /// SHA384
        /// </summary>
        SHA384,
        /// <summary>
        /// SHA512
        /// </summary>
        SHA512
    }

    /// <summary>
    /// The info of Checksum event.
    /// </summary>
    public readonly struct ChecksumEventArg
    {
        /// <summary>
        /// The position of the stream.
        /// </summary>
        public long Position { get; init; }

        /// <summary>
        /// The length of the stream.
        /// </summary>
        public long Length { get; init; }

        /// <summary>
        /// The file info of the stream.
        /// </summary>
        public FileInfo? FileInfo { get; init; }

    }

    /// <summary>
    /// The class of the checksum using SHA algorithm.
    /// </summary>
    public sealed class Checksum(SHAAlgorithm shaAlgorithm)
    {
        /// <summary>
        /// 64KB is the default buffer size.
        /// </summary>
        private const int DefaultBufferSize = 1 << 16;

        /// <summary>
        /// The type of hash algorithm.
        /// </summary>
        public SHAAlgorithm Algorithm => shaAlgorithm;

        #region GetFileHashAsync

        /// <summary>
        /// Get the hash of file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="progress">The progress reporter.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash string of the file.</returns>
        public async Task<string> GetFileHashAsync(
            string filePath,
            BinaryFormatting shaFormatting,
            int bufferSize = Checksum.DefaultBufferSize,
            IProgress<ChecksumEventArg>? progress = null,
            CancellationToken cancellationToken = default)
        {
            var hash = await this.GetFileHashAsync(filePath, bufferSize, progress, cancellationToken).ConfigureAwait(false);
            return Checksum.FormatHash(hash, shaFormatting);
        }

        /// <summary>
        /// Get the hash of file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="progress">The progress reporter.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]> GetFileHashAsync(
            string filePath,
            int bufferSize = Checksum.DefaultBufferSize,
            IProgress<ChecksumEventArg>? progress = null,
            CancellationToken cancellationToken = default)
        {
            var fileInfo = new FileInfo(filePath);
            progress?.Report(new ChecksumEventArg { FileInfo = fileInfo, Length = fileInfo.Length });

            await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true);
            return await this.GetStreamHashAsync(fileStream, bufferSize, progress, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region GetBinaryHashAsync

        /// <summary>
        /// Get the hash of binary.
        /// </summary>
        /// <param name="binary">The binary to compute.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <returns>The hash string of the binary.</returns>
        public string GetBinaryHash(byte[] binary, BinaryFormatting shaFormatting)
        {
            ArgumentNullException.ThrowIfNull(binary);
            var hash = this.GetBinaryHash(binary);
            return Checksum.FormatHash(hash, shaFormatting);
        }

        /// <summary>
        /// Get the hash of binary.
        /// </summary>
        /// <param name="binary">The binary to compute.</param>
        /// <returns>The hash binary of the input.</returns>
        public byte[] GetBinaryHash(byte[] binary)
        {
            ArgumentNullException.ThrowIfNull(binary);
            using var sha = this.CreateHasher();
            return sha.ComputeHash(binary);
        }

        #endregion

        #region GetObjectHashAsync

        /// <summary>
        /// Get the hash of object. 
        /// </summary>
        /// <param name="obj">The object to compute.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <param name="jsonSerializerOptions">The Options for Json Serializer.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash string of the file.</returns>
        public async Task<string> GetObjectHashAsync<T>(
            T obj,
            BinaryFormatting shaFormatting,
            JsonSerializerOptions? jsonSerializerOptions = null,
            int bufferSize = Checksum.DefaultBufferSize,
            CancellationToken cancellationToken = default) where T : class
        {
            var hash = await this.GetObjectHashAsync(obj, jsonSerializerOptions, bufferSize, cancellationToken).ConfigureAwait(false);
            return Checksum.FormatHash(hash, shaFormatting);
        }

        /// <summary>
        /// Get the hash of object. 
        /// </summary>
        /// <param name="obj">The object to compute.</param>
        /// <param name="jsonSerializerOptions">The Options for Json Serializer.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]> GetObjectHashAsync<T>(
            T obj,
            JsonSerializerOptions? jsonSerializerOptions = null,
            int bufferSize = Checksum.DefaultBufferSize,
            CancellationToken cancellationToken = default) where T : class
        {
            ArgumentNullException.ThrowIfNull(obj);
            await using var jsonStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(jsonStream, obj, jsonSerializerOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
            jsonStream.Position = 0;
            return await this.GetStreamHashAsync(jsonStream, bufferSize, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region GetStreamHashAsync

        /// <summary>
        /// Get the hash of stream. 
        /// </summary>
        /// <param name="stream">The stream to compute. The method will read from the stream's current position.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="progress">The progress reporter.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash string of the file.</returns>
        public async Task<string> GetStreamHashAsync(
            Stream stream,
            BinaryFormatting shaFormatting,
            int bufferSize = Checksum.DefaultBufferSize,
            IProgress<ChecksumEventArg>? progress = null,
            CancellationToken cancellationToken = default)
        {
            var hash = await this.GetStreamHashAsync(stream, bufferSize, progress, cancellationToken).ConfigureAwait(false);
            return Checksum.FormatHash(hash, shaFormatting);
        }

        /// <summary>
        /// Get the hash of stream. 
        /// </summary>
        /// <param name="stream">The stream to compute. The method will read from the stream's current position.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="progress">The progress reporter.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]> GetStreamHashAsync(
            Stream stream, 
            int bufferSize = Checksum.DefaultBufferSize, 
            IProgress<ChecksumEventArg>? progress = null, 
            CancellationToken cancellationToken = default)
        {
            using var sha = this.CreateHasher();
            var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            try
            {
                long totalBytesRead = 0;
                var streamLength = stream.CanSeek ? stream.Length : -1;
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false)) > 0)
                {
                    sha.TransformBlock(buffer, 0, bytesRead, null, 0);
                    totalBytesRead += bytesRead;
                    progress?.Report(new ChecksumEventArg { Position = totalBytesRead, Length = streamLength });
                }

                sha.TransformFinalBlock([], 0, 0);
                return sha.Hash!;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Create the hasher instance.
        /// </summary>
        /// <returns>Hash Algorithm instance.</returns>
        private HashAlgorithm CreateHasher() => this.Algorithm switch
        {
            SHAAlgorithm.SHA256 => SHA256.Create(),
            SHAAlgorithm.SHA384 => SHA384.Create(),
            SHAAlgorithm.SHA512 => SHA512.Create(),
            _ => throw new ArgumentOutOfRangeException(nameof(Algorithm), "Unsupported Algorithm.")
        };

        /// <summary>
        /// Format the hash to string.
        /// </summary>
        /// <param name="hash">Binary</param>
        /// <param name="formatting">Format</param>
        /// <returns>Formatted hash string.</returns>
        private static string FormatHash(byte[] hash, BinaryFormatting formatting) => formatting switch
        {
            BinaryFormatting.BASE64 => Convert.ToBase64String(hash),
            BinaryFormatting.HEXADECIMAL => Convert.ToHexString(hash),
            BinaryFormatting.BINARY => throw new NotSupportedException("Binary format is not supported for a string representation."),
            _ => throw new ArgumentOutOfRangeException(nameof(formatting), "Unsupported string format.")
        };

        #endregion
    }
}
