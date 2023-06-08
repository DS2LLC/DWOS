using System;
using System.Data;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Shared;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using NLog;

namespace DWOS.UI.Admin
{
    public partial class DepartmentManager: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("DepartmentManager", new UltraGridBandSettings());

        #endregion

        #region Methods

        public DepartmentManager()
        {
            this.InitializeComponent();
        }

        private void LoadData()
        {
            this.grdAirframe.DisplayLayout.Override.SupportDataErrorInfo = SupportDataErrorInfo.RowsAndCells;

            this.dsOrderStatus.EnforceConstraints = false;
            this.taDepartment.Fill(this.dsOrderStatus.d_Department);
            this.taApplicationSettings.Fill(this.dsApplicationSettings.ApplicationSettings);
            this.dsOrderStatus.EnforceConstraints = true;
        }

        private bool SaveData()
        {
            try
            {
                this.grdAirframe.UpdateData();

                if(this.dsOrderStatus.d_Department.HasErrors)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("Please correct any data errors before saving.", "Data Errors");
                    return false;
                }

                this.taDepartment.Update(this.dsOrderStatus.d_Department);

                //Update the settings to have the correct name used for the department
                foreach(var dept in this.dsOrderStatus.d_Department)
                {
                    if(!dept.IsSystemNameNull())
                    {
                        switch(dept.SystemName)
                        {
                            case "Part Marking":
                                ApplicationSettings.Current.DepartmentPartMarking = dept.DepartmentID;
                                var pm = this.dsApplicationSettings.ApplicationSettings.FindBySettingName("DepartmentPartMarking");
                                if(pm != null)
                                    pm.Value = dept.DepartmentID;
                                else
                                    this.dsApplicationSettings.ApplicationSettings.AddApplicationSettingsRow("DepartmentPartMarking", dept.DepartmentID);
                                break;
                            case "QA":
                                ApplicationSettings.Current.DepartmentQA = dept.DepartmentID;
                                var qa = this.dsApplicationSettings.ApplicationSettings.FindBySettingName("DepartmentQA");
                                if(qa != null)
                                    qa.Value = dept.DepartmentID;
                                else
                                    this.dsApplicationSettings.ApplicationSettings.AddApplicationSettingsRow("DepartmentQA", dept.DepartmentID);
                                break;
                            case "Sales":
                                ApplicationSettings.Current.DepartmentSales = dept.DepartmentID;
                                var sales = this.dsApplicationSettings.ApplicationSettings.FindBySettingName("DepartmentSales");
                                if(sales != null)
                                    sales.Value = dept.DepartmentID;
                                else
                                    this.dsApplicationSettings.ApplicationSettings.AddApplicationSettingsRow("DepartmentSales", dept.DepartmentID);
                                break;
                            case "Shipping":
                                ApplicationSettings.Current.DepartmentShipping = dept.DepartmentID;
                                var ship = this.dsApplicationSettings.ApplicationSettings.FindBySettingName("DepartmentShipping");
                                if(ship != null)
                                    ship.Value = dept.DepartmentID;
                                else
                                    this.dsApplicationSettings.ApplicationSettings.AddApplicationSettingsRow("DepartmentShipping", dept.DepartmentID);
                                break;
                            case "Outside Processing":
                                ApplicationSettings.Current.DepartmentOutsideProcessing = dept.DepartmentID;
                                var proc = this.dsApplicationSettings.ApplicationSettings.FindBySettingName(nameof(ApplicationSettings.DepartmentOutsideProcessing));
                                if(proc != null)
                                    proc.Value = dept.DepartmentID;
                                else
                                    this.dsApplicationSettings.ApplicationSettings.AddApplicationSettingsRow(nameof(ApplicationSettings.DepartmentOutsideProcessing), dept.DepartmentID);
                                break;
                            default:
                                break;
                        }
                    }
                }

                //save to database
                this.taApplicationSettings.Update(this.dsApplicationSettings.ApplicationSettings);

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
                if (this.SaveData())
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
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
            var gridRow = e.Row;

            if(gridRow.IsDataRow && !e.ReInitialize)
            {
                var view = gridRow.ListObject as DataRowView;

                if(view != null)
                {
                    var row = view.Row as OrderStatusDataSet.d_DepartmentRow;

                    if(row != null && !row.IsNull("DepartmentID"))
                    {
                        //if system row then or in use then display as bold
                        if(!row.IsSystemNameNull() || this.taDepartment.GetUsageCount(row.DepartmentID).GetValueOrDefault() > 0)
                            gridRow.Appearance.FontData.Bold = DefaultableBoolean.True;
                    }
                }
            }
        }

        private void grdAirframe_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            try
            {
                foreach(var gridRow in e.Rows)
                {
                    if(gridRow.IsDataRow)
                    {
                        var view = gridRow.ListObject as DataRowView;

                        if(view != null)
                        {
                            var row = view.Row as OrderStatusDataSet.d_DepartmentRow;

                            if(row != null)
                            {
                                //if system row then or in use then do not allow delete
                                if(!row.IsSystemNameNull())
                                {
                                    e.Cancel = true;
                                    MessageBoxUtilities.ShowMessageBoxWarn("Unable to delete system defined department.", "Delete Department");
                                }
                                else if(this.taDepartment.GetUsageCount(row.DepartmentID).GetValueOrDefault() > 0)
                                {
                                    e.Cancel = true;
                                    MessageBoxUtilities.ShowMessageBoxWarn("Unable to delete department in use.", "Delete Department");
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error deleting row.");
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
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column sort.");
            }
        }

        #endregion
    }
}