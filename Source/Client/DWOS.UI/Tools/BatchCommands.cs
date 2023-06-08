using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Reports;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinToolbars;
using System;
using System.Windows.Forms;
using DWOS.Data.Order.Activity;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.Data.Datasets.UserLoggingTableAdapters;
using DWOS.Data.Order;

namespace DWOS.UI.Tools
{
    internal class BatchAddCommand : CommandBase
    {
        #region Fields

        private Main _main = null;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get { return base.Enabled && _main.ActiveTab is BatchSummary; }
        }

        #endregion

        #region Methods

        public BatchAddCommand(ToolBase tool, Main main)
            : base(tool, "BatchOrderProcessing.Edit")
        {
            _main = main;
            _main.SelectedTabChanged += tabManager_TabActivated;
        }

        public override void OnClick()
        {
            try
            {
                var tab = _main.ActiveTab as BatchSummary;

                if (tab == null)
                    return;

                using (new MainRefreshHelper(DWOSApp.MainForm))
                {
                    var frm = new Sales.BatchOrderPanels.BatchEditorWindow();
                    var helper = new WindowInteropHelper(frm) { Owner = DWOSApp.MainForm.Handle };

                    if (frm.LoadData(null))
                    {
                        var dialogResult = frm.ShowDialog().GetValueOrDefault();

                        if (dialogResult && frm.CurrentBatch != null)
                            tab.SelectBatch(frm.CurrentBatch.BatchID);
                    }
                    else
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn("Error loading the batch editor dialog.", "Unable to load Editor");
                    }

                    GC.KeepAlive(helper);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding new batch.");
            }
        }

        public override void Dispose()
        {
            _main = null;
            base.Dispose();
        }

        #endregion

        #region Events

