using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Interop;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Order;
using DWOS.Shared;
using DWOS.UI.Admin;
using DWOS.UI.PartMarking;
using DWOS.UI.Sales;
using DWOS.UI.ShippingRec;
using DWOS.UI.Processing;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinToolbars;
using DWOS.Data.Order.Activity;
using DWOS.Reports;
using DWOS.UI.Reports;

namespace DWOS.UI.Tools
{
    internal class OrderReviewCommand : CommandBase
    {
        #region Fields

        private Main _frmMain;

        #endregion

        #region Properties

        public int BadgeCount { get; set; }

        #endregion
        
        #region Methods

        public OrderReviewCommand(ToolBase tool, Main frmMain)
            : base(tool, "OrderReview")
        {
            _frmMain = frmMain;
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
            
            _frmMain.BadgeCounter.AddBadgeCount(((ToolBaseButtonAdapter)this.Button).Tool.Key, () => BadgeCount);
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (new MainRefreshHelper(_frmMain))
                    {
                        using (var oe = new OrderEntry(OrderEntry.OrderEntryMode.Review))
                        {
                            oe.SelectedWO = _frmMain.SelectedWO;
                            oe.ShowDialog(Form.ActiveForm);
                        }
                    }
                }
                catch (Exception exc)
                {
                    string errorMsg = "Error using Order Entry.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }
        }

        public override bool Refresh()
        {
            const int orderReviewTypeId = 1; // Normal

            var enabled = base.Refresh();

            if(enabled)
            {
                using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderReviewTableAdapter())
                {
                    BadgeCount = ta.GetUserOrderIdsToFixCount(
                        SecurityManager.Current.UserID,
                        ApplicationSettings.Current.WorkStatusPendingOR,
                        orderReviewTypeId).GetValueOrDefault();
                }
            }
            else
                BadgeCount = 0;

            return enabled;
        }

