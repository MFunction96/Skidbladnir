using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xanadu.Skidbladnir.Core.Extension;
using Xanadu.Skidbladnir.IO.File;

namespace Xanadu.Skidbladnir.Test.IO.File
{
    [TestClass]
    public class ChecksumTests
    {
        private const string TestContent = "The quick brown fox jumps over the lazy dog";
        private static readonly byte[] TestContentBytes = Encoding.UTF8.GetBytes(TestContent);

        // Expected hashes for "The quick brown fox jumps over the lazy dog"
        private const string Sha256Hex = "d7a8fbb307d7809469ca9abcb0082e4f8d5651e46d3cdb762d02d0bf37c9e592";
        private const string Sha384Hex = "ca737f1014a48f4c0b6dd43cb177b0afd9e5169367544c494011e3317dbf9a509cb1e5dc1e85a941bbee3d7f2afbc9b1";
        private const string Sha512Hex = "07e547d9586f6a73f73fbac0435ed76951218fb7d0c8d788a309d785436bbb642e93a252a954f23912547d1e8a3b5ed6e1bfd7097821233fa0538f3db854fee6";

        private const string Sha256Base64 = "16j7swfXgJRpypq8sAguT41WUeRtPNt2LQLQvzfJ5ZI=";
        private const string Sha384Base64 = "ynN/EBSkj0wLbdQ8sXewr9nlFpNnVExJQBHjMX2/mlCcseXcHoWpQbvuPX8q+8mx";
        private const string Sha512Base64 = "B+VH2VhvanP3P7rAQ17XaVEhj7fQyNeIownXhUNru2Quk6JSqVTyORJUfR6KO17W4b/XCXghIz+gU489uFT+5g==";

        private static string GetTempFilePath() => Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        [TestMethod]
        [DataRow(SHAAlgorithm.SHA256, Sha256Hex)]
        [DataRow(SHAAlgorithm.SHA384, Sha384Hex)]
        [DataRow(SHAAlgorithm.SHA512, Sha512Hex)]
        public void GetBinaryHash_WithHexFormatting_ReturnsCorrectHash(SHAAlgorithm algorithm, string expectedHash)
        {
            var checksum = new Checksum(algorithm);
            var hash = checksum.GetBinaryHash(TestContentBytes, BinaryFormatting.HEXADECIMAL);
            Assert.AreEqual(expectedHash, hash, ignoreCase: true);
        }

        [TestMethod]
        [DataRow(SHAAlgorithm.SHA256, Sha256Base64)]
        [DataRow(SHAAlgorithm.SHA384, Sha384Base64)]
        [DataRow(SHAAlgorithm.SHA512, Sha512Base64)]
        public void GetBinaryHash_WithBase64Formatting_ReturnsCorrectHash(SHAAlgorithm algorithm, string expectedHash)
        {
            var checksum = new Checksum(algorithm);
            var hash = checksum.GetBinaryHash(TestContentBytes, BinaryFormatting.BASE64);
            Assert.AreEqual(expectedHash, hash);
        }

        [TestMethod]
        public void GetBinaryHash_WithBinaryFormatting_ThrowsNotSupportedException()
        {
            var checksum = new Checksum(SHAAlgorithm.SHA256);
            Assert.ThrowsExactly<NotSupportedException>(() => checksum.GetBinaryHash(TestContentBytes, BinaryFormatting.BINARY));
        }

        [TestMethod]
        [DataRow(SHAAlgorithm.SHA256, Sha256Hex)]
        [DataRow(SHAAlgorithm.SHA384, Sha384Hex)]
        [DataRow(SHAAlgorithm.SHA512, Sha512Hex)]
        public async Task GetFileHashAsync_WithHexFormatting_ReturnsCorrectHash(SHAAlgorithm algorithm, string expectedHash)
        {
            var filePath = GetTempFilePath();
            await System.IO.File.WriteAllBytesAsync(filePath, TestContentBytes);
            try
            {
                var checksum = new Checksum(algorithm);
                var hash = await checksum.GetFileHashAsync(filePath, BinaryFormatting.HEXADECIMAL);
                Assert.AreEqual(expectedHash, hash, ignoreCase: true);
            }
            finally
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }

        [TestMethod]
        [DataRow(SHAAlgorithm.SHA256, Sha256Hex)]
        [DataRow(SHAAlgorithm.SHA384, Sha384Hex)]
        [DataRow(SHAAlgorithm.SHA512, Sha512Hex)]
        public async Task GetStreamHashAsync_WithHexFormatting_ReturnsCorrectHash(SHAAlgorithm algorithm, string expectedHash)
        {
            var checksum = new Checksum(algorithm);
            using var stream = new MemoryStream(TestContentBytes);
            var hash = await checksum.GetStreamHashAsync(stream, BinaryFormatting.HEXADECIMAL);
            Assert.AreEqual(expectedHash, hash, ignoreCase: true);
        }

        [TestMethod]
        [DataRow(SHAAlgorithm.SHA256, Sha256Hex)]
        [DataRow(SHAAlgorithm.SHA384, Sha384Hex)]
        [DataRow(SHAAlgorithm.SHA512, Sha512Hex)]
        public async Task GetObjectHashAsync_WithHexFormatting_ReturnsCorrectHash(SHAAlgorithm algorithm, string expectedHash)
        {
            var testObject = new { Message = TestContent };
            var tempPath = GetTempFilePath();
            await System.IO.File.WriteAllBytesAsync(tempPath, JsonSerializer.SerializeToUtf8Bytes(testObject));
            var checksum = new Checksum(algorithm);

            // Hash of {"Message":"The quick brown fox jumps over the lazy dog"}
            var objectExpectedHash = algorithm switch
            {
                SHAAlgorithm.SHA256 => "75af3d9029149786e689cd17004454c2fd805b75d6c22cb62988f6aeeb83ff04",
                SHAAlgorithm.SHA384 => "42c0efce1ca4b63a94ab6035f9346b3eaca7c3b1279f7d9649f1ba90a5c723471f0d499732df9e51a1192bf85adf8020",
                SHAAlgorithm.SHA512 => "57334f21cf23f0a30bf9d6d480a523735b39f3ac623b5337070a2add531f12a849801f2d3122ef52f4cf0250ced423ae2974d49457ee502b0836e18fcba3dbd9",
                _ => throw new ArgumentOutOfRangeException(nameof(algorithm))
            };

            var hash = await checksum.GetObjectHashAsync(testObject, BinaryFormatting.HEXADECIMAL);
            Assert.AreEqual(objectExpectedHash, hash, ignoreCase: true);
        }

        [TestMethod]
        public void GetBinaryHash_WithNullBinary_ThrowsArgumentNullException()
        {
            var checksum = new Checksum(SHAAlgorithm.SHA256);
            Assert.ThrowsExactly<ArgumentNullException>(() => checksum.GetBinaryHash(null!));
        }

        [TestMethod]
        public async Task GetObjectHashAsync_WithNullObject_ThrowsArgumentNullException()
        {
            var checksum = new Checksum(SHAAlgorithm.SHA256);
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () => await checksum.GetObjectHashAsync<object>(null!));
        }
    }
}