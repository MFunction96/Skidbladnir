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
    /// <summary>
    /// The SHA family.
    /// </summary>
    public enum SHAAlgorithm
    {
        SHA1,
        SHA256,
        SHA512
    }

    /// <summary>
    /// The format of exportation for SHA.
    /// </summary>
    public enum SHAFormatting
    {
        Base64,
        Binary,
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
        /// When the stream position updates, it will raise this event and pass the current status on reading.
        /// </summary>
        public event EventHandler<ChecksumEventArg> CalculatePositionEvent;

        /// <summary>
        /// When the meeting 
        /// </summary>
        public event EventHandler<ChecksumEventArg> FileInfoEvent;

        /// <summary>
        /// 
        /// </summary>
        private SHAAlgorithm Algorithm { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shaAlgorithm"></param>
        public Checksum(SHAAlgorithm shaAlgorithm)
        {
            Algorithm = shaAlgorithm;
        }

        #region GetFileHashAsync

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="shaFormatting"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetFileHashAsync(string filePath, SHAFormatting shaFormatting, CancellationToken cancellationToken)
        {
            return await this.GetFileHashAsync(filePath, shaFormatting, DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="shaFormatting"></param>
        /// <param name="bufferSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetFileHashAsync(string filePath, SHAFormatting shaFormatting, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            var hash = await this.GetFileHashAsync(filePath, bufferSize, cancellationToken);
            return shaFormatting == SHAFormatting.Base64 ?
                Convert.ToBase64String(hash) :
                hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public async Task<byte[]> GetFileHashAsync(string filePath, CancellationToken cancellationToken, int bufferSize = DefaultBufferSize)
        {
            return await this.GetFileHashAsync(filePath, bufferSize, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bufferSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<byte[]> GetFileHashAsync(string filePath, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            if (!System.IO.File.Exists(filePath)) throw new FileNotFoundException(filePath);
            this.FileInfoEvent?.Invoke(this, new ChecksumEventArg { FileInfo = new FileInfo(filePath) });
            await using var fs = System.IO.File.OpenRead(filePath);
            await using var bs = new BufferedStream(fs, bufferSize);
            return await this.GetStreamHashAsync(bs, bufferSize, cancellationToken);
        }

        #endregion

        #region GetBinaryHashAsync

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binary"></param>
        /// <param name="shaFormatting"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetBinaryHashAsync(byte[] binary, SHAFormatting shaFormatting, CancellationToken cancellationToken)
        {
            return await this.GetBinaryHashAsync(binary, shaFormatting, Checksum.DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binary"></param>
        /// <param name="shaFormatting"></param>
        /// <param name="bufferSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetBinaryHashAsync(byte[] binary, SHAFormatting shaFormatting, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            var hash = await this.GetBinaryHashAsync(binary, bufferSize, cancellationToken);
            return shaFormatting == SHAFormatting.Base64 ?
                Convert.ToBase64String(hash) :
                hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binary"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<byte[]> GetBinaryHashAsync(byte[] binary, CancellationToken cancellationToken)
        {
            return await this.GetStreamHashAsync(new MemoryStream(binary), DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binary"></param>
        /// <param name="bufferSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<byte[]> GetBinaryHashAsync(byte[] binary, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            return await this.GetStreamHashAsync(new MemoryStream(binary), bufferSize, cancellationToken);
        }

        #endregion

        #region GetObjectHashAsync

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="shaFormatting"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetObjectHashAsync(object obj, SHAFormatting shaFormatting, CancellationToken cancellationToken)
        {
            return await this.GetObjectHashAsync(obj, shaFormatting, DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="shaFormatting"></param>
        /// <param name="bufferSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetObjectHashAsync(object obj, SHAFormatting shaFormatting, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            var hash = await GetObjectHashAsync(obj, bufferSize, cancellationToken);
            return shaFormatting == SHAFormatting.Base64 ?
                Convert.ToBase64String(hash) :
                hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<byte[]> GetObjectHashAsync(object obj, CancellationToken cancellationToken)
        {
            return await GetObjectHashAsync(obj, DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="bufferSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<byte[]> GetObjectHashAsync(object obj, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            if (obj == null) throw new ArgumentNullException(@"obj can not be null!");
            var binary = JsonSerializer.SerializeToUtf8Bytes(obj, new JsonSerializerOptions { IgnoreNullValues = true });
            return await this.GetBinaryHashAsync(binary, bufferSize, cancellationToken);
        }

        #endregion

        #region GetStreamHashAsync

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="shaFormatting"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetStreamHashAsync(Stream stream, SHAFormatting shaFormatting, CancellationToken cancellationToken)
        {
            return await this.GetStreamHashAsync(stream, shaFormatting, DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="shaFormatting"></param>
        /// <param name="bufferSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetStreamHashAsync(Stream stream, SHAFormatting shaFormatting, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            var hash = await this.GetStreamHashAsync(stream, bufferSize, cancellationToken);
            return shaFormatting == SHAFormatting.Base64 ?
                Convert.ToBase64String(hash) :
                hash.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<byte[]> GetStreamHashAsync(Stream stream, CancellationToken cancellationToken)
        {
            return await this.GetStreamHashAsync(stream, DefaultBufferSize, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bufferSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<byte[]> GetStreamHashAsync(Stream stream, int bufferSize = DefaultBufferSize, CancellationToken cancellationToken = default)
        {
            using HashAlgorithm sha = this.Algorithm switch
            {
                SHAAlgorithm.SHA1 => SHA1.Create(),
                SHAAlgorithm.SHA256 => SHA256.Create(),
                _ => SHA512.Create()
            };
            var buffer = new byte[bufferSize];
            var totalRead = 0L;

            while (stream.Length - totalRead >= bufferSize)
            {
                var readCount = await stream.ReadAsync(buffer.AsMemory(0, bufferSize), cancellationToken);
                if (readCount != bufferSize) throw new DataException(@"Read Count is not equal to expected!\r\n");
                totalRead += sha.TransformBlock(buffer, 0, bufferSize, null, 0);
                this.CalculatePositionEvent?.Invoke(this, new ChecksumEventArg { Position = totalRead, Length = stream.Length });
            }

            var finalSize = (int)(stream.Length - totalRead);
            var readCountFinal = await stream.ReadAsync(buffer.AsMemory(0, finalSize), cancellationToken);
            if (readCountFinal != finalSize) throw new DataException(@"Read Count is not equal to expected!\r\n");
            sha.TransformFinalBlock(buffer, 0, finalSize);
            return sha.Hash;
        }

        #endregion
    }
}
