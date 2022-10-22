using System;
using System.Runtime.InteropServices;
using Skidbladnir.Common.Interop.OpenSSL.Library;
// ReSharper disable InconsistentNaming

namespace Skidbladnir.Common.Interop.OpenSSL
{
    /// <summary>
    /// 
    /// </summary>
    internal class EVPMDCTX : UnmanagedClass
    {
        /// <summary>
        /// 
        /// </summary>
        private bool _disposed;

        private const string EVP_MD_CTX_new = "EVP_MD_CTX_new";

        private const string EVP_MD_CTX_free = "EVP_MD_CTX_free";

        [UnmanagedFunctionPointer(callingConvention: CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal delegate IntPtr EVPMDCTXNewFunc();

        [UnmanagedFunctionPointer(callingConvention: CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal delegate void EVPMDCTXFreeFunc(in IntPtr ctx);

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly EVPMDCTXNewFunc EVPMDCTXNew;

        private readonly EVPMDCTXFreeFunc EVPMDCTXFree;

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        internal EVPMDCTX(OpenSSLCryptoLib openSSLCryptoLib) : base()
        {
            this.EVPMDCTXNew = openSSLCryptoLib.ExportFunction<EVPMDCTXNewFunc>(EVPMDCTX.EVP_MD_CTX_new);
            this.EVPMDCTXFree = openSSLCryptoLib.ExportFunction<EVPMDCTXFreeFunc>(EVPMDCTX.EVP_MD_CTX_free);
            var ptr = this.EVPMDCTXNew();
            this._disposed = false;
            base.InitUnmanagedInstance(ptr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected override void ReleaseUnmanagedInstance()
        {
            base.ReleaseUnmanagedInstance();
            this.EVPMDCTXFree(base.InstancePointer);
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

    /// <summary>
    /// 
    /// </summary>
    internal static partial class NativeMethods
    {
        #region Windows

        // EVP_MD_CTX *EVP_MD_CTX_new(void);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport("mingw64/openssl/bin/libcrypto-3-x64.dll", EntryPoint = "EVP_MD_CTX_new", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr EVPMDCTXNewWindows();

        // void EVP_MD_CTX_free(EVP_MD_CTX *ctx);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        [DllImport("mingw64/openssl/bin/libcrypto-3-x64.dll", EntryPoint = "EVP_MD_CTX_free", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void EVPMDCTXFreeWindows(in IntPtr ctx);

        #endregion

        #region Linux

        // EVP_MD_CTX *EVP_MD_CTX_new(void);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport("linux-generic64/openssl/lib/libcrypto.so.3", EntryPoint = "EVP_MD_CTX_new", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr EVPMDCTXNewLinux();

        // void EVP_MD_CTX_free(EVP_MD_CTX *ctx);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        [DllImport("linux-generic64/openssl/lib/libcrypto.so.3", EntryPoint = "EVP_MD_CTX_free", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void EVPMDCTXFreeLinux(in IntPtr ctx);

        #endregion

    }
}
