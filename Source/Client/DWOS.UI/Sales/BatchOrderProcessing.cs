using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Reports.ProcessPartsReportTableAdapters;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Properties;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.UltraGauge.Resources;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using Itenso.TimePeriod;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
using NLog;
using System.Drawing;

namespace DWOS.UI.Sales
{
    public partial class BatchOrderProcessing: DataEditorBase
    {
        #region Fields

        private BarcodeScanner _scanner;

        #endregion

        #region Properties

        public int SelectedOrderId { get; set; }

        #endregion
        
        #region Methods

        public BatchOrderProcessing()
        {
            this.InitializeComponent();

            if(this.DesignMode)
                return;

            _scanner = new BarcodeScanner(Report.BARCODE_ORDER_ACTION_PREFFIX, Report.BARCODE_PACKAGE_PREFIX);
            _scanner.BarcodingFinished  += _scanner_BarcodingFinished;
            _scanner.Start();

            _log.Info("");
            _log.Info("------------------------");
            _log.Info("Batch Order Processing loaded.");
        }

        private void LoadData()
        {
            _log.Debug("Loading Data.");

            this.dsOrderProcessing.EnforceConstraints = false;

            this.taOrderBatch.FillByDepartment(this.dsOrderProcessing.OrderBatch, Properties.Settings.Default.CurrentDepartment, true);
            this.taOrderBatchItem.FillByDepartment(this.dsOrderProcessing.OrderBatchItem, Properties.Settings.Default.CurrentDepartment, true);
            this.taProcess.Fill(this.dsOrderProcessing.Process); //TODO: Do not use generic FILL method
            this.taOrderProcesses.FillByActiveBatches(this.dsOrderProcessing.OrderProcesses, Properties.Settings.Default.CurrentDepartment); 

            this.dsOrderProcessing.EnforceConstraints = true;

            this.pnlBatchOrderInfo.LoadData(this.dsOrderProcessing);
            base.AddDataPanel(this.pnlBatchOrderInfo);

            batchSettings1.Initialze((PopupControlContainerTool)toolbarManager.Tools["Settings"]);
        }

        private void LoadTOC()
        {
            tvwTOC.Nodes.Clear();

            var rootNode = new OrderBatchRootNode();
            tvwTOC.Nodes.Add(rootNode);
            rootNode.Expanded = true;
            rootNode.Select();

            foreach(var row in this.dsOrderProcessing.OrderBatch)
                rootNode.Nodes.Add(new OrderBatchNode(row));
        }

        protected override void ReloadTOC()
        {
            this.LoadTOC();
        }

        protected override bool SaveData()
        {
            try
            {
                _log.Info("Saving data.");

                base.EndAllEdits();

                //update database with batch
                this.taManager.UpdateAll(this.dsOrderProcessing);

                return true;
            }
            catch(DBConcurrencyException sqlExc)
            {
                _log.Error("Concurrency exception updating batch orders.", sqlExc);
                return false;
            }
            catch(Exception exc)
            {
                string dbError = "DataSet Errors: " + this.dsOrderProcessing.GetDataErrors();
                _log.Warn(dbError);
                ErrorMessageBox.ShowDialog("Error saving data.", new ApplicationException(dbError, exc));
                return false;
            }
            finally
            {
                this.UpdateTotalCounts();
            }
        }

        private void LoadCommands()
        {
            if(SecurityManager.Current.IsInRole("BatchOrderProcessing.Edit"))
            {
                //Delete Batch
                base.Commands.AddCommand("Delete", new DeleteCommand(toolbarManager.Tools["Delete"], tvwTOC, this));

                //Complete Batch
                var cmd = base.Commands.AddCommand("Complete", new BatchOrderProcessingCommand(toolbarManager.Tools["Complete"], tvwTOC)) as BatchOrderProcessingCommand;
                cmd.CommandClicked += this.completeBatch_OnClick;

                var add = base.Commands.AddCommand("AddBatch", new AddBatchCommand(toolbarManager.Tools["AddBatch"], tvwTOC)) as AddBatchCommand;
                add.CommandClicked += this.addBatch_CommandClicked;

                var addOrder = base.Commands.AddCommand("AddOrder", new AddBatchCommand(toolbarManager.Tools["AddOrder"], tvwTOC)) as AddBatchCommand;
                addOrder.CommandClicked += this.addOrder_CommandClicked;

                var printCmd = base.Commands.AddCommand("BatchTraveler", new PrintBatchTravelerCommand(toolbarManager.Tools["BatchTraveler"], tvwTOC)) as PrintBatchTravelerCommand;
                printCmd.SaveData = () =>
                                    {
                                        if(this.dsOrderProcessing.HasChanges())
                                        {
                                            if(MessageBoxUtilities.ShowMessageBoxYesOrNo("Data must save before printed. Do you want to save your data?", "Save Data") != DialogResult.Yes)
                                                return false;
                                        }

                                        return this.SaveData();
                                    };
            }
        }

