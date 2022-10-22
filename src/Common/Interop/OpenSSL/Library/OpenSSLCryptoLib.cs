using System;
using System.Runtime.InteropServices;

namespace Skidbladnir.Common.Interop.OpenSSL.Library
{
    internal sealed class OpenSSLCryptoLib : UnmanagedLibrary
    {
        private bool _disposed;

        private static string LibPath { get; }

        internal static bool Loaded { get; private set; }

        static OpenSSLCryptoLib()
        {
            Loaded = false;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                LibPath = "mingw64/openssl/bin/libcrypto-3-x64.dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                LibPath = "linux-generic64/openssl/lib/libcrypto.so.3";
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal OpenSSLCryptoLib()
        {
            if (Loaded)
            {
                throw new COMException($"{LibPath} is loaded");
            }

            var flag = NativeLibrary.TryLoad(LibPath, out var ptr);
            if (!flag || ptr == IntPtr.Zero)
            {
                throw new ExternalException($"Unable to load {LibPath}");
            }

            _disposed = false;
            InitUnmanagedInstance(ptr);
            Loaded = true;
        }

        protected override void ReleaseUnmanagedInstance()
        {
            base.ReleaseUnmanagedInstance();
            NativeLibrary.Free(InstancePointer);
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
                Loaded = false;
            }

            // Free any unmanaged objects here.
            //

            _disposed = true;
            // Call base class implementation.
            base.Dispose(disposing);
        }
    }
}
