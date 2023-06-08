using Infragistics.Windows.DataPresenter;
using NLog;
using System;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Settings base class for use with <see cref="GridSettingsPersistence{T}"/>.
    /// </summary>
    /// <remarks>
    /// Due to an Infragistics issue, removing columns from any grid using this class
    /// will cause the layout to reset. This is fixed in a currently available version.
    /// </remarks>
    public class XamDataGridSettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets the XML of grid customizations.
        /// </summary>
        public string Customizations { get; set; }

        #endregion

        #region Methods

        public virtual void ApplyTo(XamDataGrid grid)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            if (string.IsNullOrEmpty(Customizations))
            {
                return;
            }

            try
            {
                grid.LoadCustomizations(Customizations, out var errorMsg);

                if (!string.IsNullOrEmpty(errorMsg))
                {
                    LogManager.GetCurrentClassLogger().Warn("Error loading layout customizations:\n{0}", errorMsg);
                }
            }
            catch (System.Xml.XmlException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error loading layout customizations");
            }
            catch (InvalidOperationException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error loading layout customizations");
            }
        }

        public virtual void RetrieveSettingsFrom(XamDataGrid grid)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            Customizations = grid.SaveCustomizations();
        }

        #endregion
    }
}
