using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DWOS.UI.Admin.ReportSchedule
{
    /// <summary>
    /// View Model implementation for <see cref="ReportScheduleManager"/>.
    /// </summary>
    public class DialogViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// Occurs when the user accepts the dialog.
        /// </summary>
        public event EventHandler Accepted;

        private ReportScheduleItem _selectedItem;
        private Data.Datasets.ApplicationSettingsDataSet.ReportTypeDataTable dtReportType;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the report schedule items for this instance.
        /// </summary>
        public ObservableCollection<ReportScheduleItem> Items { get; } =
            new ObservableCollection<ReportScheduleItem>();

        /// <summary>
        /// Gets or sets the selected item for this instance.
        /// </summary>
        public ReportScheduleItem SelectedItem
        {
            get => _selectedItem;
            set => Set(nameof(SelectedItem), ref _selectedItem, value);
        }

        /// <summary>
        /// The save command.
        /// </summary>
        public ICommand Save { get; }

        #endregion


        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogViewModel"/> class.
        /// </summary>
        public DialogViewModel()
        {
            Save = new RelayCommand(DoSave, CanSave);
        }

        /// <summary>
        /// Loads dialog data from the database.
        /// </summary>
        public void LoadData()
        {
            // Load report types from the database.
            dtReportType = new Data.Datasets.ApplicationSettingsDataSet.ReportTypeDataTable();
            using (var taReportType = new ReportTypeTableAdapter())
            {
                taReportType.Fill(dtReportType);
            }

            foreach (var reportTypeRow in dtReportType)
            {
                Items.Add(new ReportScheduleItem(reportTypeRow));
            }
        }

        private void DoSave()
        {
            try
            {
                if (dtReportType == null)
                {
                    return;
                }

                // Save data
                using (var taReportType = new ReportTypeTableAdapter())
                {
                    taReportType.Update(dtReportType);
                }

                // Accept dialog
                dtReportType.Dispose();
                dtReportType = null;
                Accepted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, $"Error saving data in {nameof(ReportScheduleManager)}.");
            }
        }

        private bool CanSave() =>
            Items.All(i => string.IsNullOrEmpty(i.Error));

        #endregion
    }
}
