using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderInvoiceDataSetTableAdapters;
using DWOS.Data.Invoice;
using DWOS.Data.Order;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DWOS.UI.Admin
{
    public class ExportCSV : IInvoiceExporter
    {
        #region Fields

        public const string COL_EXPORTED = "Exported";
        public const string COL_EXPORT = "Export";
        public const string COL_ISSUES = "Issues";

        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        private List<InvoiceD> _invoices = new List<InvoiceD>();
        private DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter _taOrders;
        private OrderInvoiceDataSet _dsOrderInvoice;
        private OrderInvoiceCustomerTableAdapter _taCustomer;
        private OrderFeesTableAdapter _taOrderFees;
        private OrderInvoiceTableAdapter _taOrderInvoice;
        private OrderShipmentTableAdapter _taOrderShipmentTable;
        private PartSummaryTableAdapter _taPartSummary;
        private OrderProcessingDetailTableAdapter _taOrderProcessingDetail;
        private d_DepartmentTableAdapter _taDepartment;
        private InvoiceFieldMapper _fieldMapper;
        private int _errorCount = 0;
        private List<int> _exportedOrdersIDs;
        private string _priceFormat = null;
        private readonly IProductClassPersistence _productClassPersistence;

        public event ProgressChangedEventHandler ProgessChanged;

        #endregion

        #region Properties

        public OrderInvoiceDataSet OrderInvoices
        {
            get { return this._dsOrderInvoice; }
        }

        public InvoiceFieldMapper FieldMapper
        {
            get
            {
                if (_fieldMapper == null)
                    _fieldMapper = new InvoiceFieldMapper();

                return _fieldMapper;
            }

            set { _fieldMapper = value; }
        }

        public int ErrorCount
        {
            get { return _errorCount; }
            set { _errorCount = value; }
        }

        public List<int> ExportedOrderIDs
        {
            get 
            {
                if (_exportedOrdersIDs == null)
                    _exportedOrdersIDs = new List<int>();

                return _exportedOrdersIDs; 
            }

            set { _exportedOrdersIDs = value; }
        }

        public List<InvoiceD> ExportedInvoices
        {
            get 
            {
                if(_invoices == null)
                    _invoices = new List<InvoiceD>();

                return _invoices; 
            }
            
            set { _invoices = value; }
        }

        public bool Cancelled { get; set; }

        private string PriceFormatString
        {
            get
            {
                if(_priceFormat == null)
                    _priceFormat = "{0:." + "0".Repeat(ApplicationSettings.Current.PriceDecimalPlaces) + "}";

                return _priceFormat;
            }
        }

        #endregion

        #region Methods

        public ExportCSV()
        {
            _productClassPersistence = new ProductClassPersistence();
        }
    
        /// <summary>
        /// Loads the datasets from the DWOS database for the export operations.
        /// </summary>
        public void LoadData()
        {
            _log.Info("Loading data.");

            this._taOrders = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter();
            this._dsOrderInvoice = new OrderInvoiceDataSet();
            this._taOrderInvoice = new OrderInvoiceTableAdapter();
            this._taCustomer = new OrderInvoiceCustomerTableAdapter();
            this._taPartSummary = new PartSummaryTableAdapter();
            this._taOrderFees = new OrderFeesTableAdapter();
            this._taOrderShipmentTable = new OrderShipmentTableAdapter();
            this._taOrderProcessingDetail = new OrderProcessingDetailTableAdapter();
            this._taDepartment = new d_DepartmentTableAdapter();

            this._dsOrderInvoice.EnforceConstraints = false;

            this._dsOrderInvoice.OrderInvoice.BeginLoadData();
            this._dsOrderInvoice.OrderInvoiceCustomer.BeginLoadData();
            this._dsOrderInvoice.PartSummary.BeginLoadData();
            this._dsOrderInvoice.OrderFees.BeginLoadData();
            this._dsOrderInvoice.OrderShipment.BeginLoadData();
            this._dsOrderInvoice.OrderProcessingDetail.BeginLoadData();
            this._dsOrderInvoice.d_Department.BeginLoadData();

            this._taOrderInvoice.Fill(this._dsOrderInvoice.OrderInvoice);
            this._taCustomer.Fill(this._dsOrderInvoice.OrderInvoiceCustomer);
            this._taPartSummary.Fill(this._dsOrderInvoice.PartSummary);
            this._taOrderFees.Fill(this._dsOrderInvoice.OrderFees);
            this._taOrderShipmentTable.Fill(this._dsOrderInvoice.OrderShipment);
            this._taOrderProcessingDetail.Fill(this._dsOrderInvoice.OrderProcessingDetail);
            this._taDepartment.Fill(this._dsOrderInvoice.d_Department);

            this._dsOrderInvoice.OrderInvoice.EndLoadData();
            this._dsOrderInvoice.OrderInvoiceCustomer.EndLoadData();
            this._dsOrderInvoice.PartSummary.EndLoadData();
            this._dsOrderInvoice.OrderFees.EndLoadData();
            this._dsOrderInvoice.OrderShipment.EndLoadData();
            this._dsOrderInvoice.OrderProcessingDetail.EndLoadData();
            this._dsOrderInvoice.d_Department.EndLoadData();

            //Add column if does not exist
            if (!this._dsOrderInvoice.OrderInvoice.Columns.Contains(COL_EXPORT))
                this._dsOrderInvoice.OrderInvoice.Columns.Add(COL_EXPORT, typeof(bool)).DefaultValue = false;

            if (!this._dsOrderInvoice.OrderInvoice.Columns.Contains(COL_EXPORTED))
                this._dsOrderInvoice.OrderInvoice.Columns.Add(COL_EXPORTED, typeof(bool)).DefaultValue = false;

            if (!this._dsOrderInvoice.OrderInvoice.Columns.Contains(COL_ISSUES))
                this._dsOrderInvoice.OrderInvoice.Columns.Add(COL_ISSUES, typeof(string));

            //Setup the invoice exporting based on customer preference
            this.SetupExport();
        }

        #region Invoice Level

        /// <summary>
        /// Setup the export based on the customer invoicing level specified
        /// </summary>
        private void SetupExport()
        {
            try
            {
                //Set the invoice name and the export based on the invoicing level
                foreach (OrderInvoiceDataSet.OrderInvoiceRow order in this.OrderInvoices.OrderInvoice)
                {
                    var invoiceLevel = InvoiceHelpers.GetLevel(order);

                    switch (invoiceLevel)
                    {
                        case InvoiceLevelType.SalesOrder:
                            {
                                if (ApplicationSettings.Current.IndexSOInvoices)
                                    order["Invoice"] = InvoiceHelpers.GetSOInvoiceWithSuffix(order);
                                else
                                    order["Invoice"] = InvoiceHelpers.GetSOInvoice(order);

                                this.CheckSalesOrder(order);
                            }
                            break;
                        case InvoiceLevelType.Default:
                        case InvoiceLevelType.Package:
                        case InvoiceLevelType.WorkOrder:
                            {
                                //user may want to edit invoice name so load what it will be when exported and allow user to edit
                                order["Invoice"] = InvoiceHelpers.GetInvoiceWithPrefix(order);
                                this.CheckOrder(order, true);
                            }
                            break;
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error setting up the invoice export.");
            }
        }

        /// <summary>
        /// Set order export checked value
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="value">The value.</param>
        private void CheckOrder(OrderInvoiceDataSet.OrderInvoiceRow order, bool value)
        {
            order[COL_EXPORT] = value;
        }

        /// <summary>
        /// Set order export checked value based on any open orders contained within the sales order
        /// </summary>
        /// <param name="order">The order.</param>
        private void CheckSalesOrder(OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            //Check if sales order has open orders (all have to be closed to auto check them) but customer can overide this in UI
            if (order.IsSalesOrderIDNull())
                CheckOrder(order, true); //No sales order
            else if (OpenOrdersInSalesOrder(order.SalesOrderID))
                CheckOrder(order, false);
            else
                CheckOrder(order, true);
        }

        /// <summary>
        /// Determines if the sales order has any open orders.
        /// </summary>
        /// <param name="salesOrderID">The sales order identifier.</param>
        /// <returns></returns>
        private bool OpenOrdersInSalesOrder(int salesOrderID)
        {
            bool openOrders = false;

            OrdersDataSet.OrderDataTable dtOrder = new OrdersDataSet.OrderDataTable();

            this._taOrders.FillBySalesOrder(dtOrder, salesOrderID);
            foreach (OrdersDataSet.OrderRow row in dtOrder.Rows)
            {
                if (row.Status.EquivalentTo("Open"))
                {
                    openOrders = true;
                    break;
                }
            }

            return openOrders;
        }

        #endregion

        /// <summary>
        /// Exports the invoices to CSV.
        /// </summary>
        /// <returns>Returns the results of the export.</returns>
        public InvoiceResult Export()
        {
            int succesfulExportCount = 0;

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Export Invoices to CSV file";
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
                saveFileDialog.InitialDirectory = UserSettings.Default.LastInvoiceDirectory;

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    this.Cancelled = true;
                    _log.Info("User canceled on export invoices dialog");
                    return new InvoiceResult()
                    {
                        ExportedCount = 0,
                        TotalCount = OrderInvoices.OrderInvoice.Count,
                        ExportedOrderIDs = null,
                        ErrorCount = 0,
                        Cancelled = true
                    };
                }

                _invoices = new List<InvoiceD>();
                string fileName = saveFileDialog.FileName;
                UserSettings.Default.LastInvoiceDirectory = Path.GetDirectoryName(fileName);
                UserSettings.Default.Save();

                //Get the export field mappings based on customer defined fields to export
                _fieldMapper = new InvoiceFieldMapper();

                try
                {
                    //Create a text file for the invoices
                    using (StreamWriter sw = File.CreateText(fileName))
                    {
                        //Write header columns
                        if(ApplicationSettings.Current.InvoiceHeaderRow)
                            sw.WriteLine(_fieldMapper.ExportInvoiceHeader());

                        //Export the invoices
                        succesfulExportCount = this.ExportInvoices();

                        if (succesfulExportCount > 0)
                        {
                            //Write the exported invoice to the CSV file
                            foreach (InvoiceD invoice in _invoices)
                            {
                                //Ok to export
                                if (!invoice.Exported)
                                {
                                    continue;
                                }

                                foreach (var line in _fieldMapper.ExportInvoice(invoice))
                                {
                                    sw.WriteLine(line);
                                }

                                //Write the invoice fees to the CSV file
                                foreach (var fee in _fieldMapper.ExportInvoiceFees(invoice))
                                {
                                    sw.WriteLine(fee);
                                }
                            }
                        }
                    }

                    _log.Info(succesfulExportCount + " invoices exported to: " + fileName);

                    //Show the exported file
                    Shared.Utilities.FileLauncher.New()
                        .HandleErrorsWith((exception, errorMsg) => MessageBoxUtilities.ShowMessageBoxWarn(errorMsg, "Unable to Start", fileName))
                        .Launch(fileName);
                }
                catch (IOException exc)
                {
                    //File is being used by another process
                    MessageBoxUtilities.ShowMessageBoxWarn("File is being used by another process.", "File Error", fileName);
                    _log.Debug("File is being used by another process: " + fileName, exc);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error exporting invoices to CSV file.");
            }

            return new InvoiceResult()
            {
                ExportedCount = succesfulExportCount,
                TotalCount = OrderInvoices.OrderInvoice.Count,
                ExportedOrderIDs = ExportedOrderIDs,
                ErrorCount = ErrorCount,
                Cancelled = false
            };
        }

        public int ExportInvoices()
        {
            int totalToImportCount = 0;
            int succesfulExportCount = 0;
            int exportCount = 0;

            try
            {
                if (this.ProgessChanged != null)
                    this.ProgessChanged(this, new ProgressChangedEventArgs(0, null));

                if (this._dsOrderInvoice == null)
                    this.LoadData();

                totalToImportCount = this._dsOrderInvoice.OrderInvoice.Count;
                this.ExportedOrderIDs.Clear();
                _errorCount = 0;

                var taInvoiceProcesses = new InvoiceProcessesTableAdapter();
               
                foreach (OrderInvoiceDataSet.OrderInvoiceRow order in this._dsOrderInvoice.OrderInvoice)
                {
                    order[COL_EXPORTED] = DBNull.Value;
                    order[COL_ISSUES] = DBNull.Value;

                    
                    OrderInvoiceDataSet.InvoiceProcessesRow InvoiceProcesses = (OrderInvoiceDataSet.InvoiceProcessesRow)taInvoiceProcesses.GetProcessesByOrderID(order.OrderID).Rows[0];
                    //Only export the ones that are checked in UI grid
                    if (Convert.ToBoolean(order[COL_EXPORT]))
                    {
                        try
                        {
                            string issues;
                            string invoiceID = this.ExportOrder(order, InvoiceProcesses, out issues);

                            // ExportOrder may have returned a default order ID
                            if (order.IsInvoiceNull() || invoiceID != order.Invoice)
                            {
                                order.Invoice = invoiceID;
                            }

                            order[COL_ISSUES] = issues;

                            if (String.IsNullOrEmpty(issues))
                            {
                                this._taOrderInvoice.AddInvoice(invoiceID, order.OrderID);

                                if (!order.IsSalesOrderIDNull())
                                {
                                    this._taOrderInvoice.AddSalesOrderInvoice(invoiceID, order.SalesOrderID);
                                }

                                this.ExportedOrderIDs.Add(order.OrderID);
                                order[COL_EXPORTED] = true;
                                succesfulExportCount++;
                            }
                        }
                        catch (Exception exc)
                        {
                            _errorCount++;
                            order[COL_ISSUES] = "Error exporting invoice: " + exc.Message;
                            order[COL_EXPORTED] = false;
                            _log.Error(exc, "Error exporting invoice.");
                        }

                        exportCount++;

                        if (this.ProgessChanged != null)
                            this.ProgessChanged(this, new ProgressChangedEventArgs(Convert.ToInt32((exportCount / (double)totalToImportCount) * 100.0), null));

                        if (_errorCount >= ApplicationSettings.Current.InvoiceExportMaxErrors)
                        {
                            _log.Info("Aborting the export process, max number of errors per session has occurred.");
                            break;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error exporting invoices to CSV.");
            }

            _log.Info("Exported invoices: " + succesfulExportCount);

            return succesfulExportCount;
        }

        /// <summary>
        /// Exports the order and return the invoice id that was created.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="issues">The issues.</param>
        /// <returns>
        /// ID of created invoice; may be a default one if order's is null
        /// </returns>
        private string ExportOrder(OrderInvoiceDataSet.OrderInvoiceRow order, OrderInvoiceDataSet.InvoiceProcessesRow Processes, out string issues)
        {
            InvoiceD invoice = new InvoiceD();
            issues = null;

            try
            {
                _log.Debug("Begin importing Invoice: " + order.OrderID);

                string invoiceID = (order.IsInvoiceNull() ? InvoiceHelpers.GetInvoiceWithPrefix(order) : order.Invoice);

                OrderInvoiceDataSet.OrderInvoiceCustomerRow customer = order.OrderInvoiceCustomerRow;
                invoice.OrderID = order.OrderID.ToString();
                invoice.InvoiceID = invoiceID;
                invoice.CustomerName = customer.Name;
                invoice.CustomerID = customer.IsAccountingIDNull() ? customer.Name : customer.AccountingID.ToString();
                invoice.WO = invoice.OrderID;
                invoice.PO = order.IsPurchaseOrderNull() ? "" : order.PurchaseOrder;
                invoice.Terms = order.OrderInvoiceCustomerRow.IsPaymentTermsNull() ? "" : order.OrderInvoiceCustomerRow.PaymentTerms;
                invoice.ShipDate = order.IsCompletedDateNull() ? DateTime.Now.ToShortDateString() : order.CompletedDate.ToShortDateString();
                invoice.DueDate = CalculateDueDate(order).ToShortDateString();
                invoice.DateCreated = order.IsOrderDateNull() ? DateTime.Now.ToShortDateString() : order.OrderDate.ToShortDateString();

                var orderTotalBeforeFees = OrderPrice.CalculatePrice(
                    order.IsBasePriceNull() ? 0M : order.BasePrice,
                    order.PriceUnit,
                    0M,
                    order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                    order.IsWeightNull() ? 0M : order.Weight);

                invoice.PartName = order.PartSummaryRow.Name;
                
                invoice.PartDesc = !(order.PartSummaryRow.IsDescriptionNull())?order.PartSummaryRow.Description:"";
                invoice.Quantity = (order.IsPartQuantityNull() ? 0M : order.PartQuantity).ToString();
                invoice.PriceUnit = order.PriceUnit;
                invoice.Item = ApplicationSettings.Current.InvoicePartItemName;
                //Get Processes
                invoice.Processes      = !(Processes.IsProcess_NameNull())?Processes.Process_Name:"-none-";
                invoice.ProcessAliases = !(Processes.IsProcess_AliasNull()) ? Processes.Process_Alias:"-none-";
                invoice.ProcessDesc    = !(Processes.IsProcess_DescNull()) ? Processes.Process_Desc:"-none-";

                var processingDetails = order.GetOrderProcessingDetailRows()
                    .Where(row => row.IsValidState())
                    .ToList();

                bool useDeptLevelInvoicing = ApplicationSettings.Current.InvoiceLineItemType == InvoiceLineItemType.Department &&
                    ApplicationSettings.Current.PartPricingType == PricingType.Process &&
                    processingDetails.Count > 0 &&
                    processingDetails.Any(row => !row.IsAmountNull() && row.Amount != 0M);

                if (useDeptLevelInvoicing)
                {
                    invoice.LineItems.AddRange(CreateDepartmentLineItems(order));
                }
                else
                {
                    //Add 'Part Processing' Invoice Item
                    var lineItem = new InvoiceLineItem();
                    lineItem.SalesAccount = GetItemCode(order);
                    lineItem.Description = order.PartSummaryRow.Name;
                    lineItem.ExtPrice = string.Format(PriceFormatString, orderTotalBeforeFees);
                    lineItem.LineItem = "1";

                    bool usingLotPricing = order.PriceUnit.EquivalentTo(OrderPrice.enumPriceUnit.Lot.ToString()) ||
                        order.PriceUnit.EquivalentTo(OrderPrice.enumPriceUnit.LotByWeight.ToString());

                    if (ApplicationSettings.Current.InvoiceCalcUnitPrice && usingLotPricing)
                    {
                        lineItem.BasePrice = order.IsBasePriceNull() ?
                            string.Empty :
                            OrderPrice.CalculateEachPrice(
                                order.IsBasePriceNull() ? 0M : order.BasePrice,
                                order.PriceUnit,
                                order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                                order.IsWeightNull() ? 0M : order.Weight).ToString();
                    }
                    else
                    {
                        lineItem.BasePrice = order.IsBasePriceNull() ? string.Empty
                            : String.Format(PriceFormatString, order.BasePrice);
                    }

                    invoice.LineItems.Add(lineItem);
                }

                int lineItemCount = invoice.LineItems.Count;

                //Add Shipping
                OrderInvoiceDataSet.OrderShipmentRow[] shipments = order.GetOrderShipmentRows();
                if (shipments.Length > 0)
                {
                    invoice.Shipping = shipments[0].ShippingCarrierID;
                    invoice.ShipDate = shipments[0].DateShipped.ToShortDateString();
                    invoice.TrackingNumber = shipments[0].IsTrackingNumberNull() ? "-None-" : shipments[0].TrackingNumber;
                }

                if (!order.IsBasePriceNull())
                {
                    //Add Fees
                    foreach (OrderInvoiceDataSet.OrderFeesRow fee in order.GetOrderFeesRows())
                    {
                        lineItemCount += 1;
                        invoice.FeeItems.Add(ExportOrderFee(order, fee, lineItemCount));
                    }

                    invoice.Exported = true;
                }
                else
                {
                    _errorCount++;
                    order[COL_ISSUES] = invoice.Issues = "No price set for WO: " + order.OrderID;
                    order[COL_EXPORTED] = false;
                    invoice.Exported = false;
                }

                //Add main invoice
                _invoices.Add(invoice);
            }
            catch (Exception exc)
            {
                _errorCount++;
                issues = "Error exporting invoice: " + exc.Message;
                _log.Error(exc, "Error exporting invoice.");
            }

            return invoice.InvoiceID;
        }

        private string GetItemCode(OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            if (ApplicationSettings.Current.InvoiceLineItemType != InvoiceLineItemType.ProductClass)
            {
                return ApplicationSettings.Current.InvoicePartItemCode;
            }

            var productClass = _productClassPersistence.RetrieveForOrder(order.OrderID);

            if (productClass == null)
            {
                return ApplicationSettings.Current.InvoicePartItemCode;
            }

            if (!string.IsNullOrEmpty(productClass.AccountingCode))
            {
                return productClass.AccountingCode;
            }

            return productClass.Name;
        }

        private IEnumerable<InvoiceLineItem> CreateDepartmentLineItems(OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            if (order == null)
            {
                return null;
            }

            var lineItems = new List<InvoiceLineItem>();

            var orderWeight = order.IsWeightNull() ? 0M : order.Weight;

            var orderBasePrice = order.IsBasePriceNull() ? 0 : order.BasePrice;
            int lineItemCount = 0; // assume that these line items are first

            var processingDetails = order.GetOrderProcessingDetailRows()
                .Where(row => row.IsValidState())
                .ToList();

            var processingBasePrice = processingDetails.Sum(row => row.IsAmountNull() ? 0M : row.Amount);

            var processingFactor = 0M;

            if (processingBasePrice != 0)
            {
                processingFactor = orderBasePrice / processingBasePrice;
            }

            var detailsGrouped = processingDetails
                 .Where(row => (row.IsAmountNull() ? 0M : row.Amount) > 0M)
                .GroupBy(r => r.Department);

            bool usingLotPricing = order.PriceUnit.EquivalentTo(OrderPrice.enumPriceUnit.Lot.ToString()) ||
                order.PriceUnit.EquivalentTo(OrderPrice.enumPriceUnit.LotByWeight.ToString());

            foreach (var detailGroup in detailsGrouped)
            {
                lineItemCount += 1;
                var departmentName = detailGroup.Key;
                var departmentAmount = detailGroup.Sum(row => row.IsAmountNull() ? 0M : row.Amount);

                string salesAccount = null;

                if (!detailGroup.First().d_DepartmentRow.IsAccountingCodeNull())
                {
                    salesAccount = detailGroup.First().d_DepartmentRow.AccountingCode;
                }

                if (string.IsNullOrEmpty(salesAccount))
                {
                    salesAccount = departmentName;
                }

                var lineItemBasePrice = departmentAmount * processingFactor;

                var lineItemAmount = OrderPrice.CalculatePrice(lineItemBasePrice,
                    order.IsPriceUnitNull() ? string.Empty : order.PriceUnit,
                    0M,
                    order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                    orderWeight);

                var lineItem = new InvoiceLineItem();
                lineItem.SalesAccount = salesAccount;
                lineItem.Description = order.PartSummaryRow.Name;
                lineItem.LineItem = lineItemCount.ToString();
                lineItem.ExtPrice = string.Format(PriceFormatString, lineItemAmount);

                if (ApplicationSettings.Current.InvoiceCalcUnitPrice && usingLotPricing)
                {
                    var lineItemPrice = OrderPrice.CalculateEachPrice(
                        lineItemBasePrice,
                        order.PriceUnit,
                        order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                        orderWeight);

                    lineItem.BasePrice = lineItemPrice.ToString();
                }
                else
                {
                    lineItem.BasePrice = string.Format(PriceFormatString, lineItemBasePrice);
                }

                lineItems.Add(lineItem);
            }

            return lineItems;
        }

        private InvoiceFeeItem ExportOrderFee(OrderInvoiceDataSet.OrderInvoiceRow order, OrderInvoiceDataSet.OrderFeesRow fee, int lineItemCount)
        {
            var invoiceFee = new InvoiceFeeItem();

            string itemName;
            using (var taFeeType = new Data.Datasets.OrdersDataSetTableAdapters.OrderFeeTypeTableAdapter())
            {
                itemName = taFeeType.GetInvoiceItemName(fee.OrderFeeTypeID);
                if (string.IsNullOrEmpty(itemName))
                    itemName = "Processing Fee";

                //Fixed or percentage
                invoiceFee.FeeType = taFeeType.GetFeeType(fee.OrderFeeTypeID);
                invoiceFee.SalesAccount = itemName;
                invoiceFee.Item = fee.OrderFeeTypeID;

                invoiceFee.LineItemNum = lineItemCount.ToString();
            }

            //if RUSH then set to order priority versus just rush
            string orderFeeType = fee.OrderFeeTypeID;
            if (orderFeeType == "Rush" && !order.IsPriorityNull())
                orderFeeType = order.Priority;

            invoiceFee.OrderFeeType = orderFeeType;
            invoiceFee.Quantity = "1";

            if (invoiceFee.FeeType.EquivalentTo(OrderPrice.enumFeeType.Fixed.ToString()))
            {
                invoiceFee.Amount = String.Format(PriceFormatString, fee.Charge);
                invoiceFee.ExtAmount = String.Format(PriceFormatString, fee.Charge);
                invoiceFee.Description = orderFeeType;
            }
            else if (invoiceFee.FeeType.EquivalentTo(OrderPrice.enumFeeType.Percentage.ToString()))
            {
                //Per customer rule
                invoiceFee.FeeType = "Pct";

                //Fee percentage amount
                invoiceFee.Description = orderFeeType + " - " + String.Format("{0}%", fee.Charge);


                //Calculate fee
                var fees = OrderPrice.CalculateFees("Percentage",
                    fee.Charge,
                    order.IsBasePriceNull() ? 0M :order.BasePrice,
                    order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                    order.PriceUnit,
                    order.IsWeightNull() ? 0M : order.Weight);

                invoiceFee.Amount = string.Format(PriceFormatString, fees);
                invoiceFee.ExtAmount = invoiceFee.Amount;
            }

            return invoiceFee;
        }

        /// <summary>
        /// Calculates the due date of the order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        private static DateTime CalculateDueDate(OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            DateTime dueDate = order.IsCompletedDateNull() ? DateTime.Now : order.CompletedDate;

            if (!order.OrderInvoiceCustomerRow.IsPaymentTermsNull())
            {
                var paymentTerms = order.OrderInvoiceCustomerRow.PaymentTerms;

                if (paymentTerms == "Net 30" || paymentTerms == "1% 10 Net 30")
                {
                    dueDate = dueDate.AddDays(30);
                }
                else if (paymentTerms == "Net 15")
                {
                    dueDate = dueDate.AddDays(15);
                }
            }

            return dueDate;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _taOrders?.Dispose();
            _dsOrderInvoice?.Dispose();
            _taCustomer?.Dispose();
            _taOrderFees?.Dispose();
            _taOrderInvoice?.Dispose();
            _taOrderShipmentTable?.Dispose();
            _taPartSummary?.Dispose();
            _taOrderProcessingDetail?.Dispose();
            _taDepartment?.Dispose();
        }

        #endregion
    }
}