        private void AddOrderBatch()
        {
            var node = new EmptyLoadNode();
            tvwTOC.Nodes[0].Nodes.Add(node);
            node.Select();
        }

        private void AddOrderBatchItem(int orderID)
        {
            try
            {
                Application.DoEvents();
                _log.Info("Attempting to add a batch order item " + orderID);

                this.taOrder.FillById(this.dsOrderProcessing.OrderSummary, orderID);
                var order = this.dsOrderProcessing.OrderSummary.FindByOrderID(orderID);

                _log.Info("Attempting to validate a batch order item " + orderID);
                if (!IsValidateOrderStatus(order))
                {
                    Sound.Beep(true);
                    MessageBoxUtilities.ShowMessageBoxWarn("Order " + orderID + " is not available to be processed, ensure it exists and is in the correct department and work state.", "Invalid Order");
                    return;
                }

                taOrderProcesses.FillCurrentProcess(dsOrderProcessing.OrderProcesses, orderID);
                var opRow = dsOrderProcessing.OrderProcesses.FirstOrDefault(op => op.OrderID == orderID);

                if(opRow == null)
                {
                    Sound.Beep(true);
                    MessageBoxUtilities.ShowMessageBoxWarn("Unable to locate valid process step for order " + orderID + ".", "Invalid Order");
                    return;
                }
                
                if (opRow.Department != Properties.Settings.Default.CurrentDepartment)
                {
                    //ensure you do not try to add a process that does not belong to this department
                    string processName = this.taProcess.GetProcessName(opRow.ProcessID);

                    Sound.Beep(true);
                    MessageBoxUtilities.ShowMessageBoxWarn("The order's process '" + processName + "' department does not match the current department '" + Properties.Settings.Default.CurrentDepartment + "'.", "Invalid Order");
                    return;
                }

                var processedPartCount  = opRow.IsPartCountNull() ? 0 : opRow.PartCount;
                var totalPartCount      = order.PartQuantity;
                var remainingPartCount  = totalPartCount - processedPartCount;

                //see if this order is already in a batch
                _log.Info("Attempting to check for existing batch items " + orderID);
                var existingBatchItems = dsOrderProcessing.OrderBatchItem.Where(obi => obi.RowState != DataRowState.Deleted && obi.OrderProcessID == opRow.OrderProcessesID);
                
                if(existingBatchItems.Any())
                {
                    if (!DWOS.Data.ApplicationSettings.Current.AllowPartialProcessLoads)
                    {
                        Sound.Beep(true);
                        MessageBoxUtilities.ShowMessageBoxWarn("The order {0} already exists in the {1} load.".FormatWith(orderID, existingBatchItems.First().OrderBatchID), "Invalid Order");
                        return;
                    }
                    else
                    {
                        var plannedPartCount = existingBatchItems.Where(b => b.OrderBatchRow.Active).Sum(ei => ei.IsPartQtyNull() ? 0 : ei.PartQty);
                        remainingPartCount = remainingPartCount - plannedPartCount;

                        if (remainingPartCount < 1)
                        {
                            Sound.Beep(true);
                            MessageBoxUtilities.ShowMessageBoxWarn("The order {0} already exists in another load and order part count has been met.".FormatWith(orderID, existingBatchItems.First().OrderBatchID), "Invalid Order");
                            return;
                        }
                    }
                }

                //Get the Batch
                var orderBatch = tvwTOC.SelectedNode<OrderBatchNode>();

                //if batch process ID was set and it doesn't match new orders process
                if (orderBatch != null && orderBatch.DataRow.ProcessID > 0 && opRow.ProcessID != orderBatch.DataRow.ProcessID)
                {
                    string batchName  = this.taProcess.GetProcessName(orderBatch.DataRow.ProcessID);
                    string processName = this.taProcess.GetProcessName(opRow.ProcessID);

                    Sound.Beep(true);
                    MessageBoxUtilities.ShowMessageBoxWarn("The order's process '" + processName + "' does not match the loads process '" + batchName + "'.", "Invalid Order");
                    return;
                }

                //if not on an order batch
                if(orderBatch == null)
                {
                    //create new order batch
                    orderBatch = new OrderBatchNode(this.pnlBatchOrderInfo.AddOrderBatchRow(opRow.ProcessID));
                    tvwTOC.Nodes[0].Nodes.Add(orderBatch);

                    //see if selected item is an empty node
                    var emptyLoad = tvwTOC.SelectedNode<EmptyLoadNode>();
                    tvwTOC.Nodes[0].Select(false); //move to root
                    
                    if(emptyLoad != null)
                        emptyLoad.Remove();
                    
                    orderBatch.Select();
                }

                _log.Info("Adding the order to the batch item " + orderID);
                this.pnlBatchOrderInfo.AddOrderToBatch(opRow.OrderProcessesID, remainingPartCount);
                Sound.Beep(false); //SUCCESS

                base.Commands.RefreshAll();
            }
            catch(Exception exc)
            {
                Sound.Beep(true);
                ErrorMessageBox.ShowDialog("Error adding order to batch.", exc);
            }
        }

