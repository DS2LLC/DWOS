using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Properties;
using DWOS.UI.ShippingRec.ShippingManagerPanels;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.UltraGauge.Resources;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using NLog;
using DWOS.UI.Reports;
using System.Windows.Interop;
using DWOS.Reports.ReportData;

namespace DWOS.UI.ShippingRec
{
    public partial class ShippingManager : DataEditorBase
    {
        #region Fields

        /// <summary>
        /// User-facing placeholder text used when a value in unavailable.
        /// </summary>
        private static string NONE = "None";

        private const int DEFAULT_SHIPMENT_PACKAGE_ID = 1;

        private BarcodeScanner _scanner;
        private readonly IDictionary<int, string> _orderProductClassCache =
            new Dictionary<int, string>();

        #endregion

        #region Methods

        public ShippingManager()
        {
            InitializeComponent();

            base.DisplayCloseButtonOnly("Close");

            _scanner = new BarcodeScanner(
                Report.BARCODE_ORDER_ACTION_PREFFIX,
                Report.BARCODE_SHIPPING_PACKAGE_PREFIX,
                Report.BARCODE_SALES_ORDER_ACTION_PREFFIX);

            this._scanner.BarcodingStarted += _scanner_BarcodingStarted;
            this._scanner.BarcodingFinished += _scanner_BarcodingFinished;
            this._scanner.Start();

            tvwTOC.Override.SortComparer = new ShipmentPackageNodeSorter();

            _log.Info("------------------------");
            _log.Info("Shipping Manager instantiated.");

            base.AddDataPanel(this.pnlShipmentInfo);
            base.AddDataPanel(this.pnlShipmentSummary);
        }

        private void LoadData()
        {
            using (new UsingDataSetLoad(dsOrderShipment))
            {
                this.taShippingCarrier.Fill(this.dsOrderShipment.d_ShippingCarrier);
                this.taShipmentPackageType.Fill(this.dsOrderShipment.ShipmentPackageType);
                this.taCustomerAddress.Fill(this.dsOrderShipment.CustomerAddress);
                this.taCustomerShipping.Fill(this.dsOrderShipment.CustomerShipping);
                this.taCustomer.Fill(this.dsOrderShipment.Customer);
                this.taUsers.Fill(this.dsOrderShipment.Users);
                this.taContactTableAdapter.Fill(this.dsOrderShipment.d_Contact);
                taContactAdditionalCustomer.FillActiveForShipping(dsOrderShipment.ContactAdditionalCustomerSummary);

                //fill all active orders or orders closed this date
                this.taOrderShipment.FillByDateOrActive(this.dsOrderShipment.OrderShipment, DateTime.Now);
                this.taShipmentPackage.FillByActiveOrCloseDate(this.dsOrderShipment.ShipmentPackage, true, DateTime.Now);
            }

            this.pnlShipmentInfo.LoadData(this.dsOrderShipment, this.taOrderShipment);
            this.pnlShipmentSummary.LoadData(this.dsOrderShipment);
            this.pnlShipmentInfo.OrderDeleted += pnlShipmentInfo_OrderDeleted;
            this.pnlShipmentInfo.ValueChanged += pnlShipmentInfo_ValueChanged;
        }

        private void LoadTOC()
        {
            using(new UsingTreeLoad(tvwTOC))
            {
                tvwTOC.Nodes.Clear();

                UltraTreeNode rootNode = new ShipmentPackagesRootNode();
                tvwTOC.Nodes.Add(rootNode);
                rootNode.Expanded = true;

                foreach(var pr in this.dsOrderShipment.ShipmentPackage)
                    rootNode.Nodes.Add(new ShipmentPackageNode(pr));

                rootNode.Select();
            }
        }

        protected override void ReloadTOC()
        {
            try
            {
                // Manually disable/enable constraints.
                // UsingDataSetLoad throws an exception on dispose.
                this.dsOrderShipment.EnforceConstraints = false;

                //Reload all the packages and order shipments, incase another user is shipping also
                this.taOrderShipment.FillByDateOrActive(this.dsOrderShipment.OrderShipment, DateTime.Now);
                this.taShipmentPackage.FillByActiveOrCloseDate(this.dsOrderShipment.ShipmentPackage, true, DateTime.Now);
            }
            finally
            {
                this.dsOrderShipment.EnforceConstraints = true;
            }

            LoadTOC();
        }

        protected override bool SaveData()
        {
            UpdateTotalCounts();
            return true;
        }

        private void LoadCommands()
        {
            if(SecurityManager.Current.IsInRole("ShippingManager.Edit"))
            {
                //Delete Shipment
                var delete = Commands.AddCommand("Delete", new DeleteCommand(toolbarManager.Tools["Delete"], tvwTOC, this)) as DeleteCommand;
                delete.ItemDeleted += delete_ItemDeleted;

                //Complete Shipment
                var csc = Commands.AddCommand("Complete", new PackageCommand(toolbarManager.Tools["Complete"], tvwTOC)) as PackageCommand;
                csc.CommandClicked += csc_OnClick;

                var add = Commands.AddCommand("Add", new AddShipmentCommand(toolbarManager.Tools["Add"], tvwTOC)) as AddShipmentCommand;
                add.CommandClicked += add_CommandClicked;

                var addSalesOrder = Commands.AddCommand("AddSalesOrder", new AddSalesOrderShipmentCommand(toolbarManager.Tools["AddSalesOrder"], tvwTOC))
                    as AddSalesOrderShipmentCommand;

                addSalesOrder.CommandClicked += addSalesOrder_CommandClicked;

                var print = Commands.AddCommand("Print", new PrintNodeCommand(toolbarManager.Tools["Print"], tvwTOC)) as PrintNodeCommand;
                print.ValidateRowUnchangedBeforePrinting = false;

                var printLabel = Commands.AddCommand("PrintLabel", new PrintOrderLabelCommand(toolbarManager.Tools["PrintLabel"], tvwTOC)) as PrintOrderLabelCommand;
                printLabel.CommandClicked += printOrderLabel_OnClick;

                var printPackage = Commands.AddCommand("PackageLabel", new PrintPackageLabelCommand(toolbarManager.Tools["PackageLabel"], tvwTOC)) as PrintPackageLabelCommand;
                printPackage.CommandClicked += printPackageLabel_OnClick;

                var displayOrderMedia = Commands.AddCommand("DisplayOrderMedia", new PackageCommand(toolbarManager.Tools["DisplayOrderMedia"], tvwTOC)) as PackageCommand;
                displayOrderMedia.CommandClicked += displayOrderMedia_OnClick;

                var createPackage = Commands.AddCommand("CreatePackage", new CreatePackageCommand(toolbarManager.Tools["CreatePackage"])) as CreatePackageCommand;
                createPackage.CommandClicked += createPackage_OnClick;

                Commands.AddCommand("Refresh", new RefreshCommand(toolbarManager.Tools["Refresh"], this));
            }

            if (SecurityManager.Current.IsInRole("COC"))
            {
                var bulkCOC = Commands.AddCommand("BulkCOC", new PackageCommand(toolbarManager.Tools["BulkCOC"], tvwTOC)) as PackageCommand;
                bulkCOC.CommandClicked += BulkCOC_CommandClicked;
            }
            else
            {
                toolbarManager.Tools["BulkCOC"].SharedProps.Visible = false;
            }

            if (ApplicationSettings.Current.RepairStatementEnabled)
            {
                var statementCommand = Commands.AddCommand("RepairStatement", new PackageCommand(toolbarManager.Tools["RepairStatement"], tvwTOC)) as PackageCommand;
                statementCommand.CommandClicked += RepairStatement_CommandClicked;
            }
            else
            {
                toolbarManager.Tools["RepairStatement"].SharedProps.Visible = false;
            }

            if (ApplicationSettings.Current.BillOfLadingEnabled)
            {
                var statementCommand = Commands.AddCommand("BillOfLading", new PackageCommand(toolbarManager.Tools["BillOfLading"], tvwTOC)) as PackageCommand;
                statementCommand.CommandClicked += BillOfLading_CommandClicked;
            }
            else
            {
                toolbarManager.Tools["BillOfLading"].SharedProps.Visible = false;
            }
        }

