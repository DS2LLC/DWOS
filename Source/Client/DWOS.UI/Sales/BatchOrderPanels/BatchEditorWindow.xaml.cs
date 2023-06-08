using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;
using Infragistics.Windows.DataPresenter;
using NLog;
using DWOS.UI.Reports;

namespace DWOS.UI.Sales.BatchOrderPanels
{
    public partial class BatchEditorWindow : Window
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private const int MAX_PROCESSES_MULTIPLE = 10;
        private const int MAX_PROCESSES_SINGLE = 1;

        private Data.Datasets.OrderProcessingDataSet _dsOrderProcessing;
        private SortableObservableCollection<BatchOrderInfo> _batchOrders;
        private SortableObservableCollection<BatchProcessInfo> _batchProcesses;
        private bool _updatingCalculator;
        private int _maxProcesses = ApplicationSettings.Current.BatchMultipleProcesses ? MAX_PROCESSES_MULTIPLE : MAX_PROCESSES_SINGLE;

        #endregion

        #region Properties

        public Data.Datasets.OrderProcessingDataSet.BatchRow CurrentBatch { get; set; }

        #endregion

        #region Methods

        public BatchEditorWindow()
        {
            InitializeComponent();
            Icon = Properties.Resources.Batch32.ToWpfImage();
            SecurityManager.Current.UserUpdated += Current_UserUpdated;
        }

        public bool LoadData(int? batchId)
        {
            try
            {
                _log.Debug("Loading Data for batch {0}.".FormatWith(batchId.GetValueOrDefault()));

                btnProcessRemove.Visibility = ApplicationSettings.Current.BatchMultipleProcesses ?  System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

                _dsOrderProcessing = new Data.Datasets.OrderProcessingDataSet() { EnforceConstraints = false };

                if (batchId.HasValue && batchId.Value > 0)
                {
                    using (new UsingDataSetLoad(_dsOrderProcessing))
                    {
                        using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
                            ta.FillBy(_dsOrderProcessing.Batch, batchId.Value);

                        using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchOrderTableAdapter())
                            ta.FillBy(_dsOrderProcessing.BatchOrder, batchId.Value);

                        using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTableAdapter())
                            ta.FillBy(_dsOrderProcessing.BatchProcesses, batchId.Value);

                        using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcess_OrderProcessTableAdapter())
                            ta.FillByBatch(_dsOrderProcessing.BatchProcess_OrderProcess, batchId.Value);

                        using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.PartTableAdapter())
                            ta.FillByBatch(_dsOrderProcessing.Part, batchId.Value);

                        using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
                            ta.FillByBatch2(_dsOrderProcessing.OrderSummary, batchId.Value);

