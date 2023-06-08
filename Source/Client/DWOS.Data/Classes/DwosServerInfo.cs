namespace DWOS.Data
{
    /// <summary>
    /// Represents information about a DWOS Server.
    /// </summary>
    public sealed class DwosServerInfo
    {
        #region Fields

        public const string SERVER_ADDRESS_SCHEME = "net.tcp";

        #endregion

        #region Properties

        /// <summary>
        /// Gets a default instance of <see cref="DwosServerInfo"/> from
        /// <see cref="UserSettings"/>.
        /// </summary>
        public static DwosServerInfo Default
        {
            get
            {
                return new DwosServerInfo()
                {
                    ServerAddress = UserSettings.Default.ServerAddress,
                    ServerPort = UserSettings.Default.ServerPort,
                    FromConnectionFile = false
                };
            }
        }

        /// <summary>
        /// Gets or sets the server's address.
        /// </summary>
        public string ServerAddress { get; set; }

        /// <summary>
        /// Gets or sets the server's port.
        /// </summary>
        public int ServerPort { get; set; }

        /// <summary>
        /// Gets the server's URI.
        /// </summary>
        public string ServerAddressUri
        {
            get
            {
                return string.Format("{0}://{1}:{2}/LicenseService", SERVER_ADDRESS_SCHEME, ServerAddress, ServerPort);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if these settings are from a
        /// connection file.
        /// </summary>
        /// <value>
        /// <c>true</c> if these settings come from a connection file;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool FromConnectionFile { get; set; }

        #endregion
    }
}
