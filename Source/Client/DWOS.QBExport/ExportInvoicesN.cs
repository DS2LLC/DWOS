using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderInvoiceDataSetTableAdapters;
using DWOS.Data.Invoice;
using DWOS.Data.Order;
using NLog;
using nsoftware.InQB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DWOS.QBExport
{
    /// <summary>
    /// Exports invoices to QuickBooks.
    /// </summary>
    public class ExportInvoicesN: IInvoiceExporter
    {
        #region Fields

        public const string COLUMN_IMPORTED = "Imported";
        public const string COLUMN_EXPORTED = "Export";
        public const string COLUMN_ISSUES = "Issues";

        /// <summary>
        /// Gets a value that represents the maximum length of a PO in
        /// QuickBooks.
        /// </summary>
        private const int MAX_PO_LENGTH = 25;

        /// <summary>
        /// Gets a value that represents the maximum length of an invoice in
        /// QuickBooks.
        /// </summary>
        private const int MAX_INVOICE_LENGTH = 20;

        /// <summary>
        /// Format for QuickBooks amounts
        /// </summary>
        /// <remarks>
        /// QB can't handle more than 2 decimal places.
        /// </remarks>
        private const string QUICKBOOKS_STRING_FORMAT = "F2";

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, bool> _customerInQBCache = new Dictionary<string, bool>();
        private readonly Dictionary<string, bool> _itemInQBCache = new Dictionary<string, bool>();

        public event ProgressChangedEventHandler ProgessChanged;

        private Customer _customer;
        private Invoice _invoice;
        private Qblists _qbLists;
        private Item _qbItem;

        private string _descriptionTemplate;
        private Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter _taOrders;
        private OrderInvoiceTableAdapter _taOrderInvoice;
        private Lazy<bool> _lazyCompanyValidEmail;
        private readonly IProductClassPersistence _productClassPersistence;

        private readonly string _priceStringFormat =
            "F" + ApplicationSettings.Current.PriceDecimalPlaces;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dataset for this instance.
        /// </summary>
        public OrderInvoiceDataSet OrderInvoices { get; private set; }

        /// <summary>
        /// Gets or sets a list of exported orders.
        /// </summary>
        public List<int> ExportedOrderIDs { get; } =
            new List<int>();

        /// <summary>
        /// Gets or sets the number of errors encountered during export.
        /// </summary>
        public int ErrorCount
        {
            get;
            set;
        }

        public MidpointRounding RoundingType { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportInvoicesN"/> class.
        /// </summary>
        public ExportInvoicesN(MidpointRounding roundingType)
        {
            RoundingType = roundingType;
            _lazyCompanyValidEmail = new Lazy<bool>(CheckCompanyEmail);
            _productClassPersistence = new ProductClassPersistence();
        }

        /// <summary>
        /// Loads the datasets from the DWOS database for the export operations.
        /// </summary>
        public void LoadData()
        {
            _log.Info("Loading data.");
            SetupPersistence();
            SetupExport();
        }

        private void SetupPersistence()
        {
            this._taOrders = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter();
            this._taOrderInvoice = new OrderInvoiceTableAdapter();

            OrderInvoices = new OrderInvoiceDataSet
            {
                EnforceConstraints = false
            };

            using (new UsingDataSetLoad(OrderInvoices))
            {
                this._taOrderInvoice.Fill(this.OrderInvoices.OrderInvoice);

                using (var taCustomer = new OrderInvoiceCustomerTableAdapter())
                {
                    taCustomer.Fill(OrderInvoices.OrderInvoiceCustomer);
                }

                using (var taCustomerAddress = new OrderInvoiceCustomerAddressTableAdapter())
                {
                    taCustomerAddress.Fill(this.OrderInvoices.OrderInvoiceCustomerAddress);
                }

                using (var taPartSummary = new PartSummaryTableAdapter())
                {
                    taPartSummary.Fill(this.OrderInvoices.PartSummary);
                }

                using (var taOrderFees = new OrderFeesTableAdapter())
                {
                    taOrderFees.Fill(this.OrderInvoices.OrderFees);
                }

                using (var taOrderFeeType = new OrderFeeTypeTableAdapter())
                {
                    taOrderFeeType.Fill(this.OrderInvoices.OrderFeeType);
                }

                using (var taOrderShipment = new OrderShipmentTableAdapter())
                {
                    taOrderShipment.Fill(this.OrderInvoices.OrderShipment);
                }

                using (var taOrderProcessingDetail = new OrderProcessingDetailTableAdapter())
                {
                    taOrderProcessingDetail.Fill(this.OrderInvoices.OrderProcessingDetail);
                }

                using (var taDepartment = new d_DepartmentTableAdapter())
                {
                    taDepartment.Fill(this.OrderInvoices.d_Department);
                }

                // Load Description template
                using (var taTemplates = new Data.Datasets.ApplicationSettingsDataSetTableAdapters.TemplatesTableAdapter())
                {
                    using (var dtTemplates = taTemplates.GetDataById("QuickBooks_Description"))
                    {
                        var descriptionTemplateRow = dtTemplates.FirstOrDefault();
                        _descriptionTemplate = descriptionTemplateRow?.Template;
                    }
                }
            }

            //Add column if does not exist
            if (!this.OrderInvoices.OrderInvoice.Columns.Contains(COLUMN_EXPORTED))
                this.OrderInvoices.OrderInvoice.Columns.Add(COLUMN_EXPORTED, typeof(bool)).DefaultValue = false;

            if(!this.OrderInvoices.OrderInvoice.Columns.Contains(COLUMN_IMPORTED))
                this.OrderInvoices.OrderInvoice.Columns.Add(COLUMN_IMPORTED, typeof(bool)).DefaultValue = false;

            if(!this.OrderInvoices.OrderInvoice.Columns.Contains(COLUMN_ISSUES))
                this.OrderInvoices.OrderInvoice.Columns.Add(COLUMN_ISSUES, typeof(string));
        }

        #region Invoice Level

        /// <summary>
        /// Setup the export based on the invoicing level specified by customer preference
        /// </summary>
        private void SetupExport()
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
                                order["invoice"] = InvoiceHelpers.GetSOInvoice(order);

                            this.CheckSalesOrder(order);
                        }
                        break;
                    case InvoiceLevelType.Default:
                    case InvoiceLevelType.Package:
                    case InvoiceLevelType.WorkOrder:
                        {
                            //user may want to edit invoice name so load what it will be when exported and allow user to edit
                            order["Invoice"] = InvoiceHelpers.GetInvoiceWithPrefix(order);
                            this.CheckOrder(order);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Set order export checked value
        /// </summary>
        /// <param name="order">The order.</param>
        private void CheckOrder(OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            order[COLUMN_EXPORTED] = true;
        }

        /// <summary>
        /// Set order export checked value based on any open orders contained within the sales order
        /// </summary>
        /// <param name="order">The order.</param>
        private void CheckSalesOrder(OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            //Check if sales order has open orders (all have to be closed to auto check them) but customer can overide this in UI
            if (order.IsSalesOrderIDNull())
            {
                CheckOrder(order); //No sales order
            }
            else
            {
                order[COLUMN_EXPORTED] = !OpenOrdersInSalesOrder(order.SalesOrderID);
            }
        }

        /// <summary>
        /// Determines if the sales order has any open orders.
        /// </summary>
        /// <param name="salesOrderID">The sales order identifier.</param>
        /// <returns></returns>
        private bool OpenOrdersInSalesOrder(int salesOrderID)
        {
            OrdersDataSet.OrderDataTable dtOrder = new OrdersDataSet.OrderDataTable();

            this._taOrders.FillBySalesOrder(dtOrder, salesOrderID);
            return dtOrder.Any(row => row.Status.EquivalentTo("Open"));
        }

        #endregion


        /// <summary>
        /// Exports the invoices into quickbooks.
        /// </summary>
        /// <returns>Returns the results of the export.</returns>
        /// <exception cref="DWOS.QBExport.ExportConnectionException"></exception>
        public InvoiceResult Export()
        {
            try
            {
                string connectionErrMsg;

                if (!TestQBConnection(out connectionErrMsg))
                {
                    //const string errorMsg = "Unable to establish a connection with QuickBooks. " +
                    //                        "Make sure QuickBooks and DWOS are running as 'Administrator'.";

                    throw new ExportConnectionException(connectionErrMsg);
                }

                this.ProgessChanged?.Invoke(this, new ProgressChangedEventArgs(0, null));

                if (this.OrderInvoices == null)
                    this.LoadData();

                // Open QuickBooks connections
                this._invoice = new Invoice()
                {
                    QBConnectionString = ApplicationSettings.Current.QBConnectionString,
                    QBXMLVersion = Properties.Settings.Default.QBXMLVersion
                };
                this._invoice.OpenQBConnection();

                _customer = new Customer
                {
                    QBConnectionString = ApplicationSettings.Current.QBConnectionString,
                    QBXMLVersion = Properties.Settings.Default.QBXMLVersion
                };

                _customer.OpenQBConnection();

                _qbLists = new Qblists
                {
                    QBConnectionString = ApplicationSettings.Current.QBConnectionString,
                    QBXMLVersion = Properties.Settings.Default.QBXMLVersion
                };

                _qbLists.OpenQBConnection();

                _qbItem = new Item
                {
                    QBConnectionString = ApplicationSettings.Current.QBConnectionString,
                    QBXMLVersion = Properties.Settings.Default.QBXMLVersion
                };

                _qbItem.OpenQBConnection();

                // Start export
                int successfulExportCount;
                if (!CheckOrders())
                {
                    successfulExportCount = 0;

                    foreach (var order in OrderInvoices.OrderInvoice)
                    {
                        // Fail export for all orders; this includes them in
                        // the export report.
                        if (!order.IsNull(COLUMN_EXPORTED) && (bool)order[COLUMN_EXPORTED])
                        {
                            order[COLUMN_IMPORTED] = false;
                        }
                    }

                }
                else if (ApplicationSettings.Current.InvoiceLevel == InvoiceLevelType.SalesOrder)
                {
                    successfulExportCount = ExportSalesOrders();
                }
                else
                {
                    successfulExportCount = ExportWorkOrders();
                }

                return new InvoiceResult()
                {
                    ExportedCount = successfulExportCount,
                    TotalCount = OrderInvoices.OrderInvoice.Count,
                    ExportedOrderIDs = ExportedOrderIDs,
                    ErrorCount = ErrorCount,
                    Cancelled = false
                };
            }
            finally
            {
                this._customer?.CloseQBConnection();
                this._customer?.Dispose();

                this._invoice?.CloseQBConnection();
                this._invoice?.Dispose();

                _qbLists?.CloseQBConnection();
                _qbLists?.Dispose();

                _qbItem?.CloseQBConnection();
                _qbItem?.Dispose();

                this._invoice = null;
                this._customer = null;
                _qbLists = null;
                _qbItem = null;
            }
        }


        /// <summary>
        /// Tests the QuickBooks connection.
        /// </summary>
        /// <param name="connectionErrMsg">The connection error message, if any.</param>
        /// <returns>Success or Failure</returns>
        private bool TestQBConnection(out string connectionErrMsg)
        {
            Objsearch search = null;
            bool canConnect = false;
            connectionErrMsg = string.Empty;

            try
            {
                // Test connection by opening one from a temporary object
                search = new Objsearch
                {
                    QBConnectionString = ApplicationSettings.Current.QBConnectionString
                };

                search.OpenQBConnection();

                _log.Info("Successfully connected to QuickBooks.");

                canConnect = true;
            }
            catch (Exception exc)
            {
                connectionErrMsg =  exc.Message;
                _log.Info(exc, "Failed to establish a connection with QuickBooks." + Environment.NewLine + connectionErrMsg);
            }
            finally
            {
                if (search != null)
                {
                    if (canConnect)
                    {
                        search.CloseQBConnection();
                    }

                    search.Dispose();
                }
            }

            return canConnect;
        }

        /// <summary>
        /// Exports the sales orders.
        /// </summary>
        /// <returns></returns>
        private int ExportSalesOrders()
        {
            var successfulExportCount = 0;
            this.ExportedOrderIDs.Clear();

            var sortExpression = string.Format(
                "{0} {1}", OrderInvoices.OrderInvoice.SalesOrderIDColumn.ColumnName, "ASC");

            OrderInvoices.OrderInvoice.DefaultView.Sort = sortExpression;

            var soDict = new Dictionary<string, List<OrderInvoiceDataSet.OrderInvoiceRow>>();
            var woDict = new Dictionary<string, List<OrderInvoiceDataSet.OrderInvoiceRow>>();

            foreach (System.Data.DataRowView rowView in this.OrderInvoices.OrderInvoice.DefaultView)
            {
                if (!(rowView.Row is OrderInvoiceDataSet.OrderInvoiceRow order))
                {
                    continue;
                }

                order[COLUMN_IMPORTED] = DBNull.Value;
                order[COLUMN_ISSUES] = DBNull.Value;

                //Only export the ones that are checked in UI grid
                if (Convert.ToBoolean(order[COLUMN_EXPORTED]))
                {
                    var invoiceID = order.IsInvoiceNull() ? string.Empty : order.Invoice.ToString();

                    if (!order.IsSalesOrderIDNull())
                    {
                        string salesOrderInvoiceID;

                        if (string.IsNullOrEmpty(invoiceID))
                        {
                            salesOrderInvoiceID = order.SalesOrderID.ToString();
                        }
                        else
                        {
                            salesOrderInvoiceID = invoiceID;
                        }

                        var customerExists = this.DoesCustomerExist(order.CustomerName)
                                             || (!order.OrderInvoiceCustomerRow.IsAccountingIDNull()
                                                 && this.DoesCustomerExist(order.OrderInvoiceCustomerRow.AccountingID));

                        if (customerExists)
                        {
                            if (!soDict.ContainsKey(salesOrderInvoiceID))
                            {
                                soDict.Add(
                                    salesOrderInvoiceID, new List<OrderInvoiceDataSet.OrderInvoiceRow> { order });
                            }
                            else
                            {
                                var orders = soDict[salesOrderInvoiceID];
                                orders.Add(order);
                                soDict[salesOrderInvoiceID] = orders;
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(invoiceID))
                    {
                        // not part of a sales order but still checked, just export by itself
                        var customerExists = this.DoesCustomerExist(order.CustomerName)
                                             || (!order.OrderInvoiceCustomerRow.IsAccountingIDNull()
                                                 && this.DoesCustomerExist(order.OrderInvoiceCustomerRow.AccountingID));

                        if (customerExists)
                        {
                            if (!woDict.ContainsKey(invoiceID))
                            {
                                woDict.Add(
                                    invoiceID, new List<OrderInvoiceDataSet.OrderInvoiceRow> { order });
                            }
                            else
                            {
                                var orders = woDict[invoiceID];
                                orders.Add(order);
                                woDict[invoiceID] = orders;
                            }
                        }
                    }
                    else
                    {
                        // not part of a sales order but still checked, just export by itself
                        successfulExportCount += this.ExportWorkOrder(order);
                    }
                }
            }

            _log.Info("Exporting Sales Orders: Preparing {0} invoices for export.".FormatWith(soDict.Count));

            foreach (var kvp in woDict)
            {
                var invoiceNumber = kvp.Key;
                var orders = kvp.Value;

                if (orders.Count > 1)
                {
                    successfulExportCount += ExportGroupedOrder(invoiceNumber, orders);
                }
                else
                {
                    successfulExportCount += ExportWorkOrder(orders[0]);
                }
            }

            // Process the sales orders
            foreach (var kvp in soDict)
            {
                var invoiceNumber = kvp.Key;
                var orders = kvp.Value;

                successfulExportCount += ExportGroupedOrder(invoiceNumber, orders);
            }

            return successfulExportCount;
        }

        /// <summary>
        /// Exports the grouped order.
        /// </summary>
        /// <param name="invoiceNumber">The invoice identifier.</param>
        /// <param name="orders">The orders.</param>
        /// <returns>The successful export count.</returns>
        private int ExportGroupedOrder(string invoiceNumber, List<OrderInvoiceDataSet.OrderInvoiceRow> orders)
        {
            int successfulExportCount = 0;

            _log.Info("Begin importing sales order Invoice: " + invoiceNumber);

            this._invoice.Reset();

            this._invoice.RefNumber = invoiceNumber;

            foreach (var order in orders)
            {
                _log.Info("Exporting order {0} to Quickbooks.".FormatWith(order.OrderID));

                string orderNotes = null;

                this._invoice.TransactionDate = order.IsCompletedDateNull() ? DateTime.Now.ToShortDateString() : order.CompletedDate.ToShortDateString();

                //Address
                var customer = order.OrderInvoiceCustomerRow;

                this._invoice.CustomerName = customer.IsAccountingIDNull() || string.IsNullOrWhiteSpace(customer.AccountingID) ? customer.Name : customer.AccountingID;

                this._invoice.BillingAddress = BillingAddress(customer).Aggregate;
                this._invoice.ShippingAddress = ShippingAddress(order).Aggregate;

                // Assumption - PurchaseOrder was previously verified to be 25
                // or less characters in length.
                this._invoice.PONumber = order.IsPurchaseOrderNull() ? string.Empty : order.PurchaseOrder;

                this._invoice.TermsName = order.OrderInvoiceCustomerRow.IsPaymentTermsNull() ? string.Empty : order.OrderInvoiceCustomerRow.PaymentTerms;
                this._invoice.ShipDate = order.IsCompletedDateNull() ? DateTime.Now.ToShortDateString() : order.CompletedDate.ToShortDateString();
                this._invoice.DueDate = CalculateDueDate(order).ToShortDateString();

                //load the customer
                this._customer.GetByName(this._invoice.CustomerName);

                //set invoice preferences
                SetInvoicePreferences(order);

                this._invoice.IsPending = false;

                var qbOrderTotal = AddProcessingLineItems(order);
                AddFeeLineItems(order, qbOrderTotal, ref orderNotes);

                if (string.IsNullOrEmpty(orderNotes))
                {
                    this._taOrderInvoice.AddInvoice(this._invoice.RefNumber, order.OrderID);
                    this.ExportedOrderIDs.Add(order.OrderID);
                    order[COLUMN_IMPORTED] = true;
                    successfulExportCount++;
                }
                else
                {
                    order[COLUMN_ISSUES] = orderNotes;
                }
            }

            // Add invoice to sales order
            var isGroupForSalesOrder = orders.All(row => row.IsNull(COLUMN_ISSUES) && !row.IsSalesOrderIDNull()) &&
                                       orders.Select(row => row.SalesOrderID).Distinct().Count() == 1;

            if (isGroupForSalesOrder)
            {
                this._taOrderInvoice.AddSalesOrderInvoice(this._invoice.RefNumber, orders[0].SalesOrderID);
            }

            //Add invoice to QB
            this.AddInvoiceToQB(_invoice);

            //Set Custom Fields on the invoice
            SetInvoiceCustomerWO(_invoice, orders[0]);
            SetInvoiceTrackingNumber(_invoice, orders[0]);
            SetInvoiceClassName(this._invoice, orders[0]);

            return successfulExportCount;
        }

        /// <summary>
        /// Adds the invoice to qb.
        /// </summary>
        /// <param name="invoice">The invoice.</param>
        private void AddInvoiceToQB(Invoice invoice)
        {
            try
            {
                invoice.Add();
            }
            catch (Exception exc)
            {
                _log.Info("Error exporting order Invoice: " + invoice.RefNumber + ". " + exc.Message);
            }
        }

        /// <summary>
        /// Exports the work orders.
        /// </summary>
        /// <returns></returns>
        private int ExportWorkOrders()
        {
            var successfulExportCount = 0;
            this.ExportedOrderIDs.Clear();

            var sortExpression = string.Format(
                "{0} {1}", OrderInvoices.OrderInvoice.InvoiceColumn.ColumnName, "ASC");

            OrderInvoices.OrderInvoice.DefaultView.Sort = sortExpression;

            var woDict = new Dictionary<string, List<OrderInvoiceDataSet.OrderInvoiceRow>>();

            foreach (System.Data.DataRowView rowView in this.OrderInvoices.OrderInvoice.DefaultView)
            {
                if (!(rowView.Row is OrderInvoiceDataSet.OrderInvoiceRow order))
                {
                    continue;
                }

                order[COLUMN_IMPORTED] = DBNull.Value;
                order[COLUMN_ISSUES] = DBNull.Value;

                //Only export the ones that are checked in UI grid
                if (Convert.ToBoolean(order[COLUMN_EXPORTED]))
                {
                    var workOrderID = order.IsInvoiceNull() ? string.Empty : order.Invoice.ToString();

                    if (!string.IsNullOrEmpty(workOrderID))
                    {
                        if (this.DoesCustomerExist(order.CustomerName)
                            || (!order.OrderInvoiceCustomerRow.IsAccountingIDNull()
                                && this.DoesCustomerExist(order.OrderInvoiceCustomerRow.AccountingID)))
                        {
                            if (!woDict.ContainsKey(workOrderID))
                            {
                                woDict.Add(
                                    workOrderID, new List<OrderInvoiceDataSet.OrderInvoiceRow> { order });
                            }
                            else
                            {
                                var orders = woDict[workOrderID];
                                orders.Add(order);
                                woDict[workOrderID] = orders;
                            }
                        }
                    }
                    else
                    {
                        // not part of a sales order but still checked, just export by itself
                        successfulExportCount += ExportWorkOrder(order);
                    }
                }
            }

            _log.Info("Exporting Sales Orders: Preparing {0} invoices for export.".FormatWith(woDict.Count));

            // Export work orders
            foreach (var kvp in woDict)
            {
                var invoiceNumber = kvp.Key;
                var orders = kvp.Value;

                if (orders.Count > 1)
                {
                    successfulExportCount += ExportGroupedOrder(invoiceNumber, orders);
                }
                else
                {
                    successfulExportCount += ExportWorkOrder(orders[0]);
                }
            }

            return successfulExportCount;
        }

        /// <summary>
        /// Exports the work order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>The successful export count.</returns>
        private int ExportWorkOrder(OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            int successfulExportCount = 0;

            var customerInQB = this.DoesCustomerExist(order.CustomerName) ||
                (!order.OrderInvoiceCustomerRow.IsAccountingIDNull() && this.DoesCustomerExist(order.OrderInvoiceCustomerRow.AccountingID));

            if (customerInQB)
            {
                try
                {
                    _log.Info("Exporting Invoice for order {0}.".FormatWith(order.OrderID));

                    string invoiceID = this.ExportOrder(order, out string notes);

                    //User may have changed invoice name in grid
                    if (invoiceID != order.Invoice)
                        invoiceID = order.Invoice;

                    order[COLUMN_ISSUES] = notes;

                    if (string.IsNullOrEmpty(notes))
                    {
                        this._taOrderInvoice.AddInvoice(invoiceID, order.OrderID);
                        this.ExportedOrderIDs.Add(order.OrderID);
                        order[COLUMN_IMPORTED] = true;
                        successfulExportCount++;

                        _log.Info("Exporting Invoice for order {0} was successful.".FormatWith(order.OrderID));
                    }
                    else
                    {
                        ErrorCount++;
                        order[COLUMN_IMPORTED] = false;

                        // write the issues out to the log so we can figure out what went wrong.
                        _log.Info("QuickBooks Issue: " + notes);
                    }
                }
                catch (Exception exc)
                {
                    ErrorCount++;
                    order[COLUMN_IMPORTED] = false;
                    order[COLUMN_ISSUES] = "Error exporting invoice: " + exc.Message;

                    _log.Error(exc, "Error exporting invoice. Make sure QuickBooks is open and running in 'Administrator' mode.");
                }
            }
            else
            {
                ErrorCount++;
                order[COLUMN_IMPORTED] = false;
                order[COLUMN_ISSUES] = "Unable to locate Quickbooks Customer: " + order.CustomerName;
            }

            return successfulExportCount;
        }

        /// <summary>
        /// Exports the order and return the invoice id that was created.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="notes">The notes.</param>
        /// <returns></returns>
        private string ExportOrder(OrderInvoiceDataSet.OrderInvoiceRow order, out string notes)
        {
            notes = null;
            _log.Debug("Begin importing Invoice: " + order.OrderID);

            this._invoice.Reset();

            // Use this because the user could have changed the value in the grid ApplicationSettings.Current.GetInvoiceWithPrefix(order.OrderID);
            this._invoice.RefNumber = order.IsNull("Invoice") || order["Invoice"] == DBNull.Value ? string.Empty : order["Invoice"].ToString();
            this._invoice.TransactionDate = order.IsCompletedDateNull() ? DateTime.Now.ToShortDateString() : order.CompletedDate.ToShortDateString();

            //Address
            var customer = order.OrderInvoiceCustomerRow;

            this._invoice.CustomerName = customer.IsAccountingIDNull() || string.IsNullOrWhiteSpace(customer.AccountingID) ? customer.Name : customer.AccountingID;

            this._invoice.BillingAddress = BillingAddress(customer).Aggregate;
            this._invoice.ShippingAddress = ShippingAddress(order).Aggregate;
            this._invoice.PONumber = order.IsPurchaseOrderNull() ? string.Empty : order.PurchaseOrder;
            this._invoice.TermsName = order.OrderInvoiceCustomerRow.IsPaymentTermsNull() ? string.Empty : order.OrderInvoiceCustomerRow.PaymentTerms;
            this._invoice.ShipDate = order.IsCompletedDateNull() ? DateTime.Now.ToShortDateString() : order.CompletedDate.ToShortDateString();
            this._invoice.DueDate = CalculateDueDate(order).ToShortDateString();

            //load the customer
            var findCustomerResult = this.GetCustomerFromQB(this._invoice.CustomerName);

            if (!string.IsNullOrEmpty(findCustomerResult))
            {
                notes += findCustomerResult;
                return this._invoice.RefNumber;
            }

            //set invoice preferences
            SetInvoicePreferences(order);
            this._invoice.IsPending = false;

            var qbOrderTotal = AddProcessingLineItems(order);
            AddFeeLineItems(order, qbOrderTotal, ref notes);

            //Add invoice to QB
            this.AddInvoiceToQB(_invoice);

            //Set Custom Fields on the invoice
            SetInvoiceCustomerWO(this._invoice, order);
            SetInvoiceTrackingNumber(this._invoice, order);
            SetInvoiceClassName(this._invoice, order);

            return this._invoice.RefNumber;
        }

        private decimal AddProcessingLineItems(OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            if (order == null)
            {
                return 0M;
            }

            var adjustTotal = order.GetOrderFeesRows().Length == 0;

            var orderBasePrice = order.IsBasePriceNull() ? 0 : order.BasePrice;
            var orderTotal = OrderPrice.CalculatePrice(orderBasePrice,
                order.IsPriceUnitNull() ? string.Empty : order.PriceUnit,
                0M,
                order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                order.IsWeightNull() ? 0M : order.Weight);

            var processingDetails = order.GetOrderProcessingDetailRows()
                .Where(row => row.IsValidState())
                .ToList();

            bool useDeptLevelInvoicing = ApplicationSettings.Current.InvoiceLineItemType == InvoiceLineItemType.Department &&
                                         ApplicationSettings.Current.PartPricingType == PricingType.Process &&
                                         processingDetails.Count > 0;
                                         ////&&
                                 //        processingDetails.Any(row => !row.IsAmountNull() && row.Amount != 0M);

            if (useDeptLevelInvoicing)
            {
                var processBasePrice = processingDetails.Sum(row => row.IsAmountNull() ? 0M : row.Amount);

                var processingFactor = 0M;

                if (processBasePrice != 0)
                {
                    processingFactor = orderBasePrice / processBasePrice;
                }

                var detailsGrouped = processingDetails
                    .GroupBy(r => r.Department)
                    .Where(group => group.Sum(row => row.IsAmountNull() ? 0M : row.Amount) > 0M)
                    .ToList();

                var qbTotal = 0M;
                for (var groupIndex = 0; groupIndex < detailsGrouped.Count; ++groupIndex)
                {
                    var detailGroup = detailsGrouped[groupIndex];
                    var isLastGroup = groupIndex == detailsGrouped.Count - 1;
                    var departmentName = detailGroup.Key;
                    var invoiceLineAdd = new InvoiceItem();
                    var department = OrderInvoices.d_Department.FindByDepartmentID(departmentName);

                    // Assumption - department has been synced w/ QB during validation
                    var departmentAccountingCode = department.IsAccountingCodeNull() ?
                        null :
                        department.AccountingCode;

                    invoiceLineAdd.ItemName = departmentAccountingCode;
                    invoiceLineAdd.Description = GetDescription(order, InvoiceLineItemType.Department, departmentName);
                    invoiceLineAdd.Quantity = order.IsPartQuantityNull() ? "0" : order.PartQuantity.ToString();

                    if (ApplicationSettings.Current.InvoiceItem1 != InvoiceItemType.None)
                    {
                        var value = this.GetOtherValue(order, ApplicationSettings.Current.InvoiceItem1);
                        invoiceLineAdd.Other1 = value;
                    }

                    if (ApplicationSettings.Current.InvoiceItem2 != InvoiceItemType.None)
                    {
                        var value = this.GetOtherValue(order, ApplicationSettings.Current.InvoiceItem2);
                        invoiceLineAdd.Other2 = value;
                    }

                    var departmentAmount = detailGroup.Sum(row => row.IsAmountNull() ? 0M : row.Amount);
                    var lineItemBasePrice = departmentAmount * processingFactor;
                    var lineItemAmount = OrderPrice.CalculatePrice(lineItemBasePrice,
                        order.IsPriceUnitNull() ? string.Empty : order.PriceUnit,
                        0M,
                        order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                        order.IsWeightNull() ? 0M : order.Weight);

                    // QB can't handle more than 2 decimal places so we need to round the amount and then stick the actual amount in the 'Memo' line
                    if (ApplicationSettings.Current.PriceDecimalPlaces > 2)
                    {
                        var roundedLineItemAmount = Math.Round(lineItemAmount, 2, RoundingType);
                        this._invoice.Memo += order.OrderID +
                                              " " +
                                              departmentName +
                                              " Actual Amount: $" +
                                              lineItemAmount.ToString(_priceStringFormat);

                        qbTotal += roundedLineItemAmount;

                        if (isLastGroup && adjustTotal)
                        {
                            // Add difference so that QB price matches rounded DWOS price.
                            var qbShouldBe = Math.Round(orderTotal, 2, RoundingType);
                            roundedLineItemAmount += qbShouldBe - qbTotal;
                        }

                        invoiceLineAdd.Amount = roundedLineItemAmount.ToString(QUICKBOOKS_STRING_FORMAT);
                    }
                    else
                    {
                        qbTotal += lineItemAmount;

                        if (isLastGroup && adjustTotal)
                        {
                            // Add difference so that QB price matches DWOS price.
                            var qbShouldBe = orderTotal;
                            lineItemAmount += qbShouldBe - qbTotal;
                        }

                        invoiceLineAdd.Amount = lineItemAmount.ToString(_priceStringFormat);
                    }

                    this._invoice.LineItems.Add(invoiceLineAdd);

                }

                return qbTotal;
            }
            else
            {
                //Add 'Part Processing' Invoice Line Item
                // Assumption - invoice part has been synced w/ QB during validation
                var invoiceLineAdd = new InvoiceItem
                {
                    ItemName = GetItemName(order),
                    Description = GetDescription(order, InvoiceLineItemType.Part,""),
                    Rate = order.IsBasePriceNull() ? "0.00" : order.BasePrice.ToString(_priceStringFormat),
                    Quantity = order.IsPartQuantityNull() ? "0" : order.PartQuantity.ToString()
                };

                if (ApplicationSettings.Current.InvoiceItem1 != InvoiceItemType.None)
                {
                    var value = this.GetOtherValue(order, ApplicationSettings.Current.InvoiceItem1);
                    invoiceLineAdd.Other1 = value;
                }

                if (ApplicationSettings.Current.InvoiceItem2 != InvoiceItemType.None)
                {
                    var value = this.GetOtherValue(order, ApplicationSettings.Current.InvoiceItem2);
                    invoiceLineAdd.Other2 = value;
                }

                // QB can't handle more than 2 decimal places so we need to round the amount and then stick the actual amount in the 'Memo' line
                if (ApplicationSettings.Current.PriceDecimalPlaces > 2)
                {
                    invoiceLineAdd.Amount = Math.Round(orderTotal, 2, RoundingType).ToString(QUICKBOOKS_STRING_FORMAT);
                    this._invoice.Memo += order.OrderID + "Actual Amount: $" + orderTotal.ToString(_priceStringFormat);
                }
                else
                {
                    invoiceLineAdd.Amount = orderTotal.ToString(_priceStringFormat);
                }

                // There's no need to adjust totals because there is only one processing line item

                this._invoice.LineItems.Add(invoiceLineAdd);
                return ApplicationSettings.Current.PriceDecimalPlaces > 2
                    ? Math.Round(orderTotal, 2, RoundingType)
                    : orderTotal;
            }
        }

        private string GetItemName(OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            if (ApplicationSettings.Current.InvoiceLineItemType != InvoiceLineItemType.ProductClass)
            {
                return ApplicationSettings.Current.InvoicePartItemName;
            }

            var productClass = _productClassPersistence.RetrieveForOrder(order.OrderID);

            if (productClass == null)
            {
                return ApplicationSettings.Current.InvoicePartItemName;
            }

            if (!string.IsNullOrEmpty(productClass.AccountingCode))
            {
                return productClass.AccountingCode;
            }

            return ApplicationSettings.Current.InvoicePartItemName;
        }

        /// <summary>
        /// Gets the description text for the order using a template.
        /// </summary>
        /// <remarks>
        /// QuickBooks does not seem to have a character limit for invoice
        /// line item descriptions.
        /// </remarks>
        /// <param name="order">
        /// The order to generate a description for.
        /// </param>
        /// <returns>
        /// Descriptive text for the order.
        /// </returns>
        private string GetDescription(OrderInvoiceDataSet.OrderInvoiceRow order, InvoiceLineItemType type, string filter)
        {
            if (string.IsNullOrWhiteSpace(_descriptionTemplate))
            {
                return order.PartSummaryRow.Name;
            }

            var processesString = string.Empty;

            
            if (_descriptionTemplate.Contains("%PROCESSES%"))
            {
                IEnumerable<String> aliasNames = Enumerable.Empty<string>();
                if (type == InvoiceLineItemType.Department)
                {
                    try
                    {

                        using (var taOrderInvoice = new OrderInvoiceDetailsTableAdapter())
                        {
                            var dtProcesses = taOrderInvoice.GetOrderInvoiceData(order.OrderID, filter);
                            List<string> aliases = ((OrderInvoiceDataSet.OrderInvoiceDetailsDataTable)dtProcesses).AsEnumerable().Select(x => x["ProcessAliasName"].ToString()).ToList();

                            aliasNames = aliases;
                        }
                    }
                    catch (Exception)
                    {
                        //Do Nothing
                    }
                }
                else if(type == InvoiceLineItemType.ProductClass)
                {
                    //aliasNames = order.GetOrderInvoiceProductClassRows()
                }
                else
                {
                    aliasNames = order.GetOrderProcessingDetailRows()
                                     .Where(op => op.COCData)
                                     .OrderBy(op => op.StepOrder)
                                     .Select(op => op.ProcessAliasName);
                }


                processesString = string.Join(", ", aliasNames);
            }
            
            string pDesc = "";
            try { pDesc = order.PartSummaryRow.Description; }
            catch { }


            return _descriptionTemplate
                .Replace("%PART%", order.PartSummaryRow.Name )
                .Replace("%DESCRIPTION%", " " + pDesc)
                .Replace("%PROCESSES%", " " + processesString)
                .Replace("%WO%", " " + order.OrderID.ToString());
        }

        private void AddFeeLineItems(OrderInvoiceDataSet.OrderInvoiceRow order, decimal qbOrderTotal, ref string notes)
        {
            var orderTotal = OrderPrice.CalculatePrice(order.IsBasePriceNull() ? 0 : order.BasePrice,
                order.IsPriceUnitNull() ? string.Empty : order.PriceUnit,
                OrderPrice.CalculateFees(order, order.IsBasePriceNull() ? 0 : order.BasePrice),
                order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                order.IsWeightNull() ? 0M : order.Weight);

            var qbTotal = qbOrderTotal;

            var orderFeesRows = order.GetOrderFeesRows();

            for (var orderFeeIndex = 0; orderFeeIndex < orderFeesRows.Length; ++orderFeeIndex)
            {
                var fee = orderFeesRows[orderFeeIndex];
                var isLastFee = orderFeeIndex == orderFeesRows.Length - 1;
                var invoiceFee = new InvoiceItem();

                if(fee.OrderFeeTypeRow == null)
                {
                    notes += "Fee info not available for order fee " + fee.OrderFeeID;
                    continue;
                }
                else if(string.IsNullOrEmpty(fee.OrderFeeTypeRow.InvoiceItemName))
                {
                    notes += "An item name was not available for fee " + fee.OrderFeeTypeID;
                    continue;
                }

                invoiceFee.ItemName = fee.OrderFeeTypeRow.InvoiceItemName;

                //if RUSH then set to order priority versus just rush
                string feeType = fee.OrderFeeTypeID;
                if(feeType == "Rush" && !order.IsPriorityNull())
                    feeType = order.Priority;

                invoiceFee.Description = feeType;
                invoiceFee.Rate = fee.Charge.ToString(_priceStringFormat);
                invoiceFee.Quantity = "1";

                var feeAmount = OrderPrice.CalculateFees(
                    fee.OrderFeeTypeRow.FeeType,
                    fee.Charge,
                    order.IsBasePriceNull() ? 0M : order.BasePrice,
                    order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                    order.IsPriceUnitNull() ? OrderPrice.enumPriceUnit.Each.ToString() : order.PriceUnit,
                    order.IsWeightNull() ? 0M : order.Weight);

                // QB can't handle more than 2 decimal places so we need to round the fee amount and then stick the actual amount in the 'Memo' line
                if(ApplicationSettings.Current.PriceDecimalPlaces > 2)
                {
                    var roundedFeeAmount = Math.Round(feeAmount, 2, RoundingType);
                    this._invoice.Memo += " " + feeType + ": $" + feeAmount.ToString(_priceStringFormat);

                    qbTotal += roundedFeeAmount;

                    if (isLastFee)
                    {
                        // Add difference so that QB price matches rounded DWOS price.
                        var qbShouldBe = Math.Round(orderTotal, 2, RoundingType);
                        roundedFeeAmount += qbShouldBe - qbTotal;
                    }

                    invoiceFee.Amount = roundedFeeAmount.ToString(QUICKBOOKS_STRING_FORMAT);
                }
                else
                {
                    qbTotal += feeAmount;
                    if (isLastFee)
                    {
                        // Add difference so that QB prices matches DWOS price.
                        var qbShouldBe = orderTotal;
                        feeAmount += qbShouldBe - qbTotal;
                    }
                    invoiceFee.Amount = feeAmount.ToString(_priceStringFormat);
                }

                this._invoice.LineItems.Add(invoiceFee);
            }
        }

        private bool CheckOrders()
        {
            Data.Datasets.OrdersDataSetTableAdapters.OrderFeeTypeTableAdapter taFeeType = null;

            try
            {
                taFeeType = new Data.Datasets.OrdersDataSetTableAdapters.OrderFeeTypeTableAdapter();
                var departments = new HashSet<string>();
                var productClasses = new HashSet<ProductClassItem>();
                var terms = new HashSet<string>();

                bool isValid = true;
                bool checkGeneralPart = false;

                var settingsLineItemType = ApplicationSettings.Current.InvoiceLineItemType;
                InvoiceLineItemType actualInvoiceLineItemType;

                if (ApplicationSettings.Current.PartPricingType == PricingType.Part)
                {
                    // Do not use department-based pricing
                    actualInvoiceLineItemType = settingsLineItemType == InvoiceLineItemType.Department
                        ? InvoiceLineItemType.Part
                        : settingsLineItemType;
                }
                else
                {
                    actualInvoiceLineItemType = settingsLineItemType;
                }

                foreach (System.Data.DataRowView rowView in OrderInvoices.OrderInvoice.DefaultView)
                {
                    if (!(rowView?.Row is OrderInvoiceDataSet.OrderInvoiceRow order))
                    {
                        continue;
                    }

                    if (!Convert.ToBoolean(order[COLUMN_EXPORTED]))
                    {
                        continue;
                    }

                    if (actualInvoiceLineItemType == InvoiceLineItemType.Department)
                    {
                        // Build list of departments to check later.
                        var processingDetails = order.GetOrderProcessingDetailRows()
                            .Where(row => row.IsValidState())
                            .ToList();

                        bool useProcessLevelInvoicing = processingDetails.Count > 0 &&
                            processingDetails.Any(row => !row.IsAmountNull() && row.Amount != 0M);

                        if (useProcessLevelInvoicing)
                        {
                            foreach (var departmentName in processingDetails.Select(detail => detail.Department).Distinct())
                            {
                                departments.Add(departmentName);
                            }
                        }
                        else
                        {
                            checkGeneralPart = true;
                        }
                    }
                    else if (actualInvoiceLineItemType == InvoiceLineItemType.ProductClass)
                    {
                        // Build list of product classes to check later
                        var productClass = _productClassPersistence.RetrieveForOrder(order.OrderID);

                        if (productClass != null)
                        {
                            productClasses.Add(productClass);
                        }
                        else
                        {
                            checkGeneralPart = true;
                        }
                    }
                    else
                    {
                        checkGeneralPart = true;
                    }

                    // Build list of terms to check later
                    var hasTerms = !order.OrderInvoiceCustomerRow.IsPaymentTermsNull() &&
                        !string.IsNullOrEmpty(order.OrderInvoiceCustomerRow.PaymentTerms);

                    if (hasTerms)
                    {
                        terms.Add(order.OrderInvoiceCustomerRow.PaymentTerms);
                    }

                    // Validate order fee
                    foreach (var fee in order.GetOrderFeesRows())
                    {
                        string itemAccountName;
                        itemAccountName = taFeeType.GetInvoiceItemName(fee.OrderFeeTypeID);
                        if (string.IsNullOrEmpty(itemAccountName))
                        {
                            continue; // will fail each order with the fee
                        }

                        if (!this.DoesItemExistInQB(itemAccountName))
                        {
                            string chargeType = taFeeType.GetFeeType(fee.OrderFeeTypeID);
                            if (!AccountingFieldValidation.ValidateListItemFee(itemAccountName, fee.Charge, chargeType))
                            {
                                isValid = false;
                                // User canceled export.
                                break;
                            }
                        }
                    }

                    if (!order.IsPurchaseOrderNull() && order.PurchaseOrder.Length > MAX_PO_LENGTH)
                    {
                        isValid = false;
                        order[COLUMN_ISSUES] = "Order's PO is longer than QuickBooks's maximum length of 25 characters.";
                        break;
                    }

                    if (!order.IsInvoiceNull() && order.Invoice.Length > MAX_INVOICE_LENGTH)
                    {
                        isValid = false;
                        order[COLUMN_ISSUES] = "Order's Invoice is longer than QuickBooks's maximum length of 20 characters.";
                        break;
                    }
                }

                if (isValid)
                {
                    foreach (var department in departments)
                    {
                        if (!VerifyDepartmentAccountingCode(department))
                        {
                            isValid = false;
                            // User canceled export.
                            break;
                        }
                    }
                }

                if (isValid)
                {
                    foreach (var productClass in productClasses)
                    {
                        if (!VerifyProductClassAccountingCode(productClass))
                        {
                            isValid = false;
                            // User canceled export.
                            break;
                        }
                    }
                }

                if (isValid)
                {
                    isValid = terms.All(VerifyTerm);
                    // If this fails, then the user canceled the export.
                }

                if (isValid && checkGeneralPart)
                {
                    isValid = VerifyGeneralPart();
                    // If this fails, then the user canceled the export.
                }

                return isValid;
            }
            finally
            {
                taFeeType?.Dispose();
            }
        }

        private bool VerifyGeneralPart()
        {
            var invoiceItem = ApplicationSettings.Current.InvoicePartItemName;

            return this.DoesItemExistInQB(invoiceItem) ||
                   AccountingFieldValidation.ValidateListItem(invoiceItem);
        }

        private bool VerifyDepartmentAccountingCode(string departmentName)
        {
            var department = OrderInvoices.d_Department.FindByDepartmentID(departmentName);

            if (department == null)
            {
                return false;
            }

            var isSuccessful = true;
            var existingAccountingCode = department.IsAccountingCodeNull() ?
                null :
                department.AccountingCode;

            if (string.IsNullOrEmpty(existingAccountingCode) || !this.DoesItemExistInQB(existingAccountingCode))
            {
                isSuccessful = AccountingFieldValidation.ValidateListItemDepartment(
                    departmentName,
                    existingAccountingCode,
                    out string newAccountingCode);

                if (!string.IsNullOrEmpty(newAccountingCode) && existingAccountingCode != newAccountingCode)
                {
                    department.AccountingCode = newAccountingCode;

                    using (var taDepartment = new d_DepartmentTableAdapter())
                    {
                        taDepartment.Update(OrderInvoices.d_Department);
                    }

                    _log.Info("{0} accounting code is now {1}", departmentName, newAccountingCode);
                }
            }

            return isSuccessful;
        }

        private bool VerifyProductClassAccountingCode(ProductClassItem productClass)
        {
            if (productClass == null)
            {
                return false;
            }

            var existingAccountingCode = productClass.AccountingCode;

            var isSuccessful = true;

            if (string.IsNullOrEmpty(existingAccountingCode) || !DoesItemExistInQB(existingAccountingCode))
            {
                isSuccessful = AccountingFieldValidation.ValidateListItemProductClass(
                    productClass.Name,
                    existingAccountingCode,
                    out var newAccountingCode);

                if (!string.IsNullOrEmpty(newAccountingCode) && existingAccountingCode != newAccountingCode)
                {
                    productClass.AccountingCode = newAccountingCode;
                    _productClassPersistence.Update(productClass);
                    _log.Info($"{productClass.Name} accounting code is now {newAccountingCode}");
                }
            }

            return isSuccessful;
        }

        private bool VerifyTerm(string termName)
        {
            if (string.IsNullOrEmpty(termName))
            {
                return false;
            }

            // Check standard terms
            _qbLists.Reset();
            _qbLists.ListType = QblistsListTypes.ltStandardTerms;

            try
            {
                _qbLists.GetByName(termName);
                return true;
            }
            catch (InQBQblistsException exc)
            {
                // Could not find standard term
                _log.Info(exc, "Standard terms does not exist in QuickBooks {0}", termName);
            }

            // Check date driven terms
            _qbLists.Reset();
            _qbLists.ListType = QblistsListTypes.ltDateDrivenTerms;

            try
            {
                _qbLists.GetByName(termName);
                return true;
            }
            catch (InQBQblistsException exc)
            {
                _log.Warn(exc, "Terms does not exist in QuickBooks {0}", termName);
            }

            return AccountingFieldValidation.ValidateTerms(termName);
        }

        private static Address BillingAddress(OrderInvoiceDataSet.OrderInvoiceCustomerRow customer)
        {
            if (customer == null)
            {
                return null;
            }

            return new Address()
            {
                Line1 = customer.Name,
                Line2 = customer.IsAddress1Null() ? string.Empty : customer.Address1,
                City = customer.IsCityNull() ? string.Empty : customer.City,
                State = customer.IsStateNull() ? string.Empty : customer.State,
                PostalCode = customer.IsZipNull() ? string.Empty : customer.Zip
            };
        }

        private static Address ShippingAddress(OrderInvoiceDataSet.OrderInvoiceRow invoice)
        {
            if (invoice == null || invoice.OrderInvoiceCustomerRow == null)
            {
                return null;
            }

            var shippingAddress = invoice.GetOrderInvoiceCustomerAddressRows().FirstOrDefault();

            if (shippingAddress == null)
            {
                return BillingAddress(invoice.OrderInvoiceCustomerRow);
            }
            else
            {
                return new Address()
                {
                    Line1 = shippingAddress.Name,
                    Line2 = shippingAddress.IsAddress1Null() ? string.Empty : shippingAddress.Address1.TrimToMaxLength(40),
                    Line3 = shippingAddress.IsAddress2Null() ? string.Empty : shippingAddress.Address2,
                    City = shippingAddress.IsCityNull() ? string.Empty : shippingAddress.City,
                    State = shippingAddress.IsStateNull() ? string.Empty : shippingAddress.State,
                    PostalCode = shippingAddress.IsZipNull() ? string.Empty : shippingAddress.Zip
                };
            }
        }

        /// <summary>
        /// Gets the other value.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private string GetOtherValue(OrderInvoiceDataSet.OrderInvoiceRow order, InvoiceItemType item)
        {
            string value = string.Empty;

            switch (item)
            {
                case InvoiceItemType.CustomerWO:
                    value = order.IsCustomerWONull() ? "NA" : order.CustomerWO;
                    break;
                case InvoiceItemType.TrackingNumber:
                    OrderInvoiceDataSet.OrderShipmentRow[] shipments = order.GetOrderShipmentRows();

                    var trackingNumber = "-None-";

                    if (shipments.Length > 0 && !shipments[0].IsTrackingNumberNull())
                    {
                        trackingNumber = shipments[0].TrackingNumber.Split(',').First().TrimToMaxLength(30);
                    }

                    value = trackingNumber;
                    break;
                case InvoiceItemType.WO:
                    value = order.OrderID.ToString();
                    break;
                case InvoiceItemType.Weight:
                    //Gross Weight

                    decimal weight = 0M;

                    if (order.IsWeightNull())
                    {
                        using (var ta = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
                        {
                            var parts = ta.GetByOrder(order.OrderID);
                            var part = parts.FindByPartID(order.PartID);

                            if (part != null && !part.IsWeightNull())
                                weight = part.Weight * Convert.ToDecimal(order.PartQuantity);
                        }
                    }
                    else
                    {
                        weight = order.Weight;
                    }

                    value = weight.ToString();
                    break;
                case InvoiceItemType.PO:
                    value = order.IsPurchaseOrderNull() ? "NA" : order.PurchaseOrder;
                    break;
                case InvoiceItemType.PackingSlip:
                    var packingSlipShipments = order.GetOrderShipmentRows();
                    if (packingSlipShipments.Length > 0)
                    {
                        value = packingSlipShipments[0].ShipmentID.ToString();
                    }
                    else
                    {
                        value = "-None-";
                    }

                    break;
                default:
                    break;
            }

            return value;
        }

        /// <summary>
        /// Sets the invoice customer WO.
        /// </summary>
        /// <param name="invoice">The invoice.</param>
        /// <param name="order">The order.</param>
        private static void SetInvoiceCustomerWO(Invoice invoice, OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            try
            {
                //Add Customer WO
                if(!string.IsNullOrEmpty(ApplicationSettings.Current.InvoiceCustomerWOField))
                    invoice.SetCustomField(ApplicationSettings.Current.InvoiceCustomerWOField, order.IsCustomerWONull() ? "-None-" : order.CustomerWO.TrimToMaxLength(30));
            }
            catch(Exception exc)
            {
                string error = "Error setting Customer WO for WO {0}".FormatWith(order.OrderID);
                LogManager.GetCurrentClassLogger().Info(exc, error);
            }
        }

        /// <summary>
        /// Sets the invoice tracking number.
        /// </summary>
        /// <param name="invoice">The invoice.</param>
        /// <param name="order">The order.</param>
        private static void SetInvoiceTrackingNumber(Invoice invoice, OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            string trackingNumber = "-None-";

            try
            {
                //Add Tracking Number
                if (!string.IsNullOrEmpty(ApplicationSettings.Current.InvoiceTrackingNumberField))
                {
                    OrderInvoiceDataSet.OrderShipmentRow[] shipments = order.GetOrderShipmentRows();

                    if (shipments.Length > 0 && !shipments[0].IsTrackingNumberNull())
                    {
                        trackingNumber = shipments[0].TrackingNumber.Split(',').First().TrimToMaxLength(30);
                    }

                    if(invoice != null)
                        invoice.SetCustomField(ApplicationSettings.Current.InvoiceTrackingNumberField, trackingNumber);
                }
            }
            catch(Exception exc)
            {
                string error = "Error setting tracking number '{0}' for WO '{1}'".FormatWith(trackingNumber, order.OrderID);
                LogManager.GetCurrentClassLogger().Info(exc, error);
            }
        }

        /// <summary>
        /// Sets the name of the invoice class.
        /// </summary>
        /// <param name="invoice">The invoice.</param>
        /// <param name="order">The order.</param>
        private static void SetInvoiceClassName(Invoice invoice, OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            try
            {
                //Add QB ClassName -> A reference to the class of transaction.
                if (!string.IsNullOrEmpty(ApplicationSettings.Current.QBClass))
                    invoice.SetCustomField("ClassName", ApplicationSettings.Current.QBClass);
            }
            catch(Exception exc)
            {
                string error = "Error setting ClassName to {0} for WO {1}".FormatWith(ApplicationSettings.Current.QBClass, order.OrderID);
                LogManager.GetCurrentClassLogger().Info(exc, error);
            }
        }

        /// <summary>
        /// Gets the customer from qb.
        /// </summary>
        /// <param name="customerName">Name of the customer.</param>
        /// <returns></returns>
        private string GetCustomerFromQB(string customerName)
        {
            try
            {
                _customer.Reset();
                _customer.GetByName(customerName);

                return string.Empty;
            }
            catch (Exception)
            {
                return "Unable to locate customer: {0}".FormatWith(customerName);
            }
        }

        /// <summary>
        /// Does the customer exist.
        /// </summary>
        /// <param name="customerName">Name of the customer.</param>
        /// <returns></returns>
        private bool DoesCustomerExist(string customerName)
        {
            try
            {
                //use cache if already checked
                if(this._customerInQBCache.ContainsKey(customerName))
                    return this._customerInQBCache[customerName];
                else
                {
                    _customer.Reset();

                    this._customer.GetByName(customerName);

                    //Reset Values
                    ClearCustomCustomerField(this._customer, ApplicationSettings.Current.InvoiceCustomerWOField);
                    ClearCustomCustomerField(this._customer, ApplicationSettings.Current.InvoiceTrackingNumberField);

                    //cache the results
                    this._customerInQBCache.Add(customerName, true);

                    return true;
                }
            }
            catch(Exception exc)
            {
                _log.Warn(exc, "Customer does not exist in QuickBooks " + customerName);

                //customer does not exist so cache as false
                this._customerInQBCache.Add(customerName, false);
                return false;
            }
        }

        /// <summary>
        /// Does the item exist in quickbooks?
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <returns>true if the item exists; otherwise, false</returns>
        private bool DoesItemExistInQB(string itemName)
        {
            try
            {
                //use cache if already checked
                if (this._itemInQBCache.ContainsKey(itemName))
                {
                    return this._itemInQBCache[itemName];
                }
                else
                {
                    _qbItem.Reset();
                    _qbItem.GetByName(itemName);

                    //cache the results
                    this._itemInQBCache.Add(itemName, true);

                    return !string.IsNullOrEmpty(_qbItem.ItemName);
                }
            }
            catch (Exception exc)
            {
                _log.Warn(exc, "Item does not exist in QuickBooks " + itemName);
                return false;
            }
        }

        /// <summary>
        /// Clears the custom customer field. This is required because it cannot be set for the invoice if it is already set at the customer level.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <param name="fieldName">Name of the field.</param>
        private static void ClearCustomCustomerField(Customer customer, string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                return;
            }

            try
            {
                string customValue = customer.GetCustomField(fieldName);

                if (!string.IsNullOrEmpty(customValue))
                {
                    LogManager.GetCurrentClassLogger().Info("Clearing {0} from {1} for customer '{2}'.", fieldName, customValue, customer.CustomerName);
                    customer.SetCustomField(fieldName, string.Empty);
                }
            }
            catch (NullReferenceException exc)
            {
                // With QB2012, this clear operation might fail.
                LogManager.GetCurrentClassLogger().Warn(exc, "Error clearing QB field {0}", fieldName);
            }
        }

        /// <summary>
        /// Calculates the due date of the order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        private static DateTime CalculateDueDate(OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            DateTime dueDate = order.IsCompletedDateNull() ? DateTime.Now : order.CompletedDate;

            if(!order.OrderInvoiceCustomerRow.IsPaymentTermsNull())
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

        /// <summary>
        /// Prints the and email preferences.
        /// </summary>
        /// <param name="order">The order.</param>
        private void SetInvoicePreferences(OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            try
            {
                var isValidEmail = false;
                if (!string.IsNullOrEmpty(_customer.Email))
                    isValidEmail = _customer.Email.IsValidEmail();

                if (!string.IsNullOrEmpty(_customer.Email) && isValidEmail && _lazyCompanyValidEmail.Value)
                    this._invoice.Config("IsToBeEmailed=" + order.OrderInvoiceCustomerRow.EmailInvoice);

                this._invoice.IsToBePrinted = order.OrderInvoiceCustomerRow.PrintInvoice;
            }
            catch (Exception exc)
            {
                _log.Warn(exc, $"Error setting QuickBooks email/print preferences for WO {order.OrderID}.");
            }
        }

        private bool CheckCompanyEmail()
        {
            Company company = null;

            try
            {
                company = new Company()
                {
                    QBConnectionString = ApplicationSettings.Current.QBConnectionString,
                };

                company.Get();

                var email = company.Email;
                var isEmailValid = !string.IsNullOrEmpty(email) && email.IsValidEmail();

                if (!isEmailValid)
                {
                    _log.Warn("Company email \"{0}\" is invalid.", email);
                }

                return isEmailValid;
            }
            finally
            {
                company?.Dispose();
            }
        }


        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_customer != null)
            {
                _customer.CloseQBConnection();
                _customer.Dispose();
            }
            _customer = null;
            
            if (_invoice != null)
            {
                _invoice.CloseQBConnection();
                _invoice.Dispose();
            }
            _invoice = null;

            if (_qbLists != null)
            {
                _qbLists.CloseQBConnection();
                _qbLists.Dispose();
            }

            _qbLists = null;

            if (_qbItem != null)
            {
                _qbItem.CloseQBConnection();
                _qbItem.Dispose();
            }

            _qbItem = null;

            OrderInvoices?.Dispose();
            OrderInvoices = null;

            _taOrders?.Dispose();
            _taOrders = null;

            _taOrderInvoice?.Dispose();
            _taOrderInvoice = null;
        }

        #endregion
    }
}