        #endregion
    }

    internal class ImportExportReviewCommand : CommandBase
    {
        #region Fields

        private Main _frmMain;

        #endregion

        #region Properties

        public int BadgeCount { get; set; }

        public override bool Enabled => base.Enabled
            && ApplicationSettings.Current.ImportExportApprovalEnabled;

        #endregion

        #region Methods

        public ImportExportReviewCommand(ToolBase tool, Main frmMain)
            : base(tool, "OrderImportExportReview")
        {
            _frmMain = frmMain;
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
            HideIfDisabled = Properties.Settings.Default.AutoHideDisabledButtons ||
                !ApplicationSettings.Current.ImportExportApprovalEnabled;
            
            _frmMain.BadgeCounter.AddBadgeCount(((ToolBaseButtonAdapter)this.Button).Tool.Key, () => BadgeCount);
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (new MainRefreshHelper(_frmMain))
                    {
                        using (var oe = new OrderEntry(OrderEntry.OrderEntryMode.ImportExportReview))
                        {
                            oe.SelectedWO = _frmMain.SelectedWO;
                            oe.ShowDialog(Form.ActiveForm);
                        }
                    }
                }
                catch (Exception exc)
                {
                    string errorMsg = "Error using Order Entry.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }
        }

        public override bool Refresh()
        {
            const int orderReviewTypeId = 2; // import/export

            var enabled = base.Refresh();

            if(enabled)
            {
                using(var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderReviewTableAdapter())
                    BadgeCount = ta.GetUserOrderIdsToFixCount(
                        SecurityManager.Current.UserID,
                        ApplicationSettings.Current.WorkStatusPendingImportExportReview,
                        orderReviewTypeId).GetValueOrDefault();
            }
            else
                BadgeCount = 0;

            return enabled;
        }

        #endregion
    }

    internal class QuoteCommand : CommandBase
    {
        #region Methods

        public QuoteCommand(ToolBase tool)
            : base(tool, "Quote")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                using(var q = new Quotes())
                {
                    q.ShowDialog(Form.ActiveForm);
                }
            }
        }

        #endregion
    }

    internal class CustomerCommand : CommandBase
    {
        #region Methods

        public CustomerCommand(ToolBase tool)
            : base(tool, "CustomerManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                using (new MainRefreshHelper(DWOSApp.MainForm))
                {
                    using (var c = new Customers())
                    {
                        c.ShowDialog(Form.ActiveForm);
                    }
                }
            }
        }

        #endregion
    }

    internal class OrderEntryCommand : CommandBase
    {
        #region Fields

        private Main _frmMain;

        #endregion

        #region Methods

        public OrderEntryCommand(ToolBase tool, Main frmMain)
            : base(tool, "OrderEntry")
        {
            _frmMain = frmMain;
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                try
                {
                    using (new MainRefreshHelper(_frmMain))
                    {
                        using (var oe = new OrderEntry(OrderEntry.OrderEntryMode.Normal))
                        {
                            oe.SelectedWO = _frmMain.SelectedWO;
                            oe.ShowDialog(Form.ActiveForm);
                        }
                    }
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error using Order Entry.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }
        }

        #endregion
    }


    internal class BlanketPOCommand : CommandBase
    {
        #region Fields

        private Main _frmMain;

        #endregion

        #region Methods

        public BlanketPOCommand(ToolBase tool, Main frmMain)
            : base(tool, "BlanketPOManager")
        {
            _frmMain = frmMain;
            HideIfDisabled = true;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (new MainRefreshHelper(DWOSApp.MainForm))
                    {
                        using (var oe = new OrderEntry(OrderEntry.OrderEntryMode.BlanketPO))
                        {
                            oe.ShowDialog(Form.ActiveForm);
                        }
                    }
                }
                catch (Exception exc)
                {
                    string errorMsg = "Error using Blanket PO Manager.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }
        }

        #endregion
    }

    internal class NewPOOrderCommand : CommandBase
    {
        #region Fields

        private Main _frmMain;

        #endregion

        #region Methods

        public NewPOOrderCommand(ToolBase tool, Main frmMain)
            : base(tool, "AddBlanketPOOrder")
        {
            _frmMain = frmMain;
            HideIfDisabled = true;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                try
                {
                    using (new MainRefreshHelper(DWOSApp.MainForm))
                    {
                        using (var oe = new DWOS.UI.Sales.Order.NewBlanketPOOrderDialog())
                        {
                            oe.ShowDialog(Form.ActiveForm);
                        }
                    }
                }
                catch (Exception exc)
                {
                    string errorMsg = "Error using Find Blanket PO.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }
        }

        #endregion
    }

    internal class AddSalesOrderCommand : CommandBase
    {
        #region Fields

        private readonly Main _frmMain;

        #endregion

        #region Properties

        public override bool Enabled =>
            ApplicationSettings.Current.SalesOrderWizardEnabled
            && base.Enabled;

        #endregion

        #region Methods

        public AddSalesOrderCommand(ToolBase tool, Main frmMain)
            : base(tool, "OrderEntry.Edit")
        {
            HideIfDisabled = true;
            _frmMain = frmMain;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }
            
            using (new MainRefreshHelper(_frmMain))
            {
                var frm = new SalesOrderWizard();
                var helper = new WindowInteropHelper(frm) { Owner = DWOSApp.MainForm.Handle };
                frm.ShowDialog();
            }
        }

        #endregion
    }


    internal class PartsCommand : CommandBase
    {
        #region Fields

        private Main _frmMain;

        #endregion

        #region Methods

        public PartsCommand(ToolBase tool, Main frmMain)
            : base(tool, "PartsManager")
        {
            _frmMain = frmMain;
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                using (new MainRefreshHelper(_frmMain))
                {
                    using (var p = new PartManager())
                    {
                        p.SelectedPartID = _frmMain.SelectedPartID;
                        p.ShowDialog(Form.ActiveForm);
                    }
                }
            }
        }

        #endregion
    }

    internal class ShippingManagerCommand : CommandBase
    {
        #region Methods

        public ShippingManagerCommand(ToolBase tool)
            : base(tool, "ShippingManager")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                using (new MainRefreshHelper(DWOSApp.MainForm))
                {
                    using (var ship = new ShippingManager())
                    {
                        ship.ShowDialog(Form.ActiveForm);
                    }
                }
            }
        }
       
        #endregion
    }

    internal class OrderProcessingCommand: GridCommand
    {
        #region Properties

        public override bool Enabled
            => base.Enabled && _frmMain.SelectedWorkStatus == ApplicationSettings.Current.WorkStatusInProcess &&
               _frmMain.SelectedLocation == Properties.Settings.Default.CurrentDepartment &&
               (!ApplicationSettings.Current.MultipleLinesEnabled || _frmMain.SelectedLine == null ||
                _frmMain.SelectedLine == Properties.Settings.Default.CurrentLine) &&
               !_frmMain.SelectedInBatch;

        #endregion

        #region Methods

        public OrderProcessingCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain, "OrderProcessing")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            using (new MainRefreshHelper(_frmMain))
            {
                var processingActivity = new ProcessingActivity(_frmMain.SelectedWO,
                    new ActivityUser(SecurityManager.Current.UserID, Properties.Settings.Default.CurrentDepartment,
                        Properties.Settings.Default.CurrentLine));

                processingActivity.Initialize();

                if (processingActivity.CanSkip())
                {
                    processingActivity.SkipActivity();

                    var title = "Order";
                    var msg = string.Format("Automatically processed Order {0}",
                        processingActivity.OrderID);

                    DWOSApp.MainForm.FlyoutManager.DisplayFlyout(title, msg);
                }
                else
                {
                    using (var op = new OrderProcessing2(processingActivity))
                    {
                        op.ShowDialog(Form.ActiveForm);
                    }
                }
            }
        }

        #endregion
    }

    internal class StartProcessTimerCommand: GridCommand
    {
        #region Properties

        public override bool Enabled
        {
            get
            {
                bool hasCorrectRole = false;

                if(SecurityManager.Current != null)
                {
                    string correctRole = null;

                    if (_frmMain.SelectedWorkStatus == ApplicationSettings.Current.WorkStatusInProcess)
                    {
                        correctRole = "OrderProcessing";
                    }
                    else if (_frmMain.SelectedWorkStatus == ApplicationSettings.Current.WorkStatusPendingQI)
                    {
                        correctRole = "ControlInspection";
                    }

                    if (!string.IsNullOrEmpty(correctRole))
                    {
                        hasCorrectRole = SecurityManager.Current.IsInRole(correctRole);
                    }
                }

                return base.Enabled &&
                    _frmMain.SelectedActiveTimerCount == 0 &&
                    hasCorrectRole &&
                    _frmMain.SelectedLocation == Properties.Settings.Default.CurrentDepartment &&
                    !_frmMain.SelectedInBatch;
            }
        }

        #endregion

        #region Methods

        public StartProcessTimerCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain)
        {
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            using (new MainRefreshHelper(_frmMain))
            {
                Data.Order.TimeCollectionUtilities.StartOrderProcessTimer(_frmMain.SelectedWO);
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
    }

    internal class StopProcessTimerCommand: GridCommand
    {
        #region Fields

        private Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter _taOrderProcesses;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get
            {
                bool hasCorrectRole = false;

                if(SecurityManager.Current != null)
                {
                    string correctRole = null;

                    if (_frmMain.SelectedWorkStatus == ApplicationSettings.Current.WorkStatusInProcess)
                    {
                        correctRole = "OrderProcessing";
                    }
                    else if (_frmMain.SelectedWorkStatus == ApplicationSettings.Current.WorkStatusPendingQI)
                    {
                        correctRole = "ControlInspection";
                    }

                    if (!string.IsNullOrEmpty(correctRole))
                    {
                        hasCorrectRole = SecurityManager.Current.IsInRole(correctRole);
                    }
                }

                return base.Enabled &&
                    _frmMain.SelectedActiveTimerCount > 0 &&
                    hasCorrectRole &&
                    _frmMain.SelectedLocation == Properties.Settings.Default.CurrentDepartment &&
                    !_frmMain.SelectedInBatch;
            }
        }

        #endregion

        #region Methods

        public StopProcessTimerCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain)
        {
            _taOrderProcesses = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderProcessesTableAdapter();
        }

        public override void OnClick()
        {
            if (!Enabled)
            {
                return;
            }

            using (new MainRefreshHelper(_frmMain))
            {
                Data.Order.TimeCollectionUtilities.StopOrderProcessTimer(_frmMain.SelectedWO);
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

            _taOrderProcesses?.Dispose();
            _taOrderProcesses = null;
        }

        #endregion
    }

    //internal class BatchOrderProcessingCommand : CommandBase
    //{
    //    #region Methods

    //    public BatchOrderProcessingCommand(ToolBase tool)
    //        : base(tool, "BatchOrderProcessing")
    //    {
    //        HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
    //    }

    //    public override void OnClick()
    //    {
    //        if(Enabled)
    //        {
    //            using(var op = new BatchOrderProcessing())
    //            {
    //                op.SelectedOrderId = DWOSApp.MainForm.SelectedWO;
    //                op.ShowDialog(Form.ActiveForm);
    //                DWOSApp.MainForm.RefreshData();
    //            }
    //        }
    //    }

    //    #endregion
    //}

    internal class ReceivingCommand: CommandBase
    {
        #region Methods

        public ReceivingCommand(ToolBase tool)
            : base(tool, "Receiving")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                try
                {
                    using (new UsingWaitCursor(Form.ActiveForm))
                    {
                        using (var frm = new Receiving())
                        {
                            frm.ShowDialog(Form.ActiveForm);
                        }
                    }
                }
                catch (Exception exc)
                {
                    string errorMsg = "Error using Receiving.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }
        }

        #endregion
    }

    internal class PartCheckInCommand : CommandBase
    {
        #region Fields

        private Main _frmMain;

        #endregion

        #region Methods

        public PartCheckInCommand(ToolBase tool, Main frmMain)
            : base(tool, "PartCheckIn")
        {
            _frmMain = frmMain;
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                using (new MainRefreshHelper(_frmMain))
                {
                    using (var frm = new OrderCheckIn())
                    {
                        frm.WorkOrder = _frmMain.SelectedWO;
                        frm.ShowDialog(Form.ActiveForm);
                    }
                }
            }
        }

        #endregion
    }

    internal class RackPartsCommand : GridCommand
    {
        #region Fields

        //private Main _frmMain;

        #endregion

        #region Properties

        public override bool Enabled
            => base.Enabled && _frmMain.SelectedWorkStatus == ApplicationSettings.Current.WorkStatusInProcess &&
               _frmMain.SelectedLocation == Properties.Settings.Default.CurrentDepartment &&
               (!ApplicationSettings.Current.MultipleLinesEnabled || _frmMain.SelectedLine == null ||
                _frmMain.SelectedLine == Properties.Settings.Default.CurrentLine) &&
               !_frmMain.SelectedInBatch;

        #endregion

        #region Methods

        public RackPartsCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain, "RackParts")
        {
            _frmMain = frmMain;
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (Enabled)
            {
                using (new MainRefreshHelper(_frmMain))
                {
                    using (var frm = new PartRacking())
                    {
                        
                        var orderID = _frmMain.SelectedWO;
                        if (orderID < 1) { return; }

                        string Dept = _frmMain.SelectedLocation;
                        var taOrderSummary = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter();
                        var  order = (OrderProcessingDataSet.OrderSummaryRow)taOrderSummary.GetDataById(orderID).Rows[0];

                        
                        frm.WorkOrder = orderID;
                        frm.FormSelectedDeptMain = Dept;
                        frm.ShowDialog(Form.ActiveForm);

                    }
                }
            }
        }

        #endregion
    }

    internal class PartMarkingCommand : CommandBase
    {
        #region Fields

        private Main _frmMain;

        #endregion

        #region Properties

        public override bool Enabled
        {
            get
            {
                return base.Enabled && ApplicationSettings.Current.PartMarkingEnabled;
            }
        }

        #endregion

        #region Methods

        public PartMarkingCommand(ToolBase tool, Main frmMain)
            : base(tool, "PartMarking")
        {
            _frmMain = frmMain;
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if(Enabled)
            {
                using (new MainRefreshHelper(_frmMain))
                {
                    using (var c = new PartMarkingDialog())
                    {
                        c.LoadData(_frmMain.SelectedWO);
                        c.ShowDialog(Form.ActiveForm);
                    }
                }
            }
        }

        #endregion
    }

    internal class OrderNoteCommand : GridCommand
    {
        #region Properties

        public override bool Enabled
        {
            get { return base.Enabled && _frmMain.SelectedWO > 0; }
        }

        #endregion

        #region Methods

        public OrderNoteCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain, "AddOrderNote")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (this.Enabled)
            {
                using (new MainRefreshHelper(_frmMain))
                {
                    using (var frm = new DWOS.UI.Sales.Order.AddOrderNote())
                    {
                        frm.LoadData(_frmMain.SelectedWO);
                        frm.ShowDialog(_frmMain);
                    }
                }
            }
        }

        #endregion
    }

    internal class OrderHoldCommand : GridCommand
    {
        #region Properties

        public override bool Enabled
        {
            get { return base.Enabled && _frmMain.SelectedWO > 0; }
        }

        #endregion

        #region Methods

        public OrderHoldCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain, "OrderHold")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            if (!Enabled || !SecurityManager.Current.IsValidUser)
            {
                return;
            }

            using (new MainRefreshHelper(_frmMain))
            {
                ToggleHold();
            }
        }

        private void ToggleHold()
        {
            var orders = new OrdersDataSet() { EnforceConstraints = false };
            var taOrders = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter();
            var taHolds = new Data.Datasets.OrdersDataSetTableAdapters.OrderHoldTableAdapter();
            var taInternalRework = new Data.Datasets.OrdersDataSetTableAdapters.InternalReworkTableAdapter();
            var taHoldNotification = new OrderHoldNotificationTableAdapter();

            taOrders.FillByOrderID(orders.Order, _frmMain.SelectedWO);
            taHolds.Fill(orders.OrderHold, _frmMain.SelectedWO);

            var order = orders.Order.FirstOrDefault();

            if(order == null)
                return;

            var newHold = !order.Hold;
            string newWorkStatus = null;
            var orderType = (OrderType)order.OrderType;
            var currentUser = SecurityManager.Current;

            if (newHold) //adding a new hold
            {
                newWorkStatus = ApplicationSettings.Current.WorkStatusHold;

                var configSettings = Properties.Settings.Default;

                using (var frm = new HoldEventLog() { UserName = currentUser.UserName, PrintHoldLabel = configSettings.HoldPrintLabel })
                {
                    if (frm.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                    {
                        var hold                = orders.OrderHold.NewOrderHoldRow();
                        hold.OrderID            = order.OrderID;
                        hold.HoldReasonID       = frm.ReasonCode;
                        hold.TimeIn             = DateTime.Now;
                        hold.TimeInUser         = currentUser.UserID;
                        hold.Notes              = frm.Notes;
                        hold.OriginalWorkStatus = order.WorkStatus;
                        orders.OrderHold.AddOrderHoldRow(hold);

                        // Save any config changes
                        configSettings.HoldPrintLabel = frm.PrintHoldLabel;
                        configSettings.Save();

                        if (frm.PrintHoldLabel)
                        {
                            // Print hold label
                            using (var holdLabel = new ReworkLabelReportWrapper(order.OrderID, ReworkLabelReport.ReportLabelType.Hold))
                            {
                                holdLabel.PrintReport();
                            }
                        }

                        // Create hold notifications
                        var holdNotificationContactIds = HoldUtilities
                            .GetContactIdsForNotification(order.IsCustomerIDNull() ? -1 : order.CustomerID);

                        foreach (var contactId in holdNotificationContactIds)
                        {
                            var notificationRow = orders.OrderHoldNotification.NewOrderHoldNotificationRow();
                            notificationRow.OrderHoldRow = hold;
                            notificationRow.ContactID = contactId;
                            orders.OrderHoldNotification.AddOrderHoldNotificationRow(notificationRow);
                        }
                    }
                    else
                        return;
                }
            }
            else //removing hold
            {
                using(var taOrderReviews = new Data.Datasets.OrdersDataSetTableAdapters.OrderReviewTableAdapter())
                    taOrderReviews.FillByOrder(orders.OrderReview, order.OrderID);

                var dtOrderHold = new OrdersDataSet.OrderHoldDataTable();
                using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderHoldTableAdapter())
                    ta.Fill(dtOrderHold, order.OrderID);

                var holdRow = dtOrderHold.Where(h => h.IsTimeOutNull())
                    .OrderByDescending(h => h.OrderHoldID)
                    .FirstOrDefault();

                newWorkStatus = holdRow != null && !holdRow.IsOriginalWorkStatusNull()
                    ? holdRow.OriginalWorkStatus
                    : null;

                if (newWorkStatus == null)
                {
                    var orderReviewed = order.GetOrderReviewRows().Any(or => or.Status);
                    newWorkStatus = orderReviewed ? ApplicationSettings.Current.WorkStatusChangingDepartment : OrderControllerExtensions.GetNewOrderWorkStatus(order.CustomerID, OrderInformation.UserRequiresOrderReview);
                }

                switch (orderType)
                {
                    case OrderType.Normal:
                    case OrderType.ReworkExt:
                    case OrderType.ReworkInt:
                        break;
                    case OrderType.ReworkHold:
                        MessageBoxUtilities.ShowMessageBoxWarn("Order {0} is on a hold for internal rework. The hold must be removed when the internal rework order is joined back.".FormatWith(order.OrderID), "Internal Rework Hold");
                        return;
                    case OrderType.Lost:
                        break;
                    case OrderType.Quarantine:
                        var afterHoldWorkStatus = OrderUtilities.WorkStatusAfterQuarantine(order.RequireCoc);
                        var afterHoldLocation = OrderUtilities.LocationAfterQuarantine(order.RequireCoc);

                        var results = MessageBoxUtilities.ShowMessageBoxYesOrNo(
                            $"Order {order.OrderID} is on a hold for quarantine. Do you want to remove the hold and move this order to {afterHoldWorkStatus}?",
                            "Quarantine Hold");

                        if (results == DialogResult.Yes)
                        {
                            newWorkStatus = afterHoldWorkStatus;
                            order.CurrentLocation = afterHoldLocation;

                            //close out any open quarantines for this order
                            var orderID = order.OrderID;
                            taInternalRework.FillByReworkOrderID(orders.InternalRework, orderID);
                            var quarantines = orders.InternalRework.Where(ir => !ir.IsReworkOrderIDNull() && ir.ReworkOrderID == orderID && ir.Active);

                            quarantines.ForEach(q => q.Active = false);
                        }
                        else
                        {
                            // Keep order on hold if user answers no
                            return;
                        }
                        break;
                }

                // Close out all existing holds
                var openHolds = order.GetOrderHoldRows().Where(oh => oh.IsTimeOutNull()).ToList();
                foreach (var openHold in openHolds)
                {
                    openHold.TimeOut = DateTime.Now;
                    openHold.TimeOutUser = currentUser.UserID;
                }

                if (openHolds.Count > 1)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("Removed order from multiple holds. Please see Order Entry for details.", "Hold");
                }
            }

            order.Hold = newHold;
            order.WorkStatus = newWorkStatus;

            taOrders.Update(orders);
            taHolds.Update(orders);
            taInternalRework.Update(orders);
            taHoldNotification.Update(orders.OrderHoldNotification);

            OrderHistoryDataSet.UpdateOrderHistory(order.OrderID, "Order Hold", newHold ? "Order put on hold." : "Order removed from hold.", currentUser.UserName);

            TimeCollectionUtilities.StopAllOrderTimers(order.OrderID);

            _frmMain.UpdateHoldSelection();
        }

        protected override void frmMain_SelectedGridRowChanged(object sender, EventArgs e)
        {
            var isEnabled = base.Refresh();

            if (isEnabled)
            {
                this.Button.IsUpdating = true;
                var stateButton = ((ToolBaseButtonAdapter)base.Button).Tool as StateButtonTool;
                stateButton.Checked = _frmMain.SelectedWO > 0 && _frmMain.SelectedHoldStatus;
                this.Button.IsUpdating = false;
            }
        }

        #endregion
    }

    internal class SplitOrderCommand : GridCommand
    {
        public override bool Enabled => IsAuthorized() &&
                                        _frmMain.SelectedWO > 0;

        public SplitOrderCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain, "OrderSplit")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            OrdersDataSet dsOrders = null;
            OrdersDataSetLoader dsManager = null;

            try
            {
                if (!Enabled)
                {
                    return;
                }

                var orderId = _frmMain.SelectedWO;

                _log.Info($"Starting order split for {orderId}.");

                dsOrders = new OrdersDataSet { EnforceConstraints = false }; // Can try to save duplicate data
                dsManager = new OrdersDataSetLoader(dsOrders, OrdersDataSetLoader.OptionalDependencies.InfoForSplit);
                dsManager.LoadOrder(orderId);

                var orgOrder = dsOrders.Order.FindByOrderID(orderId);

                if (orgOrder == null)
                {
                    _log.Error("Could not load order during order split.");
                    return;
                }

                var orgQty = orgOrder.IsPartQuantityNull()
                    ? 0
                    : orgOrder.PartQuantity;

                if (orgQty > 1)
                {
                    using (new MainRefreshHelper(_frmMain))
                    {
                        var newOrders = SplitUtilities.DoSplit(orgOrder, dsOrders, dsManager.TableAdapterManager);

                        if (newOrders.Count > 0)
                        {
                            dsManager.TableAdapterManager.UpdateAll(dsOrders);

                            // Print WO Travelers
                            if (UserSettings.Default.SplitPrintTraveler)
                            {
                                var originalOrderReport = new WorkOrderTravelerReport(orgOrder);
                                originalOrderReport.PrintReport();
                                foreach (var newOrder in newOrders)
                                {
                                    var splitOrderReport = new WorkOrderTravelerReport(newOrder);
                                    splitOrderReport.PrintReport();
                                }
                            }

                            ShowSplitConfirmation(orderId, newOrders);
                        }
                    }
                }
                else
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(
                        "Unable to split an order with a quantity less than 2. Please increase the quantity.",
                        "Invalid Operation");
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error splitting order.");
            }
            finally
            {
                dsOrders?.Dispose();
                dsManager?.Dispose();
            }
        }

        private static void ShowSplitConfirmation(int originalOrderId, ICollection<OrdersDataSet.OrderRow> newOrders)
        {
            if (newOrders == null)
            {
                return;
            }

            var flyoutMessage = newOrders.Count == 1
                ? $"Split Order {newOrders.First().OrderID} from Order {originalOrderId}"
                : $"Split multiple orders from Order {originalOrderId}";

            DWOSApp.MainForm.FlyoutManager.DisplayFlyout("Order Split", flyoutMessage);
        }
    }

    internal class RejoinOrderCommand : GridCommand
    {
        #region Properties

        public override bool Enabled => IsAuthorized() &&
                                        _frmMain.SelectedWO > 0;

        #endregion

        #region Methods

        public RejoinOrderCommand(ToolBase tool, Main frmMain)
            : base(tool, frmMain, "OrderRejoin")
        {
            HideIfUnAuthorized = Properties.Settings.Default.AutoHideDisabledButtons;
        }

        public override void OnClick()
        {
            try
            {
                if (!Enabled)
                {
                    return;
                }

                using (new MainRefreshHelper(_frmMain))
                {
                    var data = LoadInitialData();

                    if (data == null)
                    {
                        // An error occurred while loading data.
                        MessageBoxUtilities.ShowMessageBoxError(
                            "There was an error with rejoining the selected order. Please try again.",
                            "Order Rejoin");

                        return;
                    }

                    var dsOrders = data.Item1;
                    var taManager = data.Item2;

                    var orgOrder = dsOrders.Order.FindByOrderID(_frmMain.SelectedWO);

                    if (orgOrder == null)
                    {
                        return;
                    }

                    LoadData(orgOrder, dsOrders);

                    var orgQty = orgOrder.IsPartQuantityNull()
                        ? 0
                        : orgOrder.PartQuantity;

                    var rejoinableOrders = dsOrders.Order
                        .Where(order => RejoinUtilities.CanRejoin(order, orgOrder))
                        .ToList();

                    if (orgQty > 0 && rejoinableOrders.Count > 0)
                    {
                        var destinationOrder = DoRejoin(orgOrder, dsOrders, taManager);

                        if (destinationOrder != null)
                        {
                            taManager.UpdateAll(dsOrders);
                            TimeCollectionUtilities.StopAllOrderTimers(orgOrder.OrderID);

                            if (UserSettings.Default.RejoinPrintTraveler)
                            {
                                var report = new WorkOrderTravelerReport(destinationOrder);
                                report.PrintReport();
                            }

                            ShowRejoinConfirmation(orgOrder, destinationOrder);
                        }
                    }
                    else if (rejoinableOrders.Count == 0)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn("There are no Work Orders that can be rejoined to this order.", "Rejoin");
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error rejoining order.");
                throw;
            }
        }

        private static void ShowRejoinConfirmation(OrdersDataSet.OrderRow orgOrder, OrdersDataSet.OrderRow destinationOrder)
        {
            if (orgOrder == null || destinationOrder == null)
            {
                return;
            }

            DWOSApp.MainForm.FlyoutManager.DisplayFlyout("Order Rejoin",
                $"Joined Order {orgOrder.OrderID} to Order {destinationOrder.OrderID}.");
        }

        private OrdersDataSet.OrderRow DoRejoin(OrdersDataSet.OrderRow orgOrder, OrdersDataSet dsOrders, TableAdapterManager taManager)
        {
            var rejoinableOrders = dsOrders.Order
                .Where(order => RejoinUtilities.CanRejoin(order, orgOrder))
                .ToList();

            var dialog = new RejoinOrder();
            var helper = new WindowInteropHelper(dialog) { Owner = DWOSApp.MainForm.Handle };
            dialog.Load(orgOrder, rejoinableOrders);
            dialog.PrintTraveler = UserSettings.Default.RejoinPrintTraveler;

            if (!dialog.ShowDialog().GetValueOrDefault())
            {
                return null;
            }

            UserSettings.Default.RejoinPrintTraveler = dialog.PrintTraveler;
            UserSettings.Default.Save();

            GC.KeepAlive(helper);

            // Assumption - results from dialog are valid
            var destinationOrder = dsOrders.Order.FindByOrderID(dialog.DestinationOrderId);
            var orderChangeReason = dialog.OrderChangeReasonId;

            LoadData(destinationOrder, dsOrders);

            var originalDestinationQuantity = destinationOrder.IsPartQuantityNull() ? 0 : destinationOrder.PartQuantity;
            var factory = new OrderFactory();
            factory.Load(dsOrders, taManager);
            factory.Rejoin(orgOrder, destinationOrder, orderChangeReason);
            var rejoinDate = orgOrder.IsCompletedDateNull () ? DateTime.Now : orgOrder.CompletedDate;

            // Rejoin containers
            foreach (var orgContainer in orgOrder.GetOrderContainersRows().Where(c => c.IsValidState() && !c.IsIsActiveNull() && c.IsActive))
            {
                var destContainer = dsOrders.OrderContainers.NewOrderContainersRow();

                destContainer.OrderRow = destinationOrder;
                destContainer.PartQuantity = orgContainer.PartQuantity;
                destContainer.IsActive = true;

                if (!orgContainer.IsWeightNull())
                {
                    destContainer.Weight = orgContainer.Weight;
                }

                destContainer.ShipmentPackageTypeID = orgContainer.ShipmentPackageTypeID;

                dsOrders.OrderContainers.AddOrderContainersRow(destContainer);


                foreach (var item in orgContainer.GetOrderContainerItemRows())
                {
                    dsOrders.OrderContainerItem.AddOrderContainerItemRow(destContainer, item.ShipmentPackageTypeID);
                }

                orgContainer.IsActive = false;
            }

            // Rejoin serial numbers
            var partOrder = 0;

            if (destinationOrder.GetOrderSerialNumberRows().Any(o => o.IsValidState() && o.Active))
            {
                partOrder = destinationOrder.GetOrderSerialNumberRows().Where(o => o.IsValidState() && o.Active).Max(s => s.PartOrder);
            }

            foreach (var orgSerial in orgOrder.GetOrderSerialNumberRows().Where(o => o.IsValidState() && o.Active))
            {
                partOrder++;

                var destSerial = dsOrders.OrderSerialNumber.NewOrderSerialNumberRow();
                destSerial.OrderRow = destinationOrder;
                destSerial.Active = true;
                destSerial.PartOrder = partOrder;

                if (!orgSerial.IsNumberNull())
                {
                    destSerial.Number = orgSerial.Number;
                }

                dsOrders.OrderSerialNumber.AddOrderSerialNumberRow(destSerial);

                orgSerial.Active = false;
                orgSerial.DateRemoved = rejoinDate;
            }

            using (var taOrderShipment = new OrderShipmentTableAdapter())
            {
                // Set quantities for open shipments for the destination order
                foreach (var destShipment in destinationOrder.GetOrderShipmentRows())
                {
                    var shipmentPackageId = destShipment.IsShipmentPackageIDNull()
                        ? -1
                        : destShipment.ShipmentPackageID;

                    if (destShipment.PartQuantity == originalDestinationQuantity && (taOrderShipment.IsShipmentPackageActive(shipmentPackageId) ?? false))
                    {
                        destShipment.PartQuantity = destinationOrder.IsPartQuantityNull()
                            ? 0
                            : destinationOrder.PartQuantity;
                    }
                }
                // Remove source order from any active shipments
                foreach (var orgOrderShipment in orgOrder.GetOrderShipmentRows())
                {
                    var shipmentPackageId = orgOrderShipment.IsShipmentPackageIDNull()
                        ? -1
                        : orgOrderShipment.ShipmentPackageID;

                    if (taOrderShipment.IsShipmentPackageActive(shipmentPackageId) ?? false)
                    {
                        orgOrderShipment.Delete();
                    }
                }
            }

            return destinationOrder;
        }

        private static Tuple<OrdersDataSet, TableAdapterManager> LoadInitialData()
        {
            var dsOrders = new OrdersDataSet
            {
                EnforceConstraints = false // Retrieving full data for only the specified order
            };

            var taFeeType = new d_FeeTypeTableAdapter();
            var taPriceUnit = new PriceUnitTableAdapter();
            var taOrderStatus = new d_OrderStatusTableAdapter();
            var taPriority = new d_PriorityTableAdapter();
            var taUserSummary = new UserSummaryTableAdapter();
            var taProcessingLine = new ProcessingLineTableAdapter();
            var taMedia = new MediaTableAdapter();
            var taOrder = new OrderTableAdapter {ClearBeforeFill = false};
            var taOrderFees = new OrderFeesTableAdapter();
            var taOrderProcesses = new OrderProcessesTableAdapter();
            var taOrderMedia = new Order_MediaTableAdapter();
            var taOrderDocumentLink = new Order_DocumentLinkTableAdapter();
            var taOrderFeeType = new OrderFeeTypeTableAdapter();
            var taDepts = new d_DepartmentTableAdapter();
            var taWorkStatus = new d_WorkStatusTableAdapter();
            var taOrderSerialNumber = new OrderSerialNumberTableAdapter();
            var taContainer = new OrderContainersTableAdapter();
            var taContainerItem = new OrderContainerItemTableAdapter();
            var taOrderChange = new OrderChangeTableAdapter();
            var taOrderShipment = new OrderShipmentTableAdapter();
            var taBatchOrder = new BatchOrderTableAdapter();
            var taBatch = new BatchTableAdapter();
            var taCoc = new COCTableAdapter();
            var taCustomerCommunication = new CustomerCommunicationTableAdapter();
            var taInternalRework = new InternalReworkTableAdapter();
            var taOrderCustomFields = new OrderCustomFieldsTableAdapter();
            var taOrderHold = new OrderHoldTableAdapter();
            var taOrderNote = new OrderNoteTableAdapter();
            var taOrderPartMark = new OrderPartMarkTableAdapter();
            var taOrderProcessAnswer = new OrderProcessAnswerTableAdapter();
            var taOrderProcessesOperator = new OrderProcessesOperatorTableAdapter();
            var taOrderReview = new OrderReviewTableAdapter();
            var taPartSummary = new PartSummaryTableAdapter();
            var taPartInspection = new PartInspectionTableAdapter();
            var taSalesOrder = new SalesOrderTableAdapter();
            var taLabor = new LaborTimeTableAdapter();

            var taManager = new TableAdapterManager
            {
                BatchOrderTableAdapter = taBatchOrder,
                BatchTableAdapter = taBatch,
                COCTableAdapter = taCoc,
                CustomerCommunicationTableAdapter = taCustomerCommunication,
                InternalReworkTableAdapter = taInternalRework,
                MediaTableAdapter = taMedia,
                Order_DocumentLinkTableAdapter = taOrderDocumentLink,
                Order_MediaTableAdapter = taOrderMedia,
                OrderChangeTableAdapter = taOrderChange,
                OrderContainersTableAdapter = taContainer,
                OrderContainerItemTableAdapter = taContainerItem,
                OrderCustomFieldsTableAdapter = taOrderCustomFields,
                OrderFeesTableAdapter = taOrderFees,
                OrderHoldTableAdapter = taOrderHold,
                OrderNoteTableAdapter = taOrderNote,
                OrderPartMarkTableAdapter = taOrderPartMark,
                OrderProcessAnswerTableAdapter = taOrderProcessAnswer,
                OrderProcessesOperatorTableAdapter = taOrderProcessesOperator,
                LaborTimeTableAdapter = taLabor,
                OrderProcessesTableAdapter = taOrderProcesses,
                OrderReviewTableAdapter = taOrderReview,
                OrderSerialNumberTableAdapter = taOrderSerialNumber,
                OrderShipmentTableAdapter = taOrderShipment,
                OrderTableAdapter = taOrder,
                PartSummaryTableAdapter = taPartSummary,
                PartInspectionTableAdapter = taPartInspection,
                SalesOrderTableAdapter = taSalesOrder
            };

            try
            {
                using (new UsingDataSetLoad(dsOrders))
                {
                    taFeeType.Fill(dsOrders.d_FeeType);
                    taPriceUnit.Fill(dsOrders.PriceUnit);
                    taOrderStatus.Fill(dsOrders.d_OrderStatus);
                    taPriority.Fill(dsOrders.d_Priority);
                    taUserSummary.Fill(dsOrders.UserSummary);
                    taProcessingLine.Fill(dsOrders.ProcessingLine);

                    // All orders involved in rejoin must be be active
                    taOrder.FillRecentByStatus(dsOrders.Order, int.MaxValue, Properties.Settings.Default.OrderStatusOpen);

                    taOrderFeeType.Fill(dsOrders.OrderFeeType);
                    taDepts.Fill(dsOrders.d_Department);
                    taWorkStatus.Fill(dsOrders.d_WorkStatus);
                }

                return new Tuple<OrdersDataSet, TableAdapterManager>(dsOrders, taManager);
            }
            catch (Exception exc)
            {
                var errorStr = "Error loading rejoin data.";
                if (dsOrders.HasErrors)
                {
                    errorStr = dsOrders.GetDataErrors();
                }

                _log.Error(exc, errorStr);

                return null;
            }
        }

        private void LoadData(OrdersDataSet.OrderRow order, OrdersDataSet dsOrders)
        {
            try
            {
                var orderId = order.OrderID;

                using (new UsingDataSetLoad(dsOrders))
                {
                    using (var taCustomerSummary = new CustomerSummaryTableAdapter { ClearBeforeFill = false })
                    {
                        taCustomerSummary.FillByOrder(dsOrders.CustomerSummary, orderId);
                    }

                    using (var taCustomerShippingSummary = new CustomerShippingSummaryTableAdapter { ClearBeforeFill = false })
                    {
                        taCustomerShippingSummary.FillByOrder(dsOrders.CustomerShippingSummary, orderId);
                    }

                    using (var taCustomerAddress = new CustomerAddressTableAdapter { ClearBeforeFill = false })
                    {
                        taCustomerAddress.FillByOrder(dsOrders.CustomerAddress, orderId);
                    }

                    using (var taPartSummary = new PartSummaryTableAdapter { ClearBeforeFill = false })
                    {
                        taPartSummary.FillByOrder(dsOrders.PartSummary, orderId);
                    }

                    using (var taMedia = new MediaTableAdapter { ClearBeforeFill = false })
                    {
                        taMedia.FillByOrder(dsOrders.Media, orderId);
                    }

                    using (var taOrderFees = new OrderFeesTableAdapter { ClearBeforeFill = false })
                    {
                        taOrderFees.FillByOrder(dsOrders.OrderFees, orderId);
                    }

                    using (var taOrderProcesses = new OrderProcessesTableAdapter { ClearBeforeFill = false })
                    {
                        taOrderProcesses.FillBy(dsOrders.OrderProcesses, orderId);
                    }

                    var taOrderMedia = new Order_MediaTableAdapter{ClearBeforeFill = false};
                    {
                        taOrderMedia.FillByOrder(dsOrders.Order_Media, orderId);
                    }

                    using (var taOrderDocumentLink = new Order_DocumentLinkTableAdapter { ClearBeforeFill = false })
                    {
                        taOrderDocumentLink.FillByOrder(dsOrders.Order_DocumentLink, orderId);
                    }

                    using (var taOrderSerialNumber = new OrderSerialNumberTableAdapter { ClearBeforeFill = false })
                    {
                        taOrderSerialNumber.FillByOrder(dsOrders.OrderSerialNumber, orderId);
                    }

                    using (var taContainer = new OrderContainersTableAdapter { ClearBeforeFill = false })
                    {
                        taContainer.FillByOrder(dsOrders.OrderContainers, orderId);
                    }

                    using (var taContainerItem = new OrderContainerItemTableAdapter { ClearBeforeFill = false })
                    {
                        taContainerItem.FillByOrder(dsOrders.OrderContainerItem, orderId);
                    }

                    using (var taOrderChange = new OrderChangeTableAdapter { ClearBeforeFill = false })
                    {
                        taOrderChange.FillByOrderID(dsOrders.OrderChange, orderId);
                    }

                    using (var taOrder = new OrderTableAdapter { ClearBeforeFill = false })
                    {
                        foreach (var orderChange in dsOrders.OrderChange)
                        {
                            var parentOrderId = orderChange.ParentOrderID;
                            var childOrderId = orderChange.ChildOrderID;

                            if (dsOrders.Order.FindByOrderID(parentOrderId) == null)
                            {
                                taOrder.FillByOrderID(dsOrders.Order, parentOrderId);
                            }

                            if (dsOrders.Order.FindByOrderID(childOrderId) == null)
                            {
                                taOrder.FillByOrderID(dsOrders.Order, childOrderId);
                            }
                        }
                    }

                    using (var taOrderShipment = new OrderShipmentTableAdapter { ClearBeforeFill = false })
                    {
                        taOrderShipment.FillByOrder(dsOrders.OrderShipment, orderId);
                    }

                    using (var taBatchOrder = new BatchOrderTableAdapter { ClearBeforeFill = false })
                    {
                        taBatchOrder.FillByOrder(dsOrders.BatchOrder, orderId);
                    }

                    using (var taBatch = new BatchTableAdapter { ClearBeforeFill = false })
                    {
                        taBatch.FillByOrder(dsOrders.Batch, orderId);
                    }

                    using (var taCoc = new COCTableAdapter { ClearBeforeFill = false })
                    {
                        taCoc.FillByOrderNoData(dsOrders.COC, orderId);
                    }
                    using (var taCustomerCommunication = new CustomerCommunicationTableAdapter { ClearBeforeFill = false })
                    {
                        taCustomerCommunication.FillByOrder(dsOrders.CustomerCommunication, orderId);
                    }
                    using (var taContact = new ContactSummaryTableAdapter { ClearBeforeFill = false })
                    {
                        taContact.FillByOrder(dsOrders.ContactSummary, orderId);
                    }

                    using (var taInternalRework = new InternalReworkTableAdapter { ClearBeforeFill = false })
                    {
                        taInternalRework.FillByOrderID(dsOrders.InternalRework, orderId);
                    }

                    using (var taOrderCustomFields = new OrderCustomFieldsTableAdapter { ClearBeforeFill = false })
                    {
                        taOrderCustomFields.FillByOrder(dsOrders.OrderCustomFields, orderId);
                    }

                    using (var taOrderHold = new OrderHoldTableAdapter { ClearBeforeFill = false })
                    {
                        taOrderHold.Fill(dsOrders.OrderHold, orderId);
                    }

                    using (var taOrderNote = new OrderNoteTableAdapter { ClearBeforeFill = false })
                    {
                        taOrderNote.FillByOrder(dsOrders.OrderNote, orderId);
                    }

                    using (var taOrderPartMark = new OrderPartMarkTableAdapter { ClearBeforeFill = false })
                    {
                        taOrderPartMark.FillByOrder(dsOrders.OrderPartMark, orderId);
                    }

                    using (var taOrderProcessAnswer = new OrderProcessAnswerTableAdapter { ClearBeforeFill = false })
                    {
                        taOrderProcessAnswer.FillBy(dsOrders.OrderProcessAnswer, orderId);
                    }

                    using (var taOrderProcessesOperator = new OrderProcessesOperatorTableAdapter { ClearBeforeFill = false })
                    {
                        taOrderProcessesOperator.FillByOrder(dsOrders.OrderProcessesOperator, orderId);
                    }

                    using (var taLabor = new LaborTimeTableAdapter { ClearBeforeFill = false })
                    {
                        taLabor.FillByOrder(dsOrders.LaborTime, orderId);
                    }

                    using (var taOrderReview = new OrderReviewTableAdapter { ClearBeforeFill = false })
                    {
                        taOrderReview.FillByOrder(dsOrders.OrderReview, orderId);
                    }

                    using (var taPartInspectionType = new PartInspectionTypeTableAdapter { ClearBeforeFill = false })
                    {
                        taPartInspectionType.Fill(dsOrders.PartInspectionType);
                    }

                    using (var taPartInspection = new PartInspectionTableAdapter { ClearBeforeFill = false })
                    {
                        taPartInspection.FillBy(dsOrders.PartInspection, orderId);
                    }

                    using (var taSalesOrder = new SalesOrderTableAdapter { ClearBeforeFill = false })
                    {
                        taSalesOrder.FillByOrderID(dsOrders.SalesOrder, orderId);
                    }

                    using (var taOrderTemplate = new OrderTemplateTableAdapter { ClearBeforeFill = false })
                    {
                        taOrderTemplate.FillByOrder(dsOrders.OrderTemplate, orderId);
                    }
                }
            }
            catch (Exception exc)
            {
                var errorStr = dsOrders.GetDataErrors();

                if (string.IsNullOrEmpty(errorStr))
                {
                    errorStr = "Error loading data.";
                }

                _log.Error(exc, errorStr);
            }

        }

        #endregion
    }
}