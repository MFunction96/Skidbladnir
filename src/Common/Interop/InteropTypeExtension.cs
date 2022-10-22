using System;

namespace Skidbladnir.Common.Interop
{
    public static class InteropTypeExtension
    {
        public static IntPtr ToIntPtr(this UIntPtr uIntPtr)
        {
            return new IntPtr((long)uIntPtr.ToUInt64());
        }
    }
}
