using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Xanadu.Skidbladnir.Core.Extension
{
    /// <summary>
    /// Cast object to another type.
    /// </summary>
    public static class CastExtension
    {
        #region Object

        /// <summary>
        /// Serialize object to json stream with UTF-8 encoding.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Memory stream</returns>
        /// <exception cref="ArgumentNullException">Object is null.</exception>
        /// <exception cref="InvalidCastException">Unable to cast string to json stream. Please use Encoding.UTF8.GetBytes().</exception>
        public static Stream ToJsonStream(this object obj)
        {
            switch (obj)
            {
                case null:
                    throw new ArgumentNullException(nameof(obj));
                case string:
                    throw new InvalidCastException("Unable to cast string to bson.");
            }

            var json = JsonConvert.SerializeObject(obj, Formatting.None);
            return new MemoryStream(Encoding.UTF8.GetBytes(json));
        }

        #endregion

        #region String

        /// <summary>
        /// Convert string to secure string. It usually used for Powershell.
        /// </summary>
        /// <param name="str">String to cast.</param>
        /// <returns>String in SecureString.</returns>
        public static SecureString ToSecureString(this string str)
        {
            var ss = new SecureString();
            foreach (var c in str)
            {
                ss.AppendChar(c);
            }

            return ss;
        }

        /// <summary>
        /// Convert secure string to string. It usually used for Powershell.
        /// </summary>
        /// <param name="ss">SecureString to cast.</param>
        /// <returns>Normal string.</returns>
        public static string ToStr(this SecureString ss)
        {
            var returnValue = nint.Zero;
            try
            {
                returnValue = Marshal.SecureStringToGlobalAllocUnicode(ss);
                return Marshal.PtrToStringUni(returnValue)!;
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(returnValue);
            }
        }

        /// <summary>
        /// Convert UTF-8 string to base64.
        /// </summary>
        /// <param name="str">String to cast.</param>
        /// <returns>Base64 string.</returns>
        public static string ToBase64(this string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        #endregion

        #region Byte

        /// <summary>
        /// Convert byte array to hexadecimal string.
        /// </summary>
        /// <param name="bytes">Byte array to cast.</param>
        /// <returns>Hexadecimal string</returns>
        public static string ToHexadecimal(this byte[] bytes)
        {
            return bytes.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        #endregion

    }
}