using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
        public FileInfo? FileInfo { get; set; }

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
        /// When the stream position updates, it will raise this event and pass the current status on reading.
        /// </summary>
        public event EventHandler<ChecksumEventArg>? CalculatePositionEvent;

        /// <summary>
        /// When the meeting 
        /// </summary>
        public event EventHandler<ChecksumEventArg>? FileInfoEvent;

        /// <summary>
        /// The type of hash algorithm.
        /// </summary>
        public SHAAlgorithm Algorithm { get; }

        /// <summary>
        /// The constructor for the checksum.
        /// </summary>
        /// <param name="shaAlgorithm"></param>
        public Checksum(SHAAlgorithm shaAlgorithm)
        {
            this.Algorithm = shaAlgorithm;
        }

        #region GetFileHashAsync

        /// <summary>
        /// Get the hash of file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash string of the file.</returns>
        public async Task<string> GetFileHashAsync(string filePath, BinaryFormatting shaFormatting, CancellationToken cancellationToken)
        {
            return await GetFileHashAsync(filePath, shaFormatting, Checksum.DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// Get the hash of file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash string of the file.</returns>
        public async Task<string> GetFileHashAsync(string filePath, BinaryFormatting shaFormatting, int bufferSize = Checksum.DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            var hash = await GetFileHashAsync(filePath, bufferSize, cancellationToken);
            if (hash is null)
            {
                return string.Empty;
            }

            return shaFormatting == BinaryFormatting.Base64
                ? Convert.ToBase64String(hash)
                : hash.ToHexadecimal();
        }

        /// <summary>
        /// Get the hash of file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]?> GetFileHashAsync(string filePath, CancellationToken cancellationToken, int bufferSize = Checksum.DefaultBufferSize)
        {
            return await this.GetFileHashAsync(filePath, bufferSize, cancellationToken);
        }

        /// <summary>
        /// Get the hash of file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]?> GetFileHashAsync(string filePath, int bufferSize = Checksum.DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            this.FileInfoEvent?.Invoke(this, new ChecksumEventArg { FileInfo = new FileInfo(filePath) });
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
        public async Task<string> GetBinaryHashAsync(byte[] binary, BinaryFormatting shaFormatting, CancellationToken cancellationToken)
        {
            return await this.GetBinaryHashAsync(binary, shaFormatting, Checksum.DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// Get the hash of binary.
        /// </summary>
        /// <param name="binary">The binary to compute.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash string of the file.</returns>
        public async Task<string> GetBinaryHashAsync(byte[] binary, BinaryFormatting shaFormatting, int bufferSize = Checksum.DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            var hash = await this.GetBinaryHashAsync(binary, bufferSize, cancellationToken);
            if (hash == null)
            {
                return string.Empty;
            }

            return shaFormatting == BinaryFormatting.Base64
                ? Convert.ToBase64String(hash)
                : hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        /// <summary>
        /// Get the hash of binary.
        /// </summary>
        /// <param name="binary">The binary to compute.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]?> GetBinaryHashAsync(byte[] binary, CancellationToken cancellationToken)
        {
            return await this.GetBinaryHashAsync(binary, Checksum.DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// Get the hash of binary.
        /// </summary>
        /// <param name="binary">The binary to compute.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns></returns>
        public Task<byte[]?> GetBinaryHashAsync(byte[] binary, int bufferSize = Checksum.DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var totalRead = 0L;
                using HashAlgorithm sha = this.Algorithm switch
                {
                    SHAAlgorithm.SHA1 => SHA1.Create(),
                    SHAAlgorithm.SHA256 => SHA256.Create(),
                    _ => SHA512.Create()
                };
                int pos;
                for (pos = 0; pos + bufferSize < binary.Length; pos += bufferSize)
                {
                    totalRead += sha.TransformBlock(binary, pos, bufferSize, null, 0);
                    this.CalculatePositionEvent?.Invoke(this, new ChecksumEventArg { Position = totalRead, Length = binary.Length });
                }

                var finalSize = (int)(binary.Length - totalRead);
                if (finalSize > 0)
                {
                    sha.TransformFinalBlock(binary, pos, finalSize);
                }

                return sha.Hash;
            }, cancellationToken);
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
        public async Task<string> GetObjectHashAsync(object obj, BinaryFormatting shaFormatting, CancellationToken cancellationToken)
        {
            return await this.GetObjectHashAsync(obj, shaFormatting, Checksum.DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// Get the hash of object. 
        /// </summary>
        /// <param name="obj">The object to compute.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash string of the file.</returns>
        public async Task<string> GetObjectHashAsync(object obj, BinaryFormatting shaFormatting, int bufferSize = Checksum.DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            var hash = await this.GetObjectHashAsync(obj, bufferSize, cancellationToken);
            if (hash == null)
            {
                return string.Empty;
            }

            return shaFormatting == BinaryFormatting.Base64
                ? Convert.ToBase64String(hash)
                : hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        /// <summary>
        /// Get the hash of object. 
        /// </summary>
        /// <param name="obj">The object to compute.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]?> GetObjectHashAsync(object obj, CancellationToken cancellationToken)
        {
            return await this.GetObjectHashAsync(obj, Checksum.DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// Get the hash of object. 
        /// </summary>
        /// <param name="obj">The object to compute.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]?> GetObjectHashAsync(object obj, int bufferSize = Checksum.DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            switch (obj)
            {
                case null:
                    throw new ArgumentNullException(nameof(obj));
                case string objStr:
                    {
                        var binary = Encoding.UTF8.GetBytes(objStr);
                        return await this.GetBinaryHashAsync(binary, bufferSize, cancellationToken);
                    }
                default:
                    {
                        return await this.GetStreamHashAsync(obj.ToJsonStream(), bufferSize, cancellationToken);
                    }
            }
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
        public async Task<string> GetStreamHashAsync(Stream stream, BinaryFormatting shaFormatting, CancellationToken cancellationToken)
        {
            return await this.GetStreamHashAsync(stream, shaFormatting, Checksum.DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// Get the hash of stream. 
        /// </summary>
        /// <param name="stream">The stream to compute.</param>
        /// <param name="shaFormatting">The format of hash.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash string of the file.</returns>
        public async Task<string> GetStreamHashAsync(Stream stream, BinaryFormatting shaFormatting, int bufferSize = Checksum.DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            var hash = await GetStreamHashAsync(stream, bufferSize, cancellationToken);
            if (hash is null)
            {
                return string.Empty;
            }

            return shaFormatting == BinaryFormatting.Base64
                ? Convert.ToBase64String(hash)
                : hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        /// <summary>
        /// Get the hash of stream. 
        /// </summary>
        /// <param name="stream">The stream to compute.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]?> GetStreamHashAsync(Stream stream, CancellationToken cancellationToken)
        {
            return await GetStreamHashAsync(stream, Checksum.DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// Get the hash of stream. 
        /// </summary>
        /// <param name="stream">The stream to compute.</param>
        /// <param name="bufferSize">The size of buffer.</param>
        /// <param name="cancellationToken">The token for cancellation.</param>
        /// <returns>The hash binary of the file.</returns>
        public async Task<byte[]?> GetStreamHashAsync(Stream stream, int bufferSize = Checksum.DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            using HashAlgorithm sha = this.Algorithm switch
            {
                SHAAlgorithm.SHA1 => SHA1.Create(),
                SHAAlgorithm.SHA256 => SHA256.Create(),
                _ => SHA512.Create()
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
                this.CalculatePositionEvent?.Invoke(this, new ChecksumEventArg { Position = totalRead, Length = bufferedStream.Length });
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
