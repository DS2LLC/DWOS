using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Data.Datasets.PartsDatasetTableAdapters;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Admin;
using DWOS.UI.Reports;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.UltraGauge.Resources;
using Infragistics.Win;
using Infragistics.Shared;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Interop;
using DWOS.Data.Order;
using DWOS.Reports.ReportData;


namespace DWOS.UI.Sales
{
    public partial class OrderEntry: DataEditorBase
    {
        #region Fields

        private const string FILTER_STATUS = "<Filter>";
        private const string ORDER_REVIEW_STATUS = "Order Review";
        private const string IMPORT_EXPORT_REVIEW_STATUS = "Import/Export Review";
        private const decimal MAX_WEIGHT = 999999.99M;

        public enum OrderEntryMode { Normal, Review, BlanketPO, ImportExportReview }

        private readonly List<int> _customFieldsLoaded = new List<int>();
        private List<int> _customerPartsLoaded = new List<int>();
        private List<int> _successfullyReviewedOrders = new List<int>();
        private List<int> _batchesCreatedAfterReview = new List<int>();
        private Dictionary<string, bool> _dataLoadedByStatus = new Dictionary<string, bool>();
        private readonly HashSet<int> _loadedOrders = new HashSet<int>();
        private bool _inSavingData;
        private DataRowChangeEventHandler _onOrderRowChanged;
        private DataRowChangeEventHandler _onCustomerCommRowChanged;
        private DataRowChangeEventHandler _onSalesOrderRowChanged;
        private bool _resetStatusFilter;
        private ValueListItem _previousStatusFilter;
        private readonly FlyoutManager _flyoutManager;

        //private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Properties

        private int StatusBarOrderCount
        {
            set { this.ultraStatusBar1.Panels["OrderCount"].Text = value + " Orders"; }
        }

        public int SelectedWO { get; set; }

        private ComboBoxTool StatusFilter
        {
            get { return base.toolbarManager.Tools["StatusTool"] as ComboBoxTool; }
        }

        public OrderEntryMode Mode { get; set; }

        private string ModeDisplayName
        {
            get
            {
                switch(this.Mode)
                {
                    case OrderEntryMode.Normal:
                        return "Order Entry";
                    case OrderEntryMode.Review:
                        return "Order Review";
                    case OrderEntryMode.BlanketPO:
                        return "Blanket PO Manager";
                    case OrderEntryMode.ImportExportReview:
                        return "Import/Export Review";
                    default:
                        return "NA";
                }
            }
        }

        protected override bool AllowChanges =>
            SecurityManager.Current.IsInRole("OrderEntry.Edit");

        private OrderRootNode RootNode => tvwTOC.Nodes.Count == 1
            ? tvwTOC.Nodes[0] as OrderRootNode
            : null;

        #endregion Properties

        #region Methods

        public OrderEntry(OrderEntryMode mode)
        {
            this.InitializeComponent();
            _flyoutManager = new FlyoutManager(this);

            if(DesignMode)
                return;

            this.Mode = mode;
            
            base.AddDataPanel(this.dpCustomerComm);
            base.AddDataPanel(this.dpOrder);
            base.AddDataPanel(this.dpOrderProcessing);
            base.AddDataPanel(this.dpOrderPartMarking);
            base.AddDataPanel(this.dpOrderNoteInfo);
            base.AddDataPanel(this.dpOrderChangeInfo);
            base.AddDataPanel(this.dpSalesOrder);

            base.AddDataPanel(this.dpCOC);
            base.AddDataPanel(this.dpShipping);
            base.AddDataPanel(this.dpOrderReview);
            base.AddDataPanel(this.dpInternalRework);
            base.AddDataPanel(this.dpOrderChangeInfo);
            base.AddDataPanel(this.dpBlanketPO);
            base.AddDataPanel(this.dpOrderContainer);
            base.AddDataPanel(this.dpBatch);
            base.AddDataPanel(this.dpLabor);
            base.AddDataPanel(this.dpBulkCOC);
            base.AddDataPanel(this.dpBatchCoc);
            base.AddDataPanel(this.dpHold);
            base.AddDataPanel(this.dpSerialNumber);
            base.AddDataPanel(this.dpPartInspection);
            AddDataPanel(dpBillOfLading);
            AddDataPanel(dpOrderApproval);

            tvwTOC.Override.SortComparer = new OrderEntryNodeSorter();
        }

        private void InitialLoadData()
        {
            using(new UsingTimeMe("Initial data load"))
            {
                this.dsOrders.EnforceConstraints = false;
                this.dsOrders.ContactSummary.BeginLoadData();
                this.dsOrders.d_OrderStatus.BeginLoadData();
                this.dsOrders.d_Priority.BeginLoadData();
                this.dsOrders.UserSummary.BeginLoadData();
                this.dsOrders.OrderFeeType.BeginLoadData();
                this.dsOrders.d_ShippingCarrier.BeginLoadData();
                this.dsOrders.CustomerSummary.BeginLoadData();
                this.dsOrders.CustomerShippingSummary.BeginLoadData();
                this.dsOrders.CustomerAddress.BeginLoadData();
                this.dsOrders.SalesOrder.BeginLoadData();
                this.dsOrders.Lists.BeginLoadData();
                this.dsOrders.ListValues.BeginLoadData();
                this.dsOrders.ProductClass.BeginLoadData();
                this.dsOrders.WorkDescription.BeginLoadData();
                this.dsOrders.OrderReviewType.BeginLoadData();

                //Generic Loads
                this.taHoldReason.Fill(this.dsOrders.d_HoldReason);
                this.taOrderStatus.Fill(this.dsOrders.d_OrderStatus);
                this.taPriority.Fill(this.dsOrders.d_Priority);
                this.taUserSummary.Fill(this.dsOrders.UserSummary);
                this.taOrderFeeType.Fill(this.dsOrders.OrderFeeType);
                this.taShippingCarrier.Fill(this.dsOrders.d_ShippingCarrier);
                this.taCustomerShippingSummary.Fill(this.dsOrders.CustomerShippingSummary);
                this.taCustomerAddress.Fill(this.dsOrders.CustomerAddress);
                this.taPriceUnit.Fill(this.dsOrders.PriceUnit);
                this.taCustomerSummary.FillByActiveOrInUse(this.dsOrders.CustomerSummary);
                this.taCustomerFee.Fill(this.dsOrders.CustomerFee);
                this.taContactSummary.Fill(this.dsOrders.ContactSummary);
                this.taSalesOrder.Fill(this.dsOrders.SalesOrder);

                using(var taDepts = new d_DepartmentTableAdapter())
                {
                    taDepts.Fill(this.dsOrders.d_Department);
                }
                using(var taWS = new d_WorkStatusTableAdapter())
                {
                    taWS.Fill(this.dsOrders.d_WorkStatus);
                }

                using (var taLine = new ProcessingLineTableAdapter())
                {
                    taLine.Fill(dsOrders.ProcessingLine);
                }

                taLists.Fill(dsOrders.Lists);
                taListValues.Fill(dsOrders.ListValues);
                taProductClass.Fill(dsOrders.ProductClass);
                taPartInspectionType.Fill(dsOrders.PartInspectionType);
                taShipmentPackageType.Fill(dsOrders.ShipmentPackageType);
                taOrderApprovalTerm.Fill(dsOrders.OrderApprovalTerm);
                taWorkDescription.Fill(dsOrders.WorkDescription);
                taOrderReviewType.Fill(dsOrders.OrderReviewType);

                this.dsOrders.ContactSummary.EndLoadData();
                this.dsOrders.d_OrderStatus.EndLoadData();
                this.dsOrders.d_Priority.EndLoadData();
                this.dsOrders.UserSummary.EndLoadData();
                this.dsOrders.OrderFeeType.EndLoadData();
                this.dsOrders.d_ShippingCarrier.EndLoadData();
                this.dsOrders.CustomerSummary.EndLoadData();
                this.dsOrders.CustomerShippingSummary.EndLoadData();
                this.dsOrders.CustomerAddress.EndLoadData();
                this.dsOrders.SalesOrder.EndLoadData();
                this.dsOrders.Lists.EndLoadData();
                this.dsOrders.ListValues.EndLoadData();
                this.dsOrders.ProductClass.EndLoadData();
                this.dsOrders.WorkDescription.EndLoadData();
                this.dsOrders.OrderReviewType.EndLoadData();
            }

            //pre-load that no statuses have been loaded
            foreach (OrdersDataSet.d_OrderStatusRow item in this.dsOrders.d_OrderStatus)
                _dataLoadedByStatus.Add(item.OrderStatusID, false);
            
            //Load panels
            this.dpCustomerComm.LoadData(this.dsOrders, this.taContactSummary);
            this.dpOrder.LoadData(this.dsOrders);
            this.dpOrder.ClearPartSummaryBeforeFill = false;
            this.dpCOC.LoadData(this.dsOrders, this.taCOC);
            this.dpShipping.LoadData(this.dsOrders, this.taOrderShipment);
            this.dpOrderReview.LoadData(this.dsOrders);
            this.dpOrderProcessing.LoadData(this.dsOrders);
            this.dpOrderPartMarking.LoadData(this.dsOrders);
            this.dpOrderNoteInfo.LoadData(this.dsOrders);
            this.dpInternalRework.LoadData(this.dsOrders);
            this.dpOrderChangeInfo.LoadData(this.dsOrders);
            this.dpSalesOrder.LoadData(this.dsOrders);
            this.dpBlanketPO.LoadData(this.dsOrders, this.taPartSummary, this.taOrder);
            this.dpOrderContainer.LoadData(this.dsOrders);
            this.dpBatch.LoadData(this.dsOrders, this.taBatchOrder, this.taPartSummary);
            this.dpLabor.LoadData(this.dsOrders, this.taBatchProcesses);
            this.dpBulkCOC.LoadData(this.dsOrders);
            this.dpBatchCoc.LoadData(dsOrders, taBatchCoc);
            this.dpHold.LoadData(this.dsOrders);
            this.dpSerialNumber.LoadData(this.dsOrders);
            this.dpPartInspection.LoadData(this.dsOrders);
            dpBillOfLading.LoadData(dsOrders);
            dpOrderApproval.LoadData(dsOrders);

            this.dpOrder.BeforeCustomerChanged += this.dpOrder_BeforeCustomerChanged;
            this.dpOrder.PartsReloaded += this.dpOrder_PartsReloaded;
            this.dpOrder.AfterChildRowAdded += dpOrder_AfterChildRowAdded;
            this.dpOrder.BeforeChildRowDeleted += dpOrder_BeforeChildRowDeleted;
            this.dpOrder.QuickFilter += this.dpOrder_QuickFilter;
            this.dpBatch.QuickFilter += this.dpOrder_QuickFilter;
            this.dpInternalRework.GoToOrder += GoToOrder;
            this.dpOrderChangeInfo.GoToOrder += GoToOrder;

            this.dpBlanketPO.BeforeCustomerChanged += this.dpOrder_BeforeCustomerChanged;
            this.dpBlanketPO.PartsReloaded += this.dpBlanketPO_PartsReloaded;

            this._onSalesOrderRowChanged = this.SalesOrder_RowChanged;
            this._onOrderRowChanged = this.Order_RowChanged;
            this._onCustomerCommRowChanged = this.CustomerCommunication_RowChanged;

            this.dsOrders.Order.RowChanged += this._onOrderRowChanged;
            this.dsOrders.CustomerCommunication.RowChanged += this._onCustomerCommRowChanged;
            dsOrders.OrderChange.OrderChangeRowDeleting += OnOrderChangeRowDeleting;
        }

        private void LoadData(string status)
        {
            using(new UsingTimeMe("Loading Data: " + status))
            {
                //if not loaded let then loaded
                bool isLoaded;
                
                if(this._dataLoadedByStatus.TryGetValue(status, out isLoaded) && !isLoaded)
                {
                    _log.Debug("Loading data: " + status);
                    this.dsOrders.EnforceConstraints = false;

                    this.dsOrders.Order.RowChanged -= this._onOrderRowChanged;
                    this.dsOrders.Order.BeginLoadData();
                                       
                    if(status == ORDER_REVIEW_STATUS)
                    {
                        this.taOrder.FillByWorkStatus(this.dsOrders.Order, ApplicationSettings.Current.WorkStatusPendingOR);
                    }
                    else if(status == IMPORT_EXPORT_REVIEW_STATUS)
                    {
                        taOrder.FillByWorkStatus(
                            this.dsOrders.Order,
                            ApplicationSettings.Current.WorkStatusPendingImportExportReview);
                    }
                    else if (this.Mode == OrderEntryMode.BlanketPO)
                    {
                        this.taOrderTemplate.FillByActive(this.dsOrders.OrderTemplate, Properties.Settings.Default.OrderStatusOpen == status);
                    }
                    else
                    {
                        int maxCount;
                        if (status == Properties.Settings.Default.OrderStatusOpen)
                        {
                            //only filter if not open
                            maxCount = int.MaxValue;
                        }
                        else
                        {
                            maxCount = UserSettings.Default.MaxClosedOrders;
                        }

                        this.taOrder.FillRecentByStatus(this.dsOrders.Order, maxCount, status);
                    }

                    this.dsOrders.Order.EndLoadData();
                    this.dsOrders.Order.RowChanged += this._onOrderRowChanged;

                    //update that data was loaded
                    this._dataLoadedByStatus[status] = true;
                }
            }
        }
        
        protected override void ReloadTOC()
        {
            try
            {
                _log.Debug("ReloadTOC");
                
                var status = this.dsOrders.d_OrderStatus.FindByOrderStatusID(((ValueListItem)this.StatusFilter.SelectedItem).DataValue.ToString());

                if(status != null) 
                    this.LoadTOC(status);

                this.UpdateTotalCounts();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error reloading TOC.";
                _log.Error(exc, errorMsg);
            }
        }

