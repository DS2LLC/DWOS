using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Admin.Schedule
{
    public partial class ProcessLeadTimeSettings : Form
    {
        #region Fields

        private Dictionary <Data.Datasets.ScheduleDataset.d_ProcessCategoryRow, int> _rowUsages = new Dictionary <Data.Datasets.ScheduleDataset.d_ProcessCategoryRow, int>();
        private ISet<string> _categoriesToKeep = new HashSet<string>();
        private GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("ProcessLeadTimeSettings", new UltraGridBandSettings());

        #endregion

        #region Methods

        public ProcessLeadTimeSettings()
        {
            InitializeComponent();
        }
        public void DoNotAllowDeletionOf(IEnumerable<string> processCategories)
        {
            if (processCategories == null)
            {
                return;
            }

            foreach (var categoryID in processCategories)
            {
                _categoriesToKeep.Add(categoryID);
            }
        }

        private void LoadData()
        {
            taProcessCategory.Fill(dsSchedule.d_ProcessCategory);
        }

        private void SaveData()
        {
            taProcessCategory.Update(dsSchedule.d_ProcessCategory);
        }

        private bool IsInUse(Data.Datasets.ScheduleDataset.d_ProcessCategoryRow categoryRow)
        {
            if (_categoriesToKeep.Contains(categoryRow.ProcessCategory))
            {
                return true;
            }

            if (!_rowUsages.ContainsKey(categoryRow))
            {
                _rowUsages.Add(categoryRow, taProcessCategory.GetUsageCount(categoryRow.ProcessCategory).GetValueOrDefault());
            }

            return _rowUsages[categoryRow] > 0;
        }

        #endregion

        #region Events

        private void ProcessLeadTimeSettings_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveData();
            this.Close();
        }

        private void grdProcessCat_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
        {
            foreach(var row in e.Rows)
            {
                if (!row.IsDataRow)
                {
                    continue;
                }

                var view = row.ListObject as DataRowView;
                
                if (view != null)
                {
                    var catRow = view.Row as Data.Datasets.ScheduleDataset.d_ProcessCategoryRow;

                    if(catRow != null && IsInUse(catRow))
                    {
                        e.Cancel = true;
                        e.DisplayPromptMsg = false;
                        MessageBoxUtilities.ShowMessageBoxWarn("Unable to delete categories that are in use.", "Unable to Delete");
                        return;
                    }
                }
            }
        }

        private void grdProcessCat_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                grdProcessCat.AfterColPosChanged -= grdProcessCat_AfterColPosChanged;
                grdProcessCat.AfterSortChange -= grdProcessCat_AfterSortChange;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(grdProcessCat.DisplayLayout.Bands[0]);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing grid layout.");
            }
            finally
            {
                grdProcessCat.AfterColPosChanged += grdProcessCat_AfterColPosChanged;
                grdProcessCat.AfterSortChange += grdProcessCat_AfterSortChange;
            }
        }

        private void grdProcessCat_AfterColPosChanged(object sender, Infragistics.Win.UltraWinGrid.AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdProcessCat.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column position.");
            }
        }

        private void grdProcessCat_AfterSortChange(object sender, Infragistics.Win.UltraWinGrid.BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdProcessCat.DisplayLayout.Bands[0]);
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
