using DWOS.Shared.Data;
using DWOS.Shared.Settings;
using Microsoft.Win32;

namespace DWOS.Data
{
    /// <summary>
    /// Defines user settings relating to shipping.
    /// </summary>
    public class ShippingSettings : NestedRegistrySettingsBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value that indicates the status of the 'quick print' feature.
        /// </summary>
        /// <value>
        /// <c>true</c> if quick print is enabled; otherwise, <c>false</c>.
        /// </value>
        [DataColumn(DefaultValue = true)]
        public bool QuickPrint { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether or not the application
        /// produces a packing slip (list) whenever an order ships.
        /// </summary>
        /// <value>
        /// <c>true</c> to show/print a packing slip; otherwise, <c>false</c>.
        /// </value>
        [DataColumn(DefaultValue = false)]
        public bool PrintPackingList { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether or not the application
        /// prints an order label when adding an order to a package.
        /// </summary>
        /// <value>
        /// <c>true</c> to automatically print an order shipping label;
        /// otherwise, <c>false</c>.
        /// </value>
        [DataColumn(DefaultValue = true)]
        public bool PrintOrderLabel { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether or not the application
        /// prints a package label when creating a new shipment package.
        /// </summary>
        /// <value>
        /// <c>true</c> to automatically print a package shipping label;
        /// otherwise, <c>false</c>.
        /// </value>
        [DataColumn(DefaultValue = true)]
        public bool PrintPackageLabel { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether or not the application
        /// prints an order's COC when adding an order to a package.
        /// </summary>
        /// <value>
        /// <c>true</c> to automatically print a COC;
        /// otherwise, <c>false</c>.
        /// </value>
        [DataColumn]
        public bool PrintCoc { get; set; }

        /// <summary>
        /// Gets or sets the number of packing slip copies to print.
        /// </summary>
        [DataColumn(DefaultValue = 1)]
        public int PackingListCount { get; set; }

        /// <summary>
        /// Gets or sets the number of Bill of Lading copies to print.
        /// </summary>
        [DataColumn(DefaultValue = 3)]
        public int BillOfLadingCount { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingSettings"/>
        /// class.
        /// </summary>
        /// <param name="hive"></param>
        /// <param name="baseKey"></param>
        public ShippingSettings(RegistryKey hive, string baseKey) : base(hive, baseKey)
        {
        }

        #endregion
    }
}