        private void LoadTOC(OrdersDataSet.d_OrderStatusRow status)
        {
            try
            {
                _log.Info("Loading TOC by " + status.OrderStatusID);

                using(new UsingTimeMe("Load TOC by Status: " + status.OrderStatusID))
                {
                    //ensure the data is loaded
                    this.LoadData(status.OrderStatusID);

                    using(new UsingTreeLoad(tvwTOC))
                    {
                        var rootNode = RootNode;

                        if(rootNode == null)
                        {
                            tvwTOC.Nodes.Clear();
                            rootNode = new OrderRootNode(this.dsOrders);
                            tvwTOC.Nodes.Add(rootNode);
                            rootNode.Expanded = true;
                        }
                        else
                        {
                            rootNode.Nodes.Clear();
                            rootNode.Text = status.OrderStatusID + " Orders";
                        }

                        //clear any selected nodes, will force tool refresh (required if rootNode is selected before and after (i.e. never fires selection change))
                        tvwTOC.PerformAction(UltraTreeAction.ClearAllSelectedNodes, false, false);

                        //if loading Blanket PO's
                        if(Mode == OrderEntryMode.BlanketPO)
                        {
                            rootNode.Text = "Blanket PO's";
                            var loadActive = status.OrderStatusID == Properties.Settings.Default.OrderStatusOpen;
                            var poRows = this.dsOrders.OrderTemplate.Where(w => w.IsValidState() && w.IsActive == loadActive);

                            foreach(var poRow in poRows)
                            {
                                rootNode.Nodes.Add(new BlanketPONode(poRow));
                            }

                            rootNode.Select();
                        }
                        else
                        {
                            string filter;

                            //if in review mode then only show orders with the status order review
                            if (Mode == OrderEntryMode.Review)
                            {
                                filter = this.dsOrders.Order.WorkStatusColumn.ColumnName + " = '" + ApplicationSettings.Current.WorkStatusPendingOR + "'";
                            }
                            else if (Mode == OrderEntryMode.ImportExportReview)
                            {
                                filter = dsOrders.Order.WorkStatusColumn.ColumnName + " = '" + ApplicationSettings.Current.WorkStatusPendingImportExportReview + "'";
                            }
                            else
                            {
                                filter = this.dsOrders.Order.StatusColumn.ColumnName + " = '" + status.OrderStatusID + "'";
                            }

                            //load only row that meet current load status
                            var rows = this.dsOrders.Order.Select(filter, this.dsOrders.Order.OrderIDColumn.ColumnName) as OrdersDataSet.OrderRow[];

                            //Load Sales Orders
                           
                            OrdersDataSet.SalesOrderRow[] salesOrders = null;
                            if (Mode == OrderEntryMode.Review || Mode == OrderEntryMode.ImportExportReview)
                            {
                                salesOrders = this.dsOrders.SalesOrder.Select() as OrdersDataSet.SalesOrderRow[];
                            }
                            else
                            {

                                using (var taSalesOrder = new SalesOrderTableAdapter())
                                {
                                    if (status.OrderStatusID == Properties.Settings.Default.OrderStatusClosed)
                                    {
                                        taSalesOrder.FillByClosedOrder(this.dsOrders.SalesOrder);
                                        salesOrders = this.dsOrders.SalesOrder.Select("", this.dsOrders.SalesOrder.SalesOrderIDColumn.ColumnName) as OrdersDataSet.SalesOrderRow[];
                                    }
                                    else
                                    {
                                        taSalesOrder.Fill(this.dsOrders.SalesOrder);
                                        salesOrders = this.dsOrders.SalesOrder.Select(filter, this.dsOrders.SalesOrder.SalesOrderIDColumn.ColumnName) as OrdersDataSet.SalesOrderRow[];
                                    }
                                }

                            }

                            foreach (var salesOrder in (salesOrders ?? Enumerable.Empty<OrdersDataSet.SalesOrderRow>()))
                            {
                                var salesNode = new SalesOrderNode(salesOrder, status, this.dsOrders);
                                rootNode.Nodes.Add(salesNode);
                            }

                            //Load the Work Orders
                            foreach (var pr in rows ?? Enumerable.Empty<OrdersDataSet.OrderRow>())
                            {
                                var order = new OrderNode(pr, this.Mode);
                                
                                //See if any belong to a Sales Order
                                if(!pr.IsSalesOrderIDNull())
                                {
                                    var so = rootNode.FindNodes <SalesOrderNode>(node => node.DataRow.SalesOrderID == pr.SalesOrderID);
                                    //if (so?.Count == 0)
                                    //{
                                    //    salesOrders = this.dsOrders.SalesOrder.Select($"SalesOrderID = {pr.SalesOrderID}", this.dsOrders.SalesOrder.SalesOrderIDColumn.ColumnName) as OrdersDataSet.SalesOrderRow[];


                                    //}
                                    if (so?.Count == 1)
                                    {
                                        //Add WO to the SO
                                        so[0].Nodes.Add(order);
                                    }
                                }
                                else
                                {
                                    rootNode.Nodes.Add(order);
                                }
                            }

                            //Don't show empty sales order nodes in order review
                            if (Mode == OrderEntryMode.Review || Mode == OrderEntryMode.ImportExportReview)
                            {
                                foreach (var so in rootNode.FindNodes <SalesOrderNode>(node => node.Nodes.Count == 0))
                                {
                                    so.Visible = false;
                                }
                            }

                            rootNode.Select();
                            this.StatusBarOrderCount = rows?.Length ?? 0;
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error loading the orders.", exc);
            }
        }

        private void LoadTOC(OrderSearchField field, string searchCriteria, string customFieldName, bool exactMatch)
        {
            try
            {
                _log.Debug("Loading TOC with Filter: " + field + " = " + searchCriteria + " MODE: " +  Mode.ToString());

                using (new UsingWaitCursor(this))
                {
                    using (new UsingTreeLoad(tvwTOC))
                    {
                        if (_validators.ValidateControls())
                        {
                            //clear any selected nodes, will force tool refresh (required if rootNode is selected before and after)
                            tvwTOC.PerformAction(UltraTreeAction.ClearAllSelectedNodes, false, false);
                                
                            var rootNode = tvwTOC.Nodes[0];
                            rootNode.Nodes.Clear();

                            if (field == OrderSearchField.Custom)
                            {
                                rootNode.Text = "Filtered Orders [" + customFieldName + " : " + searchCriteria + "]";
                            }
                            else
                            {
                                rootNode.Text = "Filtered Orders [" + field + " : " + searchCriteria + "]";
                            }

                            int filterIndex = this.StatusFilter.ValueList.FindStringExact(FILTER_STATUS);
                            this.StatusFilter.SelectedIndex = Math.Max(filterIndex, 0);

                            ICollection<OrdersDataSet.OrderRow> filteredRows = null;

                            using (new UsingTimeMe("filtering data"))
                            {
                                filteredRows = OrderEntryFilter.FilterOrders(new OrderEntryFilter.FilterOrdersParams
                                {
                                    OrdersDataset = dsOrders,
                                    Mode = Mode,
                                    SearchField = field,
                                    CustomFieldName = customFieldName,
                                    SearchCriteria = searchCriteria,
                                    ExactMatch = exactMatch
                                });
                            }

                            //load all found rows
                            if (filteredRows != null && filteredRows.Count > 0)
                            {
                                //if in order review mode then ensure orders are still in order review mode
                                if (Mode == OrderEntryMode.Review)
                                {
                                    filteredRows = filteredRows.Where(or => or.WorkStatus == ApplicationSettings.Current.WorkStatusPendingOR).ToArray();
                                }
                                else if (Mode == OrderEntryMode.ImportExportReview)
                                {
                                    filteredRows = filteredRows
                                        .Where(or => or.WorkStatus == ApplicationSettings.Current.WorkStatusPendingImportExportReview)
                                        .ToArray();
                                }

                                _log.Debug("Search Filter found " + filteredRows.Count + " rows.");

                                // Add nodes for Work Orders and their Sales Orders
                                var salesOrderNodes = new List<SalesOrderNode>();
                                foreach (var pr in filteredRows)
                                {
                                    var orderNode = new OrderNode(pr, this.Mode);

                                    if (pr.SalesOrderRow != null)
                                    {
                                        var salesOrderNode = salesOrderNodes.FirstOrDefault(n => n.DataRow.SalesOrderID == pr.SalesOrderRow.SalesOrderID);

                                        if (salesOrderNode == null)
                                        {
                                            salesOrderNode = new SalesOrderNode(pr.SalesOrderRow, dsOrders)
                                            {
                                                Expanded = true
                                            };

                                            rootNode.Nodes.Add(salesOrderNode);
                                            salesOrderNodes.Add(salesOrderNode);
                                        }

                                        salesOrderNode.Nodes.Add(orderNode);
                                    }
                                    else
                                    {
                                        rootNode.Nodes.Add(orderNode);
                                    }
                                }
                            }

                            this.StatusBarOrderCount = filteredRows == null ? 0 : filteredRows.Count;

                            if (rootNode.HasNodes)
                            {
                                // Sort (using the comparer) and select the first visible order node
                                var comparer = new OrderEntryNodeSorter();
                                rootNode.Nodes.OfType<UltraTreeNode>().OrderBy(o => o, comparer).First().Select();
                            }
                            else
                            {
                                rootNode.Select();
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error filtering the rows based on the search criteria, filter: " + field + " = " + searchCriteria;
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void LoadTOC(List <int> orders)
        {
            if (!_validators.ValidateControls())
            {
                return;
            }

            //clear any selected nodes, will force tool refresh (required if rootNode is selected before and after)
            tvwTOC.PerformAction(UltraTreeAction.ClearAllSelectedNodes, false, false);

            var rootNode = tvwTOC.Nodes[0];
            rootNode.Nodes.Clear();
            rootNode.Text = "Filtered Orders";

            var filterIndex = this.StatusFilter.ValueList.FindStringExact(FILTER_STATUS);
            this.StatusFilter.SelectedIndex = filterIndex >= 0 ? filterIndex : 0;

            var filteredRows = this.dsOrders.Order.Where(or => or.RowState != DataRowState.Deleted && orders.Contains(or.OrderID)).ToArray();

            if(filteredRows.Length > 0)
            {
                //if in order review mode then ensure orders are still in order review mode
                if (Mode == OrderEntryMode.Review)
                {
                    filteredRows = filteredRows.Where(or => or.WorkStatus == ApplicationSettings.Current.WorkStatusPendingOR).ToArray();
                }
                else if (Mode == OrderEntryMode.ImportExportReview)
                {
                    filteredRows = filteredRows
                        .Where(or => or.WorkStatus == ApplicationSettings.Current.WorkStatusPendingImportExportReview)
                        .ToArray();
                }

                _log.Debug("Search Filter found " + filteredRows.Length + " rows.");

                foreach(var pr in filteredRows)
                    rootNode.Nodes.Add(new OrderNode(pr, this.Mode));
            }

            this.StatusBarOrderCount = filteredRows.Length;

            //select first order node if exists
            if(rootNode.Nodes.Count > 0)
                rootNode.Nodes[0].Select();
            else
                rootNode.Select();
        }

        private void LoadParts(int customerID)
        {
            if (this._customerPartsLoaded.Contains(customerID))
            {
                return;
            }

            using(new UsingTimeMe("Loading Parts by customer: " + customerID))
            {
                this.dpOrder.PartsLoading = true;
                this.dpBlanketPO.PartsLoading = true;

                this.taPartSummary.ClearBeforeFill = true;
                this.taPartSummary.FillByCustomerActive(this.dsOrders.PartSummary, customerID);
                this._customerPartsLoaded.Add(customerID);

                this.dpOrder.PartsLoading = false;
                this.dpBlanketPO.PartsLoading = false;
            }
        }

        private void LoadCustomFields(int customerID)
        {
            if (this._customFieldsLoaded.Contains(customerID))
            {
                return;
            }

            this.taCustomField.FillByCustomer(this.dsOrders.CustomField, customerID);
            this._customFieldsLoaded.Add(customerID);
        }

        private void LoadStatusValueList()
        {
            _log.Debug("Loading Filter combo box");

            var statusValues = new List <string>();
            
            statusValues.Add(FILTER_STATUS);
            
            //if in review mode then remove open/closed from the list and add Order Review
            if(this.Mode == OrderEntryMode.Review)
            {
                this.dsOrders.d_OrderStatus.Addd_OrderStatusRow(ORDER_REVIEW_STATUS);
                this.dsOrders.d_OrderStatus.AcceptChanges();

                _dataLoadedByStatus.Add(ORDER_REVIEW_STATUS, false);
                statusValues.Add(ORDER_REVIEW_STATUS);
            }
            else if (Mode == OrderEntryMode.ImportExportReview)
            {
                dsOrders.d_OrderStatus.Addd_OrderStatusRow(IMPORT_EXPORT_REVIEW_STATUS);
                dsOrders.d_OrderStatus.AcceptChanges();

                _dataLoadedByStatus.Add(IMPORT_EXPORT_REVIEW_STATUS, false);
                statusValues.Add(IMPORT_EXPORT_REVIEW_STATUS);
            }
            else
            {
                statusValues.Add(Properties.Settings.Default.OrderStatusOpen);
                statusValues.Add(Properties.Settings.Default.OrderStatusClosed);
            }

            //add all values to the combo box
            foreach (var status in statusValues)
            {
                Bitmap statusImg;
                if (status == Properties.Settings.Default.OrderStatusOpen)
                {
                    statusImg = Properties.Resources.Status_Open;
                }
                else if (status == Properties.Settings.Default.OrderStatusClosed)
                {
                    statusImg = Properties.Resources.Status_Closed;
                }
                else
                {
                    statusImg = Properties.Resources.Status_None;
                }

                var statusFilterItem = new ValueListItem(status, status);
                statusFilterItem.Appearance.Image = statusImg;
                this.StatusFilter.ValueList.ValueListItems.Add(statusFilterItem);
            }
        }

        protected override bool SaveData()
        {
            ReceivingTableAdapter taReceiving = null;

            try
            {
                this._inSavingData = true;
                _log.Info("Saving Data in mode " + ModeDisplayName);

                var appSettings = ApplicationSettings.Current;

                base.EndAllEdits();

                List<DataRow> addedSalesOrders                      = DataUtilities.GetRowsByRowState(this.dsOrders.SalesOrder, DataRowState.Added);
                Dictionary<DataRow, object[]> deletedSalesOrders    = DataUtilities.GetRowsAndValuesByRowState(this.dsOrders.SalesOrder, DataRowState.Deleted | DataRowState.Detached);
                List<DataRow> addedOrders                           = DataUtilities.GetRowsByRowState(this.dsOrders.Order, DataRowState.Added);
                Dictionary<DataRow, object[]> modifiedOrders        = DataUtilities.GetRowsAndValuesByRowState(this.dsOrders.Order, DataRowState.Modified);
                Dictionary<DataRow, object[]> deletedOrders         = DataUtilities.GetRowsAndValuesByRowState(this.dsOrders.Order, DataRowState.Deleted | DataRowState.Detached);
                var addedOrderProcesses                             = DataUtilities.GetRowsByRowState(this.dsOrders.OrderProcesses, DataRowState.Added).Where(r => ((OrdersDataSet.OrderProcessesRow)r).OrderID > 0);
                var addedOrderReviews                               = DataUtilities.GetRowsByRowState(this.dsOrders.OrderReview, DataRowState.Added);
                Dictionary<DataRow, object[]> modifiedProcesses     = DataUtilities.GetRowsAndValuesByRowState(this.dsOrders.OrderProcesses, DataRowState.Modified);

                var addedOrderApprovals = DataUtilities.GetRowsByRowState(dsOrders.OrderApproval, DataRowState.Added);
                var deletedOrderApprovals = DataUtilities.GetRowsAndValuesByRowState(dsOrders.OrderApproval, DataRowState.Deleted);

                if (!appSettings.OrderReviewEnabled && appSettings.AutomaticallyBatchSalesOrder)
                {
                    var batchedSalesOrders = new HashSet<int>();

                    // Create batches for new Sales Orders.
                    foreach (var salesOrder in addedSalesOrders.OfType<OrdersDataSet.SalesOrderRow>())
                    {
                        if (salesOrder.GetOrderRows().Length == 0)
                        {
                            continue;
                        }

                        AutoCreateBatch(salesOrder);
                        batchedSalesOrders.Add(salesOrder.SalesOrderID);
                    }

                    // Create batches for Sales Orders that have no batches but have
                    // new Work Orders.
                    foreach (var workOrder in addedOrders.OfType<OrdersDataSet.OrderRow>())
                    {
                        var salesOrder = workOrder.SalesOrderRow;

                        if (salesOrder == null || batchedSalesOrders.Contains(salesOrder.SalesOrderID))
                        {
                            continue;
                        }

                        // Create new batches
                        AutoCreateBatch(salesOrder);
                        batchedSalesOrders.Add(salesOrder.SalesOrderID);
                    }

                    // Reset batches for orders that have not been processed but have different parts

                    foreach (var workOrder in modifiedOrders.Keys.OfType<OrdersDataSet.OrderRow>())
                    {
                        var salesOrder = workOrder.SalesOrderRow;
                        var originalPartId = workOrder[dsOrders.Order.PartIDColumn, DataRowVersion.Original] as int?;
                        var newPartId = workOrder.IsPartIDNull() ? (int?)null : workOrder.PartID;

                        var skipBatchUpdate = originalPartId == newPartId
                            || salesOrder == null
                            || batchedSalesOrders.Contains(salesOrder.SalesOrderID);

                        if (skipBatchUpdate)
                        {
                            continue;
                        }

                        // Create new batches
                        AutoCreateBatch(salesOrder);
                        batchedSalesOrders.Add(salesOrder.SalesOrderID);
                    }

                    foreach (var salesOrderId in batchedSalesOrders)
                    {
                        var salesOrderNode = tvwTOC.Nodes[0]
                            .FindNode<SalesOrderNode>(node => node.DataRow.IsValidState() && node.DataRow.SalesOrderID == salesOrderId);

                        if (salesOrderNode == null)
                        {
                            continue;
                        }

                        foreach (var orderNode in salesOrderNode.FindNodes<OrderNode>())
                        {
                            orderNode.UpdateBatchNodes(this);
                        }
                    }
                }

                var addedBatches = DataUtilities.GetRowsByRowState(dsOrders.Batch, DataRowState.Added);

                var deletedBatches = DataUtilities.GetRowsAndValuesByRowState(dsOrders.Batch,
                    DataRowState.Deleted);
                var deletedBatchOrders = DataUtilities.GetRowsAndValuesByRowState(dsOrders.BatchOrder,
                    DataRowState.Deleted);

                this.dsOrders.EnforceConstraints = false;

                var updatedRecords = UpdateDatabase();

                if (!updatedRecords.HasValue)
                {
                    _log.Info("User declined to merge changes.");
                    MessageBoxUtilities.ShowMessageBoxOK("Data was not saved.", "Order Entry");
                    return false;
                }

                _log.Info("Order records updated: " + updatedRecords);

                foreach (var addedOrderProcess in addedOrderProcesses.OfType<OrdersDataSet.OrderProcessesRow>())
                    OrderHistoryDataSet.UpdateOrderHistory(addedOrderProcess.OrderID, this.ModeDisplayName, "Order process {0} added.".FormatWith(addedOrderProcess.ProcessAliasSummaryRow == null ? addedOrderProcess.ProcessAliasID.ToString() : addedOrderProcess.ProcessAliasSummaryRow.ProcessName), SecurityManager.Current.UserName);

                foreach(var addedOrderReview in addedOrderReviews.OfType<OrdersDataSet.OrderReviewRow>())
                    OrderHistoryDataSet.UpdateOrderHistory(addedOrderReview.OrderID, this.ModeDisplayName, "Order review status set to " + (addedOrderReview.Status ? "Pass" : "Fail"), SecurityManager.Current.UserName);

                //For all new sales orders
                foreach (DataRow addedSalesOrder in addedSalesOrders)
                {
                    var or = addedSalesOrder as OrdersDataSet.SalesOrderRow;

                    //if added successfully
                    if (or != null && or.SalesOrderID > 0)
                    {
                        OrderHistoryDataSet.UpdateOrderHistory(or.SalesOrderID, this.ModeDisplayName, "Sales Order created.", SecurityManager.Current.UserName);
                    }
                }

                //for all new orders
                foreach(DataRow addedOrder in addedOrders)
                {
                    var or = addedOrder as OrdersDataSet.OrderRow;

                    //if added successfully
                    if(or != null && or.OrderID > 0)
                    {
                        // if has a receiving order then close it
                        if(!or.IsReceivingIDNull())
                        {
                            if(taReceiving == null)
                                taReceiving = new ReceivingTableAdapter();

                            taReceiving.CloseReceivingOrder(or.OrderID, or.ReceivingID);
                        }

                        //TODO:
                        //IF QUOTE IMPORT

                        OrderHistoryDataSet.UpdateOrderHistory(or.OrderID, this.ModeDisplayName, "Order created.", SecurityManager.Current.UserName);

                        // Consider new, saved orders as loaded.
                        MarkOrderAsLoaded(or.OrderID);
                    }
                }

                //NOTE changes
                foreach(var mo in modifiedOrders)
                {
                    var timersStopped = false;
                    var newValues = mo.Key.ItemArray;

                    for (int i = 0; i < newValues.Length; i++)
                    {
                        object newValue = newValues[i];
                        object originalValue = mo.Value[i];

                        if (!Equals(newValue, originalValue))
                        {
                            var tableColumnName = mo.Key.Table.Columns[i].ColumnName;
                            int orderId = (int)mo.Key["OrderID"];

                            var updateMsg = "{0} value changed from {1} to {2}".FormatWith(
                                tableColumnName,
                                originalValue ?? "NULL",
                                newValue ?? "NULL");

                            OrderHistoryDataSet.UpdateOrderHistory(orderId,
                                this.ModeDisplayName,
                                updateMsg,
                                SecurityManager.Current.UserName);


                            if (!timersStopped && (tableColumnName == dsOrders.Order.WorkStatusColumn.ColumnName || tableColumnName == dsOrders.Order.StatusColumn.ColumnName))
                            {
                                timersStopped = true;

                                // Close any running timers
                                TimeCollectionUtilities.StopAllOrderTimers(orderId);

                                // Don't sync any timers as processing is unfinished when
                                // rejoining orders that are in-process.
                            }

                            if (tableColumnName == dsOrders.Order.AdjustedEstShipDateColumn.ColumnName)
                            {
                                var orgDate = originalValue as DateTime? ?? DateTime.MinValue;
                                var newDate = newValue as DateTime? ?? DateTime.MinValue;

                                var estShipDate = mo.Key[dsOrders.Order.EstShipDateColumn] as DateTime?;

                                if (newDate > orgDate && newDate > estShipDate)
                                {
                                    // Queue late order notifications if:
                                    // - The operator enters a new Adjusted Est. Ship Date, or
                                    // - The operator updates the Adjusted Est. Ship Date to be
                                    //   later than its previous value.
                                    var customerId = mo.Key.IsNull(dsOrders.Order.CustomerIDColumn)
                                        ? -1
                                        : Convert.ToInt32(mo.Key[dsOrders.Order.CustomerIDColumn]);

                                    var notificationContactIds = LateOrderUtilities
                                        .GetContactIdsForNotification(customerId);

                                    foreach (var contactId in notificationContactIds)
                                    {
                                        taLateOrderNotification.Insert(orderId, contactId, null,
                                            SecurityManager.Current.UserID);
                                    }
                                }
                            }
                        }
                    }
                }

                //NOTE changes
                foreach (var mo in modifiedProcesses)
                {
                    var newValues = mo.Key.ItemArray;

                    for (int i = 0; i < newValues.Length; i++)
                    {
                        if (!Object.Equals(newValues[i], mo.Value[i]))
                        {
                            string updateMsg = "For process {0}; {1} value changed from {2} to {3}".FormatWith(
                                mo.Value[dsOrders.OrderProcesses.ProcessIDColumn.Ordinal],
                                mo.Key.Table.Columns[i].ColumnName,
                                mo.Value[i] ?? "NULL",
                                newValues[i] ?? "NULL");

                            OrderHistoryDataSet.UpdateOrderHistory((int)mo.Key["OrderID"],
                                this.ModeDisplayName,
                                updateMsg,
                                SecurityManager.Current.UserName);
                        }
                    }
                }

                //NOTE deleted orders
                foreach(var mo in deletedOrders)
                    OrderHistoryDataSet.UpdateOrderHistory((int)mo.Value[0], this.ModeDisplayName, String.Format("Order {0} deleted.", mo.Value[0]), SecurityManager.Current.UserName);

                //NOTE deleted sales orders
                foreach (var mo in deletedSalesOrders)
                    OrderHistoryDataSet.UpdateOrderHistory((int)mo.Value[0], this.ModeDisplayName, String.Format("Sales Order {0} deleted.", mo.Value[0]), SecurityManager.Current.UserName);

                // NOTE deleted batches
                using (var taEventLog = new Data.Datasets.UserLoggingTableAdapters.UserEventLogTableAdapter())
                {
                    foreach (var deletedBatchData in deletedBatches)
                    {
                        var batchIdIndex = dsOrders.Batch.Columns.IndexOf(dsOrders.Batch.BatchIDColumn);
                        var batchId = deletedBatchData.Value[batchIdIndex] as int?;

                        taEventLog.Insert(SecurityManager.Current.UserID,
                            "Delete",
                            ModeDisplayName,
                            "Workflow",
                            $"Batch: {batchId}");
                    }
                }

                foreach (var deletedBatchOrderData in deletedBatchOrders)
                {
                    var orderIdIndex = dsOrders.BatchOrder.Columns.IndexOf(dsOrders.BatchOrder.OrderIDColumn);
                    var orderId = deletedBatchOrderData.Value[orderIdIndex] as int?;

                    var batchIdIndex = dsOrders.BatchOrder.Columns.IndexOf(dsOrders.BatchOrder.BatchIDColumn);
                    var batchId = deletedBatchOrderData.Value[batchIdIndex] as int?;

                    if (orderId.HasValue && batchId.HasValue)
                    {
                        OrderHistoryDataSet.UpdateOrderHistory(
                            orderId.Value,
                            ModeDisplayName,
                            $"Order {orderId} removed from batch {batchId}.",
                            SecurityManager.Current.UserName);
                    }
                }

                // NOTE added batches
                foreach (var newBatch in addedBatches.OfType<OrdersDataSet.BatchRow>())
                {
                    var ordersInBatch = newBatch.GetBatchOrderRows()
                        .Select(batchOrder => batchOrder.OrderRow);

                    foreach (var workOrder in ordersInBatch)
                    {
                        OrderHistoryDataSet.UpdateOrderHistory(
                            workOrder.OrderID,
                            ModeDisplayName,
                            $"Order {workOrder.OrderID} added to batch {newBatch.BatchID}.",
                            SecurityManager.Current.UserName);
                    }
                }

                // NOTE new requests for order approval
                foreach (var newApproval in addedOrderApprovals.OfType<OrdersDataSet.OrderApprovalRow>())
                {
                    OrderHistoryDataSet.UpdateOrderHistory(
                        newApproval.OrderID,
                        ModeDisplayName,
                        $"Approval {newApproval.OrderApprovalID} created for Order.",
                        SecurityManager.Current.UserName);
                }

                // NOTE deleted requests for order approval
                foreach (var deletedApprovalPair in deletedOrderApprovals)
                {
                    var orderId = deletedApprovalPair.Value[dsOrders.OrderApproval.OrderIDColumn.Ordinal] as int?
                        ?? -1;

                    var approvalId = deletedApprovalPair.Value[dsOrders.OrderApproval.OrderApprovalIDColumn.Ordinal] as int?
                        ?? -1;

                    OrderHistoryDataSet.UpdateOrderHistory(
                        orderId,
                        ModeDisplayName,
                        $"Deleted Approval {approvalId}.",
                        SecurityManager.Current.UserName);
                }

                //delete the PO media on local file system that were uploaded
                if (UserSettings.Default.CleanupMediaAfterUpload)
                {
                    this.dpOrder.DeleteUploadedMediaFiles();
                }

                if (!appSettings.OrderCheckInEnabled)
                {
                    var batchesToAutoCheckIn = new List<OrdersDataSet.BatchRow>();

                    foreach (var orderId in _successfullyReviewedOrders.Distinct())
                    {
                        var orderRow = dsOrders.Order.FindByOrderID(orderId);

                        var shouldAutoCheckInBatch = appSettings.AutomaticallyBatchSalesOrder
                            && !orderRow.IsSalesOrderIDNull();

                        if (shouldAutoCheckInBatch)
                        {
                            // Reviewed order is either in a batch or will be in one
                            // after all Work Orders in the Sales Order are reviewed
                            batchesToAutoCheckIn.AddRange(orderRow.GetBatchOrderRows()
                                .Select(batchOrder => batchOrder.BatchRow)
                                .Where(batch => batch.WorkStatus == appSettings.WorkStatusChangingDepartment));
                        }
                        else
                        {
                            // Auto check-in reviewed order
                            var orderCheckIn = new OrderCheckInController(orderId);
                            var checkInResult = orderCheckIn.AutoCheckIn(SecurityManager.Current.UserID);

                            if (!checkInResult.Response)
                            {
                                _log.Warn($"Auto check-in failed for order {orderId}.");
                            }
                        }
                    }

                    // Auto check-in batches
                    foreach (var batchId in batchesToAutoCheckIn.Select(b => b.BatchID).Distinct())
                    {
                        var batchCheckIn = new BatchCheckInController(batchId);
                        var checkInResult = batchCheckIn.AutoCheckIn(SecurityManager.Current.UserID);

                        if (!checkInResult.Response)
                        {
                            _log.Warn($"Auto check-in failed for batch {batchId}.");
                        }
                    }
                }

                _successfullyReviewedOrders.Clear();

                return true;
            }
            catch(Exception exc)
            {
                _log.Warn("DataSet Errors: " + dsOrders.GetDataErrors());
                ErrorMessageBox.ShowDialog("Error saving data.", exc);

                // If any errors occur while OrderEntry is saving data, they
                // are likely major issues that cannot be corrected.
                MessageBoxUtilities.ShowMessageBoxError($"There was an error saving your data. Please close {ModeDisplayName} and try again.",
                    ModeDisplayName);
                return false;
            }
            finally
            {
                this._inSavingData = false;
                
                if(taReceiving != null)
                    taReceiving.Dispose();

                taReceiving = null;

                this.UpdateTotalCounts();
            }
        }

        private void AutoCreateBatch(OrdersDataSet.SalesOrderRow salesOrder)
        {
            // Assumption: All data for existing batches is loaded.
            // AutoCreateBatch is for a sales order, and clicking a Sales Order
            // should load all Work Order data, including batch data.
            try
            {
                var appSettings = ApplicationSettings.Current;

                // Group orders together by process
                var orderIdProcessesDict = new Dictionary<int, List<int>>();

                foreach (var wo in salesOrder.GetOrderRows())
                {
                    if (wo.GetOrderProcessesRows().Length == 0)
                    {
                        // Processes are not loaded - load them
                        taOrderProcesses.FillBy(dsOrders.OrderProcesses, wo.OrderID);
                    }

                    if (wo.GetOrderProcessesRows().Length == 0)
                    {
                        // Invalid Work Order with no processes
                        continue;
                    }

                    if (appSettings.BatchMultipleProcesses)
                    {
                        orderIdProcessesDict[wo.OrderID] = wo.GetOrderProcessesRows()
                            .OrderBy(op => op.StepOrder)
                            .Select(op => op.ProcessID)
                            .ToList();
                    }
                    else
                    {
                        orderIdProcessesDict[wo.OrderID] = wo.GetOrderProcessesRows()
                            .OrderBy(op => op.StepOrder)
                            .Take(1)
                            .Select(op => op.ProcessID)
                            .ToList();
                    }
                }

                var batches = new List<BatchData>();

                // Determine processes for existing batches
                foreach (var existingBatch in salesOrder.GetBatchRows())
                {
                    if (existingBatch.GetBatchProcessesRows().Length == 0)
                    {
                        // Invalid batch with no processes
                        continue;
                    }

                    var processIds = existingBatch.GetBatchProcessesRows()
                        .OrderBy(bp => bp.StepOrder)
                        .Select(bp => bp.ProcessID)
                        .ToList();

                    batches.Add(new BatchData(existingBatch.BatchID, processIds));
                }

                // Determine orders to be batched together
                foreach (var orderIdAndProcesses in orderIdProcessesDict)
                {
                    var orderId = orderIdAndProcesses.Key;
                    var processes = orderIdAndProcesses.Value;

                    var matchingBatch = batches.FirstOrDefault(b => b.Matches(processes));
                    if (matchingBatch != null)
                    {
                        matchingBatch.OrderIds.Add(orderId);
                    }
                    else
                    {
                        batches.Add(new BatchData(processes, orderId));
                    }
                }

                // Create/update batches
                foreach (var batch in batches)
                {
                    if (batch.BatchId.HasValue)
                    {
                        // Update batch
                        var batchRow = dsOrders.Batch.FindByBatchID(batch.BatchId.Value);
                        var existingBatchOrderRows = batchRow
                            .GetBatchOrderRows()
                            .ToList();

                        // Remove orders that do not belong
                        var batchOrdersToRemove = new List<OrdersDataSet.BatchOrderRow>();
                        foreach (var existingBatchOrderRow in existingBatchOrderRows)
                        {
                            if (batch.OrderIds.Contains(existingBatchOrderRow.OrderID))
                            {
                                continue;
                            }

                            batchOrdersToRemove.Add(existingBatchOrderRow);
                        }

                        foreach (var batchOrder in batchOrdersToRemove)
                        {
                            existingBatchOrderRows.Remove(batchOrder);
                            batchOrder.Delete();
                        }

                        // Add new orders to batch
                        foreach (var orderId in batch.OrderIds)
                        {
                            if (existingBatchOrderRows.Any(batchOrder => batchOrder.OrderID == orderId))
                            {
                                continue;
                            }

                            var orderRow = dsOrders.Order.FindByOrderID(orderId);
                            dsOrders.BatchOrder.AddBatchOrderRow(batchRow,
                                orderRow,
                                orderRow.IsPartQuantityNull() ? 0 : orderRow.PartQuantity);
                        }

                        if (batchRow.GetBatchOrderRows().Length == 0)
                        {
                            // Delete empty batches
                            batchRow.Delete();
                        }

                        _log.Info($"Automatically updated batch {batch.BatchId} for Orders {string.Join(", ", batch.OrderIds)}");
                    }
                    else
                    {
                        // Create batch
                        var firstOrderRow = dsOrders.Order.FindByOrderID(batch.OrderIds.First());
                        var newBatchRow = dsOrders.Batch.NewBatchRow();
                        newBatchRow.Active = true;
                        newBatchRow.OpenDate = DateTime.Now;
                        newBatchRow.WorkStatus = appSettings.WorkStatusChangingDepartment;
                        newBatchRow.CurrentLocation = appSettings.DepartmentSales;
                        newBatchRow.SchedulePriority = 0;
                        newBatchRow.SalesOrderID = salesOrder.SalesOrderID;
                        dsOrders.Batch.AddBatchRow(newBatchRow);

                        foreach (var orderId in batch.OrderIds)
                        {
                            var orderRow = dsOrders.Order.FindByOrderID(orderId);
                            dsOrders.BatchOrder.AddBatchOrderRow(newBatchRow,
                                orderRow,
                                orderRow.IsPartQuantityNull() ? 0 : orderRow.PartQuantity);
                        }

                        var orderProcessesByStep = newBatchRow.GetBatchOrderRows()
                            .SelectMany(batchOrder => batchOrder.OrderRow.GetOrderProcessesRows())
                            .GroupBy(orderProcess => orderProcess.StepOrder)
                            .ToList();

                        foreach (var step in orderProcessesByStep)
                        {
                            var firstOrderProcess = step.First();
                            var stepOrder = step;
                            var newBatchProcessRow = dsOrders.BatchProcesses.NewBatchProcessesRow();
                            newBatchProcessRow.BatchRow = newBatchRow;
                            newBatchProcessRow.ProcessID = firstOrderProcess.ProcessID;
                            newBatchProcessRow.StepOrder = firstOrderProcess.StepOrder;
                            newBatchProcessRow.Department = firstOrderProcess.Department;
                            dsOrders.BatchProcesses.AddBatchProcessesRow(newBatchProcessRow);

                            foreach (var orderProcessRow in step)
                            {
                                dsOrders.BatchProcess_OrderProcess.AddBatchProcess_OrderProcessRow(
                                    newBatchProcessRow,
                                    orderProcessRow);
                            }
                        }

                        _log.Info($"Automatically created batch for Orders {string.Join(", ", batch.OrderIds)}");
                    }
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error creating batch.", exc);
            }
        }

        private int? UpdateDatabase()
        {
            DeleteDuplicateMediaLinks();

            while (true)
            {
                try
                {
                    return taManager.UpdateAll(dsOrders);
                }
                catch (DBConcurrencyException exc)
                {
                    var knownTables = new List<string>
                    {
                        dsOrders.Order.TableName,
                        dsOrders.OrderProcesses.TableName,
                        dsOrders.OrderCustomFields.TableName,
                    };

                    var rowsWithErrors = new DataRow[exc.RowCount];
                    exc.CopyToRows(rowsWithErrors);

                    if (rowsWithErrors.Any(s => !knownTables.Contains(s.Table.TableName)))
                    {
                        // Cannot handle error
                        _log.Warn($"Cannot handle concurrency error outside of {string.Join(",", knownTables)}.");
                        throw;
                    }

                    _log.Warn(exc, "Concurrency error while saving data.");

                    const string promptText = "There are some changes on the " +
                        "server that need to be merged in order to save your " +
                        "changes.\n\nMerge changes?";

                    var prompt = MessageBoxUtilities.ShowMessageBoxYesOrNo(promptText, "Order Entry");

                    if (prompt != DialogResult.Yes)
                    {
                        return null;
                    }

                    var orders = rowsWithErrors.OfType<OrdersDataSet.OrderRow>().ToList();
                    var orderProcesses = rowsWithErrors.OfType<OrdersDataSet.OrderProcessesRow>().ToList();
                    var orderCustomFields = rowsWithErrors.OfType<OrdersDataSet.OrderCustomFieldsRow>().ToList();
                    var orderIds = orders.Select(o => o.OrderID)
                        .Concat(orderProcesses.Select(o => o.OrderID))
                        .Concat(orderCustomFields.Select(field => field.OrderID))
                        .Distinct();

                    MergeOrderUpdates(orders);
                    MergeOrderProcessesUpdates(orderProcesses);
                    MergeOrderCustomFieldUpdates(orderCustomFields);

                    var successText = $"Successfully merged your changes for the following orders:\n{string.Join("\n", orderIds)}";

                    MessageBoxUtilities.ShowMessageBoxOK(successText, "Order Entry");
                }
            }
        }

        /// <summary>
        /// Fixes a customer-specific(?) issue involving duplicate media link
        /// rows.
        /// </summary>
        private void DeleteDuplicateMediaLinks()
        {
            // Order_Media
            var addedOrderMedia = dsOrders.Order_Media
                .Where(om => om.RowState == DataRowState.Added)
                .ToList();

            var checkedOrderMedia = new List<OrdersDataSet.Order_MediaRow>();

            foreach (var media in addedOrderMedia)
            {
                if (checkedOrderMedia.Any(m => m.OrderID == media.OrderID && m.MediaID == media.MediaID))
                {
                    media.Delete();
                }
                else
                {
                    checkedOrderMedia.Add(media);
                }
            }

            // SalesOrder_Media
            var addedSalesOrderMedia = dsOrders.SalesOrder_Media
                .Where(om => om.RowState == DataRowState.Added)
                .ToList();

            var checkedSalesOrderMedia = new List<OrdersDataSet.SalesOrder_MediaRow>();

            foreach (var media in addedSalesOrderMedia)
            {
                if (checkedSalesOrderMedia.Any(m => m.SalesOrderID == media.SalesOrderID && m.MediaID == media.MediaID))
                {
                    media.Delete();
                }
                else
                {
                    checkedSalesOrderMedia.Add(media);
                }
            }
        }

        private void MergeOrderUpdates(ICollection<OrdersDataSet.OrderRow> conflictedRows)
        {
            if (!conflictedRows.Any())
            {
                return;
            }

            using (var dsOrderTemp = new OrdersDataSet.OrderDataTable())
            {
                foreach (var row in conflictedRows)
                {
                    taOrder.FillByOrderID(dsOrderTemp, row.OrderID);
                }

                var mergeLogger = new MergeLogger(dsOrders.Order, dsOrderTemp,
                    dsOrders.Order.OrderIDColumn.ColumnName);

                mergeLogger.LogValues();
                dsOrders.Order.Merge(dsOrderTemp, true, MissingSchemaAction.Error);
            }
        }

        private void MergeOrderProcessesUpdates(ICollection<OrdersDataSet.OrderProcessesRow> conflictedRows)
        {
            if (!conflictedRows.Any())
            {
                return;
            }

            using (var dsOrderProcessesTemp = new OrdersDataSet.OrderProcessesDataTable())
            {
                foreach (var row in conflictedRows)
                {
                    taOrderProcesses.FillByID(dsOrderProcessesTemp, row.OrderProcessesID);
                }

                var mergeLogger = new MergeLogger(dsOrders.OrderProcesses, dsOrderProcessesTemp,
                    dsOrders.OrderProcesses.OrderProcessesIDColumn.ColumnName);

                mergeLogger.LogValues();
                dsOrders.OrderProcesses.Merge(dsOrderProcessesTemp, true, MissingSchemaAction.Error);
            }
        }

        private void MergeOrderCustomFieldUpdates(List<OrdersDataSet.OrderCustomFieldsRow> conflictedRows)
        {
            if (!conflictedRows.Any())
            {
                return;
            }

            using (var dsOrderOrderCustomFieldsTemp = new OrdersDataSet.OrderCustomFieldsDataTable())
            {
                foreach (var orderId in conflictedRows.Select(r => r.OrderID).Distinct())
                {
                    taOrderCustomFields.FillByOrder(dsOrderOrderCustomFieldsTemp, orderId);
                }

                // Manually logging w/o merge logger - OrderCustomFields has no ID column
                _log.Info("Merging OrderCustomFields");
                foreach (var conflictedRow in conflictedRows)
                {
                    var newRow = dsOrderOrderCustomFieldsTemp.FindByOrderIDCustomFieldID(
                        conflictedRow.OrderID,
                        conflictedRow.CustomFieldID);

                    _log.Info($"Client: {conflictedRow.OrderID}, {conflictedRow.CustomFieldID} - " +
                        $"'{(conflictedRow.IsValueNull() ? null : conflictedRow.Value)}'");

                    if (newRow != null)
                    {
                        _log.Info($"Server: {newRow.OrderID}, {newRow.CustomFieldID} - " +
                            $"'{(newRow.IsValueNull() ? null : newRow.Value)}'");
                    }
                    else
                    {
                        _log.Info($"Server: N/A");
                    }
                }

                // Perform merge
                dsOrders.OrderCustomFields.Merge(dsOrderOrderCustomFieldsTemp, true, MissingSchemaAction.Error);
            }
        }

        private void LoadCommands()
        {
            Commands.AddCommand("VideoTutorial", new VideoCommand(toolbarManager.Tools["VideoTutorial"]) { Url = VideoLinks.OrderEntryTutorial });

            //show daily total gauge if in normal mode
            toolbarManager.Ribbon.Tabs[0].Groups["dailyTotalGroup"].Visible = Mode == OrderEntryMode.Normal;

            if(SecurityManager.Current.IsInRole("OrderEntry.Edit"))
            {
                _log.Debug("Loading commands.");

                if(Mode == OrderEntryMode.Normal)
                {
                    //Move Work order
                    var moveWorkOrderTool = toolbarManager.Tools["MoveWorkOrder"];
                    
                    var moveWorkOrder =  Commands.AddCommand("MoveWorkOrder", new MoveWorkOrderCommand(moveWorkOrderTool, tvwTOC)) as MoveWorkOrderCommand;
                    moveWorkOrder.IsValidToAdd = () =>
                    {
                        var isValid = tvwTOC?.ActiveNode is OrderNode;
                        moveWorkOrderTool.CustomizedVisible = isValid ? DefaultableBoolean.True : DefaultableBoolean.False;
                        return isValid;
                    };

                    //Sales order
                    var addSalesOrder = Commands.AddCommand("AddSalesOrder", new SalesOrderCommand(toolbarManager.Tools["AddSalesOrder"], tvwTOC)) as SalesOrderCommand;
                    addSalesOrder.AddNode = this.OnAddSalesOrderNode;
                    addSalesOrder.IsValidToAdd = () => this.StatusFilter.Text == Properties.Settings.Default.OrderStatusOpen;

                    //Work order
                    var add = Commands.AddCommand("AddOrder", new AddOrderCommand(toolbarManager.Tools["AddOrder"], tvwTOC)) as AddOrderCommand;
                    add.AddNode = this.OnAddOrderNode;
                    add.IsValidToAdd = () => this.StatusFilter.Text == Properties.Settings.Default.OrderStatusOpen;

                    Commands.AddCommand("Split", new SplitCommand(toolbarManager.Tools["Split"], tvwTOC, this));
                    Commands.AddCommand("Copy", new CopyPasteCommand(toolbarManager.Tools["Copy"], tvwTOC, "OrderEntry.OrderCopy"));
                }
                else
                {
                    toolbarManager.Tools["AddOrder"].SharedProps.Visible = false;
                    toolbarManager.Tools["Split"].SharedProps.Visible = false;
                    toolbarManager.Tools["Copy"].SharedProps.Visible = false;
                    toolbarManager.Tools["AddSalesOrder"].SharedProps.Visible = false;
                }

                if (Mode == OrderEntryMode.Normal && SecurityManager.Current.IsInRole("OrderRejoin"))
                {
                    Commands.AddCommand("Rejoin", new RejoinCommand(toolbarManager.Tools["Rejoin"], tvwTOC, dpOrder.PartQty, this, "OrderRejoin"));
                }
                else
                {
                    toolbarManager.Tools["Rejoin"].SharedProps.Visible = false;
                }

                //Create Part Marking command if app uses it
                if(ApplicationSettings.Current.PartMarkingEnabled && this.Mode != OrderEntryMode.BlanketPO)
                {
                    var pm = Commands.AddCommand("PartMark", new AddPartMarkCommand(toolbarManager.Tools["PartMark"], tvwTOC)) as AddPartMarkCommand;
                    pm.AddNode = this.OnAddPartMarkNode;
                }
                else
                    toolbarManager.Tools["PartMark"].SharedProps.Visible = false;

                //Create COC command if app uses it
                if (ApplicationSettings.Current.COCEnabled && Mode == OrderEntryMode.Normal)
                {
                    var revCOC = Commands.AddCommand("ReviseCOC", new ReviseCOCCommand(toolbarManager.Tools["ReviseCOC"], tvwTOC)) as ReviseCOCCommand;
                    revCOC.ReviseCoc += this.ReviseCOCNode;
                    revCOC.ReviseBatchCoc += ReviseBatchCocNode;
                }
                else
                    toolbarManager.Tools["ReviseCOC"].SharedProps.Visible = false;

                if(Mode == OrderEntryMode.Normal || Mode == OrderEntryMode.BlanketPO)
                {
                    var dc = Commands.AddCommand("Delete", new DeleteCommand(toolbarManager.Tools["Delete"], tvwTOC, this, "OrderEntry.OrderDelete")) as DeleteCommand;
                    dc.CancelCommand += this.DeleteCommand_CancelCommand;

                    var removeCommand = Commands.AddCommand("RemoveFromSalesOrder", new RemoveFromSalesOrderCommand(toolbarManager.Tools["RemoveFromSalesOrder"], tvwTOC, this))
                        as RemoveFromSalesOrderCommand;

                    removeCommand.RemovedOrdersFromSalesOrder += OnRemovedOrdersFromSalesOrder;
                }
                else
                {
                    toolbarManager.Tools["Delete"].SharedProps.Visible = false;
                    toolbarManager.Tools["RemoveFromSalesOrder"].SharedProps.Visible = false;
                }

                ImportFromQuoteCommand importFromQuote = null;
                if (Mode == OrderEntryMode.Normal)
                {
                    importFromQuote = Commands.AddCommand("ImportFromQuote", new ImportFromQuoteCommand(toolbarManager.Tools["ImportFromQuote"], this.dsOrders, tvwTOC)) as ImportFromQuoteCommand;
                    importFromQuote.IsCurrentNodeValid += IsValidControls;
                    importFromQuote.AddNode += this.OnAddOrderFromQuotePart;
                    importFromQuote.IsValidToAdd = () => this.StatusFilter.Text == Properties.Settings.Default.OrderStatusOpen;
                }

                else
                    toolbarManager.Tools["Import"].SharedProps.Visible = false;

                if (Mode == OrderEntryMode.Normal)
                {
                    var importFromReceiving = Commands.AddCommand("Import", new ImportFromReceivingCommand(toolbarManager.Tools["ImportFromReceiving"], this.dsOrders, tvwTOC, importFromQuote)) as ImportFromReceivingCommand;
                    importFromReceiving.IsCurrentNodeValid += IsValidControls;
                    importFromReceiving.AddNode += this.OnAddOrderFromReceiving;
                    importFromReceiving.IsValidToAdd = () => this.StatusFilter.Text == Properties.Settings.Default.OrderStatusOpen;
                }
                else
                    toolbarManager.Tools["Import"].SharedProps.Visible = false;

                if(Mode == OrderEntryMode.Normal)
                {
                    var text = toolbarManager.Tools["HistoryLength"] as MaskedEditTool;
                    text.SharedProps.Enabled = true;
                    text.Value = Properties.Settings.Default.OrderMaxNumberofDays;
                }
                else
                    toolbarManager.Tools["HistoryLength"].SharedProps.Visible = false;

                if(Mode == OrderEntryMode.Normal)
                {
                    var rwo = Commands.AddCommand("ReworkOrder", new ReworkOrderCommand(toolbarManager.Tools["ReworkOrder"], tvwTOC, this.dpOrder)) as ReworkOrderCommand;
                    rwo.ReworkNodeAdded += () => this.SelectStatusFilterValue(Properties.Settings.Default.OrderStatusOpen);
                }
                else
                    toolbarManager.Tools["ReworkOrder"].SharedProps.Visible = false;

                if (Mode == OrderEntryMode.Normal)
                    Commands.AddCommand("OrderHold", new OrderHoldToggleCommand(toolbarManager.Tools["OrderHold"], tvwTOC, this, "OrderEntry.Hold"));
                else
                    toolbarManager.Tools["OrderHold"].SharedProps.Visible = false;

                if (Mode == OrderEntryMode.Normal)
                    Commands.AddCommand("Found", new FoundOrderCommand(toolbarManager.Tools["Found"], tvwTOC, this));
                else
                    toolbarManager.Tools["Found"].SharedProps.Visible = false;

                Commands.AddCommand("Forward", new GoForwardBackwardCommand(toolbarManager.Tools["Forward"], true, this));
                Commands.AddCommand("Back", new GoForwardBackwardCommand(toolbarManager.Tools["Back"], false, this));
            }

            if(Mode == OrderEntryMode.BlanketPO)
                toolbarManager.Tools["Filter"].SharedProps.Visible = false;
            else
            {
                //ensure filters will work only if user can change node selection
                (toolbarManager.Tools["Filter"]).BeforeToolDropdown += (s, ee) => ee.Cancel = !IsValidControls();
                (toolbarManager.Tools["CustomFilter"]).BeforeToolDropdown += (s, ee) => ee.Cancel = !IsValidControls();

                quickFindOrder.FindOrder += quickFindOrder_FindOrder;
            }

            Commands.AddCommand("History", new OrderHistoryCommand(toolbarManager.Tools["History"], tvwTOC));

            if(Mode == OrderEntryMode.BlanketPO)
                toolbarManager.Tools["AddCommunication"].SharedProps.Visible = false;
            else
            {
                var addComm = Commands.AddCommand("AddCommunication", new AddCommunicationCommand(toolbarManager.Tools["AddCommunication"], tvwTOC)) as AddCommunicationCommand;
                addComm.AddNode = this.OnAddCommunicationNode;
            }

            if(Mode == OrderEntryMode.BlanketPO)
                toolbarManager.Tools["AddNote"].SharedProps.Visible = false;
            else
            {
                var addNote = Commands.AddCommand("AddNote", new AddOrderNoteCommand(toolbarManager.Tools["AddNote"], tvwTOC)) as AddOrderNoteCommand;
                addNote.AddNode = this.OnAddOrderNoteNode;
            }

            var printCommand = Commands.AddCommand("Print", new PrintNodeCommand(toolbarManager.Tools["Print"], tvwTOC)) as PrintNodeCommand;
            printCommand.BeforePrinted += printCommand_BeforePrinted;
            printCommand.AfterPrinted += printCommand_AfterPrinted;

            if (Mode == OrderEntryMode.Normal)
                Commands.AddCommand("Search", new SearchCommand(toolbarManager.Tools["AdvancedSearch"], tvwTOC, this));
            else
                toolbarManager.Tools["AdvancedSearch"].SharedProps.Visible = false;

            if (Mode == OrderEntryMode.Review || Mode == OrderEntryMode.ImportExportReview)
                Commands.AddCommand("OrderReview", new AddOrderReviewCommand(toolbarManager.Tools["OrderReview"], tvwTOC, this));
            else
                toolbarManager.Tools["OrderReview"].SharedProps.Visible = false;

            if (Mode == OrderEntryMode.BlanketPO)
            {
                var add = Commands.AddCommand("NewBlanketPO", new AddBlanketPOCommand(toolbarManager.Tools["NewBlanketPO"], tvwTOC)) as AddBlanketPOCommand;
                add.AddNode = this.OnAddBlanketPONode;
                add.IsValidToAdd = () => this.StatusFilter.Text == Properties.Settings.Default.OrderStatusOpen;
            }
            else
                toolbarManager.Tools["NewBlanketPO"].SharedProps.Visible = false;

            if (Mode == OrderEntryMode.BlanketPO)
            {
                var add = Commands.AddCommand("IssueNewOrder", new AddOrderCommand(toolbarManager.Tools["IssueNewOrder"], tvwTOC)) as AddOrderCommand;
                add.AddNode = this.OnAddOrderNode;
                add.IsValidToAdd = () => this.StatusFilter.Text == Properties.Settings.Default.OrderStatusOpen && tvwTOC.SelectedNode<BlanketPONode>() != null;
            }
            else
                toolbarManager.Tools["IssueNewOrder"].SharedProps.Visible = false;

            //Create Blanket PO From Quote
            if (Mode == OrderEntryMode.BlanketPO)
            {
                var add = Commands.AddCommand("CreateFromQuote", new AddOrderCommand(toolbarManager.Tools["CreateFromQuote"], tvwTOC)) as AddOrderCommand;
                add.AddNode = this.OnAddBlanketPOFromQuote;
                add.IsValidToAdd = () => this.StatusFilter.Text == Properties.Settings.Default.OrderStatusOpen;
            }
            else
                toolbarManager.Tools["CreateFromQuote"].SharedProps.Visible = false;

            //Add Container
            if ((Mode == OrderEntryMode.BlanketPO || Mode == OrderEntryMode.Normal) && SecurityManager.Current.IsInRole("AddContainers"))
            {
                var add = Commands.AddCommand("AddContainer", new AddOrderContainerCommand(toolbarManager.Tools["AddContainer"], tvwTOC, this)) as AddOrderContainerCommand;
                add.AddNode = this.OnAddContainer;
            }
            else
                toolbarManager.Tools["AddContainer"].SharedProps.Visible = false;

            // Request approval
            if ((Mode == OrderEntryMode.Normal || Mode == OrderEntryMode.Review || Mode == OrderEntryMode.ImportExportReview)
                && ApplicationSettings.Current.OrderApprovalEnabled
                && SecurityManager.Current.IsInRole("OrderEntry.OrderApproval"))
            {
                Commands.AddCommand("RequestApproval", new RequestOrderApprovalCommand(toolbarManager.Tools["RequestApproval"], tvwTOC, this));
            }
            else
            {
                toolbarManager.Tools["RequestApproval"].SharedProps.Visible = false;
            }

            Commands.AddCommand("WIPFilter", new WIPFilterCommand(toolbarManager.Tools["WIPFilter"], tvwTOC, this));

            if (Mode == OrderEntryMode.Review || Mode == OrderEntryMode.ImportExportReview)
            {
                Commands.AddCommand("OrdersToFix", new OrderToFixFilterCommand(toolbarManager.Tools["OrdersToFix"], tvwTOC, this));
                Commands.AddCommand("OrdersToReview", new OrderToReviewFilterCommand(toolbarManager.Tools["OrdersToReview"], tvwTOC, this));
            }
            else
            {
                toolbarManager.Tools["OrdersToFix"].SharedProps.Visible = false;
                toolbarManager.Tools["OrdersToReview"].SharedProps.Visible = false;
            }

            toolbarManager.Ribbon.Tabs[0].Groups["BlanketPO"].Visible = Mode == OrderEntryMode.BlanketPO;
        }

        /// <summary>
        ///   Add a new order to the data store and the TOC.
        /// </summary>
        /// <param name="pn"> The root node. </param>
        /// <returns> </returns>
        private void AddOrder(OrderRootNode pn)
        {
            _log.Info("Adding a new order.");

            //create new data source
            OrdersDataSet.OrderRow cr = this.dpOrder.AddOrderRow();

            //create new ui nodes
            var cn = new OrderNode(cr, this.Mode);
            pn.Nodes.Add(cn);
            cn.Select();
        }

        /// <summary>
        /// Adds the order to the data store and the sales order node in the TOC.
        /// </summary>
        /// <param name="salesOrderNode">The salesOrderNode.</param>
        private void AddOrder(SalesOrderNode salesOrderNode)
        {
            _log.Info("Adding a new work order to sales order {0}.", salesOrderNode.ID);

            //create new data source
            var salesOrder = salesOrderNode.DataRow;
            OrdersDataSet.OrderRow orderRow = this.dpOrder.AddOrderRow(salesOrder.CustomerID);

            orderRow.SalesOrderID = salesOrder.SalesOrderID;
            orderRow.OrderDate = salesOrder.OrderDate;
            orderRow.CustomerID = salesOrder.CustomerID;
            orderRow.CustomerWO = salesOrder.IsCustomerWONull() ? string.Empty : salesOrder.CustomerWO;
            orderRow.PurchaseOrder = salesOrder.IsPurchaseOrderNull() ? string.Empty : salesOrder.PurchaseOrder;

            if (!salesOrder.IsEstShipDateNull())
            {
                orderRow.EstShipDate = salesOrder.EstShipDate;
            }

            if (!salesOrder.IsRequiredDateNull())
            {
                orderRow.RequiredDate = salesOrder.RequiredDate;
            }
            
            //Add the sales media to order media
            var mediaJunctionRows = salesOrder.GetSalesOrder_MediaRows();
            mediaJunctionRows.ForEach(mjr => this.dsOrders.Order_Media.Rows.Add(orderRow.OrderID, mjr.MediaID));

            //create new ui nodes
            var cn = new OrderNode(orderRow, this.Mode);
            salesOrderNode.Nodes.Add(cn);
            cn.Select();
        }

        private OrdersDataSet.OrderRow AddOrder(BlanketPONode blanketPONode, int partQuantity, Dictionary<int, string> customFields)
        {
            if(partQuantity < 1)
                return null;

            _log.Info("Adding a new work order based on blanket PO {0}.", blanketPONode.ID);

            //create new order based on the blanket PO info
            var orderRow = this.dpOrder.AddOrderRow();
            orderRow.OrderDate = DateTime.Now; // Blanket PO may have contract date; use today's date instead
            orderRow.OrderTemplateID = blanketPONode.DataRow.OrderTemplateID;
            orderRow.CustomerID = blanketPONode.DataRow.CustomerID;
            orderRow.PartID = blanketPONode.DataRow.PartID;
            orderRow.PartQuantity = partQuantity;
            orderRow.ContractReviewed = true;

            if(!blanketPONode.DataRow.IsPurchaseOrderNull())
                orderRow.PurchaseOrder = blanketPONode.DataRow.PurchaseOrder;
            if (!blanketPONode.DataRow.IsPriorityNull())
                orderRow.Priority = blanketPONode.DataRow.Priority;
            if (!blanketPONode.DataRow.IsShippingMethodNull())
                orderRow.ShippingMethod = blanketPONode.DataRow.ShippingMethod;
            if (!blanketPONode.DataRow.IsCustomerAddressIDNull())
                orderRow.CustomerAddressID = blanketPONode.DataRow.CustomerAddressID;
            if (!blanketPONode.DataRow.IsPriceUnitNull())
                orderRow.PriceUnit = blanketPONode.DataRow.PriceUnit;
            if (!blanketPONode.DataRow.IsBasePriceNull())
                orderRow.BasePrice = blanketPONode.DataRow.BasePrice;
            if (!blanketPONode.DataRow.IsWeightNull())
            {
                // Due to storage limits, BlanketPO weight could be different than the actual weight.
                var part = this.dsOrders.PartSummary.FindByPartID(blanketPONode.DataRow.PartID);
                if (part != null && !part.IsWeightNull())
                {
                    orderRow.Weight = Math.Min(part.Weight * Convert.ToDecimal(partQuantity), MAX_WEIGHT);
                }
            }

            //Add the custom fields and values for the customer
            DWOS.Data.Datasets.OrdersDataSetTableAdapters.CustomFieldTableAdapter taCustomField = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.CustomFieldTableAdapter();

            //Get the custom fields for customer
            taCustomField.FillByCustomer(this.dsOrders.CustomField, orderRow.CustomerID);
            var customer = this.dsOrders.CustomerSummary.FindByCustomerID(orderRow.CustomerID);

            foreach (var customFieldRow in customer?.GetCustomFieldRows() ?? Enumerable.Empty<OrdersDataSet.CustomFieldRow>())
            {
                //Add the custom field to the order
                string value = string.Empty;
                if (customFields.ContainsKey(customFieldRow.CustomFieldID))
                {
                    value = customFields[customFieldRow.CustomFieldID];
                }

                this.dsOrders.OrderCustomFields.AddOrderCustomFieldsRow(orderRow, customFieldRow, value);
            }

            orderRow.EndEdit();

            //force add children data since the part was pre-selected
            dpOrder.AddPartProcesses(orderRow);
            dpOrder.AddOrderPartMark(orderRow);          

            //create new ui nodes
            var cn = new OrderNode(orderRow, this.Mode);
            blanketPONode.Nodes.Add(cn);
            cn.Select();

            return orderRow;
        }
        
        private void AddOrder(int receivingID, int? salesOrderID, bool isRework)
        {
            _log.Info("Adding a new order from receiving order: " + receivingID);

            using(var ta = new ReceivingTableAdapter())
            {
                var dt = ta.GetByID(receivingID);

                if(dt != null && dt.Count == 1)
                {
                    //attempt to load parts before the order is added to ensure part is located
                    this.LoadParts(dt[0].CustomerID);
                }
            }

            //create new data source
            var cr = this.dpOrder.AddOrderRowFromReceivingOrder(receivingID);
            if (cr != null)
            {
                // If needed, load the customer
                if (!cr.IsCustomerIDNull() && dsOrders.CustomerSummary.FindByCustomerID(cr.CustomerID) == null)
                {
                    taCustomerSummary.FillByCustomerID(dsOrders.CustomerSummary, cr.CustomerID);
                }

                // Set remaining properties of the new row
                cr.OrderType = isRework ? (int)OrderType.ReworkExt : (int)OrderType.Normal;
                cr.OriginalOrderType = cr.OrderType;

                //create new ui nodes
                var cn = new OrderNode(cr, this.Mode);
                
                //Add to sales order node otherwise just add to the tree
                if (salesOrderID.GetValueOrDefault(0) > 0)
                {
                    var salesNode = tvwTOC.Nodes.FindNode<SalesOrderNode>(so => so.DataRow.SalesOrderID == salesOrderID);
                    if (salesNode != null)
                    {
                        cr.SalesOrderID = salesOrderID.GetValueOrDefault();
                        salesNode.Nodes.Add(cn);
                    }
                }
                else
                {
                    tvwTOC.Nodes[0].Nodes.Add(cn);
                }
                
                cn.Select();
            }
        }

        private void AddOrderFromQuotePart(int quotePartId, int? salesOrderId, int? receivingId, int? receivingQty)
        {
            _log.Info("Adding a new order from quote part: " + quotePartId);

            var orderRow = this.dpOrder.AddOrderRowFromQuotePart(quotePartId, receivingId, receivingQty);
           
            if (orderRow != null)
            {
                using (var ta = new ReceivingTableAdapter())
                {
                    var dt = ta.GetByID(receivingId.Value);

                    if (dt != null && dt.Count == 1)
                    {
                        //attempt to load parts before the order is added to ensure part is located
                        this.LoadParts(dt[0].CustomerID);
                    }
                }

                if (receivingId.HasValue)
                        orderRow.ReceivingID = receivingId.Value;

                // If needed, load the customer
                if (!orderRow.IsCustomerIDNull() && dsOrders.CustomerSummary.FindByCustomerID(orderRow.CustomerID) == null)
                {
                    taCustomerSummary.FillByCustomerID(dsOrders.CustomerSummary, orderRow.CustomerID);
                }

                // Set remaining properties of the new row
                orderRow.OrderType = (int)OrderType.Normal;
                orderRow.OriginalOrderType = orderRow.OrderType;

                //create new ui nodes
                var orderNode = new OrderNode(orderRow, this.Mode);

                //Add to sales order node if part of a sales order
                if (salesOrderId != null)
                {
                    var salesNode = tvwTOC.Nodes.FindNode<SalesOrderNode>(so => so.DataRow.SalesOrderID == salesOrderId);
                    if (salesNode != null)
                    {
                        orderRow.SalesOrderID = salesOrderId.Value;
                        salesNode.Nodes.Add(orderNode);
                    }
                    else
                        _log.Error($"An error occurred locating the sales order node with id: {salesOrderId}");
                }
                else //Just add to the tree
                    tvwTOC.Nodes[0].Nodes.Add(orderNode);

                orderNode.Select();
            }
            else
                MessageBoxUtilities.ShowMessageBoxError("System is unable to import part from quote at this time", "Import Quote Part");
        }

        private void AddSalesOrder(OrderRootNode pn)
        {
            _log.Info("Adding a new sales order.");

            //create new data source
            this.dpSalesOrder.CurrentCustomerID = 0;
            OrdersDataSet.SalesOrderRow salesRow = this.dpSalesOrder.AddSalesOrderRow();

            //create new ui nodes
            var node = new SalesOrderNode(salesRow, this.dsOrders);
            pn.Nodes.Add(node);
            node.Select();
        }


        /// <summary>
        ///   Adds the communication.
        /// </summary>
        /// <param name="pn"> The order node. </param>
        /// <returns> </returns>
        private void AddCommunication(OrderNode pn)
        {
            _log.Info("Adding a new communication to " + pn.Text);

            OrdersDataSet.CustomerCommunicationRow cr = this.dpCustomerComm.AddCustomerCommunicationRow(pn.DataRow.OrderID, SecurityManager.Current.UserID);

            //create new ui nodes
            var cn = new CustomerCommunicationNode(cr);
            pn.Nodes.Add(cn);
            cn.Select();
        }

        private void AddOrderNote(OrderNode pn)
        {
            _log.Info("Adding a new note to " + pn.Text);

            var cr = this.dpOrderNoteInfo.AddNote(pn.DataRow.OrderID);

            //create new ui nodes
            var cn = new OrderNoteNode(cr);
            pn.Nodes.Add(cn);
            cn.Select();
        }

        private void AddBlanketPONode()
        {
            _log.Info("Adding a new blanket order.");

            //create new data source
            var row = this.dpBlanketPO.AddOrderRow();

            //create new ui nodes
            var node = new BlanketPONode(row);
            tvwTOC.Nodes[0].Nodes.Add(node);
            node.Select();
        }

        private void AddOrderContainer(OrderNode orderNode, int partQuantity, int containerQty)
        {
            if (containerQty < 1 || partQuantity < 1)
                return;

            _log.Info("Adding a new container to " + orderNode.Text);

            this.dpOrderContainer.AddContainers(orderNode.DataRow.OrderID, containerQty, partQuantity / containerQty);

            //create new ui nodes
            var cn = new ContainerNode(orderNode.DataRow);
            orderNode.Nodes.Add(cn);
            cn.Select();
        }

        private void AddPartMarkTemplate(OrderNode pn)
        {
            _log.Info("Adding a new part mark template to " + pn.Text);

            OrdersDataSet.OrderPartMarkRow cr = this.dpOrderPartMarking.Add(pn.DataRow);

            //create new ui nodes
            var cn = new PartMarkNode(cr);
            pn.Nodes.Add(cn);
            cn.Select();
        }

        protected override void LoadNode(UltraTreeNode node)
        {
            try
            {
                if(node != null)
                    _log.Info("Loading node: " + node.Text);

                var dataNode = node as IDataNode;
                //if node's data source is deleted
                if(dataNode?.DataRow?.RowState == DataRowState.Deleted)
                {
                    if(node.Parent != null)
                        node.Parent.Select();
                    
                    node.Remove();
                    DisplayPanel(null);
                    return;
                }

                using(new UsingWaitCursor(this))
                {
                    
                    if (node is OrderNode orderNode)
                    {
                        //load data dynamically
                        LoadParts(orderNode.DataRow.CustomerID);
                        orderNode.LoadChildrenNodes(this);

                        //only editable if order is NOT closed and No answers have been created for the order
                        dpOrder.Editable = (orderNode.DataRow.IsStatusNull() || orderNode.DataRow.Status != "Closed") && this.taOrder.GetAnswerCount(orderNode.DataRow.OrderID).GetValueOrDefault(0) == 0;

                        GoForwardBackwardCommand.AddOrderToStack(orderNode.DataRow.OrderID);

                        //move to the record now
                        DisplayPanel(dpOrder); //have to display panel first because MediaWidget will not allow selection of nodes if the tvw/panel is not enabled

                        //WO in a sales order are not allowed to change customer
                        dpOrder.DisableCustomerSelection = node.Parent is SalesOrderNode || node.Parent is BlanketPONode;
                        dpOrder.MoveToRecord(orderNode.ID);
                        
                    }
                    else if (node is CustomerCommunicationNode commNode)
                    {
                        var customerId = ((OrderNode)node.Parent).DataRow.CustomerID;
                        var validCustomerIds = new List<int> { customerId };

                        dpCustomerComm.ValidCustomerIds = validCustomerIds;
                        dpCustomerComm.MoveToRecord(commNode.ID);
                        DisplayPanel(dpCustomerComm);
                    }
                    else if (node is COCNode cocNode)
                    {
                        dpCOC.MoveToRecord(cocNode.ID);
                        DisplayPanel(dpCOC);
                    }
                    else if (node is ShipmentNode shipmentNode)
                    {
                        dpShipping.MoveToRecord(shipmentNode.ID);
                        DisplayPanel(dpShipping);
                    }
                    else if (node is OrderReviewNode orderReviewNode)
                    {
                        dpOrderReview.MoveToRecord(orderReviewNode.ID);
                        DisplayPanel(dpOrderReview);
                    }
                    else if (node is BlanketPONode blanketPONode)
                    {
                        //load data dynamically
                        LoadParts(blanketPONode.DataRow.CustomerID);
                        blanketPONode.LoadChildrenNodes(this);

                        dpBlanketPO.Editable = blanketPONode.DataRow.GetOrderRows().Length < 1 &&
                            SecurityManager.Current.IsInRole("BlanketPOManager.Edit");

                        dpBlanketPO.MoveToRecord(blanketPONode.ID);
                        DisplayPanel(dpBlanketPO);
                    }
                    else if (node is OrderProcessingNode orderProcessingNode)
                    {
                        dpOrderProcessing.MoveToRecord(orderProcessingNode.ID);
                        DisplayPanel(dpOrderProcessing);
                    }
                    else if (node is PartMarkNode partMarkNode)
                    {
                        dpOrderPartMarking.MoveToRecord(partMarkNode.ID);
                        DisplayPanel(dpOrderPartMarking);
                    }
                    else if (node is OrderNoteNode orderNoteNode)
                    {
                        dpOrderNoteInfo.MoveToRecord(orderNoteNode.ID);
                        DisplayPanel(dpOrderNoteInfo);
                    }
                    else if (node is ContainerNode containerNode)
                    {
                        dpOrderContainer.MoveToRecord(containerNode.OrderRow.OrderID);
                        DisplayPanel(dpOrderContainer);
                    }
                    else if (node is OrderInternalReworkNode internalReworkNode)
                    {
                        dpInternalRework.MoveToRecord(internalReworkNode.ID);
                        DisplayPanel(dpInternalRework);
                    }
                    else if (node is OrderChangeNode orderChangeNode)
                    {
                        dpOrderChangeInfo.MoveToRecord(orderChangeNode.ID);
                        DisplayPanel(dpOrderChangeInfo);
                    }
                    else if (node is SalesOrderNode salesOrderNode)
                    {
                        //Add the orders for this sales node
                        salesOrderNode.LoadChildrenNodes(this);

                        //Move to the record now
                        DisplayPanel(dpSalesOrder);
                        dpSalesOrder.MoveToRecord(salesOrderNode.ID);

                        //Sales order that contain WO are not allowed to change customer
                        dpSalesOrder.DisableCustomerSelection = node.Nodes.Count > 0;
                    }
                    else if (node is BatchNode batchNode)
                    {
                        dpBatch.MoveToRecord(batchNode.ID);
                        DisplayPanel(dpBatch);
                    }
                    else if (node is LaborNode laborNode)
                    {
                        dpLabor.MoveToRecord(laborNode.ID);
                        DisplayPanel(dpLabor);
                    }
                    else if (node is BulkCOCNode bulkCOCNode)
                    {
                        dpBulkCOC.MoveToRecord(bulkCOCNode.DataRow.BulkCOCID); // BulkCOC shows BulkCOC
                        DisplayPanel(dpBulkCOC);
                    }
                    else if (node is BatchCocNode batchCocNode)
                    {
                        dpBatchCoc.MoveToRecord(batchCocNode.DataRow.BatchCOCID); // Shows BatchCOC instead of BatchCOCOrder
                        DisplayPanel(dpBatchCoc);
                    }
                    else if (node is HoldNode holdNode)
                    {
                        dpHold.MoveToRecord(holdNode.ID);
                        DisplayPanel(dpHold);
                    }
                    else if (node is SerialNumberNode serialNode)
                    {
                        dpSerialNumber.MoveToRecord(serialNode.ID);
                        DisplayPanel(dpSerialNumber);
                    }
                    else if (node is PartInspectionNode partInspectionNode)
                    {
                        dpPartInspection.MoveToRecord(partInspectionNode.ID);
                        DisplayPanel(dpPartInspection);
                    }
                    else if (node is BillOfLadingNode billOfLadingNode)
                    {
                        dpBillOfLading.MoveToRecord(billOfLadingNode.DataRow.BillOfLadingID); // BoL shows BillOfLading
                        DisplayPanel(dpBillOfLading);
                    }
                    else if (node is OrderApprovalNode orderApprovalNode)
                    {
                        dpOrderApproval.MoveToRecord(orderApprovalNode.DataRow.OrderApprovalID);
                        DisplayPanel(dpOrderApproval);
                    }
                    else
                    {
                        DisplayPanel(null);
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error loading node: " + (node != null ? node.ToString() : "null");
                _log.Error(exc, errorMsg);
            }
        }

        protected override void LoadNodes(List<IDataNode> nodes)
        {
            base.LoadNodes(nodes);

            if (nodes == null)
            {
                return;
            }

            foreach (IDataNode node in nodes)
            {
                var salesOrderNode = node as SalesOrderNode;

                // Load nested sales orders - otherwise, OrderEntry can crash
                // because it can delete an older sales order without loading
                // its orders.
                if (salesOrderNode != null)
                {
                    salesOrderNode.LoadChildrenNodes(this);
                }
            }
        }

        protected override void SaveSelectedNode()
        {
        }

        private void SelectStatusFilterValue(string status)
        {
            //select default order status in the filter cbo to trigger load Open Orders
            int index = this.StatusFilter.ValueList.FindStringExact(status);
            this.StatusFilter.SelectedIndex = Math.Max(index, 0);
        }

        private void ReviseCOCNode(COCNode node)
        {
            _log.Info("Adding a revised COC " + node.Text);

            OrdersDataSet.COCRow cr = this.dpCOC.AddCOCRow(node.DataRow, SecurityManager.Current.UserID);

            //create new ui nodes
            var cn = new COCNode(cr);
            node.Parent.Nodes.Add(cn);
            cn.Select();

            OrderHistoryDataSet.UpdateOrderHistory(node.DataRow.OrderID, this.ModeDisplayName, "COC was revised as COC " + cr.COCID + ".", SecurityManager.Current.UserName);
        }

        private void ReviseBatchCocNode(BatchCocNode node)
        {
            var originalBatchCocRow = node.DataRow.BatchCOCRow;

            // Create new batch COC
            var revisedBatchCocRow = dsOrders.BatchCOC.NewBatchCOCRow();
            revisedBatchCocRow.BatchID = originalBatchCocRow.BatchID;
            revisedBatchCocRow.DateCertified = originalBatchCocRow.DateCertified;
            revisedBatchCocRow.QAUser = SecurityManager.Current.UserID;
            revisedBatchCocRow.COCInfo = originalBatchCocRow.COCInfo;
            revisedBatchCocRow.IsCompressed = originalBatchCocRow.IsCompressed;
            dsOrders.BatchCOC.AddBatchCOCRow(revisedBatchCocRow);

            foreach (var originalBatchCocOrderRow in originalBatchCocRow.GetBatchCOCOrderRows())
            {
                var revisedBatchCocOrderRow = dsOrders.BatchCOCOrder.NewBatchCOCOrderRow();
                revisedBatchCocOrderRow.BatchCOCRow = revisedBatchCocRow;
                revisedBatchCocOrderRow.OrderID = originalBatchCocOrderRow.OrderID;
                dsOrders.BatchCOCOrder.AddBatchCOCOrderRow(revisedBatchCocOrderRow);
            }

            var mainBatchOrderRow = revisedBatchCocRow.GetBatchCOCOrderRows()
                .FirstOrDefault(batchCocOrder => batchCocOrder.OrderID == node.DataRow.OrderID);

            // View new batch certificate
            var cn = new BatchCocNode(mainBatchOrderRow);
            node.Parent.Nodes.Add(cn);
            cn.Select();

            // Order History
            foreach (var batchCocOrder in revisedBatchCocRow.GetBatchCOCOrderRows())
            {
                OrderHistoryDataSet.UpdateOrderHistory(
                    node.DataRow.OrderID,
                    ModeDisplayName,
                    $"Revised Batch COC {originalBatchCocRow.BatchCOCID}.",
                    SecurityManager.Current.UserName);
            }
        }

        protected override void OnDispose()
        {
            try
            {
                if(this.dpOrder != null)
                {
                    this.dpOrder.BeforeCustomerChanged -= this.dpOrder_BeforeCustomerChanged;
                    this.dpOrder.PartsReloaded -= this.dpOrder_PartsReloaded;
                    this.dpOrder.AfterChildRowAdded -= dpOrder_AfterChildRowAdded;
                    this.dpOrder.BeforeChildRowDeleted -= dpOrder_BeforeChildRowDeleted;
                    this.dpOrder.QuickFilter -= this.dpOrder_QuickFilter;
                }

                if (this.dpBatch != null)
                {
                    this.dpBatch.QuickFilter -= this.dpOrder_QuickFilter;
                }

                if(this.dpBlanketPO != null)
                {
                    this.dpBlanketPO.BeforeCustomerChanged -= this.dpOrder_BeforeCustomerChanged;
                    this.dpBlanketPO.PartsReloaded -= this.dpBlanketPO_PartsReloaded;
                }
                

                if(this.quickFindOrder != null)
                    this.quickFindOrder.FindOrder -= this.quickFindOrder_FindOrder;

                this._dataLoadedByStatus = null;
                this._customerPartsLoaded = null;
                
                Properties.Settings.Default.Save();

                if(this._onOrderRowChanged != null && this.dsOrders != null && this.dsOrders.Order != null)
                    this.dsOrders.Order.RowChanged -= this._onOrderRowChanged;

                this._onOrderRowChanged = null;

                if (this._onCustomerCommRowChanged != null && this.dsOrders != null && this.dsOrders.CustomerCommunication  != null)
                    this.dsOrders.CustomerCommunication.RowChanged -= this._onCustomerCommRowChanged;

                this._onCustomerCommRowChanged = null;

                if (this._onSalesOrderRowChanged != null && this.dsOrders != null && this.dsOrders.SalesOrder != null)
                    this.dsOrders.SalesOrder.RowChanged -= this._onSalesOrderRowChanged;

                this._onSalesOrderRowChanged = null;

                if (dsOrders?.OrderChange != null)
                {
                    dsOrders.OrderChange.OrderChangeRowDeleting -= OnOrderChangeRowDeleting;
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error on OE form close.";
                _log.Error(exc, errorMsg);
            }

            base.OnDispose();
        }

        private void UpdateTotalCounts()
        {
            try
            {
                if (Mode == OrderEntryMode.Review || Mode == OrderEntryMode.ImportExportReview)
                {
                    return;
                }

                //NOTE: Don't use existing orders dataset because an order could have been opened and closed on the same day
                using(var ta = new Data.Reports.ProcessPartsReportTableAdapters.OrderCreationTableAdapter())
                {
                    var fromDate = DateTime.Today;
                    var toDate = fromDate.AddDays(1);

                    var ordersCreated = ta.GetData(fromDate, toDate);
                    int partCount = 0;

                    ordersCreated.ForEach(ocr => partCount += ocr.IsPartQuantityNull() ? 0 : ocr.PartQuantity);

                    var g = this.guagePartCount.Gauges[0] as DigitalGauge;
                    g.Text = partCount.ToString();
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error updating total closed order count.");
            }
        }
        
        private void GoToOrder(int orderID)
        {
            var orderNode = tvwTOC.Nodes[0].Nodes.FindNodeBFS(n => n is OrderNode && n.Visible && ((OrderNode)n).DataRow.RowState != DataRowState.Deleted && ((OrderNode)n).DataRow.OrderID == orderID);
            
            if(orderNode != null)
                orderNode.Select(true);
            else
            {
                LoadTOC(OrderSearchField.WO, orderID.ToString(), string.Empty, true);
            }
        }

        private void GoToOrder(int orderID, bool isTextFilter)
        {
            var orderNode = tvwTOC.Nodes[0].Nodes.FindNodeBFS(n => n is OrderNode && n.Visible && ((OrderNode)n).DataRow.RowState != DataRowState.Deleted && ((OrderNode)n).DataRow.OrderID == orderID);

            if (orderNode != null)
                orderNode.Select(true);
            else
            {
                if (isTextFilter)
                {
                    _resetStatusFilter = true;
                    _previousStatusFilter = (ValueListItem)this.StatusFilter.SelectedItem;
                }

                LoadTOC(OrderSearchField.WO, orderID.ToString(), string.Empty, true);
            }
        }

        private static void PrintOrderProcessSheets(bool quickPrint, OrderNode orderNode)
        {
            using(var ta = new Data.Datasets.OrderProcessingDataSetTableAdapters.ProcessTableAdapter())
            {
                foreach(var orderProcessRow in orderNode.DataRow.GetOrderProcessesRows())
                {
                    var isPaperless = ta.GetIsPaperless(orderProcessRow.ProcessID);

                    if(!isPaperless.GetValueOrDefault())
                    {
                        using(var report = new OrderProcessSheetReport() {OrderProcessId = orderProcessRow.OrderProcessesID})
                        {
                            if(quickPrint)
                                report.PrintReport();
                            else
                                report.DisplayReport();
                        }
                    }
                }
            }
        }

        private void PrintRejoinedOrderSummaries(bool quickPrint, OrderNode orderNode)
        {
            // Assumption - order node is fully loaded
            var orderId = orderNode.DataRow.OrderID;

            foreach (var rejoinRow in dsOrders.OrderChange.Where(c => c.IsValidState() && c.ChildOrderID == orderId && c.ChangeType == (int)OrderChangeType.Rejoin))
            {
                var rejoinedOrderId = rejoinRow.ParentOrderID;

                if (_loadedOrders.Contains(rejoinedOrderId))
                {
                    var rejoinedOrderRow = dsOrders.Order.FindByOrderID(rejoinedOrderId);

                    if (rejoinedOrderRow != null && rejoinedOrderRow.IsValidState())
                    {
                        // Print WO Summary using row
                        using (var summary = new WorkOrderSummaryReport(rejoinedOrderRow) { HideIncompleteProcesses = true })
                        {
                            if (quickPrint)
                            {
                                summary.PrintReport();
                            }
                            else
                            {
                                summary.DisplayReport();
                            }
                        }
                    }

                    // Otherwise, the row was deleted so skip it.
                }
                else
                {
                    // Order has not been loaded - load it & show report
                    using (var summary = new WorkOrderSummaryReportWrapper(rejoinedOrderId, true))
                    {
                        if (quickPrint)
                        {
                            summary.PrintReport();
                        }
                        else
                        {
                            summary.DisplayReport();
                        }
                    }
                }
            }
        }

        private void MarkOrderAsLoaded(int orderId)
        {
            _loadedOrders.Add(orderId);
        }

        #endregion Methods

        #region Events

        private void OrderEntry_Load(object sender, EventArgs e)
        {
            _loadingData = true; //prevent TOC from loading

            try
            {
                using(new UsingWaitCursor(this))
                {
                    txtNodeFilter.Enabled = false;

                    SuspendLayout();

                    if(DesignMode)
                        return;

                    using(new UsingTimeMe("loading order entry"))
                    {
                        tvwTOC.Override.SelectionType = SelectType.Extended;

                        this.StatusFilter.ToolValueChanged += this.StatusFilter_ToolValueChanged;
                        
                        //Load all data
                        this.InitialLoadData();
                        this.LoadStatusValueList();
                        LoadValidators();

                        //load commands if we have customers
                        if(dsOrders.CustomerSummary.Any())
                            this.LoadCommands();

                        switch(Mode)
                        {
                            case OrderEntryMode.Normal:
                                this.SelectStatusFilterValue(Properties.Settings.Default.OrderStatusOpen); //select default order status in the filter cbo to trigger load Open Orders
                                break;
                            case OrderEntryMode.Review:
                                 this.SelectStatusFilterValue(ORDER_REVIEW_STATUS);
                                this.Text = "Order Review";
                                break;
                            case OrderEntryMode.BlanketPO:
                                this.SelectStatusFilterValue(Properties.Settings.Default.OrderStatusOpen);
                                this.Text = "Blanket PO";
                                break;
                            case OrderEntryMode.ImportExportReview:
                                SelectStatusFilterValue(IMPORT_EXPORT_REVIEW_STATUS);
                                Text = "Import/Export Review";
                                break;
                            default:
                                break;
                        }

                        ReloadTOC();
                        _loadingData = false;

                        //if user has selected WO then select here
                        if(this.SelectedWO > 0)
                            base.RestoreLastSelectedNode(OrderNode.CreateKey(OrderNode.KEY_PREFIX, this.SelectedWO.ToString()));

                        //wire up bulk field change command after dpOrder Initialized (?)
                        if (SecurityManager.Current.IsInRole("OrderEntry.Edit") && this.Mode != OrderEntryMode.BlanketPO)
                            Commands.AddCommand("BulkFieldChange", new BulkFieldChangeCommand(toolbarManager.Tools["BulkFieldChange"], tvwTOC, this.dsOrders, this.dpOrder));
                        else
                            toolbarManager.Tools["BulkFieldChange"].SharedProps.Visible = false;

                        this.UpdateTotalCounts();
                    }
                }
            }
            catch(Exception exc)
            {
                _log.Warn("DataSet Errors: " + this.dsOrders.GetDataErrors());
                splitContainer1.Enabled = false;
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
            }
            finally
            {
                ResumeLayout();
                txtNodeFilter.Enabled = true;
            }
        }

        private void StatusFilter_ToolValueChanged(object sender, ToolEventArgs e)
        {
            try
            {
                using(new UsingWaitCursor(this))
                {
                    if(!_loadingData && this.StatusFilter.SelectedItem != null)
                        this.ReloadTOC();
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error changing filter.", exc);
            }
        }

        private void OnAddOrderNode(object sender, EventArgs e)
        {
            try
            {
                if(IsValidControls())
                {
                    _validators.Enabled = false;

                    //Check if we need to add order to a sales order node
                    if (tvwTOC.SelectedNode<SalesOrderNode>() != null)
                    {
                        var selectedSalesNode = tvwTOC.SelectedNode<SalesOrderNode>();
                        if(selectedSalesNode != null)
                            this.AddOrder(selectedSalesNode);
                    }
                    else if (tvwTOC.SelectedNode<OrderNode>() != null)
                    {
                        var selectedOrderNode = tvwTOC.SelectedNode<OrderNode>();
                        
                        if (selectedOrderNode.Parent is SalesOrderNode)
                            this.AddOrder(selectedOrderNode.Parent as SalesOrderNode);
                        else if (tvwTOC.Nodes[0] is OrderRootNode)
                            this.AddOrder((OrderRootNode)tvwTOC.Nodes[0]);
                        //selectedOrderNode.
                    }
                    else if (tvwTOC.SelectedNode<BlanketPONode>() != null)
                    {
                        var node = tvwTOC.SelectedNode<BlanketPONode>();
                        
                        if(node != null)
                        {
                            dpBlanketPO.EndEditing(); //end any edits to ensure that all answers are pushed to the datasource
                            
                            using(var frm = new Order.NewOrderForBlanketPODialog())
                            {
                                frm.LoadData(dsOrders, taOrder, node.DataRow);

                                if(frm.ShowDialog() == DialogResult.OK && frm.PartQuantity > 0)
                                {
                                    var order = this.AddOrder(node, frm.PartQuantity, frm.CustomFields);
                                    if(order != null)
                                    {
                                        //apply changes
                                        btnApply_Click(this, EventArgs.Empty);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //Just add order to tree
                        if (tvwTOC.Nodes[0] is OrderRootNode)
                            this.AddOrder((OrderRootNode)tvwTOC.Nodes[0]);
                    }

                    _validators.Enabled = true;
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error adding new order node.";
                _log.Error(exc, errorMsg);
            }
        }

        private void OnAddBlanketPONode(object sender, EventArgs e)
        {
            try
            {
                if (IsValidControls())
                {
                    _validators.Enabled = false;

                    AddBlanketPONode();

                    _validators.Enabled = true;
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error adding new blanket PO.";
                _log.Error(exc, errorMsg);
            }
        }

        private void OnAddContainer(object sender, EventArgs e)
        {
            try
            {
                if (IsValidControls())
                {
                    _validators.Enabled = false;

                    var node = tvwTOC.SelectedNode <OrderNode>();
                    if (node != null)
                    {
                        var partQuantity = node.DataRow.IsPartQuantityNull() ? 0 : node.DataRow.PartQuantity;

                        if (partQuantity == 0)
                        {
                            const string msg = "Cannot add a container to an order with a quantity of 0.";
                            MessageBoxUtilities.ShowMessageBoxOK(msg, "Order Entry");
                        }
                        else
                        {
                            AddOrderContainer(node, partQuantity, 1);
                        }

                    }

                    _validators.Enabled = true;
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error adding new container to order.";
                _log.Error(exc, errorMsg);
            }
        }

        private void OnAddBlanketPOFromQuote(object sender, EventArgs e)
        {
            PartsDataset.PartRow newPart = null;

            try
            {
                if (IsValidControls())
                {
                    _validators.Enabled = false;

                    using (var quoteSearch = new QuoteSearch())
                    {
                        if (quoteSearch.ShowDialog() == DialogResult.OK && quoteSearch.SelectedQuotePart != null)
                        {
                            using (var partManager = new PartManager())
                            {
                                //Need to let the part manager load before calling method to add quote part
                                partManager.Load += (s, evt) =>
                                {
                                    newPart = partManager.CreatePartFromQuote(quoteSearch.SelectedQuotePart);
                                };

                                //Did user actually create the part from quote in part manager
                                if (partManager.ShowDialog() == DialogResult.OK && newPart != null)
                                {
                                    _log.Info("Adding a blanket PO from a quote.");

                                    //create order template
                                    var orderRow = this.dpBlanketPO.AddOrderRow(newPart, newPart.CustomerID);
                                    orderRow.InitialQuantity = quoteSearch.SelectedQuotePart.Quantity;

                                    //create new ui node
                                    var node = new BlanketPONode(orderRow);
                                    tvwTOC.Nodes[0].Nodes.Add(node);
                                    node.Select();
                                }
                            }
                        }
                    }

                    _validators.Enabled = true;
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error adding new blanket PO from quote.";
                _log.Error(exc, errorMsg);
            }
        }

        private void OnAddSalesOrderNode(object sender, EventArgs e)
        {
            try
            {
                if (IsValidControls())
                {
                    _validators.Enabled = false;

                    if (tvwTOC.Nodes[0] is OrderRootNode)
                        this.AddSalesOrder((OrderRootNode)tvwTOC.Nodes[0]);

                    _validators.Enabled = true;
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error adding new sales order node.";
                _log.Error(exc, errorMsg);
            }
        }

        private void OnAddOrderFromQuotePart(int quotePartID, int? salesOrderID, int? receivingId, int? receivingQty)
        {
            try
            {
                if (IsValidControls())
                {
                    _validators.Enabled = false;
                    this.AddOrderFromQuotePart(quotePartID, salesOrderID, receivingId, receivingQty);
                    _validators.Enabled = true;
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error adding new order from quote.";
                _log.Error(exc, errorMsg);
            }
        }

        private void OnAddOrderFromReceiving(int receivingID, int salesOrderID, bool isRework)
        {
            try
            {
                if(IsValidControls())
                {
                    _validators.Enabled = false;
                    this.AddOrder(receivingID, salesOrderID, isRework);
                    _validators.Enabled = true;
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error adding new order from receiving.";
                _log.Error(exc, errorMsg);
            }
        }

        private void OnAddCommunicationNode(object sender, EventArgs e)
        {
            try
            {
                var selectedNode = tvwTOC.SelectedNode<OrderNode>();

                if(selectedNode != null && IsValidControls())
                {
                    _validators.Enabled = false;
                    this.AddCommunication(selectedNode);
                    _validators.Enabled = true;
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error on add communication node.";
                _log.Error(exc, errorMsg);
            }
        }

        private void OnAddOrderNoteNode(object sender, EventArgs e)
        {
            try
            {
                var selectedNode = tvwTOC.SelectedNode<OrderNode>();

                if (selectedNode != null && IsValidControls())
                {
                    _validators.Enabled = false;
                    this.AddOrderNote(selectedNode);
                    _validators.Enabled = true;
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error on add communication node.";
                _log.Error(exc, errorMsg);
            }
        }

        private void OnAddPartMarkNode(object sender, EventArgs e)
        {
            try
            {
                var selectedNode = tvwTOC.SelectedNode<OrderNode>();

                if(selectedNode != null && IsValidControls())
                {
                    _validators.Enabled = false;
                    this.AddPartMarkTemplate(selectedNode);
                    _validators.Enabled = true;
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error on add communication node.";
                _log.Error(exc, errorMsg);
            }
        }

        private void quickFindOrder_FindOrder(object sender, EventArgs e)
        {
            try
            {
                LoadTOC(quickFindOrder.SelectedField,
                    Data.Datasets.Utilities.SqlBless(quickFindOrder.SearchItem),
                    quickFindOrder.CustomFieldName,
                    quickFindOrder.ExactMatch);

                ((PopupControlContainerTool)toolbarManager.Tools["Filter"]).ClosePopup();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error on quick find order.";
                _log.Error(exc, errorMsg);
            }
        }

        private bool DeleteCommand_CancelCommand(List<IDeleteNode> itemsToDelete)
        {
            try
            {
                //if is order node and has already been saved.
                if(itemsToDelete.Count > 0)
                {
                    //Sales Orders
                    bool anySavedSalesOrderItems = itemsToDelete.OfType<SalesOrderNode>().Any(salesOrder => salesOrder.DataRow.RowState != DataRowState.Added && salesOrder.DataRow.RowState != DataRowState.Deleted);
                    if (anySavedSalesOrderItems)
                    {
                        var itemsAsString = new StringBuilder();

                        foreach (var so in itemsToDelete.OfType <SalesOrderNode>())
                        {
                            itemsAsString.Append("Sales Order:" + so.DataRow.SalesOrderID + "|");
                            foreach (var order in so.DataRow.GetOrderRows())
                            {
                                itemsAsString.Append("Order:" + order.OrderID + "|");
                            }
                        }

                        var results = MessageBoxUtilities.ShowMessageBoxYesOrNo("Are you sure you want to delete the Sales Order and all child Order(s)?", "Delete Sales Order", "All child orders will be deleted when the Sales Order is deleted.");

                        if (results == DialogResult.No)
                        {
                            //Cancel delete
                            return true;
                        }

                        using (var frm = new UserEventLog { Operation = "Delete", Form = "Sales Order", UserID = SecurityManager.Current.UserID, UserName = SecurityManager.Current.UserName, TransactionDetails = itemsAsString.ToString() })
                        {
                            return frm.ShowDialog(this) != DialogResult.OK;
                        }
                    }

                    //Orders
                    bool anySavedItems = itemsToDelete.OfType<OrderNode>().Any(order => order.DataRow.RowState != DataRowState.Added && order.DataRow.RowState != DataRowState.Deleted);
                    if(anySavedItems)
                    {
                        var itemsAsString = new StringBuilder();
                        foreach (var order in itemsToDelete.OfType<OrderNode>())
                        {
                            itemsAsString.Append("Order:" + order.DataRow.OrderID + "|");
                        }

                        using(var frm = new UserEventLog{Operation = "Delete", Form = "Order", UserID = SecurityManager.Current.UserID, UserName = SecurityManager.Current.UserName, TransactionDetails = itemsAsString.ToString()})
                        {
                            return frm.ShowDialog(this) != DialogResult.OK;
                        }
                    }
                }

                return false;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error on delete command cancel.";
                _log.Error(exc, errorMsg);
                return false;
            }
        }

        private void OnRemovedOrdersFromSalesOrder(object sender, EventArgsTemplate<IList<int>> e)
        {
            try
            {
                var orderIds = e.Item;

                var msg = orderIds.Count() == 1
                    ? $"Removed Order {orderIds.First()} from its Sales Order."
                    : "Removed selected Work Orders from their Sales Orders.";

                _flyoutManager.DisplayFlyout(Text, msg);

                foreach (var orderId in orderIds)
                {
                    _log.Info($"Removed {orderId} from its Sales Order.");
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after removing an order from a sales order.");
            }
        }

        private void Order_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            try
            {
                //don't bother processing if in the middle of a save
                if(this._inSavingData)
                    return;

                switch(e.Row.RowState)
                {
                    case DataRowState.Modified:
                        int changeCount = 0;

                        foreach(DataColumn column in e.Row.Table.Columns)
                        {
                            if(e.Row.IsNull(column, DataRowVersion.Original) || e.Row.IsNull(column, DataRowVersion.Current))
                            {
                                if(e.Row.IsNull(column, DataRowVersion.Original) && e.Row.IsNull(column, DataRowVersion.Current))
                                {
                                    //nothing changed
                                }
                                else
                                {
                                    changeCount++;
                                    _log.Info("For order " + e.Row["OrderID"] + " the column " + column.ColumnName + " value was modified.");
                                }
                            }
                            else if(!e.Row[column, DataRowVersion.Original].Equals(e.Row[column, DataRowVersion.Current]))
                            {
                                changeCount++;
                                _log.Info("For order " + e.Row["OrderID"] + " the column " + column.ColumnName + " value was modified.");
                            }
                        }

                        if(changeCount == 0)
                            e.Row.RejectChanges();
                        else
                            goto case DataRowState.Added;
                        break;
                    case DataRowState.Unchanged:
                    case DataRowState.Added:
                        //find node in TOC and update its UI
                        string key = OrderNode.CreateKey(OrderNode.KEY_PREFIX, e.Row["OrderID"].ToString());
                        var on = tvwTOC.GetNodeByKey(key) as OrderNode;

                        if(on != null)
                            on.UpdateNodeUI();
                        break;
                    case DataRowState.Deleted:
                    case DataRowState.Detached:
                    default:
                        //don't touch if deleted or detached as calls to check values will throw exception
                        break;
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error updating order nodes edit state icon.");
            }
        }

        private void SalesOrder_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            try
            {
                //don't bother processing if in the middle of a save
                if (this._inSavingData)
                    return;

                switch (e.Row.RowState)
                {
                    case DataRowState.Modified:
                        int changeCount = 0;

                        foreach (DataColumn column in e.Row.Table.Columns)
                        {
                            if (e.Row.IsNull(column, DataRowVersion.Original) || e.Row.IsNull(column, DataRowVersion.Current))
                            {
                                if (e.Row.IsNull(column, DataRowVersion.Original) && e.Row.IsNull(column, DataRowVersion.Current))
                                {
                                    //nothing changed
                                }
                                else
                                {
                                    changeCount++;
                                    _log.Info("For order " + e.Row["SalesOrderID"] + " the column " + column.ColumnName + " value was modified.");
                                }
                            }
                            else if (!e.Row[column, DataRowVersion.Original].Equals(e.Row[column, DataRowVersion.Current]))
                            {
                                changeCount++;
                                _log.Info("For order " + e.Row["SalesOrderID"] + " the column " + column.ColumnName + " value was modified.");
                            }
                        }

                        if (changeCount == 0)
                            e.Row.RejectChanges();
                        else
                            goto case DataRowState.Added;
                        break;
                    case DataRowState.Unchanged:
                    case DataRowState.Added:
                        //find node in TOC and update it's UI
                        string key = SalesOrderNode.CreateKey(SalesOrderNode.KEY_PREFIX, e.Row["SalesOrderID"].ToString());
                        var on = tvwTOC.GetNodeByKey(key) as SalesOrderNode;

                        if (on != null)
                            on.UpdateNodeUI();
                        break;
                    case DataRowState.Deleted:
                    case DataRowState.Detached:
                    default:
                        //don't touch if deleted or detached as calls to check values will throw exception
                        break;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating sales order nodes.");
            }
        }
        
        private void CustomerCommunication_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            try
            {
                //don't bother processing if in the middle of a save
                if (this._inSavingData)
                    return;

                switch (e.Row.RowState)
                {
                    case DataRowState.Modified:
                        int changeCount = 0;

                        foreach (DataColumn column in e.Row.Table.Columns)
                        {
                            if (e.Row.IsNull(column, DataRowVersion.Original) || e.Row.IsNull(column, DataRowVersion.Current))
                            {
                                if (e.Row.IsNull(column, DataRowVersion.Original) && e.Row.IsNull(column, DataRowVersion.Current))
                                {
                                    //nothing changed
                                }
                                else
                                {
                                    changeCount++;
                                    _log.Info("For order " + e.Row["OrderID"] + " the column " + column.ColumnName + " value was modified.");
                                }
                            }
                            else if (!e.Row[column, DataRowVersion.Original].Equals(e.Row[column, DataRowVersion.Current]))
                            {
                                changeCount++;
                                _log.Info("For order " + e.Row["OrderID"] + " the column " + column.ColumnName + " value was modified.");
                            }
                        }

                        break;

                    //don't touch if deleted or detached as calls to check values will throw exception
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error tracking customer comm changes.");
            }
        }

        private void dpOrder_BeforeCustomerChanged(int newCustomerID)
        {
            try
            {
                this.LoadParts(newCustomerID);
                this.LoadCustomFields(newCustomerID);
            }
            catch(Exception exc)
            {
                string errorMsg = "Error on order customer change.";
                _log.Error(exc, errorMsg);
            }
        }

        private void dpOrder_PartsReloaded()
        {
            try
            {
                //when parts get cleared then update cache checker
                this._customerPartsLoaded.Clear();
                this._customerPartsLoaded.Add(this.dpOrder.CurrentCustomerID);
            }
            catch(Exception exc)
            {
                string errorMsg = "Error after parts reloaded.";
                _log.Error(exc, errorMsg);
            }
        }

        private void dpBlanketPO_PartsReloaded()
        {
            try
            {
                //when parts get cleared then update cache checker
                this._customerPartsLoaded.Clear();
                this._customerPartsLoaded.Add(this.dpBlanketPO.CurrentCustomerID);
            }
            catch (Exception exc)
            {
                string errorMsg = "Error after parts reloaded for blanket po.";
                _log.Error(exc, errorMsg);
            }
        }

        private void dpOrder_QuickFilter(OrderSearchField field, string value)
        {
            LoadTOC(field, Data.Datasets.Utilities.SqlBless(value), string.Empty, true);
        }

        private void txtNodeFilter_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                if (!_loadingData)
                {
                    if(e.Button.Key == "GO")
                    {
                        if(int.TryParse(this.txtNodeFilter.Text, out var orderID))
                            this.GoToOrder(orderID, true);
                    }

                    else if(e.Button.Key == "Reset")
                    {
                        if (_resetStatusFilter)
                            StatusFilter.SelectedItem = _previousStatusFilter;

                        this.txtNodeFilter.Text = null;
                        this.ReloadTOC();
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error updating filter.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void txtNodeFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!_loadingData && e.KeyChar == '\r')
                {
                    e.Handled = true;
                    if (int.TryParse(this.txtNodeFilter.Text, out var orderID))
                        this.GoToOrder(orderID, true);
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error updating filter.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void dpOrder_AfterChildRowAdded(OrderChildRowType type, DataRow newRow)
        {
            try
            {
                switch (type)
                {
                    case OrderChildRowType.PartMark:
                        var pmRow = newRow as OrdersDataSet.OrderPartMarkRow;
                        if (pmRow != null)
                        {
                            var orderNode = tvwTOC.Nodes.FindNode<OrderNode>(on => on.DataRow.OrderID == pmRow.OrderID);
                            if (orderNode != null)
                            {
                                var pmNode = orderNode.Nodes.OfType<PartMarkNode>().FirstOrDefault(pm => pm.DataRow.RowState != DataRowState.Deleted && pm.DataRow.OrderPartMarkID == pmRow.OrderPartMarkID);
                                if (pmNode == null)
                                    orderNode.Nodes.Add(new PartMarkNode(pmRow));
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error after child row added to order.");
            }
        }

        private void dpOrder_BeforeChildRowDeleted(OrderChildRowType type, DataRow deletedRow)
        {
            try
            {
                switch (type)
                {
                    case OrderChildRowType.PartMark:
                        var pmRow = deletedRow as OrdersDataSet.OrderPartMarkRow;
                        if (pmRow != null)
                        {
                            var orderNode = tvwTOC.Nodes.FindNode<OrderNode>(on => on.DataRow.OrderID == pmRow.OrderID);
                            if (orderNode != null)
                            {
                                var pmNode = orderNode.Nodes.OfType<PartMarkNode>().FirstOrDefault(pm => pm.DataRow.RowState != DataRowState.Deleted && pm.DataRow.OrderPartMarkID == pmRow.OrderPartMarkID);
                                if(pmNode != null)
                                    pmNode.Remove(); //Remove from tree
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error before child row deleted to order.");
            }
        }

        private void printCommand_BeforePrinted(IReportNode node, string reportType, bool quickPrint)
        {
            // Load data prior to report creation
            try
            {
                var orderNode = node as OrderNode;
                orderNode?.LoadChildrenNodes(this);

                var salesNode = node as SalesOrderNode;
                salesNode?.LoadChildrenNodes(this);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error loading order nodes prior to printing report.");
            }
        }
        
        private void printCommand_AfterPrinted(IReportNode node, string reportType, bool quickPrint)
        {
            try
            {
                if (node is OrderNode orderNode)
                {
                    if (reportType == "Work Order Traveler" && Properties.Settings.Default.PrintOrderProcessSheets)
                    {
                        PrintOrderProcessSheets(quickPrint, orderNode);
                    }
                    else if (reportType == "Work Order Summary" && ApplicationSettings.Current.PrintSummariesForRejoinedOrders)
                    {
                        PrintRejoinedOrderSummaries(quickPrint, orderNode);
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error printing secondary print jobs.");
            }
        }

        private void ToolbarManager_BeforeShortcutKeyProcessed(object sender, BeforeShortcutKeyProcessedEventArgs e)
        {
            // Workaround for Infragistics issue where it doesn't show popups using shortcut keys
            if (e.Tool?.Key != "Filter")
            {
                return;
            }

            var x = (Width / 2) - (e.Tool.Control.Width / 2);
            var y = (Height / 2) - (e.Tool.Control.Height / 2);

            toolbarManager.ShowPopup("Filter", PointToScreen(new Point(x, y)));
        }

        private void toolbarManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "Settings")
            {
                this.ctlSettings.LoadSettings();
            }
        }

        private void toolbarManager_AfterToolCloseup(object sender, ToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "Settings")
            {
                this.ctlSettings.SaveSettings();
            }
        }

        private void OnOrderChangeRowDeleting(object sender, OrdersDataSet.OrderChangeRowChangeEvent args)
        {
            try
            {
                if (args.Action != DataRowAction.Delete)
                {
                    return;
                }

                // Remove change nodes for the row - this is not done
                // automatically when deleting an order because the node may
                // belong to another order.
                foreach (var nodeToRemove in tvwTOC.NodesOfType<OrderChangeNode>().Where(oc => oc.DataRow == args.Row))
                {
                    nodeToRemove.Remove();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling order change deletion.");
            }
        }

        #endregion Events

        #region Nodes

        #region Nested type: COCNode

        private class COCNode : DataNode<OrdersDataSet.COCRow>, IReportNode, ISortByDate
        {
            private const string KEY_PREFIX = "COC";

            #region Properties

            public override bool CanDelete
            {
                get { return false; }
            }

            #endregion Properties

            #region Methods

            public COCNode(OrdersDataSet.COCRow cr)
                : base(cr, cr.COCID.ToString(), KEY_PREFIX, string.Empty)
            {
                Override.NodeAppearance.Image = "Cert3";

                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                Text = "COC " + base.DataRow.COCID + " - " + base.DataRow.DateCertified.ToShortDateString();
            }

            #endregion Methods

            #region IReportNode Members

            public IReport CreateReport(string reportType)
            {
                try
                {
                    if (reportType == "COC Label")
                    {
                        return new COCLabelReport()
                        {
                            OrderId = base.DataRow.OrderID,
                            COCId = base.DataRow.COCID
                        };
                    }
                    else
                    {
                        return new COCReport(base.DataRow.COCID);
                    }
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error creating COC report.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                    return null;
                }
            }

            public string[] ReportTypes()
            {
                return new []
                {
                    "COC",
                    "COC Label"
                };
            }

            #endregion

            #region ISortByDate Members

            public DateTime? SortByDate
            {
                get
                {
                    if(DataRow != null && DataRow.RowState != DataRowState.Deleted)
                        return base.DataRow.DateCertified;
                    else
                        return null;
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: CustomerCommunicationNode

        private class CustomerCommunicationNode : DataNode<OrdersDataSet.CustomerCommunicationRow>, ISortByDate
        {
            private const string KEY_PREFIX = "CC";

            #region Methods

            public CustomerCommunicationNode(OrdersDataSet.CustomerCommunicationRow cr)
                : base(cr, cr.CusomterCommunicationID.ToString(), KEY_PREFIX, string.Empty)
            {
                Override.NodeAppearance.Image = "Communication"; //this.LeftImages.Add(UI.Properties.Resources.Communication);

                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                string name = base.DataRow.IsContactIDNull() ? "Communication" : base.DataRow.ContactSummaryRow.Name;
                Text = name + " - " + base.DataRow.CreationDate.ToShortDateString();
                
            }

            #endregion Methods

            #region ISortByDate Members

            public DateTime? SortByDate
            {
                get
                {
                    if(DataRow != null && DataRow.RowState != DataRowState.Deleted)
                        return base.DataRow.CreationDate.AddMinutes(5); //add a few minutes so it is after the shipping notification node
                    else
                        return null;
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: ISortByDate

        private interface ISortByDate
        {
            DateTime? SortByDate { get; }
        }

        #endregion

        #region Nested type: IOrderNode

        private interface IOrderNode
        {
            int SortByNumber { get; }

            int Priority { get; }
        }

        #endregion

        #region Nested type: OrderEntryNodeSorter

        private class OrderEntryNodeSorter : IComparer, IComparer<UltraTreeNode>
        {
            #region Methods

            private static int DoCompare(object x, object y)
            {
                if (x is IOrderNode && y is IOrderNode)
                {
                    int xPriority = ((IOrderNode)x).Priority;
                    int yPriority = ((IOrderNode)y).Priority;

                    int xID = ((IOrderNode)x).SortByNumber;
                    int yID = ((IOrderNode)y).SortByNumber;

                    if (xID < 0) // x is new
                    {
                        return 1;
                    }

                    if (yID < 0) // y is new
                    {
                        return -1;
                    }

                    // Ascending Order
                    var priorityCompare = xPriority.CompareTo(yPriority);

                    return priorityCompare == 0 ?
                        xID.CompareTo(yID) :
                        priorityCompare;
                }
                else if(x is ISortByDate && y is ISortByDate)
                    return ((ISortByDate)x).SortByDate.GetValueOrDefault().CompareTo(((ISortByDate)y).SortByDate.GetValueOrDefault());
                else if (x is UltraTreeNode && y is UltraTreeNode)
                    return ((UltraTreeNode)x).Text.CompareTo(((UltraTreeNode)y).Text);
                else
                    return 0;
            }

            #endregion

            #region IComparer Members

            public int Compare(object x, object y)
            {
                return DoCompare(x, y);
            }

            #endregion

            #region IComparer<UltraTreeNode> Members

            public int Compare(UltraTreeNode x, UltraTreeNode y)
            {
                return DoCompare(x, y);
            }

            #endregion
        }

        #endregion

        #region Nested type: OrderNode

        private class OrderNode : DataNode<OrdersDataSet.OrderRow>, IReportNode, IOrderNode
        {

            #region Fields

            public const string KEY_PREFIX = "OR";

            #endregion

            #region Properties

            private bool IsDataLoaded { get; set; }

            public override string ID
            {
                get { return DataRow != null ? DataRow.OrderID.ToString() : "0"; }
            }

            public OrderEntryMode Mode { get; set; }

            public override bool CanDelete => true;

            #endregion

            #region Methods

            public OrderNode(OrdersDataSet.OrderRow cr, OrderEntryMode mode)
                : base(cr, cr.OrderID.ToString(), KEY_PREFIX, cr.OrderID.ToString())
            {
                this.Mode = mode;
                this.UpdateNodeUI();
            }
            
            public override string ClipboardDataFormat
            {
                get
                {
                    if (DataRow.SalesOrderRow?.Status == Properties.Settings.Default.OrderStatusClosed)
                    {
                        // Disallow copy of orders that are part of closed
                        // sales orders. This should not prevent users from
                        // copying the sales order
                        return null;
                    }
                    else
                    {
                        return GetType().FullName;
                    }
                }
            }

            internal void LoadChildrenNodes(OrderEntry oef)
            {
                try
                {
                    if (this.IsDataLoaded)
                        return;
                    
                    //if this is a new order then don't bother loading many of the available child row types
                    if (DataRow.RowState == DataRowState.Added)
                    {
                        // add an order processing node
                        Nodes.Add(new OrderProcessingNode(base.DataRow));

                        // add part marking node
                        var pmRows = base.DataRow.GetOrderPartMarkRows() ?? Enumerable.Empty<OrdersDataSet.OrderPartMarkRow>();

                        foreach (var orwRow in pmRows)
                            Nodes.Add(new PartMarkNode(orwRow));

                        // add container node
                        var containerRows = base.DataRow.GetOrderContainersRows();
                        if(containerRows != null && containerRows.Length > 0)
                            Nodes.Add(new ContainerNode(this.DataRow));

                        this.IsDataLoaded = true;
                        return;
                    }

                    using(new UsingTimeMe("Loading Nodes for order " + DataRow.OrderID))
                    {
                        using(new UsingTreeLoad(base.Control))
                        {
                            //load order custom fields
                            oef.taOrderCustomFields.FillByOrder(oef.dsOrders.OrderCustomFields, base.DataRow.OrderID);
                           
                            //load order fees
                            oef.taOrderFees.FillByOrder(oef.dsOrders.OrderFees, base.DataRow.OrderID);
                            
                            //load media
                            oef.taOrder_Media.FillByOrder(oef.dsOrders.Order_Media, base.DataRow.OrderID);
                            oef.taMedia.FillByOrderWOMedia(oef.dsOrders.Media, base.DataRow.OrderID);
                            oef.taOrderDocumentLink.FillByOrder(oef.dsOrders.Order_DocumentLink, base.DataRow.OrderID);
                            
                            //Load Communications
                            oef.taCustomerCommunication.FillByOrder(oef.dsOrders.CustomerCommunication, base.DataRow.OrderID);
                            var rows = base.DataRow.GetCustomerCommunicationRows() ?? Enumerable.Empty<OrdersDataSet.CustomerCommunicationRow>();

                            foreach(OrdersDataSet.CustomerCommunicationRow row in rows)
                                Nodes.Add(new CustomerCommunicationNode(row));

                            //Load COCs
                            oef.taCOC.FillByOrderNoData(oef.dsOrders.COC, base.DataRow.OrderID);
                            var cocRows = base.DataRow.GetCOCRows() ?? Enumerable.Empty<OrdersDataSet.COCRow>();

                           foreach(OrdersDataSet.COCRow cocRow in cocRows)
                                Nodes.Add(new COCNode(cocRow));

                            //Load Shipments
                            LoadShipmentNodes(oef);
                           
                            //Load Order Review
                            oef.taOrderReview.FillByOrder(oef.dsOrders.OrderReview, base.DataRow.OrderID);
                            OrdersDataSet.OrderReviewRow[] orRows = base.DataRow.GetOrderReviewRows();

                            if(orRows != null && orRows.Length > 0)
                            {
                                foreach(OrdersDataSet.OrderReviewRow orRow in orRows)
                                    Nodes.Add(new OrderReviewNode(orRow));
                            }

                            //Load Order Changes
                            LoadChangeNodes(oef);

                            //Load Order Processing Node
                            if (DataRow.GetOrderProcessesRows().Length == 0)
                            {
                                oef.taOrderProcesses.FillBy(oef.dsOrders.OrderProcesses, base.DataRow.OrderID);
                            }

                            oef.taProcessAliasSummary.FillByOrder(oef.dsOrders.ProcessAliasSummary, base.DataRow.OrderID);
                            Nodes.Add(new OrderProcessingNode(base.DataRow));

                            //Load Inspections
                            oef.taPartInspection.FillBy(oef.dsOrders.PartInspection, base.DataRow.OrderID);

                            foreach (var partInspection in DataRow.GetOrderProcessesRows().SelectMany(op => op.GetPartInspectionRows()))
                            {
                                Nodes.Add(new PartInspectionNode(partInspection));
                            }

                            oef.taOrderProcessAnswer.FillBy(oef.dsOrders.OrderProcessAnswer, base.DataRow.OrderID);

                            
                            //Load Order Part Mark
                            oef.taOrderPartMark.FillByOrder(oef.dsOrders.OrderPartMark, base.DataRow.OrderID);
                            var pmRows = base.DataRow.GetOrderPartMarkRows();

                            if(pmRows != null && pmRows.Length > 0)
                            {
                                foreach(var orwRow in pmRows)
                                    Nodes.Add(new PartMarkNode(orwRow));
                            }
                            
                            //Load Order Notes
                            oef.taOrderNote.FillByOrder(oef.dsOrders.OrderNote, base.DataRow.OrderID);
                            var notes = base.DataRow.GetOrderNoteRows();

                            if (notes != null && notes.Length > 0)
                            {
                                foreach (OrdersDataSet.OrderNoteRow row in notes)
                                    Nodes.Add(new OrderNoteNode(row));
                            }

                            //Load Order Holds
                            LoadHoldNodes(oef);

                            //Load Internal Rework
                            oef.taInternalRework.FillByReworkOrderID(oef.dsOrders.InternalRework, base.DataRow.OrderID);
                            foreach (var row in base.DataRow.GetInternalReworkRowsByFK_Order_InternalRework())
                            {
                                Nodes.Add(new OrderInternalReworkNode(this.DataRow, row));
                            }
                                
                            oef.taInternalRework.FillByOriginalOrderID(oef.dsOrders.InternalRework, base.DataRow.OrderID);
                            foreach(var row in base.DataRow.GetInternalReworkRows())
                            {
                                if (row.IsReworkOrderIDNull() || row.ReworkOrderID != row.OriginalOrderID) //don't add this internal re-work since it already got added from above
                                    Nodes.Add(new OrderInternalReworkNode(this.DataRow, row));
                            }

                            //Load Container Node
                            LoadContainerNode(oef);

                            // Load Batch/BatchCOC Nodes & Some Labor Node data
                            // Some of this data may have been previously loaded
                            using (var dtBatchTemp = new OrdersDataSet.BatchDataTable())
                            {
                                oef.taBatch.FillByOrder(dtBatchTemp, DataRow.OrderID);

                                foreach (var newBatchRow in dtBatchTemp)
                                {
                                    if (oef.dsOrders.Batch.FindByBatchID(newBatchRow.BatchID) != null)
                                    {
                                        // Previously loaded batch data
                                        continue;
                                    }

                                    // Safely merge data
                                    oef.dsOrders.Batch.Rows.Add(newBatchRow.ItemArray).AcceptChanges();

                                    // Load the rest of the batch data
                                    oef.taBatchProcesses.FillByBatch(oef.dsOrders.BatchProcesses, newBatchRow.BatchID);
                                    oef.taBatchProcessesOperator.FillByBatch(oef.dsOrders.BatchProcessesOperator, newBatchRow.BatchID);
                                    oef.taBatchOperator.FillByBatch(oef.dsOrders.BatchOperator, newBatchRow.BatchID);
                                    oef.taBatchOrder.FillByBatchID(oef.dsOrders.BatchOrder, newBatchRow.BatchID);
                                    oef.taBatchProcess_OrderProcess.FillByBatch(oef.dsOrders.BatchProcess_OrderProcess, newBatchRow.BatchID);
                                }
                            }

                            using (var dtBatchCocTemp = new OrdersDataSet.BatchCOCDataTable())
                            {
                                oef.taBatchCoc.FillByOrderNoData(dtBatchCocTemp, DataRow.OrderID);

                                foreach (var newBatchCocRow in dtBatchCocTemp)
                                {
                                    if (oef.dsOrders.BatchCOC.FindByBatchCOCID(newBatchCocRow.BatchCOCID) != null)
                                    {
                                        // Previously loaded batch COC
                                        continue;
                                    }

                                    // Safely merge data
                                    oef.dsOrders.BatchCOC.Rows.Add(newBatchCocRow.ItemArray).AcceptChanges();

                                    // Load batch COC
                                    oef.taBatchCocOrder.FillByBatchCoc(oef.dsOrders.BatchCOCOrder, newBatchCocRow.BatchCOCID);
                                    oef.taBatchCocNotification.FillByBatchCoc(oef.dsOrders.BatchCOCNotification, newBatchCocRow.BatchCOCID);
                                }
                            }

                            bool hasAnyBatchOperators = false;
                            foreach (var batchID in DataRow.GetBatchOrderRows().Select(batchOrder => batchOrder.BatchID).Distinct())
                            {
                                var batch = oef.dsOrders.Batch.FirstOrDefault(b => b.IsValidState() && b.BatchID == batchID);

                                if (batch == null)
                                {
                                    LogManager.GetCurrentClassLogger().Warn("Could not find batch w/ BatchID = {0}", batchID);
                                    continue;
                                }

                                Nodes.Add(new BatchNode(this.DataRow, batch));

                                var hasBatchProcessOperators = batch.GetBatchProcessesRows()
                                    .SelectMany(row => row.GetBatchProcessesOperatorRows())
                                    .Any();

                                if (hasBatchProcessOperators)
                                {
                                    oef.taLaborTime.FillByBatch(oef.dsOrders.LaborTime, batch.BatchID);
                                }

                                var hasBatchOperators = batch.GetBatchOperatorRows().Length > 0;

                                if (hasBatchOperators)
                                {
                                    oef.taBatchOperatorTime.FillByBatch(oef.dsOrders.BatchOperatorTime, batch.BatchID);
                                }

                                hasAnyBatchOperators = hasAnyBatchOperators || hasBatchProcessOperators || hasBatchOperators;
                            }

                            foreach (var batchCocOrder in DataRow.GetBatchCOCOrderRows())
                            {
                                Nodes.Add(new BatchCocNode(batchCocOrder));
                            }

                            // Load Labor node
                            oef.taOrderProcessesOperator.FillByOrder(oef.dsOrders.OrderProcessesOperator, base.DataRow.OrderID);
                            oef.taOrderOperator.FillByOrder(oef.dsOrders.OrderOperator, DataRow.OrderID);

                            var hasAnyProcessOperators = DataRow.GetOrderProcessesRows()
                                .Any(processRow => processRow.GetOrderProcessesOperatorRows().Length > 0);

                            if (hasAnyProcessOperators)
                            {
                                oef.taLaborTime.FillByOrder(oef.dsOrders.LaborTime, base.DataRow.OrderID);
                            }

                            var hasAnyOperators = DataRow.GetOrderOperatorRows().Length > 0;

                            if (hasAnyOperators)
                            {
                                oef.taOrderOperatorTime.FillByOrder(oef.dsOrders.OrderOperatorTime, DataRow.OrderID);
                            }

                            if (hasAnyProcessOperators || hasAnyOperators || hasAnyBatchOperators)
                            {
                                Nodes.Add(new LaborNode(this.DataRow));
                            }

                            // Load BulkCOC
                            oef.taBulkCOCOrder.FillByOrder(oef.dsOrders.BulkCOCOrder, base.DataRow.OrderID);

                            foreach (var bulkCOCOrder in base.DataRow.GetBulkCOCOrderRows())
                            {
                                if (bulkCOCOrder.BulkCOCRow == null)
                                {
                                    oef.taBulkCOC.FillByBulkCOC(oef.dsOrders.BulkCOC, bulkCOCOrder.BulkCOCID);
                                }

                                if (bulkCOCOrder.BulkCOCRow == null)
                                {
                                    LogManager.GetCurrentClassLogger().Warn(
                                        "Could not retrieve Bulk COC for BulkCOCOrder {0}",
                                        bulkCOCOrder.BulkCOCOrderID);
                                }
                                else
                                {
                                    Nodes.Add(new BulkCOCNode(bulkCOCOrder));
                                }
                            }

                            // Load bills of lading and associated media
                            oef.taBillOfLadingOrder.FillByOrder(oef.dsOrders.BillOfLadingOrder, DataRow.OrderID);

                            foreach (var billOfLadingOrder in DataRow.GetBillOfLadingOrderRows())
                            {
                                if (billOfLadingOrder.BillOfLadingRow == null)
                                {
                                    var billOfLadingId = billOfLadingOrder.BillOfLadingID;
                                    oef.taBillOfLading.FillByBillOfLading(oef.dsOrders.BillOfLading, billOfLadingId);
                                    oef.taBillOfLadingMedia.FillByBillOfLading(oef.dsOrders.BillOfLadingMedia, billOfLadingId);
                                    oef.taBillOfLadingDocumentLink.FillByBillOfLading(oef.dsOrders.BillOfLadingDocumentLink, billOfLadingId);
                                    oef.taMedia.FillByBillOfLading(oef.dsOrders.Media, billOfLadingId);
                                }

                                if (billOfLadingOrder.BillOfLadingRow == null)
                                {
                                    LogManager.GetCurrentClassLogger().Warn(
                                        $"Could not retrieve Bill of Lading for BillOfLadingOrder {billOfLadingOrder.BillOfLadingOrderID}");
                                }
                                else
                                {
                                    Nodes.Add(new BillOfLadingNode(billOfLadingOrder));
                                }
                            }

                            // Load serial numbers
                            LoadSerialNumberNode(oef);

                            // Load order product class
                            oef.taOrderProductClass.FillByOrder(oef.dsOrders.OrderProductClass, DataRow.OrderID);

                            // Load order approvals
                            oef.taOrderApproval.FillByOrder(oef.dsOrders.OrderApproval, DataRow.OrderID);
                            oef.taOrderApprovalMedia.FillByOrder(oef.dsOrders.OrderApprovalMedia, DataRow.OrderID);

                            foreach (var orderApprovalRow in DataRow.GetOrderApprovalRows())
                            {
                                Nodes.Add(new OrderApprovalNode(orderApprovalRow));

                                foreach (var orderApprovalMediaRow in orderApprovalRow.GetOrderApprovalMediaRows())
                                {
                                    if (oef.dsOrders.Media.FindByMediaID(orderApprovalMediaRow.MediaID) == null)
                                    {
                                        oef.taMedia.FillById(
                                            oef.dsOrders.Media,
                                            orderApprovalMediaRow.MediaID);
                                    }
                                }
                            }

                            // Load work descriptions
                            oef.taOrderWorkDescription.FillByOrder(oef.dsOrders.OrderWorkDescription, DataRow.OrderID);

                            this.IsDataLoaded = true;
                        }
                    }

                    oef.MarkOrderAsLoaded(DataRow.OrderID);
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error loading order node children.";
                    LogManager.GetCurrentClassLogger().Error(exc, errorMsg);

                    this.IsDataLoaded = true;
                }
            }

            public override void UpdateNodeUI()
            {
                if(base.DataRow != null)
                {
                    if (base.DataRow.OrderType == (int)OrderType.ReworkExt)
                        Override.NodeAppearance.Image = "rework";
                    else if (base.DataRow.OrderType == (int)OrderType.ReworkInt)
                        Override.NodeAppearance.Image = "intRework";
                    else if (base.DataRow.OrderType == (int)OrderType.Lost)
                        Override.NodeAppearance.Image = "lost";
                    else if (base.DataRow.OrderType == (int)OrderType.Quarantine)
                        Override.NodeAppearance.Image = "quarantine";
                    else if (base.DataRow.OrderType == (int)OrderType.ReworkHold)
                        Override.NodeAppearance.Image = "hold";
                    else if(base.DataRow.Hold)
                        Override.NodeAppearance.Image = "hold";
                    else
                        Override.NodeAppearance.Image = "Order";

                    Text = OrderUtilities.GetDisplayString(DataRow, ApplicationSettings.Current.OrderItemFormat);

                    if (Mode == OrderEntryMode.Review || Mode == OrderEntryMode.ImportExportReview)
                    {
                        //if my own order
                        base.Override.NodeAppearance.ForeColor = DataRow.CreatedBy == SecurityManager.Current.UserID ? Color.Gray : Color.Black;
                    }
                    else
                    {
                        if(base.DataRow.Status == "Open" && (!base.DataRow.IsEstShipDateNull() && base.DataRow.EstShipDate.Subtract(DateTime.Now).Days <= 0))
                            base.Override.NodeAppearance.ForeColor = Color.Red;
                        else
                            base.Override.NodeAppearance.ForeColor = Color.Black;
                    }

                    if(base.DataRow.Status == "Closed")
                        base.Override.NodeAppearance.FontData.Bold = DefaultableBoolean.True;
                    
                    RightImages.Clear();

                    switch(base.DataRow.RowState)
                    {
                        case DataRowState.Added:
                            RightImages.Add(Properties.Resources.Add_16);
                            break;
                        case DataRowState.Modified:
                            RightImages.Add(Properties.Resources.Modified);
                            break;
                        case DataRowState.Unchanged:
                        case DataRowState.Deleted:
                        case DataRowState.Detached:
                        default:
                            break;
                    }

                    foreach (var node in Nodes.OfType<IDataNode>())
                    {
                        node.UpdateNodeUI();
                    }
                }
            }

            #endregion Methods

            #region IReportNode Members

            public IReport CreateReport(string reportType)
            {
                //save last report ran so we can reselect it
                Properties.Settings.Default.LastSelectedOrderReport = reportType;

                if(reportType == "Work Order Summary")
                    return new WorkOrderSummaryReport(base.DataRow);
                else if (reportType == "Work Order Label")
                    return new WorkOrderLabelReport() { Order = base.DataRow };
                else if (reportType == "Rework Label")
                    return new ReworkLabelReport(base.DataRow, ReworkLabelReport.ReportLabelType.Rework);
                else
                    return new WorkOrderTravelerReport(base.DataRow);
            }

            public string[] ReportTypes()
            {
                //set order of display based on what was last selected so it is ordered by MRU
                if(Properties.Settings.Default.LastSelectedOrderReport == "Work Order Label")
                    return new[] { "Work Order Label", "Work Order Traveler", "Work Order Summary", "Rework Label" };
                else
                    return new[] { "Work Order Traveler", "Work Order Label", "Work Order Summary", "Rework Label" };
            }

            public void LoadSerialNumberNode(OrderEntry oef)
            {
                if (Nodes.OfType<SerialNumberNode>().Any())
                {
                    return;
                }

                oef.taOrderSerial.FillByOrder(oef.dsOrders.OrderSerialNumber, DataRow.OrderID);

                if (DataRow.GetOrderSerialNumberRows().Any(s => !s.Active))
                {
                    Nodes.Add(new SerialNumberNode(DataRow));
                }
            }

            public void LoadContainerNode(OrderEntry oef)
            {
                if (Nodes.OfType<ContainerNode>().Any())
                {
                    return;
                }

                oef.taOrderContainers.FillByOrder(oef.dsOrders.OrderContainers, base.DataRow.OrderID);
                oef.taOrderContainerItem.FillByOrder(oef.dsOrders.OrderContainerItem, base.DataRow.OrderID);
                
                var containerRows = base.DataRow.GetOrderContainersRows();
                if(containerRows != null && containerRows.Length > 0)
                    Nodes.Add(new ContainerNode(this.DataRow));
            }

            public void LoadChangeNodes(OrderEntry oef)
            {
                if (!IsDataLoaded)
                {
                    oef.taOrderChange.FillByOrderID(oef.dsOrders.OrderChange, base.DataRow.OrderID);
                }

                var orderChanges = oef.dsOrders.OrderChange
                    .Select("ChildOrderID = {0} OR ParentOrderID = {0}".FormatWith(base.DataRow.OrderID)) as OrdersDataSet.OrderChangeRow[];

                if (orderChanges != null)
                {
                    foreach (var orwRow in orderChanges)
                    {
                        if (Nodes.OfType<OrderChangeNode>().All(n => n.DataRow.OrderChangeID != orwRow.OrderChangeID))
                        {
                            Nodes.Add(new OrderChangeNode(orwRow, base.DataRow));
                        }
                    }
                }
            }

            public void LoadShipmentNodes(OrderEntry oef)
            {
                if (!IsDataLoaded)
                {
                    oef.taOrderShipment.FillByOrder(oef.dsOrders.OrderShipment, base.DataRow.OrderID);
                }

                var shipRows = base.DataRow.GetOrderShipmentRows() ?? Enumerable.Empty<OrdersDataSet.OrderShipmentRow>();

                foreach (OrdersDataSet.OrderShipmentRow shipRow in shipRows)
                {
                    if (shipRow.IsShipmentPackageIDNull())
                    {
                        // This shouldn't be the case.
                        LogManager.GetCurrentClassLogger()
                            .Warn($"Order Shipment {shipRow.ShipmentID} has a null ShipmentID");

                        continue;
                    }

                    if (Nodes.OfType<ShipmentNode>().All(n => n.DataRow.ShipmentID != shipRow.ShipmentID))
                    {
                        // Load dependencies
                        if (oef.dsOrders.ShipmentPackage.FindByShipmentPackageID(shipRow.ShipmentPackageID) == null)
                        {
                            oef.taShipmentPackage.FillByShipmentPackage(oef.dsOrders.ShipmentPackage, shipRow.ShipmentPackageID);
                        }

                        // Add node
                        Nodes.Add(new ShipmentNode(shipRow, oef.dsOrders));
                    }
                }
            }

            public void LoadHoldNodes(OrderEntry oef)
            {
                if (!IsDataLoaded)
                {
                    oef.taOrderHold.Fill(oef.dsOrders.OrderHold, base.DataRow.OrderID);
                }

                foreach (var hold in DataRow.GetOrderHoldRows())
                {
                    if (Nodes.OfType<HoldNode>().All(n => n.DataRow.OrderHoldID != hold.OrderHoldID))
                    {
                        Nodes.Add(new HoldNode(hold));
                    }
                }
            }

            public void UpdateBatchNodes(OrderEntry oef)
            {
                var currentBatchesForOrder = DataRow.GetBatchOrderRows()
                    .Select(batchOrder => batchOrder.BatchID)
                    .ToList();

                var batchOrderNodes = this.FindNodes<BatchNode>();
                var nodesToRemove = batchOrderNodes
                    .Where(node => !node.DataRow.IsValidState() || !currentBatchesForOrder.Contains(node.DataRow.BatchID))
                    .ToList();

                foreach (var node in nodesToRemove)
                {
                    Nodes.Remove(node);
                    batchOrderNodes.Remove(node);
                }

                foreach (var batchOrder in DataRow.GetBatchOrderRows())
                {
                    if (batchOrder.BatchRow == null)
                    {
                        LogManager.GetCurrentClassLogger()
                            .Warn("Could not find batch w/ BatchID = {0}", batchOrder.BatchID);

                        continue;
                    }

                    if (batchOrderNodes.All(node => node.DataRow.BatchID != batchOrder.BatchID))
                    {
                        var batchNode = new BatchNode(DataRow, batchOrder.BatchRow);
                        Nodes.Add(batchNode);
                        batchOrderNodes.Add(batchNode);
                    }
                }
            }

            #endregion

            #region IOrderNode Members

            public int SortByNumber
            {
                get { return DataRow != null && DataRow.IsValidState() ? DataRow.OrderID : 0; }
            }

            public int Priority
            {
                get
                {
                    return 2;
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: SalesOrderNode

        private class SalesOrderNode : DataNode<OrdersDataSet.SalesOrderRow>, IReportNode, IOrderNode
        {
            #region Fields

            private OrdersDataSet _dataset;

            #endregion

            #region Properties

            public const string KEY_PREFIX = "SO";

            private bool IsDataLoaded { get; set; }

            public override string ID
            {
                get { return DataRow != null ? DataRow.SalesOrderID.ToString() : "0"; }
            }

            /// <summary>
            /// Used in filtering orders when dynamically loading them.
            /// </summary>
            public OrdersDataSet.d_OrderStatusRow FilterStatus
            {
                get;
                private set;
            }
            public override string ClipboardDataFormat
            {
                get { return GetType().FullName; }
            }

            public override bool HasChanges
            {
                get
                {
                    return base.HasChanges ||
                        this.DataRow.GetOrderRows().Any(order => order.RowState != DataRowState.Unchanged);
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of this class.
            /// </summary>
            /// <param name="sor">Sales Order instance.</param>
            public SalesOrderNode(OrdersDataSet.SalesOrderRow sor, OrdersDataSet dataset)
                : base(sor, sor.SalesOrderID.ToString(), KEY_PREFIX, sor.SalesOrderID.ToString())
            {
                this.UpdateNodeUI();
                this._dataset = dataset;
            }

            /// <summary>
            /// Initializes a new instance of this class.
            /// </summary>
            /// <param name="sor">Sales Order instance.</param>
            /// <param name="filterStatus">Status to use for filtering orders when dynamically loading them.</param>
            public SalesOrderNode(OrdersDataSet.SalesOrderRow sor, OrdersDataSet.d_OrderStatusRow filterStatus, OrdersDataSet dataset)
                : base(sor, sor.SalesOrderID.ToString(), KEY_PREFIX, sor.SalesOrderID.ToString())
            {
                this.UpdateNodeUI();
                FilterStatus = filterStatus;
                _dataset = dataset;
            }

            internal void LoadChildrenNodes(OrderEntry oef)
            {
                try
                {
                    if (this.IsDataLoaded)
                        return;

                    //if this is a new order then don't bother loading child rows, there aren't any
                    if (DataRow.RowState == DataRowState.Added)
                    {
                        //add an order node before bailing though
                        //Nodes.Add(new OrderNode(base.DataRow));

                        this.IsDataLoaded = true;
                        return;
                    }
                    
                    using (new UsingTimeMe("Loading nodes for sales order " + base.DataRow.SalesOrderID))
                    {
                        using (new UsingTreeLoad(base.Control))
                        {
                            //load media
                            oef.taSalesOrder_Media.FillBySalesOrder(oef.dsOrders.SalesOrder_Media, base.DataRow.SalesOrderID);
                            oef.taMedia.FillBySalesOrderMedia(oef.dsOrders.Media, base.DataRow.SalesOrderID);
                            oef.taSalesOrderDocumentLink.FillBySalesOrder(oef.dsOrders.SalesOrder_DocumentLink, base.DataRow.SalesOrderID);

                            // Dynamically load dependent orders & create
                            // nodes for them if they haven't been added yet.
                            if (this.FilterStatus != null)
                            {
                                // Preserve changes to existing Work Orders by merging them
                                // instead of loading them normally
                                using (var dtOrderTemp = new OrdersDataSet.OrderDataTable())
                                {
                                    oef.taOrder.FillBySalesOrder(dtOrderTemp, DataRow.SalesOrderID);

                                    // Safely merge data
                                    foreach (var newOrderRow in dtOrderTemp)
                                    {
                                        if (oef.dsOrders.Order.FindByOrderID(newOrderRow.OrderID) == null)
                                        {
                                            oef.dsOrders.Order.Rows.Add(newOrderRow.ItemArray).AcceptChanges();
                                        }
                                    }
                                }

                                var matchingOrders = oef.dsOrders.Order
                                    .Where(o => o.IsValidState() && !o.IsSalesOrderIDNull() && o.SalesOrderID == base.DataRow.SalesOrderID);

                                var addedOrders = new HashSet<int>();
                                foreach (var node in Nodes)
                                {
                                    OrderNode orderNode = node as OrderNode;

                                    if (orderNode != null)
                                    {
                                        addedOrders.Add(orderNode.DataRow.OrderID);
                                    }
                                }

                                foreach (var order in matchingOrders)
                                {
                                    bool isOrderAbsent = !addedOrders.Contains(order.OrderID) &&
                                        order.Status == FilterStatus.OrderStatusID;

                                    if (isOrderAbsent)
                                    {
                                        Nodes.Add(new OrderNode(order, oef.Mode));
                                        addedOrders.Add(order.OrderID);
                                    }
                                }
                            }

                            // Load children for order nodes - required for copying sales order
                            foreach (var orderNode in Nodes.OfType<OrderNode>())
                            {
                                orderNode.LoadChildrenNodes(oef);
                            }

                            this.IsDataLoaded = true;
                        }
                    }
                }
                catch (Exception exc)
                {
                    string errorMsg = "Error loading sales order node children.";
                    LogManager.GetCurrentClassLogger().Error(exc, errorMsg);

                    this.IsDataLoaded = true;
                }
            }

            public override void UpdateNodeUI()
            {
                if (base.DataRow != null)
                {
                    Override.NodeAppearance.Image = "sales";

                    RightImages.Clear();

                    switch (base.DataRow.RowState)
                    {
                        case DataRowState.Added:
                            RightImages.Add(Properties.Resources.Add_16);
                            break;
                        case DataRowState.Modified:
                            RightImages.Add(Properties.Resources.Modified);
                            break;
                    }

                    if (base.DataRow.IsValidState())
                    {
                        Text = OrderUtilities.GetDisplayString(DataRow, ApplicationSettings.Current.OrderItemFormat);
                    }
                }
            }
            public override bool CanPasteData(string format)
            {
                return this.DataRow?.Status != Properties.Settings.Default.OrderStatusClosed &&
                    format == typeof(OrderNode).FullName;
            }

            public override UltraTreeNode PasteData(string format, DataRowProxy proxy)
            {
                if (proxy == null)
                {
                    return null;
                }

                //Only Keep the following relations
                var validRelations = new List<string>
                {
                    "FK_OrderProcesses_Order",
                    "FK_OrderFees_Order",
                    "FK_Order_Media_Order",
                    "FK_OrderCustomFields_Order",
                    "FK_Order_SalesOrder",
                    "FK_OrderPartMark_Order"
                };

                //remove rows that shouldn't be copied
                proxy.ChildProxies = proxy.ChildProxies.Where(prox => validRelations.Contains(prox.ParentRelation)).ToList();

                //set dates for order processes to null
                var orderProcesses = proxy.ChildProxies.Where(prox => prox.ParentRelation == "FK_OrderProcesses_Order");

                foreach (var orderProcess in orderProcesses)
                {
                    var startDate = orderProcess.ColumnNameArray.IndexOf(c => c == "StartDate");
                    orderProcess.ItemArray[startDate] = null;
                    var endDate = orderProcess.ColumnNameArray.IndexOf(c => c == "EndDate");
                    orderProcess.ItemArray[endDate] = null;
                    var estShipDate = orderProcess.ColumnNameArray.IndexOf(c => c == "EstEndDate");
                    orderProcess.ItemArray[estShipDate] = null;

                    var partCount = orderProcess.ColumnNameArray.IndexOf(c => c == "PartCount");
                    orderProcess.ItemArray[partCount] = null;

                    //update order process's answers
                    orderProcess.ChildProxies.Clear();
                }

                var or = DataNode<DataRow>.AddPastedDataRows(proxy, this._dataset.Order) as OrdersDataSet.OrderRow;

                //update all OrderID's on OrderProcessAnswers and Part Inspections
                or.GetOrderProcessesRows().ForEach(op => op.GetOrderProcessAnswerRows().ForEach(opa => opa.OrderID = or.OrderID));
                or.GetOrderProcessesRows().ForEach(op => op.GetPartInspectionRows().ForEach(pi => pi.OrderID = or.OrderID));

                or.OrderDate = DateTime.Now;
                or.Status = Properties.Settings.Default.OrderStatusOpen;
                or.CreatedBy = SecurityManager.Current.UserID;
                or.ContractReviewed = false;
                or.WorkStatus = OrderControllerExtensions.GetNewOrderWorkStatus(or.CustomerID, SecurityManager.Current.CurrentUser == null || SecurityManager.Current.CurrentUser.RequireOrderReview);

                // Remove part marking dates
                foreach (var partMark in or.GetOrderPartMarkRows())
                {
                    partMark.SetPartMarkedDateNull();
                }

                //In case order is closed
                or.SetCompletedDateNull();
                or.SetInvoiceNull();
                or.SetReceivingIDNull();

                or.OrderType = (int)OrderType.Normal;
                or.OriginalOrderType = (int)OrderType.Normal;
                or.CurrentLocation = ApplicationSettings.Current.DepartmentSales;

                var orderNode = new OrderNode(or, OrderEntryMode.Normal);
                base.Nodes.Add(orderNode);
                orderNode.Select();
                orderNode.BringIntoView();
                return orderNode;
            }
 
            #endregion Methods

            #region IReportNode Members

            public IReport CreateReport(string reportType)
            {
                if (reportType == "Default" || reportType == "Sales Order Traveler")
                {
                    return new SalesOrderTravelerReport(DataRow);
                }

                if (reportType == "Sales Order Price Sheet")
                {
                    return new SalesOrderPriceReport(base.DataRow);
                }

                if (reportType == "Work Order Label(s)")
                {
                    return new SalesOrderWorkOrderLabelsReport(base.DataRow);
                }

                if (reportType == "Work Order Traveler(s)")
                {
                    return new SalesOrderWorkOrderTravelersReport(base.DataRow);
                }

                return null;
            }

            public string[] ReportTypes()
            {
                return new[] { "Sales Order Traveler", "Sales Order Price Sheet", "Work Order Label(s)", "Work Order Traveler(s)" };
            }

            #endregion

            #region IOrderNode Members

            public int SortByNumber
            {
                get { return DataRow != null && DataRow.IsValidState() ? DataRow.SalesOrderID : 0; }
            }

            public int Priority
            {
                get
                {
                    return 1;
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: OrderProcessingNode

        private class OrderProcessingNode : DataNode<OrdersDataSet.OrderRow>, ISortByDate
        {
            private const string KEY_PREFIX = "ORP";

            #region Properties

            public override bool CanDelete
            {
                get { return false; }
            }

            #endregion Properties

            #region Methods

            public OrderProcessingNode(OrdersDataSet.OrderRow cr) :
                base(cr, cr.OrderID.ToString(), KEY_PREFIX, "Order Processing")
            {
                Override.NodeAppearance.Image = "processing";
            }

            public override void UpdateNodeUI() { }

            #endregion Methods

            #region ISortByDate Members

            public DateTime? SortByDate
            {
                get
                {
                    if (DataRow != null && DataRow.RowState != DataRowState.Deleted && !base.DataRow.IsCompletedDateNull())
                        return base.DataRow.CompletedDate;
                    else
                        return null;
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: PartInspectionNotesNode

        private class PartInspectionNode : DataNode<OrdersDataSet.PartInspectionRow>, ISortByDate
        {
            private const string KEY_PREFIX = "PRT_INSP";

            #region Properties

            public override bool CanDelete => false;

            #endregion Properties

            #region Methods

            public PartInspectionNode(OrdersDataSet.PartInspectionRow cr)
                : base(cr, cr.PartInspectionID.ToString(), KEY_PREFIX, string.Empty)
            {
                Override.NodeAppearance.Image = "inspection";
                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                if (DataRow == null || !DataRow.IsValidState())
                {
                    Text = string.Empty;
                    return;
                }

                var dateString = DataRow.IsInspectionDateNull()
                    ? "N/A"
                    : DataRow.InspectionDate.ToShortDateString();

                Text = @"Inspection - " + dateString;
            }

            #endregion Methods

            #region ISortByDate Members

            public DateTime? SortByDate => DataRow != null && DataRow.IsValidState() && !DataRow.IsInspectionDateNull()
                ? DataRow.InspectionDate
                : (DateTime?) null;

            #endregion
        }

        #endregion

        #region Nested type: OrderReviewNode

        private class OrderReviewNode : DataNode<OrdersDataSet.OrderReviewRow>, ISortByDate
        {
            private const string KEY_PREFIX = "ORW";

            #region Properties

            public override bool CanDelete
            {
                get { return false; }
            }

            #endregion Properties

            #region Methods

            public OrderReviewNode(OrdersDataSet.OrderReviewRow cr)
                : base(cr, cr.OrderReviewID.ToString(), KEY_PREFIX, string.Empty)
            {
                Override.NodeAppearance.Image = "ReviewOrder";

                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                Color nodeColor;
                if (base.DataRow.IsStatusNull())
                {
                    nodeColor = Color.Orange;
                }
                else if (base.DataRow.Status)
                {
                    nodeColor = Color.Green;
                }
                else
                {
                    nodeColor = Color.Red;
                }

                Override.NodeAppearance.ForeColor = nodeColor;

                var title = $"{DataRow.OrderReviewTypeRow.Name} - ";
                base.Text = title + base.DataRow.DateReviewed.ToShortDateString();
            }

            #endregion Methods

            #region ISortByDate Members

            public DateTime? SortByDate
            {
                get
                {
                    if (DataRow != null && DataRow.RowState != DataRowState.Deleted)
                        return base.DataRow.DateReviewed;
                    else
                        return null;
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: BlanketPONode

        private class BlanketPONode : DataNode<OrdersDataSet.OrderTemplateRow>, IOrderNode
        {
            private const string KEY_PREFIX = "BPO";

            #region Properties

            public override bool CanDelete
            {
                get { return true; }
            }

            private bool IsDataLoaded { get; set; }

            #endregion Properties

            #region Methods

            public BlanketPONode(OrdersDataSet.OrderTemplateRow cr)
                : base(cr, cr.OrderTemplateID.ToString(), KEY_PREFIX, string.Empty)
            {
                Override.NodeAppearance.Image = "BlanketPO"; 
                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                Text = OrderUtilities.GetDisplayString(DataRow, ApplicationSettings.Current.OrderItemFormat);
            }

            internal void LoadChildrenNodes(OrderEntry oef)
            {
                try
                {
                    if (this.IsDataLoaded)
                        return;

                    //if this is a new order then don't bother loading child rows, there aren't any
                    if (DataRow.RowState == DataRowState.Added)
                    {
                        this.IsDataLoaded = true;
                        return;
                    }

                    using (new UsingTimeMe("Loading Nodes for blanket PO." + DataRow.OrderTemplateID))
                    {
                        using (new UsingTreeLoad(base.Control))
                        {
                            oef.taOrder.FillByOrderTemplate(oef.dsOrders.Order, this.DataRow.OrderTemplateID);
                            var rows = base.DataRow.GetOrderRows();

                            if (rows != null && rows.Length > 0)
                            {
                                foreach (var row in rows)
                                    Nodes.Add(new OrderNode(row, oef.Mode));
                            }
                            
                            this.IsDataLoaded = true;
                        }
                    }
                }
                catch (Exception exc)
                {
                    string errorMsg = "Error loading blanket PO node children.";
                    LogManager.GetCurrentClassLogger().Error(exc, errorMsg);

                    this.IsDataLoaded = true;
                }
            }

            public override void Delete()
            {
                if (DataRow != null)
                {
                    DataRow.CancelEdit();
                    DataRow.Delete();
                    DataRow = null;
                }

                Remove();
            }

            #endregion Methods

            #region IOrderNode Members

            public int SortByNumber
            {
                get { return DataRow != null && DataRow.IsValidState() ? DataRow.OrderTemplateID : 0; }
            }

            public int Priority
            {
                get
                {
                    return 1;
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: OrderChangeNode

        private class OrderChangeNode : DataNode<OrdersDataSet.OrderChangeRow>
        {
            #region Fields

            public const string KEY_PREFIX = "OC";

            #endregion

            #region Properties

            public override bool CanDelete
            {
                get { return false; }
            }

            protected OrdersDataSet.OrderRow OriginalOrder { get; set; }

            #endregion Properties

            #region Methods

            public OrderChangeNode(OrdersDataSet.OrderChangeRow cr, OrdersDataSet.OrderRow originalOrder)
                : base(cr, originalOrder.OrderID + "_" + cr.OrderChangeID.ToString(), KEY_PREFIX, string.Empty)
            {
                Override.NodeAppearance.Image = "rework";
                this.OriginalOrder = originalOrder;
                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                bool isOriginal = false;

                //determine if this is the rework node
                if (this.OriginalOrder != null)
                    isOriginal = this.OriginalOrder.OrderID == this.DataRow.ParentOrderID; //if order is not rework than this is

                //IF NOT Original, then this points to original
                var changeType = (OrderChangeType)this.DataRow.ChangeType;

                if (!isOriginal)
                {
                    if (changeType == OrderChangeType.Rejoin)
                    {
                        Text = $"Rejoined Order - {DataRow.ParentOrderID}";
                    }
                    else
                    {
                        Text = "Original Order - " + base.DataRow.ParentOrderID;
                    }
                    Override.NodeAppearance.Image = "Order";
                }
                else //ELSE point to child
                {
                    switch (changeType)
                    {
                        case OrderChangeType.ExtRework:
                            Text = "Order Rework - " + base.DataRow.ChildOrderID;
                            Override.NodeAppearance.Image = "rework";
                            break;
                        case OrderChangeType.Split:
                            Text = "Order Split - " + base.DataRow.ChildOrderID;
                            Override.NodeAppearance.Image = "split";
                            break;
                        case OrderChangeType.Rejoin:
                            Text = "Order Rejoin - " + base.DataRow.ChildOrderID;
                            Override.NodeAppearance.Image = "rejoin";
                            break;
                        default:
                            Text = base.DataRow.ChildOrderID.ToString();
                            break;
                    }
                }
            }

            public override void Dispose()
            {
                this.OriginalOrder = null;
                base.Dispose();
            }

            #endregion Methods
        }

        #endregion

        #region Nested type: OrderInternalReworkNode

        /// <summary>
        /// This class shows link to the rework node.
        /// </summary>
        private class OrderInternalReworkNode : DataNode<OrdersDataSet.InternalReworkRow>, IReportNode
        {
            #region Fields

            public const string KEY_PREFIX = "IR";

            #endregion

            #region Properties

            public override bool CanDelete
            {
                get { return false; }
            }

            protected OrdersDataSet.OrderRow CurrentOrder { get; set; }

            #endregion Properties

            #region Methods

            public OrderInternalReworkNode(OrdersDataSet.OrderRow orderRow, OrdersDataSet.InternalReworkRow cr)
                : base(cr, orderRow.OrderID + "_" + cr.InternalReworkID, KEY_PREFIX, "Internal Rework")
            {
                this.CurrentOrder = orderRow;
                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                if(CurrentOrder.OrderID == base.DataRow.OriginalOrderID)
                    Text = "Internal Rework - " + (base.DataRow.IsReworkOrderIDNull() ? "NA" : base.DataRow.ReworkOrderID.ToString());
                else
                    Text = "Internal Rework - " + base.DataRow.OriginalOrderID;

                Override.NodeAppearance.Image = "intRework";

                var reworkType = ReworkType.None;
                Enum.TryParse(base.DataRow.ReworkType, out reworkType);

                switch(reworkType)
                {
                    case ReworkType.Full:
                        Override.NodeAppearance.Image = "intRework";
                        break;
                    case ReworkType.Split:
                        Override.NodeAppearance.Image = "split";
                        break;
                    case ReworkType.SplitHold:
                        Override.NodeAppearance.Image = "hold";
                        break;
                    case ReworkType.Quarantine:
                        Override.NodeAppearance.Image = "quarantine";
                        break;
                    case ReworkType.Lost:
                        Override.NodeAppearance.Image = "lost";
                        break;
                    default:
                        Override.NodeAppearance.Image = "intRework";
                        break;
                }
            }

            public override void Dispose()
            {
                this.CurrentOrder = null;
                base.Dispose();
            }

            #endregion Methods

            #region IReportNode Members

            public IReport CreateReport(string reportType)
            {
                //save last report ran so we can reselect it
                Properties.Settings.Default.LastSelectedOrderReport = reportType;

                if (reportType == "Rework Label")
                    return new ReworkLabelReport(CurrentOrder, ReworkLabelReport.ReportLabelType.Rework);
                else
                    return new WorkOrderTravelerReport(CurrentOrder);
            }

            public string[] ReportTypes()
            {
                //set order of display based on what was last selected so it is ordered by MRU
                return new[] { "Work Order Traveler", "Rework Label" };
            }

            #endregion
        }

        #endregion

        #region Nested type: OrderRootNode

        private class OrderRootNode : UltraTreeNode, ICopyPasteNode
        {
            #region Properties

            private OrdersDataSet _dataset;

            #endregion Properties

            #region Methods

            public OrderRootNode(OrdersDataSet dataset)
                : base("ROOT", "Orders")
            {
                this._dataset = dataset;
                Override.NodeAppearance.Image = "Order"; //this.LeftImages.Add(UI.Properties.Resources.Order);
            }

            public override void Dispose()
            {
                this._dataset = null;
                base.Dispose();
            }

            #endregion Methods

            #region ICopyPasteNode Members

            public UltraTreeNode PasteData(string format, DataRowProxy proxy)
            {
                if (proxy == null)
                {
                    return null;
                }

                UltraTreeNode newNode;
                if (format == typeof(OrderNode).FullName)
                {
                    newNode = PasteWorkOrder(proxy);
                }
                else if (format == typeof(SalesOrderNode).FullName)
                {
                    newNode = PasteSalesOrder(proxy);
                }
                else
                {
                    throw new ArgumentException("Invalid clipboard data format {0}".FormatWith(format), "format");
                }

                base.Nodes.Add(newNode);
                newNode.Select();
                newNode.BringIntoView();
                return newNode;
            }

            private OrderNode PasteWorkOrder(DataRowProxy proxy)
            {
                //Only Keep the following relations
                var validRelations = new List<string>
                {
                    "FK_OrderProcesses_Order",
                    "FK_OrderFees_Order",
                    "FK_Order_Media_Order",
                    "FK_OrderCustomFields_Order",
                    "FK_OrderPartMark_Order",
                    "FK_OrderProductClass_Order",
                    "FK_OrderWorkDescription_Order",
                };

                //remove rows that shouldn't be copied
                proxy.ChildProxies = proxy.ChildProxies.Where(prox => validRelations.Contains(prox.ParentRelation)).ToList();

                //set dates for order processes to null
                var orderProcesses = proxy.ChildProxies.Where(prox => prox.ParentRelation == "FK_OrderProcesses_Order");

                using (var taProcessRequisite = new Data.Datasets.ProcessesDatasetTableAdapters.ProcessRequisiteTableAdapter())
                {
                    var pastProcesses = new HashSet<int>();

                    foreach(var orderProcess in orderProcesses)
                    {
                        var startDate = orderProcess.ColumnNameArray.IndexOf(c => c == "StartDate");
                        orderProcess.ItemArray[startDate] = null;
                        var endDate = orderProcess.ColumnNameArray.IndexOf(c => c == "EndDate");
                        orderProcess.ItemArray[endDate] = null;
                        var estShipDate = orderProcess.ColumnNameArray.IndexOf(c => c == "EstEndDate");
                        orderProcess.ItemArray[estShipDate] = null;

                        var partCount = orderProcess.ColumnNameArray.IndexOf(c => c == "PartCount");
                        orderProcess.ItemArray[partCount] = null;

                        var processIDColumn = orderProcess.ColumnNameArray.IndexOf(c => c == "ProcessID");
                        var processID = Convert.ToInt32(orderProcess.ItemArray[processIDColumn]);

                        var requisiteHoursColumn = orderProcess.ColumnNameArray.IndexOf(c => c == "RequisiteHours");

                        // Update RequisiteHours column based on current ProcessRequisite values.
                        if (pastProcesses.Count > 0)
                        {
                            using (var dtProcessRequisite = new ProcessesDataset.ProcessRequisiteDataTable())
                            {
                                taProcessRequisite.FillByParent(dtProcessRequisite, processID);
                                var matchingProcessReq = dtProcessRequisite
                                    .Where(p => pastProcesses.Contains(p.ChildProcessID))
                                    .OrderByDescending(r => r.Hours)
                                    .FirstOrDefault();

                                orderProcess.ItemArray[requisiteHoursColumn] = matchingProcessReq?.Hours;
                            }
                        }
                        else
                        {
                            orderProcess.ItemArray[requisiteHoursColumn] = null;
                        }

                        //update order process's answers
                        orderProcess.ChildProxies.Clear();

                        pastProcesses.Add(processID);
                    }
                }

                var or      = DataNode<DataRow>.AddPastedDataRows(proxy, this._dataset.Order) as OrdersDataSet.OrderRow;

                //update all OrderID's on OrderProcessAnswers and Part Inspections
                or.GetOrderProcessesRows().ForEach(op => op.GetOrderProcessAnswerRows().ForEach(opa => opa.OrderID = or.OrderID));
                or.GetOrderProcessesRows().ForEach(op => op.GetPartInspectionRows().ForEach(pi => pi.OrderID = or.OrderID));
                
                or.OrderDate        = DateTime.Now;
                or.Status           = Properties.Settings.Default.OrderStatusOpen;
                or.CreatedBy        = SecurityManager.Current.UserID;
                or.ContractReviewed = false;
                or.WorkStatus       = OrderControllerExtensions.GetNewOrderWorkStatus(or.CustomerID, SecurityManager.Current.CurrentUser == null || SecurityManager.Current.CurrentUser.RequireOrderReview);

                // Remove part marking dates
                foreach (var partMark in or.GetOrderPartMarkRows())
                {
                    partMark.SetPartMarkedDateNull();
                }

                // In case order is closed
                or.SetCompletedDateNull();
                or.SetInvoiceNull();
                or.SetReceivingIDNull();
                
                or.OrderType    = (int)OrderType.Normal;
                or.CurrentLocation  = ApplicationSettings.Current.DepartmentSales;

                return new OrderNode(or, OrderEntryMode.Normal);
            }

            private SalesOrderNode PasteSalesOrder(DataRowProxy proxy)
            {
                var orderNodes = new List<OrderNode>();
                var orders = proxy.ChildProxies.Where(prox => prox.ParentRelation == "FK_Order_SalesOrder");
                foreach (var order in orders)
                {
                    var orderNode = PasteWorkOrder(order);
                    orderNodes.Add(orderNode);
                }

                var validRelations = new HashSet<string>()
                {
                    "FK_SalesOrder_Media_SalesOrder"
                };

                proxy.ChildProxies = proxy.ChildProxies
                    .Where(childProxy => validRelations.Contains(childProxy.ParentRelation))
                    .ToList();

                var dr = DataNode<DataRow>.AddPastedDataRows(proxy, this._dataset.SalesOrder) as OrdersDataSet.SalesOrderRow;

                dr.SetCompletedDateNull();
                dr.SetInvoiceNull();
                dr.OrderDate = DateTime.Now;
                dr.CreatedBy = SecurityManager.Current.UserID;
                dr.Status = Properties.Settings.Default.OrderStatusOpen;

                var salesOrderNode = new SalesOrderNode(dr, this._dataset);

                foreach (var orderNode in orderNodes)
                {
                    orderNode.DataRow.SalesOrderID = dr.SalesOrderID;
                    salesOrderNode.Nodes.Add(orderNode);
                }

                return salesOrderNode;
            }

            public bool CanPasteData(string format)
            {
                return format == typeof(OrderNode).FullName ||
                    format == typeof(SalesOrderNode).FullName;
            }

            public string ClipboardDataFormat
            {
                get { return null; }
            }

            #endregion
        }

        #endregion

        #region Nested type: PartMarkNode

        private class PartMarkNode : DataNode<OrdersDataSet.OrderPartMarkRow>, ISortByDate
        {
            private const string KEY_PREFIX = "PM";

            #region Methods

            public PartMarkNode(OrdersDataSet.OrderPartMarkRow cr)
                : base(cr, cr.OrderPartMarkID.ToString(), KEY_PREFIX, "Part Mark")
            {
                Override.NodeAppearance.Image = "PartMark";

                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                if (base.DataRow.RowState == DataRowState.Deleted)
                    return;

                if(!base.DataRow.IsPartMarkedDateNull())
                    Text = "Part Mark - " + base.DataRow.PartMarkedDate.ToShortDateString();
                else
                    Text = "Part Mark Template";
            }

            #endregion Methods

            #region ISortByDate Members

            public DateTime? SortByDate
            {
                get
                {
                    if(DataRow != null && DataRow.RowState != DataRowState.Deleted && !base.DataRow.IsPartMarkedDateNull())
                        return base.DataRow.PartMarkedDate.AddMinutes(-2);
                    else
                        return null;
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: ShipmentNode

        private class ShipmentNode : DataNode<OrdersDataSet.OrderShipmentRow>, IReportNode, ISortByDate
        {
            private const string KEY_PREFIX = "OS";
            private readonly OrdersDataSet _dsOrders;

            #region Properties

            public override bool CanDelete
            {
                get { return false; }
            }

            #endregion Properties

            #region Methods

            public ShipmentNode(OrdersDataSet.OrderShipmentRow cr, OrdersDataSet dsOrders)
                : base(cr, cr.ShipmentID.ToString(), KEY_PREFIX, string.Empty)
            {
                if (dsOrders == null)
                {
                    throw new ArgumentNullException(nameof(dsOrders));
                }

                _dsOrders = dsOrders;
                Override.NodeAppearance.Image = "ShippingTruck";

                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                Text = DataRow.IsShipmentPackageIDNull()
                    ? $@"Shipment - {DataRow.DateShipped.ToShortDateString()}"
                    : $@"Shipment {DataRow.ShipmentPackageID} - {DataRow.DateShipped.ToShortDateString()}";
            }

            #endregion Methods

            #region IReportNode Members

            public IReport CreateReport(string reportType)
            {
                if (reportType == "Statement of Repairs")
                {
                    var reportData = RepairStatementData.GetReportData(
                        DataRow.ShipmentPackageID,
                        _dsOrders,
                        SecurityManager.Current);

                    return new RepairsStatementsReport(reportData);
                }

                OrderShipmentDataSet.OrderShipmentRow[] rows;
                using(var taOS = new Data.Datasets.OrderShipmentDataSetTableAdapters.OrderShipmentTableAdapter())
                {
                    OrderShipmentDataSet.OrderShipmentDataTable orderShipments = null;

                    //Get all orders with same shipment package id
                    if(!base.DataRow.IsShipmentPackageIDNull())
                        orderShipments = taOS.GetShipmentsByShipmentPackageID(base.DataRow.ShipmentPackageID);
                        //Get all orders with same PackingSlipID
                    else if(!base.DataRow.IsPackingSlipIDNull())
                        orderShipments = taOS.GetShipmentsByPackingSlipID(base.DataRow.PackingSlipID);
                    else //if no packing slip id then try to get all orders that shipped by same customer at same time
                        orderShipments = taOS.GetShipmentsByCustomerDate(base.DataRow.OrderRow.CustomerID, base.DataRow.DateShipped);

                    rows = orderShipments.Select() as OrderShipmentDataSet.OrderShipmentRow[];
                }

                OrderShipmentDataSet.ShipmentPackageRow shipment;

                using (var taShipment = new Data.Datasets.OrderShipmentDataSetTableAdapters.ShipmentPackageTableAdapter())
                {
                    shipment = taShipment.GetByShipmentPackageID(DataRow.ShipmentPackageID).FirstOrDefault();
                }

                return new PackingListReport(shipment, rows);
            }

            public string[] ReportTypes()
            {
                if (ApplicationSettings.Current.RepairStatementEnabled)
                {
                    return new[]
                    {
                        "Packing List",
                        "Statement of Repairs"
                    };
                }
                return null;
            }

            #endregion

            #region ISortByDate Members

            public DateTime? SortByDate
            {
                get
                {
                    if(DataRow != null && DataRow.RowState != DataRowState.Deleted)
                        return base.DataRow.DateShipped;
                    else
                        return null;
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: ContainerNode

        private class ContainerNode : UltraTreeNode, IDataNode, IDeleteNode, IReportNode
        {
            private const string KEY_PREFIX = "CON";

            #region Properties

            public bool CanDelete
            {
                get { return true; }
            }

            public OrdersDataSet.OrderRow OrderRow { get; set; }

            public DataRow DataRow
            {
                get { return this.OrderRow; }
            }

            public bool HasChanges
            {
                get { return DataRow != null && DataRow.RowState != DataRowState.Unchanged; }
            }

            public bool IsRowValid
            {
                get { return this.DataRow != null && this.DataRow.RowState != DataRowState.Deleted && this.DataRow.RowState != DataRowState.Detached; }
            }

            public string SortKey => "Z" + Key;

            #endregion Properties

            #region Methods

            public ContainerNode(OrdersDataSet.OrderRow cr)
                : base(KEY_PREFIX + "-" + cr.OrderID.ToString(), "Container")
            {
                this.OrderRow = cr;
                Override.NodeAppearance.Image = "Container";
                this.UpdateNodeUI();
            }

            public void UpdateNodeUI()
            {
                Text = "Containers [" + OrderRow.GetOrderContainersRows().Count() + "]";
            }

            public void Delete()
            {
                if (this.OrderRow != null)
                {
                    foreach (var row in OrderRow.GetOrderContainersRows())
                    {
                        if(row.IsValidState())
                            row.Delete();
                    }
                }
                
                Remove();
            }

            #endregion Methods

            #region IReportNode Members

            public IReport CreateReport(string reportType)
            {
                IReport returnReport = null;

                if (reportType == "COC Container Label")
                {
                    int? selectedCOC = null;

                    if (this.OrderRow.GetCOCRows().Count() > 1)
                    {
                        // Select from list
                        using (var cocForm = new ComboBoxForm())
                        {
                            cocForm.Text = "COC";

                            foreach (var coc in this.OrderRow.GetCOCRows().OrderBy(i => i.COCID))
                            {
                                cocForm.ComboBox.Items.Add(coc.COCID.ToString());
                            }

                            cocForm.ComboBox.SelectedIndex = 0;
                            cocForm.FormLabel.Text = "COC to Print:";

                            if (cocForm.ShowDialog(ActiveForm) == DialogResult.OK)
                            {
                                NullableParser.TryParse(cocForm.ComboBox.Text, out selectedCOC);
                            }
                        }
                    }
                    else
                    {
                        selectedCOC = this.OrderRow.GetCOCRows().First().COCID;
                    }

                    if (selectedCOC.HasValue)
                    {
                        returnReport = new COCContainerLabelReport()
                        {
                            COCId = selectedCOC.Value,
                            OrderId = this.OrderRow.OrderID
                        };
                    }
                }
                else
                {
                    returnReport = new ContainerLabelReport() { Order = this.OrderRow };
                    
                    //((ILabelReport)returnReport)?.PrintLabel();
                }

                return returnReport;
            }

            public string[] ReportTypes()
            {
                if (this.OrderRow.GetCOCRows().Count() > 0)
                {
                    return new[] { "Container Label", "COC Container Label" };
                }
                else
                {
                    return new[] { "Container Label" };
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: OrderNoteNode

        private class OrderNoteNode : DataNode<OrdersDataSet.OrderNoteRow>, ISortByDate
        {
            private const string KEY_PREFIX = "ORDERNOTE";

            #region Properties

            public override bool CanDelete
            {
                get { return true; }
            }

            #endregion Properties

            #region Methods

            public OrderNoteNode(OrdersDataSet.OrderNoteRow cr)
                : base(cr, cr.OrderNoteID.ToString(), KEY_PREFIX, string.Empty)
            {
                Override.NodeAppearance.Image = "Note_16"; 

                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                if (DataRow != null && DataRow.RowState != DataRowState.Deleted)
                {
                    Text = "Note {0} - {1}".FormatWith(base.DataRow.NoteType, base.DataRow.TimeStamp.ToShortDateString());
                }
            }

            #endregion Methods
            
            #region ISortByDate Members

            public DateTime? SortByDate
            {
                get
                {
                    if (DataRow != null && DataRow.RowState != DataRowState.Deleted)
                        return base.DataRow.TimeStamp;
                    else
                        return null;
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: BatchNode

        private class BatchNode : DataNode<OrdersDataSet.BatchRow>
        {
            private const string KEY_PREFIX = "BATCH";

            #region Properties

            public override bool CanDelete
            {
                get { return false; }
            }

            #endregion Properties

            #region Methods

            public BatchNode(OrdersDataSet.OrderRow order, OrdersDataSet.BatchRow cr)
                : base(cr, order.OrderID + "-" + cr.BatchID.ToString(), KEY_PREFIX, string.Empty)
            {
                Override.NodeAppearance.Image = "Batch"; 

                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                if (DataRow != null && DataRow.RowState != DataRowState.Deleted)
                {
                    Text = "Batch {0}".FormatWith(base.DataRow.BatchID);
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: LaborNode

        private class LaborNode : DataNode<OrdersDataSet.OrderRow>
        {
            private const string KEY_PREFIX = "LABOR";

            #region Properties

            public override bool CanDelete
            {
                get { return false; }
            }

            #endregion Properties

            #region Methods

            public LaborNode(OrdersDataSet.OrderRow order)
                : base(order, order.OrderID.ToString(), KEY_PREFIX, string.Empty)
            {
                Override.NodeAppearance.Image = "Labor";
                Text = "Labor";

                this.UpdateNodeUI();
            }

            #endregion
        }

        #endregion

        #region NestedType: BulkCOCNode

        private class BulkCOCNode : DataNode<OrdersDataSet.BulkCOCOrderRow>, IReportNode
        {
            #region Fields

            private const string KEY_PREFIX = "BULK_COC";

            #endregion

            #region Properties

            public override bool CanDelete
            {
                get { return false; }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the <see cref="BulkCOCNode"/> class.
            /// </summary>
           /// <param name="bulkCOCOrder"></param>
            public BulkCOCNode(OrdersDataSet.BulkCOCOrderRow bulkCOCOrder)
                : base(bulkCOCOrder, bulkCOCOrder.BulkCOCOrderID.ToString(), KEY_PREFIX, string.Empty)
            {
                Override.NodeAppearance.Image = "Cert3";

                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                if (base.DataRow.BulkCOCRow == null)
                {
                    Text = "Bulk Cert " + base.DataRow.BulkCOCID;
                }
                else
                {
                    Text = "Bulk Cert " + base.DataRow.BulkCOCID + " - " +
                        base.DataRow.BulkCOCRow.DateCertified.ToShortDateString();
                }
            }

            #endregion

            #region IReportNode Members

            public IReport CreateReport(string reportType)
            {
                return new BulkCOCReport(base.DataRow.BulkCOCID);
            }

            public string[] ReportTypes()
            {
                return new []
                {
                    "Bulk Certificate"
                };
            }

            #endregion
        }

        #endregion

        #region NestedType: BulkCOCNode

        private class BatchCocNode : DataNode<OrdersDataSet.BatchCOCOrderRow>, IReportNode
        {
            #region Fields

            private const string KEY_PREFIX = "BATCH_COC";

            #endregion

            #region Properties

            public override bool CanDelete => false;

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the <see cref="BulkCOCNode"/> class.
            /// </summary>
            /// <param name="bulkCOCOrder"></param>
            public BatchCocNode(OrdersDataSet.BatchCOCOrderRow batchCocOrder)
                : base(batchCocOrder, batchCocOrder.BatchCOCOrderID.ToString(), KEY_PREFIX, string.Empty)
            {
                Override.NodeAppearance.Image = "Cert3";

                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                if (DataRow.BatchCOCRow == null)
                {
                    Text = "Batch Cert " + DataRow.BatchCOCID;
                }
                else
                {
                    Text = "Batch Cert " + DataRow.BatchCOCID + " - " +
                        DataRow.BatchCOCRow.DateCertified.ToShortDateString();
                }
            }

            #endregion

            #region IReportNode Members

            public IReport CreateReport(string reportType)
            {
                try
                {
                    var batchCoc = DataRow.BatchCOCRow;

                    if (batchCoc != null)
                    {
                        return BatchCocReport.From(batchCoc);
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error creating Batch Certificate printout.", exc);
                }

                return null;
            }

            public string[] ReportTypes()
            {
                return new[]
                {
                    "Batch Certificate"
                };
            }

            #endregion
        }

        #endregion

        #region NestedType: BillOfLadingNode

        private class BillOfLadingNode : DataNode<OrdersDataSet.BillOfLadingOrderRow>, IReportNode
        {
            #region Fields

            private const string KEY_PREFIX = "BOL_";

            #endregion

            #region Properties

            public override bool CanDelete => false;

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the <see cref="BillOfLadingNode"/> class.
            /// </summary>
            public BillOfLadingNode(OrdersDataSet.BillOfLadingOrderRow billOfLadingOrder)
                : base(billOfLadingOrder, billOfLadingOrder.BillOfLadingOrderID.ToString(), KEY_PREFIX, string.Empty)
            {
                Override.NodeAppearance.Image = "document";
                UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                Text = $"Bill of Lading { DataRow.BillOfLadingID }";
            }

            #endregion

            #region IReportNode Members

            public IReport CreateReport(string reportType) =>
                new BillOfLadingReport(DataRow.BillOfLadingID);

            public string[] ReportTypes() => new[]
                {
                    "Bill of Lading"
                };

            #endregion
        }

        #endregion

        #region NestedType: OrderApprovalNode

        private class OrderApprovalNode : DataNode<OrdersDataSet.OrderApprovalRow>
        {
            #region Fields

            private const string KEY_PREFIX = "OrderApproval_";

            #endregion

            #region Properties

            public override bool CanDelete => DataRow.IsValidState()
                && DataRow.Status == nameof(OrderApprovalStatus.Pending);

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the <see cref="OrderApprovalNode"/> class.
            /// </summary>
            public OrderApprovalNode(OrdersDataSet.OrderApprovalRow orderApprovalRow)
                : base(orderApprovalRow, orderApprovalRow.OrderApprovalID.ToString(), KEY_PREFIX, string.Empty)
            {
                Override.NodeAppearance.Image = "document";
                UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                Text = $"Approval { DataRow.OrderApprovalID }";
            }

            #endregion
        }

        #endregion

        #region Nested Type: HoldNode

        private class HoldNode : DataNode<OrdersDataSet.OrderHoldRow>, IReportNode
        {
            #region Fields

            private const string KEY_PREFIX = "Hold";

            #endregion

            #region Methods

            public HoldNode(OrdersDataSet.OrderHoldRow cr)
                : base(cr, cr.OrderHoldID.ToString(), KEY_PREFIX, string.Empty)
            {
                Override.NodeAppearance.Image = "hold";

                this.UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                base.Text = string.Format("Hold - {0}", base.DataRow.TimeIn.ToShortDateString());
            }

            #endregion

            #region IReportNode Members

            public IReport CreateReport(string reportType)
            {
                var orderRow = DataRow.OrderRow;

                return orderRow == null
                    ? null
                    : new ReworkLabelReport(orderRow, ReworkLabelReport.ReportLabelType.Hold);
            }

            public string[] ReportTypes() => new[]
                {
                    "Hold Label"
                };

            #endregion
        }

        #endregion

        #region Nested Type: SerialNumberNode

        private class SerialNumberNode : DataNode<OrdersDataSet.OrderRow>
        {
            private const string KEY_PREFIX = "SERIAL";

            #region Properties

            public override bool CanDelete => false;

            #endregion Properties

            #region Methods

            public SerialNumberNode(OrdersDataSet.OrderRow order)
                : base(order, order.OrderID.ToString(), KEY_PREFIX, string.Empty)
            {
                Override.NodeAppearance.Image = "Serial";
                Text = "Serial Number History";
            }

            #endregion
        }
        #endregion

        #endregion Nodes

        #region Commands

        #region Nested type: AddCommunicationCommand

        private class AddCommunicationCommand : TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddNode;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get
                {
                    var orderNode = _node as OrderNode;

                    return orderNode != null &&
                        orderNode.IsRowValid &&
                        orderNode.DataRow.Status == Properties.Settings.Default.OrderStatusOpen;
                }
            }

            #endregion Properties

            #region Methods

            public AddCommunicationCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                if(this.AddNode != null)
                    this.AddNode(this, EventArgs.Empty);
            }

            public override void Dispose()
            {
                AddNode = null;

                base.Dispose();
            }

            #endregion Methods
        }

        #endregion

        #region Nested type: AddOrderNoteCommand

        private class AddOrderNoteCommand : TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddNode;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get { return _node is OrderNode && ((OrderNode)_node).IsRowValid; }
            }

            #endregion Properties

            #region Methods

            public AddOrderNoteCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                if (this.AddNode != null)
                    this.AddNode(this, EventArgs.Empty);
            }

            public override void Dispose()
            {
                AddNode = null;

                base.Dispose();
            }

            #endregion Methods
        }

        #endregion

        #region Nested type: AddOrderCommand

        private class AddOrderCommand : TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddNode;
            public Func<bool> IsValidToAdd;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get { return this.IsValidToAdd != null && this.IsValidToAdd(); }
            }

            #endregion Properties

            #region Methods

            public AddOrderCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                if(this.AddNode != null)
                    this.AddNode(this, EventArgs.Empty);
            }

            public override void Dispose()
            {
                AddNode = null;
                IsValidToAdd = null;

                base.Dispose();
            }

            #endregion Methods
        }

        #endregion

        #region Nested type: AddBlanketPOCommand

        private class AddBlanketPOCommand : TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddNode;
            public Func<bool> IsValidToAdd;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get { return base.Enabled && (this.IsValidToAdd?.Invoke() ?? false); }
            }

            #endregion Properties

            #region Methods

            public AddBlanketPOCommand(ToolBase tool, UltraTree toc)
                : base(tool, "BlanketPOManager.Edit")
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                if (this.AddNode != null)
                    this.AddNode(this, EventArgs.Empty);
            }

            public override void Dispose()
            {
                AddNode = null;
                IsValidToAdd = null;

                base.Dispose();
            }

            #endregion Methods
        }

        #endregion

        #region Nested type: AddOrderContainerCommand

        private class AddOrderContainerCommand : TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddNode;
            private OrderEntry _orderEntry;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get
                {
                    return _orderEntry.StatusFilter.Text == Properties.Settings.Default.OrderStatusOpen &&
                        _node is OrderNode &&
                        _node.Nodes.FindNode<ContainerNode>(n => n.IsRowValid) == null;
                }
            }

            #endregion Properties

            #region Methods

            public AddOrderContainerCommand(ToolBase tool, UltraTree toc, OrderEntry oe)
                : base(tool)
            {
                base.TreeView = toc;
                _orderEntry = oe;
            }

            public override void OnClick()
            {
                if (this.AddNode != null)
                    this.AddNode(this, EventArgs.Empty);


            }

            public override void Dispose()
            {
                AddNode = null;
                _orderEntry = null;

                base.Dispose();
            }

            #endregion Methods
        }

        #endregion

        #region Nested type: SalesOrderCommand

        private class SalesOrderCommand : TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddNode;
            public Func<bool> IsValidToAdd;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get { return this.IsValidToAdd?.Invoke() ?? false; }
            }

            #endregion Properties

            #region Methods

            public SalesOrderCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                if (this.AddNode != null)
                    this.AddNode(this, EventArgs.Empty);
            }

            public override void Dispose()
            {
                AddNode = null;
                IsValidToAdd = null;

                base.Dispose();
            }

            #endregion Methods
        }

        private class MoveWorkOrderCommand : TreeNodeCommandBase
        {
            #region Fields

            public EventHandler MoveNode;
            public Func<bool> IsValidToAdd;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get { return this.IsValidToAdd?.Invoke() ?? false; }

            }


            #endregion Properties

            #region Methods

            public MoveWorkOrderCommand(ToolBase tool, UltraTree toc): base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                try
                {
                    if (this.MoveNode != null)
                        this.MoveNode(this, EventArgs.Empty);

                    var frmMoveOrder = new DWOS.UI.Sales.Dialogs.MoveWorkOrder();
                    frmMoveOrder.orderRow = ((OrderNode)_node).DataRow;

                    if (frmMoveOrder.ShowDialog() == DialogResult.OK)
                    {
                        UltraTreeNode newParentNode = base.TreeView.Nodes[0];
                        if (!((OrderNode)_node).DataRow.IsSalesOrderIDNull())
                        {
                            var soNode = base.TreeView.FindNode((node) => { return node.Key.ToString() == $"SO-{((OrderNode)_node).DataRow.SalesOrderID.ToString()}"; });
                            newParentNode = (soNode != null) ? soNode : newParentNode;
                        }
                        //make copy of node ID
                        string oNodeID = _node.Key.ToString();
                        //Move node to new parent
                        _node.Reposition(newParentNode.Nodes);
                        //Find the node
                        UltraTreeNode oNode = base.TreeView.FindNode((node) => { return node.Key.ToString() == oNodeID; });
                        //Set WO node active
                        base.TreeView.ActiveNode = oNode;
                        oNode.Select();
                        newParentNode.Expanded = true;
                        base.TreeView.Focus();
                    }
                }
                catch (Exception exc)
                {
                    _log.Error(exc, "Error Moving Order");
                }
            }



            public override void Dispose()
            {
                MoveNode = null;
                IsValidToAdd = null;

                base.Dispose();
            }

            #endregion Methods
        }

        #endregion

        #region Nested type: AddPartMarkCommand

        private class AddPartMarkCommand : TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddNode;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get
                {
                    //if no other template for this order
                    var orderNode = _node as OrderNode;

                    return orderNode != null &&
                        orderNode.DataRow.PartSummaryRow != null &&
                        !orderNode.DataRow.GetOrderPartMarkRows().Any() &&
                        orderNode.DataRow.Status == Properties.Settings.Default.OrderStatusOpen;
                }
            }

            #endregion Properties

            #region Methods

            public AddPartMarkCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                if(this.AddNode != null)
                    this.AddNode(this, EventArgs.Empty);
            }

            public override void Dispose()
            {
                AddNode = null;

                base.Dispose();
            }

            #endregion Methods
        }

        #endregion

        #region Nested type: BulkFieldChangeCommand

        private class BulkFieldChangeCommand : TreeNodeCommandBase
        {
            #region Fields

            private OrderInformation _dpOrder;
            private OrdersDataSet _dsOrders;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get { return base.TreeView.SelectedNodes.Count > 0; }
            }

            #endregion Properties

            #region Methods

            public BulkFieldChangeCommand(ToolBase tool, UltraTree toc, OrdersDataSet dsOrders, OrderInformation dpOrder)
                : base(tool)
            {
                this._dsOrders = dsOrders;
                this._dpOrder = dpOrder;
                base.TreeView = toc;

                base.AllowMultipleSelection = true;
            }

            public override void OnClick()
            {
                try
                {
                    var orders = TreeView.SelectedNodes.OfType<OrderNode>().ToList();
                    if (orders.Count <= 0)
                    {
                        return;
                    }

                    using(var frm = new OrderBulkFieldChange())
                    {
                        var orderRows = orders.ConvertAll(n => n.DataRow).ToArray();
                        frm.LoadData(this._dsOrders, orderRows);

                        if(frm.ShowDialog(ActiveForm) == DialogResult.OK)
                        {
                            // Update fields
                            foreach (OrdersDataSet.OrderRow order in orderRows)
                            {
                                if (order == null)
                                {
                                    continue;
                                }

                                if(frm.POChanged)
                                    order.PurchaseOrder = frm.POValue;
                                if(frm.OrderDateChanged)
                                    order.OrderDate = frm.OrderDateValue;
                                if (frm.ShipDateChanged)
                                    order.EstShipDate = frm.EstShipValue;
                                if(frm.ReqDateChanged)
                                    order.RequiredDate = frm.ReqDateValue;
                                if(frm.CustomerWOChanged)
                                    order.CustomerWO = frm.CustomerWOValue;
                                if(frm.PriorityChanged)
                                    order.Priority = frm.PriorityValue;
                                if(frm.PartQtyChanged)
                                    order.PartQuantity = frm.PartQtyValue;
                                if(frm.UnitPriceChanged)
                                    order.BasePrice = frm.UnitPriceValue;
                            }

                            // Update PO Document
                            try
                            {
                                const int maxExtensionLength = 10;
                                const int maxFileNameLength = 50;

                                if (frm.PODocChanged && !string.IsNullOrEmpty(frm.PODocFilePathValue))
                                {
                                    var fileName = System.IO.Path.GetFileName(frm.PODocFilePathValue);
                                    var name = System.IO.Path.GetFileNameWithoutExtension(fileName).TrimToMaxLength(maxFileNameLength);
                                    var fName = System.IO.Path.GetFileName(fileName).TrimToMaxLength(maxFileNameLength);
                                    var ext = System.IO.Path.GetExtension(fileName).Replace(".", "");
                                    if (ext.Length > maxExtensionLength)
                                    {
                                        // Cannot save - file extension is too long
                                        MessageBoxUtilities.ShowMessageBoxError(
                                            $"Unable to save a file with an extension that's longer than {maxExtensionLength} characters in length.\nPlease rename the file and try again.",
                                            "Media");
                                    }
                                    else
                                    {
                                        //Add the actual media only once
                                        byte[] b = MediaUtilities.CreateMediaStream(frm.PODocFilePathValue);

                                        if (b == null || b.Length == 0)
                                        {
                                            MessageBoxUtilities.ShowMessageBoxError(
                                                "Unable to save blank document.",
                                                "Media");
                                        }
                                        else
                                        {
                                            _log.Info($"Adding new media named '{fileName}'");
                                            var mr = this._dsOrders.Media.AddMediaRow(name, fName, ext, b);

                                            this._dpOrder.AddMediaToDelete(frm.PODocFilePathValue);


                                            foreach (OrdersDataSet.OrderRow order in orderRows)
                                            {
                                                if (order == null)
                                                {
                                                    continue;
                                                }

                                                //Assign the ids in the junction table
                                                this._dsOrders.Order_Media.AddOrder_MediaRow(order, mr);
                                            }

                                            if (string.IsNullOrWhiteSpace(ext))
                                            {
                                                const string extensionWarningMessage = "The file that you " +
                                                    "selected has a blank file extension." +
                                                    "\nYou may be unable to open this file.";

                                                MessageBoxUtilities.ShowMessageBoxWarn
                                                    (extensionWarningMessage,
                                                    "Media");
                                            }
                                        }
                                    }
                                }
                            }
                            catch (IOException exc)
                            {
                                if (exc.Message.Contains("being used by another process"))
                                {
                                    MessageBoxUtilities.ShowMessageBoxError(
                                        "The file that you selected is in-use. Please close all other programs using the file and try again.",
                                        "Order Entry");
                                }
                                else
                                {
                                    throw;
                                }

                            }
                        }
                    }
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error in bulk field change command.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }

            public override void Dispose()
            {
                this._dpOrder = null;
                this._dsOrders = null;

                base.Dispose();
            }

            #endregion Methods
        }

        #endregion

        #region Nested type: RemoveFromSalesOrderCommand

        private class RemoveFromSalesOrderCommand : TreeNodeCommandBase
        {
            #region Fields

            /// <summary>
            /// Occurs when this instance removes at least one Work Order from
            /// its Sales Order.
            /// </summary>
            /// <remarks>
            /// The event returns an enumerable of order IDs.
            /// </remarks>
            public event EventHandler<EventArgsTemplate<IList<int>>> RemovedOrdersFromSalesOrder;

            #endregion

            #region Properties

            public OrderEntry OrderEntry { get; }

            public override bool Enabled
            {
                get
                {
                    if (!IsAuthorized() || TreeView == null)
                    {
                        return false;
                    }

                    var selectedOrderNodes = TreeView.SelectedNodes
                        .OfType<OrderNode>()
                        .ToList();

                    return selectedOrderNodes.Count == TreeView.SelectedNodes.Count
                        && selectedOrderNodes.Count > 0
                        && selectedOrderNodes.All(node => node.CanDelete && node.DataRow.IsValidState() && !node.DataRow.IsSalesOrderIDNull());
                }
            }

            #endregion

            #region Methods

            public RemoveFromSalesOrderCommand(ToolBase tool, UltraTree tvwTOC, OrderEntry orderEntry)
                : base(tool, "OrderEntry.OrderDelete")
            {
                TreeView = tvwTOC;
                OrderEntry = orderEntry;
            }

            public override void OnClick()
            {
                var rootNode = OrderEntry.RootNode;

                if (rootNode == null)
                {
                    _log.Warn("Root node not found - cannot remove work orders from sales order.");
                    return;
                }

                var workOrderNodes = TreeView.SelectedNodes
                    .OfType<OrderNode>()
                    .ToList();

                var removedOrderIds = new List<int>();

                foreach (var workOrderNode in workOrderNodes)
                {
                    var workOrderRow = workOrderNode.DataRow;
                    if (!(workOrderNode.Parent is SalesOrderNode parentNode) || !workOrderRow.IsValidState())
                    {
                        _log.Warn("Cannot move order - is not part of a sales order or otherwise invalid.");
                        continue;
                    }

                    parentNode.Nodes.Remove(workOrderNode);
                    workOrderRow.SetSalesOrderIDNull();
                    OrderEntry.tvwTOC.Nodes[0].Nodes.Add(workOrderNode);

                    // Re-select work order - otherwise, selection is cleared
                    workOrderNode.Selected = true;

                    removedOrderIds.Add(workOrderRow.OrderID);
                }

                if (removedOrderIds.Count > 0)
                {
                    RemovedOrdersFromSalesOrder?.Invoke(this,
                        new EventArgsTemplate<IList<int>>(removedOrderIds));
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: ImportFromQuoteCommand

        private class ImportFromQuoteCommand : TreeNodeCommandBase
        {
            #region Fields

            //These are our provided set of callbacks/ data for the command from our 'parent' Order Entry
            private OrdersDataSet _orders;
            public Action<int, int?, int?, int?> AddNode;
            public Func<bool> IsCurrentNodeValid;
            public Func<bool> IsValidToAdd;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get { return this.IsValidToAdd?.Invoke() ?? false; }
            }

            #endregion Properties

            #region Methods

            public ImportFromQuoteCommand(ToolBase tool, OrdersDataSet orders, UltraTree toc)
               : base(tool)
            {
                this._orders = orders;
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                //if current not is not valid then don't import 
                if (!(this.IsCurrentNodeValid?.Invoke() ?? false))
                    return;

                var topLevelOrdersNodeSelected = TreeView.Nodes[0].Selected;

                string customerName = string.Empty;
                SalesOrderNode selectedSalesOrderNode = null;
                SalesOrderNode parentSalesOrderNode = null;

                if (!topLevelOrdersNodeSelected) //Check which node is selected then
                {
                    //If user is on a sales order node, then get customer name from it
                    selectedSalesOrderNode = TreeView.SelectedNode<SalesOrderNode>();
                    if (selectedSalesOrderNode != null)
                    {
                        customerName = selectedSalesOrderNode.DataRow.CustomerSummaryRow.Name;
                        if (string.IsNullOrWhiteSpace(customerName))
                        {
                            _log.Error(
                                $"Error trying to import from quote under sales order {selectedSalesOrderNode.ID}");
                            return;
                        }
                    }
                    else //Not on sales node, check children
                    {
                        var selectedWorkOrderNode = TreeView.SelectedNode<OrderNode>();
                        if (selectedWorkOrderNode == null)
                        {
                            MessageBoxUtilities.ShowMessageBoxWarn(
                                "An Order must be selected to import from quote.", "No Order Found");
                            return;
                        }

                        //If we are a WO under a sales order, pass customer name to ImportQuoteToOE dialog
                        parentSalesOrderNode = selectedWorkOrderNode.Parent as SalesOrderNode;
                        if (parentSalesOrderNode != null)
                        {
                            customerName = parentSalesOrderNode.DataRow.CustomerSummaryRow.Name;
                            if (string.IsNullOrWhiteSpace(customerName))
                            {
                                _log.Error(
                                    $"Error trying to import from quote under sales order {parentSalesOrderNode.ID}");
                                return;
                            }
                        }
                    }
                }

                using (var importQuote = new ImportQuoteToOE(customerName))
                {
                    if (importQuote.ShowDialog() == DialogResult.OK && importQuote.SelectedQuotePart != null)
                    {
                        var quotePartId = importQuote.SelectedQuotePart.QuotePartID;

                        //Check if we need to add to SO
                        int? salesOrderId = null;
                        if (parentSalesOrderNode != null)
                            salesOrderId = int.Parse(parentSalesOrderNode.ID);
                        if (selectedSalesOrderNode != null)
                            salesOrderId = int.Parse(selectedSalesOrderNode.ID);

                        this.AddNode(quotePartId, salesOrderId, null, null);
                    }
                }
            }

            public override void Dispose()
            {
                _orders = null;
                AddNode = null;
                IsCurrentNodeValid = null;
                IsValidToAdd = null;

                base.Dispose();
            }

            #endregion
        }
        #endregion

        #region Nested type: ImportFromReceivingCommand

        private class ImportFromReceivingCommand : TreeNodeCommandBase
        {
            #region Fields

            private OrdersDataSet _orders;
            public Action<int, int, bool> AddNode;
            public Func<bool> IsCurrentNodeValid;
            public Func<bool> IsValidToAdd;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get { return this.IsValidToAdd?.Invoke() ?? false; }
            }

            private ImportFromQuoteCommand ImportFromQuote { get; set; }

            #endregion Properties

            #region Methods

            public ImportFromReceivingCommand(ToolBase tool, OrdersDataSet orders, UltraTree toc, ImportFromQuoteCommand importFromQuote)
                : base(tool)
            {
                this._orders = orders;
                base.TreeView = toc;
                ImportFromQuote = importFromQuote;
            }

            public override void OnClick()
            {
                try
                {
                    //if current not is not valid then don't import 
                    if(!(this.IsCurrentNodeValid?.Invoke() ?? false))
                        return;

                    using(var frm = new ReceivingOrderImport())
                    {
                        //get list of all existing receiving orders being used that are new
                        var recIDCol = this._orders.Order.ReceivingIDColumn.ColumnName;
                        var rows     = this._orders.Order.Select(recIDCol + " IS NOT NULL", recIDCol, DataViewRowState.Added);
                        var recIDs   = new List<int>();

                        foreach(DataRow item in rows)
                        {
                            if(!item.IsNull(recIDCol))
                                recIDs.Add(Convert.ToInt32(item[recIDCol]));
                        }

                        //If user is on a sales order node, then pass the id to the import dialog
                        var selectedSalesNode = TreeView.SelectedNode<SalesOrderNode>();
                        if (selectedSalesNode != null)
                            frm.NodeSalesOrderID = selectedSalesNode.DataRow.SalesOrderID;

                        frm.UsedReceivingOrders = recIDs;

                        if(frm.ShowDialog(ActiveForm) == DialogResult.OK)
                        {
                            int? receivingId = frm.SelectedReceivingOrder;
                            int? salesOrderCarryOverID = frm.AssignToSalesOrderID;
                            if (receivingId.HasValue && this.AddNode != null)
                            {
                                try
                                {
                                    //Check if we have any outstanding quotes for the receiving order selected
                                    using (var taReceiving =
                                        new Data.Datasets.PartsDatasetTableAdapters.ReceivingTableAdapter())
                                    {
                                        var receivingData = taReceiving.GetByID(receivingId.Value);
                                        
                                        if (receivingData.Rows.Count != 0)
                                        {
                                            var receivingQty = receivingData[0].PartQuantity;
                                            var receivingPartId = receivingData[0].PartID;
                                            var customerId = receivingData[0].CustomerID;
                                            using (var taPart =
                                                new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
                                            {
                                                var pt = new PartsDataset.PartDataTable();
                                                taPart.FillByPartID(pt, receivingPartId);
                                                if (pt.Rows.Count != 0)
                                                {
                                                    var receivingPartName = pt[0].Name;
                                                    using (var taQuote =
                                                        new Data.Datasets.QuoteDataSetTableAdapters.QuoteTableAdapter())
                                                    {
                                                        var activeQuotes =
                                                            taQuote.GetActiveQuotesByCustomerAndPart(customerId,
                                                                receivingPartName);
                                                        if (activeQuotes.Any())
                                                        {
                                                            var res = MessageBoxUtilities.ShowMessageBoxYesOrNo(
                                                                $"{activeQuotes.Rows.Count} active quote(s) found for the selected part, would you like to import from quote instead?",
                                                                "Active Quotes Found");
                                                            if (res == DialogResult.Yes)
                                                            {
                                                                var customerName = "";
                                                                using (var taCustomer =
                                                                    new Data.Datasets.CustomersDatasetTableAdapters.
                                                                        CustomerTableAdapter())
                                                                {
                                                                    customerName =
                                                                        taCustomer.GetCustomerNameById(customerId);
                                                                }

                                                                var quoteIds = activeQuotes.Select(q => q.QuoteID)
                                                                    .ToList();
                                                                //Load import from quote, pass it the specified params for filtering
                                                                using (var importQuote = new ImportQuoteToOE(customerName, receivingPartName, quoteIds))
                                                                {
                                                                    if (importQuote.ShowDialog() == DialogResult.OK &&
                                                                        importQuote.SelectedQuotePart != null)
                                                                    {
                                                                        var quotePartId = importQuote.SelectedQuotePart
                                                                            .QuotePartID;
                                                                        var salesOrderId = salesOrderCarryOverID > 0
                                                                            ? salesOrderCarryOverID
                                                                            : null;
                                                                        ImportFromQuote.AddNode(quotePartId, salesOrderId, receivingId, receivingQty);
                                                                      
                                                                    }
                                                                }
                                                                return;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception exc)
                                {
                                    _log.Error(exc, $"Error checking for existing active quotes on receiving order {receivingId}");
                                }
                                //Add order from receiving without importing from quote
                                this.AddNode(receivingId.GetValueOrDefault(), salesOrderCarryOverID.GetValueOrDefault(), frm.IsRework);
                            }
                               
                            else
                                MessageBoxUtilities.ShowMessageBoxWarn("No Order Receipt selected.", "Invalid Selection");
                        }
                    }
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error filtering data.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }

            public override void Dispose()
            {
                _orders = null;
                AddNode = null;
                IsCurrentNodeValid = null;
                IsValidToAdd = null;

                base.Dispose();
            }

            #endregion Methods
        }

        #endregion

        #region Nested type: OrderHistoryCommand

        private class OrderHistoryCommand : TreeNodeCommandBase
        {
            #region Properties

            public override bool Enabled
            {
                get { return _node is OrderNode; }
            }

            #endregion Properties

            #region Methods

            public OrderHistoryCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                try
                {
                    using(var frm = new OrderHistory())
                    {
                        frm.OrderID = ((OrderNode)_node).DataRow.OrderID;
                        frm.ShowDialog(ActiveForm);
                    }
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error showing order history.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }

            #endregion Methods
        }

        #endregion

        #region Nested type: ReviseCOCCommand

        private class ReviseCOCCommand : TreeNodeCommandBase
        {
            #region Fields

            public Action<COCNode> ReviseCoc;
            public Action<BatchCocNode> ReviseBatchCoc;

            #endregion Fields

            #region Properties

            public override bool Enabled =>
                _node is COCNode
                || _node is BatchCocNode;

            #endregion Properties

            #region Methods

            public ReviseCOCCommand(ToolBase tool, UltraTree toc)
                : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                try
                {
                    if (_node is COCNode cocNode)
                    {
                        ReviseCoc?.Invoke(cocNode);
                    }
                    else if (_node is BatchCocNode batchCocNode)
                    {
                        ReviseBatchCoc?.Invoke(batchCocNode);
                    }
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error revising COC.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }

            public override void Dispose()
            {
                ReviseCoc = null;

                base.Dispose();
            }

            #endregion Methods
        }

        #endregion

        #region Nested type: ReworkOrderCommand

        private class ReworkOrderCommand : TreeNodeCommandBase
        {
            #region Fields

            public event Action ReworkNodeAdded;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get
                {
                    var orderNode = _node as OrderNode;
                    return orderNode != null &&
                        !orderNode.DataRow.IsStatusNull() &&
                        orderNode.DataRow.Status == "Closed";
                }
            }

            public OrderInformation OrderInfo
            {
                get;
                set;
            }

            #endregion Properties

            #region Methods

            public ReworkOrderCommand(ToolBase tool, UltraTree toc, OrderInformation orderInfo)
                : base(tool)
            {
                OrderInfo = orderInfo;
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                try
                {
                    using(var frm = new OrderRework())
                    {
                        //copy the order
                        var dr = ((IDataNode)_node).DataRow as OrdersDataSet.OrderRow;
                        var dt = dr.Table as OrdersDataSet.OrderDataTable;
                        var ds = dt.DataSet as OrdersDataSet;
                        
                        frm.OrderID = dr.OrderID.ToString();

                        if(frm.ShowDialog(ActiveForm) == DialogResult.OK)
                        {
                            //call to allow OrderEntry to reload open orders before adding this new order
                            this.ReworkNodeAdded?.Invoke();

                            var defaultShipDate = DateTime.Now.AddBusinessDays(ApplicationSettings.Current.OrderLeadTime);

                            //paste node to parent
                            var reworkRow                                          = CopyCommand.CopyRows(dr);
                            reworkRow.ItemArray[dt.OrderIDColumn.Ordinal]          = null; //null out order id
                            reworkRow.ItemArray[dt.BasePriceColumn.Ordinal]        = 0; //reset price
                            reworkRow.ItemArray[dt.StatusColumn.Ordinal]           = Properties.Settings.Default.OrderStatusOpen; //set status to rework
                            reworkRow.ItemArray[dt.InvoiceColumn.Ordinal]          = null; //set status to rework
                            reworkRow.ItemArray[dt.OrderDateColumn.Ordinal]        = DateTime.Now; //set status to rework
                            reworkRow.ItemArray[dt.CreatedByColumn.Ordinal]        = SecurityManager.Current.UserID;
                            reworkRow.ItemArray[dt.ContractReviewedColumn.Ordinal] = false;
                            reworkRow.ItemArray[dt.WorkStatusColumn.Ordinal]       = ApplicationSettings.Current.WorkStatusChangingDepartment;
                            reworkRow.ItemArray[dt.CurrentLocationColumn.Ordinal]  = ApplicationSettings.Current.DepartmentSales;
                            reworkRow.ItemArray[dt.OrderTypeColumn.Ordinal]         = (int)OrderType.ReworkExt;
                            reworkRow.ItemArray[dt.OriginalOrderTypeColumn.Ordinal] = (int)OrderType.ReworkExt;
                            reworkRow.ItemArray[dt.EstShipDateColumn.Ordinal]      = defaultShipDate;
                            reworkRow.ItemArray[dt.CompletedDateColumn.Ordinal]    = null; //set completed date to 

                            ApplicationSettingsDataSet.FieldsDataTable fields;

                            using (var ta = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.FieldsTableAdapter())
                            {
                                fields = ta.GetByCategory("Order");
                            }

                            var requiredDateField = fields.FirstOrDefault(f => f.Name == "Required Date");

                            if (requiredDateField?.IsVisible ?? true)
                            {
                                reworkRow.ItemArray[dt.RequiredDateColumn.Ordinal] = defaultShipDate;
                            }

                            //ensure part is active
                            using(var taParts = new PartSummaryTableAdapter())
                            {
                                taParts.UpdateActiveState(dr.PartID);
                            }

                            //add new node/row
                            var on = ((OrderRootNode)base.TreeView.Nodes[0]).PasteData(typeof(OrderNode).FullName, reworkRow) as OrderNode;

                            //Paste resets the values so reset them back
                            on.DataRow.CustomerWO   = dr.IsCustomerWONull() ? string.Empty : dr.CustomerWO;
                            on.DataRow.PartQuantity = dr.PartQuantity;
                            on.DataRow.PartID       = dr.PartID;
                            on.DataRow.OrderType    = (int)OrderType.ReworkExt;
                            on.DataRow.OrderType    = (int)OrderType.ReworkExt;

                            //add link between rework and this node
                            var changeRow           = ds.OrderChange.NewOrderChangeRow();
                            changeRow.ChangeType    = (int)OrderChangeType.ExtRework;
                            changeRow.ReasonCode    = frm.ReasonCode;
                            changeRow.ParentOrderID = dr.OrderID;
                            changeRow.ChildOrderID  = on.DataRow.OrderID;
                            changeRow.UserID        = SecurityManager.Current.UserID;
                            changeRow.DateCreated   = DateTime.Now;
                            changeRow.Notes         = frm.Notes;
                            
                            ds.OrderChange.AddOrderChangeRow(changeRow);

                            
                            OrderInfo.AddPartProcesses(on.DataRow);
                            OrderInfo.AddOrderPartMark(on.DataRow);

                            OrderHistoryDataSet.UpdateOrderHistory(dr.OrderID, "Order Entry", "Order rework created on this order.", SecurityManager.Current.UserName);

                            (base.TreeView.Nodes[0]).Select();
                            on.Select();
                        }
                    }
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error creating rework order.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }

            public override void Dispose()
            {
                ReworkNodeAdded = null;

                base.Dispose();
            }

            #endregion Methods
        }

        #endregion

        #region Nested type: SplitCommand

        private class SplitCommand : TreeNodeCommandBase
        {
            #region Fields

            private OrderEntry _orderEntry;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get
                {
                    var orderNode = _node as OrderNode;

                    return IsAuthorized() &&
                        orderNode != null &&
                        orderNode.DataRow.Status == Properties.Settings.Default.OrderStatusOpen &&
                        !orderNode.DataRow.IsPartQuantityNull() &&
                        orderNode.DataRow.OrderID > 0;
                }
            }

            #endregion Properties

            #region Methods

            public SplitCommand(ToolBase tool, UltraTree toc, OrderEntry oe)
                : base(tool, "OrderSplit")
            {
                _orderEntry = oe;
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                try
                {
                    _orderEntry.EndAllEdits();

                    var currentNode = base._node as OrderNode;

                    if (currentNode == null || !_orderEntry.IsValidControls())
                    {
                        return;
                    }

                    var orgOrder = currentNode.DataRow;

                    var orgQty = orgOrder.IsPartQuantityNull()
                        ? 0
                        : orgOrder.PartQuantity;

                    if (orgQty > 1)
                    {
                        var newOrders = SplitUtilities.DoSplit(orgOrder, _orderEntry.dsOrders, _orderEntry.taManager);

                        if (newOrders.Count > 0)
                        {
                            // Save data
                            // Otherwise, WO Travelers will show unsaved data and temporary OrderIDs.
                            _orderEntry.SaveData();

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

                            // Create nodes
                            foreach(var newOrder in newOrders)
                            {
                                var newSplitNode = new OrderNode(newOrder, OrderEntryMode.Normal);

                                //Add to sales order node otherwise just add to the tree
                                if (!newOrder.IsSalesOrderIDNull() && newOrder.SalesOrderID > 0)
                                {
                                    var salesNode = this.TreeView.Nodes.FindNode<SalesOrderNode>(snode => snode.DataRow.SalesOrderID == newOrder.SalesOrderID);
                                    if (salesNode != null)
                                        salesNode.Nodes.Add(newSplitNode);
                                }
                                else
                                {
                                    this.TreeView.Nodes[0].Nodes.Add(newSplitNode);
                                }

                                newSplitNode.Select();
                            }

                            currentNode.LoadSerialNumberNode(_orderEntry);
                            currentNode.LoadChangeNodes(_orderEntry);
                        }
                    }
                    else
                        MessageBoxUtilities.ShowMessageBoxWarn(
                            "Unable to split an order with a quantity less than 2. Please increase the quantity.",
                            "Invalid Operation");
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error splitting the order.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }

            public override void Dispose()
            {
                _orderEntry = null;

                base.Dispose();
            }

            #endregion Methods
        }

        #endregion

        #region Nested type: RejoinCommand

        private class RejoinCommand : TreeNodeCommandBase
        {
            #region Fields

            private UltraNumericEditor _qtyEditor;
            private OrderEntry _orderEntry;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get
                {
                    var orderNode = _node as OrderNode;

                    return IsAuthorized() &&
                        orderNode != null &&
                        orderNode.DataRow.Status == Properties.Settings.Default.OrderStatusOpen &&
                        !orderNode.DataRow.IsPartQuantityNull() &&
                        orderNode.DataRow.OrderID > 0;;
                }
            }

            #endregion

            #region Methods

            public RejoinCommand(ToolBase tool, UltraTree toc, UltraNumericEditor qtyEditor, OrderEntry orderEntry, string securityRole)
                : base(tool, securityRole)
            {
                _qtyEditor = qtyEditor;
                _orderEntry = orderEntry;
                TreeView = toc;
            }

            public override void OnClick()
            {
                if (!Enabled)
                {
                    return;
                }
                try
                {
                    _orderEntry.EndAllEdits();

                    var currentNode = _node as OrderNode;

                    if (currentNode != null && _orderEntry.IsValidControls() && _qtyEditor.Value != null)
                    {
                        var orgQty = Convert.ToInt32(_qtyEditor.Value);
                        // Assumption: All open orders have been loaded
                        var rejoinableOrders = _orderEntry.dsOrders.Order
                            .Where(order => RejoinUtilities.CanRejoin(order, currentNode.DataRow))
                            .ToList();

                        if (orgQty > 0 && rejoinableOrders.Count > 0)
                        {
                            Rejoin(currentNode, orgQty, rejoinableOrders);
                        }
                        else if (rejoinableOrders.Count == 0)
                        {
                            MessageBoxUtilities.ShowMessageBoxWarn("There are no Work Orders that can be rejoined to this order.", "Rejoin");
                        }
                    }
                }
                catch (Exception exc)
                {
                    var errorMsg = "Error splitting the order.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }

            private void Rejoin(OrderNode orgNode, int orgQty, List<OrdersDataSet.OrderRow> rejoinableOrders)
            {
                var orgOrder = orgNode.DataRow;

                var dialog = new RejoinOrder();
                var helper = new WindowInteropHelper(dialog) { Owner = DWOSApp.MainForm.Handle };
                dialog.Load(orgOrder, rejoinableOrders);
                dialog.PrintTraveler = UserSettings.Default.RejoinPrintTraveler;

                if (!(dialog.ShowDialog() ?? true))
                {
                    return;
                }

                UserSettings.Default.RejoinPrintTraveler = dialog.PrintTraveler;
                UserSettings.Default.Save();

                orgOrder.PartQuantity = orgQty;

                GC.KeepAlive(helper);

                // Assumption - results from dialog are valid
                //Note: Order node was never found with following statement
                //var destinationNode = TreeView.Nodes[0].Nodes.OfType<OrderNode>().FirstOrDefault(i => i.DataRow.OrderID == dialog.DestinationOrderId);
                var destinationNode = TreeView.FindNode(n => n is OrderNode && ((OrderNode)n).DataRow.OrderID == dialog.DestinationOrderId) as OrderNode;
                var destinationOrder = destinationNode?.DataRow;
                var orderChangeReason = dialog.OrderChangeReasonId;

                if (destinationNode != null && destinationOrder != null)
                {
                    var originalDestinationQuantity = destinationOrder.IsPartQuantityNull() ? 0 : destinationOrder.PartQuantity;
                    var factory = new OrderFactory();
                    factory.Load(_orderEntry.dsOrders, _orderEntry.taManager);
                    factory.Rejoin(orgOrder, destinationOrder, orderChangeReason);
                    var rejoinDate = orgOrder.IsCompletedDateNull () ? DateTime.Now : orgOrder.CompletedDate;

                    // Rejoin containers
                    foreach (var orgContainer in orgOrder.GetOrderContainersRows().Where(c => c.IsValidState() && !c.IsIsActiveNull() && c.IsActive))
                    {
                        var destContainer = _orderEntry.dsOrders.OrderContainers.NewOrderContainersRow();

                        destContainer.OrderRow = destinationOrder;
                        destContainer.PartQuantity = orgContainer.PartQuantity;
                        destContainer.IsActive = true;

                        if (!orgContainer.IsWeightNull())
                        {
                            destContainer.Weight = orgContainer.Weight;
                        }

                        destContainer.ShipmentPackageTypeID = orgContainer.ShipmentPackageTypeID;

                        _orderEntry.dsOrders.OrderContainers.AddOrderContainersRow(destContainer);

                        foreach (var item in orgContainer.GetOrderContainerItemRows())
                        {
                            _orderEntry.dsOrders.OrderContainerItem.AddOrderContainerItemRow(destContainer, item.ShipmentPackageTypeID);
                        }

                        orgContainer.IsActive = false;
                    }

                    // Rejoin serial numbers
                    destinationNode.LoadSerialNumberNode(_orderEntry); // Load existing serial numbers

                    var partOrder = 0;

                    if (destinationOrder.GetOrderSerialNumberRows().Any(o => o.IsValidState() && o.Active))
                    {
                        partOrder = destinationOrder.GetOrderSerialNumberRows().Where(o => o.IsValidState() && o.Active).Max(s => s.PartOrder);
                    }

                    foreach (var orgSerial in orgOrder.GetOrderSerialNumberRows().Where(o => o.IsValidState() && o.Active))
                    {
                        partOrder++;

                        var destSerial = _orderEntry.dsOrders.OrderSerialNumber.NewOrderSerialNumberRow();
                        destSerial.OrderRow = destinationOrder;
                        destSerial.Active = true;
                        destSerial.PartOrder = partOrder;

                        if (!orgSerial.IsNumberNull())
                        {
                            destSerial.Number = orgSerial.Number;
                        }

                        _orderEntry.dsOrders.OrderSerialNumber.AddOrderSerialNumberRow(destSerial);

                        orgSerial.Active = false;
                        orgSerial.DateRemoved = rejoinDate;
                    }

                    // Set quantities for open shipments for the destination order
                    destinationNode.LoadShipmentNodes(_orderEntry);
                    foreach (var destShipment in destinationOrder.GetOrderShipmentRows())
                    {
                        var shipmentPackageId = destShipment.IsShipmentPackageIDNull()
                            ? -1
                            : destShipment.ShipmentPackageID;

                        if (destShipment.PartQuantity == originalDestinationQuantity && (_orderEntry.taOrderShipment.IsShipmentPackageActive(shipmentPackageId) ?? false))
                        {
                            destShipment.PartQuantity = destinationOrder.IsPartQuantityNull()
                                ? 0
                                : destinationOrder.PartQuantity;
                        }
                    }

                    // Remove source order from any active shipments
                    foreach (var orgShipNode in orgNode.Nodes.OfType<ShipmentNode>().ToList())
                    {
                        var shipmentPackageId = orgShipNode.DataRow.IsShipmentPackageIDNull()
                            ? -1
                            : orgShipNode.DataRow.ShipmentPackageID;

                        if (_orderEntry.taOrderShipment.IsShipmentPackageActive(shipmentPackageId) ?? false)
                        {
                            orgNode.Nodes.Remove(orgShipNode);
                            orgShipNode.DataRow.Delete();
                        }
                    }

                    // Save data
                    // Otherwise, WO Traveler will show unsaved data.
                    _orderEntry.SaveData();

                    if (UserSettings.Default.RejoinPrintTraveler)
                    {
                        var report = new WorkOrderTravelerReport(destinationOrder);
                        report.PrintReport();
                    }


                    destinationNode.LoadContainerNode(_orderEntry); // Rejoin may add containers
                    destinationNode.LoadChangeNodes(_orderEntry);
                    destinationNode.Select();
                }
            }

            public override void Dispose()
            {
                _qtyEditor = null;
                _orderEntry = null;

                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region Nested type: OrderHoldCommand

        private class OrderHoldToggleCommand : TreeNodeCommandBase
        {
            #region Fields

            private OrderEntry _orderEntry;
            private bool _inCheckingButton;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get
                {
                    var orderNode = _node as OrderNode;
                    return IsAuthorized() &&
                        orderNode != null &&
                        orderNode.DataRow.Status == Properties.Settings.Default.OrderStatusOpen;
                }
            }

            #endregion Properties

            #region Methods

            public OrderHoldToggleCommand(ToolBase tool, UltraTree toc, OrderEntry orderEntry, string securityRole)
                : base(tool, securityRole)
            {
                _orderEntry = orderEntry;
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                if(_inCheckingButton)
                    return;

                var orderNode = _node as OrderNode;

                if(orderNode != null)
                {
                    var newHold = !orderNode.DataRow.Hold;
                    string newWorkStatus = null;
                    var currentUser = SecurityManager.Current;

                    if (newHold)
                    {
                        newWorkStatus = ApplicationSettings.Current.WorkStatusHold;

                        var configSettings = Properties.Settings.Default;

                        using (var frm = new HoldEventLog() { UserName = currentUser.UserName, PrintHoldLabel = configSettings.HoldPrintLabel })
                        {
                            if(frm.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                            {
                                var hold                = _orderEntry.dsOrders.OrderHold.NewOrderHoldRow();
                                hold.OrderID            = orderNode.DataRow.OrderID;
                                hold.HoldReasonID       = frm.ReasonCode;
                                hold.TimeIn             = DateTime.Now;
                                hold.TimeInUser         = currentUser.UserID;
                                hold.Notes              = frm.Notes;
                                hold.OriginalWorkStatus = orderNode.DataRow.WorkStatus;
                                _orderEntry.dsOrders.OrderHold.AddOrderHoldRow(hold);

                                orderNode.LoadHoldNodes(_orderEntry); // show new hold in UI

                                // Save any config changes
                                configSettings.HoldPrintLabel = frm.PrintHoldLabel;
                                configSettings.Save();

                                if (frm.PrintHoldLabel)
                                {
                                    // Print hold label
                                    using (var holdLabel = new ReworkLabelReport(orderNode.DataRow, ReworkLabelReport.ReportLabelType.Hold))
                                    {
                                        holdLabel.PrintReport();
                                    }
                                }

                                // Create Hold Notifications
                                var holdNotificationContactIds = HoldUtilities
                                    .GetContactIdsForNotification(orderNode.DataRow.IsCustomerIDNull() ? -1 : orderNode.DataRow.CustomerID);

                                foreach (var contactId in holdNotificationContactIds)
                                {
                                    var notificationRow = _orderEntry.dsOrders.OrderHoldNotification.NewOrderHoldNotificationRow();
                                    notificationRow.OrderHoldRow = hold;
                                    notificationRow.ContactID = contactId;
                                    _orderEntry.dsOrders.OrderHoldNotification.AddOrderHoldNotificationRow(notificationRow);
                                }
                            }
                            else
                                return;
                        }
                    }
                    else
                    {
                        var dtOrderHold = new OrdersDataSet.OrderHoldDataTable();
                        using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderHoldTableAdapter())
                            ta.Fill(dtOrderHold, orderNode.DataRow.OrderID);

                        var holdRow = dtOrderHold.Where(h => h.IsTimeOutNull())
                            .OrderByDescending(h => h.OrderHoldID)
                            .FirstOrDefault();

                        newWorkStatus = holdRow != null && !holdRow.IsOriginalWorkStatusNull()
                            ? holdRow.OriginalWorkStatus
                            : null;

                        if (newWorkStatus == null)
                        {
                            var orderReviewed = orderNode.DataRow.GetOrderReviewRows().Any(or => or.Status);
                            newWorkStatus = orderReviewed ?
                                ApplicationSettings.Current.WorkStatusChangingDepartment :
                                OrderControllerExtensions.GetNewOrderWorkStatus(orderNode.DataRow.CustomerID, OrderInformation.UserRequiresOrderReview);
                        }

                        var orderType = (OrderType) orderNode.DataRow.OrderType;

                        switch(orderType)
                        {
                            case OrderType.Normal:
                            case OrderType.ReworkExt:
                            case OrderType.ReworkInt:
                                break;
                            case OrderType.ReworkHold:
                                MessageBoxUtilities.ShowMessageBoxWarn("Order {0} is on a hold for internal rework. The hold must be removed when the internal rework order is joined back.".FormatWith(orderNode.DataRow.OrderID), "Internal Rework Hold");
                                return;
                            case OrderType.Lost:
                                break;
                            case OrderType.Quarantine:
                                var afterHoldWorkStatus = OrderUtilities.WorkStatusAfterQuarantine(orderNode.DataRow.RequireCoc);

                                var afterHoldLocation = OrderUtilities.LocationAfterQuarantine(orderNode.DataRow.RequireCoc);

                                var results = MessageBoxUtilities.ShowMessageBoxYesOrNo(
                                    $"Order {orderNode.DataRow.OrderID} is on a hold for quarantine. Do you want to remove the hold and move this order to {afterHoldWorkStatus}?",
                                    "Quarantine Hold");

                                if(results == DialogResult.Yes)
                                {
                                    newWorkStatus = afterHoldWorkStatus;
                                    orderNode.DataRow.CurrentLocation = afterHoldLocation;

                                    //close out any open quarantines for this order
                                    var orderID = orderNode.DataRow.OrderID;
                                    var quarantines = _orderEntry.dsOrders.InternalRework
                                        .Where(ir => ir.IsValidState() && !ir.IsReworkOrderIDNull() && ir.ReworkOrderID == orderID && ir.Active);

                                    if(!quarantines.Any())
                                    {
                                        _orderEntry.taInternalRework.FillByReworkOrderID(_orderEntry.dsOrders.InternalRework, orderID);
                                        quarantines = _orderEntry.dsOrders.InternalRework
                                            .Where(ir => ir.IsValidState() && !ir.IsReworkOrderIDNull() && ir.ReworkOrderID == orderID && ir.Active);
                                    }

                                    foreach (var q in quarantines)
                                    {
                                        q.Active = false;
                                    }
                                }
                                else
                                {
                                    // Keep order on hold if user answers no
                                    return;
                                }
                                break;
                        }

                        // Close out all existing holds

                        var openHolds = orderNode.DataRow.GetOrderHoldRows().Where(oh => oh.IsTimeOutNull()).ToList();

                        foreach (var openHold in openHolds)
                        {
                            openHold.TimeOut = DateTime.Now;
                            openHold.TimeOutUser = currentUser.UserID;
                        }

                        if (openHolds.Count > 1)
                        {
                            MessageBoxUtilities.ShowMessageBoxWarn("Removed order from multiple holds.", "Hold");
                        }
                    }

                    orderNode.DataRow.Hold = newHold;
                    orderNode.DataRow.WorkStatus = newWorkStatus;

                    OrderHistoryDataSet.UpdateOrderHistory(orderNode.DataRow.OrderID, "Order Entry", newHold ? "Order put on hold." : "Order removed from hold.", currentUser.UserName);
                }
            }

            public override void Dispose()
            {
                _orderEntry = null;
                base.Dispose();
            }

            public override void OnAfterSelect(UltraTreeNode selectedNode)
            {
                base.OnAfterSelect(selectedNode);

                if(this.Enabled)
                {
                    _inCheckingButton = true;
                    var stateButton = ((ToolBaseButtonAdapter) base.Button).Tool as StateButtonTool;
                    stateButton.Checked = ((OrderNode) selectedNode).DataRow.Hold;
                    _inCheckingButton = false;
                }
            }

            #endregion Methods
        }

        #endregion

        private class SearchCommand : TreeNodeCommandBase
        {
            #region Fields

            private OrderEntry _orderEntry = null;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.TreeView != null && base.TreeView.Nodes.Count > 0; }
            }

            #endregion

            #region Methods

            public SearchCommand(ToolBase tool, UltraTree toc, OrderEntry orderEntry)
                : base(tool)
            {
                base.TreeView = toc;
                _orderEntry = orderEntry;
            }

            public override void OnClick()
            {
                try
                {
                    using (var frm = new OrderSearch())
                    {
                        if (frm.ShowDialog(_orderEntry) == DialogResult.OK && frm.SelectedOrder != null)
                        {
                            _orderEntry.GoToOrder(frm.SelectedOrder.OrderID);
                        }
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error updating visibility of nodes.");
                }
            }

            public override void Dispose()
            {
                _orderEntry = null;
                base.Dispose();
            }

            #endregion
        }

        private class FoundOrderCommand : TreeNodeCommandBase
        {
            #region Fields

            private OrderEntry _orderEntry = null;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get
                {
                    var orderNode = _node as OrderNode;

                    return orderNode != null &&
                        orderNode.DataRow.Status == Properties.Settings.Default.OrderStatusClosed &&
                        orderNode.DataRow.OrderType == (int)OrderType.Lost;
                }
            }

            #endregion

            #region Methods

            public FoundOrderCommand(ToolBase tool, UltraTree toc, OrderEntry manager)
                : base(tool)
            {
                base.TreeView = toc;
                _orderEntry = manager;
            }

            public override void OnClick()
            {
                try
                {
                    var orderNode = _node as OrderNode;

                    if(orderNode != null)
                    {
                        var selectedOrder = orderNode.DataRow;

                        string workStatus;
                        if (selectedOrder.GetOrderProcessesRows().Any(i => i.IsValidState() && i.IsEndDateNull()))
                        {
                            workStatus = ApplicationSettings.Current.WorkStatusChangingDepartment;
                        }
                        else
                        {
                            var orderType = (OrderType)selectedOrder.OrderType;
                            var isPartMarking = selectedOrder.GetOrderPartMarkRows().Length > 0;
                            workStatus = OrderUtilities.WorkStatusAfterProcessing(orderType, isPartMarking, selectedOrder.RequireCoc);
                        }

                        selectedOrder.WorkStatus = workStatus;
                        selectedOrder.Status    = Properties.Settings.Default.OrderStatusOpen;
                        selectedOrder.OrderType = selectedOrder.OriginalOrderType;
                        selectedOrder.Hold = false;
                        selectedOrder.SetInvoiceNull();
                        selectedOrder.SetCompletedDateNull();

                        var orderId         = selectedOrder.OrderID;
                        var internalRework  = _orderEntry.dsOrders.InternalRework.FirstOrDefault(or => or.Active && or.ReworkType == ReworkType.Lost.ToString() && (or.OriginalOrderID == orderId || (!or.IsReworkOrderIDNull() && or.ReworkOrderID == orderId)));

                        if(internalRework == null)
                        {
                            _orderEntry.taInternalRework.FillByOriginalOrderID(_orderEntry.dsOrders.InternalRework, orderId);
                            _orderEntry.taInternalRework.FillByReworkOrderID(_orderEntry.dsOrders.InternalRework, orderId);
                        }

                        internalRework = _orderEntry.dsOrders.InternalRework.FirstOrDefault(or => or.Active && or.ReworkType == ReworkType.Lost.ToString() && (or.OriginalOrderID == orderId || (!or.IsReworkOrderIDNull() && or.ReworkOrderID == orderId)));
                        if(internalRework != null)
                            internalRework.Active = false;

                        OrderHistoryDataSet.UpdateOrderHistory(selectedOrder.OrderID, "Order Entry", "Order was found.", SecurityManager.Current.UserName);
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error finding node.");
                }
            }

            public override void Dispose()
            {
                _orderEntry = null;
                base.Dispose();
            }

            #endregion
        }

        private class GoForwardBackwardCommand : CommandBase
        {
            #region Fields

            private OrderEntry _orderEntry = null;
            private bool _isForward;

            private static CircularBuffer<int> _undoStack = new CircularBuffer<int>(25);
            private static CircularBuffer<int> _redoStack = new CircularBuffer<int>(25);
            private static event EventHandler StackChanged;
            private static bool _inMoveOperation;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _isForward ? _redoStack.Length > 0 : _undoStack.Length > 0; }
            }

            #endregion

            #region Methods

            public GoForwardBackwardCommand(ToolBase tool, bool isForward, OrderEntry orderEntry)
                : base(tool)
            {
                _orderEntry = orderEntry;
                _isForward  = isForward;
                StackChanged += GoForwardBackwardCommand_StackChanged;
                
                Refresh();
            }

            public override void OnClick()
            {
                try
                {
                    _inMoveOperation = true;
                    
                    if(_isForward)
                    {
                        if(_redoStack.Length > 0)
                        {
                            var selected = _orderEntry.tvwTOC.SelectedNode<OrderNode>();
                            var currentOrderID = -1;
                            
                            if (selected != null)
                                currentOrderID = selected.DataRow.OrderID;

                            var orderID = _redoStack.Pop();
                            
                            //if just moved to this order then pop again to get the last order moved to
                            if (currentOrderID == orderID)
                            {
                                _undoStack.Enqueue(orderID);
                                if (_redoStack.Length > 0)
                                    orderID = _redoStack.Pop();
                            }

                            _orderEntry.GoToOrder(orderID);
                            _undoStack.Enqueue(orderID);
                        }
                    }
                    else
                    {
                        if (_undoStack.Length > 0)
                        {
                            var selected = _orderEntry.tvwTOC.SelectedNode <OrderNode>();
                            var currentOrderID = -1;
                            
                            if(selected != null)
                                currentOrderID = selected.DataRow.OrderID;

                            var orderID = _undoStack.Pop();
                            
                            //if just moved to this order then pop again to get the last order moved to
                            if(currentOrderID == orderID)
                            {
                                _redoStack.Enqueue(orderID);
                                if (_undoStack.Length > 0)
                                    orderID = _undoStack.Pop();
                            }

                            _orderEntry.GoToOrder(orderID);
                            _redoStack.Enqueue(orderID);
                        }
                    }

                    if(StackChanged != null)
                        StackChanged(null, EventArgs.Empty);
                }
                catch(Exception exc)
                {
                    _log.Error(exc, "Error on go forward or backward.");
                }
                finally
                {
                    _inMoveOperation = false;
                }
            }

            public static void AddOrderToStack(int orderID)
            {
                if(_inMoveOperation)
                    return;

                _undoStack.Enqueue(orderID);

                if(StackChanged != null)
                    StackChanged(null, EventArgs.Empty);
            }

            public override void Dispose()
            {
                _orderEntry = null;
                StackChanged -= GoForwardBackwardCommand_StackChanged;
 	            
                base.Dispose();
            }

            #endregion

            #region Events

            private void GoForwardBackwardCommand_StackChanged(object sender, EventArgs e)
            {
                this.Refresh();
            }

            #endregion
        }

        private class AddOrderReviewCommand : TreeNodeCommandBase
        {
            #region Fields

            private OrderEntry _orderEntry;
            private readonly ISet<string> _workStatuses = new HashSet<string>
            {
                ApplicationSettings.Current.WorkStatusPendingOR,
                ApplicationSettings.Current.WorkStatusPendingImportExportReview,
            };

            #endregion Fields

            #region Properties

            public override bool Enabled => _node is OrderNode orderNode
                && orderNode.IsRowValid
                && _workStatuses.Contains(orderNode.DataRow.WorkStatus);

            #endregion Properties

            #region Methods

            public AddOrderReviewCommand(ToolBase tool, UltraTree toc, OrderEntry orderEntry)
                : base(tool)
            {
                base.TreeView = toc;
                _orderEntry = orderEntry;
            }

            public override void OnClick()
            {
                try
                {
                    if (!SecurityManager.Current.IsValidUser) //Ensure there is a valid user. RAYGUN
                        return;

                    var orderNode = _node as OrderNode;

                    if (orderNode == null)
                        return;

                    ApplicationSettings appSettings = ApplicationSettings.Current;
                    if (!appSettings.AllowReviewYourOwnOrders && orderNode.DataRow.CreatedBy == SecurityManager.Current.UserID)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn("Unable to review an order that you created.", "Review Own Order", "DWOS is set to not allow users to review their own orders.");
                        return;
                    }

                    if (orderNode.DataRow.GetOrderProcessesRows().Length == 0)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn(
                            "This order does not have any processes. Please add at least one process to the part.",
                            "Review Order");

                        return;
                    }

                    using (var dlg = new OrderReviewStatus())
                    {
                        dlg.LoadData(SecurityManager.Current.UserID, orderNode.DataRow.OrderID, _orderEntry.dsOrders);

                        if (dlg.ShowDialog(_orderEntry) == DialogResult.OK)
                        {
                            var nextNode = orderNode.GetNextVisibleSibling() as UltraTreeNode;

                            //if passed then update work status
                            if (dlg.ReviewStatus)
                            {
                                var customerId = orderNode.DataRow.IsCustomerIDNull()
                                    ? -1
                                    : orderNode.DataRow.CustomerID;

                                orderNode.DataRow.WorkStatus = _orderEntry.Mode == OrderEntryMode.ImportExportReview
                                    ? OrderControllerExtensions.GetNewOrderWorkStatus(
                                        customerId,
                                        SecurityManager.Current.CurrentUser == null || SecurityManager.Current.CurrentUser.RequireOrderReview)
                                    : appSettings.WorkStatusChangingDepartment;

                                _orderEntry._successfullyReviewedOrders.Add(orderNode.DataRow.OrderID);

                                AddReceiptNotification(SecurityManager.Current.UserID, orderNode.DataRow.OrderID,orderNode);
                            }

                            var orderReviewTypeId = _orderEntry.Mode == OrderEntryMode.ImportExportReview
                                ? 2
                                : 1;

                            _orderEntry.dsOrders.OrderReview.AddOrderReviewRow(
                                orderNode.DataRow,
                                _orderEntry.dsOrders.UserSummary.FindByUserID(SecurityManager.Current.UserID),
                                DateTime.Now,
                                dlg.Notes,
                                dlg.ReviewStatus,
                                _orderEntry.dsOrders.OrderReviewType.FindByOrderReviewTypeID(orderReviewTypeId));

                            orderNode.Visible = false;

                            //hide sales order if now visible children
                            if(orderNode.Parent is SalesOrderNode)
                            {
                                var so = orderNode.Parent as SalesOrderNode;
                                var visibleCount = so.Nodes.OfType <OrderNode>().Count(w => w.Visible);
                                if(visibleCount < 1)
                                    nextNode = so.GetNextVisibleSibling(); //set next node to the SO's sibling since it will be hidden
                                so.Visible = visibleCount > 0;
                            }

                            //move off node to change selection in OR panel
                            if (nextNode != null)
                                nextNode.Select();
                            else
                                TreeView.Nodes[0].Select(false);
                        }
                    }

                    // Load data for all orders in the Sales Order
                    // for this Work Order.
                    if (orderNode.Parent is SalesOrderNode salesOrderNode)
                    {
                        salesOrderNode.LoadChildrenNodes(_orderEntry);
                    }

                    // Automatically create batches (if enabled)
                    var autoCreateBatches = appSettings.AutomaticallyBatchSalesOrder
                        && !orderNode.DataRow.IsSalesOrderIDNull()
                        && orderNode.DataRow.SalesOrderRow.GetOrderRows().All(o => o.WorkStatus == appSettings.WorkStatusChangingDepartment);

                    if (autoCreateBatches)
                    {
                        _orderEntry.AutoCreateBatch(orderNode.DataRow.SalesOrderRow);
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error adding order review status.");
                }
            }



            public void AddReceiptNotification(int UserID, int OrderID, OrderNode Order)
            {
                try 
                { 
                                
                    var dsOrders = new OrdersDataSet
                    {
                        //EnforceConstraints = false
                    };


                    // Load Contacts
                    using (var taContacts = new ContactTableAdapter())
                    {
                        var dsCustomer = new CustomersDataset()
                        {
                            EnforceConstraints = false
                        };
                        taContacts.FillBy(dsCustomer.Contact, Order.DataRow.CustomerID);

                        var contacts = dsCustomer.Contact
                            .Where(c => !c.IsNameNull() && !c.IsEmailAddressNull() && c.Active && c.OrderReceiptNotification)
                            .ToList();

                        if (contacts.Count > 0)
                            foreach (var contact in contacts)
                            {
                                using (var taOrderReceipt = new OrderReceiptNotificationTableAdapter())
                                {
                                    //taOrderReceipt.Fill(dsOrders.OrderReceiptNotification);
                                    taOrderReceipt.Insert(OrderID, contact.ContactID, UserID, null);
                                }

                            }
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error adding order Receipt Notification.");
                }

            }



            public override void Dispose()
            {
                _orderEntry = null;
                base.Dispose();
            }

            #endregion Methods
        }

        /// <summary>
        /// Filters the TOC based on the orders that are visible in the WIP screen.
        /// </summary>
        private class WIPFilterCommand : TreeNodeCommandBase
        {
            #region Fields

            private OrderEntry _orderEntry;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get { return true; }
            }

            #endregion Properties

            #region Methods

            public WIPFilterCommand(ToolBase tool, UltraTree toc, OrderEntry orderEntry)
                : base(tool)
            {
                base.TreeView = toc;
                _orderEntry = orderEntry;
            }

            public override void OnClick()
            {
                try
                {
                    if (!SecurityManager.Current.IsValidUser) //Ensure there is a valid user. RAYGUN
                        return;

                    if(DWOSApp.MainForm != null && DWOSApp.MainForm.ActiveTab is IOrderSummary)
                    {
                        var orderSummary = DWOSApp.MainForm.ActiveTab as IOrderSummary;
                        var orders = orderSummary.GetFilteredOrders();

                        if(orders.Count > 0)
                           _orderEntry.LoadTOC(orders);
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error adding order review status.");
                }
            }

            public override void Dispose()
            {
                _orderEntry = null;
                base.Dispose();
            }

            #endregion Methods
        }

        /// <summary>
        /// Filters the TOC to show all orders that need to be reviwed
        /// </summary>
        private class OrderToReviewFilterCommand : TreeNodeCommandBase
        {
            #region Fields

            private OrderEntry _orderEntry;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get { return true; }
            }

            #endregion Properties

            #region Methods

            public OrderToReviewFilterCommand(ToolBase tool, UltraTree toc, OrderEntry orderEntry)
                : base(tool)
            {
                base.TreeView = toc;
                _orderEntry = orderEntry;
            }

            public override void OnClick()
            {
                try
                {
                    if (!SecurityManager.Current.IsValidUser) //Ensure there is a valid user. RAYGUN
                        return;

                    if (DWOSApp.MainForm != null && DWOSApp.MainForm.ActiveTab is IOrderSummary)
                    {
                        //get all orders to review that current user did not create
                        var ordersToReview = new OrdersDataSet.OrderReviewDataTable();
                        ordersToReview.Constraints.Clear();
                        var columnsToRemove = ordersToReview.Columns.OfType<DataColumn>().Where(c => c.ColumnName != "OrderID").ToList();
                        columnsToRemove.ForEach(c => ordersToReview.Columns.Remove(c));

                        var workStatus = _orderEntry.Mode == OrderEntryMode.ImportExportReview
                            ? ApplicationSettings.Current.WorkStatusPendingImportExportReview
                            : ApplicationSettings.Current.WorkStatusPendingOR;

                        _orderEntry.taOrderReview.FillOrderIdsByWorkStatus(
                            ordersToReview,
                            workStatus);

                        var orderIdsToReview = ordersToReview.Select(r => r.OrderID).Distinct();

                        _orderEntry.LoadTOC(orderIdsToReview.ToList());
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error adding order review status.");
                }
            }

            public override void Dispose()
            {
                _orderEntry = null;
                base.Dispose();
            }

            #endregion Methods
        }

        /// <summary>
        /// Filters the Orders that need to be fixed by the current user.
        /// </summary>
        private class OrderToFixFilterCommand : TreeNodeCommandBase
        {
            #region Fields

            private OrderEntry _orderEntry;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get { return true; }
            }

            #endregion Properties

            #region Methods

            public OrderToFixFilterCommand(ToolBase tool, UltraTree toc, OrderEntry orderEntry)
                : base(tool)
            {
                base.TreeView = toc;
                _orderEntry = orderEntry;
            }

            public override void OnClick()
            {
                try
                {
                    if (!SecurityManager.Current.IsValidUser) //Ensure there is a valid user. RAYGUN
                        return;

                    if (DWOSApp.MainForm != null && DWOSApp.MainForm.ActiveTab is IOrderSummary)
                    {
                        var currentUser = SecurityManager.Current.UserID;

                        //get all orders
                        var ordersToFix = new OrdersDataSet.OrderReviewDataTable();
                        ordersToFix.Constraints.Clear();
                        var columnsToRemove = ordersToFix.Columns.OfType<DataColumn>().Where(c => c.ColumnName != "OrderID").ToList();
                        columnsToRemove.ForEach(c => ordersToFix.Columns.Remove(c));

                        var workStatus = _orderEntry.Mode == OrderEntryMode.ImportExportReview
                            ? ApplicationSettings.Current.WorkStatusPendingImportExportReview
                            : ApplicationSettings.Current.WorkStatusPendingOR;

                        var orderReviewTypeId = _orderEntry.Mode == OrderEntryMode.ImportExportReview ? 2 : 1;

                        _orderEntry.taOrderReview.FillUserOrderIdsToFix(ordersToFix, currentUser, workStatus, orderReviewTypeId);
                        var orderIdsToFix = ordersToFix.Select(r => r.OrderID).Distinct();

                        _orderEntry.LoadTOC(orderIdsToFix.ToList());
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error adding order review status.");
                }
            }

            public override void Dispose()
            {
                _orderEntry = null;
                base.Dispose();
            }

            #endregion Methods
        }

        private class RequestOrderApprovalCommand : TreeNodeCommandBase
        {
            #region Properties

            public override bool Enabled
            {
                get
                {
                    return base.Enabled
                        && _node is OrderNode orderNode
                        && orderNode.IsRowValid
                        && !orderNode.DataRow.IsStatusNull()
                        && orderNode.DataRow.Status == Properties.Settings.Default.OrderStatusOpen;
                }
            }

            private OrderEntry _orderEntry { get; }

            #endregion

            #region Methods

            public RequestOrderApprovalCommand(ToolBase tool, UltraTree toc, OrderEntry oe)
                : base(tool, "OrderEntry.OrderApproval")
            {
                TreeView = toc;
                _orderEntry = oe;
            }

            public override void OnClick()
            {
                _orderEntry.EndAllEdits();

                var orderNode = _node as OrderNode;

                if (orderNode == null || !orderNode.IsRowValid ||  !_orderEntry.IsValidControls())
                {
                    return;
                }

                // Put WO on hold
                if (!orderNode.DataRow.Hold)
                {
                    PutOrderOnHold(orderNode);
                }

                // Order must be on hold before creating approval
                if (orderNode.DataRow.Hold)
                {
                    // Create approval
                    var approvalRow = _orderEntry.dsOrders.OrderApproval.NewOrderApprovalRow();
                    approvalRow.OrderRow = orderNode.DataRow;
                    approvalRow.UserID = SecurityManager.Current.UserID;
                    approvalRow.DateCreated = DateTime.Now;
                    approvalRow.Status = nameof(OrderApprovalStatus.Pending);
                    _orderEntry.dsOrders.OrderApproval.AddOrderApprovalRow(approvalRow);

                    // Select new approval
                    var approvalNode = new OrderApprovalNode(approvalRow);
                    orderNode.Nodes.Add(approvalNode);
                    approvalNode.Select();
                }
                else
                {
                    MessageBoxUtilities.ShowMessageBoxWarn(
                        "An order has to be on hold before creating an aproval request for it.",
                        _orderEntry.ModeDisplayName);
                }
            }

            private void PutOrderOnHold(OrderNode orderNode)
            {
                var currentUser = SecurityManager.Current;
                var configSettings = Properties.Settings.Default;

                using (var frm = new HoldEventLog() { UserName = currentUser.UserName, PrintHoldLabel = configSettings.HoldPrintLabel })
                {
                    if (frm.ShowDialog(ActiveForm) == DialogResult.OK)
                    {
                        var hold = _orderEntry.dsOrders.OrderHold.NewOrderHoldRow();
                        hold.OrderID = orderNode.DataRow.OrderID;
                        hold.HoldReasonID = frm.ReasonCode;
                        hold.TimeIn = DateTime.Now;
                        hold.TimeInUser = currentUser.UserID;
                        hold.Notes = frm.Notes;
                        hold.OriginalWorkStatus = orderNode.DataRow.WorkStatus;
                        _orderEntry.dsOrders.OrderHold.AddOrderHoldRow(hold);

                        orderNode.LoadHoldNodes(_orderEntry); // show new hold in UI

                        // Save any config changes
                        configSettings.HoldPrintLabel = frm.PrintHoldLabel;
                        configSettings.Save();

                        if (frm.PrintHoldLabel)
                        {
                            // Print hold label
                            using (var holdLabel = new ReworkLabelReport(orderNode.DataRow, ReworkLabelReport.ReportLabelType.Hold))
                            {
                                holdLabel.PrintReport();
                            }
                        }

                        orderNode.DataRow.Hold = true;
                        orderNode.DataRow.WorkStatus = ApplicationSettings.Current.WorkStatusHold;

                        OrderHistoryDataSet.UpdateOrderHistory(
                            orderNode.DataRow.OrderID,
                            "Order Entry",
                            "Order put on hold.",
                            currentUser.UserName);
                    }
                }
            }

            #endregion
        }

        #endregion Commands

        #region BatchData

        private class BatchData
        {
            public List<int> ProcessIds { get; }

            public List<int> OrderIds { get; }

            public int? BatchId { get; }

            public BatchData(List<int> processIds, int orderId)
            {
                ProcessIds = processIds ?? throw new ArgumentNullException();
                OrderIds = new List<int> { orderId };
            }

            public BatchData(int batchId, List<int> processIds)
            {
                BatchId = batchId;
                ProcessIds = processIds ?? throw new ArgumentNullException();
                OrderIds = new List<int>();
            }

            public bool Matches(List<int> processIds) =>
                Enumerable.SequenceEqual(ProcessIds, processIds);
        }

        #endregion
    }
}