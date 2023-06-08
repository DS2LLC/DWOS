using System;
using System.Data;
using System.Windows.Forms;
using DWOS.Shared;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinGrid;
using NLog;

namespace DWOS.UI.Admin
{
    public partial class ManufacturerManager: Form
    {
        #region Fields

        private GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("ManufacturerManager", new UltraGridBandSettings());

        #endregion

        #region Methods

        public ManufacturerManager()
        {
            this.InitializeComponent();
        }

        private void LoadData()
        {
            this.dsParts.EnforceConstraints = false;
            this.dsParts.d_Manufacturer.BeginLoadData();

            this.taManufacturer.Fill(this.dsParts.d_Manufacturer);

            this.dsParts.d_Manufacturer.EndLoadData();
            //dsParts.EnforceConstraints = true;
        }

        private bool SaveData()
        {
            try
            {
                this.grdManufacturer.UpdateData();

                if (!ValidateData())
                {
                    return false;
                }

                this.taManufacturer.Update(this.dsParts.d_Manufacturer);
                return true;
            }
            catch(Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error saving data.");
                return false;
            }
        }

        private bool ValidateData()
        {
            bool result = true;

            try
            {
                var manufacturerColumn = dsParts.d_Manufacturer.ManufacturerIDColumn.ColumnName;

                foreach (UltraGridRow row in grdManufacturer.DisplayLayout.Bands[0].GetRowEnumerator(GridRowType.DataRow))
                {
                    UltraGridCell cell = row.Cells[manufacturerColumn];
                    var drv = row.ListObject as DataRowView;

                    if (cell != null && drv != null)
                    {
                        if (string.IsNullOrEmpty(cell.Text) || cell.Value == null)
                        {
                            drv.Row.SetColumnError(manufacturerColumn, "Material can not be empty");
                            result = false;
                        }
                        else
                        {
                            drv.Row.ClearErrors();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error validating data.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
                result = false;
            }

            return result;
        }

        #endregion

        #region Events

        private void ManufacturerManager_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadData();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error loading Manufacturer Manager.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
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
                string errorMsg = "Error saving manufacture results.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void grdManufacturer_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            try
            {
                if (e.NewValue == null || e.Cell.Column.Key != dsParts.d_Manufacturer.ManufacturerIDColumn.ColumnName)
                {
                    return;
                }

                if (this.dsParts.d_Manufacturer.FindByManufacturerID(e.NewValue.ToString()) != null)
                {
                    const string warningMessage = "Manufacturer already exists. Duplicate manufacturers are not allowed.";

                    e.Cancel = true;
                    MessageBoxUtilities.ShowMessageBoxWarn(warningMessage, "Duplicate Manufacturers");
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating manufacturer data.");
            }
        }

        private void grdManufacturer_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (e.Cell.Column.Key == this.dsParts.d_Manufacturer.ManufacturerIDColumn.ColumnName && e.Cell.Value != null && e.Cell.Value.ToString().EndsWith(" "))
                e.Cell.Value = e.Cell.Value.ToString().Trim();
        }

        private void grdManufacturer_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            try
            {
                foreach (var gridRow in e.Rows)
                {
                    if (gridRow.IsDataRow)
                    {
                        var view = gridRow.ListObject as DataRowView;

                        if (view != null)
                        {
                            var row = view.Row as PartsDataset.d_ManufacturerRow;

                            if (row != null)
                            {
                                //if row is in use, do not delete
                                if (this.taManufacturer.GetUsageCount(row.ManufacturerID).GetValueOrDefault() > 0)
                                {
                                    e.Cancel = true;
                                    MessageBoxUtilities.ShowMessageBoxWarn("Unable to delete, manufacturer in use.", "Delete Manufacturer");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error deleting row.");
            }
        }

        private void grdManufacturer_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                grdManufacturer.AfterColPosChanged -= grdManufacturer_AfterColPosChanged;
                grdManufacturer.AfterSortChange -= grdManufacturer_AfterSortChange;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(grdManufacturer.DisplayLayout.Bands[0]);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing grid layout.");
            }
            finally
            {
                grdManufacturer.AfterColPosChanged += grdManufacturer_AfterColPosChanged;
                grdManufacturer.AfterSortChange += grdManufacturer_AfterSortChange;
            }
        }

        private void grdManufacturer_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdManufacturer.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column position.");
            }
        }

        
        private void grdManufacturer_AfterSortChange(object sender, BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdManufacturer.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column sort.");
            }
        }

        #endregion
    }
}