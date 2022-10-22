// ReSharper disable InconsistentNaming

namespace Skidbladnir.Common.Interop.OpenSSL.Component
{
    public enum EVPDigest : uint
    {
        BLAKE2B512 = 0,
        BLAKE2S256 = 1,
        MD4 = 2,
        MD5 = 3,
        MDC2 = 4,
        RMD160 = 5,
        SHA1 = 6,
        SHA224 = 7,
        SHA256 = 8,
        SHA3_224 = 9,
        SHA3_256 = 10,
        SHA3_384 = 11,
        SHA3_512 = 12,
        SHA384 = 13,
        SHA512 = 14,
        SHA512_224 = 15,
        SHA512_256 = 16,
        SHAKE128 = 17,
        SHAKE256 = 18,
        SM3 = 19
    }
}