        protected override void LoadNode(UltraTreeNode node)
        {
            if(node is OrderBatchNode)
            {
                var curNode = (OrderBatchNode)node;

                DisplayPanel(this.pnlBatchOrderInfo);
                this.pnlBatchOrderInfo.MoveToRecord(curNode.ID);
            }
            else
                DisplayPanel(null);
        }

        protected override void SaveSelectedNode()
        {
            //Required to be implemented
        }

        private void CompleteBatch(OrderBatchNode batch)
        {
            try
            {
                using(new UsingWaitCursor(this))
                {
                    if(batch == null)
                        return;

                    _log.Info("Begin complete batch processing batch " + batch.Text);

                    ValidateAllOrdersBeforeProcessing(batch);
                    this.pnlBatchOrderInfo.EndEditing();

                    var batchOrders = batch.DataRow.GetOrderBatchItemRows();

                    //ensure there are items to batch; NOTE: ValidateAllOrdersBeforeProcessing could change the count
                    if(batchOrders.Length < 1)
                        return;
                    
                    var results = AnswerQuestions(batchOrders[0]); //answer the questions with the first order

                    //copy answers to all other orders
                    if(batchOrders.Length > 1)
                        CopyAnswers(batchOrders[0], batchOrders.Skip(1).ToArray()); 
                    
                    //save all answers
                    this.taOrderProcessAnswer.Update(this.dsOrderProcessing.OrderProcessAnswer);    

                    //if closed out then process all
                    if(results != null && results.IsProcessComplete)
                    {
                        foreach (var bo in batchOrders.Skip(1))
                        {
                            var activity = new ProcessingActivity(bo.OrderProcessesRow.OrderID) { OrderProcessID = bo.OrderProcessID, CurrentProcessedPartQty = bo.PartQty };
                            activity.Complete();
                        }

                        //since completed then close out the batch
                        batch.DataRow.Active    = false;
                        batch.DataRow.CloseDate = DateTime.Now.Date;
                        batch.Visible           = false;

                        tvwTOC.Nodes[0].Select();
                    }

                    taManager.UpdateAll(dsOrderProcessing);

                    this.UpdateTotalCounts();
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error completing batch load.";
                ErrorMessageBox.ShowDialog(errorMsg, new ApplicationException("DataSet Errors: " + this.dsOrderProcessing.GetDataErrors(), exc));
            }
        }
        
        private ProcessingActivity.ProcessingActivityResults AnswerQuestions(OrderProcessingDataSet.OrderBatchItemRow batchItem)
        {
            ProcessingActivity.ProcessingActivityResults results = null;
            
            int orderID = this.taProcess.GetOrderID(batchItem.OrderProcessID).GetValueOrDefault();
            if (orderID < 1)
                return null;

            var orderProcessID = batchItem.OrderProcessID;

            if (orderProcessID < 1)
                return null; 
            
            //load order processing form and answer the questions
            using (var op = new OrderProcessing2(new ProcessingActivity(orderID) { OrderProcessID = orderProcessID, CurrentProcessedPartQty = batchItem.PartQty }) { Mode = OrderProcessing2.ProcessingMode.Batch })
            {
                if (op.ShowDialog(this) == DialogResult.OK && !op.IsProcessingCanceled && op.ActivityResults != null)
                {
                    _log.Info("Order Processing completed " + orderID);
                    results = op.ActivityResults;
                }
                else
                {
                    _log.Info("Order Processing canceled " + orderID);
                    MessageBoxUtilities.ShowMessageBoxWarn("Order processing canceled and will not be applied to the load.", "Order Processing Canceled");
                    return null;
                }
            }
            
            //load all process questions and steps locally
            this.taProcessSteps.FillBy(this.dsOrderProcessing.ProcessSteps, batchItem.OrderProcessesRow.ProcessID);
            this.taProcessQuestion.FillBy(this.dsOrderProcessing.ProcessQuestion, batchItem.OrderProcessesRow.ProcessID);

            _log.Info("Batch answering questions: Order " + orderID + " for Order Process " + orderProcessID);

            dsOrderProcessing.OrderProcessAnswer.Clear(); //remove existing ones since they were updated in the order processing dialog
            this.taOrderProcessAnswer.FillByOrderProcessesID(this.dsOrderProcessing.OrderProcessAnswer, orderProcessID);

            var partQtyAnswers = this.dsOrderProcessing.OrderProcessAnswer.Where(opa => opa.OrderProcessesID == orderProcessID && opa.ProcessQuestionRow.InputType == InputType.PartQty.ToString());
            partQtyAnswers.ForEach(pqa =>
                                   {
                                       pqa.Answer = batchItem.PartQty.ToString();
                                       if(pqa.IsCompletedByNull()) //ensure all answers are answered since PartQty is skipped in Order Processing with batch
                                       {
                                           pqa.Completed = true;
                                           pqa.CompletedBy = SecurityManager.Current.UserID;
                                           pqa.CompletedData = DateTime.Now;
                                       }
                                   });

            return results;
        }

        private void CopyAnswers(OrderProcessingDataSet.OrderBatchItemRow fromBatchItem, OrderProcessingDataSet.OrderBatchItemRow[] toBatchItems)
        {
            var fromAnswers = this.dsOrderProcessing.OrderProcessAnswer.Where(opa => opa.OrderProcessesID == fromBatchItem.OrderProcessID && opa.Completed);

            foreach(var orderBatchItemRow in toBatchItems)
            {
                if(orderBatchItemRow.OrderProcessesRow == null)
                    this.taOrderProcesses.FillByID(this.dsOrderProcessing.OrderProcesses, orderBatchItemRow.OrderProcessID);
                var orderID         = orderBatchItemRow.OrderProcessesRow.OrderID;
                var orderProcessID  = orderBatchItemRow.OrderProcessID;

                //create then load the process answers
                OrderProcessingDataSet.OrderSummaryDataTable.CreateOrderAnswers(orderID);
                this.taOrderProcessAnswer.FillByOrderProcessesID(this.dsOrderProcessing.OrderProcessAnswer, orderProcessID);

                var toAnswers = this.dsOrderProcessing.OrderProcessAnswer.Where(opa => opa.OrderProcessesID == orderProcessID);

                foreach (var fromAnswer in fromAnswers)
                {
                    var toAnswer = toAnswers.FirstOrDefault(opa => opa.ProcessQuestionID == fromAnswer.ProcessQuestionID);
                    
                    if (toAnswer != null && !toAnswer.Completed)
                    {
                        //if question is part qty then set it based on this order
                        if (toAnswer.ProcessQuestionRow.InputType == InputType.PartQty.ToString())
                            toAnswer.Answer = orderBatchItemRow.PartQty.ToString();
                        else
                            toAnswer.Answer = fromAnswer.IsAnswerNull() ? null : fromAnswer.Answer;

                        toAnswer.Completed = fromAnswer.Completed;

                        if (fromAnswer.IsCompletedDataNull())
                            toAnswer.SetCompletedDataNull();
                        else
                            toAnswer.CompletedData = fromAnswer.CompletedData;

                        if (fromAnswer.IsCompletedByNull())
                            toAnswer.SetCompletedByNull();
                        else
                            toAnswer.CompletedBy = fromAnswer.CompletedBy;
                    }
                }
            }
        }

        private void UpdateTotalCounts()
        {
            try
            {
                using(var taBatch = new OrderBatchTableAdapter())
                {
                    object closedOrdersObject = taBatch.GetClosedOrderCount(Settings.Default.CurrentDepartment);
                    int closeOrderCount = 0;

                    if(closedOrdersObject != null && closedOrdersObject != DBNull.Value)
                        int.TryParse(closedOrdersObject.ToString(), out closeOrderCount);

                    var g = this.guagePartCount.Gauges[0] as DigitalGauge;
                    g.Text = closeOrderCount.ToString();
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error updating total closed order count.";
                _log.Error(errorMsg, exc);
            }
        }

        private void DisposeForm()
        {
            try
            {
                if(this._scanner != null)
                {
                    this._scanner.Dispose();
                    this._scanner = null;
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error("Error disposing keyboard listener.", exc);
            }
        }

        private void SelectBatchLoadNode(int batchId)
        {
            try
            {
                var node = tvwTOC.Nodes[0].FindNode<OrderBatchNode>(p => p.DataRow.OrderBatchID == batchId);
                
                if (node != null)
                    node.Select();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error("Error selecting batch load id.", exc);
            }
        }

        private bool IsValidateOrderStatus(OrderProcessingDataSet.OrderSummaryRow order)
        {
            return order != null && order.Status == Properties.Settings.Default.OrderStatusOpen && order.WorkStatus == Properties.Settings.Default.WorkStatusInProcess && order.CurrentLocation == Properties.Settings.Default.CurrentDepartment;
        }

        private void ValidateAllOrdersBeforeProcessing(OrderBatchNode batch)
        {
            try
            {
                var batchItems = batch.DataRow.GetOrderBatchItemRows();
                var toRemove = new List<OrderProcessingDataSet.OrderBatchItemRow>();

                foreach (var orderBatchItemRow in batchItems)
                {
                    var op = orderBatchItemRow.OrderProcessesRow;

                    if (op == null)
                    {
                        taOrderProcesses.FillByID(dsOrderProcessing.OrderProcesses, orderBatchItemRow.OrderProcessID);
                        op = this.dsOrderProcessing.OrderProcesses.FindByOrderProcessesID(orderBatchItemRow.OrderProcessID);
                    }

                    if (op != null)
                    {
                        //if order process got completed outside of batching then remove it
                        if (!op.IsEndDateNull())
                            toRemove.Add(orderBatchItemRow);
                        else
                        {
                            var order = this.dsOrderProcessing.OrderSummary.FindByOrderID(op.OrderID);

                            if (order == null)
                            {
                                this.taOrder.FillById(this.dsOrderProcessing.OrderSummary, op.OrderID);
                                order = this.dsOrderProcessing.OrderSummary.FindByOrderID(op.OrderID);
                            }

                            if (order != null)
                            {
                                //if order no longer valid then remove it
                                if (!IsValidateOrderStatus(order))
                                    toRemove.Add(orderBatchItemRow);
                            }
                        }
                    }
                }

                if (toRemove.Count > 0)
                {
                    var orderIDs = toRemove.ConvertAll<string>(obi => obi.OrderProcessesRow.OrderID.ToString());
                    var orderConcat = string.Join(", ", orderIDs);
                    MessageBoxUtilities.ShowMessageBoxWarn("The following orders are no longer in the correct state '{0}' and will be removed from the batch.".FormatWith(orderConcat), "Invalid Orders", "The orders status has changed since added to the batch.");

                    toRemove.ForEach(obi => pnlBatchOrderInfo.RemoveOrderBatchItem(obi.OrderBatchItemID));

                    Commands.RefreshAll();
                }
            }
            catch (Exception exc)
            {
                _log.Error("Error validating all items in a batch.", exc);
            }

        }

        #endregion

        #region Events

        private void BatchOrderProcessingManager_Load(object sender, EventArgs e)
        {
            _loadingData = true; //prevent TOC from loading

            try
            {
                if(DesignMode)
                    return;

                Cursor = Cursors.WaitCursor;
                this.LoadCommands();

                Application.DoEvents();

                this.LoadData();
                this.LoadTOC();
                LoadValidators();

                this.UpdateTotalCounts();
                tvwTOC.Override.Sort = SortType.Ascending;

                _loadingData = false;

                if(this.SelectedOrderId > 0)
                {
                    var selectedNode = tvwTOC.FindNode(n => n is OrderBatchNode && ((OrderBatchNode) n).ContainsOrder(SelectedOrderId));
                    
                    if(selectedNode != null)
                        selectedNode.Select();
                }

                splitContainer1.Panel2.Enabled = SecurityManager.Current.IsInRole("BatchOrderProcessing.Edit");
            }
            catch(Exception exc)
            {
                _log.Error(this.dsOrderProcessing.GetDataErrors());
                splitContainer1.Enabled = false;
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        protected override void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            _log.Info("Canceling form saves btnCancel_Click: " + Name);
            Close();
        }
        
        private void _scanner_BarcodingFinished(object sender, BarcodeScanner.BarcodeScannerEventArgs e)
        {
            int orderID;

            if (e.Output != null && int.TryParse(e.Output, out orderID))
            {
                if (e.Postfix == Report.BARCODE_ORDER_ACTION_PREFFIX)
                    this.AddOrderBatchItem(orderID);
                else if (e.Postfix == Report.BARCODE_PACKAGE_PREFIX)
                    SelectBatchLoadNode(orderID);
            }
        }

        private void completeBatch_OnClick(object sender, EventArgs e)
        {
            if(tvwTOC.SelectedNodes.Count == 1)
            {
                if(IsValidControls())
                {
                    var batchNode = tvwTOC.SelectedNodes[0] as OrderBatchNode;

                    if(batchNode != null)
                        this.CompleteBatch(batchNode);
                }
            }
        }

        private void addBatch_CommandClicked(object sender, EventArgs e)
        {
            try
            {
                this.AddOrderBatch();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error adding batch.";
                _log.Error(errorMsg, exc);
            }
        }

        private void addOrder_CommandClicked(object sender, EventArgs e)
        {
            try
            {
                var keepGoing = true;
                                
                while(keepGoing)
                {
                    using(var add = new BatchOrderPanels.AddBatchItem())
                    {
                        var selectedNode = tvwTOC.SelectedNode<OrderBatchNode>();

                        if(selectedNode != null && selectedNode.IsRowValid)
                            add.LoadData(selectedNode.DataRow);
                        else
                            add.LoadData(dsOrderProcessing);

                        if(add.ShowDialog(this) == DialogResult.OK)
                        {
                            if (add.SelectedWO.HasValue)
                                this.AddOrderBatchItem(add.SelectedWO.Value);
                            else
                            {
                                Sound.Beep(true);
                                MessageBoxUtilities.ShowMessageBoxWarn("No valid orders selected to be processed, ensure it exists and is in the correct department and work state.", "Invalid Order");
                            }
                        }
                        else
                            keepGoing = false;
                    }
                }
            }
            catch(Exception exc)
            {
                _log.Error("Error adding order batch item.", exc);
            }
        }

        private void BatchOrderProcessingManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK && dsOrderProcessing.HasChanges())
            {
                var r = MessageBoxUtilities.ShowMessageBoxYesOrNo("Some changes have not been saved. Would you like to save your changes before closing?", "Unsaved Changes", "Use the OK button to save changes.");
                
                if (r == DialogResult.Yes)
                    this.SaveData();
            }
        }

        #endregion

        #region Nodes

        #region Nested type: OrderBatchNode

        private class OrderBatchNode : DataNode<OrderProcessingDataSet.OrderBatchRow>
        {
            #region Fields

            private const string KEY_PREFIX = "OB";
            public int PartUsageCount = -1;
            private string _processName;

            private static Image _imageCache = null;

            #endregion

            #region Properties

            private string ProcessName
            {
                get
                {
                    if(this._processName == null)
                    {
                        OrderProcessingDataSet.ProcessRow process = ((OrderProcessingDataSet)base.DataRow.Table.DataSet).Process.FindByProcessID(DataRow.ProcessID);
                        if(process != null)
                            this._processName = process.Name;
                        else
                            this._processName = "New";
                    }

                    return this._processName;
                }
            }

            #endregion

            #region Methods

            public OrderBatchNode(OrderProcessingDataSet.OrderBatchRow cr)
                : base(cr, cr.OrderBatchID.ToString(), KEY_PREFIX, "Order Batch")
            {
                if (_imageCache == null)
                    _imageCache = Properties.Resources.Package_16;

                LeftImages.Add(_imageCache);

                this.UpdateNodeUI();

                DataRow.Table.RowChanged += this.Table_RowChanged;
            }

            private void Table_RowChanged(object sender, DataRowChangeEventArgs e)
            {
                if(e.Action == DataRowAction.Change && e.Row == DataRow)
                {
                    this._processName = null;
                    this.UpdateNodeUI();
                }
            }

            public override void UpdateNodeUI()
            {
                try
                {
                    if (DataRow != null && DataRow.RowState != DataRowState.Deleted)
                    {
                        //find total number of packages for this customer
                        Text = this.ProcessName + " [" + base.DataRow.OrderBatchID + "]";
                        Visible = DataRow.Active;
                    }
                    else
                        Visible = false;
                }
                catch (Exception exc)
                {
                    NLog.LogManager.GetCurrentClassLogger().Error("Error updating node UI.", exc);
                }
            }

            internal int GetBatchItemCount()
            {
                if (DataRow == null || !DataRow.IsValidState())
                    return 0;
                else
                {
                    OrderProcessingDataSet.OrderBatchItemRow[] items = DataRow.GetOrderBatchItemRows();
                    return items != null ? items.Length : 0;
                }
            }

            public bool ContainsOrder(int orderId)
            {
                if (DataRow == null || !DataRow.IsValidState())
                    return false;

               return DataRow.GetOrderBatchItemRows().FirstOrDefault(opr => opr.OrderProcessesRow != null && opr.OrderProcessesRow.OrderID == orderId) != null;
            }

            #endregion
        }

        #endregion

        #region Nested type: OrderBatchRootNode

        private class OrderBatchRootNode : UltraTreeNode
        {
            #region Fields

            #endregion

            #region Methods

            public OrderBatchRootNode()
                : base("ROOT", "Order Loads")
            {
                LeftImages.Add(Properties.Resources.Package_16);
            }

            #endregion
        }

        #endregion

        private class EmptyLoadNode : UltraTreeNode
        {
            public EmptyLoadNode()
            {
                this.Text = "Empty Load";
                this.Override.NodeAppearance.Image = Properties.Resources.Package_Empty_16;
            }
        }

        #endregion

        #region Commands

        #region Nested type: AddBatchCommand

        private class AddBatchCommand: TreeNodeCommandBase
        {
            #region Fields

            public event EventHandler CommandClicked;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return true; }
            }

            #endregion

            #region Methods

            public AddBatchCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                if(this.Enabled)
                    this.CommandClicked(this, EventArgs.Empty);
            }

