using System;
using System.Data;
using System.Windows;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Shared;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;

using NLog;

namespace DWOS.UI.Processing
{
    public partial class BatchCheckInWindow : Window
    {
        #region Fields
        
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private Data.Datasets.OrderProcessingDataSet _dsOrderProcessing;
        private SecurityWatcher _watcher;

        #endregion

        #region Properties

        private OrderProcessingDataSet.BatchRow SelectedBatch { get; set; }

        #endregion

        #region Methods

        public BatchCheckInWindow()
        {
            InitializeComponent();

            _watcher = new SecurityWatcher(() => Close());
        }
        
        public bool LoadData(int? batchId)
        {
            try
            {
                this.txtDepartment.Text = Settings.Default.CurrentDepartment;
                
                _dsOrderProcessing = new Data.Datasets.OrderProcessingDataSet();

                using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
                    ta.FillByActive(_dsOrderProcessing.Batch);

                cboBatch.ItemsSource = _dsOrderProcessing.Batch.DefaultView;
                
                if(batchId.HasValue)
                {
                    //select batch
                    cboBatch.Text = batchId.Value.ToString();
                }

                return true;
            }
            catch (Exception exc)
            {
                _log.Warn("DataSet Errors: " + _dsOrderProcessing.GetDataErrors());
                _log.Error(exc, "Error loading data for batch.");
                return false;
            }
        }

        private bool SaveData()
        {
            try
            {
                if(SelectedBatch == null)
                    return false;

                _log.Info("Checking Batch " + SelectedBatch.BatchID + " into " + Settings.Default.CurrentDepartment + ".");

                var checkIn = new BatchCheckInController(SelectedBatch.BatchID);
                return checkIn.CheckIn(Properties.Settings.Default.CurrentDepartment, SecurityManager.Current.UserID).Response;
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error saving data.", exc);
                return false;
            }
        }

        private bool IsValidSelection()
        {
            this.SelectedBatch = null;

            if(cboBatch.SelectedItem is DataRowView)
            {
                this.SelectedBatch = ((DataRowView)cboBatch.SelectedItem).Row as OrderProcessingDataSet.BatchRow;
                
                if(this.SelectedBatch != null)
                {
                    var checkIn = new BatchCheckInController(SelectedBatch.BatchID);
                    var checkInResults = checkIn.IsValid(Settings.Default.CurrentDepartment, Settings.Default.CurrentLine);

                    if(!checkInResults.Response)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(checkInResults.Description, "Batch Check In");
                        return false;
                    }
                    else
                        return true;
                }
            }

            return false;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }

            base.OnClosed(e);
        }

        #endregion

        #region Events

        private void MetroWindow_Initialized(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnOK.IsEnabled = false;
                if (IsValidSelection() && SaveData())
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error on closing batch check in.", exc);
            }
            finally
            {
                btnOK.IsEnabled = true;
            }
        }
        
        #endregion
    }
}
