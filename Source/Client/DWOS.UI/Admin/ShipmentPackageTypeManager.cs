using System;
using System.Data;
using DWOS.Shared;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Admin
{
    public partial class ShipmentPackageTypeManager : Form
    {
        #region Fields

        private GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("ShipmentPackageTypeManager", new UltraGridBandSettings());

        #endregion

        #region Methods

        public ShipmentPackageTypeManager()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            taPackageType.Fill(dsOrderDataset.ShipmentPackageType);
        }

        private bool SaveData()
        {
            try
            {
                grdPackageType.UpdateData();

                if (dsOrderDataset.ShipmentPackageType.HasErrors)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("Please correct any data errors before saving.",
                        "Data Errors");
                    return false;
                }

                taPackageType.Update(dsOrderDataset.ShipmentPackageType);

                return true;
            }
            catch (Exception exc)
            {
                string errorMsg = "Error saving data.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);

                return false;
            }
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (SaveData())
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error saving data.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void ShipmentPackageTypeManager_Load(object sender, EventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception exc)
            {
                string errorMsg = "Error loading shipping package type manager.";
                LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
            }
        }

        private void grdPackageType_BeforeRowsDeleted(object sender,
            Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
        {
            try
            {
                foreach (var gridRow in e.Rows)
                {
                    if (!gridRow.IsDataRow)
                    {
                        continue;
                    }

                    var view = gridRow.ListObject as DataRowView;


                    if (view?.Row is OrderShipmentDataSet.ShipmentPackageTypeRow row)
                    {
                        if (row.SystemDefined)
                        {
                            e.Cancel = true;
                            MessageBoxUtilities.ShowMessageBoxWarn(
                                "Unable to delete system-defined shipment package type.",
                                "Delete Shipment Container Type");
                        }
                        else if (taPackageType.GetUsageCount(row.ShipmentPackageTypeID) > 0)
                        {
                            e.Cancel = true;
                            MessageBoxUtilities.ShowMessageBoxWarn(
                                "Unable to delete in-use shipment package type.",
                                "Delete Shipment Container Type");
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error deleting row.");
            }
        }


        private void grdPackageType_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                grdPackageType.AfterColPosChanged -= grdPackageType_AfterColPosChanged;
                grdPackageType.AfterSortChange -= grdPackageType_AfterSortChange;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(grdPackageType.DisplayLayout.Bands[0]);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing grid layout.");
            }
            finally
            {
                grdPackageType.AfterColPosChanged += grdPackageType_AfterColPosChanged;
                grdPackageType.AfterSortChange += grdPackageType_AfterSortChange;
            }
        }

        private void grdPackageType_AfterColPosChanged(object sender, Infragistics.Win.UltraWinGrid.AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdPackageType.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column position.");
            }
        }

        private void grdPackageType_AfterSortChange(object sender, Infragistics.Win.UltraWinGrid.BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdPackageType.DisplayLayout.Bands[0]);
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
