using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Order;
using DWOS.Reports;
using DWOS.UI.Properties;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinEditors;
using NLog;
using DWOS.UI.Reports;
using System.Collections.Generic;
using System.Windows.Interop;

namespace DWOS.UI.QA
{
    public partial class ReworkAssessment : Form
    {
        #region Fields

        private CommandManager _cmdManager = new CommandManager();

        private OrdersDataSetLoader _dsManager;

        private bool _inNumValueChanged;

        private int? _processAliasID;

        private ReworkCategoryType _reworkCategory = ReworkCategoryType.Rework;

        #endregion

        #region Properties

        public int OrderID { get; set; }

        public int InternalReworkID { get; set; }

        public OrdersDataSet.OrderRow OrderRow { get; set; }

        public OrdersDataSet.OrderRow NewOrderRow { get; set; }

        public int TotalPartQuantity { get; set; }

        public ReworkType SelectedReworkType
        {
            get { return this.cboReworkType.SelectedItem != null ? (ReworkType) this.cboReworkType.SelectedItem.DataValue : ReworkType.None; }
        }

        public string HoldLocation
        {
            get { return this.cboHoldLocation.Enabled ? this.cboHoldLocation.Text : null; }
        }

        public int ReworkReasonID
        {
            get { return this.cboReason.SelectedItem.DataValue == null ? 1 : Convert.ToInt32(this.cboReason.SelectedItem.DataValue); }
        }

        public int? SelectedProcessAliasID
        {
            get { return this.cboProcessAlias.SelectedItem.DataValue == null ? (int?) null : Convert.ToInt32(this.cboProcessAlias.SelectedItem.DataValue); }
            set { this._processAliasID = value; }
        }

        public string ReworkNotes
        {
            get { return this.txtNotes.Text; }
        }

        public int AcceptedQty
        {
            get { return Convert.ToInt32(this.numPassed.Value); }
        }

        public int FailedQty
        {
            get { return Convert.ToInt32(this.numFailed.Value); }
        }

        public decimal OriginalBasePrice { get; set; }

        public bool PrintOriginalOrder { get; set; }

        public bool PrintNewOrder { get; set; }

        public bool InControlInspection { get; set; }

        public bool CanContinueInspections { get; set; }

        private bool IsActiveRework { get; set; }

        private bool IsInProcess { get; set; }

        private int OrderProcessCount { get; set; }

        #endregion

        #region Methods

        public ReworkAssessment()
        {
            InitializeComponent();
            IsActiveRework = true;
            _dsManager = new OrdersDataSetLoader(
                dsOrders,
                OrdersDataSetLoader.OptionalDependencies.InfoForSplit);
        }

