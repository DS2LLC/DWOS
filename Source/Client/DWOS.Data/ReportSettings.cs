using DWOS.Shared.Data;
using DWOS.Shared.Settings;
using Microsoft.Win32;

namespace DWOS.Data
{
    /// <summary>
    /// Defines user settings for reports.
    /// </summary>
    public class ReportSettings : NestedRegistrySettingsBase
    {
        #region Properties

        /// <summary>
        /// Gets the Late Order Report grouping type.
        /// </summary>
        [DataColumn(DefaultValue = "Customer")]
        public string LateOrderReportGroupType { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportSettings"/> class.
        /// </summary>
        /// <param name="hive"></param>
        /// <param name="baseKey"></param>
        public ReportSettings(RegistryKey hive, string baseKey) : base(hive, baseKey)
        {
        }

        #endregion
    }
}
