using System;

namespace DWOS.Data
{
    /// <summary>
    /// Attribute used to link a property (or field) to a
    /// <see cref="Data.ScannerSettingsType"/> value.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ScannerTypeAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets or sets the scanner settings type.
        /// </summary>
        public ScannerSettingsType SettingsType { get; set; }

        #endregion


        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ScannerTypeAttribute"/> test.
        /// </summary>
        /// <param name="settingsType"></param>
        public ScannerTypeAttribute(ScannerSettingsType settingsType)
        {
            SettingsType = settingsType;
        }

        #endregion


    }
}
