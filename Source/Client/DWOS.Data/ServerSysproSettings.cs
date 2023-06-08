using DWOS.Shared.Data;
using DWOS.Shared.Settings;
using Microsoft.Win32;

namespace DWOS.Data
{
    /// <summary>
    /// Defines server settings related to SYSPRO integration.
    /// </summary>
    public sealed class ServerSysproSettings : NestedRegistrySettingsBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the update interval in minutes.
        /// </summary>
        [DataColumn(DefaultValue = 60)]
        public int UpdateIntervalMinutes { get; set; }

        /// <summary>
        /// Gets or sets the number of days to wait before deleting a
        /// successfully processed invoice.
        /// </summary>
        [DataColumn(DefaultValue = 60)]
        public int CleanSuccessfulInvoiceDays { get; set; }

        /// <summary>
        /// Gets or sets the directory that invoice files go to.
        /// </summary>
        [DataColumn]
        public string SaveDirectory { get; set; }

        /// <summary>
        /// Gets or sets the directory for error files.
        /// </summary>
        [DataColumn]
        public string ErrorDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if orders should be automatically invoiced.
        /// </summary>
        /// <returns>
        /// <c>true</c> if orders should be automatically invoiced;
        /// otherwise, <c>false</c>.
        /// </returns>
        [DataColumn]
        public bool AutomaticInvoicing { get; set; }

        /// <summary>
        /// Gets or sets a setting indicating if SYSPRO integration is enabled
        /// </summary>
        /// <remarks>
        /// <see cref="ApplicationSettings.SysproIntegrationEnabled"/> might
        /// not have an up-to-date value.
        /// </remarks>
        /// <value>
        /// <c>true </c> if SYSPRO integration is enabled;
        /// otherwise, <c>false</c>.
        /// </value>
        [DataColumn]
        public bool Enabled { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ServerSysproSettings"/> class.
        /// </summary>
        /// <param name="hive"></param>
        /// <param name="baseKey"></param>
        public ServerSysproSettings(RegistryKey hive, string baseKey) : base(hive, baseKey)
        {
        }

        #endregion
    }
}
