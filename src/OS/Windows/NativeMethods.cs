using System;
using System.Runtime.InteropServices;
using System.Text;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Xanadu.Skidbladnir.OS.Windows
{
    /// <summary>
    /// 平台调用方法访问非托管代码方法
    /// </summary>
    internal static class NativeMethods
    {
        #region Windows

        /// <summary>
        /// 写入注册表数据。详情参阅：https://msdn.microsoft.com/en-us/library/ms724923(VS.85).aspx。
        /// </summary>
        /// <param name="hKey">
        /// 已打开注册表句柄。
        /// </param>
        /// <param name="lpValueName">
        /// 注册表键名。
        /// </param>
        /// <param name="lpReserved">
        /// 保留参数，必须为0。
        /// </param>
        /// <param name="dwType">
        /// 注册表键值类型。
        /// </param>
        /// <param name="lpData">
        /// 注册表键值。
        /// </param>
        /// <param name="lpcbData">
        /// 注册表键值占用空间大小。
        /// </param>
        /// <returns>
        /// Windows错误代码，详情参阅MSDN。
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RegSetValueExW")]
        public static extern int RegSetValueEx(
            IntPtr hKey,
            [MarshalAs(UnmanagedType.LPWStr)] string lpValueName,
            int lpReserved,
            int dwType,
            [MarshalAs(UnmanagedType.LPWStr)] string lpData,
            int lpcbData);

        /// <summary>
        /// 写入注册表数据。详情参阅：https://msdn.microsoft.com/en-us/library/ms724923(VS.85).aspx。
        /// </summary>
        /// <param name="hKey">
        /// 已打开注册表句柄。
        /// </param>
        /// <param name="lpValueName">
        /// 注册表键名。
        /// </param>
        /// <param name="lpReserved">
        /// 保留参数，必须为0。
        /// </param>
        /// <param name="dwType">
        /// 注册表键值类型。
        /// </param>
        /// <param name="lpData">
        /// 注册表键值。
        /// </param>
        /// <param name="lpcbData">
        /// 注册表键值占用空间大小。
        /// </param>
        /// <returns>
        /// Windows错误代码，详情参阅MSDN。
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RegSetValueExW")]
        public static extern int RegSetValueEx(
            IntPtr hKey,
            [MarshalAs(UnmanagedType.LPWStr)] string lpValueName,
            int lpReserved,
            int dwType,
            IntPtr lpData,
            int lpcbData);

        /// <summary>
        /// 写入注册表数据。详情参阅：https://msdn.microsoft.com/en-us/library/ms724923(VS.85).aspx。
        /// </summary>
        /// <param name="hKey">
        /// 已打开注册表句柄。
        /// </param>
        /// <param name="lpValueName">
        /// 注册表键名。
        /// </param>
        /// <param name="lpReserved">
        /// 保留参数，必须为0。
        /// </param>
        /// <param name="dwType">
        /// 注册表键值类型。
        /// </param>
        /// <param name="lpData">
        /// 注册表键值。
        /// </param>
        /// <param name="lpcbData">
        /// 注册表键值占用空间大小。
        /// </param>
        /// <returns>
        /// Windows错误代码，详情参阅MSDN。
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RegSetValueExW")]
        public static extern int RegSetValueEx(
            IntPtr hKey,
            [MarshalAs(UnmanagedType.LPWStr)] string lpValueName,
            int lpReserved,
            int dwType,
            byte[] lpData,
            int lpcbData);

        /// <summary>
        /// 获取注册表键值信息。详情参阅：https://msdn.microsoft.com/en-us/library/ms724911(v=vs.85).aspx。
        /// </summary>
        /// <param name="hKey">
        /// 已打开的注册表句柄。
        /// </param>
        /// <param name="lpValueName">
        /// 注册表键名。
        /// </param>
        /// <param name="lpReserved">
        /// 保留参数，必须为0。
        /// </param>
        /// <param name="lpType">
        /// 注册表键值类型。
        /// </param>
        /// <param name="lpData">
        /// 注册表键值。
        /// </param>
        /// <param name="lpcbData">
        /// 注册表键值占用空间大小。
        /// </param>
        /// <returns>
        /// Windows错误代码，详情参阅MSDN。
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW")]
        public static extern int RegQueryValueEx(
            IntPtr hKey,
            [MarshalAs(UnmanagedType.LPWStr)] string lpValueName,
            IntPtr lpReserved,
            out int lpType,
            IntPtr lpData,
            ref int lpcbData);

        /// <summary>
        /// 获取注册表键值信息。详情参阅：https://msdn.microsoft.com/en-us/library/ms724911(v=vs.85).aspx。
        /// </summary>
        /// <param name="hKey">
        /// 已打开的注册表句柄。
        /// </param>
        /// <param name="lpValueName">
        /// 注册表键名。
        /// </param>
        /// <param name="lpReserved">
        /// 保留参数，必须为0。
        /// </param>
        /// <param name="lpType">
        /// 注册表键值类型。
        /// </param>
        /// <param name="lpData">
        /// 注册表键值。
        /// </param>
        /// <param name="lpcbData">
        /// 注册表键值占用空间大小。
        /// </param>
        /// <returns>
        /// Windows错误代码，详情参阅MSDN。
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW")]
        public static extern int RegQueryValueEx(
            IntPtr hKey,
            [MarshalAs(UnmanagedType.LPWStr)] string lpValueName,
            IntPtr lpReserved,
            out int lpType,
            StringBuilder lpData,
            ref int lpcbData);

        /// <summary>
        /// 创建并打开注册表键。详情参阅：https://msdn.microsoft.com/en-us/library/ms724844(v=vs.85).aspx。
        /// </summary>
        /// <param name="hKey">
        /// 注册表根键。
        /// </param>
        /// <param name="lpSubKey">
        /// 注册表子键。
        /// </param>
        /// <param name="lpReserved">
        /// 保留参数，必须为0。
        /// </param>
        /// <param name="lpClass">
        /// 用户定义注册表类。
        /// </param>
        /// <param name="dwOptions">
        /// 注册表创建选项。
        /// </param>
        /// <param name="samDesired">
        /// 注册表访问权限。
        /// </param>
        /// <param name="lpSecurityAttributes">
        /// 注册表安全访问标识符。
        /// </param>
        /// <param name="phkResult">
        /// 注册表键句柄。
        /// </param>
        /// <param name="lpdwDisposition">
        /// 注册表操作类型。
        /// </param>
        /// <returns>
        /// Windows错误代码，详情参阅MSDN。
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RegCreateKeyExW")]
        public static extern int RegCreateKeyEx(
            IntPtr hKey,
            [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey,
            int lpReserved,
            [MarshalAs(UnmanagedType.LPWStr)] string lpClass,
            int dwOptions,
            int samDesired,
            IntPtr lpSecurityAttributes,
            out IntPtr phkResult,
            out int lpdwDisposition);

        /// <summary>
        /// 打开注册表键。详情参阅：https://msdn.microsoft.com/en-us/library/ms724897(v=vs.85).aspx。
        /// </summary>
        /// <param name="hKey">
        /// 注册表根键。
        /// </param>
        /// <param name="lpSubKey">
        /// 注册表子键。
        /// </param>
        /// <param name="ulOptions">
        /// 注册表打开选项。
        /// </param>
        /// <param name="samDesired">
        /// 注册表访问权限。
        /// </param>
        /// <param name="phkResult">
        /// 注册表键句柄。
        /// </param>
        /// <returns>
        /// Windows错误代码，详情参阅MSDN。
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RegOpenKeyExW")]
        public static extern int RegOpenKeyEx(
            IntPtr hKey,
            [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey,
            int ulOptions,
            int samDesired,
            out IntPtr phkResult);

        /// <summary>
        /// 删除注册表键。详情参阅：https://msdn.microsoft.com/en-us/library/ms724847(v=vs.85).aspx。
        /// </summary>
        /// <param name="hKey">
        /// 注册表根键。
        /// </param>
        /// <param name="lpSubKey">
        /// 注册表子键。
        /// </param>
        /// <param name="samDesired">
        /// 注册表访问权限。
        /// </param>
        /// <param name="lpReserved">
        /// 保留参数，必须为0。
        /// </param>
        /// <returns>
        /// Windows错误代码，详情参阅MSDN。
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RegDeleteKeyExW")]
        public static extern int RegDeleteKeyEx(
            IntPtr hKey,
            [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey,
            int samDesired,
            int lpReserved);

        /// <summary>
        /// 删除注册表键值。详情参阅：https://msdn.microsoft.com/en-us/library/ms724851(v=vs.85).aspx。
        /// </summary>
        /// <param name="hKey">
        /// 已打开的注册表句柄。
        /// </param>
        /// <param name="lpValueName">
        /// 注册表键名。
        /// </param>
        /// <returns>
        /// Windows错误代码，详情参阅MSDN。
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RegDeleteValueW")]
        public static extern int RegDeleteValue(
            IntPtr hKey,
            [MarshalAs(UnmanagedType.LPWStr)] string lpValueName);

        /// <summary>
        /// 枚举当前子键下所有子键，详情参阅：https://msdn.microsoft.com/en-us/library/ms724862(VS.85).aspx
        /// </summary>
        /// <param name="hKey">
        /// 已打开的注册表句柄。
        /// </param>
        /// <param name="dwIndex">
        /// 注册表子键索引。
        /// </param>
        /// <param name="lpName">
        /// 注册表子键名称。
        /// </param>
        /// <param name="lpcName">
        /// 注册表子键名称占用空间大小。
        /// </param>
        /// <param name="lpReserved">
        /// 保留参数，必须为0。
        /// </param>
        /// <param name="lpClass">
        /// 用户定义注册表类。
        /// </param>
        /// <param name="lpcClass">
        /// 用户定义注册表类占用空间大小。
        /// </param>
        /// <param name="lpftLastWriteTime">
        /// 最后一次修改时间
        /// </param>
        /// <returns>
        /// Windows错误代码，详情参阅MSDN。
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RegEnumKeyExW")]
        public static extern int RegEnumKeyEx(
            IntPtr hKey,
            int dwIndex,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpName,
            ref int lpcName,
            IntPtr lpReserved,
            IntPtr lpClass,
            IntPtr lpcClass,
            out FILETIME lpftLastWriteTime);

        /// <summary>
        /// 枚举当前子键下所有键名。详情参阅：https://msdn.microsoft.com/en-us/library/ms724865(v=vs.85).aspx。
        /// </summary>
        /// <param name="hKey">
        /// 已打开的注册表句柄。
        /// </param>
        /// <param name="dwIndex">
        /// 注册表键名索引。
        /// </param>
        /// <param name="lpValueName">
        /// 注册表键名。
        /// </param>
        /// <param name="lpcchValueName">
        /// 注册表键名占用空间大小。
        /// </param>
        /// <param name="lpReserved">
        /// 保留参数，必须为0。
        /// </param>
        /// <param name="lpType">
        /// 注册表键值。
        /// </param>
        /// <param name="lpData"></param>
        /// <param name="lpcbData"></param>
        /// <returns>
        /// Windows错误代码，详情参阅MSDN。
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RegEnumValueW")]
        public static extern int RegEnumValue(
            IntPtr hKey,
            int dwIndex,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpValueName,
            ref int lpcchValueName,
            IntPtr lpReserved,
            out int lpType,
            IntPtr lpData,
            ref int lpcbData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hKey">
        /// 已打开的注册表句柄。
        /// </param>
        /// <returns>
        /// Windows错误代码，详情参阅MSDN。
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int RegCloseKey(IntPtr hKey);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// Windows错误代码，详情参阅MSDN。
        /// </returns>
        [DllImport("kernel32.dll")]
        public static extern int GetLastError();

        #endregion

    }
}
