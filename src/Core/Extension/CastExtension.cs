using System.Runtime.InteropServices;
using System.Security;

namespace Xanadu.Skidbladnir.Core.Extension
{
    /// <summary>
    /// Cast object to another type.
    /// </summary>
    public static class CastExtension
    {
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
        public static string ToNormalString(this SecureString ss)
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

        #endregion

    }
}