        private void LoadData()
        {
            try
            {
                _dsManager.LoadOrder(OrderID);
                OrderRow = dsOrders.Order.FindByOrderID(OrderID);

                OriginalBasePrice = OrderRow.IsBasePriceNull() ? 0M :  OrderRow.BasePrice;
                this.txtOrderID.Text = OrderID.ToString();
                this.txtPartID.Text = OrderRow.PartSummaryRow != null ? OrderRow.PartSummaryRow.Name : OrderRow.PartID.ToString();
                this.txtUser.Text = SecurityManager.Current.UserName;
                this.dteDate.DateTime = DateTime.Now;

                var orderQuantity = OrderRow.IsPartQuantityNull() ? 0 : OrderRow.PartQuantity;

                if (orderQuantity > 0)
                {
                    this.numPassed.MaxValue = orderQuantity;
                    this.numFailed.MaxValue = orderQuantity;
                }

                //set original values
                this.numPassed.Value = this.numOriginal.Value = TotalPartQuantity = orderQuantity;

                //load rework types
                this.cboReworkType.Items.Add(ReworkType.Full, "Rework Full").Appearance.Image = Properties.Resources.Repair_16;
                this.cboReworkType.Items.Add(ReworkType.Split, "Rework Split").Appearance.Image = Properties.Resources.Order_Split_16;
                this.cboReworkType.Items.Add(ReworkType.SplitHold, "Rework Split/Hold").Appearance.Image = Properties.Resources.Hold_16;
                this.cboReworkType.Items.Add(ReworkType.Quarantine, "Quarantine").Appearance.Image = Properties.Resources.Quarantine_16;
                this.cboReworkType.Items.Add(ReworkType.Lost, "Lost").Appearance.Image = Properties.Resources.Lost_16;

                //load hold locations
                LoadHoldLocations();

                //Load Rework Reasons
                LoadReworkReasons();

                this.cboReworkType.SelectedIndex = 0;

                //Load Process
                var orderProcesses = this.dsOrders.OrderProcesses.Where(op => op.OrderID == OrderID);
                this.cboProcessAlias.Items.Clear();
                var defaultProcess = this.cboProcessAlias.Items.Add(null, "<NONE>");

                foreach(OrdersDataSet.OrderProcessesRow orderProcessesRow in orderProcesses)
                {
                    string name = orderProcessesRow.ProcessAliasSummaryRow.Name;

                    if(!orderProcessesRow.ProcessAliasSummaryRow.IsProcessNameNull())
                    {
                        if(orderProcessesRow.ProcessAliasSummaryRow.ProcessName != orderProcessesRow.ProcessAliasSummaryRow.Name)
                            name = orderProcessesRow.ProcessAliasSummaryRow.ProcessName + " [" + orderProcessesRow.ProcessAliasSummaryRow.Name + "]";
                    }

                    this.cboProcessAlias.Items.Add(orderProcessesRow.ProcessAliasID, name);
                }

                //if any process any 
                this.IsInProcess = this.dsOrders.OrderProcesses.Any(op => op.OrderID == OrderID && op.IsEndDateNull() && op.IsStartDateNull());
                this.OrderProcessCount = this.dsOrders.OrderProcesses.Count;

                if (this._processAliasID.HasValue)
                    defaultProcess = this.cboProcessAlias.FindItemByValue <int?>(p => p.HasValue && p.Value == this._processAliasID.Value);
                else
                {
                    //find the first incomplete process
                    var unfinishedProcess = this.dsOrders.OrderProcesses.Where(op => op.OrderID == OrderID).FirstOrDefault(op => op.IsEndDateNull());
                    //use incomplete process OR last completed process
                    var orderProcess = unfinishedProcess ?? this.dsOrders.OrderProcesses.Where(op => op.OrderID == OrderID).LastOrDefault(op => !op.IsStartDateNull() && !op.IsEndDateNull());

                    if(orderProcess != null)
                        defaultProcess = this.cboProcessAlias.FindItemByValue <int?>(p => p.HasValue && p.Value == orderProcess.ProcessAliasID);
                }

                if(defaultProcess != null)
                    this.cboProcessAlias.SelectedItem = defaultProcess;
                else
                    this.cboProcessAlias.SelectedIndex = 0;

                LoadCommands();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading data for rework assessment.");
            }
        }

        private void LoadCommands()
        {
            var reasonCmd = this._cmdManager.AddCommand("btnAddReason", new InternalReworkReasonEditorCommand((EditorButton) this.cboReason.ButtonsLeft[0], this._reworkCategory)) as InternalReworkReasonEditorCommand;
            reasonCmd.AfterClick += reason_AfterClick;

            var holdCmd = this._cmdManager.AddCommand("btnAddHoldLocation", new HoldReasonEditorCommand((EditorButton) this.cboHoldLocation.ButtonsLeft[0])) as HoldReasonEditorCommand;
            holdCmd.AfterClick += holdCmd_AfterClick;
        }