        private void tabManager_TabActivated(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal class BatchDeleteCommand : CommandBase
    {
        #region Fields

        private Main _main = null;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get { return base.Enabled && _main.ActiveTab is BatchSummary && ((BatchSummary)_main.ActiveTab).SelectedBatch > 0; }
        }

        #endregion

        #region Methods

        public BatchDeleteCommand(ToolBase tool, Main main)
            : base(tool, "BatchOrderProcessing.Edit")
        {
            _main = main;
            _main.SelectedTabChanged += tabManager_TabActivated;
            _main.SelectedGridRowChanged += _main_SelectedGridRowChanged;
        }

        public override void OnClick()
        {
            try
            {
                var tab = _main.ActiveTab as BatchSummary;

                if (tab == null)
                    return;

                if (MessageBoxUtilities.ShowMessageBoxYesOrNo("Are you sure you want to delete batch '{0}'?".FormatWith(tab.SelectedBatch), "Delete Batch") != DialogResult.Yes)
                {
                    return;
                }

                using (new MainRefreshHelper(_main))
                {
                    var deleteTransactionDetails = "Batch: {0}".FormatWith(tab.SelectedBatch);
                    var operation = "Delete";
                    var form = "Batch";
                    var userId = SecurityManager.Current.UserID;

                    if (ApplicationSettings.Current.ShowBatchDeletePrompt)
                    {
                        // Show dialog
                        DialogResult userEventLogResult;

                        using (var frm = new UserEventLog { Operation = operation, Form = form, UserID = userId, UserName = SecurityManager.Current.UserName, TransactionDetails = deleteTransactionDetails })
                        {
                            userEventLogResult = frm.ShowDialog(_main);
                        }

                        if (userEventLogResult == DialogResult.OK)
                        {
                            using (var ta = new BatchTableAdapter())
                            {
                                ta.Delete(tab.SelectedBatch);
                            }
                        }
                    }
                    else
                    {
                        // Automatically delete batch with event log
                        using (var taEventLog = new UserEventLogTableAdapter())
                        {
                            taEventLog.Insert(userId, operation, form, "Workflow", deleteTransactionDetails);
                        }

                        using (var ta = new BatchTableAdapter())
                        {
                            ta.Delete(tab.SelectedBatch);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error deleting batch.");
            }
        }

        public override void Dispose()
        {
            _main = null;
            base.Dispose();
        }

        #endregion

        #region Events

        private void tabManager_TabActivated(object sender, EventArgs e)
        {
            Refresh();
        }

        private void _main_SelectedGridRowChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal class BatchEditCommand : CommandBase
    {
        #region Fields

        private Main _main = null;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get { return base.Enabled && _main.ActiveTab is BatchSummary && ((BatchSummary)_main.ActiveTab).SelectedBatch > 0; }
        }

        #endregion

        #region Methods

        public BatchEditCommand(ToolBase tool, Main main)
            : base(tool, "BatchOrderProcessing.Edit")
        {
            _main = main;
            _main.SelectedTabChanged += tabManager_TabActivated;
            _main.SelectedGridRowChanged += _main_SelectedGridRowChanged;
        }

        public override void OnClick()
        {
            try
            {
                var tab = _main.ActiveTab as BatchSummary;

                if (tab == null)
                    return;

                using (new MainRefreshHelper(_main))
                {
                    var selectedBatchId = ((BatchSummary)_main.ActiveTab).SelectedBatch;
                    if(selectedBatchId < 1)
                        return;

                    var frm = new Sales.BatchOrderPanels.BatchEditorWindow();
                    var helper = new WindowInteropHelper(frm) { Owner = DWOSApp.MainForm.Handle };

                    if (frm.LoadData(selectedBatchId))
                    {
                        frm.ShowDialog();
                    }
                    else
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn("Error loading the batch editor dialog.", "Unable to load Editor");
                    }

                    GC.KeepAlive(helper);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error deleting batch.");
            }
        }

        public override void Dispose()
        {
            _main = null;
            base.Dispose();
        }

        #endregion

        #region Events

        private void tabManager_TabActivated(object sender, EventArgs e)
        {
            Refresh();
        }

        private void _main_SelectedGridRowChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal class BatchPrintCommand : CommandBase
    {
        #region Fields

        private Main _main = null;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get { return base.Enabled && _main.ActiveTab is BatchSummary && ((BatchSummary)_main.ActiveTab).SelectedBatch > 0; }
        }

        #endregion

        #region Methods

        public BatchPrintCommand(ToolBase tool, Main main)
            : base(tool)
        {
            _main = main;
            _main.SelectedTabChanged += tabManager_TabActivated;
            _main.SelectedGridRowChanged += _main_SelectedGridRowChanged;
        }

        public override void OnClick()
        {
            try
            {
                var tab = _main.ActiveTab as BatchSummary;

                if (tab == null)
                    return;

                var selectedBatchId = ((BatchSummary)_main.ActiveTab).SelectedBatch;
                if (selectedBatchId < 1)
                    return;

                var rep = new BatchTraveler2Report(selectedBatchId);
                rep.DisplayReport();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error printing batch.");
            }
        }

        public override void Dispose()
        {
            _main = null;
            base.Dispose();
        }

        #endregion

        #region Events

        private void tabManager_TabActivated(object sender, EventArgs e)
        {
            Refresh();
        }

        private void _main_SelectedGridRowChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal class BatchCheckInCommand : CommandBase
    {
        #region Fields

        private Main _main = null;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get { return base.Enabled && _main.ActiveTab is BatchSummary && ((BatchSummary)_main.ActiveTab).SelectedBatch > 0; }
        }

        #endregion

        #region Methods

        public BatchCheckInCommand(ToolBase tool, Main main)
            : base(tool, "PartCheckIn")
        {
            _main = main;
            _main.SelectedTabChanged += tabManager_TabActivated;
            _main.SelectedGridRowChanged += _main_SelectedGridRowChanged;
        }

        public override void OnClick()
        {
            try
            {
                var tab = _main.ActiveTab as BatchSummary;

                if (tab == null)
                    return;

                using (new MainRefreshHelper(_main))
                {
                    var selectedBatchId = ((BatchSummary)_main.ActiveTab).SelectedBatch;

                    var frm = new Processing.BatchCheckInWindow();
                    var helper = new WindowInteropHelper(frm) { Owner = DWOSApp.MainForm.Handle };

                    if (frm.LoadData(selectedBatchId))
                    {
                        frm.ShowDialog();
                    }
                    else
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn("Error loading the batch check-in dialog.", "Unable to load Batch Check In");
                    }

                    GC.KeepAlive(helper);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error checking in batch.");
            }
        }

        public override void Dispose()
        {
            _main = null;
            base.Dispose();
        }

        #endregion

        #region Events

        private void tabManager_TabActivated(object sender, EventArgs e)
        {
            Refresh();
        }

        private void _main_SelectedGridRowChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal class BatchProcessingCommand : CommandBase
    {
        #region Fields

        private Main _main = null;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get
            {
                var batchTab = _main.ActiveTab as IBatchSummary;

                return base.Enabled && batchTab != null && batchTab.SelectedBatch > 0 &&
                       batchTab.SelectedWorkStatus == ApplicationSettings.Current.WorkStatusInProcess &&
                       batchTab.SelectedLocation == Properties.Settings.Default.CurrentDepartment &&
                       (!ApplicationSettings.Current.MultipleLinesEnabled || !batchTab.SelectedLine.HasValue || batchTab.SelectedLine == Properties.Settings.Default.CurrentLine);
            }
        }

        #endregion

        #region Methods

        public BatchProcessingCommand(ToolBase tool, Main main)
            : base(tool, "BatchOrderProcessing")
        {
            _main = main;
            _main.SelectedTabChanged += tabManager_TabActivated;
            _main.SelectedGridRowChanged += _main_SelectedGridRowChanged;
        }

        public override void OnClick()
        {
            try
            {
                var selectedBatchId = (_main.ActiveTab as BatchSummary)?.SelectedBatch ?? -1;
                if (selectedBatchId < 1)
                {
                    return;
                }

                using (new MainRefreshHelper(_main))
                {
                    _log.Info("In batch processing command for batch " + selectedBatchId);

                    var batchInfo = BatchProcessInfo.NewBatchProcessInfo(selectedBatchId);

                    var validationResults = batchInfo.Validate();
                    if (!validationResults.IsValid)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(validationResults.ErrorMessage,
                            validationResults.ErrorHeader,
                            validationResults.ErrorFooter);

                        return;
                    }

                    batchInfo.AnswerQuestions();

                    if (batchInfo.ProcessingResults == null)
                        return;

                    batchInfo.CopyAnswers();

                    //save any changes thus far, because UpdateCost and ProcessingActivity query the DB for answers
                    batchInfo.SaveChanges();

                    if (batchInfo.IsBatchProcessComplete)
                    {
                        batchInfo.CompleteProcessing();
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error processing batch.");
            }
        }

        public override void Dispose()
        {
            _main = null;
            base.Dispose();
        }

        #endregion

        #region Events

        private void tabManager_TabActivated(object sender, EventArgs e)
        {
            Refresh();
        }

        private void _main_SelectedGridRowChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal class BatchInspectionCommand : CommandBase
    {
        #region Fields

        private Main _main = null;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get { return base.Enabled && _main.ActiveTab is BatchSummary && ((BatchSummary)_main.ActiveTab).SelectedWorkStatus == ApplicationSettings.Current.WorkStatusPendingQI; }
        }

        #endregion

        #region Methods

        public BatchInspectionCommand(ToolBase tool, Main main)
            : base(tool, "ControlInspection")
        {
            _main = main;
            _main.SelectedTabChanged += tabManager_TabActivated;
            _main.SelectedGridRowChanged += _main_SelectedGridRowChanged;
        }

        public override void OnClick()
        {
            try
            {
                using (new MainRefreshHelper(_main))
                {
                    DoBatchInspection();
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error inspecting batch.");
            }
        }

        private void DoBatchInspection()
        {
            var tab = _main.ActiveTab as BatchSummary;

            if(tab == null)
                return;

            var selectedBatchId = tab.SelectedBatch;

            if(selectedBatchId < 1)
                return;

            var dsProcessing = new OrderProcessingDataSet();
            var taManager   = new Data.Datasets.OrderProcessingDataSetTableAdapters.TableAdapterManager();

            _log.Debug("Loading batch data for batch " + selectedBatchId);

            //Load the data
            using(new UsingDataSetLoad(dsProcessing))
            {
                using(var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
                    ta.FillByBatch2(dsProcessing.OrderSummary, selectedBatchId);

                taManager.BatchTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchTableAdapter() {ClearBeforeFill = false};
                taManager.BatchOrderTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchOrderTableAdapter() {ClearBeforeFill = false};
                taManager.BatchProcessesTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.BatchProcessesTableAdapter() {ClearBeforeFill = false};
                taManager.OrderProcessesTableAdapter = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter() {ClearBeforeFill = false};

                taManager.BatchTableAdapter.FillBy(dsProcessing.Batch, selectedBatchId);
                taManager.BatchOrderTableAdapter.FillBy(dsProcessing.BatchOrder, selectedBatchId);
                taManager.BatchProcessesTableAdapter.FillBy(dsProcessing.BatchProcesses, selectedBatchId);
                taManager.OrderProcessesTableAdapter.FillByBatch(dsProcessing.OrderProcesses, selectedBatchId);
            }

            var batchInfo = new BatchInspectionInfo(dsProcessing, selectedBatchId);

            if(!batchInfo.Validate())
                return;

            var appSettings = ApplicationSettings.Current;

            ControlInspectionActivity.ControlInspectionActivityResults lastResults = null;

            //for each inspection type required
            while (true)
            {
                //for each batch order that is not already deleted
                foreach(var batchOrder in batchInfo.BatchOrders.Where(bo => bo.IsValidState()))
                {
                    // Skip any order that isn't pending inspection
                    var order = dsProcessing.OrderSummary.FindByOrderID(batchOrder.OrderID);

                    if (order?.WorkStatus != appSettings.WorkStatusPendingQI)
                    {
                        continue;
                    }

                    //inspect this batches order
                    using(var op = new PartInspection())
                    {
                        _log.Debug("Begin inspecting batch for order id" + batchOrder.OrderID);
                        op.Activity = new ControlInspectionActivity(batchOrder.OrderID, SecurityManager.Current.UserID);

                        //if canceled then exit and inspect the order later
                        if(op.ShowDialog(Form.ActiveForm) != DialogResult.OK)
                            return;

                        //if rework, then remove order from batch and tell user
                        if(op.ActivityResults != null && op.ActivityResults.ReworkOriginalOrder)
                        {
                            MessageBoxUtilities.ShowMessageBoxWarn("Order {0} requires rework. The order will be removed from the batch.".FormatWith(batchOrder.OrderID), "Batch Inspection", "Assess any rework on the order as needed.");
                            batchOrder.Delete();
                        }
                        else
                            lastResults = op.ActivityResults;
                    }
                }

                //if no more inspections
                if (lastResults == null || lastResults.RemainingInspections < 1)
                    break; //exit loop

                //reset, in case we cancel out or are out of orders because they all got deleted
                lastResults = null;
            }

            // Batch inspection is complete - cleanup batch and stop timers
            CleanupBatch(batchInfo);
            Data.Order.TimeCollectionUtilities.StopAllBatchTimers(batchInfo.Batch.BatchID);

            var batchOrderCount = batchInfo.Batch.GetBatchOrderRows().Count(b => b.IsValidState());

            if (batchOrderCount < 1)
            {
                // Close batch
                batchInfo.Batch.WorkStatus = appSettings.WorkStatusCompleted;
                batchInfo.Batch.Active = false;
                batchInfo.Batch.CloseDate = DateTime.Now;

                MessageBoxUtilities.ShowMessageBoxWarn("No orders remaining in the batch. Batch will be closed.", "Batch Inspection");
            }
            else
            {
                //update work status of batch to match orders
                var orderWorkStatusDict = batchInfo.Batch.GetBatchOrderRows().Select(batchOrder => batchOrder.OrderID)
                    .ToDictionary(orderId => orderId, orderId => GetWorkStatus(orderId));

                var workStatus = BatchUtilities.WorkStatusForBatch(orderWorkStatusDict.Values);

                var remainingProcesses  = batchInfo.Batch.GetBatchProcessesRows().Count(pr => pr.IsEndDateNull());

                if (BatchUtilities.CanBatchFromInspection(remainingProcesses, workStatus))
                {
                    // Continue batch
                    batchInfo.Batch.WorkStatus = workStatus;

                    // All orders in the batch may not have the same status/location.
                    // Use an order with the batch's work status when setting the batch's location.
                    var matchingOrderId = orderWorkStatusDict
                        .FirstOrDefault(kv => kv.Value == workStatus)
                        .Key;

                    batchInfo.Batch.CurrentLocation = GetCurrentLocation(matchingOrderId);
                }
                else
                {
                    // Close batch
                    batchInfo.Batch.WorkStatus = appSettings.WorkStatusCompleted;
                    batchInfo.Batch.Active = false;
                    batchInfo.Batch.CloseDate = DateTime.Now;

                    DWOSApp.MainForm.FlyoutManager.DisplayFlyout("Batch", "Batch {0} closed".FormatWith(selectedBatchId));
                }
            }

            taManager.UpdateAll(dsProcessing);

            if (!appSettings.OrderCheckInEnabled && batchInfo.Batch.WorkStatus == appSettings.WorkStatusChangingDepartment)
            {
                // Auto check-in
                var batchCheckIn = new BatchCheckInController(selectedBatchId);
                var checkInResult = batchCheckIn.AutoCheckIn(SecurityManager.Current.UserID);

                if (!checkInResult.Response)
                {
                    _log.Warn($"Auto check-in failed for batch {selectedBatchId}.");
                }
            }
        }

        private static void CleanupBatch(BatchInspectionInfo batchInfo)
        {
            // Check for orders that failed inspection on mobile
            using(var taOrder = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter())
            {
                using(var taPartInspection = new Data.Datasets.PartInspectionDataSetTableAdapters.PartInspectionTableAdapter())
                {
                    var batchOrders = batchInfo.BatchOrders
                        .Where(bo => bo.IsValidState())
                        .ToList();

                    foreach (var batchOrder in batchOrders)
                    {
                        var order = taOrder.GetByOrderID(batchOrder.OrderID)[0];

                        if(order.WorkStatus == ApplicationSettings.Current.WorkStatusPendingReworkAssessment)
                        {
                            // Order may need to be reworked
                            batchOrder.Delete();
                            continue;
                        }

                        var dtPartInspection = new PartInspectionDataSet.PartInspectionDataTable();
                        taPartInspection.FillByOrderID(dtPartInspection, batchOrder.OrderID);

                        var partInspection = dtPartInspection.FirstOrDefault();
                        if(partInspection?.RejectedQty > 0 && partInspection.AcceptedQty == 0) // rework needed, remove from batch
                        {
                            batchOrder.Delete();
                        }
                    }
                }
            }
        }

        private string GetWorkStatus(int orderId)
        {
            using (var taOrderSummary = new OrderSummaryTableAdapter())
            {
                return taOrderSummary.GetWorkStatus(orderId);
            }
        }

        private string GetCurrentLocation(int orderId)
        {
            using (var taOrderSummary = new OrderSummaryTableAdapter())
            {
                return taOrderSummary.GetCurrentLocation(orderId);
            }
        }

        public override void Dispose()
        {
            if(_main != null)
            {
                _main.SelectedTabChanged -= tabManager_TabActivated;
                _main.SelectedGridRowChanged -= _main_SelectedGridRowChanged;
            }

            _main = null;
            base.Dispose();
        }

        #endregion

        #region Events

        private void tabManager_TabActivated(object sender, EventArgs e)
        {
            Refresh();
        }

        private void _main_SelectedGridRowChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion

        #region BatchInspectionInfo

        private class BatchInspectionInfo
        {
            private OrderProcessingDataSet OrderProcessing { get; set; }
            public OrderProcessingDataSet.BatchRow Batch { get; set; }
            public OrderProcessingDataSet.BatchProcessesRow CurrentBatchProcess { get; set; }
            public List<OrderProcessingDataSet.BatchOrderRow> BatchOrders { get; set; }

            public BatchInspectionInfo(OrderProcessingDataSet dsOrderProcessing, int batchID)
            {
                NLog.LogManager.GetCurrentClassLogger().Debug("Creating BatchInspectionInfo for batch {0}.", batchID);

                OrderProcessing = dsOrderProcessing;
                Batch           = dsOrderProcessing.Batch.FindByBatchID(batchID);

                if(Batch != null)
                {
                    BatchOrders = Batch.GetBatchOrderRows().OrderBy(bo => bo.BatchOrderID).ToList();
                    CurrentBatchProcess = Batch.GetBatchProcessesRows().OrderBy(ob => ob.StepOrder).FirstOrDefault(r => r.IsEndDateNull());
                }
            }

            internal bool Validate()
            {
                if (Batch == null)
                    return false;

                if (BatchOrders == null || BatchOrders.Count < 1)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("Batch " + Batch.BatchID + " has no orders.", "No Orders");
                    return false;
                }

                return true;
            }
        }

        #endregion
    }

    internal class StartBatchProcessTimerCommand : GridCommand
    {
        #region Properties

        public override bool Enabled
        {
            get
            {
                var activeTab = _frmMain.ActiveTab as BatchSummary;

                if (activeTab == null)
                {
                    return false;
                }

                bool inProcess = activeTab.SelectedWorkStatus == ApplicationSettings.Current.WorkStatusInProcess;
                bool pendingInspection = activeTab.SelectedWorkStatus == ApplicationSettings.Current.WorkStatusPendingQI;
                bool hasCorrectRole = false;

                if(SecurityManager.Current != null)
                {
                    string correctRole = null;

                    if (inProcess)
                    {
                        correctRole = "BatchOrderProcessing";
                    }
                    else if (pendingInspection)
                    {
                        correctRole = "ControlInspection";
                    }

                    if (!string.IsNullOrEmpty(correctRole))
                    {
                        hasCorrectRole = SecurityManager.Current.IsInRole(correctRole);
                    }
                }

                return base.Enabled &&
                    activeTab.SelectedActiveTimerCount == 0 &&
                    hasCorrectRole &&
                    (inProcess || pendingInspection) &&
                    activeTab.SelectedLocation == Properties.Settings.Default.CurrentDepartment &&
                    activeTab.SelectedBatch > 0;
            }
        }

        #endregion

        #region Methods

        public StartBatchProcessTimerCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain)
        {
            frmMain.SelectedTabChanged += FrmMain_SelectedTabChanged;
            frmMain.DataRefreshed += FrmMain_DataRefreshed;
        }

        public override void OnClick()
        {
            var activeTab = _frmMain.ActiveTab as BatchSummary;

            if (!Enabled || activeTab == null)
            {
                return;
            }

            using (new MainRefreshHelper(_frmMain))
            {
                Data.Order.TimeCollectionUtilities
                    .StartBatchProcessTimer(activeTab.SelectedBatch);
            }
        }

        public override bool Refresh()
        {
            bool baseRefresh = base.Refresh();
            bool usingTimers = ApplicationSettings.Current.TimeTrackingEnabled;
            Button.Visible = usingTimers;

            return baseRefresh && usingTimers;
        }

        #endregion

        #region Events

        private void FrmMain_SelectedTabChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void FrmMain_DataRefreshed(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal class StopBatchProcessTimerCommand: GridCommand
    {
        #region Properties

        public override bool Enabled
        {
            get
            {
                var activeTab = _frmMain.ActiveTab as BatchSummary;

                if (activeTab == null)
                {
                    return false;
                }

                bool inProcess = activeTab.SelectedWorkStatus == ApplicationSettings.Current.WorkStatusInProcess;
                bool pendingInspection = activeTab.SelectedWorkStatus == ApplicationSettings.Current.WorkStatusPendingQI;
                bool hasCorrectRole = false;

                if(SecurityManager.Current != null)
                {
                    string correctRole = null;

                    if (inProcess)
                    {
                        correctRole = "BatchOrderProcessing";
                    }
                    else if (pendingInspection)
                    {
                        correctRole = "ControlInspection";
                    }

                    if (!string.IsNullOrEmpty(correctRole))
                    {
                        hasCorrectRole = SecurityManager.Current.IsInRole(correctRole);
                    }
                }

                return base.Enabled &&
                    activeTab.SelectedActiveTimerCount > 0 &&
                    hasCorrectRole &&
                    (inProcess || pendingInspection) &&
                    activeTab.SelectedLocation == Properties.Settings.Default.CurrentDepartment &&
                    activeTab.SelectedBatch > 0;
            }
        }

        #endregion

        #region Methods

        public StopBatchProcessTimerCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain)
        {
            frmMain.SelectedTabChanged += FrmMain_SelectedTabChanged;
            frmMain.DataRefreshed += FrmMain_DataRefreshed;
        }

        public override void OnClick()
        {
            var activeTab = _frmMain.ActiveTab as BatchSummary;
            if (!Enabled || activeTab == null)
            {
                return;
            }

            using (new MainRefreshHelper(_frmMain))
            {
                Data.Order.TimeCollectionUtilities.StopBatchProcessTimer(activeTab.SelectedBatch);
            }
        }

        public override bool Refresh()
        {
            bool baseRefresh = base.Refresh();
            bool usingTimers = ApplicationSettings.Current.TimeTrackingEnabled;
            Button.Visible = usingTimers;

            return baseRefresh && usingTimers;
        }

        #endregion

        #region Events

        private void FrmMain_SelectedTabChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void FrmMain_DataRefreshed(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal class StartBatchLaborTimerCommand: GridCommand
    {
        #region Fields

        private BatchCommandUtilities _utils;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get
            {
                var activeTab = _frmMain.ActiveTab as BatchSummary;

                if (activeTab == null)
                {
                    return false;
                }

                var selectedWorkStatus = activeTab.SelectedWorkStatus;

                return base.Enabled &&
                    activeTab.SelectedBatch > 0 &&
                    _utils.HasCorrectRole(SecurityManager.Current, selectedWorkStatus) &&
                    (!_utils.IsProcessingStatus(selectedWorkStatus) || _utils.IsCurrentDepartment(activeTab.SelectedLocation)) &&
                    !_utils.HasActiveLaborTimer(activeTab.SelectedBatch, selectedWorkStatus, SecurityManager.Current.UserID);
            }
        }

        #endregion

        #region Methods

        public StartBatchLaborTimerCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain)
        {
            _utils = new BatchCommandUtilities();

            frmMain.SelectedTabChanged += FrmMain_SelectedTabChanged;
            frmMain.DataRefreshed += FrmMain_DataRefreshed;
        }

        public override void OnClick()
        {
            var activeTab = _frmMain.ActiveTab as BatchSummary;

            if (activeTab == null)
            {
                return;
            }

            using (new MainRefreshHelper(_frmMain))
            {
                Data.Order.TimeCollectionUtilities
                    .StartBatchLaborTimer(activeTab.SelectedBatch, SecurityManager.Current.UserID);

                if (_utils.IsProcessingStatus(activeTab.SelectedWorkStatus))
                {
                    Data.Order.TimeCollectionUtilities
                        .StartBatchProcessTimer(activeTab.SelectedBatch);
                }
            }
        }

        public override bool Refresh()
        {
            bool baseRefresh = base.Refresh();
            bool usingTimers = ApplicationSettings.Current.TimeTrackingEnabled;
            Button.Visible = usingTimers;

            return baseRefresh && usingTimers;
        }

        public override void Dispose()
        {
            base.Dispose();

            _utils?.Dispose();
            _utils = null;
        }

        #endregion

        #region Events

        private void FrmMain_DataRefreshed(object sender, EventArgs e)
        {
            Refresh();
        }

        private void FrmMain_SelectedTabChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal class StopBatchLaborTimerCommand : GridCommand
    {
        #region Fields

        private BatchCommandUtilities _utils;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get
            {
                var activeTab = _frmMain.ActiveTab as BatchSummary;

                if (activeTab == null)
                {
                    return false;
                }

                var selectedWorkStatus = activeTab.SelectedWorkStatus;

                return base.Enabled &&
                    activeTab.SelectedBatch > 0 &&
                    _utils.HasCorrectRole(SecurityManager.Current, selectedWorkStatus) &&
                    (!_utils.IsProcessingStatus(selectedWorkStatus) || _utils.IsCurrentDepartment(activeTab.SelectedLocation)) &&
                    _utils.IsActiveOperator(activeTab.SelectedBatch, selectedWorkStatus, SecurityManager.Current.UserID);
            }
        }

        #endregion

        #region Methods

        public StopBatchLaborTimerCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain)
        {
            _utils = new BatchCommandUtilities();

            frmMain.SelectedTabChanged += FrmMain_SelectedTabChanged;
            frmMain.DataRefreshed += FrmMain_DataRefreshed;
        }

        public override void OnClick()
        {
            var activeTab = _frmMain.ActiveTab as BatchSummary;

            if (activeTab == null)
            {
                return;
            }

            using (new MainRefreshHelper(_frmMain))
            {
                Data.Order.TimeCollectionUtilities
                    .StopBatchLaborTimer(activeTab.SelectedBatch, SecurityManager.Current.UserID);
            }
        }

        public override bool Refresh()
        {
            bool baseRefresh = base.Refresh();
            bool usingTimers = ApplicationSettings.Current.TimeTrackingEnabled;
            Button.Visible = usingTimers;

            return baseRefresh && usingTimers;
        }

        public override void Dispose()
        {
            base.Dispose();

            _utils?.Dispose();
            _utils = null;
        }

        #endregion

        #region Events

        private void FrmMain_DataRefreshed(object sender, EventArgs e)
        {
            Refresh();
        }

        private void FrmMain_SelectedTabChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal class PauseBatchLaborTimerCommand : GridCommand
    {
        #region Fields

        private BatchCommandUtilities _utils;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get
            {
                var activeTab = _frmMain.ActiveTab as BatchSummary;

                if (activeTab == null)
                {
                    return false;
                }

                var selectedWorkStatus = activeTab.SelectedWorkStatus;

                return base.Enabled &&
                    activeTab.SelectedBatch > 0 &&
                    _utils.HasCorrectRole(SecurityManager.Current, selectedWorkStatus) &&
                    (!_utils.IsProcessingStatus(selectedWorkStatus) || _utils.IsCurrentDepartment(activeTab.SelectedLocation)) &&
                    _utils.HasActiveLaborTimer(activeTab.SelectedBatch, selectedWorkStatus, SecurityManager.Current.UserID);
            }
        }

        #endregion

        #region Methods

        public PauseBatchLaborTimerCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain)
        {
            _utils = new BatchCommandUtilities();

            frmMain.SelectedTabChanged += FrmMain_SelectedTabChanged;
            frmMain.DataRefreshed += FrmMain_DataRefreshed;
        }

        public override void OnClick()
        {
            var activeTab = _frmMain.ActiveTab as BatchSummary;

            if (activeTab == null)
            {
                return;
            }

            using (new MainRefreshHelper(_frmMain))
            {
                Data.Order.TimeCollectionUtilities
                    .PauseBatchLaborTimer(activeTab.SelectedBatch, SecurityManager.Current.UserID);
            }
        }

        public override bool Refresh()
        {
            bool baseRefresh = base.Refresh();
            bool usingTimers = ApplicationSettings.Current.TimeTrackingEnabled;
            Button.Visible = usingTimers;

            return baseRefresh && usingTimers;
        }

        public override void Dispose()
        {
            base.Dispose();

            _utils?.Dispose();
            _utils = null;
        }

        #endregion

        #region Events

        private void FrmMain_DataRefreshed(object sender, EventArgs e)
        {
            Refresh();
        }

        private void FrmMain_SelectedTabChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }


    internal class BatchPartMarkingCommand : CommandBase
    {
        #region Fields

        private Main _main = null;

        #endregion

        #region Properties

        public override bool Enabled => base.Enabled
            && _main.ActiveTab is BatchSummary batchSummary
            && batchSummary.SelectedBatch > 0
            && batchSummary.SelectedWorkStatus == ApplicationSettings.Current.WorkStatusPartMarking;

        #endregion

        #region Methods

        public BatchPartMarkingCommand(ToolBase tool, Main main)
            : base(tool, "PartMarking")
        {
            _main = main;
            _main.SelectedTabChanged += TabManager_TabActivated;
            _main.SelectedGridRowChanged += Main_SelectedGridRowChanged;
        }

        public override void OnClick()
        {
            try
            {
                if (!(_main.ActiveTab is BatchSummary tab) || !Enabled)
                {
                    return;
                }

                using (new MainRefreshHelper(_main))
                {
                    var selectedBatchId = ((BatchSummary)_main.ActiveTab).SelectedBatch;

                    DoBatchPartMarking(selectedBatchId);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error performing part marking for batch.");
            }
        }

        private static void DoBatchPartMarking(int selectedBatchId)
        {
            var appSettings = ApplicationSettings.Current;

            using (var dtOrdersBeforeMarking = new OrderProcessingDataSet.OrderSummaryDataTable())
            {
                using (var taOrderSummary = new OrderSummaryTableAdapter())
                {
                    taOrderSummary.FillByBatch2(dtOrdersBeforeMarking, selectedBatchId);
                }

                foreach (var orderRow in dtOrdersBeforeMarking)
                {
                    if (orderRow.WorkStatus != appSettings.WorkStatusPartMarking)
                    {
                        continue;
                    }

                    using (var dialog = new PartMarking.PartMarkingDialog())
                    {
                        dialog.LoadData(orderRow.OrderID);
                        var dialogResult = dialog.ShowDialog(Form.ActiveForm);

                        if (dialogResult == DialogResult.Cancel)
                        {
                            break;
                        }
                    }
                }
            }

            bool finishedPartMarking;
            string workStatus;
            using (var dtOrdersAfterPartMarking = new OrderProcessingDataSet.OrderSummaryDataTable())
            {
                using (var taOrderSummary = new OrderSummaryTableAdapter())
                {
                    taOrderSummary.FillByBatch2(dtOrdersAfterPartMarking, selectedBatchId);
                }

                finishedPartMarking = dtOrdersAfterPartMarking
                    .All(wo => wo.WorkStatus != appSettings.WorkStatusPartMarking);

                var orderWorkStatuses = dtOrdersAfterPartMarking.Select(wo => wo.WorkStatus);
                workStatus = BatchUtilities.WorkStatusForBatch(orderWorkStatuses);
            }

            if (finishedPartMarking)
            {
                TimeCollectionUtilities.StopAllBatchTimers(selectedBatchId);

                if (BatchUtilities.CanBatchAfterProcessing(workStatus))
                {
                    MoveBatch(selectedBatchId, workStatus);
                }
                else
                {
                    CloseBatch(selectedBatchId);
                }
            }
        }

        private static void MoveBatch(int selectedBatchId, string workStatus)
        {
            var appSettings = ApplicationSettings.Current;

            using (var taBatch = new BatchTableAdapter())
            {
                using (var dtBatch = taBatch.GetDataBy(selectedBatchId))
                {
                    var batch = dtBatch.FirstOrDefault();

                    if (batch != null)
                    {
                        batch.WorkStatus = workStatus;
                    }

                    taBatch.Update(dtBatch);
                }
            }
        }

        private static void CloseBatch(int selectedBatchId)
        {
            var appSettings = ApplicationSettings.Current;

            using (var taBatch = new BatchTableAdapter())
            {
                using (var dtBatch = taBatch.GetDataBy(selectedBatchId))
                {
                    var batch = dtBatch.FirstOrDefault();

                    if (batch != null)
                    {
                        batch.WorkStatus = appSettings.WorkStatusCompleted;
                        batch.Active = false;
                        batch.CloseDate = DateTime.Now;
                    }

                    taBatch.Update(dtBatch);
                }
            }

            DWOSApp.MainForm.FlyoutManager.DisplayFlyout("Batch", "Batch {0} closed".FormatWith(selectedBatchId));
        }

        public override void Dispose()
        {
            _main = null;
            base.Dispose();
        }

        #endregion

        #region Events

        private void TabManager_TabActivated(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Main_SelectedGridRowChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal class BatchCocCommand : CommandBase
    {
        #region Fields

        private Main _main = null;

        #endregion

        #region Properties

        public override bool Enabled => base.Enabled
            && _main.ActiveTab is BatchSummary batchSummary
            && batchSummary.SelectedBatch > 0
            && batchSummary.SelectedWorkStatus == ApplicationSettings.Current.WorkStatusFinalInspection;

        #endregion

        #region Methods

        public BatchCocCommand(ToolBase tool, Main main)
            : base(tool, "COC")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
            _main = main;
            _main.SelectedTabChanged += TabManager_TabActivated;
            _main.SelectedGridRowChanged += Main_SelectedGridRowChanged;
        }

        public override void OnClick()
        {
            try
            {
                if (!(_main.ActiveTab is BatchSummary tab) || !Enabled)
                {
                    return;
                }

                using (new MainRefreshHelper(_main))
                {
                    var selectedBatchId = ((BatchSummary)_main.ActiveTab).SelectedBatch;

                    DoBatchCoc(selectedBatchId);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error performing part marking for batch.");
            }
        }


        private static void DoBatchCoc(int selectedBatchId)
        {
            var appSettings = ApplicationSettings.Current;

            var finishedBatchCerting = true;
            using (var dtOrdersBeforeCert = new OrderProcessingDataSet.OrderSummaryDataTable())
            {
                using (var taOrderSummary = new OrderSummaryTableAdapter())
                {
                    taOrderSummary.FillByBatch2(dtOrdersBeforeCert, selectedBatchId);
                }

                var ordersByCustomer = dtOrdersBeforeCert
                    .GroupBy(wo => wo.IsCustomerIDNull() ? -1 : wo.CustomerID);

                foreach (var customerGroup in ordersByCustomer)
                {
                    var customerId = customerGroup.Key;
                    var orderIds = new List<int>();

                    foreach (var order in customerGroup)
                    {
                        if (order.WorkStatus == appSettings.WorkStatusFinalInspection)
                        {
                            orderIds.Add(order.OrderID);
                        }
                    }

                    if (orderIds.Count > 0)
                    {
                        var createCertDialog = new QA.CreateBatchCoc();
                        var helper = new WindowInteropHelper(createCertDialog)
                        {
                            Owner = DWOSApp.MainForm.Handle
                        };

                        createCertDialog.LoadNew(selectedBatchId, customerId, orderIds);

                        if (!(createCertDialog.ShowDialog() ?? false))
                        {
                            // Cancel batch COC
                            finishedBatchCerting = false;
                            break;
                        }
                    }
                }
            }

            if (finishedBatchCerting)
            {
                CloseBatch(selectedBatchId);
            }
        }

        private static void CloseBatch(int selectedBatchId)
        {
            var appSettings = ApplicationSettings.Current;

            using (var taBatch = new BatchTableAdapter())
            {
                using (var dtBatch = taBatch.GetDataBy(selectedBatchId))
                {
                    var batch = dtBatch.FirstOrDefault();

                    if (batch != null)
                    {
                        batch.WorkStatus = appSettings.WorkStatusCompleted;
                        batch.Active = false;
                        batch.CloseDate = DateTime.Now;
                    }

                    taBatch.Update(dtBatch);
                }
            }

            DWOSApp.MainForm.FlyoutManager.DisplayFlyout("Batch", "Batch {0} closed".FormatWith(selectedBatchId));
        }

        public override void Dispose()
        {
            _main = null;
            base.Dispose();
        }

        #endregion

        #region Events

        private void TabManager_TabActivated(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Main_SelectedGridRowChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }

    internal sealed class BatchCommandUtilities : IDisposable
    {
        private bool _disposed;
        private readonly LaborTimeTableAdapter _taLaborTime = new LaborTimeTableAdapter();
        private readonly BatchProcessesOperatorTableAdapter _taProcessOperator = new BatchProcessesOperatorTableAdapter();
        private readonly BatchOperatorTableAdapter _taOrderOperator = new BatchOperatorTableAdapter();
        private readonly BatchOperatorTimeTableAdapter _taOperatorTime = new BatchOperatorTimeTableAdapter();

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _taLaborTime.Dispose();
            _taProcessOperator.Dispose();
            _taOrderOperator.Dispose();
            _taOperatorTime.Dispose();
            _disposed = true;
        }

        public bool HasCorrectRole(SecurityManager currentSecurityManager, string selectedWorkStatus)
        {
            if (currentSecurityManager == null)
            {
                return false;
            }

            string correctRole = null;

            if (selectedWorkStatus == ApplicationSettings.Current.WorkStatusChangingDepartment)
            {
                correctRole = "PartCheckIn";
            }
            else if (selectedWorkStatus == ApplicationSettings.Current.WorkStatusInProcess)
            {
                correctRole = "BatchOrderProcessing";
            }
            else if (selectedWorkStatus == ApplicationSettings.Current.WorkStatusPendingQI)
            {
                correctRole = "ControlInspection";
            }

            return string.IsNullOrEmpty(correctRole) || currentSecurityManager.IsInRole(correctRole);
        }

        public bool IsProcessingStatus(string selectedWorkStatus) =>
            ApplicationSettings.Current.ProcessingWorkStatuses.Contains(selectedWorkStatus);

        public bool IsCurrentDepartment(string selectedLocation) =>
            selectedLocation == Properties.Settings.Default.CurrentDepartment;

        public bool HasActiveLaborTimer(int batchId, string workStatus, int userId)
        {
            if (IsProcessingStatus(workStatus))
            {
                var countForProcess = _taLaborTime.GetBatchUserActiveTimerCount(batchId, userId) ?? 0;
                return countForProcess > 0;
            }

            var countForBatch = _taOperatorTime.GetUserActiveTimerCount(batchId, userId) ?? 0;
            return countForBatch > 0;
        }

        public bool IsActiveOperator(int batchId, string workStatus, int userId)
        {
            if (IsProcessingStatus(workStatus))
            {
                var countForProcess = _taProcessOperator.GetUserOperatorCount(nameof(OperatorStatus.Active), userId, batchId) ?? 0;
                return countForProcess > 0;
            }

            var countForBatch = _taOrderOperator.GetUserOperatorCount(nameof(OperatorStatus.Active), userId, batchId) ?? 0;
            return countForBatch > 0;
        }
    }
}