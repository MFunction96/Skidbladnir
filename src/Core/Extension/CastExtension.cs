using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Xanadu.Skidbladnir.Core.Extension
{
    public static class CastExtension
    {
        #region Object

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

        public static SecureString ToSecureString(this string str)
        {
            var ss = new SecureString();
            foreach (var c in str)
            {
                ss.AppendChar(c);
            }

            return ss;
        }

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

        public static string ToBase64(this string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        #endregion

        #region Byte

        public static string ToHexadecimal(this byte[] bytes)
        {
            return bytes.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        #endregion

    }
}