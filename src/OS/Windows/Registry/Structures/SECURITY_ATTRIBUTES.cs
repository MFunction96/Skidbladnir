using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Xanadu.Skidbladnir.OS.Windows.Registry.Structures
{
    /// <summary>
    /// 安全标识符结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public IntPtr lpSecurityDescriptor;
        public bool bInheritHandle;
    }
}
