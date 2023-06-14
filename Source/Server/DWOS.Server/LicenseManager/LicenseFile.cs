using System;
using System.Text;
using DWOS.Shared.Utilities;

namespace DWOS.LicenseManager
{
    /// <summary>
    ///   Represents the license details for this customer. This key is loaded from the DS2 servers and is saved to the local isolated storage.
    /// </summary>
    public class LicenseFile
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the name of the company.
        /// </summary>
        /// <value> The name of the company. </value>
        public string CompanyName { get; set; }

        /// <summary>
        ///   Gets or sets the activations.
        /// </summary>
        /// <value> The activations. </value>
        public int Activations { get; set; }

        /// <summary>
        ///   Gets or sets the license expiration.
        /// </summary>
        /// <value> The license expiration. </value>
        public DateTime LicenseExpiration { get; set; }

        /// <summary>
        /// Protects the specified license file as a byte[].
        /// </summary>
        /// <param name="license">The license.</param>
        /// <returns>System.Byte[][].</returns>
        public static byte[] Protect(LicenseFile license)
        {
            try
            {
                if (license == null)
                    return new byte[0];

                byte[] aditionalEntropy = { 9, 8, 7, 6, 5 };
                string secret           = license.CompanyName + "|" + license.Activations + "|" + license.LicenseExpiration.ToBinary().ToString();
                var binarySecret        = Encoding.Unicode.GetBytes(secret);
                var protectedSecret     = System.Security.Cryptography.ProtectedData.Protect(binarySecret, aditionalEntropy, System.Security.Cryptography.DataProtectionScope.LocalMachine);

                return protectedSecret;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error converting license file to a protected secret.");
                return new byte[0];
            }
        }

        /// <summary>
        /// Unprotect the license file from the byte[].
        /// </summary>
        /// <param name="protectedSecret">The protected secret.</param>
        /// <returns>LicenseFile.</returns>
        public static LicenseFile UnProtect(byte[] protectedSecret)
        {
            try
            {
                if (protectedSecret == null || protectedSecret.Length < 1)
                    return null;

                byte[] aditionalEntropy = { 9, 8, 7, 6, 5 };
                var binarySecret = System.Security.Cryptography.ProtectedData.Unprotect(protectedSecret, aditionalEntropy, System.Security.Cryptography.DataProtectionScope.LocalMachine);

                var secret = binarySecret.Length < 1 ? null : Encoding.Unicode.GetString(binarySecret);
                var secrets = secret.Split('|');

                var license = new LicenseFile() { CompanyName = secrets[0], Activations = Convert.ToInt32(secrets[1]), LicenseExpiration = DateTime.FromBinary(Convert.ToInt64(secrets[2])) };

                return license;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error converting bytes to a license file.");
                return null;
            }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return this.CompanyName + ", " + this.Activations + ", " + this.LicenseExpiration.ToShortDateString();
        }

        #endregion
    }
}