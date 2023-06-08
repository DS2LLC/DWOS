using System;
using System.Windows.Forms;
using DWOS.Shared;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinGrid;
using NLog;

namespace DWOS.UI.Admin
{
    using DWOS.Data.Datasets.PartsDatasetTableAdapters;

    public partial class AirframeManager: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private bool _loaded;
        private GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("AirframeManager", new UltraGridBandSettings());

        #endregion

        #region Methods

        public AirframeManager()
        {
            this.InitializeComponent();
        }

        private void LoadData()
        {
            this.dsParts.EnforceConstraints = false;
            this.dsParts.d_Manufacturer.BeginLoadData();

            this.taManufacturer.Fill(this.dsParts.d_Manufacturer);
            this.taAirframe.Fill(this.dsParts.d_Airframe);

            this.dsParts.d_Manufacturer.EndLoadData();

            this._loaded = true;
        }

        private bool SaveData()
        {
            try
            {
                this.grdAirframe.UpdateData();

                if (dsParts.d_Airframe.HasErrors || dsParts.d_Airframe.Select("AirframeId IS NULL").Length > 0)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("Please fix any errors any the table.", "Table Errors", "Ensure all errors are fixed before saving.");
                    return false;
                }

                this.taAirframe.Update(this.dsParts.d_Airframe);

                return true;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving data.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);

                return false;
            }
        }

        #endregion

        #region Events

        /// <summary>
        ///   Handles the Load event of the AirframeManager control.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> The <see cref="System.EventArgs" /> instance containing the event data. </param>
        private void AirframeManager_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadData();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error loading airframe manager.";
                _log.Error(exc, errorMsg);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if(this.SaveData())
                    Close();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving data.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void grdAirframe_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if(e.Row != null)
            {
                var airframeIdCell = e.Row.Cells["AirframeID"];

                if (airframeIdCell.Value != null && airframeIdCell.Value.ToString() == "<None>")
                {
                    e.Row.Cells[1].Activation = Activation.Disabled;
                    e.Row.Cells[0].Activation = Activation.Disabled;
                }
                

                if(airframeIdCell.Value == null || airframeIdCell.Value == DBNull.Value || String.IsNullOrWhiteSpace(airframeIdCell.Value.ToString()))
                {
                    airframeIdCell.Row.DataErrorInfo.RowError = "Value required.";
                }
            }
        }

        private void grdAirframe_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                grdAirframe.AfterColPosChanged -= grdAirframe_AfterColPosChanged;
                grdAirframe.AfterSortChange -= grdAirframe_AfterSortChange;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(grdAirframe.DisplayLayout.Bands[0]);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing grid layout.");
            }
            finally
            {
                grdAirframe.AfterColPosChanged += grdAirframe_AfterColPosChanged;
                grdAirframe.AfterSortChange += grdAirframe_AfterSortChange;
            }
        }

        private void grdAirframe_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdAirframe.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column position.");
            }
        }

        private void grdAirframe_AfterSortChange(object sender, BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdAirframe.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column position.");
            }
        }

        private void grdAirframe_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            if (!_loaded || e.NewValue == null || e.Cell.Column.Key != "AirframeID")
            {
                return;
            }

            var found = this.dsParts.d_Airframe.FindByAirframeID(e.NewValue.ToString());

            if(found != null)
            {
                e.Cancel = true;
                MessageBoxUtilities.ShowMessageBoxWarn("Airframe already exists. Duplicate airframes are not allowed.", "Duplicate Airframes");
            }
        }

        private void grdAirframe_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            foreach (var row in e.Rows)
            {
                var modelName = row.Cells["AirframeID"].Value.ToString();
                int? usageCount;
                using (var taParts = new PartTableAdapter())
                {
                    usageCount = taParts.ModelUsageCount(modelName);
                }

                if (usageCount != null && usageCount > 0)
                {
                    var result = MessageBox.Show(modelName + " is in use in " + usageCount + " parts. The model will be marked as inactive.", "Warning", MessageBoxButtons.OKCancel);

                    if (result == DialogResult.OK)
                        row.Cells["IsActive"].Value = false;

                    // cancel the delete. Whether the user clicks OK or Cancel the part should remain
                    e.Cancel = true;
                }
            }
        }

        #endregion

    }
}