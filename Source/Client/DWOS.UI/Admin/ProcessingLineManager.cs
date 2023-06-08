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
    public partial class ProcessingLineManager: Form
    {
        #region Fields

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("ProcessingLineManager", new UltraGridBandSettings());

        #endregion

        #region Methods

        public ProcessingLineManager()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            grdProcessingLine.DisplayLayout.Override.SupportDataErrorInfo = SupportDataErrorInfo.RowsAndCells;

            using (new UsingDataSetLoad(dsOrderStatus))
            {
                taDepartment.Fill(dsOrderStatus.d_Department);
                taProcessingLine.Fill(dsOrderStatus.ProcessingLine);
            }

            // Department list
            var dbDepartmentColumn = dsOrderStatus.ProcessingLine.DepartmentIDColumn.ColumnName;

            var departmentColumn = grdProcessingLine.DisplayLayout.Bands[0].Columns[dbDepartmentColumn];

            var departmentValueList = new ValueList();
            departmentValueList.ValueListItems.Add(null, string.Empty);

            foreach (var departmentRow in dsOrderStatus.d_Department.OrderBy(d => d.DepartmentID))
            {
                departmentValueList.ValueListItems.Add(departmentRow.DepartmentID);
            }

            departmentColumn.ValueList = departmentValueList;
        }

        private bool SaveData()
        {
            try
            {
                grdProcessingLine.UpdateData();

                if(dsOrderStatus.ProcessingLine.HasErrors)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("Please correct any data errors before saving.", "Data Errors");
                    return false;
                }

                taProcessingLine.Update(dsOrderStatus.ProcessingLine);

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

        private void AirframeManager_Load(object sender, EventArgs e)
        {
            try
            {
                LoadData();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error loading processing line manager.";
                _log.Error(exc, errorMsg);
            }
        }

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
            catch(Exception exc)
            {
                string errorMsg = "Error saving data.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void grdProcessingLine_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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

                    var row = view?.Row as OrderStatusDataSet.ProcessingLineRow;

                    if (row != null && taProcessingLine.GetUsageCount(row.ProcessingLineID).GetValueOrDefault() > 0)
                    {
                        e.Cancel = true;
                        MessageBoxUtilities.ShowMessageBoxWarn("Unable to delete processing line that was previously used.",
                            "Delete Processing Line");
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error deleting row.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void grdProcessingLine_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                grdProcessingLine.AfterColPosChanged -= grdProcessingLine_AfterColPosChanged;
                grdProcessingLine.AfterSortChange -= grdProcessingLine_AfterSortChange;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(grdProcessingLine.DisplayLayout.Bands[0]);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing grid layout.");
            }
            finally
            {
                grdProcessingLine.AfterColPosChanged += grdProcessingLine_AfterColPosChanged;
                grdProcessingLine.AfterSortChange += grdProcessingLine_AfterSortChange;
            }
        }

        private void grdProcessingLine_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdProcessingLine.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column position.");
            }
        }

        private void grdProcessingLine_AfterSortChange(object sender, BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdProcessingLine.DisplayLayout.Bands[0]);
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