using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Skidbladnir.Common.Interop.OpenSSL.Library;

// ReSharper disable InconsistentNaming

namespace Skidbladnir.Common.Interop.OpenSSL.Component
{
    internal class Crypto : UnmanagedClass
    {
        private bool _disposed;

        private const string EVP_DigestInit_ex2 = "EVP_DigestInit_ex2";

        private const string EVP_DigestUpdate = "EVP_DigestUpdate";

        private const string EVP_DigestFinal_ex = "EVP_DigestFinal_ex";

        [UnmanagedFunctionPointer(callingConvention: CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal delegate int EVPDigestInitEx2Func(in IntPtr ctx, in IntPtr type, in IntPtr parmas);

        [UnmanagedFunctionPointer(callingConvention: CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal delegate int EVPDigestUpdateFunc(in IntPtr ctx, in IntPtr d, in UIntPtr cnt);

        [UnmanagedFunctionPointer(callingConvention: CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal delegate int EVPDigestFinalExFunc(in IntPtr ctx, out IntPtr md, out uint s);

        private readonly EVPDigestInitEx2Func EVPDigestInitEx2;
        private readonly EVPDigestUpdateFunc EVPDigestUpdate;
        private readonly EVPDigestFinalExFunc EVPDigestFinalEx;

        internal Crypto(OpenSSLCryptoLib openSSLCryptoLib, EVPDigest digest) : base()
        {
            this.EVPDigestInitEx2 = openSSLCryptoLib.ExportFunction<EVPDigestInitEx2Func>(Crypto.EVP_DigestInit_ex2);
            this.EVPDigestUpdate = openSSLCryptoLib.ExportFunction<EVPDigestUpdateFunc>(Crypto.EVP_DigestUpdate);
            this.EVPDigestFinalEx = openSSLCryptoLib.ExportFunction<EVPDigestFinalExFunc>(Crypto.EVP_DigestFinal_ex);
            this._disposed = false;
        }

        internal int EVPDigestInitEx2Wrapper(EVPMDCTX ctx, EVPMD type, IEnumerable<string> parmas = null)
        {
            var ptr = IntPtr.Zero;
            var arr = parmas as string[] ?? parmas?.ToArray() ?? Array.Empty<string>();
            if (arr.Any())
            {
                throw new NotImplementedException();
            }

            return this.EVPDigestInitEx2(ctx.InstancePointer, type.InstancePointer, ptr);
        }

        internal int EVPDigestUpdateWrapper(EVPMDCTX ctx, byte[] d, UIntPtr cnt)
        {
            var ptr = Marshal.AllocHGlobal(cnt.ToIntPtr());
            Marshal.Copy(d, 0, ptr, d.Length);
            return this.EVPDigestUpdate(ctx.InstancePointer, ptr, cnt);
        }

        internal int EVPDigestFinalExWrapper(EVPMDCTX ctx, out byte[] md, out uint s)
        {
            var rv = this.EVPDigestFinalEx(ctx.InstancePointer, out var mdPtr, out s);
            md = new byte[s];
            Marshal.Copy(mdPtr, md, 0, (int)s);
            return rv;
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">The type of disposing.</param>
        protected override void Dispose(bool disposing)
        {
            if (this._disposed) return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //

            this._disposed = true;
            // Call base class implementation.
            base.Dispose(disposing);
        }
    }
}