        private void AddShipmentExt(int orderID)
        {
            if(InvokeRequired)
                BeginInvoke(new Action <int>(AddShipment), orderID);
            else
                AddShipment(orderID);
        }

        private void AddEmptyPackage()
        {
            var spn = new EmptyPackageNode();
            tvwTOC.Nodes[0].Nodes.Add(spn);
            spn.Select();
        }

        private ShipmentPackageNode CreateShipmentPackage(int customerID)
        {
            //create new shipment package
            var newPackageRow = this.pnlShipmentInfo.AddShipmentPackage(customerID, GetNextPackageNumber(customerID, DEFAULT_SHIPMENT_PACKAGE_ID));

            this.taShipmentPackage.Update(newPackageRow);

            //create new ui node
            var spn = new ShipmentPackageNode(newPackageRow);
            tvwTOC.Nodes[0].Nodes.Add(spn);
            spn.Select();

            this.pnlShipmentSummary.AddShipmentLog(spn.DataRow.CustomerRow.Name, "New Package '" + spn.Text + "' added.", SecurityManager.Current.UserName, ShippingChange.PackageCreated);

            return spn;
        }

        private void AddShipment(int orderID)
        {
            try
            {
                var selectedNode = tvwTOC.SelectedNodes.Count == 1 ? tvwTOC.SelectedNodes[0] : null;
                ShipmentPackageNode package = null;

                var printPackageLabel = false;
                if(selectedNode is EmptyPackageNode)
                {
                    var customerID = this.taOrderShipment.GetCustomerID(orderID).GetValueOrDefault();

                    if(customerID > 0)
                    {
                        printPackageLabel = true;
                        package = CreateShipmentPackage(customerID);

                        // Remove empty box, then select the new package
                        selectedNode.Remove();
                        package.Select();

                        // Update text for all other customer packages to reflect newly-added package
                        foreach (var node in tvwTOC.Nodes[0].Nodes.OfType<ShipmentPackageNode>())
                        {
                            if (node.DataRow.CustomerID != customerID || node.DataRow.ShipmentPackageID == package.DataRow.ShipmentPackageID)
                            {
                                continue;
                            }

                            node.UpdateNodeUI();
                        }
                    }
                    else
                    {
                        //Not a valid order
                        _log.Info("Order " + orderID + " was not added to a package.");
                        NotifyOrderError(orderID, "Not a valid order.");
                    }
                }
                else if(selectedNode is ShipmentPackageNode)
                    package = (ShipmentPackageNode) selectedNode;

                if(package != null)
                {
                    var ordersToPrint = new List<int>();
                    var success = AddShipment(package, orderID);
                    if(success)
                    {
                        ordersToPrint.Add(orderID);
                        var quarantinedOrderIds = GetQuarantinedOrders(orderID);
                        foreach(var quarantinedOrderId in quarantinedOrderIds)
                        {
                            var results = MessageBoxUtilities.ShowMessageBoxYesOrNo("The order {0} has a related quarantined order, do you want to add the quarantined order {1} to the package.".FormatWith(orderID, quarantinedOrderId), "Related Quarantined Order");

                            if (results == DialogResult.Yes)
                            {
                                AddShipment(package, quarantinedOrderId);
                                ordersToPrint.Add(quarantinedOrderId);
                            }
                        }
                    }

                    // Wait until adding a order before trying to print the package label
                    if (printPackageLabel)
                    {
                        PrintPackageLabel(package.DataRow);
                    }

                    // Print order-specific documents after printing the package label
                    foreach (var orderIdToPrint in ordersToPrint)
                    {
                        PrintOrderCOC(orderIdToPrint);
                        PrintOrderLabel(orderIdToPrint);
                    }

                    package.UpdateNodeUI();
                }
                else
                {
                    //No package selected to add to
                    _log.Info("Order " + orderID + " was not added to a package.");
                    NotifyOrderError(orderID, "No package selected. Select an existing package or create a new one.");
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error adding order {0} to shipment package.".FormatWith(orderID), exc);
            }
        }

        private bool AddShipment(ShipmentPackageNode packageNode, int orderID)
        {
            try
            {
                _log.Info("Attempting to add a Shipment " + orderID);
                string invalidOrderMsg = null;

                //ensure order is in proper state to added as new shipment
                if(IsValidOrder(packageNode, orderID, out invalidOrderMsg))
                {
                    //ensure selected/loaded
                    if(!packageNode.Selected)
                        packageNode.Select();

                    // if this is the first order added to the package then set the packages carrier id and address
                    if(packageNode.DataRow.GetOrderShipmentRows().Length == 0)
                    {
                        var customerShipping = this.pnlShipmentInfo.GetShippingCarrier(orderID);
                        if(customerShipping != null)
                        {
                            packageNode.DataRow.ShippingCarrierID = customerShipping.CarrierID;
                            packageNode.DataRow.CarrierCustomerNumber = customerShipping.CarrierCustomerNumber;
                        }
                        else
                        {
                            // Fallback to first shipping carrier
                            packageNode.DataRow.ShippingCarrierID =
                                dsOrderShipment.d_ShippingCarrier.FirstOrDefault()?.CarrierID;
                        }

                        var address = this.pnlShipmentInfo.GetCustomerAddress(orderID);
                        if (address != null)
                        {
                            packageNode.DataRow.CustomerAddressID = address.CustomerAddressID;
                        }
                    }

                    //add node to currently selected order
                    var orderShipment = this.pnlShipmentInfo.AddOrderShipment(orderID);
                    this.pnlShipmentInfo.EndEditing();
                    this.taOrderShipment.Update(orderShipment);
                    this.taShipmentPackage.Update(packageNode.DataRow);

                    packageNode.UpdateNodeUI();

                    //reset selection and reselect to force commands to refresh
                    tvwTOC.Nodes[0].Select();
                    packageNode.Select();

                    //notify that was successful
                    Sound.Beep();
                    OrderHistoryDataSet.UpdateOrderHistory(orderID, "Shipping", "Order " + orderID + " added to package " + packageNode.DataRow.ShipmentPackageID + ".", SecurityManager.Current.UserName);
                    NotifyOrderSuccess(orderID, "Added to package " + packageNode.DataRow.PackageNumber);
                    this.pnlShipmentSummary.AddShipmentLog(packageNode.DataRow.CustomerRow.Name, "Order " + orderID + " added to package.", SecurityManager.Current.UserName, ShippingChange.PackageCreated);
                    return true;
                }
                else
                {
                    //notify warning didn't work for some reason
                    _log.Info("Order " + orderID + " was not added to a package.");
                    NotifyOrderError(orderID, invalidOrderMsg);
                }

                return false;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error adding order shipment.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);

                NotifyOrderError(orderID, "Error processing package.");
                return false;
            }
            finally
            {
                UpdateTotalCounts();
            }
        }

