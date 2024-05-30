using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Xanadu.Skidbladnir.OS.Windows.Registry.Enums;

namespace Xanadu.Skidbladnir.OS.Windows.Registry
{
    /// <inheritdoc cref="RegPath" />
    /// <summary>
    /// 注册表键信息类。
    /// </summary>
    [Serializable]
    public class RegKey : RegPath
    {
        /// <summary>
        /// 注册表键值类型。
        /// </summary>
        public REG_KEY_TYPE LpKind { get; set; }
        /// <summary>
        /// 注册表键值。
        /// </summary>
        public object LpValue { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// 注册表键信息类序列化构造函数。
        /// </summary>
        public RegKey() { }
        /// <inheritdoc />
        /// <summary>
        /// 注册表键信息类构造函数。
        /// </summary>
        /// <param name="hKey">
        /// 注册表根键。
        /// </param>
        /// <param name="lpSubKey">
        /// 注册表子键。
        /// </param>
        /// <param name="lpValueName">
        /// 注册表键名。
        /// </param>
        /// <param name="lpKind">
        /// 注册表键值类型。
        /// </param>
        /// <param name="lpValue">
        /// 注册表键值。
        /// </param>
        public RegKey(
            REG_ROOT_KEY hKey,
            string lpSubKey,
            string lpValueName = "",
            REG_KEY_TYPE lpKind = REG_KEY_TYPE.REG_UNKNOWN,
            object lpValue = null) :
            base(hKey, lpSubKey, lpValueName)
        {
            LpKind = lpKind;
            LpValue = lpValue;
        }
        /// <inheritdoc />
        /// <summary>
        /// 注册表键信息类构造函数。
        /// </summary>
        /// <param name="regPath">
        /// 注册表键路径信息类。
        /// </param>
        /// <param name="lpKind">
        /// 注册表键值类型。
        /// </param>
        /// <param name="lpValue">
        /// 注册表键值。
        /// </param>
        public RegKey(RegPath regPath, REG_KEY_TYPE lpKind = REG_KEY_TYPE.REG_UNKNOWN, object lpValue = null) :
            base(regPath)
        {
            LpKind = lpKind;
            LpValue = lpValue;
        }

        /// <inheritdoc />
        /// <summary>
        /// 注册表路径信息类构造函数。
        /// </summary>
        /// <param name="jsonFile">
        /// Json文件位置。
        /// </param>
        public RegKey(string jsonFile)
        {
            var json = File.ReadAllText(jsonFile);
            var regkey = JsonSerializer.Deserialize<RegKey>(json);
            HKey = regkey.HKey;
            LpSubKey = regkey.LpSubKey;
            LpValueName = regkey.LpValueName;
            LpKind = regkey.LpKind;
            LpValue = regkey.LpValue;
        }
        /// <inheritdoc />
        /// <summary>
        /// 注册表键信息类复制构造函数。
        /// </summary>
        /// <param name="regKey">
        /// 注册表键信息类。
        /// </param>
        public RegKey(RegKey regKey) :
            base(regKey.HKey, regKey.LpSubKey, regKey.LpValueName)
        {
            LpKind = regKey.LpKind;
            LpValue = regKey.LpValue;
        }
        /// <summary>
        /// 获取注册表路径信息。
        /// </summary>
        /// <returns>
        /// 注册表路径信息类。
        /// </returns>
        public RegPath GetRegPath()
        {
            return new RegPath(HKey, LpSubKey, LpValueName);
        }
        /// <summary>
        /// 获取当前对象的深表副本。
        /// </summary>
        /// <returns>
        /// 当前对象的深表副本。
        /// </returns>
        public new object Clone()
        {
            return MemberwiseClone();
        }
        /// <summary>
        /// 注册表路径信息类默认排序规则。
        /// </summary>
        /// <param name="obj">
        /// 待比较的对象。
        /// </param>
        /// <returns>
        /// 大小比较结果。
        /// </returns>
        public new int CompareTo(object obj)
        {
            if (!(obj is RegKey regkey)) throw new NullReferenceException();
            var flag = base.CompareTo(obj);
            if (flag != 0) return flag;
            if (LpKind < regkey.LpKind) return 1;
            if (LpKind > regkey.LpKind) return -1;
            return string.CompareOrdinal(LpValue.ToString(), regkey.LpValue.ToString());
        }

