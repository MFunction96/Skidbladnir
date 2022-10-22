using System;
using System.Runtime.InteropServices;
using Skidbladnir.Common.Interop.OpenSSL.Library;
// ReSharper disable InconsistentNaming

namespace Skidbladnir.Common.Interop.OpenSSL.Component
{
    public sealed class EVPMD : UnmanagedClass
    {
        private bool _disposed;

        private const string EVP_get_digestbyname = "EVP_get_digestbyname";

        private const string EVP_MD_free = "EVP_MD_free";

        [UnmanagedFunctionPointer(callingConvention: CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal delegate IntPtr EVPGetDigestByNameFunc(in string name);

        [UnmanagedFunctionPointer(callingConvention: CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal delegate void EVPMDFreeFunc(in IntPtr md);

        internal static string[] Digests => new[]
        {
            "blake2b512",
            "blake2s256",
            "md4",
            "md5",
            "mdc2",
            "rmd160",
            "sha1",
            "sha224",
            "sha256",
            "sha3-224",
            "sha3-256",
            "sha3-384",
            "sha3-512",
            "sha384",
            "sha512",
            "sha512-224",
            "sha512-256",
            "shake128",
            "shake256",
            "sm3"
        };

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        // ReSharper disable once InconsistentNaming
        private readonly EVPGetDigestByNameFunc EVPGetDigestByName;

        // ReSharper disable once InconsistentNaming
        private readonly EVPMDFreeFunc EVPMDFree;

        internal EVPMD(OpenSSLCryptoLib openSSLCryptoLib, EVPDigest digest) : base()
        {
            EVPGetDigestByName = openSSLCryptoLib.ExportFunction<EVPGetDigestByNameFunc>(EVP_get_digestbyname);
            EVPMDFree = openSSLCryptoLib.ExportFunction<EVPMDFreeFunc>(EVP_MD_free);
            var ptr = EVPGetDigestByName(Digests[(uint)digest]);
            _disposed = false;
            InitUnmanagedInstance(ptr);
        }

        protected override void ReleaseUnmanagedInstance()
        {
            base.ReleaseUnmanagedInstance();
            EVPMDFree(InstancePointer);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">The type of disposing.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //

            _disposed = true;
            // Call base class implementation.
            base.Dispose(disposing);
        }
    }
}