        public void SaveData()
        {
            switch(SelectedReworkType)
            {
                case ReworkType.Full:
                    SaveFull();
                    break;
                case ReworkType.Split:
                    SaveSplit();
                    break;
                case ReworkType.SplitHold:
                    SaveSplitHold();
                    break;
                case ReworkType.Quarantine:
                    SaveQuarantine();
                    break;
                case ReworkType.Lost:
                    SaveLost();
                    break;
                default:
                    return;
            }

            OrdersDataSet.InternalReworkRow internalRework = this.dsOrders.InternalRework.NewInternalReworkRow();
            internalRework.DateCreated = DateTime.Now;
            internalRework.CreatedBy = SecurityManager.Current.UserID;
            internalRework.OriginalOrderID = OrderID;
            internalRework.ReworkType = SelectedReworkType.ToString();
            internalRework.HoldLocationID = HoldLocation;
            internalRework.ReworkReasonID = ReworkReasonID;
            internalRework.Active = IsActiveRework;
            internalRework.Notes = ReworkNotes;

            int? selectedProcessAliasID = SelectedProcessAliasID;
            if(selectedProcessAliasID.HasValue)
                internalRework.ProcessAliasID = selectedProcessAliasID.Value;

            if(NewOrderRow != null)
                internalRework.ReworkOrderID = NewOrderRow.OrderID;

            //if this is a FULL Quarantine then close out this internal rework since moving staight to shipping
            if(SelectedReworkType == ReworkType.Quarantine && NewOrderRow == null)
                internalRework.Active = false;

            this.dsOrders.InternalRework.AddInternalReworkRow(internalRework);

            if (OrderRow.WorkStatus == ApplicationSettings.Current.WorkStatusPendingReworkAssessment)
            {
                // If the original order is still pending rework assessment, move it to the next step
                // Assumption: Order was moved to this status while in inspection
                var currentOrderProcess = dsOrders.OrderProcesses
                    .Where(op => op.OrderID == OrderRow.OrderID)
                    .OrderBy(op => op.StepOrder)
                    .LastOrDefault(op => !op.IsEndDateNull());

                if (currentOrderProcess == null)
                {
                    // Should not normally occur
                    LogManager.GetCurrentClassLogger().Warn(
                        $"Could not find current process for order {OrderRow.OrderID} that was pending rework assessment.");

                    OrderRow.WorkStatus = ApplicationSettings.Current.WorkStatusChangingDepartment;
                }
                else
                {
                    LogManager.GetCurrentClassLogger().Info($"Moving order {OrderRow.OrderID} that was pending rework assessment.");

                    using (var dtProcessInspections = new OrderProcessingDataSet.ProcessInspectionsDataTable())
                    {
                        using (var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessInspectionsTableAdapter())
                        {
                            ta.FillByProcess(dtProcessInspections, currentOrderProcess.ProcessID);
                        }

                        var completedInspectionCount = currentOrderProcess.GetPartInspectionRows()
                            .Count(inspection => !inspection.IsStatusNull() && inspection.Status);

                        if (completedInspectionCount == dtProcessInspections.Count)
                        {
                            // Completed all inspections
                            OrderRow.WorkStatus = GetWorkStatusAfterInspection(OrderRow);
                        }
                        else
                        {
                            // Need to go back to inspection
                            OrderRow.WorkStatus = ApplicationSettings.Current.WorkStatusPendingQI;
                        }
                    }
                }

                OrderHistoryDataSet.UpdateOrderHistory(OrderID,
                    "Rework Assessment",
                    $"WorkStatus value changed from {ApplicationSettings.Current.WorkStatusPendingReworkAssessment} to {OrderRow.WorkStatus}",
                    SecurityManager.Current.UserName);
            }

            _dsManager.TableAdapterManager.UpdateAll(dsOrders);

            InternalReworkID = internalRework.InternalReworkID;

            try
            {
                if (PrintOriginalOrder && OrderRow != null)
                {
                    if (ApplicationSettings.Current.DefaultPrinterType == DWOS.Data.PrinterType.Document)
                    {
                        var wt = new WorkOrderTravelerReport(OrderRow);
                        if (this.chkQuickPrint.Checked)
                            wt.PrintReport();
                        else
                            wt.DisplayReport();
                    }
                    else
                    {
                        var wl = new ReworkLabelReport(OrderRow, ReworkLabelReport.ReportLabelType.Rework);
                        if (this.chkQuickPrint.Checked)
                            wl.PrintReport();
                        else
                            wl.DisplayReport();
                    }

                    Application.DoEvents(); //refresh before doing next report
                }

                if(PrintNewOrder && NewOrderRow != null)
                {
                    if (ApplicationSettings.Current.DefaultPrinterType == DWOS.Data.PrinterType.Document)
                    {
                        var wt = new WorkOrderTravelerReport(NewOrderRow);
                        if (this.chkQuickPrint.Checked)
                            wt.PrintReport();
                        else
                            wt.DisplayReport();
                    }
                    else
                    {
                        var wl = new ReworkLabelReport(NewOrderRow, ReworkLabelReport.ReportLabelType.Rework);
                        if (this.chkQuickPrint.Checked)
                            wl.PrintReport();
                        else
                            wl.DisplayReport();
                    }

                    Application.DoEvents(); //refresh before doing next report
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error printing WO traveler and/or label.");
            }

            if(NewOrderRow != null)
            {
                OrderHistoryDataSet.UpdateOrderHistory(OrderID, "Rework Assessment", "Order was assessed for rework with rework type {0} for reason {1}.".FormatWith(SelectedReworkType, this.cboReason.Text), SecurityManager.Current.UserName);
                OrderHistoryDataSet.UpdateOrderHistory(OrderID, "Rework Assessment", "Child order was created from rework {0}.".FormatWith(NewOrderRow.OrderID), SecurityManager.Current.UserName);
                OrderHistoryDataSet.UpdateOrderHistory(NewOrderRow.OrderID, "Rework Assessment", "Order was created from rework from parent order {0}.".FormatWith(OrderID), SecurityManager.Current.UserName);
            }
            else
                OrderHistoryDataSet.UpdateOrderHistory(OrderID, "Rework Assessment", "Order was assessed for rework with rework type {0} for reason {1}.".FormatWith(SelectedReworkType, this.cboReason.Text), SecurityManager.Current.UserName);

            Data.Order.TimeCollectionUtilities.CompleteOrderProcessTimers(OrderID);
        }

        private static string GetWorkStatusAfterInspection(OrdersDataSet.OrderRow orderRow)
        {
            if (orderRow.GetOrderProcessesRows().All(op => !op.IsEndDateNull()))
            {
                var orderType = (OrderType)orderRow.OrderType;
                bool isPartMarking;
                using (var taOrder = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
                {
                    isPartMarking = Convert.ToBoolean(taOrder.IsPartMarking(orderRow.OrderID));
                }

                return OrderUtilities.WorkStatusAfterProcessing(orderType, isPartMarking, orderRow.RequireCoc);
            }

            return ApplicationSettings.Current.WorkStatusChangingDepartment;
        }

        private void SaveFull()
        {
            /*********************************************************************
             * Mark order as waiting for rework planning
             * 
             *********************************************************************/

            //mark to do rework planning
            OrderRow.WorkStatus =  ApplicationSettings.Current.WorkStatusPendingReworkPlanning;

            PrintOriginalOrder = true;
            PrintNewOrder = false;
            CanContinueInspections = false;

            //Have to close any open processes so they can be reworked afterwards
            OrderRow.GetOrderProcessesRows().Where(r => !r.IsStartDateNull() && r.IsEndDateNull()).ForEach(r => r.EndDate = DateTime.Now);
        }

        private void SaveLost()
        {
            /*********************************************************************
             * Split to create new lost order
             * Mark order as lost
             * Close order 
             * 
             *********************************************************************/

            //if didnt lose the whole order then split it
            if(AcceptedQty > 0)
                SplitOrder(AcceptedQty, FailedQty);

            PrintNewOrder = false;
            PrintOriginalOrder = AcceptedQty > 0; //if all parts where lost then do not print anything

            OrdersDataSet.OrderRow lostOrder = NewOrderRow ?? OrderRow;

            lostOrder.WorkStatus =  ApplicationSettings.Current.WorkStatusCompleted;
            lostOrder.Status = Settings.Default.OrderStatusClosed;
            lostOrder.OrderType = (int) OrderType.Lost;
            lostOrder.Hold = true;
            lostOrder.BasePrice = 0M;
            lostOrder.Invoice = "NA"; //will prevent from invoicing this part
            lostOrder.CompletedDate = DateTime.Now; //close

            if (NewOrderRow != null)
            {
                // When the order is found, treat it like a normal order
                NewOrderRow.OriginalOrderType = (int)OrderType.Normal;
            }

            //if lost guys got put in new order then original order can continue on
            CanContinueInspections = NewOrderRow != null;

            OrderTableAdapter taOrders = null;
            SalesOrderTableAdapter taSalesOrders = null;
            OrderFeesTableAdapter taOrderFees = null;

            try
            {
                taOrders = new OrderTableAdapter();
                taSalesOrders = new SalesOrderTableAdapter();
                taOrderFees = new OrderFeesTableAdapter();

                if (lostOrder == OrderRow)
                {
                    //Verify and close sales order if there are no other orders assigned to it
                    if (!lostOrder.IsSalesOrderIDNull())
                    {
                        // Check if there are any open orders that are assigned to this sales order
                        // At this point, the DB does not contain changes made to lostOrder.
                        int count = taOrders.GetOpenOrdersCountBySalesOrderID(lostOrder.SalesOrderID).GetValueOrDefault();
                        if (count == 1)
                        {
                            taSalesOrders.FillBySalesOrder(this.dsOrders.SalesOrder, lostOrder.SalesOrderID);
                            if (this.dsOrders.SalesOrder.Count == 1)
                            {
                                this.dsOrders.SalesOrder[0].Status = "Closed";
                                this.dsOrders.SalesOrder[0].CompletedDate = DateTime.Now;
                                taSalesOrders.Update(this.dsOrders.SalesOrder);
                            }
                        }
                    }

                    // Delete all fees for lost orders
                    taOrderFees.DeleteByOrder(lostOrder.OrderID);
                }
            }
            finally
            {
                taSalesOrders?.Dispose();
                taOrderFees?.Dispose();
            }
        }

        private void SaveQuarantine()
        {
            /*********************************************************************
             * Split to create new quarantine order
             * Mark new order as quarantine
             * Put quarantine on Hold
             * 
             *********************************************************************/

            //if didn't quarantine the whole order then split it
            if(AcceptedQty > 0)
            {
                SplitOrder(AcceptedQty, FailedQty);
                PrintNewOrder = true;
            }

            PrintOriginalOrder = true;

            if(NewOrderRow != null)
            {
                //mark to do rework planning
                NewOrderRow.WorkStatus =  ApplicationSettings.Current.WorkStatusHold;
                NewOrderRow.OrderType = (int) OrderType.Quarantine;
                NewOrderRow.OriginalOrderType = (int)OrderType.Quarantine;
                NewOrderRow.Hold = true;

                CanContinueInspections = true;

                //Remove all order processes that have not started since they are not going to be ever used [TTP:423]
                OrdersDataSet.OrderProcessesRow[] op = NewOrderRow.GetOrderProcessesRows();
                op.Where(r => r.IsStartDateNull()).ToList().ForEach(r => r.Delete());
            }
            else
            {
                OrderRow.WorkStatus = OrderUtilities.WorkStatusAfterQuarantine(OrderRow.RequireCoc);
                OrderRow.CurrentLocation = OrderUtilities.LocationAfterQuarantine(OrderRow.RequireCoc);

                OrderRow.OrderType = (int) OrderType.Quarantine;
                OrderRow.Hold = false;

                CanContinueInspections = false;

                //Remove all order processes that have not started since they are not going to be ever used [TTP:423]
                OrdersDataSet.OrderProcessesRow[] op = OrderRow.GetOrderProcessesRows();
                op.Where(r => r.IsStartDateNull()).ToList().ForEach(r => r.Delete());
            }

            IsActiveRework = false; //close out the rework as will never be closed else where
        }

        private void SaveSplit()
        {
            /*********************************************************************
            * Split to create new order
            * Mark new order as normal
            * 
            *********************************************************************/

            //create new split rework order
            SplitOrder(AcceptedQty, FailedQty);

            CanContinueInspections = true;
            PrintNewOrder = true;
            PrintOriginalOrder = true;

            NewOrderRow.WorkStatus =  ApplicationSettings.Current.WorkStatusPendingReworkPlanning;
            NewOrderRow.OrderType = (int) OrderType.Normal;
            NewOrderRow.OriginalOrderType = (int)OrderType.Normal;
        }

        private void SaveSplitHold()
        {
            /*********************************************************************
           * Split to create new order
           * Mark new order as hold
           * 
           *********************************************************************/
            //create new split rework order
            SplitOrder(AcceptedQty, FailedQty);

            //update original order, put it on hold
            OrderRow.WorkStatus =  ApplicationSettings.Current.WorkStatusHold;
            OrderRow.OrderType = (int) OrderType.ReworkHold;
            OrderRow.Hold = true;
            OrderRow.CurrentLocation = ApplicationSettings.Current.DepartmentQA;

            NewOrderRow.WorkStatus =  ApplicationSettings.Current.WorkStatusPendingReworkPlanning;
            NewOrderRow.OrderType = (int) OrderType.ReworkInt;
            NewOrderRow.OriginalOrderType = (int)OrderType.ReworkInt;
            NewOrderRow.Priority = Settings.Default.OrderPriorityExpedite;

            //Remove all order processes that are not started CR602
            NewOrderRow.GetOrderProcessesRows().Where(r => r.IsStartDateNull()).ForEach(r => r.Delete());

            //Close any processes
            NewOrderRow.GetOrderProcessesRows().Where(r => !r.IsStartDateNull() && r.IsEndDateNull()).ForEach(r => r.EndDate = DateTime.Now);

            PrintNewOrder = true;
            PrintOriginalOrder = true;
            CanContinueInspections = true;
        }

        private void SplitOrder(int acceptedQuantity, int failedQuantity)
        {
            OrdersDataSet.OrderRow origOrder = this.dsOrders.Order.FindByOrderID(OrderID);
            var factory = new Sales.OrderFactory();
            factory.Load(dsOrders, _dsManager.TableAdapterManager);

            var splitInfo = new List<SplitOrderInfo>
            {
                new SplitOrderInfo { IsOriginalOrder = true, Order = origOrder.OrderID.ToString(), PartQty = acceptedQuantity },
                new SplitOrderInfo { IsOriginalOrder = false, Order = "New", PartQty = failedQuantity}
            };

            NewOrderRow = factory.SplitRework(origOrder, splitInfo)?.FirstOrDefault();

            if (NewOrderRow == null)
            {
                LogManager.GetCurrentClassLogger().Error("Did not create new order during rework split.");
                return;
            }

            var ordersInSplit = new List<OrdersDataSet.OrderRow>
            {
                origOrder,
                NewOrderRow
            };

            // Split containers
            if (origOrder.GetOrderContainersRows().Length > 0)
            {
                var dialog = new Sales.SplitContainers();
                var helper = new WindowInteropHelper(dialog) { Owner = DWOSApp.MainForm.Handle };

                dialog.Load(origOrder.OrderID, TotalPartQuantity, ordersInSplit, dsOrders.ShipmentPackageType);

                if (dialog.ShowDialog() ?? false)
                {
                    dialog.Sync(dsOrders.OrderContainers, dsOrders.OrderContainerItem);
                }

                GC.KeepAlive(helper);
            }

            // Split serial numbers
            // Assumption: If an order only has one serial number, then it does not need to be removed.
            if (origOrder.GetOrderSerialNumberRows().Length > 1)
            {
                var dialog = new Sales.SplitSerialNumbers();
                var helper = new WindowInteropHelper(dialog) { Owner = DWOSApp.MainForm.Handle };

                dialog.Load(origOrder.OrderID, ordersInSplit);

                if (dialog.ShowDialog() ?? false)
                {
                    dialog.Sync(dsOrders.OrderSerialNumber);
                }
            }

            // Set shipment quantities
            // Rework *should* happen before shipping but it's not guaranteed.
            using (var taOrderShipment = new OrderShipmentTableAdapter())
            {
                foreach (var orgShipment in origOrder.GetOrderShipmentRows())
                {
                    var shipmentPackageId = orgShipment.IsShipmentPackageIDNull()
                        ? -1
                        : orgShipment.ShipmentPackageID;

                    var updateShipmentQty = orgShipment.PartQuantity == TotalPartQuantity
                        && (taOrderShipment.IsShipmentPackageActive(shipmentPackageId) ?? false);

                    if (updateShipmentQty)
                    {
                        orgShipment.PartQuantity = origOrder.IsPartQuantityNull()
                            ? 0
                            : origOrder.PartQuantity;
                    }
                }
            }

            foreach (var shipment in NewOrderRow.GetOrderShipmentRows())
            {
                shipment.PartQuantity = failedQuantity;
            }
        }

        private void UpdatePrice(OrdersDataSet.OrderRow origOrder, OrdersDataSet.OrderRow newOrder)
        {
            if(newOrder != null)
            {
                //if LOT then split the price
                if (OrderPrice.IsPriceUnitLot(newOrder.PriceUnit))
                {
                    decimal origPrice;

                    if (TotalPartQuantity == 0)
                    {
                        origPrice = OriginalBasePrice;
                    }
                    else
                    {
                        origPrice = (AcceptedQty / (decimal) TotalPartQuantity) * OriginalBasePrice;
                    }

                    decimal newPrice = OriginalBasePrice - origPrice;

                    newOrder.BasePrice = newPrice;
                    origOrder.BasePrice = origPrice;
                }
            }
        }

        private void UpdateWeight(OrdersDataSet.OrderRow origOrder, OrdersDataSet.OrderRow newOrder)
        {
            if (origOrder == null || newOrder == null || origOrder.IsWeightNull())
            {
                return;
            }

            decimal perPieceWeight = origOrder.Weight / TotalPartQuantity;
            newOrder.Weight = perPieceWeight * FailedQty;
            origOrder.Weight = perPieceWeight * AcceptedQty;
        }

        public void DisplayReworkSummary()
        {
            try
            {
                using (var reworkSummary = new ReworkSummary { OrderID = OrderID })
                {
                    if (NewOrderRow != null)
                        reworkSummary.NewOrderID = NewOrderRow.OrderID;

                    reworkSummary.InternalReworkID = InternalReworkID;
                    reworkSummary.ShowDialog(this);
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error showing rework summary.");
            }
        }

        private void LoadHoldLocations()
        {
            //load hold locations
            using(var ta = new Data.Datasets.ListsDataSetTableAdapters.d_HoldLocationTableAdapter())
            {
                ListsDataSet.d_HoldLocationDataTable table = ta.GetData();
                this.cboHoldLocation.DisplayMember = table.HoldLocationColumn.ColumnName;
                this.cboHoldLocation.ValueMember = table.HoldLocationColumn.ColumnName;
                this.cboHoldLocation.DataSource = table;

                if(this.cboHoldLocation.Items.Count > 0)
                    this.cboHoldLocation.SelectedIndex = 0;
            }
        }

        private void LoadReworkReasons()
        {
            using (var ta = new Data.Datasets.ListsDataSetTableAdapters.d_ReworkReasonTableAdapter())
            {
                ListsDataSet.d_ReworkReasonDataTable table = ta.GetData();
                this.cboReason.DisplayMember = table.NameColumn.ColumnName;
                this.cboReason.ValueMember = table.ReworkReasonIDColumn.ColumnName;
                this.cboReason.DataSource = table.DefaultView;

                if(this.cboReason.Items.Count > 0)
                    this.cboReason.SelectedIndex = 0;
            }
        }

        private void DisposeMe()
        {
            if(this._cmdManager != null)
                this._cmdManager.Dispose();

            this._cmdManager = null;

            _dsManager?.Dispose();
            _dsManager = null;
        }

        private void ReworkTypeChanged()
        {
            lblAccepted.Text = "Accepted:";
            lblRejected.Text = "Rejected:";
            numPassed.Enabled = true;
            numFailed.Enabled = true;

            if(this.cboReworkType.SelectedItem != null)
            {
                var type = (ReworkType) this.cboReworkType.SelectedItem.DataValue;

                switch(type)
                {
                    case ReworkType.SplitHold:
                        this.cboHoldLocation.Enabled = true;
                        break;
                    case ReworkType.Quarantine:
                        lblAccepted.Text = "Remaining:";
                        lblRejected.Text = "Quarantine:";
                        this.cboHoldLocation.Enabled = true;
                        break;
                    case ReworkType.Lost:
                        lblAccepted.Text = "Remaining:";
                        lblRejected.Text = "Lost:";
                        this.cboHoldLocation.Enabled = false;
                        break;
                    case ReworkType.None:
                        this.cboHoldLocation.Enabled = false;
                        break;
                    case ReworkType.Full:
                        this.cboHoldLocation.Enabled = false;
                        numPassed.Value = 0;
                        numPassed.Enabled = false;
                        numFailed.Value = TotalPartQuantity;
                        numFailed.Enabled = false;
                        break;
                    case ReworkType.Split:
                        this.cboHoldLocation.Enabled = false;
                        break;
                    default:
                        this.cboHoldLocation.Enabled = false;
                        break;
                }

                var reasonView = this.cboReason.DataSource as DataView;

                if(reasonView != null)
                {
                    switch(type)
                    {
                        case ReworkType.Quarantine:
                            this._reworkCategory = ReworkCategoryType.Quarantine;
                            //reasonView.RowFilter = "ReworkCategory = '{0}'".FormatWith(ReworkCategoryType.Quarantine.ToString());
                            break;
                        case ReworkType.Lost:
                            this._reworkCategory = ReworkCategoryType.Lost;
                            //reasonView.RowFilter = "ReworkCategory = '{0}'".FormatWith(ReworkCategoryType.Lost.ToString());
                            break;
                        case ReworkType.SplitHold:
                        case ReworkType.None:
                        case ReworkType.Full:
                        case ReworkType.Split:
                        default:
                            this._reworkCategory = ReworkCategoryType.Rework;
                            //reasonView.RowFilter = "ReworkCategory = '{0}'".FormatWith(ReworkCategoryType.Rework.ToString());
                            break;
                    }

                    reasonView.RowFilter = "ReworkCategory = '{0}'".FormatWith(this._reworkCategory.ToString());

                    if(this.cboReason.Items.Count > 0)
                        this.cboReason.SelectedIndex = 0;
                }
            }
            else
                this.cboHoldLocation.Enabled = false;
        }

        #endregion

        #region Events

        private void RouteAdjustment_Load(object sender, EventArgs e) { LoadData(); }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                //if is split hold and order is currently in process then not legal as the original order in the split will not finish the current process.
                //Per 14380: Show the 'Cannot split/hold an order that is currently in process' warning message when the order is In Process
                //Check the Work Status of the order instead of its processes.
                if (SelectedReworkType == ReworkType.SplitHold && this.OrderRow?.WorkStatus == "In Process" && !this.InControlInspection)
                {
                    if(this.OrderProcessCount > 1)
                        MessageBoxUtilities.ShowMessageBoxWarn("Cannot split/hold an order that is currently in process. This will cause the original order (parts not being re-worked) to not finish the processes.", "Invalid Rework Type", "Suggest using Full or Split rework option or finish the processes.");
                    else
                        MessageBoxUtilities.ShowMessageBoxWarn("Cannot split/hold an order that is currently in process. This will cause the original order (parts not being re-worked) to not finish the process.", "Invalid Rework Type", "Suggest using Full or Split rework option or finish the process.");

                    return;
                }

                //if failed everything and this is a split then we arented
                if (AcceptedQty < 1 && (SelectedReworkType == ReworkType.Split || SelectedReworkType == ReworkType.SplitHold))
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("Cannot split an order that does not have both accepted and rejected parts.", "No Accepted Parts", "Suggest using Full rework option.");
                    return;
                }

                //only save if actually failed something
                if(FailedQty < 1)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("At least 1 part must be rejected to perform rework.", "No Rejected Part");
                    return;
                }

                if(TotalPartQuantity != 0 && (FailedQty + AcceptedQty != TotalPartQuantity))
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("Total quantity must equal {0}.".FormatWith(TotalPartQuantity), "Incorrect Total Count");
                    return;
                }