            #endregion
        }

        #endregion

        #region Nested type: BatchOrderProcessingCommand

        private class BatchOrderProcessingCommand: TreeNodeCommandBase
        {
            #region Fields

            public event EventHandler CommandClicked;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _node is OrderBatchNode && ((OrderBatchNode)_node).GetBatchItemCount() > 0; }
            }

            #endregion

            #region Methods

            public BatchOrderProcessingCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                Button.Enabled = false;

                if(this.CommandClicked != null)
                    this.CommandClicked(this, EventArgs.Empty);

                Button.Enabled = true;
            }

            #endregion
        }

        #endregion

        #region Nested type: PrintBatchTravelerCommand

        private class PrintBatchTravelerCommand: TreeNodeCommandBase
        {
            #region Fields

            public Func<bool> SaveData;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _node is OrderBatchNode && ((OrderBatchNode)_node).GetBatchItemCount() > 0; }
            }

            #endregion

            #region Methods

            public PrintBatchTravelerCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                if(this.SaveData != null && this.SaveData())
                {
                    var rep = new BatchTravelerReport(((OrderBatchNode)_node).DataRow.OrderBatchID);
#if DEBUG
                    rep.DisplayReport();
#else
                    rep.PrintReport();
#endif
                }
            }

            #endregion
        }

        #endregion

        #endregion
    }
}