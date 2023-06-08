using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Reports;
using DWOS.UI.Utilities;


namespace DWOS.UI.Sales.BatchOrderPanels
{
    /// <summary>
    /// Interaction logic for AddBatchOrder.xaml
    /// </summary>
    public partial class AddBatchOrder : Window
    {
        #region Fields

        private Data.Datasets.OrderProcessingDataSet.OrderProcessSummaryDataTable _orderProcessingSummary;
        private BarcodeScanner _scanner = null;

        #endregion

        #region Properties

        public int? SelectedWO
        {
            get
            {
                if (cboOrders.SelectedItem != null)
                    return ((Data.Datasets.OrderProcessingDataSet.OrderProcessSummaryRow)((DataRowView)cboOrders.SelectedItem).Row).OrderID;

                return null;
            }
        }

        #endregion

        #region Methods

        public AddBatchOrder()
        {
            InitializeComponent();
        }

        public void LoadData(Data.Datasets.OrderProcessingDataSet.BatchRow batchRow)
        {
            try
            {
                _orderProcessingSummary = new Data.Datasets.OrderProcessingDataSet.OrderProcessSummaryDataTable();

                //load all active orders
                using(var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessSummaryTableAdapter())
                {
                    if(batchRow.GetBatchProcessesRows().Any())
                    {
                        //find first batch process, if found one filter list by only orders that are in this process
                        var firstProcess = batchRow.GetBatchProcessesRows().OrderBy(br => br.StepOrder).FirstOrDefault();

                        if(firstProcess != null)
                            ta.FillBy(_orderProcessingSummary, firstProcess.ProcessID, ApplicationSettings.Current.WorkStatusInProcess, Properties.Settings.Default.CurrentDepartment);
                        else
                            ta.FillByStatusLocation(_orderProcessingSummary, ApplicationSettings.Current.WorkStatusInProcess, Properties.Settings.Default.CurrentDepartment);
                    }
                    else
                        ta.FillByStatusLocation(_orderProcessingSummary, ApplicationSettings.Current.WorkStatusInProcess, Properties.Settings.Default.CurrentDepartment);
                }

                //remove all orders that are already in this batch
                foreach(var batchOrder in batchRow.GetBatchOrderRows())
                {
                    if(batchOrder.IsValidState())
                    {
                        var boProcess = _orderProcessingSummary.FirstOrDefault(ps => ps.OrderID == batchOrder.OrderID);
                        
                        if(boProcess != null)
                            _orderProcessingSummary.Rows.Remove(boProcess);
                    }
                }

                // Remove all orders that are already in other batches and have no more available parts to process
                var dtTemp = new OrderProcessingDataSet.BatchOrderDataTable();
                using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchOrderTableAdapter())
                    ta.Fill(dtTemp);

                var rowsToRemove = new List<Data.Datasets.OrderProcessingDataSet.OrderProcessSummaryRow>();
                foreach (var bop in _orderProcessingSummary)
                {
                    var batchOrders = dtTemp.Where(bo => bo.OrderID == bop.OrderID);

                    var usedPartCount = 0;
                    foreach (var bo in batchOrders)
                    {
                        using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
                        {
                            var dt = new OrderProcessingDataSet.BatchDataTable();
                            ta.FillBy(dt, bo.BatchID);

                            if (dt.Rows.Count > 0)
                            {
                                var batch = dt.Rows[0] as Data.Datasets.OrderProcessingDataSet.BatchRow;
                                if (batch != null && batch.Active)
                                    usedPartCount += bo.PartQuantity;
                            }
                        }
                    }

                    if (bop.IsPartQuantityNull() || usedPartCount >= bop.PartQuantity)
                    {
                        rowsToRemove.Add(bop);
                    }
                }
                
                foreach (var row in rowsToRemove)
                    _orderProcessingSummary.Rows.Remove(row);

                cboOrders.ItemsSource = _orderProcessingSummary.DefaultView;

                _scanner = new BarcodeScanner(Report.BARCODE_ORDER_ACTION_PREFFIX);
                _scanner.BarcodingFinished += _scanner_BarcodingFinished;
                _scanner.Start();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading data.");
            }
        }

        private void SelectBatch(int orderId)
        {
            cboOrders.SelectedItem = null;
            cboOrders.SelectedValuePath = "OrderID";
            cboOrders.SelectedValue = orderId;
        }

        #endregion

        #region Events
        
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            //this.Close();
        }

        private void _scanner_BarcodingFinished(object sender, BarcodeScanner.BarcodeScannerEventArgs e)
        {
            try
            {
                int orderId;

                if (e.Output != null && int.TryParse(e.Output, out orderId))
                {
                    if (e.Postfix == Report.BARCODE_ORDER_ACTION_PREFFIX)
                    {
                        SelectBatch(orderId);
                        
                        //if selected the item then close dialog
                        if(this.SelectedWO.GetValueOrDefault() == orderId)
                            btnOK_Click(this, new RoutedEventArgs());
                        else
                            MessageBoxUtilities.ShowMessageBoxWarn("Order '{0}' can not be added to this batch.".FormatWith(orderId), "Invalid Order");
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error during barcode finishing for Add Batch scanner.");
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                _orderProcessingSummary = null;
                
                if(_scanner != null)
                {
                    _scanner.BarcodingFinished -= _scanner_BarcodingFinished;
                    _scanner.Dispose();
                }

                _scanner = null;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error during closing.");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // This is used instead of the 'FocusManager.FocusedElement'
            // approach it's guaranteed to work whenever the window loads.
            cboOrders.Focus();
        }

        #endregion
    }
}
