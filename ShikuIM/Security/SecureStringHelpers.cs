using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ShikuIM.Security
{

    /// <summary>
    /// Helpers for  the <see cref="SecureString"/> class
    /// </summary>
    public static class SecureStringHelpers
    {
        #region SecureString To String
        /// <summary>
        /// SecureString To String
        /// </summary>
        /// <param name="secureString">Source securestring</param>
        /// <returns>Ready text of securestring</returns>
        public static string UnSecure(this SecureString secureString)
        {
            //Make sure we have a secure string 
            if (secureString == null)
            {
                return string.Empty;
            }
            //Get a pointer for an unsecure string in memory
            var unmanagedString = IntPtr.Zero;
            try
            {
                //Unsecures the password
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                //Clean up any memory allocation
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
        #endregion

        #region String To SecureString
        /// <summary>
        /// String To SecureString
        /// </summary>
        /// <param name="sourceString">Source text</param>
        /// <returns>Ready SecureString</returns>
        public static SecureString ToSecureString(this string sourceString)
        {
            if (string.IsNullOrWhiteSpace(sourceString))
            {
                return null;
            }
            else
            {
                SecureString objSecureString = new SecureString();
                foreach (char c in sourceString.ToCharArray())
                {
                    objSecureString.AppendChar(c);
                }
                return objSecureString;
            }
        }
        #endregion


    }
}
