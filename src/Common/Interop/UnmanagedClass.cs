using System;
using System.Runtime.InteropServices;

namespace Skidbladnir.Common.Interop
{
    public abstract class UnmanagedClass : IDisposable
    {
        private bool _disposed;

        private GCHandle _gcHandle;

        internal IntPtr InstancePointer { get; private set; }

        protected UnmanagedClass()
        {
            this._disposed = false;
        }

        protected virtual void InitUnmanagedInstance(IntPtr instancePointer)
        {
            this.InstancePointer = instancePointer;
            this._gcHandle = GCHandle.FromIntPtr(instancePointer);
        }

        protected virtual void ReleaseUnmanagedInstance()
        {
            this._gcHandle.Free();
        }

        /// <summary>
        /// Implement IDisposable. Do not make this method virtual. A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// </summary>
        /// <param name="disposing">If disposing equals true, the method has been called directly, or indirectly by a user's code. Managed and unmanaged resources can be disposed. If disposing equals false, the method has been called by the runtime from inside the finalizer and you should not reference other objects. Only unmanaged resources can be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (this._disposed)
            {
                return;
            }

            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources.
                this.ReleaseUnmanagedInstance();
            }
            // Call the appropriate methods to clean up
            // unmanaged resources here.
            // If disposing is false,
            // only the following code is executed.

            this._disposed = true;
        }

        /// <summary>
        /// The finalize method.
        /// </summary>
        ~UnmanagedClass()
        {
            this.Dispose(false);
        }
    }
}
