using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xanadu.Skidbladnir.OS.Windows.Registry.Enums;

namespace Xanadu.Skidbladnir.OS.Windows.Registry
{
    /// <inheritdoc cref="IComparable"/>
    /// <inheritdoc cref="ICloneable" />
    /// <summary>
    /// 注册表路径信息类。
    /// </summary>
    [Serializable]
    public class RegPath : ICloneable, IComparable
    {
        /// <summary>
        /// 注册表根键。
        /// </summary>
        public REG_ROOT_KEY HKey { get; set; }
        /// <summary>
        /// 注册表子键。
        /// </summary>
        public string LpSubKey { get; set; } = string.Empty;

        /// <summary>
        /// 注册表键名。
        /// </summary>
        public string LpValueName { get; set; } = string.Empty;
        /// <summary>
        /// 注册表路径信息类序列化构造函数。
        /// </summary>
        public RegPath()
        {

        }
        /// <summary>
        /// 注册表路径信息类构造函数。
        /// </summary>
        /// <param name="path">
        /// 注册表路径信息。
        /// </param>
        /// <param name="refMark">
        /// 是否为字符串引用。
        /// </param>
        public RegPath(string path, bool refMark = false)
        {
            if (refMark) path = path[1..^1];
            var index1 = path.IndexOf(@"\", StringComparison.Ordinal);
            var index2 = path.LastIndexOf(@"\", StringComparison.Ordinal);
            var tmp = path.Substring(0, index1);
            if (tmp == @"HKEY_CLASSES_ROOT") HKey = REG_ROOT_KEY.HKEY_CLASSES_ROOT;
            else if (tmp == @"HKEY_CURRENT_USER") HKey = REG_ROOT_KEY.HKEY_CURRENT_CONFIG;
            else if (tmp == @"HKEY_LOCAL_MACHINE") HKey = REG_ROOT_KEY.HKEY_LOCAL_MACHINE;
            else if (tmp == @"HKEY_USERS") HKey = REG_ROOT_KEY.HKEY_USERS;
            else if (tmp == @"HKEY_PERFORMANCE_DATA") HKey = REG_ROOT_KEY.HKEY_PERFORMANCE_DATA;
            else if (tmp == @"HKEY_CURRENT_CONFIG") HKey = REG_ROOT_KEY.HKEY_CURRENT_CONFIG;
            else HKey = REG_ROOT_KEY.HKEY_DYN_DATA;
            LpSubKey = path.Substring(index1 + 1, index2 - index1 - 1);
            LpValueName = path.Substring(index2 + 1, path.Length - index2 - 1);
        }
        /// <summary>
        /// 注册表路径信息类构造函数。
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
        public RegPath(REG_ROOT_KEY hKey, string lpSubKey, string lpValueName = "")
        {
            HKey = hKey;
            LpSubKey = lpSubKey;
            LpValueName = lpValueName;
        }

        /// <summary>
        /// 注册表路径信息类构造函数。
        /// </summary>
        /// <param name="jsonFile">
        /// Json文件位置。
        /// </param>
        public RegPath(string jsonFile)
        {
            var json = File.ReadAllText(jsonFile);
            var regPath = JsonConvert.DeserializeObject<RegPath>(json)!;
            HKey = regPath.HKey;
            LpSubKey = regPath.LpSubKey;
            LpValueName = regPath.LpValueName;
        }

        /// <summary>
        /// 注册表路径信息类复制构造函数。
        /// </summary>
        /// <param name="regPath">
        /// 注册表路径信息类。
        /// </param>
        public RegPath(RegPath regPath)
        {
            HKey = regPath.HKey;
            LpSubKey = regPath.LpSubKey;
            LpValueName = regPath.LpValueName;
        }
        /// <summary>
        /// 导出注册表信息到XML。
        /// </summary>
        /// <param name="jsonFile">
        /// XML文件路径。
        /// </param>
        /// <returns>
        /// true为导出成功。
        /// false为导出失败。
        /// </returns>
        public async Task ExportJson(string jsonFile)
        {
            var json = JsonConvert.SerializeObject(this);
            await File.WriteAllTextAsync(jsonFile, json);
        }
        /// <inheritdoc />
        /// <summary>
        /// 获取当前对象的深表副本。
        /// </summary>
        /// <returns>
        /// 当前对象的深表副本。
        /// </returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// 打开注册表子键句柄
        /// </summary>
        /// <returns>
        /// 注册表子键句柄
        /// </returns>
        protected Task<IntPtr> RegOpenKey()
        {
            return Task.Run(() =>
            {
                int regOpenKeyEx;
                IntPtr phkResult;
                if (Environment.Is64BitOperatingSystem)
                {
                    regOpenKeyEx = NativeMethods.RegOpenKeyEx(new IntPtr((int)HKey), LpSubKey, 0,
                        (int)KEY_SAM_FLAGS.KEY_WOW64_64KEY |
                        (int)KEY_ACCESS_TYPE.KEY_READ, out phkResult);
                }
                else
                {
                    regOpenKeyEx = NativeMethods.RegOpenKeyEx(new IntPtr((int)HKey), LpSubKey, 0,
                        (int)KEY_ACCESS_TYPE.KEY_READ, out phkResult);
                }

                if (regOpenKeyEx == (int)ERROR_CODE.ERROR_FILE_NOT_FOUND)
                {
                    throw new NullReferenceException(@"注册表访问失败" + '\n' + regOpenKeyEx + '\n' + nameof(RegOpenKey));
                }

                if (regOpenKeyEx != (int)ERROR_CODE.ERROR_SUCCESS)
                {
                    throw new Exception(@"注册表访问失败" + '\n' + regOpenKeyEx + '\n' + nameof(RegOpenKey));
                }

                return phkResult;
            });
        }

        /// <summary>
        /// 转换注册表所需数据。
        /// </summary>
        /// <param name="lpKind">
        /// 注册表键值类型。
        /// </param>
        /// <param name="lpData">
        /// 注册表键值。
        /// </param>
        /// <param name="lpcbData">
        /// 注册表键值所需内存。
        /// </param>
        /// <returns>
        /// 转换为已封装数据。
        /// </returns>
        protected Task<RegKey> ConvertData(REG_KEY_TYPE lpKind, IntPtr lpData, int lpcbData)
        {
            return Task.Run(() =>
            {
                RegKey regKey;
                if (lpKind == REG_KEY_TYPE.REG_DWORD)
                {
                    var lpValue = Marshal.ReadInt32(lpData);
                    regKey = new RegKey(this, lpKind, lpValue);
                }
                else if (lpKind == REG_KEY_TYPE.REG_QWORD)
                {
                    var lpValue = Marshal.ReadInt64(lpData);
                    regKey = new RegKey(this, lpKind, lpValue);
                }
                else if (lpKind == REG_KEY_TYPE.REG_SZ ||
                         lpKind == REG_KEY_TYPE.REG_EXPAND_SZ ||
                         lpKind == REG_KEY_TYPE.REG_MULTI_SZ)
                {
                    var lpValue = Marshal.PtrToStringUni(lpData);
                    lpValue = lpValue?.Trim();
                    regKey = new RegKey(this, lpKind, lpValue);
                }
                else if (lpKind == REG_KEY_TYPE.REG_BINARY)
                {
                    var lpdatabin = new byte[lpcbData];
                    Marshal.Copy(lpData, lpdatabin, 0, lpcbData);
                    regKey = new RegKey(this, lpKind, lpdatabin);
                }
                else
                {
                    throw new Exception(@"注册表访问失败" + '\n' + @"注册表数据类型异常" + '\n' + nameof(ConvertData));
                }

                return regKey;
            });
        }

        /// <summary>
        /// 获取注册表键信息。
        /// </summary>
        /// <exception cref="Exception">
        /// 非托管代码获取注册表时产生的异常，详情请参阅MSDN。
        /// </exception>
        /// <returns>
        /// 注册表键信息。
        /// </returns>
        public async Task<RegKey> Get()
        {
            RegKey regkey;
            try
            {
                var phkresult = await RegOpenKey();
                var lpcbData = 0;
                NativeMethods.RegQueryValueEx(phkresult, LpValueName, IntPtr.Zero, out var lpkind, IntPtr.Zero, ref lpcbData);
                if (lpcbData == 0)
                {
                    NativeMethods.RegCloseKey(phkresult);
                    throw new Exception(@"注册表访问失败" + '\n' + @"无法获取缓冲区大小" + '\n' + nameof(Get));
                }
                var lpdata = Marshal.AllocHGlobal(lpcbData);
                var reggetvaluetemp = NativeMethods.RegQueryValueEx(phkresult, LpValueName, IntPtr.Zero, out lpkind, lpdata, ref lpcbData);
                if (reggetvaluetemp != (int)ERROR_CODE.ERROR_SUCCESS)
                {
                    throw new Exception(@"注册表访问失败" + '\n' + reggetvaluetemp + '\n' + nameof(Get));
                }
                NativeMethods.RegCloseKey(phkresult);
                if (reggetvaluetemp != (int)ERROR_CODE.ERROR_SUCCESS)
                {
                    throw new Exception(@"注册表访问失败" + '\n' + reggetvaluetemp + '\n' + nameof(Get));
                }

                regkey = await ConvertData((REG_KEY_TYPE)lpkind, lpdata, lpcbData);
            }
            catch (Exception)
            {
                regkey = new RegKey(this);
            }
            return regkey;
        }

        /// <summary>
        /// 枚举当前子键下所有子键信息。
        /// </summary>
        /// <returns>
        /// 枚举得到的注册表键名信息。
        /// </returns>
        public async Task<List<RegPath>> EnumKey()
        {
            var phkresult = await RegOpenKey();
            var list = new List<RegPath>();
            for (var index = 0; ; index++)
            {
                var sb = new StringBuilder(0x7FFF);
                var size = 0x7FFF;
                var regenumkeytmp = NativeMethods.RegEnumKeyEx(phkresult, index, sb, ref size, IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero, out _);
                if (regenumkeytmp == (int)ERROR_CODE.ERROR_NO_MORE_ITEMS)
                {
                    break;
                }

                if (regenumkeytmp != (int)ERROR_CODE.ERROR_SUCCESS)
                {
                    throw new Exception(@"注册表键值枚举失败" + '\n' + regenumkeytmp + '\n' + nameof(EnumKey));
                }

                list.Add(new RegPath(HKey, LpSubKey + @"\" + sb));
            }

            NativeMethods.RegCloseKey(phkresult);
            list.Sort();
            return list;
        }

        /// <summary>
        /// 枚举当前子键下所有键名信息。
        /// </summary>
        /// <param name="defaultReg">
        /// 是否包含默认注册表项。
        /// </param>
        /// <returns>
        /// 枚举得到的注册表键名信息。
        /// </returns>
        public async Task<List<RegKey>> EnumValue(bool defaultReg = false)
        {
            var phkresult = await RegOpenKey();
            var list = new List<RegKey>();
            for (var index = 0; ; index++)
            {
                var sb = new StringBuilder(0x7FFF);
                var size = 0x7FFF;
                var lpcbdata = 0;
                var regenumvaluetmp = NativeMethods.RegEnumValue(phkresult, index, sb, ref size, IntPtr.Zero,
                    out var lpkind,
                    IntPtr.Zero, ref lpcbdata);
                size += 2;
                if (regenumvaluetmp == (int)ERROR_CODE.ERROR_NO_MORE_ITEMS) break;
                if (regenumvaluetmp == (int)ERROR_CODE.ERROR_FILE_NOT_FOUND)
                    throw new NullReferenceException(@"注册表键值枚举失败" + '\n' + regenumvaluetmp + '\n' +
                                                     nameof(EnumValue));
                if (regenumvaluetmp != (int)ERROR_CODE.ERROR_SUCCESS)
                    throw new Exception(@"注册表键值枚举失败" + '\n' + regenumvaluetmp + '\n' + nameof(EnumValue));
                var lpdata = Marshal.AllocHGlobal(lpcbdata);
                regenumvaluetmp = NativeMethods.RegEnumValue(phkresult, index, sb, ref size, IntPtr.Zero,
                    out lpkind,
                    lpdata, ref lpcbdata);
                if (regenumvaluetmp != (int)ERROR_CODE.ERROR_SUCCESS)
                    throw new Exception(@"注册表键值枚举失败" + '\n' + regenumvaluetmp + '\n' + nameof(EnumValue));
                var str = sb.ToString().Trim();
                if (!defaultReg && str == string.Empty) continue;
                var regkey = await ConvertData((REG_KEY_TYPE)lpkind, lpdata, lpcbdata);
                list.Add(regkey);

            }

            NativeMethods.RegCloseKey(phkresult);
            list.Sort();
            return list;
        }

        /// <summary>
        /// 删除指定注册表键。
        /// </summary>
        /// <returns>
        /// 异步方法运行状态。
        /// </returns>
        public Task Delete()
        {
            return Task.Run(() =>
            {
                int regdelkeytmp;
                if (string.IsNullOrEmpty(LpValueName))
                {
                    regdelkeytmp = NativeMethods.RegDeleteKeyEx(new IntPtr((int)HKey), LpSubKey,
                        (int)KEY_SAM_FLAGS.KEY_WOW64_64KEY | (int)KEY_ACCESS_TYPE.KEY_SET_VALUE, 0);
                    if (regdelkeytmp != (int)ERROR_CODE.ERROR_SUCCESS)
                    {
                        throw new Exception(@"注册表访问失败" + '\n' + regdelkeytmp + '\n' + nameof(Delete));
                    }
                }
                else
                {
                    regdelkeytmp = NativeMethods.RegOpenKeyEx(new IntPtr((int)HKey), LpSubKey, 0,
                        (int)KEY_SAM_FLAGS.KEY_WOW64_64KEY |
                        (int)KEY_ACCESS_TYPE.KEY_SET_VALUE, out var phkresult);
                    if (regdelkeytmp != (int)ERROR_CODE.ERROR_SUCCESS)
                    {
                        throw new Exception(@"注册表访问失败" + '\n' + regdelkeytmp + '\n' + nameof(Delete));
                    }

                    regdelkeytmp = NativeMethods.RegDeleteValue(phkresult, LpValueName);
                    if (regdelkeytmp != (int)ERROR_CODE.ERROR_SUCCESS)
                    {
                        throw new Exception(@"注册表访问失败" + '\n' + regdelkeytmp + '\n' + nameof(Delete));
                    }

                    NativeMethods.RegCloseKey(phkresult);
                }
            });
        }

        /// <inheritdoc />
        /// <summary>
        /// 注册表路径信息类默认排序规则。
        /// </summary>
        /// <param name="obj">
        /// 待比较的对象。
        /// </param>
        /// <returns>
        /// 大小比较结果。
        /// </returns>
        public int CompareTo(object? obj)
        {
            if (!(obj is RegPath regpath)) throw new NullReferenceException();
            if (HKey < regpath.HKey) return 1;
            if (HKey > regpath.HKey) return -1;
            var flag = string.CompareOrdinal(LpSubKey, regpath.LpSubKey);
            return flag != 0 ? flag : string.CompareOrdinal(LpValueName, LpValueName);
        }
    }
}
