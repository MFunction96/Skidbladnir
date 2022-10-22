using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Skidbladnir.Common.Interop
{
    internal abstract class UnmanagedLibrary : UnmanagedClass
    {
        private bool _disposed;

        private ISet<IntPtr> FuncPtrs { get; }

        private IList<GCHandle> GCHandles { get; }

        internal UnmanagedLibrary() : base()
        {
            this.GCHandles = new List<GCHandle>();
            this.FuncPtrs = new HashSet<IntPtr>();
            this._disposed = false;
        }

        internal virtual T ExportFunction<T>(string entryPoint)
        {
            var flag = NativeLibrary.TryGetExport(this.InstancePointer, entryPoint, out var ptr);
            if (!flag || ptr == IntPtr.Zero)
            {
                throw new ExternalException($"Unable to load {entryPoint}!");
            }

            if (!this.FuncPtrs.Contains(ptr))
            {
                this.FuncPtrs.Add(ptr);
                this.GCHandles.Add(GCHandle.FromIntPtr(ptr));
            }

            return Marshal.GetDelegateForFunctionPointer<T>(ptr);
        }

        protected override void ReleaseUnmanagedInstance()
        {
            foreach (var gcHandle in this.GCHandles)
            {
                gcHandle.Free();
            }

            this.GCHandles.Clear();
            base.ReleaseUnmanagedInstance();
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