                if(this.cboReason.SelectedItem == null)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("A valid rework reason must be selected.", "No Rework Reason");
                    return;
                }

                if(this.cboHoldLocation.SelectedItem == null)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("A hold location must be set.", "No Hold Location");
                    return;
                }

                DialogResult = DialogResult.OK;

                //if not in a control inspection then save now
                if(!InControlInspection)
                {
                    SaveData();
                    DisplayReworkSummary();
                    Close();
                }
                else
                    Hide();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on OK of rework assessment.");
            }
        }

        private void numAccepted_ValueChanged(object sender, EventArgs e)
        {
            if(this._inNumValueChanged)
                return;

            this._inNumValueChanged = true;

            int newValue = TotalPartQuantity - Convert.ToInt32(this.numPassed.Value);
            if(newValue >= Convert.ToInt32(this.numPassed.MinValue) && newValue <= Convert.ToInt32(this.numPassed.MaxValue))
                this.numFailed.Value = newValue;

            this._inNumValueChanged = false;
        }

        private void numFailed_ValueChanged(object sender, EventArgs e)
        {
            if(this._inNumValueChanged)
                return;

            this._inNumValueChanged = true;

            int newValue = TotalPartQuantity - Convert.ToInt32(this.numFailed.Value);
            if(newValue >= Convert.ToInt32(this.numPassed.MinValue) && newValue <= Convert.ToInt32(this.numPassed.MaxValue))
                this.numPassed.Value = newValue;

            this._inNumValueChanged = false;
        }

        private void cboReworkType_ValueChanged(object sender, EventArgs e)
        {
            ReworkTypeChanged();

            // Set the ReworkCategoryType on the command after updating
            if(this._cmdManager != null)
            {
                var reasonCmd = this._cmdManager.FindCommand <InternalReworkReasonEditorCommand>() as InternalReworkReasonEditorCommand;
                if(reasonCmd != null)
                    reasonCmd.Category = this._reworkCategory;
            }
        }

        private void holdCmd_AfterClick(object sender, EventArgs e)
        {
            //reload the hold locations from DB
            LoadHoldLocations();
            ReworkTypeChanged();
        }

        private void reason_AfterClick(object sender, EventArgs e)
        {
            // reload the rework reasons from DB
            LoadReworkReasons();
            ReworkTypeChanged();
        }

        private void pnlHoldLocation_MouseHover(object sender, EventArgs e)
        {
            try
            {
                // Show tooltip for disabled control
                if (cboHoldLocation.Enabled)
                {
                    return;
                }

                ultraToolTipManager1.ShowToolTip(cboHoldLocation);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error showing hold location tooltip.");
            }
        }
        private void pnlPassed_MouseHover(object sender, EventArgs e)
        {
            try
            {
                // Show tooltip for disabled control
                if (numPassed.Enabled)
                {
                    return;
                }

                ultraToolTipManager1.ShowToolTip(numPassed);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error showing qty passed tooltip.");
            }
        }

        private void pnlHoldLocation_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                // Hide tooltip for disabled control
                if (cboHoldLocation.Enabled || !ultraToolTipManager1.IsToolTipVisible(cboHoldLocation))
                {
                    return;
                }

                ultraToolTipManager1.HideToolTip();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error hiding hold location tooltip.");
            }
        }

        private void pnlPassed_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                // Hide tooltip for disabled control
                if (numPassed.Enabled || !ultraToolTipManager1.IsToolTipVisible(numPassed))
                {
                    return;
                }

                ultraToolTipManager1.HideToolTip();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error hiding qty passed tooltip.");
            }
        }

        #endregion
    }

}