        /// <summary>
        /// 设置注册表键。
        /// </summary>
        /// <returns>
        /// 异步方法运行状态。
        /// </returns>
        public Task Set()
        {
            return Task.Run(() =>
            {
                int regsetvaluetmp, exists;
                IntPtr phkResult;
                if (Environment.Is64BitOperatingSystem)
                {
                    regsetvaluetmp = NativeMethods.RegCreateKeyEx(new IntPtr((int)HKey), LpSubKey, 0, null,
                        (int)OPERATE_OPTION.REG_OPTION_NON_VOLATILE,
                        (int)KEY_SAM_FLAGS.KEY_WOW64_64KEY | (int)KEY_ACCESS_TYPE.KEY_READ |
                        (int)KEY_ACCESS_TYPE.KEY_WRITE, IntPtr.Zero, out phkResult, out exists);
                }
                else
                {
                    regsetvaluetmp = NativeMethods.RegCreateKeyEx(new IntPtr((int)HKey), LpSubKey, 0, null,
                        (int)OPERATE_OPTION.REG_OPTION_NON_VOLATILE,
                        (int)KEY_ACCESS_TYPE.KEY_READ |
                        (int)KEY_ACCESS_TYPE.KEY_WRITE, IntPtr.Zero, out phkResult, out exists);
                }
                if (regsetvaluetmp != (int)ERROR_CODE.ERROR_SUCCESS && exists != (int)REG_CREATE_DISPOSITION.REG_OPENED_EXISTING_KEY)
                {
                    throw new Exception(@"注册表访问失败" + '\n' + regsetvaluetmp + '\n' + nameof(Set));
                }
                IntPtr lpdata;
                int lpcbData;
                if (LpKind == REG_KEY_TYPE.REG_SZ ||
                    LpKind == REG_KEY_TYPE.REG_EXPAND_SZ ||
                    LpKind == REG_KEY_TYPE.REG_MULTI_SZ)
                {
                    if (!(LpValue is string lpdatastr)) throw new NullReferenceException();
                    lpcbData = lpdatastr.Length + 1 << 1;
                    lpdata = Marshal.StringToHGlobalUni(lpdatastr);
                }
                else if (LpKind == REG_KEY_TYPE.REG_DWORD)
                {
                    lpcbData = Marshal.SizeOf(typeof(int));
                    lpdata = Marshal.AllocHGlobal(lpcbData);
                    Marshal.WriteInt32(lpdata, (int)LpValue);
                }
                else if (LpKind == REG_KEY_TYPE.REG_QWORD)
                {
                    lpcbData = Marshal.SizeOf(typeof(long));
                    lpdata = Marshal.AllocHGlobal(lpcbData);
                    Marshal.WriteInt64(lpdata, (long)LpValue);
                }
                else if (LpKind == REG_KEY_TYPE.REG_BINARY)
                {
                    if (!(LpValue is byte[] lpdatabin)) throw new NullReferenceException();
                    lpcbData = lpdatabin.Length;
                    lpdata = Marshal.AllocHGlobal(lpcbData);
                    Marshal.Copy(lpdatabin, 0, lpdata, lpcbData);
                }
                else
                {
                    throw new Exception(@"注册表访问失败" + '\n' + regsetvaluetmp + '\n' + nameof(Set));
                }
                regsetvaluetmp =
                    NativeMethods.RegSetValueEx(phkResult, LpValueName, 0, (int)LpKind, lpdata, lpcbData);
                NativeMethods.RegCloseKey(phkResult);
                if (regsetvaluetmp != (int)ERROR_CODE.ERROR_SUCCESS)
                {
                    throw new Exception(@"注册表访问失败" + '\n' + regsetvaluetmp + '\n' + nameof(Set));
                }
            });
        }
    }
}