        private void AddSalesOrderShipment(int salesOrderId)
        {
            try
            {
                var selectedNode = tvwTOC.SelectedNodes.Count == 1 ? tvwTOC.SelectedNodes[0] : null;
                ShipmentPackageNode package = null;

                var printPackageLabel = false;
                if (selectedNode is EmptyPackageNode)
                {
                    var customerId = taOrderShipment.GetCustomerIdFromSalesOrder(salesOrderId)
                        ?? -1;

                    if (customerId > 0)
                    {
                        printPackageLabel = true;
                        package = CreateShipmentPackage(customerId);

                        // Remove empty box, then select the new package
                        selectedNode.Remove();
                        package.Select();

                        // Update text for all other customer packages to reflect newly-added package
                        foreach (var node in tvwTOC.Nodes[0].Nodes.OfType<ShipmentPackageNode>())
                        {
                            if (node.DataRow.CustomerID != customerId || node.DataRow.ShipmentPackageID == package.DataRow.ShipmentPackageID)
                            {
                                continue;
                            }

                            node.UpdateNodeUI();
                        }
                    }
                    else
                    {
                        //Not a valid order
                        _log.Info("Sales Order " + salesOrderId + " was not added to a package.");
                        NotifyOrderError(salesOrderId, "Not a valid sales order.");
                    }
                }
                else if (selectedNode is ShipmentPackageNode)
                {
                    package = (ShipmentPackageNode)selectedNode;
                }
                else
                {
                    //No package selected to add to
                    _log.Info("Sales Order " + salesOrderId + " was not added to a package.");
                    NotifyOrderError(salesOrderId, "No package selected. Select an existing package or create a new one.");
                }

                if (package != null)
                {
                    var ordersToPrint = new List<int>();
                    if (AddSalesOrderShipment(package, salesOrderId))
                    {
                        var orderIds = GetOpenOrderIdsInSalesOrder(salesOrderId);
                        ordersToPrint.AddRange(orderIds);

                        foreach (var orderId in orderIds)
                        {
                            foreach (var quarantinedOrderId in GetQuarantinedOrders(orderId))
                            {
                                var quarantimePromptMessage = $"The order {orderId} " +
                                    $"has a related quarantined order, do you want to " +
                                    $"add the quarantined order {quarantinedOrderId} " +
                                    $"to the package.";

                                var results = MessageBoxUtilities.ShowMessageBoxYesOrNo(
                                    quarantimePromptMessage,
                                    "Related Quarantined Order");

                                if (results == DialogResult.Yes)
                                {
                                    AddShipment(package, quarantinedOrderId);
                                    ordersToPrint.Add(quarantinedOrderId);
                                }
                            }
                        }
                    }

                    // Wait until adding a order before trying to print the package label
                    if (printPackageLabel)
                    {
                        PrintPackageLabel(package.DataRow);
                    }

                    // Print order-specific documents after printing the package label
                    foreach (var orderIdToPrint in ordersToPrint)
                    {
                        PrintOrderCOC(orderIdToPrint);
                        PrintOrderLabel(orderIdToPrint);
                    }

                    package.UpdateNodeUI();
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog(
                    $"Error adding sales order {salesOrderId} to shipment package.",
                    exc);
            }
        }

        private bool AddSalesOrderShipment(ShipmentPackageNode packageNode, int salesOrderId)
        {
            try
            {
                _log.Info($"Attempting to add a Shipment with Sales Order {salesOrderId}");

                var doesSalesOrderExist = taUtility.DoesSalesOrderExist(salesOrderId) ?? false;

                if (!doesSalesOrderExist)
                {
                    _log.Info("Sales Order " + salesOrderId + " was not added to a package.");
                    NotifyOrderError(salesOrderId, "Not a valid sales order.");
                    return false;
                }

                var orderIds = GetOpenOrderIdsInSalesOrder(salesOrderId);
                var isSalesOrderValid = orderIds.Count > 0;
                var isNewPackage = packageNode.DataRow.GetOrderShipmentRows().Length == 0;

                foreach (var orderId in orderIds)
                {
                    if (IsValidOrder(packageNode, orderId, out var invalidOrderMsg))
                    {
                        continue;
                    }

                    _log.Info($"Order {orderId} is not valid.");
                    NotifyOrderError(orderId, invalidOrderMsg);
                    isSalesOrderValid = false;
                    break;
                }

                var checkProductClass = isSalesOrderValid
                    && isNewPackage
                    && ApplicationSettings.Current.RequireSingleProductClassForShipments;

                if (checkProductClass)
                {
                    var productClassCount = orderIds
                        .Select(orderId => GetProductClass(orderId))
                        .Distinct()
                        .Count();

                    if (productClassCount != 1)
                    {
                        NotifyOrderError(salesOrderId, "Orders in the Sales Order are in different product classes.");
                        isSalesOrderValid = false;
                    }
                }

                if (isSalesOrderValid)
                {
                    //ensure selected/loaded
                    if (!packageNode.Selected)
                    {
                        packageNode.Select();
                    }

                    var firstOrderId = orderIds.First();

                    // if this is the first order added to the package then set the packages carrier id and address
                    if (packageNode.DataRow.GetOrderShipmentRows().Length == 0)
                    {
                        var customerShipping = pnlShipmentInfo.GetShippingCarrier(firstOrderId);
                        if (customerShipping != null)
                        {
                            packageNode.DataRow.ShippingCarrierID = customerShipping.CarrierID;
                            packageNode.DataRow.CarrierCustomerNumber = customerShipping.CarrierCustomerNumber;
                        }

                        var address = pnlShipmentInfo.GetCustomerAddress(firstOrderId);
                        if (address != null)
                        {
                            packageNode.DataRow.CustomerAddressID = address.CustomerAddressID;
                        }
                    }

                    // Add nodes & shipment rows for work orders
                    var orderShipments = new List<OrderShipmentDataSet.OrderShipmentRow>();
                    foreach (var orderId in orderIds)
                    {
                        orderShipments.Add(pnlShipmentInfo.AddOrderShipment(orderId));
                    }

                    pnlShipmentInfo.EndEditing();
                    taOrderShipment.Update(orderShipments.ToArray());
                    taShipmentPackage.Update(packageNode.DataRow);

                    packageNode.UpdateNodeUI();

                    //reset selection and reselect to force commands to refresh
                    tvwTOC.Nodes[0].Select();
                    packageNode.Select();

                    //notify that was successful
                    Sound.Beep();

                    // Update all orders
                    foreach (var orderId in orderIds)
                    {
                        OrderHistoryDataSet.UpdateOrderHistory(
                            orderId,
                            "Shipping",
                            $"Order {orderId} added to package {packageNode.DataRow.ShipmentPackageID}.",
                            SecurityManager.Current.UserName);
                    }

                    NotifyOrderSuccess(salesOrderId, "Added to package " + packageNode.DataRow.PackageNumber);

                    pnlShipmentSummary.AddShipmentLog(
                        packageNode.DataRow.CustomerRow.Name,
                        $"Sales Order {salesOrderId} added to package.",
                        SecurityManager.Current.UserName,
                        ShippingChange.PackageCreated);

                    return true;
                }

                return false;
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog(
                    "Error adding sales order shipment.",
                    exc);

                NotifyOrderError(salesOrderId, "Error processing package.");
                return false;
            }
            finally
            {
                UpdateTotalCounts();
            }
        }

        private List<int> GetOpenOrderIdsInSalesOrder(int salesOrderId)
        {
            using (var dtOrder = taOrders.GetDataBySalesOrder(salesOrderId))
            {
                return dtOrder
                    .Where(order => order.Status == Settings.Default.OrderStatusOpen)
                    .Select(order => order.OrderID)
                    .ToList();
            }
        }

        private List <int> GetQuarantinedOrders(int orderID)
        {
            try
            {
                using (var taInternalRework = new Data.Datasets.OrdersDataSetTableAdapters.InternalReworkTableAdapter { ClearBeforeFill = true })
                {
                    //see if this is a split
                    var quarantines = taInternalRework.GetQuarantined(orderID);
                    if (quarantines.Any())
                    {
                        return quarantines.Where(qi => !qi.IsReworkOrderIDNull()).Select(q => q.ReworkOrderID).ToList();
                    }

                    return new List<int>();
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error getting quarantined orders for order {0}.".FormatWith(orderID));
                return new List <int>();
            }
        }

        private void NotifyOrderSuccess(int orderId, string msg)
        {
            const int successIntervalMilliseconds = 2500;

            try
            {
                timerScanUI.Stop();
                pnlStatus.BackColor = Color.Green;
                lblStatus.Text = orderId.ToString();
                lblStatusMsg.Text = msg;
                timerScanUI.Interval = successIntervalMilliseconds;
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error notifying user of Order Success.");
            }
            finally
            {
                timerScanUI.Start();
            }
        }

        private void NotifyOrderError(int orderId, string msg)
        {
            const int errorIntervalMilliseconds = 8000;

            try
            {
                Sound.BeepError();

                timerScanUI.Stop();
                pnlStatus.BackColor = Color.Red;
                lblStatus.Text = orderId.ToString();
                lblStatusMsg.Text = msg;
                timerScanUI.Interval = errorIntervalMilliseconds;
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error notifying user of Order Error.");
            }
            finally
            {
                timerScanUI.Start();
            }
        }

        private void NotifyScannerReading()
        {
            if(InvokeRequired)
                BeginInvoke(new Action(NotifyScannerReading));
            else
            {
                this.pnlStatus.BackColor = Color.Blue;
                this.lblStatus.Text = null;
                this.lblStatusMsg.Text = "Reading";
            }
        }

        private void NotifyScannerReadingReset()
        {
            if(InvokeRequired)
                BeginInvoke(new Action(NotifyScannerReadingReset));
            else
            {
                this.pnlStatus.BackColor = Color.Yellow;
                this.lblStatus.Text = "Online";
                this.lblStatusMsg.Text = null;
            }
        }

        private void PrintOrderLabel(int orderID)
        {
            try
            {
                if(UserSettings.Default.Shipping.PrintOrderLabel)
                {
                    var report = new ReportGenerator(() => new ShippingOrderLabel(orderID));
                    report.BeginPrint();
                }
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error printing order label.";

                if (exc is LabelPrinterException)
                {
                    _log.Warn(exc, errorMsg);
                }
                else
                {

                    _log.Error(exc, errorMsg);
                }
            }
        }

        private void PrintPackageLabel(OrderShipmentDataSet.ShipmentPackageRow package)
        {
            try
            {
                if(UserSettings.Default.Shipping.PrintPackageLabel)
                {
                    var report = new ReportGenerator(() => new ShippingPackageLabel(package));
                    report.BeginPrint();
                }
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error printing shipping label.";

                if (exc is LabelPrinterException)
                {
                    _log.Warn(exc, errorMsg);
                }
                else
                {

                    _log.Error(exc, errorMsg);
                }
            }
        }

        private void PrintOrderCOC(int orderID)
        {
            try
            {
                if(UserSettings.Default.Shipping.PrintCoc)
                {
                    using(var taCOC = new Data.Datasets.COCDatasetTableAdapters.COCTableAdapter())
                    {
                        int? cocID = taCOC.GetMostRecentCOCIDByOrderID(orderID);

                        if (cocID.HasValue)
                        {
                            var report = new ReportGenerator(() => new COCReport(cocID.Value));

                            if (UserSettings.Default.Shipping.QuickPrint)
                            {
                                report.BeginPrint(1, false);
                            }
                            else
                            {
                                report.Display();
                            }
                        }
                        else
                        {
                            _log.Debug("Could not find COC for order {0}", orderID);
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error printing COC.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        protected override void LoadNode(UltraTreeNode node)
        {
            if(node is ShipmentPackageNode)
            {
                var curNode = (ShipmentPackageNode) node;

                DisplayPanel(this.pnlShipmentInfo);
                this.pnlShipmentInfo.MoveToRecord(curNode.ID);
            }
            else if(node is ShipmentPackagesRootNode)
            {
                pnlShipmentSummary.RefreshData();
                DisplayPanel(this.pnlShipmentSummary);
            }
            else
                DisplayPanel(null);
        }

        private bool IsValidOrder(ShipmentPackageNode packageNode, int orderID, out string invalidOrderMsg)
        {
            string errMsg = null;
            invalidOrderMsg = null;

            //validate order is ready to be shipped by being in the shipping department
            object shipStatus = this.taOrderShipment.IsShipping(orderID);
            int shipStatusCount = 0;

            if(shipStatus == null || shipStatus == DBNull.Value || !Int32.TryParse(shipStatus.ToString(), out shipStatusCount) || shipStatusCount != 1)
                errMsg = "Order is not in Shipping.";
            else
            {
                var orders = this.dsOrderShipment.OrderShipment.Select("OrderID = " + orderID);
                if(orders.Length > 0)
                    errMsg = "Order Duplicate! Order is already in a package.";
                else
                {
                    var customerID = this.taOrderShipment.GetCustomerID(orderID).GetValueOrDefault();
                    if(packageNode.DataRow.CustomerID != customerID)
                        errMsg = "Customer Mismatch! Order belongs to a different customer.";
                    else
                    {
                        // if package has children (aka carrier has already been set) then ensure they are the same carrier and address
                        if(packageNode.DataRow.GetOrderShipmentRows().Length > 0 && packageNode.DataRow.CustomerRow.GetCustomerShippingRows().Length > 0)
                        {
                            var customerShipping = this.pnlShipmentInfo.GetShippingCarrier(orderID);
                            if (!packageNode.DataRow.IsShippingCarrierIDNull() && (customerShipping == null || packageNode.DataRow.ShippingCarrierID != customerShipping.CarrierID))
                            {
                                errMsg = "Shipping Mismatch! Order's carrier is not package carrier.";
                            }
                            else
                            {
                                var customerAddress = this.pnlShipmentInfo.GetCustomerAddress(orderID);
                                if (!packageNode.DataRow.IsCustomerAddressIDNull() && (customerAddress == null || packageNode.DataRow.CustomerAddressID != customerAddress.CustomerAddressID))
                                {
                                    errMsg = "Shipping Mismatch! Order's address is not package's address.";
                                }
                                else if (ApplicationSettings.Current.RequireSingleProductClassForShipments)
                                {
                                    var hasOrders = packageNode.DataRow.GetOrderShipmentRows().Length > 0;

                                    if (hasOrders && GetProductClass(orderID) != GetProductClass(packageNode))
                                    {
                                        errMsg = "Shipping Mismatch! Order is not in the correct product class.";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if(!String.IsNullOrEmpty(errMsg))
            {
                _log.Info("Warning: " + errMsg);
                invalidOrderMsg = errMsg;
                return false;
            }

            return true;
        }

        private string GetProductClass(int orderId)
        {
            if (_orderProductClassCache.ContainsKey(orderId))
            {
                return _orderProductClassCache[orderId];
            }

            string productClass;
            using (var dtOrderProductClass = new OrdersDataSet.OrderProductClassDataTable())
            {
                using (var taOrderProductClass = new Data.Datasets.OrdersDataSetTableAdapters.OrderProductClassTableAdapter())
                {
                    taOrderProductClass.FillByOrder(dtOrderProductClass, orderId);
                }

                var productClassRow = dtOrderProductClass.FirstOrDefault();
                productClass = productClassRow == null || productClassRow.IsProductClassNull()
                    ? string.Empty
                    : productClassRow.ProductClass;
            }

            _orderProductClassCache[orderId] = productClass;
            return productClass;
        }

        private string GetProductClass(ShipmentPackageNode packageNode)
        {
            if (packageNode == null || !packageNode.DataRow.IsValidState())
            {
                return null;
            }

            var orderShipments = packageNode.DataRow.GetOrderShipmentRows();

            if (orderShipments.Length > 0)
            {
                return GetProductClass(orderShipments[0].OrderID);
            }

            return null;
        }

        private bool CompleteShipment(ShipmentPackageNode shipmentNode)
        {
            var shipmentCompletion = new ShipmentCompletion(dsOrderShipment);

            try
            {
                // Unregister value changed handler because it always saves changes
                // to the packages even when it should roll them back on transaction failure.
                pnlShipmentInfo.ValueChanged -= pnlShipmentInfo_ValueChanged;

                //if not a valid user then exit
                if (!SecurityManager.Current.IsValidUser)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("You are not logged-in. Please login and try again.", "Invalid User");
                    return false;
                }

                _log.Info("Completing shipment: " + shipmentNode.Text + " - " + shipmentNode.DataRow.ShipmentPackageID);

                //end current edits to ensure update of binding source
                this.pnlShipmentInfo.EndEditing();

                var shipmentPackageId = shipmentNode.DataRow.ShipmentPackageID;

                var shipRow = dsOrderShipment.ShipmentPackage.FindByShipmentPackageID(shipmentPackageId);
                var orderShipments = shipRow.GetOrderShipmentRows();

                //hide shipment node during shipment completion
                shipmentNode.Visible = false;
                tvwTOC.Nodes[0].Select();

                // Close orders first
                // If completion fails, it can be easily reverted through the Reset Order tool.
                shipmentCompletion.CloseOrders(orderShipments);

                // Then close the shipment
                shipmentCompletion.CloseShipment(shipRow, orderShipments, SecurityManager.Current);

                this.pnlShipmentSummary.AddShipmentLog(shipmentNode.DataRow.CustomerRow.Name, "Package '" + shipmentNode.Text + "' completed.", SecurityManager.Current.UserName, ShippingChange.PackageClosed);

                _log.Info($"Successfully completed shipment {shipmentNode.DataRow.ShipmentPackageID}.");
                return true;
            }
            catch(Exception exc)
            {
                const string errorMsg = "Error completing shipment.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);

                MessageBoxUtilities.ShowMessageBoxError(
                    $"There was an error closing shipment {shipmentNode.DataRow.ShipmentPackageID}. Please close {Text} and try again.",
                    Text);

                return false;
            }
            finally
            {
                try
                {
                    shipmentNode.UpdateNodeUI();
                    pnlShipmentInfo.ValueChanged += pnlShipmentInfo_ValueChanged;
                    UpdateTotalCounts();
                }
                catch (Exception exc)
                {
                    _log.Error(exc, "Error while cleaning up after shipping completion.");
                }

                shipmentCompletion.Dispose();
            }
        }

        private void PrintRepairStatements(OrderShipmentDataSet.ShipmentPackageRow shipPackage)
        {
            try
            {
                if (!ApplicationSettings.Current.RepairStatementEnabled || shipPackage.CustomerAddressRow == null || !shipPackage.CustomerAddressRow.RequireRepairStatement)
                {
                    return;
                }

                var reportData = RepairStatementData.GetReportData(shipPackage.ShipmentPackageID, SecurityManager.Current);
                if (reportData != null)
                {
                    var report = new ReportGenerator(() => new RepairsStatementsReport(reportData));

                    if (UserSettings.Default.Shipping.QuickPrint)
                        report.BeginPrint();
                    else
                        report.Display();
                }
            }
            catch (Exception exc)
            {
                var errorMsg = "Error printing statements of repairs.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void PrintPackingList(OrderShipmentDataSet.ShipmentPackageRow shipPackage)
        {
            try
            {
                if(UserSettings.Default.Shipping.PrintPackingList)
                {
                    var report = new ReportGenerator(() => new PackingListReport(shipPackage, shipPackage.GetOrderShipmentRows()));

                    if (UserSettings.Default.Shipping.QuickPrint)
                    {
                        report.BeginPrint(UserSettings.Default.Shipping.PackingListCount);
                    }
                    else
                    {
                        report.Display();
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error printing package " + shipPackage == null ? NONE : shipPackage.ShipmentPackageID.ToString();
                _log.Error(exc, errorMsg);
            }
        }

        private void UpdateTotalCounts()
        {
            try
            {
                var g = this.guageOrderCount.Gauges[0] as DigitalGauge;

                object partObject = this.taOrderShipment.GetShippedTodayPartCount();
                int partReceivedCount = 0;

                if(partObject != null && partObject != DBNull.Value)
                    int.TryParse(partObject.ToString(), out partReceivedCount);

                g.Text = partReceivedCount.ToString();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error updating total closed order count.";
                _log.Error(exc, errorMsg);
            }
        }

        protected override void OnDispose()
        {
            try
            {
                if (this._scanner != null)
                {
                    this._scanner.Dispose();
                    this._scanner = null;
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error on Shipping Manager form close.";
                _log.Error(exc, errorMsg);
            }

            base.OnDispose();
        }

        private void DisplayOrderMedia(int orderID)
        {
            try
            {
                using(var ta = new Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter())
                {
                    using(var mediaTable = ta.GetDataByOrderNoMedia(orderID))
                    {
                        if(mediaTable.Count > 0)
                            MediaUtilities.OpenMedia(mediaTable[0].MediaID, mediaTable[0].FileExtension, mediaTable[0].IsMediaNull() ? null : mediaTable[0].Media);
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error displaying media.");
            }
        }

        private void SelectShipmentPackageNode(int packageId)
        {
            try
            {
                var node = tvwTOC.Nodes[0].FindNode <ShipmentPackageNode>(p => p.DataRow.ShipmentPackageID == packageId);
                if(node != null)
                    node.Select();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error selecting shipment package id.");
            }
        }

        private int GetNextPackageNumber(int customerId, int shipmentPackageTypeId)
        {
            int nextPackNumber = 1;

            var custPackages = dsOrderShipment.ShipmentPackage.SelectPackages(customerId, shipmentPackageTypeId);

            //if packages already exist then get max package number + 1 for next package
            if(custPackages.Length > 0)
                nextPackNumber = custPackages[0].PackageNumber + 1;

            return nextPackNumber;
        }

        #endregion

        #region Events

        private void ShippingManager_Load(object sender, EventArgs e)
        {
            _loadingData = true; //prevent TOC from loading

            try
            {
                if(DesignMode)
                    return;

                using(new UsingWaitCursor(this))
                {
                    LoadCommands();

                    Application.DoEvents();

                    LoadData();
                    LoadTOC();
                    LoadValidators();

                    _loadingData = false;

                    splitContainer1.Panel2.Enabled = SecurityManager.Current.IsInRole("ShippingManager.Edit");
                    UpdateTotalCounts();
                }
            }
            catch(Exception exc)
            {
                _log.Error(this.dsOrderShipment.GetDataErrors());
                splitContainer1.Enabled = false;
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
            }
        }

        private void _scanner_BarcodingFinished(object sender, BarcodeScanner.BarcodeScannerEventArgs e)
        {
            int orderID;

            if(e.Output != null && int.TryParse(e.Output, out orderID))
            {
                if (e.Postfix == Report.BARCODE_ORDER_ACTION_PREFFIX)
                {
                    AddShipmentExt(orderID);
                }
                else if (e.Postfix == Report.BARCODE_SHIPPING_PACKAGE_PREFIX)
                {
                    SelectShipmentPackageNode(orderID);
                }
                else if (e.Postfix == Report.BARCODE_SALES_ORDER_ACTION_PREFFIX)
                {
                    AddSalesOrderShipmentExt(orderID);
                }
            }
        }

        private void _scanner_BarcodingStarted(object sender, EventArgs e) { NotifyScannerReading(); }

        private void csc_OnClick(object sender, EventArgs e)
        {
            try
            {
                if (tvwTOC.SelectedNodes.Count == 1 && IsValidControls())
                {
                    var shipmentNode = tvwTOC.SelectedNode<ShipmentPackageNode>();

                    if (shipmentNode != null)
                    {
                        bool completedShipment;
                        using (new UsingTimeMe("Complete Shipment", LogLevel.Info))
                        {
                            completedShipment = CompleteShipment(shipmentNode);
                        }

                        if (completedShipment)
                        {
                            //Move print methods after orders closed per 14236 & 14239
                            PrintRepairStatements(shipmentNode.DataRow);
                            PrintPackingList(shipmentNode.DataRow);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error completing shipment", exc);
            }
        }

        private void printOrderLabel_OnClick(object sender, EventArgs e)
        {
            if(tvwTOC.SelectedNodes.Count == 1)
            {
                var shipmentNode = tvwTOC.SelectedNode <ShipmentPackageNode>();

                if(shipmentNode != null)
                {
                    int? orderID = this.pnlShipmentInfo.SelectedOrderID;

                    if(orderID.HasValue)
                        PrintOrderLabel(this.pnlShipmentInfo.SelectedOrderID.GetValueOrDefault());
                }
            }
        }

        private void printPackageLabel_OnClick(object sender, EventArgs e)
        {
            if(tvwTOC.SelectedNodes.Count == 1)
            {
                var shipmentNode = tvwTOC.SelectedNode <ShipmentPackageNode>();

                if(shipmentNode != null)
                    PrintPackageLabel(shipmentNode.DataRow);
            }
        }

        private void displayOrderMedia_OnClick(object sender, EventArgs e)
        {
            if(tvwTOC.SelectedNodes.Count == 1)
            {
                var shipmentNode = tvwTOC.SelectedNode <ShipmentPackageNode>();

                if(shipmentNode != null)
                {
                    int? orderID = this.pnlShipmentInfo.SelectedOrderID;

                    if(orderID.HasValue)
                        DisplayOrderMedia(this.pnlShipmentInfo.SelectedOrderID.GetValueOrDefault());
                }
            }
        }

        private void createPackage_OnClick(object sender, EventArgs e) { AddEmptyPackage(); }

        private void add_CommandClicked(object sender, EventArgs e)
        {
            string valueEntered = String.Empty;

            try
            {
                using(var tb = new TextBoxForm())
                {
                    tb.Text = "Add WO";
                    tb.FormLabel.Text = "Work Order:";
                    tb.FormTextBox.Focus();

                    if(tb.ShowDialog(this) == DialogResult.OK)
                    {
                        valueEntered = tb.FormTextBox.Text;

                        int wo = 0;
                        if(int.TryParse(tb.FormTextBox.Text, out wo))
                            AddShipmentExt(wo);
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error adding new shipment manually. Order ID Entered: " + valueEntered;
                _log.Error(exc, errorMsg);
            }
        }

        private void addSalesOrder_CommandClicked(object sender, EventArgs e)
        {
            var valueEntered = string.Empty;
            try
            {
                using (var tb = new TextBoxForm())
                {
                    tb.Text = "Add SO";
                    tb.FormLabel.Text = "Sales Order:";
                    tb.FormTextBox.Focus();

                    if (tb.ShowDialog(this) == DialogResult.OK)
                    {
                        valueEntered = tb.FormTextBox.Text;

                        if (int.TryParse(tb.FormTextBox.Text, out int salesOrderId))
                        {
                            AddSalesOrderShipmentExt(salesOrderId);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error adding new shipment manually. Sales Order ID Entered: " + valueEntered;
                _log.Error(exc, errorMsg);
            }
        }

        private void AddSalesOrderShipmentExt(int salesOrderId)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(AddSalesOrderShipment), salesOrderId);
            }
            else
            {
                AddSalesOrderShipment(salesOrderId);
            }
        }

        private void BulkCOC_CommandClicked(object sender, EventArgs e)
        {
            try
            {
                var selectedNode = (tvwTOC.SelectedNodes.Count == 1 ? tvwTOC.SelectedNodes[0] : null) as ShipmentPackageNode;

                if (selectedNode == null)
                {
                    return;
                }

                var window = new QA.BulkCOCDialog(selectedNode.DataRow);
                var helper = new WindowInteropHelper(window) { Owner = DWOSApp.MainForm.Handle };
                window.ShowDialog();
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error creating bulk certificate", exc);
            }
        }

        private void RepairStatement_CommandClicked(object sender, EventArgs e)
        {
            try
            {
                var selectedNode = (tvwTOC.SelectedNodes.Count == 1 ? tvwTOC.SelectedNodes[0] : null) as ShipmentPackageNode;

                if (selectedNode == null)
                {
                    return;
                }

                var reportData = RepairStatementData.GetReportData(
                    selectedNode.DataRow.ShipmentPackageID,
                    SecurityManager.Current);

                var report = new RepairsStatementsReport(reportData);
                report.DisplayReport();
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error creating statement of repairs.", exc);
            }
        }


        private void BillOfLading_CommandClicked(object sender, EventArgs e)
        {
            try
            {
                var selectedNode = (tvwTOC.SelectedNodes.Count == 1 ? tvwTOC.SelectedNodes[0] : null) as ShipmentPackageNode;

                if (selectedNode == null)
                {
                    return;
                }

                // Save bill of lading to database
                var billOfLadingRow = dsOrderShipment.BillOfLading.AddBillOfLadingRow(DateTime.Now, selectedNode.DataRow);

                foreach (var orderShipmentRow in selectedNode.DataRow.GetOrderShipmentRows())
                {
                    var billOfLadingOrderRow = dsOrderShipment.BillOfLadingOrder.NewBillOfLadingOrderRow();
                    billOfLadingOrderRow.BillOfLadingRow = billOfLadingRow;
                    billOfLadingOrderRow.OrderID = orderShipmentRow.OrderID;
                    dsOrderShipment.BillOfLadingOrder.AddBillOfLadingOrderRow(billOfLadingOrderRow);
                }

                taManager.UpdateAll(dsOrderShipment);

                // Show/print bill of lading
                using (var billOfLading = new BillOfLadingReport(billOfLadingRow.BillOfLadingID))
                {
                    if (UserSettings.Default.Shipping.QuickPrint)
                    {
                        billOfLading.PrintReport(UserSettings.Default.Shipping.BillOfLadingCount);
                    }
                    else
                    {
                        billOfLading.DisplayReport();
                    }
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error creating bill of lading.", exc);
            }
        }

        private void timerScanUI_Tick(object sender, EventArgs e)
        {
            this.timerScanUI.Stop();
            NotifyScannerReadingReset();
        }

        private void delete_ItemDeleted(object sender, NodeDeletedEventArgs e)
        {
            try
            {
                if(e.Node is ShipmentPackageNode)
                {
                    var shipmentNode = (ShipmentPackageNode) e.Node;

                    this.taManager.UpdateAll(this.dsOrderShipment);

                    this.pnlShipmentSummary.AddShipmentLog("Unknown", "Package '" + shipmentNode.Text + "' canceled.", SecurityManager.Current.UserName, ShippingChange.PackageDeleted);
                    pnlShipmentSummary.RefreshData();
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error publishing change of package.";
                _log.Error(exc, errorMsg);
            }
            finally
            {
                UpdateTotalCounts();
            }
        }

        private void pnlShipmentInfo_OrderDeleted(object sender, ShipmentInfo.OrderDeletedEventArgs e)
        {
            try
            {
                //delete row from database
                DataRow[] orderRows = this.dsOrderShipment.OrderShipment.Select(this.dsOrderShipment.OrderShipment.ShipmentIDColumn.ColumnName + " = " + e.OrderShipmentID, "", DataViewRowState.Deleted);
                if(orderRows != null)
                {
                    foreach(var or in orderRows)
                        this.taOrderShipment.Update(or);
                }

                //notify everyone else
                _log.Info("On Order Deleted: " + e.OrderShipmentID);

                string customerName = "Unknown";
                OrderShipmentDataSet.ShipmentPackageRow pack = this.dsOrderShipment.ShipmentPackage.FindByShipmentPackageID(e.ShipmentPackageID);

                if(pack != null)
                    customerName = pack.CustomerRow.Name;

                this.pnlShipmentSummary.AddShipmentLog(customerName, "Order " + e.OrderShipmentID + " deleted.", SecurityManager.Current.UserName, ShippingChange.OrderRemoved);
            }
            catch(Exception exc)
            {
                string errorMsg = "Error publishing change of order in package.";
                _log.Error(exc, errorMsg);
            }
            finally
            {
                UpdateTotalCounts();
            }
        }

        private void toolbarManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if(e.Tool.Key == "Settings")
                this.ctlShippingSettings.LoadSettings();
        }

        private void toolbarManager_AfterToolCloseup(object sender, ToolDropdownEventArgs e)
        {
            if(e.Tool.Key == "Settings")
                this.ctlShippingSettings.SaveSettings();
        }

        private void pnlShipmentInfo_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                var shipmentPackage = this.pnlShipmentInfo.CurrentRecord as OrderShipmentDataSet.ShipmentPackageRow;

                if(shipmentPackage != null)
                    this.taShipmentPackage.Update(shipmentPackage);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating shipment package change.");
            }
        }

        #endregion

        #region Nodes

        #region Nested type: ShipmentPackageNode

        internal class EmptyPackageNode : UltraTreeNode, IDeleteNode
        {
            public EmptyPackageNode()
            {
                Text = "Empty Box";
                Override.NodeAppearance.Image = Properties.Resources.Package_Empty_16;
            }

            public bool CanDelete => true;

            public void Delete()
            {
                Remove();
            }
        }

        internal class ShipmentPackageNode : DataNode <OrderShipmentDataSet.ShipmentPackageRow>, IReportNode
        {
            #region Fields

            public const string KEY_PREFIX = "PA";
            private static Image _imageCache;
            private static Image _imageEmptyCache;
            public int PartUsageCount = -1;
            private readonly OrderShipmentDataSet.ShipmentPackageDataTable _dtShipmentPackage;

            #endregion

            #region Properties

            #endregion

            #region Methods

            public ShipmentPackageNode(OrderShipmentDataSet.ShipmentPackageRow cr) : base(cr, cr.ShipmentPackageID.ToString(), KEY_PREFIX, "Shipment")
            {
                _dtShipmentPackage = cr.Table as OrderShipmentDataSet.ShipmentPackageDataTable;

                if(_imageCache == null)
                {
                    _imageCache = Properties.Resources.Package_16;
                    _imageEmptyCache = Properties.Resources.Package_Empty_16;
                }

                LeftImages.Add(_imageCache);

                UpdateNodeUI();
            }

            public override void UpdateNodeUI()
            {
                try
                {
                    if(base.DataRow.RowState == DataRowState.Deleted)
                        return;

                    //find total number of packages for this customer
                    if(DataRow != null && DataRow.RowState != DataRowState.Deleted && DataRow.RowState != DataRowState.Detached)
                    {
                        var rows = _dtShipmentPackage.SelectPackages(DataRow.CustomerID, DataRow.ShipmentPackageTypeID);

                        var maxPackageNumbers = rows.Length > 0 ? Convert.ToInt32(rows[0]["PackageNumber"]) : 0;
                        var packageTypeName = DataRow.ShipmentPackageTypeRow.Name;
                        Text = base.DataRow.CustomerRow.Name +
                               $" [{packageTypeName} {DataRow.PackageNumber} of {maxPackageNumbers}]";

                        //Update image
                        LeftImages.Clear();
                        if(DataRow.GetOrderShipmentRows().Any())
                            LeftImages.Add(_imageCache);
                        else
                            LeftImages.Add(_imageEmptyCache);

                        Visible = DataRow.Active;
                    }
                    else
                        Visible = false;
                }
                catch(Exception exc)
                {
                    const string errorMsg = "Error updating node UI.";
                    LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
                }
            }

            public OrderShipmentDataSet.OrderShipmentRow[] GetOrderShipments()
            {
                if(base.DataRow != null)
                {
                    DataTable dt = DataRow.Table.DataSet.Tables["OrderShipment"];
                    return dt.Select("ShipmentPackageID = " + ID) as OrderShipmentDataSet.OrderShipmentRow[];
                }

                return new OrderShipmentDataSet.OrderShipmentRow[] {};
            }

            #endregion

            #region IReportNode Members

            public IReport CreateReport(string reportType)
            {
                var report = new ShippingDetailReport();
                report.FromDate = report.ToDate = DateTime.Now;
                return report;
            }

            public string[] ReportTypes() { return null; }

            #endregion
        }

        #endregion

        #region Nested type: ShipmentPackageNodeSorter

        public class ShipmentPackageNodeSorter : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                if(x is ShipmentPackageNode && y is ShipmentPackageNode)
                {
                    var xNode = (ShipmentPackageNode) x;
                    var yNode = (ShipmentPackageNode) y;

                    //if names are not the same then compare them
                    if(xNode.DataRow.CustomerID != yNode.DataRow.CustomerID)
                        return xNode.DataRow.CustomerRow.Name.CompareTo(yNode.DataRow.CustomerRow.Name);
                    //else compare package numbers
                    return xNode.DataRow.PackageNumber.CompareTo(yNode.DataRow.PackageNumber);
                }
                if(x is UltraTreeNode && y is UltraTreeNode)
                    return ((UltraTreeNode) x).Text.CompareTo(((UltraTreeNode) y).Text);
                return 0;
            }

            #endregion
        }

        #endregion

        #region Nested type: ShipmentPackagesRootNode

        internal class ShipmentPackagesRootNode : UltraTreeNode, IReportNode
        {
            #region Fields

            #endregion

            #region Methods

            public ShipmentPackagesRootNode() : base("ROOT", "Packages") { LeftImages.Add(Properties.Resources.Package_16); }

            public string SortKey => 0.ToString();

            #endregion

            #region IReportNode Members

            public IReport CreateReport(string reportType)
            {
                var report = new ShippingDetailReport();
                report.FromDate = report.ToDate = DateTime.Now;
                return report;
            }

            public string[] ReportTypes() { return null; }

            #endregion
        }

        #endregion

        #endregion

        #region Commands

        #region Nested type: AddShipmentCommand

        internal class AddShipmentCommand : TreeNodeCommandBase
        {
            #region Fields

            public event EventHandler CommandClicked;

            #endregion

            #region Methods

            public AddShipmentCommand(ToolBase tool, UltraTree toc) : base(tool) { base.TreeView = toc; }

            public override void OnClick()
            {
                if(Enabled)
                    CommandClicked(this, EventArgs.Empty);
            }

            #endregion
        }

        #endregion

        #region Nested type: AddSalesOrderShipmentCommand

        internal class AddSalesOrderShipmentCommand : TreeNodeCommandBase
        {
            #region Fields

            public event EventHandler CommandClicked;

            #endregion

            #region Methods

            public AddSalesOrderShipmentCommand(ToolBase tool, UltraTree toc) : base(tool)
            {
                TreeView = toc;
            }

            public override void OnClick()
            {
                if (Enabled)
                {
                    CommandClicked(this, EventArgs.Empty);
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: RefreshCommand

        internal class RefreshCommand : TreeNodeCommandBase
        {
            #region Fields

            private ShippingManager _manager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.TreeView != null; }
            }

            #endregion

            #region Methods

            public RefreshCommand(ToolBase tool, ShippingManager manager) : base(tool)
            {
                base.TreeView = manager.tvwTOC;
                this._manager = manager;
            }

            public override void OnClick()
            {
                try
                {
                    if(Enabled)
                        this._manager.ReloadTOC();
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error refreshing shipping manager.");
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: PackageCommand

        internal class PackageCommand : TreeNodeCommandBase
        {
            #region Fields

            public event EventHandler CommandClicked;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _node is ShipmentPackageNode && ((ShipmentPackageNode)_node).GetOrderShipments().Length > 0; }
            }

            #endregion

            #region Methods

            public PackageCommand(ToolBase tool, UltraTree toc) : base(tool) { base.TreeView = toc; }

            public override void OnClick()
            {
                CommandClicked?.Invoke(this, EventArgs.Empty);
            }

            #endregion
        }

        #endregion

        #region Nested type: PrintOrderLabelCommand

        internal class PrintOrderLabelCommand : TreeNodeCommandBase
        {
            #region Fields

            public event EventHandler CommandClicked;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _node is ShipmentPackageNode && ((ShipmentPackageNode) _node).GetOrderShipments().Length > 0; }
            }

            #endregion

            #region Methods

            public PrintOrderLabelCommand(ToolBase tool, UltraTree toc) : base(tool) { base.TreeView = toc; }

            public override void OnClick()
            {
                if(!UserSettings.Default.Shipping.PrintOrderLabel)
                    MessageBoxUtilities.ShowMessageBoxWarn("Labels have not been enabled to print. Please enable the labels to be printed in the File > Settings menu.", "Label Printing Disabled");
                else if(CommandClicked != null)
                    CommandClicked(this, EventArgs.Empty);
            }

            #endregion
        }

        #endregion

        #region Nested type: PrintPackageLabelCommand

        internal class PrintPackageLabelCommand : TreeNodeCommandBase
        {
            #region Fields

            public event EventHandler CommandClicked;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _node is ShipmentPackageNode && ((ShipmentPackageNode) _node).GetOrderShipments().Length > 0; }
            }

            #endregion

            #region Methods

            public PrintPackageLabelCommand(ToolBase tool, UltraTree toc) : base(tool) { base.TreeView = toc; }

            public override void OnClick()
            {
                if(!UserSettings.Default.Shipping.PrintOrderLabel)
                    MessageBoxUtilities.ShowMessageBoxWarn("Labels have not been enabled to print. Please enable the labels to be printed in the File > Settings menu.", "Label Printing Disabled");
                else if(CommandClicked != null)
                    CommandClicked(this, EventArgs.Empty);
            }

            #endregion
        }

        #endregion

        #region Nested type: CreatePackageCommand

        internal class CreatePackageCommand : CommandBase
        {
            #region Fields

            public event EventHandler CommandClicked;

            #endregion

            #region Methods

            public CreatePackageCommand(ToolBase tool) : base(tool) { base.Refresh(); }

            public override void OnClick()
            {
                if(CommandClicked != null)
                    CommandClicked(this, EventArgs.Empty);
            }

            #endregion
        }

        #endregion

        #endregion
    }
}