                        using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter())
                            ta.FillByBatch(_dsOrderProcessing.Process, batchId.Value);

                        using (var ta = new ProcessingLineTableAdapter())
                        {
                            ta.Fill(_dsOrderProcessing.ProcessingLine);
                        }
                    }

                    this.CurrentBatch = _dsOrderProcessing.Batch.FindByBatchID(batchId.Value);
                }
                else
                {
                    CurrentBatch = _dsOrderProcessing.Batch.NewBatchRow();
                    CurrentBatch.Active = true;
                    CurrentBatch.OpenDate = DateTime.Now;
                    CurrentBatch.WorkStatus = ApplicationSettings.Current.WorkStatusChangingDepartment;
                    CurrentBatch.CurrentLocation = Settings.Default.CurrentDepartment;
                    CurrentBatch.AmpsPerSquareFoot = 0;
                    _dsOrderProcessing.Batch.AddBatchRow(CurrentBatch);

                    CurrentBatch.SetCloseDateNull();
                    CurrentBatch.SetAmpsPerSquareFootNull();
                    using (new UsingDataSetLoad(_dsOrderProcessing))
                    {
                        using (var ta = new ProcessingLineTableAdapter())
                        {
                            ta.Fill(_dsOrderProcessing.ProcessingLine);
                        }
                    }
                }

                txtBatch.Text = this.CurrentBatch.BatchID.ToString();
                txtFixture.Text = this.CurrentBatch.IsFixtureNull() ? null : this.CurrentBatch.Fixture;
                txtLocation.Text = this.CurrentBatch.CurrentLocation;
                txtStatus.Text = this.CurrentBatch.IsWorkStatusNull() ? null : this.CurrentBatch.WorkStatus;

                var nextDepartment = CurrentBatch.GetBatchProcessesRows().OrderBy(bp => bp.StepOrder).FirstOrDefault(bp => bp.IsEndDateNull())?.Department ?? CurrentBatch.CurrentLocation;

                var processingLines = ProcessingLineItem.AllLineItems(_dsOrderProcessing.ProcessingLine)
                    .Where(line => line.Department == null || line.Department == nextDepartment ||(!CurrentBatch.IsCurrentLineNull() && line.ProcessingLineId == CurrentBatch.CurrentLine))
                    .ToList();

                cboLine.ItemsSource = processingLines;

                cboLine.SelectedItem = CurrentBatch.IsCurrentLineNull()
                    ? processingLines.FirstOrDefault(l => !l.ProcessingLineId.HasValue)
                    : processingLines.FirstOrDefault(l => l.ProcessingLineId == CurrentBatch.CurrentLine);

                //fill batch orders
                _batchOrders = new SortableObservableCollection<BatchOrderInfo>();

                foreach (var bo in _dsOrderProcessing.BatchOrder)
                {
                    var order = _dsOrderProcessing.OrderSummary.FindByOrderID(bo.OrderID);
                    _batchOrders.Add(new BatchOrderInfo(bo, order.PartRow));
                }

                //fill batch processes
                _batchProcesses = new SortableObservableCollection<BatchProcessInfo>();

                foreach (var bp in _dsOrderProcessing.BatchProcesses)
                {
                    _batchProcesses.Add(new BatchProcessInfo(bp));
                }

                this.grdOrders.DataSource = _batchOrders;
                this.grdProcesses.DataSource = _batchProcesses;

                //Enable buttons
                bool allowAdd = !_batchProcesses.Any(bp => bp.EndDate.HasValue);

                if (allowAdd && batchId.HasValue)
                {
                    int answeredCount = 0;
                    using (var taOrderProcessAnswer = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessAnswerTableAdapter())
                    {
                        answeredCount = taOrderProcessAnswer.GetCompletedCountByBatch(batchId.Value).GetValueOrDefault();
                    }

                    allowAdd = answeredCount == 0;
                }

                btnAddOrder.IsEnabled = allowAdd;

                btnProcessRemove.IsEnabled = grdProcesses.SelectedItems.Records.OfType<DataRecord>().Any();
                btnDeleteOrder.IsEnabled = grdOrders.SelectedItems.Records.OfType<DataRecord>().Any();

                btnSplitOrder.IsEnabled = grdOrders.SelectedItems.Records.OfType<DataRecord>().Any();
                btnSplitOrder.Visibility = SecurityManager.Current.IsInRole("OrderEntry.Edit") ? Visibility.Visible : Visibility.Collapsed;

                var lineVisibility = ApplicationSettings.Current.MultipleLinesEnabled ? Visibility.Visible : Visibility.Collapsed;
                lblLine.Visibility = lineVisibility;
                cboLine.Visibility = lineVisibility;

                //initialize partial load info
                HasPartialLoad();

                return true;
            }
            catch (Exception exc)
            {
                _log.Warn("DataSet Errors: " + _dsOrderProcessing.GetDataErrors());
                _log.Error(exc, "Error loading data for batch.");
                return false;
            }
        }

        private void ShowAddBatchOrderDialog()
        {
            try
            {
                while (true)
                {
                    var addBatchOrderDialog = new BatchOrderPanels.AddBatchOrder(){Owner = this};
                    addBatchOrderDialog.LoadData(this.CurrentBatch);

                    if(addBatchOrderDialog.ShowDialog().GetValueOrDefault())
                    {
                        if (addBatchOrderDialog.SelectedWO.HasValue)
                            this.AddOrder(addBatchOrderDialog.SelectedWO.Value);
                        else
                        {
                            Sound.BeepError();
                            MessageBoxUtilities.ShowMessageBoxWarn("No valid orders selected to be processed, ensure it exists and is in the correct department and work state.", "Invalid Order");
                        }
                    }
                    else
                        break;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding order batch item.");
            }
        }

        private void AddOrder(int orderId)
        {
            _log.Debug("Adding order {0}.".FormatWith(orderId));

            bool isFirstOrderAdded = _batchOrders.Count == 0;

            var order = _dsOrderProcessing.OrderSummary.FindByOrderID(orderId);

            //load order
            if(order == null)
            {
                using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter() {ClearBeforeFill = false})
                    ta.FillById(_dsOrderProcessing.OrderSummary, orderId);

                order = _dsOrderProcessing.OrderSummary.FindByOrderID(orderId);

                using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.PartTableAdapter() { ClearBeforeFill = false })
                    ta.FillByPart(_dsOrderProcessing.Part, order.PartID);
            }

            if(!IsValidOrder(order))
                return;

            var usedPartCount = 0;

            // Figure out how many parts are available based on parts used in other batches
            var dtBatchOrder = new OrderProcessingDataSet.BatchOrderDataTable();
            using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchOrderTableAdapter())
                ta.FillByOrder(dtBatchOrder, order.OrderID);

            using (var taBatch = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
            {
                foreach (var bo in dtBatchOrder)
                {
                    var dtBatch = new OrderProcessingDataSet.BatchDataTable();
                    taBatch.FillBy(dtBatch, bo.BatchID);

                    if (dtBatch.Rows.Count > 0)
                    {
                        var batchRow = dtBatch.Rows[0] as Data.Datasets.OrderProcessingDataSet.BatchRow;
                        if (batchRow != null && batchRow.Active)
                            usedPartCount += bo.PartQuantity;
                    }
                }
            }

            var orderQuantity = order.IsPartQuantityNull() ? 0 : order.PartQuantity;
            var batchPartQuantity = orderQuantity - usedPartCount;

            var bachOrderRow = _dsOrderProcessing.BatchOrder.AddBatchOrderRow(this.CurrentBatch, orderId, batchPartQuantity);
            var batchInfo = new BatchOrderInfo(bachOrderRow, order.PartRow);
            batchInfo.PropertyChanged += batchInfo_PropertyChanged;
            _batchOrders.Add(batchInfo);

            ValidateProcesses(isFirstOrderAdded);
        }

        private bool IsValidOrder(Data.Datasets.OrderProcessingDataSet.OrderSummaryRow order)
        {
            _log.Debug("Checking if order is valid {0}.".FormatWith(order.OrderID));

            //can be in process or changing departments
            if(order.WorkStatus != ApplicationSettings.Current.WorkStatusChangingDepartment && order.WorkStatus != ApplicationSettings.Current.WorkStatusInProcess)
                return false;

            //must be in same department
            if (order.CurrentLocation != this.CurrentBatch.CurrentLocation)
                return false;

            var currentBatchProcess = _batchProcesses.FirstOrDefault(bp => !bp.EndDate.HasValue);

            if(currentBatchProcess != null)
            {
                var firstBatchProcessId = currentBatchProcess.BatchProcess.ProcessID;

                using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter() { ClearBeforeFill = false })
                    ta.FillBy(_dsOrderProcessing.OrderProcesses, order.OrderID);

                var currentOrderProcess = _dsOrderProcessing.OrderProcesses.FirstOrDefault(op => op.OrderID == order.OrderID && op.IsEndDateNull());

                //next process of order does not match current process of this batch
                if(currentOrderProcess == null || currentOrderProcess.ProcessID != firstBatchProcessId)
                    return false;
            }

            return true;
        }

        private void ValidateProcesses(bool doInitialSync)
        {
            _log.Debug("Begin validating processes.");

            if(_batchOrders == null || _batchOrders.Count < 1)
                return;

            //load all data
            foreach(var bo in _batchOrders)
            {
                //if not loaded this WO
                if(!_dsOrderProcessing.OrderProcesses.Any(op => op.OrderID == bo.WO))
                {
                    using(var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter() {ClearBeforeFill = false})
                        ta.FillByOrder(_dsOrderProcessing.Process, bo.WO);

                    using(var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter() {ClearBeforeFill = false})
                        ta.FillBy(_dsOrderProcessing.OrderProcesses, bo.WO);
                }
            }

            //update max processes based on our orders
            _maxProcesses = ApplicationSettings.Current.BatchMultipleProcesses && !HasPartialLoad() ? MAX_PROCESSES_MULTIPLE : MAX_PROCESSES_SINGLE;

            if(_batchOrders.Count == 1 && doInitialSync)
            {
                _log.Debug("Adding initial list of processes.");

                //get all processes that are not completed
                var processes = _dsOrderProcessing.OrderProcesses.Where(op => op.IsEndDateNull() && op.OrderID == _batchOrders[0].WO).OrderBy(s => s.StepOrder);

                //get all process id's based on max processes we can handle
                var  requireProcesses = processes.Take(_maxProcesses).Select(op => op.ProcessID).ToArray();
                SyncProcesses(requireProcesses);
            }
            else
            {
                _log.Debug("Multiple orders to add processes from.");

                //get all current processes
                var currentProcesses = _batchProcesses.Select(bp => bp.BatchProcess.ProcessID).Take(_maxProcesses).ToList();

                //see which ones we have to remove
                for(var index = 0; index < currentProcesses.Count; index++)
                {
                    var processId = currentProcesses[index];
                    var isValid = true;

                    foreach(var bo in _batchOrders)
                    {
                        var boProcesses = _dsOrderProcessing.OrderProcesses.Where(op => op.IsEndDateNull() && op.OrderID == bo.WO).OrderBy(s => s.StepOrder).ToList();

                        //if doesn't have this many processes OR the processes do not match at this index
                        if(index >= boProcesses.Count || boProcesses[index].ProcessID != processId)
                        {
                            isValid = false;
                            break;
                        }
                    }

                    //if hit a non valid process then null this process id out and all remaining processes
                    if(!isValid)
                    {
                        _log.Debug("Hit invalid process nulling all remaining out from {0} to {1}.", index, currentProcesses.Count);

                        for (var index2 = index; index2 < currentProcesses.Count; index2++)
                            currentProcesses[index2] = 0;

                        break;
                    }
                }

                SyncProcesses(currentProcesses.ToArray());
            }
        }

        private void SyncProcesses(int[] processes)
        {
            _log.Debug("Begin syncing {0} processes.".FormatWith(processes.Length));

            // Reset BatchProcess <-> OrderProcess relations
            foreach (var bo in _batchProcesses)
            {
                foreach (var bpop in bo.BatchProcess.GetBatchProcess_OrderProcessRows())
                {
                    bpop.Delete();
                }
            }

            // Relations that have been removed throughout this
            // dialog's lifetime may need to be reverted later in this method.
            IEnumerable<DataRow> removedRelations = new DataView(_dsOrderProcessing.BatchProcess_OrderProcess, string.Empty, string.Empty, DataViewRowState.Deleted)
                .OfType<DataRowView>()
                .Select(view => view.Row);

            _log.Debug("Updating batch processes.");

            //update batch process rows
            for(int index = 0; index < MAX_PROCESSES_MULTIPLE; index++)
            {
                var processID       = index <= processes.GetUpperBound(0) ?  processes[index] : 0;
                var currentProcess  =  _dsOrderProcessing.BatchProcesses.FirstOrDefault(w => w.IsValidState() && w.StepOrder == index + 1);

                if (processID < 1) //if no process required at this index
                {
                    //if we have a process in this slot then remove it
                    if (currentProcess != null)
                        currentProcess.Delete();
                }
                else if (currentProcess != null) //if does have a current process
                {
                    //if processes don't match then match them
                    if(currentProcess.ProcessID != processID)
                        currentProcess.ProcessID = processID;
                }
                else //does not have current process
                {
                    var process = _dsOrderProcessing.Process.FindByProcessID(processID);
                    currentProcess = _dsOrderProcessing.BatchProcesses.AddBatchProcessesRow(CurrentBatch, process, index + 1, process.Department, DateTime.MinValue, DateTime.MinValue, 0);
                    currentProcess.SetEndDateNull();
                    currentProcess.SetStartDateNull();
                    currentProcess.SetProcessDurationMinutesNull();
                }
            }

            _log.Debug("Verifying batch process step order.");

            //verify step order of processes are correct
            var batchProcesses = _dsOrderProcessing.BatchProcesses.Where(w => w.IsValidState()).OrderBy(s => s.StepOrder).ToArray();
            var stepOrder = 1;

            foreach (var bp in batchProcesses)
            {
                bp.StepOrder = stepOrder;
                stepOrder++;
            }

            //relate all order processes to the batch process
            var batchProcessIDColumn = _dsOrderProcessing.BatchProcess_OrderProcess.BatchProcessIDColumn;
            var orderProcessIDColumn = _dsOrderProcessing.BatchProcess_OrderProcess.OrderProcessIDColumn;

            foreach(var bo in _dsOrderProcessing.BatchOrder.Where(w => w.IsValidState()))
            {
                var orderProcesses = _dsOrderProcessing.OrderProcesses.Where(op => op.OrderID == bo.OrderID && op.IsEndDateNull()).OrderBy(op => op.StepOrder).ToList();

                foreach (var batchProcess in batchProcesses)
                {
                    var op = orderProcesses.FirstOrDefault(w => w.ProcessID == batchProcess.ProcessID);
                    if(op != null)
                    {
                        var removedRelationMatch = removedRelations.FirstOrDefault(
                            (removedRelation) =>
                            {
                                var removedBatchProcessID = Convert.ToInt32(removedRelation[batchProcessIDColumn, DataRowVersion.Original]);
                                var removedOrderProcessID = Convert.ToInt32(removedRelation[orderProcessIDColumn, DataRowVersion.Original]);

                                return removedBatchProcessID == batchProcess.BatchProcessID &&
                                       removedOrderProcessID == op.OrderProcessesID;
                            });

                        if (removedRelationMatch == null)
                        {
                            _dsOrderProcessing.BatchProcess_OrderProcess.AddBatchProcess_OrderProcessRow(batchProcess, op);

                            foreach (var opr in op.GetBatchProcess_OrderProcessRows())
                            {
                                opr.BatchProcessID = batchProcess.BatchProcessID;
                            }

                            orderProcesses.Remove(op);
                        }
                        else
                        {
                            _log.Debug("Matched deleted relation for BPID = {0}, OPID = {1}", batchProcess.BatchProcessID, op.OrderProcessesID);

                            // During save, inserts & updates occur before deletions.
                            // Revert a previously removed relationship instead of adding
                            // a duplicate that crashes the dialog.
                            removedRelationMatch.RejectChanges();
                        }
                    }
                }
            }

            //update view models
            _batchProcesses.Clear();

            foreach (var bp in batchProcesses)
            {
                _batchProcesses.Add(new BatchProcessInfo(bp));
            }
        }

        private bool AutoCheckInBatch()
        {
            //if NEW batch
            if (CurrentBatch.BatchID < 0)
            {
                _log.Debug("Auto checking in batch.");
                var firstProcess = CurrentBatch.GetBatchProcessesRows().OrderBy(ob => ob.StepOrder).FirstOrDefault();

                //if first process is not started and is in current department then check it in
                if(firstProcess != null && firstProcess.IsStartDateNull() && firstProcess.Department == Properties.Settings.Default.CurrentDepartment)
                {
                    firstProcess.StartDate = DateTime.Now;

                    CurrentBatch.WorkStatus = ApplicationSettings.Current.WorkStatusInProcess;
                    CurrentBatch.CurrentLocation = Properties.Settings.Default.CurrentDepartment; //should already be set to this, just for completeness

                    //check in all order processes also to keep in sync
                    foreach(var row in firstProcess.GetBatchProcess_OrderProcessRows())
                    {
                        row.OrderProcessesRow.StartDate = firstProcess.StartDate;
                    }

                    return true;
                }
            }

            return false;
        }

        private bool SaveData()
        {
            try
            {
                if(this.CurrentBatch == null)
                    return false;

                _log.Debug("Saving Data.");
                this.CurrentBatch.Fixture = txtFixture.Text;

                var processingLineId = (cboLine.SelectedItem as ProcessingLineItem)?.ProcessingLineId;
                if (processingLineId.HasValue)
                {
                    CurrentBatch.CurrentLine = processingLineId.Value;
                }
                else
                {
                    CurrentBatch.SetCurrentLineNull();
                }

                //automatically check in batch if new and right department
                var checkedIn = AutoCheckInBatch();

                using (var taManager = new Data.Datasets.OrderProcessingDataSetTableAdapters.TableAdapterManager())
                {
                    taManager.BatchTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter();
                    taManager.BatchOrderTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchOrderTableAdapter();
                    taManager.BatchProcessesTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTableAdapter();
                    taManager.OrderProcessesTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter();
                    taManager.BatchProcess_OrderProcessTableAdapter = new BatchProcess_OrderProcessTableAdapter();

                    taManager.UpdateAll(_dsOrderProcessing);

                    //update that all batches have been added
                    foreach(var batchOrder in CurrentBatch.GetBatchOrderRows())
                        OrderHistoryDataSet.UpdateOrderHistory(batchOrder.OrderID, "Batch Editor", "Order " + batchOrder.OrderID + " added to  batch " + this.CurrentBatch.BatchID + ".", SecurityManager.Current.UserName);

                    using (var taOrder = new OrderSummaryTableAdapter())
                    {
                        foreach(var batchOrder in CurrentBatch.GetBatchOrderRows())
                        {
                            if (checkedIn)
                            {
                                taOrder.UpdateWorkStatus(ApplicationSettings.Current.WorkStatusInProcess, batchOrder.OrderID);
                                taOrder.UpdateOrderLocation(Settings.Default.CurrentDepartment, batchOrder.OrderID);

                                OrderHistoryDataSet.UpdateOrderHistory(batchOrder.OrderID, "Batch Editor", "Order " + batchOrder.OrderID + " checked in to " + Settings.Default.CurrentDepartment + ".", SecurityManager.Current.UserName);
                            }

                            // Stop order timers in case someone started one
                            Data.Order.TimeCollectionUtilities.StopAllOrderTimers(batchOrder.OrderID);
                        }
                    }
                }

                return true;
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error saving data.", exc);
                return false;
            }
        }

        private void ShowSplitOrderDialog(BatchOrderInfo orderInfo)
        {
            try
            {
                _log.Info("Splitting batch order {0} within batch {1}.".FormatWith(orderInfo.BatchOrderRow.BatchOrderID, orderInfo.BatchOrderRow.BatchID));

                var splitOrderDialog = new BatchOrderPanels.SplitBatchOrder() { Owner = this };
                splitOrderDialog.LoadData(orderInfo.BatchOrderRow, orderInfo.MaxQuantity);

                //get split info
                if (splitOrderDialog.ShowDialog().GetValueOrDefault())
                {
                    //update the order summary with the new orders part qty, used to determine if this order is a partial load or not
                    var order = _dsOrderProcessing.OrderSummary.FindByOrderID(orderInfo.BatchOrderRow.OrderID);

                    if (order != null)
                        order.PartQuantity = splitOrderDialog.OrderQuantity;

                    //reset batch order info to the new order qty
                    orderInfo.MaxQuantity = splitOrderDialog.OrderQuantity;
                    orderInfo.TotalQuantity = splitOrderDialog.OrderQuantity;
                    orderInfo.Quantity = splitOrderDialog.OrderQuantity; //trigger to update UI
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error on splitting order.");
            }
        }

        private void PrintTraveler()
        {
            try
            {
                if (CurrentBatch != null)
                {
                    _log.Debug("Print Traveler.");
                    var report = new BatchTraveler2Report(CurrentBatch.BatchID);

                    if (chkPrint.IsChecked.GetValueOrDefault())
                        report.PrintReport();
                    else
                        report.DisplayReport();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error printing batch traveler.");
            }
        }

        private void RemoveSelectedOrder()
        {
            try
            {
                _log.Debug("Remove selected orders.");
                var selectedRecords = grdOrders.SelectedItems.Records;

                foreach (var record in selectedRecords.OfType<DataRecord>())
                {
                    var recordToRemove = record.DataItem as BatchOrderInfo;

                    if (recordToRemove != null)
                    {
                        recordToRemove.PropertyChanged -= batchInfo_PropertyChanged;
                        _batchOrders.Remove(recordToRemove);

                        if (recordToRemove.BatchOrderRow != null)
                        {
                            recordToRemove.BatchOrderRow.Delete();
                        }
                    }
                }

                ValidateProcesses(false);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error removing selected order.");
            }
        }

        private void RemoveSelectedProcess()
        {
            try
            {
                _log.Debug("Remove selected processes.");
                var selectedRecords = grdProcesses.SelectedItems.Records;
                var items2remove = new List<BatchProcessInfo>();

                foreach (var record in selectedRecords.OfType<DataRecord>())
                    items2remove.Add(record.DataItem as BatchProcessInfo);

                foreach (var bo in items2remove)
                {
                    _batchProcesses.Remove(bo);
                    foreach (var bpop in bo.BatchProcess.GetBatchProcess_OrderProcessRows())
                    {
                        bpop.Delete();
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error removing selected process.");
            }
        }

        private bool HasPartialLoad()
        {
            var isPartialOrders = false;

            foreach(var bo in _batchOrders)
            {
                var order = _dsOrderProcessing.OrderSummary.FindByOrderID(bo.WO);
                if(order != null && !order.IsPartQuantityNull() && bo.Quantity != order.PartQuantity)
                {
                    isPartialOrders = true;
                    break;
                }
            }

            lblPartialOrders.Visibility = isPartialOrders ? Visibility.Visible : Visibility.Collapsed;
            return isPartialOrders;
        }

        private void LoadCalculator()
        {
            try
            {
                txtAmpsSF.TextChanged -= txtAmpsSF_TextChanged;
                if (CurrentBatch == null || CurrentBatch.IsAmpsPerSquareFootNull())
                {
                    txtAmpsSF.Text = 3.ToString();
                }
                else
                {
                    txtAmpsSF.Text = CurrentBatch.AmpsPerSquareFoot.ToString("N2");
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error loading amperage calculator data.");
            }
            finally
            {
                txtAmpsSF.TextChanged += txtAmpsSF_TextChanged;
            }
        }

        private void UpdateCalculator()
        {
            try
            {
                if (_updatingCalculator || _batchOrders == null)
                    return;

                _updatingCalculator = true;

                var totalSA = _batchOrders.Sum(bo => (bo.SurfaceArea * 0.0069444));

                txtTotalSurfaceArea.Text = totalSA.ToString("N2");
                txtTotalAmps.Text        = "0";

                if (totalSA > 0)
                {
                    var ampsPerSquareFoot = 0.0;

                    if (double.TryParse(txtAmpsSF.Text, out ampsPerSquareFoot))
                    {
                        if (CurrentBatch != null)
                        {
                            CurrentBatch.AmpsPerSquareFoot = ampsPerSquareFoot;
                        }

                        txtTotalAmps.Text = (totalSA * ampsPerSquareFoot).ToString("N2");
                    }
                }

                _updatingCalculator = false;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error update surface area.");
            }
        }

        /// <summary>
        /// Updates the calculator input values based on the answer.
        /// </summary>
        private void UpdateCalculatorFromAnswer()
        {
            try
            {
                if (_updatingCalculator || _batchOrders == null)
                    return;

                _updatingCalculator = true;

                //sa * amps sf = totalAmps
                txtAmpsSF.Text = "0";

                if (!String.IsNullOrWhiteSpace(txtTotalSurfaceArea.Text) && !string.IsNullOrEmpty(txtTotalAmps.Text))
                {
                    var totalSA = _batchOrders.Sum(bo => bo.Quantity * (bo.SurfaceArea * 0.0069444));
                    var totalAmps = 0.0;

                    if (double.TryParse(txtTotalAmps.Text, out totalAmps))
                    {
                        var ampsPerSquareFoot = Math.Round(totalAmps / totalSA, 2);
                        txtAmpsSF.Text = ampsPerSquareFoot.ToString("N0");

                        if (CurrentBatch != null)
                        {
                            if (!double.IsInfinity(ampsPerSquareFoot) && !double.IsNaN(ampsPerSquareFoot))
                            {
                                CurrentBatch.AmpsPerSquareFoot = ampsPerSquareFoot;
                            }
                            else
                            {
                                CurrentBatch.SetAmpsPerSquareFootNull();
                            }
                        }
                    }
                }

                _updatingCalculator = false;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error update surface area.");
            }
        }

        private void Dispose()
        {
            try
            {
                if (_dsOrderProcessing != null)
                    _dsOrderProcessing.Dispose();
                _dsOrderProcessing = null;

                if (_batchOrders != null)
                    _batchOrders.Clear();
                _batchOrders = null;

                if (_batchProcesses != null)
                    _batchProcesses.Clear();
                _batchProcesses = null;

                SecurityManager.Current.UserUpdated -= Current_UserUpdated;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error disposing batch editor window.");
            }
        }

        #endregion

        #region Events

        private void grdProcesses_SelectedItemsChanged(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e)
        {
            try
            {
                var selectedProcesses = grdProcesses.SelectedItems
                    .Records
                    .OfType<DataRecord>()
                    .Select(record => record.DataItem as BatchProcessInfo)
                    .Where(proc => proc != null)
                    .ToList();

                var lastProcess = _batchProcesses.LastOrDefault();

                var canRemoveProcess = selectedProcesses.Count == 1
                    && selectedProcesses.First() == lastProcess
                    && _batchProcesses.Count > 1
                    && !selectedProcesses.First().StartDate.HasValue;

                btnProcessRemove.IsEnabled = canRemoveProcess;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after changing grdProcesses selection.");
            }
        }

        private void grdOrders_SelectedItemsChanged(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e)
        {
            bool enableButtons = grdOrders.SelectedItems.Records.OfType <DataRecord>().Any();
            btnDeleteOrder.IsEnabled = enableButtons;
            btnSplitOrder.IsEnabled = enableButtons;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //Automatically start adding new orders if the is a new batch...
                if (this.CurrentBatch == null || this.CurrentBatch.RowState == DataRowState.Added)
                    ShowAddBatchOrderDialog();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding orders on load.");
            }
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            ShowAddBatchOrderDialog();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnOK.IsEnabled = false;

                bool validData = this.CurrentBatch != null &&
                    _batchOrders.Count >= 1 &&
                    _batchProcesses.Count >= 1;

                if (validData && SaveData())
                {
                    PrintTraveler();

                    this.DialogResult = true;
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error on ok batch editor.", exc);
            }
            finally
            {
                btnOK.IsEnabled = true;
            }
        }

        private void btnRemoveProcess_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelectedProcess();
            ValidateProcesses(false);
        }

        private void btnRemoveOrder_Click(object sender, RoutedEventArgs e) { RemoveSelectedOrder(); }

        private void calculator_Opened(object sender, EventArgs e)
        {
            LoadCalculator();
            UpdateCalculator();
        }

        private void txtAmpsSF_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateCalculator();
        }

        private void txtTotalAmps_TextChanged(object sender, TextChangedEventArgs e) { UpdateCalculatorFromAnswer(); }

        private void Current_UserUpdated(object sender, UserUpdatedEventArgs e)
        {
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
                Dispatcher.Invoke(new Action(Close));
            else
            {
                //if user changes then close form w/o saving
                Close();
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e) { Dispose(); }
        
        private void batchInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == "Quantity")
                    ValidateProcesses(false);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error on property changed: Quantity");
            }
        }

        private void grdOrders_InitializeRecord(object sender, Infragistics.Windows.DataPresenter.Events.InitializeRecordEventArgs e)
        {
            var dr = e.Record as DataRecord;

            if (dr != null)
            {
                if (dr.Cells["Quantity"].EditorStyle == null && dr.DataItem is BatchOrderInfo)
                {
                    var batchInfo = dr.DataItem as BatchOrderInfo;
                    var style = new Style(typeof(Infragistics.Windows.Editors.XamNumericEditor));
                    var mask = "{number:1-" + batchInfo.MaxQuantity.ToString() + "}";
                    style.Setters.Add(new Setter(Infragistics.Windows.Editors.XamNumericEditor.MaskProperty, mask));

                    if(ApplicationSettings.Current.AllowPartialProcessLoads)
                        style.Setters.Add(new Setter(Infragistics.Windows.Editors.XamNumericEditor.ValueToDisplayTextConverterProperty, new PartialQuantityDisplayConverter()));
                    else
                        style.Setters.Add(new Setter(Infragistics.Windows.Editors.XamNumericEditor.IsReadOnlyProperty, true));

                    dr.Cells["Quantity"].EditorStyle = style;
                }
            }
        }

        private void btnSplitOrder_Click(object sender, RoutedEventArgs e)
        {
            var orderRecord = grdOrders.SelectedItems.Records.OfType <DataRecord>().FirstOrDefault();

            if(orderRecord != null && orderRecord.DataItem is BatchOrderInfo)
            {
                ShowSplitOrderDialog(orderRecord.DataItem as BatchOrderInfo);
            }
        }

        #endregion

        #region BatchOrderInfo

        public class BatchOrderInfo: INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            internal Data.Datasets.OrderProcessingDataSet.BatchOrderRow BatchOrderRow { get; set; }
            private Data.Datasets.OrderProcessingDataSet.PartRow PartRow { get; set; }

            public int WO 
            {
                get { return BatchOrderRow.OrderID; }
            }

            public string Part
            {
                get
                {
                    return PartRow.Name;
                }
            }

            /// <summary>
            /// Gets the maximum quantity that this batch order can be set to.
            /// </summary>
            /// <value>The maximum quantity.</value>
            public int MaxQuantity { get; set; }

            /// <summary>
            /// Gets the total quantity of the order.
            /// </summary>
            /// <value>The total quantity.</value>
            public int TotalQuantity { get; set; }

            /// <summary>
            /// Gets or sets the quantity of the batch order.
            /// </summary>
            /// <value>The quantity.</value>
            public int Quantity
            {
                get
                {
                    return BatchOrderRow.PartQuantity;
                }
                set
                {
                    // Verify the quantity before setting the value.  If this order is in another batch then we should only be able to put the remaining part qty in.
                    var dtTemp = new OrderProcessingDataSet.BatchOrderDataTable();
                    using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchOrderTableAdapter())
                        ta.FillByOrder(dtTemp, BatchOrderRow.OrderID);

                    var usedPartCount = 0;
                    foreach (var bo in dtTemp.Where(bo => bo.BatchOrderID != BatchOrderRow.BatchOrderID))
                    {
                        using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter())
                        {
                            var dtBatch = new OrderProcessingDataSet.BatchDataTable();
                            ta.FillBy(dtBatch, bo.BatchID);

                            if (dtBatch.Rows.Count > 0)
                            {
                                var batchRow = dtBatch.Rows[0] as Data.Datasets.OrderProcessingDataSet.BatchRow;
                                if (batchRow != null && batchRow.Active)
                                    usedPartCount += bo.PartQuantity;
                            }
                        }
                    }

                    var availableQty = TotalQuantity - usedPartCount;

                    if (value <= availableQty)
                        BatchOrderRow.PartQuantity = value;

                    OnPropertyChanged("Quantity");
                }
            }

            public decimal Weight
            {
                get { return PartRow.IsWeightNull() ? 0 : BatchOrderRow.PartQuantity * PartRow.Weight; }
            }

            public double SurfaceArea
            {
                get { return  BatchOrderRow.PartQuantity * PartRow.SurfaceArea; }
            }

            public BatchOrderInfo(Data.Datasets.OrderProcessingDataSet.BatchOrderRow batchOrder, Data.Datasets.OrderProcessingDataSet.PartRow part)
            {
                BatchOrderRow = batchOrder;
                PartRow = part;

                if(ApplicationSettings.Current.AllowPartialProcessLoads)
                {
                    // Check the original order quantity for max value
                    using(var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter())
                    {
                        var order = ta.GetByOrderID(batchOrder.OrderID).FirstOrDefault();

                        if(order != null)
                            TotalQuantity = order.PartQuantity;
                    }

                    var totalProcessedParts = 0;
                    using(var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter())
                    {
                        var dtOrderProcesses = new OrderProcessingDataSet.OrderProcessesDataTable();
                        ta.FillCurrentProcess(dtOrderProcesses, batchOrder.OrderID);

                        var process = dtOrderProcesses.FirstOrDefault();
                        if(process != null)
                            totalProcessedParts = process.IsPartCountNull() ? 0 : process.PartCount;
                    }

                    MaxQuantity = TotalQuantity - totalProcessedParts;

                    // Update quantity 
                    if (MaxQuantity < batchOrder.PartQuantity)
                        Quantity = MaxQuantity;
                }
                else //else set them all to the same value
                {
                    TotalQuantity = batchOrder.PartQuantity;
                    MaxQuantity = batchOrder.PartQuantity;
                }
            }

            private void OnPropertyChanged(string propertyName)
            {
                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region BatchProcessInfo

        public class BatchProcessInfo
        {
            public Data.Datasets.OrderProcessingDataSet.BatchProcessesRow BatchProcess { get; private set; }

            public int Step
            {
                get { return BatchProcess.StepOrder; }
            }

            public string Process
            {
                get
                {
                    return BatchProcess.ProcessRow == null ? "" : BatchProcess.ProcessRow.Name;
                }
            }

            public string Department
            {
                get { return BatchProcess.ProcessRow == null ? "" : BatchProcess.Department; }
            }

            public DateTime? StartDate
            {
                get { return BatchProcess.IsStartDateNull() ? (DateTime?)null  :  BatchProcess.StartDate; }
            }

            public DateTime? EndDate
            {
                get { return BatchProcess.IsEndDateNull() ? (DateTime?)null : BatchProcess.EndDate; }
            }
            
            public BatchProcessInfo(Data.Datasets.OrderProcessingDataSet.BatchProcessesRow batchProcess)
            {
                BatchProcess = batchProcess;
            }
        }
    }

    #endregion

    #region ProcessingLineItem

    public class ProcessingLineItem
    {
        #region Properties

        public int? ProcessingLineId { get; private set; }

        public string Name { get; private set; }

        public string Department { get; private set; }

        #endregion

        #region Methods

        public static List<ProcessingLineItem> AllLineItems(OrderProcessingDataSet.ProcessingLineDataTable dtProcessingLine)
        {
            var list = new List<ProcessingLineItem> { new ProcessingLineItem() };

            if (dtProcessingLine == null)
            {
                return list;
            }

            list.AddRange(dtProcessingLine.Select(ProcessingLine));

            return list;
        }

        private static ProcessingLineItem ProcessingLine(OrderProcessingDataSet.ProcessingLineRow row)
        {
            if (row == null)
            {
                return null;
            }

            return new ProcessingLineItem
            {
                Name = row.Name,
                ProcessingLineId = row.ProcessingLineID,
                Department = row.IsDepartmentIDNull() ? null : row.DepartmentID
            };
        }


        #endregion
    }

    #endregion

    public class StepOrderImageConverter : IValueConverter
    {
        #region IValueConverter Members

        private static Dictionary<string, BitmapImage> _images = new Dictionary<string, BitmapImage>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var batchProcess = value as BatchEditorWindow.BatchProcessInfo;
            if (batchProcess != null)
            {
                if (batchProcess.StartDate.HasValue && batchProcess.EndDate.HasValue)
                    return GetImage("completed");
                if (batchProcess.StartDate.HasValue && !batchProcess.EndDate.HasValue)
                    return GetImage("inprocess");
                
                return GetImage("incomplete");
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }

        private static BitmapImage GetImage(string name)
        {
            if (!_images.ContainsKey(name))
            {
                BitmapImage img = null;

                switch (name)
                {
                    case "completed":
                        img = WPFUtilities.ToWpfImage("RunComplete24.png");
                        break;
                    case "inprocess":
                        img = WPFUtilities.ToWpfImage("RunCurrent24.png");
                        break;
                    case "incomplete":
                        img = WPFUtilities.ToWpfImage("RunCancel24.png");
                        break;
                }

                if (img != null)
                    _images.Add(name, img);
            }

            return _images.ContainsKey(name) ? _images[name] : null;
        }

        #endregion
    }

    
    public class PartialQuantityDisplayConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BatchEditorWindow.BatchOrderInfo info = null;

            var editor = parameter as Infragistics.Windows.Editors.XamNumericEditor;
            if (editor != null)
            {
                if(editor.DataContext != null)
                    info = ((DataRecord)editor.DataContext).DataItem as BatchEditorWindow.BatchOrderInfo;
            }
            else
            {
                info = value as BatchEditorWindow.BatchOrderInfo;
            }

            if(info != null)
            {
                return info.Quantity + " / " + info.TotalQuantity;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var editor = parameter as Infragistics.Windows.Editors.XamNumericEditor;
            return editor.Value;
            
        }

        #endregion IValueConverter Members
    }

    public class QuantityMaskConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            var batchOrder = value as BatchEditorWindow.BatchOrderInfo;
            if (batchOrder != null)
            {
                var mask = "{number:1-" + batchOrder.MaxQuantity.ToString() + "}";
                return mask;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}
