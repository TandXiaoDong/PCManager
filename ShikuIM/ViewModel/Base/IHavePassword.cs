using System.Security;

namespace ShikuIM.ViewModel.Base
{

    /// <summary>
    /// An Interface fot a class that can provide password
    /// </summary>
    public interface IHavePassword
    {

        /// <summary>
        /// The Login Secure Password
        /// </summary>
        SecureString LoginSecurePassword { get; set; }

        /// <summary>
        /// The Login Secure Password
        /// </summary>
        SecureString FirstRegisterSecurePassword { get; set; }


        /// <summary>
        /// The Login Secure Password
        /// </summary>
        SecureString FinalRegisterSecurePassword { get; set; }


    }
}
