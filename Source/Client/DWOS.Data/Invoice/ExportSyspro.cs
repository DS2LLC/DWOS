using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderInvoiceDataSetTableAdapters;
using NLog;

namespace DWOS.Data.Invoice
{
    /// <summary>
    /// <see cref="IInvoiceExporter"/> implementation that integrates with SYSPRO.
    /// </summary>
    public class ExportSyspro : IInvoiceExporter
    {
        #region Fields

        private const string COLUMN_IMPORTED = "Imported";
        private const string COLUMN_EXPORTED = "Export";
        private const string COLUMN_ISSUES = "Issues";
        private const string INVOICE_DATE_FORMAT = "yyyy-MM-dd";
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly ISysproInvoicePersistence _invoicePersistence;
        private OrderInvoiceTableAdapter _taOrderInvoice;
        private OrderInvoiceSerialNumberTableAdapter _taSerial;
        private OrderInvoiceProductClassTableAdapter _taProductClass;
        private OrderCustomFieldsSummaryTableAdapter _taCustomField;
        private OrderInvoiceCustomerTableAdapter _taCustomer;
        private OrderInvoiceCustomerAddressTableAdapter _taCustomerAddress;
        private OrderInvoiceSalesOrderTableAdapter _taSalesOrder;
        private PartSummaryTableAdapter _taPartSummary;
        private OrderFeesTableAdapter _taOrderFees;
        private OrderFeeTypeTableAdapter _taOrderFeeType;
        private OrderShipmentTableAdapter _taOrderShipmentTable;
        private OrderProcessingDetailTableAdapter _taOrderProcessingDetail;
        private SysproInvoiceTableAdapter _taSyspro;
        private SysproInvoiceOrderTableAdapter _taSysproOrder;
        private SysproInvoiceSettings _settings;

        #endregion

        #region Methods

        public ExportSyspro(ISysproInvoicePersistence invoicePersistence)
        {
            _invoicePersistence = invoicePersistence;
        }

        private void SetupPersistence()
        {
            OrderInvoices = new OrderInvoiceDataSet();
            _taOrderInvoice = new OrderInvoiceTableAdapter();
            _taSerial = new OrderInvoiceSerialNumberTableAdapter();
            _taProductClass = new OrderInvoiceProductClassTableAdapter();
            _taCustomField = new OrderCustomFieldsSummaryTableAdapter();
            _taCustomer = new OrderInvoiceCustomerTableAdapter();
            _taCustomerAddress = new OrderInvoiceCustomerAddressTableAdapter();
            _taSalesOrder = new OrderInvoiceSalesOrderTableAdapter();
            _taPartSummary = new PartSummaryTableAdapter();
            _taOrderFees = new OrderFeesTableAdapter();
            _taOrderFeeType = new OrderFeeTypeTableAdapter();
            _taOrderShipmentTable = new OrderShipmentTableAdapter();
            _taOrderProcessingDetail = new OrderProcessingDetailTableAdapter();
            _taSyspro = new SysproInvoiceTableAdapter();
            _taSysproOrder = new SysproInvoiceOrderTableAdapter();

            OrderInvoices.EnforceConstraints = false;

            OrderInvoices.OrderInvoice.BeginLoadData();
            OrderInvoices.OrderInvoiceSerialNumber.BeginLoadData();
            OrderInvoices.OrderInvoiceProductClass.BeginLoadData();
            OrderInvoices.OrderCustomFieldsSummary.BeginLoadData();
            OrderInvoices.OrderInvoiceCustomer.BeginLoadData();
            OrderInvoices.OrderInvoiceCustomerAddress.BeginLoadData();
            OrderInvoices.OrderInvoiceSalesOrder.BeginLoadData();
            OrderInvoices.PartSummary.BeginLoadData();
            OrderInvoices.OrderFees.BeginLoadData();
            OrderInvoices.OrderFeeType.BeginLoadData();
            OrderInvoices.OrderShipment.BeginLoadData();
            OrderInvoices.OrderProcessingDetail.BeginLoadData();
            OrderInvoices.d_Department.BeginLoadData();

            _taOrderInvoice.Fill(OrderInvoices.OrderInvoice);
            _taSerial.Fill(OrderInvoices.OrderInvoiceSerialNumber);
            _taProductClass.Fill(OrderInvoices.OrderInvoiceProductClass);
            _taCustomField.Fill(OrderInvoices.OrderCustomFieldsSummary);
            _taCustomer.Fill(OrderInvoices.OrderInvoiceCustomer);
            _taCustomerAddress.Fill(OrderInvoices.OrderInvoiceCustomerAddress);
            _taSalesOrder.Fill(OrderInvoices.OrderInvoiceSalesOrder);
            _taPartSummary.Fill(OrderInvoices.PartSummary);
            _taOrderFees.Fill(OrderInvoices.OrderFees);
            _taOrderFeeType.Fill(OrderInvoices.OrderFeeType);
            _taOrderShipmentTable.Fill(OrderInvoices.OrderShipment);
            _taOrderProcessingDetail.Fill(OrderInvoices.OrderProcessingDetail);

            OrderInvoices.OrderInvoice.EndLoadData();
            OrderInvoices.OrderInvoiceSerialNumber.EndLoadData();
            OrderInvoices.OrderInvoiceProductClass.EndLoadData();
            OrderInvoices.OrderCustomFieldsSummary.EndLoadData();
            OrderInvoices.OrderInvoiceCustomer.EndLoadData();
            OrderInvoices.OrderInvoiceCustomerAddress.EndLoadData();
            OrderInvoices.OrderInvoiceSalesOrder.EndLoadData();
            OrderInvoices.PartSummary.EndLoadData();
            OrderInvoices.OrderFees.EndLoadData();
            OrderInvoices.OrderFeeType.EndLoadData();
            OrderInvoices.OrderShipment.EndLoadData();
            OrderInvoices.OrderProcessingDetail.EndLoadData();

            //Add column if does not exist
            if (!OrderInvoices.OrderInvoice.Columns.Contains(COLUMN_EXPORTED))
                OrderInvoices.OrderInvoice.Columns.Add(COLUMN_EXPORTED, typeof(bool)).DefaultValue = false;

            if (!OrderInvoices.OrderInvoice.Columns.Contains(COLUMN_IMPORTED))
                OrderInvoices.OrderInvoice.Columns.Add(COLUMN_IMPORTED, typeof(bool)).DefaultValue = false;

            if (!OrderInvoices.OrderInvoice.Columns.Contains(COLUMN_ISSUES))
                OrderInvoices.OrderInvoice.Columns.Add(COLUMN_ISSUES, typeof(string));
        }

        private void SetupExport()
        {
            foreach (var order in OrderInvoices.OrderInvoice)
            {
                var invoiceLevel = InvoiceHelpers.GetLevel(order);

                order["Invoice"] = InvoiceHelpers.GetInvoiceWithPrefix(order);

                if (invoiceLevel == InvoiceLevelType.SalesOrder)
                {
                    if (ApplicationSettings.Current.IndexSOInvoices)
                        order["Invoice"] = InvoiceHelpers.GetSOInvoiceWithSuffix(order);
                    else
                        order["invoice"] = InvoiceHelpers.GetSOInvoice(order);
                }

                order[COLUMN_EXPORTED] = order.IsSalesOrderIDNull() ||
                                         !OpenOrdersInSalesOrder(order.SalesOrderID);
            }
        }

        /// <summary>
        /// Determines if the sales order has any open orders.
        /// </summary>
        /// <param name="salesOrderId">The sales order identifier.</param>
        /// <returns></returns>
        private bool OpenOrdersInSalesOrder(int salesOrderId)
        {
            using (var dtOrder = new OrdersDataSet.OrderDataTable())
            {
                using (var taOrders = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter())
                {
                    taOrders.FillBySalesOrder(dtOrder, salesOrderId);
                }

                return dtOrder
                    .Where(row => !row.IsStatusNull())
                    .Any(row => row.Status.EquivalentTo("Open"));
            }
        }

        private ExportData DoExportSingleFile()
        {
            var currentDate = DateTime.Now;
            var transmissionReference = GetTransmissionReference(currentDate);
            var fileName = _invoicePersistence.GetFileName(transmissionReference);

            if (string.IsNullOrEmpty(fileName))
            {
                _log.Info("User canceled export.");
                return null;
            }
            else if (File.Exists(fileName))
            {
                _log.Warn($"Export file exists: {fileName}");
                return null;
            }

            var rootElement = new XElement("SalesOrders");

            var document = new XDocument()
            {
                Declaration = new XDeclaration("1.0", "Windows-1252", string.Empty)
            };

            document.Add(rootElement);

            var orderIds = new List<int>();
            var exportCount = 0;
            var errorCount = 0;

            try
            {
                ProgessChanged?.Invoke(this, new ProgressChangedEventArgs(0, null));

                rootElement.Add(new XElement("TransmissionHeader",
                    new XElement("TransmissionReference", transmissionReference),
                    new XElement("DatePrepared", currentDate.ToString("yyyy-MM-dd")),
                    new XElement("TimePrepared", currentDate.ToString("HH:mm"))));

                var ordersElement = new XElement("Orders");
                rootElement.Add(ordersElement);

                var ordersToExport = new List<OrderInvoiceDataSet.OrderInvoiceRow>();

                foreach (var order in OrderInvoices.OrderInvoice)
                {
                    order[COLUMN_IMPORTED] = DBNull.Value;
                    order[COLUMN_ISSUES] = DBNull.Value;

                    if (!Convert.ToBoolean(order[COLUMN_EXPORTED]))
                    {
                        continue;
                    }

                    if (order.IsInvoiceNull())
                    {
                        order.Invoice = InvoiceHelpers.GetInvoiceWithPrefix(order);
                    }

                    ordersToExport.Add(order);
                }

                var groupedOrders = ordersToExport.GroupBy(i => i.Invoice);
                foreach (var invoice in groupedOrders)
                {
                    var invoiceList = invoice.ToList();
                    var successfulExport = true;
                    try
                    {
                        var invoiceElements = ExportOrderElements(invoiceList, 1);
                        foreach (var elem in invoiceElements)
                        {
                            ordersElement.Add(elem);
                        }
                    }
                    catch (Exception exc)
                    {
                        successfulExport = false;
                        errorCount += 1;
                        foreach (var order in invoice)
                        {
                            order[COLUMN_IMPORTED] = false;
                            order[COLUMN_ISSUES] = "Error exporting invoice: " + exc.Message;
                        }

                        _log.Error(exc, "Error exporting invoice.");
                    }

                    var progressPercentage = Convert.ToInt32(exportCount / (double)invoiceList.Count * 100.0);

                    ProgessChanged?.Invoke(this,
                        new ProgressChangedEventArgs(progressPercentage, null));

                    if (successfulExport)
                    {
                        exportCount += 1;
                        orderIds.AddRange(invoice.Select(m => m.OrderID));

                        foreach (var order in invoice)
                        {
                            _taOrderInvoice.AddInvoice(order.Invoice, order.OrderID);
                            order[COLUMN_IMPORTED] = true;
                        }

                        var isGroupForSalesOrder = invoice.All(row => !row.IsSalesOrderIDNull()) &&
                                                   invoice.Select(row => row.SalesOrderID).Distinct().Count() == 1;

                        if (isGroupForSalesOrder)
                        {
                            var firstOrder = invoice.First();
                            _taOrderInvoice.AddSalesOrderInvoice(firstOrder.Invoice, firstOrder.SalesOrderID);
                        }
                    }

                    if (errorCount >= ApplicationSettings.Current.InvoiceExportMaxErrors)
                    {
                        _log.Info("Aborting the export process, max number of errors per session has occurred.");
                        break;
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error exporting invoices.");
            }

            _log.Info("Exported invoices: " + exportCount);

            return new ExportData
            {
                Documents = new List<ExportDocument>
                {
                    new ExportDocument
                    {
                        Document = document,
                        FileName = fileName,
                        TransmissionReference = transmissionReference,
                        ExportedOrderIds = orderIds
                    }
                },

                ExportCount = exportCount,
                ErrorCount = errorCount
            };
        }

        private ExportData DoExportMultipleFiles()
        {
            var currentDate = DateTime.Now;
            var directory = _invoicePersistence.GetDirectory();

            if (string.IsNullOrEmpty(directory))
            {
                // User canceled
                return null;
            }

            var exportCount = 0;
            var errorCount = 0;
            var exportDocuments = new List<ExportDocument>();

            try
            {
                ProgessChanged?.Invoke(this, new ProgressChangedEventArgs(0, null));

                var ordersToExport = new List<OrderInvoiceDataSet.OrderInvoiceRow>();

                foreach (var order in OrderInvoices.OrderInvoice)
                {
                    order[COLUMN_IMPORTED] = DBNull.Value;
                    order[COLUMN_ISSUES] = DBNull.Value;

                    if (!Convert.ToBoolean(order[COLUMN_EXPORTED]))
                    {
                        continue;
                    }

                    if (order.IsInvoiceNull())
                    {
                        order.Invoice = InvoiceHelpers.GetInvoiceWithPrefix(order);
                    }

                    ordersToExport.Add(order);
                }

                var groupedOrders = ordersToExport.GroupBy(i => i.Invoice);
                foreach (var invoice in groupedOrders)
                {
                    var invoiceList = invoice.ToList();

                    var transmissionReference = GetTransmissionReference(invoice.Key);

                    bool successfulExport;
                    var rootElement = new XElement("SalesOrders");

                    var document = new XDocument
                    {
                        Declaration = new XDeclaration("1.0", "Windows-1252", string.Empty)
                    };

                    document.Add(rootElement);

                    var fileName = Path.Combine(directory, $"{transmissionReference}.xml");

                    if (File.Exists(fileName))
                    {
                        successfulExport = false;
                        errorCount++;

                        foreach (var order in invoice)
                        {
                            order[COLUMN_IMPORTED] = false;
                            order[COLUMN_ISSUES] = "Error exporting invoice: file exists.";
                        }
                    }
                    else
                    {
                        successfulExport = true;

                        rootElement.Add(new XElement("TransmissionHeader",
                            new XElement("TransmissionReference", transmissionReference),
                            new XElement("DatePrepared", currentDate.ToString("yyyy-MM-dd")),
                            new XElement("TimePrepared", currentDate.ToString("HH:mm"))));

                        var ordersElement = new XElement("Orders");
                        rootElement.Add(ordersElement);

                        try
                        {
                            var invoiceElements = ExportOrderElements(invoiceList, 1);
                            foreach (var elem in invoiceElements)
                            {
                                ordersElement.Add(elem);
                            }
                        }
                        catch (Exception exc)
                        {
                            successfulExport = false;
                            errorCount++;
                            foreach (var order in invoice)
                            {
                                order[COLUMN_IMPORTED] = false;
                                order[COLUMN_ISSUES] = "Error exporting invoice: " + exc.Message;
                            }

                            _log.Error(exc, "Error exporting invoice.");
                        }
                    }

                    var progressPercentage = Convert.ToInt32(exportCount / (double)invoiceList.Count * 100.0);

                    ProgessChanged?.Invoke(this,
                        new ProgressChangedEventArgs(progressPercentage, null));

                    if (successfulExport)
                    {
                        exportCount++;
                        var orderIds = new List<int>();
                        orderIds.AddRange(invoice.Select(m => m.OrderID));

                        foreach (var order in invoice)
                        {
                            _taOrderInvoice.AddInvoice(order.Invoice, order.OrderID);
                            order[COLUMN_IMPORTED] = true;
                        }

                        var isGroupForSalesOrder = invoice.All(row => !row.IsSalesOrderIDNull()) &&
                                                   invoice.Select(row => row.SalesOrderID).Distinct().Count() == 1;

                        if (isGroupForSalesOrder)
                        {
                            var firstOrder = invoice.First();
                            _taOrderInvoice.AddSalesOrderInvoice(firstOrder.Invoice, firstOrder.SalesOrderID);
                        }

                        exportDocuments.Add(new ExportDocument
                        {
                            Document = document,
                            TransmissionReference = transmissionReference,
                            FileName = fileName,
                            ExportedOrderIds = orderIds
                        });
                    }

                    if (errorCount >= ApplicationSettings.Current.InvoiceExportMaxErrors)
                    {
                        _log.Info("Aborting the export process, max number of errors per session has occurred.");
                        break;
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error exporting invoices.");
            }

            _log.Info("Exported invoices: " + exportCount);

            return new ExportData
            {
                Documents = exportDocuments,
                ExportCount = exportCount,
                ErrorCount = errorCount
            };
        }

        private static string GetTransmissionReference(DateTime currentDate)
        {
            var ms = Convert.ToInt32((currentDate - currentDate.Date).TotalMilliseconds);

            // The reference can be anything as long as it is 12 alphanumeric characters long.
            // This particular one includes:
            // - Last two digits of the year
            // - Day of the year (always padded to 3 digits)
            // - Milliseconds since midnight represented in hexadecimal
            //
            // Warning: DateTime.Now is not precise up to the millsecond,
            // so it is still possible to have a duplicate reference.
            return $"{currentDate:yy}{currentDate.DayOfYear:000}{ms:X7}";
        }

        private static string GetTransmissionReference(string invoice)
        {
            const int maxLengthTransmissionReference = 12;

            return new Regex(@"[^a-zA-z0-9]")
                .Replace(invoice, string.Empty)
                .TrimToMaxLength(maxLengthTransmissionReference)
                .PadLeft(maxLengthTransmissionReference, '0');
        }

        private List<XElement> ExportOrderElements(IList<OrderInvoiceDataSet.OrderInvoiceRow> invoiceOrders,
            int startingPoLine)
        {
            var currentPoLine = startingPoLine;
            var orderHeader = ExportOrderHeader(invoiceOrders);
            var orderElements = new List<XElement>();

            foreach (var order in invoiceOrders)
            {
                var firstPoNumber = currentPoLine;

                // Line Items
                if (_settings.LineItem == SysproInvoiceSettings.LineItemType.Part)
                {
                    orderElements.Add(ExportPartStockLine(order, currentPoLine));
                    currentPoLine++;
                }
                else if (_settings.LineItem == SysproInvoiceSettings.LineItemType.Process)
                {
                    var stockLines = ExportProcessStockLines(order, currentPoLine);
                    orderElements.AddRange(stockLines);
                    currentPoLine += stockLines.Count;
                }

                // Comments after StockLine
                var stockLineCommentsLines = ExportCommentLines(order,
                    SysproInvoiceSettings.CommentPosition.AfterStockLine, currentPoLine, firstPoNumber);

                orderElements.AddRange(stockLineCommentsLines);
                currentPoLine += stockLineCommentsLines.Count;

                // Fees
                var feeLines = ExportFees(order, currentPoLine);
                orderElements.AddRange(feeLines);
                currentPoLine += feeLines.Count;

                // Freight Line
                if (_settings.IncludeEmptyFreightLine)
                {
                    orderElements.Add(new XElement("FreightLine",
                      new XElement("CustomerPoLine", currentPoLine),
                      new XElement("LineActionType", "A")));
                }

                // Normal Comments
                var normalCommentLines = ExportCommentLines(order,
                    SysproInvoiceSettings.CommentPosition.AfterEverythingElse, currentPoLine, firstPoNumber);

                orderElements.AddRange(normalCommentLines);
                currentPoLine += normalCommentLines.Count;
            }

            return new List<XElement>
            {
                orderHeader,
                new XElement("OrderDetails", orderElements)
            };
        }

        private XElement ExportOrderHeader(IList<OrderInvoiceDataSet.OrderInvoiceRow> orders)
        {
            var element = new XElement("OrderHeader",
                new XElement("OrderActionType", "A"));

            var firstOrder = orders.FirstOrDefault();

            if (firstOrder == null)
            {
                _log.Error("Attempted to create an order header for an invoice with zero orders.");
                return element;
            }

            OrderInvoiceDataSet.OrderInvoiceSalesOrderRow salesOrder =
                null;

            if (!firstOrder.IsSalesOrderIDNull())
            {
                var salesOrderId = firstOrder.SalesOrderID;

                if (orders.All(o => !o.IsSalesOrderIDNull() && o.SalesOrderID == salesOrderId))
                {
                    salesOrder = firstOrder.GetOrderInvoiceSalesOrderRows().FirstOrDefault();
                }
            }

            foreach (var literal in _settings.OrderHeaderMap.Literals)
            {
                element.Add(new XElement(literal.Syspro, literal.Value));
            }

            foreach (var customField in _settings.OrderHeaderMap.CustomFields)
            {
                element.Add(new XElement(customField.Syspro, GetCustomField(firstOrder, customField.TokenName)));
            }

            if (salesOrder == null)
            {
                foreach (var field in _settings.OrderHeaderMap.Fields)
                {
                    element.Add(new XElement(field.Syspro, GetField(firstOrder, field.Dwos)));
                }
            }
            else
            {
                foreach (var field in _settings.OrderHeaderMap.Fields)
                {
                    var fieldValue = GetField(salesOrder, field.Dwos);

                    if (string.IsNullOrEmpty(fieldValue))
                    {
                        fieldValue = GetField(firstOrder, field.Dwos);
                    }

                    element.Add(new XElement(field.Syspro, fieldValue));
                }
            }

            return element;
        }

        private XElement ExportPartStockLine(OrderInvoiceDataSet.OrderInvoiceRow order, int poLine)
        {
            var element = new XElement("StockLine",
                new XElement("LineActionType", "A"),
                new XElement("CustomerPoLine", poLine));

            foreach (var literal in _settings.StockLineMap.Literals)
            {
                element.Add(new XElement(literal.Syspro, literal.Value));
            }

            foreach (var customField in _settings.StockLineMap.CustomFields)
            {
                element.Add(new XElement(customField.Syspro, GetCustomField(order, customField.TokenName)));
            }

            foreach (var field in _settings.StockLineMap.Fields)
            {
                element.Add(new XElement(field.Syspro, GetField(order, field.Dwos)));
            }

            return element;
        }

        private List<XElement> ExportProcessStockLines(OrderInvoiceDataSet.OrderInvoiceRow order, int startingPoLine)
        {
            var processingBasePrice =
                order.GetOrderProcessingDetailRows().Sum(row => row.IsAmountNull() ? 0M : row.Amount);

            if (processingBasePrice <= 0M)
            {
                _log.Info($"{order.OrderID} is missing per-process price information.");
                return new List<XElement> { ExportPartStockLine(order, startingPoLine) };
            }

            var orderTotal = OrderPrice.CalculatePrice(order.IsBasePriceNull() ? 0 : order.BasePrice,
                order.IsPriceUnitNull() ? string.Empty : order.PriceUnit,
                0M,
                order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                order.IsWeightNull() ? 0M : order.Weight);

            var processingTotal = OrderPrice.CalculatePrice(processingBasePrice,
                order.IsPriceUnitNull() ? string.Empty : order.PriceUnit,
                0M,
                order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                order.IsWeightNull() ? 0M : order.Weight);

            var processingFactor = 0M;

            if (processingTotal != 0M)
            {
                processingFactor = orderTotal / processingTotal;
            }

            var elements = new List<XElement>();
            var currentPoLine = startingPoLine;

            foreach (var orderProcess in order.GetOrderProcessingDetailRows())
            {
                var element = new XElement("StockLine",
                    new XElement("LineActionType", "A"),
                    new XElement("CustomerPoLine", currentPoLine));

                var processAmount = orderProcess.IsAmountNull() ? 0M : orderProcess.Amount;
                var lineItemBasePrice = processAmount * processingFactor;

                var lineItemAmount = OrderPrice.CalculatePrice(lineItemBasePrice,
                    order.IsPriceUnitNull() ? string.Empty : order.PriceUnit,
                    0M,
                    order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                    order.IsWeightNull() ? 0M : order.Weight);

                foreach (var literal in _settings.StockLineMap.Literals)
                {
                    element.Add(new XElement(literal.Syspro, literal.Value));
                }

                foreach (var customField in _settings.StockLineMap.CustomFields)
                {
                    element.Add(new XElement(customField.Syspro, GetCustomField(order, customField.TokenName)));
                }

                foreach (var field in _settings.StockLineMap.Fields)
                {
                    var fieldValue = GetField(orderProcess, field.Dwos, lineItemAmount);

                    if (string.IsNullOrEmpty(fieldValue) && field.Dwos != SysproInvoiceSettings.FieldType.ProcessProductCode)
                    {
                        fieldValue = GetField(order, field.Dwos);
                    }

                    element.Add(new XElement(field.Syspro, fieldValue));
                }

                elements.Add(element);
                currentPoLine++;
            }

            return elements;
        }

        private List<XElement> ExportFees(OrderInvoiceDataSet.OrderInvoiceRow order, int startingPoLine)
        {
            var elements = new List<XElement>();
            var currentPoLine = startingPoLine;

            foreach (var fee in order.GetOrderFeesRows())
            {
                if (!_settings.IncludeDiscountsInFees && fee.Charge < 0)
                {
                    // Skip discount
                    continue;
                }

                var feeElement = new XElement("StockLine",
                    new XElement("LineActionType", "A"),
                    new XElement("CustomerPoLine", currentPoLine));

                foreach (var literal in _settings.StockLineFeeMap.Literals)
                {
                    feeElement.Add(new XElement(literal.Syspro, literal.Value));
                }

                foreach (var customField in _settings.StockLineFeeMap.CustomFields)
                {
                    feeElement.Add(new XElement(customField.Syspro, GetCustomField(order, customField.TokenName)));
                }

                foreach (var field in _settings.StockLineFeeMap.Fields)
                {
                    var fieldValue = GetField(fee, field.Dwos);

                    if (string.IsNullOrEmpty(fieldValue))
                    {
                        fieldValue = GetField(order, field.Dwos);
                    }

                    feeElement.Add(new XElement(field.Syspro, fieldValue));
                }

                elements.Add(feeElement);
                currentPoLine++;
            }

            return elements;
        }

        private List<XElement> ExportCommentLines(OrderInvoiceDataSet.OrderInvoiceRow order,
            SysproInvoiceSettings.CommentPosition position, int startingPoLine,
            int attachedLineNumber)
        {
            var currentPoLine = startingPoLine;
            var elements = new List<XElement>();

            var comments = _settings.CommentMap
                .AllFields()
                .Where(i => i.Position == position)
                .OrderBy(i => i.Order);

            foreach (var exportComment in comments)
            {
                var literal = exportComment as SysproInvoiceSettings.CommentLiteral;
                if (literal != null)
                {
                    var comment = ExportCommentLiteral(literal, currentPoLine);
                    comment.Add(new XElement("AttachedLineNumber", attachedLineNumber));
                    elements.Add(comment);
                    currentPoLine++;
                    continue;
                }

                var customField = exportComment as SysproInvoiceSettings.CommentCustomField;
                if (customField != null)
                {
                    var comment = ExportCommentCustomField(customField, order, currentPoLine);
                    comment.Add(new XElement("AttachedLineNumber", attachedLineNumber));
                    elements.Add(comment);
                    currentPoLine++;
                    continue;
                }

                var field = exportComment as SysproInvoiceSettings.CommentField;
                if (field != null)
                {
                    var comment = ExportCommentField(field, order, currentPoLine);
                    comment.Add(new XElement("AttachedLineNumber", attachedLineNumber));
                    elements.Add(comment);
                    currentPoLine++;
                    continue;
                }

                _log.Error($"Comment has unsupported type:{exportComment.GetType()}");
            }

            return elements;
        }

        private XElement ExportCommentField(SysproInvoiceSettings.CommentField field,
            OrderInvoiceDataSet.OrderInvoiceRow order, int poNumber)
        {
            var commentType = field.Type == SysproInvoiceSettings.CommentType.Order ? "O" : "I";

            var comment = string.IsNullOrEmpty(field.Format)
                ? GetField(order, field.Dwos)
                : string.Format(field.Format, GetField(order, field.Dwos));

            return new XElement("CommentLine",
                new XElement("LineActionType", "A"),
                new XElement("CustomerPoLine", poNumber),
                new XElement("Comment", comment),
                new XElement("CommentType", commentType));
        }

        private XElement ExportCommentCustomField(SysproInvoiceSettings.CommentCustomField field,
            OrderInvoiceDataSet.OrderInvoiceRow order, int poNumber)
        {
            var commentType = field.Type == SysproInvoiceSettings.CommentType.Order ? "O" : "I";

            var comment = string.IsNullOrEmpty(field.Format)
                ? GetCustomField(order, field.TokenName)
                : string.Format(field.Format, GetCustomField(order, field.TokenName));

            return new XElement("CommentLine",
                new XElement("LineActionType", "A"),
                new XElement("CustomerPoLine", poNumber),
                new XElement("Comment", comment),
                new XElement("CommentType", commentType));
        }

        private XElement ExportCommentLiteral(SysproInvoiceSettings.CommentLiteral field, int poNumber)
        {
            var commentType = field.Type == SysproInvoiceSettings.CommentType.Order ? "O" : "I";
            var comment = field.Value;

            return new XElement("CommentLine",
                new XElement("LineActionType", "A"),
                new XElement("CustomerPoLine", poNumber),
                new XElement("Comment", comment),
                new XElement("CommentType", commentType));
        }

        private string GetField(OrderInvoiceDataSet.OrderInvoiceRow order, SysproInvoiceSettings.FieldType dwos)
        {
            var customer = order.OrderInvoiceCustomerRow;
            var orderProcess = order.GetOrderProcessingDetailRows().OrderBy(op => op.StepOrder).FirstOrDefault();
            var orderShipment = order.GetOrderShipmentRows().FirstOrDefault();
            var shippingAddress = orderShipment?.OrderInvoiceCustomerAddressRow;
            var serialNumber = order.GetOrderInvoiceSerialNumberRows().Where(s => s.Active).OrderBy(s => s.PartOrder);
            var productClass = order.GetOrderInvoiceProductClassRows().FirstOrDefault();

            var fieldValue = string.Empty;

            var priceUnit = order.IsPriceUnitNull()
                ? OrderPrice.enumPriceUnit.Each
                : OrderPrice.ParsePriceUnit(order.PriceUnit);

            switch (dwos)
            {
                case SysproInvoiceSettings.FieldType.OrderId:
                    fieldValue = order.OrderID.ToString();
                    break;
                case SysproInvoiceSettings.FieldType.OrderDate:
                    fieldValue = order.IsOrderDateNull()
                        ? string.Empty
                        : order.OrderDate.ToString(INVOICE_DATE_FORMAT);
                    break;
                case SysproInvoiceSettings.FieldType.OrderRequiredDate:
                    fieldValue = order.IsRequiredDateNull()
                        ? string.Empty
                        : order.RequiredDate.ToString(INVOICE_DATE_FORMAT);
                    break;
                case SysproInvoiceSettings.FieldType.OrderPurchaseOrder:
                    fieldValue = order.IsPurchaseOrderNull() ? string.Empty : order.PurchaseOrder;
                    break;
                case SysproInvoiceSettings.FieldType.OrderCustomerWo:
                    fieldValue = order.IsCustomerWONull() ? string.Empty : order.CustomerWO;
                    break;
                case SysproInvoiceSettings.FieldType.OrderSerialNumber:
                    var serialNos = "";

                    foreach (var v in serialNumber)
                    {
                        if ((v != null) && !v.IsNumberNull())
                        {
                            serialNos += v.Number + ",";
                        }
                    }

                    serialNos = serialNos.RemoveFromEnd(",");
                    fieldValue = serialNos;
                    break;
                case SysproInvoiceSettings.FieldType.OrderProductClass:
                    if (productClass != null)
                    {
                        fieldValue = productClass.IsProductClassNull() ? string.Empty : productClass.ProductClass;
                    }
                    break;
                case SysproInvoiceSettings.FieldType.OrderQuantity:
                    if (OrderPrice.GetPriceByType(priceUnit) == PriceByType.Quantity)
                    {
                        fieldValue = (order.IsPartQuantityNull() ? 0 : order.PartQuantity).ToString();
                    }
                    else
                    {
                        var weight = (order.IsWeightNull() ? 0M : order.Weight);
                        fieldValue = FormatWeight(weight);
                    }
                    break;
                case SysproInvoiceSettings.FieldType.OrderUnitPrice:
                    if (!order.IsBasePriceNull())
                    {
                        fieldValue = FormatPrice(order.BasePrice);
                    }
                    break;
                case SysproInvoiceSettings.FieldType.OrderSubtotal:
                    {
                        var price = OrderPrice.CalculatePrice(
                            order.IsBasePriceNull() ? 0M : order.BasePrice,
                            priceUnit.ToString(),
                            0M,
                            order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                            order.IsWeightNull() ? 0M : order.Weight);

                        fieldValue = FormatPrice(price);
                    }
                    break;
                case SysproInvoiceSettings.FieldType.OrderTotalPrice:
                    {
                        var basePrice = order.IsBasePriceNull() ? 0M : order.BasePrice;
                        var price = OrderPrice.CalculatePrice(
                            basePrice,
                            priceUnit.ToString(),
                            OrderPrice.CalculateFees(order, basePrice),
                            order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                            order.IsWeightNull() ? 0M : order.Weight);

                        fieldValue = FormatPrice(price);
                    }
                    break;
                case SysproInvoiceSettings.FieldType.OrderUnit:
                    if (!order.IsPriceUnitNull())
                    {
                        var priceType = OrderPrice.GetPricingStrategy(OrderPrice.ParsePriceUnit(order.PriceUnit));
                        fieldValue = priceType == PricingStrategy.Each
                            ? _settings.PriceUnitEach
                            : _settings.PriceUnitLot;
                    }
                    break;

                case SysproInvoiceSettings.FieldType.OrderImportValue:
                    if (!order.IsImportedPriceNull())
                    {
                        fieldValue = FormatPrice(order.ImportedPrice);
                    }
                    break;
                case SysproInvoiceSettings.FieldType.OrderInvoice:
                    fieldValue = order.IsInvoiceNull()
                        ? string.Empty
                        : order.Invoice;
                    break;
                case SysproInvoiceSettings.FieldType.OrderShippingCarrier:
                    fieldValue = orderShipment?.ShippingCarrierID;
                    break;
                case SysproInvoiceSettings.FieldType.OrderDiscountTotal:
                    fieldValue = FormatPrice(GetDiscountTotal(order));
                    break;
                case SysproInvoiceSettings.FieldType.OrderDiscountTotalPercent:
                    {
                        var discounts = order.GetOrderFeesRows()
                            .Where(fee => fee.Charge < 0);

                        var discountTotal = GetDiscountTotal(order);

                        var orderSubtotal = OrderPrice.CalculatePrice(
                            order.IsBasePriceNull() ? 0M : order.BasePrice,
                            priceUnit.ToString(),
                            0M,
                            order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                            order.IsWeightNull() ? 0M : order.Weight);

                        if (orderSubtotal != 0)
                        {
                            var percent = (discountTotal / orderSubtotal) * 100;
                            fieldValue = percent.ToString();
                        }
                    }

                    break;
                case SysproInvoiceSettings.FieldType.CustomerAccountingId:
                    if (customer != null)
                    {
                        fieldValue = customer.IsAccountingIDNull() ? string.Empty : customer.AccountingID;
                    }
                    break;
                case SysproInvoiceSettings.FieldType.CustomerAddressLine1:
                    if (customer != null)
                    {
                        fieldValue = customer.IsAddress1Null() ? string.Empty : customer.Address1;
                    }
                    break;
                case SysproInvoiceSettings.FieldType.CustomerAddressLine2:
                    if (customer != null)
                    {
                        fieldValue = customer.IsAddress2Null() ? string.Empty : customer.Address2;
                    }
                    break;
                case SysproInvoiceSettings.FieldType.CustomerAddressCityStateZip:
                    if (customer != null)
                    {
                        var city = customer.IsCityNull() ? null : customer.City;
                        var state = customer.IsStateNull() ? null : customer.State;
                        var zip = customer.IsZipNull() ? null : customer.Zip;
                        fieldValue = $"{city}, {state} {zip}";
                    }
                    break;
                case SysproInvoiceSettings.FieldType.CustomerAddressZip:
                    if (customer != null)
                    {
                        fieldValue = customer.IsZipNull() ? null : customer.Zip;
                    }
                    break;
                case SysproInvoiceSettings.FieldType.CustomerShipName:
                    if (shippingAddress != null)
                    {
                        fieldValue = shippingAddress.Name;
                    }
                    break;
                case SysproInvoiceSettings.FieldType.CustomerShipAddressLine1:
                    if (shippingAddress != null)
                    {
                        fieldValue = shippingAddress.IsAddress1Null() ? string.Empty : shippingAddress.Address1;
                    }
                    break;
                case SysproInvoiceSettings.FieldType.CustomerShipAddressLine2:
                    if (shippingAddress != null)
                    {
                        fieldValue = shippingAddress.IsAddress2Null() ? string.Empty : shippingAddress.Address2;
                    }
                    break;
                case SysproInvoiceSettings.FieldType.CustomerShipAddressCityStateZip:
                    if (shippingAddress != null)
                    {
                        var city = shippingAddress.IsCityNull() ? null : shippingAddress.City;
                        var state = shippingAddress.IsStateNull() ? null : shippingAddress.State;
                        var zip = shippingAddress.IsZipNull() ? null : shippingAddress.Zip;
                        fieldValue = $"{city}, {state} {zip}";
                    }
                    break;
                case SysproInvoiceSettings.FieldType.CustomerShipAddressZip:
                    if (shippingAddress != null)
                    {
                        fieldValue = shippingAddress.IsZipNull() ? string.Empty : shippingAddress.Zip;
                    }
                    break;
                case SysproInvoiceSettings.FieldType.PartName:
                    fieldValue = order.PartSummaryRow?.Name ?? "N/A";
                    break;
                case SysproInvoiceSettings.FieldType.PartDescription:
                    if (order.PartSummaryRow != null)
                    {
                        fieldValue = order.PartSummaryRow.IsDescriptionNull() ? null : order.PartSummaryRow.Description;
                    }
                    break;
                case SysproInvoiceSettings.FieldType.ProcessProductCode:
                    if (orderProcess != null)
                    {
                        fieldValue = orderProcess.IsProcessCodeNull() ? string.Empty : orderProcess.ProcessCode;
                    }
                    break;
                case SysproInvoiceSettings.FieldType.ShipDate:
                    if (orderShipment != null)
                    {
                        fieldValue = orderShipment.DateShipped.ToString(INVOICE_DATE_FORMAT);
                    }
                    break;
                case SysproInvoiceSettings.FieldType.PackageNumber:
                    if (orderShipment != null)
                    {
                        fieldValue = orderShipment.ShipmentPackageID.ToString();
                    }
                    break;
                default:
                    // Unsupported
                    _log.Warn($"Specified invalid field type {dwos} for order.");
                    break;
            }

            return fieldValue;
        }

        private static decimal GetDiscountTotal(OrderInvoiceDataSet.OrderInvoiceRow order)
        {
            var discounts = order.GetOrderFeesRows()
                .Where(fee => fee.Charge < 0);

            var discountTotal = 0M;

            foreach (var discount in discounts)
            {
                discountTotal += OrderPrice.CalculateFees(
                    discount.OrderFeeTypeRow?.FeeType,
                    discount.Charge,
                    order.IsBasePriceNull() ? 0M : order.BasePrice,
                    order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                    order.IsPriceUnitNull() ? nameof(OrderPrice.enumPriceUnit.Each) : order.PriceUnit,
                    order.IsWeightNull() ? 0M : order.Weight);
            }

            return -1 * discountTotal;
        }

        private string GetField(OrderInvoiceDataSet.OrderInvoiceSalesOrderRow salesOrder,
            SysproInvoiceSettings.FieldType dwos)
        {
            string fieldValue;
            switch (dwos)
            {
                case SysproInvoiceSettings.FieldType.OrderId:
                    fieldValue = salesOrder.SalesOrderID.ToString();
                    break;
                case SysproInvoiceSettings.FieldType.OrderDate:
                    fieldValue = salesOrder.IsOrderDateNull()
                        ? string.Empty
                        : salesOrder.OrderDate.ToString(INVOICE_DATE_FORMAT);
                    break;
                case SysproInvoiceSettings.FieldType.OrderRequiredDate:
                    fieldValue = salesOrder.IsRequiredDateNull()
                        ? string.Empty
                        : salesOrder.RequiredDate.ToString(INVOICE_DATE_FORMAT);
                    break;
                case SysproInvoiceSettings.FieldType.OrderPurchaseOrder:
                    fieldValue = salesOrder.IsPurchaseOrderNull() ? string.Empty : salesOrder.PurchaseOrder;
                    break;
                case SysproInvoiceSettings.FieldType.OrderCustomerWo:
                    fieldValue = salesOrder.IsCustomerWONull() ? string.Empty : salesOrder.CustomerWO;
                    break;
                default:
                    // Unsupported
                    fieldValue = null;
                    break;
            }

            return fieldValue;
        }

        private string GetField(OrderInvoiceDataSet.OrderProcessingDetailRow orderProcess,
            SysproInvoiceSettings.FieldType dwos, decimal lineItemPrice)
        {
            string fieldValue;

            switch (dwos)
            {
                case SysproInvoiceSettings.FieldType.ProcessProductCode:
                    fieldValue = orderProcess.IsProcessCodeNull() ? string.Empty : orderProcess.ProcessCode;
                    break;
                case SysproInvoiceSettings.FieldType.OrderSubtotal:
                    fieldValue = lineItemPrice.ToString(CultureInfo.InvariantCulture);
                    break;
                default:
                    // Unsupported
                    fieldValue = null;
                    break;
            }

            return fieldValue;
        }

        private string GetField(OrderInvoiceDataSet.OrderFeesRow fee, SysproInvoiceSettings.FieldType dwos)
        {
            var order = fee.OrderInvoiceRow;
            var feeType = fee.OrderFeeTypeRow;
            var fieldValue = string.Empty;

            switch (dwos)
            {
                case SysproInvoiceSettings.FieldType.FeeName:
                    if (feeType != null)
                    {
                        fieldValue = feeType.OrderFeeTypeID;
                    }
                    break;
                case SysproInvoiceSettings.FieldType.FeeInvoiceItemName:
                    if (feeType != null)
                    {
                        fieldValue = feeType.InvoiceItemName;
                    }
                    break;
                case SysproInvoiceSettings.FieldType.FeeTotal:
                    if (order != null && feeType != null)
                    {
                        var feeTotal = OrderPrice.CalculateFees(feeType.FeeType,
                            fee.Charge,
                            order.IsBasePriceNull() ? 0M : order.BasePrice,
                            order.IsPartQuantityNull() ? 0 : order.PartQuantity,
                            order.IsPriceUnitNull() ? OrderPrice.enumPriceUnit.Each.ToString() : order.PriceUnit,
                            order.IsWeightNull() ? 0M : order.Weight);

                        fieldValue = FormatFee(feeTotal);
                    }
                    break;
                default:
                    // Unsupported
                    fieldValue = null;
                    break;
            }

            return fieldValue;
        }

        private string GetCustomField(OrderInvoiceDataSet.OrderInvoiceRow order, string tokenName)
        {
            var customField = order.GetOrderCustomFieldsSummaryRows()
                .FirstOrDefault(
                    o =>
                        !o.IsTokenNameNull() &&
                        string.Equals(o.TokenName, tokenName, StringComparison.InvariantCultureIgnoreCase));

            if (customField == null || customField.IsValueNull())
            {
                return string.Empty;
            }

            return customField.Value;
        }

        private static string FormatWeight(decimal weight)
        {
            return Math.Round(weight, 6).ToString(CultureInfo.InvariantCulture);
        }

        private static string FormatPrice(decimal priceTotal)
        {
            return Math.Round(priceTotal, 5).ToString(CultureInfo.InvariantCulture);
        }

        private static string FormatFee(decimal feeTotal)
        {
            return Math.Round(feeTotal, 2).ToString(CultureInfo.InvariantCulture);
        }

        private OrderInvoiceDataSet.SysproInvoiceRow SaveSysproRow(ExportDocument doc)
        {
            var newRow = OrderInvoices.SysproInvoice.NewSysproInvoiceRow();
            newRow.TransmissionReference = doc.TransmissionReference;
            newRow.FileName = Path.GetFileName(doc.FileName);
            newRow.Status = SysproInvoiceStatus.Pending.ToString();
            newRow.Created = DateTime.Now;
            OrderInvoices.SysproInvoice.AddSysproInvoiceRow(newRow);

            foreach (var orderId in doc.ExportedOrderIds)
            {
                var newOrderRow = OrderInvoices.SysproInvoiceOrder.NewSysproInvoiceOrderRow();
                newOrderRow.OrderId = orderId;
                newOrderRow.SysproInvoiceId = newRow.SysproInvoiceId;
                OrderInvoices.SysproInvoiceOrder.AddSysproInvoiceOrderRow(newOrderRow);
            }

            _taSyspro.Update(OrderInvoices.SysproInvoice);
            _taSysproOrder.Update(OrderInvoices.SysproInvoiceOrder);

            return newRow;
        }

        private void FailInvoice(OrderInvoiceDataSet.SysproInvoiceRow invoice)
        {
            invoice.Status = SysproInvoiceStatus.Failed.ToString();
            _taSyspro.Update(invoice);
        }

        #endregion

        #region IInvoiceExporter Members

        public event ProgressChangedEventHandler ProgessChanged;

        public OrderInvoiceDataSet OrderInvoices { get; private set; }

        public void Dispose()
        {
            OrderInvoices?.Dispose();
            _taOrderInvoice?.Dispose();
            _taSerial?.Dispose();
            _taProductClass?.Dispose();
            _taCustomField?.Dispose();
            _taCustomer?.Dispose();
            _taCustomerAddress?.Dispose();
            _taSalesOrder?.Dispose();
            _taPartSummary?.Dispose();
            _taOrderFees?.Dispose();
            _taOrderFeeType?.Dispose();
            _taOrderShipmentTable?.Dispose();
            _taOrderProcessingDetail?.Dispose();
        }

        public InvoiceResult Export()
        {
            var succesfulExportCount = 0;
            var errorCount = 0;
            var exportedOrderIDs = new List<int>();

            try
            {
                if (OrderInvoices == null)
                {
                    LoadData();
                }

                if (OrderInvoices == null)
                {
                    throw new InvalidOperationException("Cannot load order invoices");
                }

                var exportData = _settings.GenerateSingleFile
                    ? DoExportSingleFile()
                    : DoExportMultipleFiles();

                if (exportData == null)
                {
                    return new InvoiceResult
                    {
                        ExportedCount = 0,
                        TotalCount = OrderInvoices.OrderInvoice.Count,
                        ExportedOrderIDs = null,
                        ErrorCount = 0,
                        Cancelled = true
                    };
                }

                errorCount = exportData.ErrorCount;
                exportedOrderIDs.AddRange(exportData.Documents.SelectMany(d => d.ExportedOrderIds));
                succesfulExportCount = exportData.ExportCount;

                foreach (var doc in exportData.Documents)
                {
                    var sysproRow = SaveSysproRow(doc);

                    try
                    {
                        _invoicePersistence.Save(doc.Document, doc.FileName);
                        _log.Info(succesfulExportCount + " invoices exported to: " + doc.FileName);
                    }
                    catch (Exception exc)
                    {
                        if (exc is IOException)
                        {
                            //File is being used by another process
                            _log.Debug(exc, "File is being used by another process: " + doc.FileName, exc);
                        }
                        else
                        {
                            _log.Error(exc, "Error saving SYSPRO invoice.");
                        }

                        FailInvoice(sysproRow);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error exporting invoices to XML file.");
            }


            return new InvoiceResult
            {
                ExportedCount = succesfulExportCount,
                TotalCount = OrderInvoices.OrderInvoice.Count,
                ExportedOrderIDs = exportedOrderIDs,
                ErrorCount = errorCount,
                Cancelled = false
            };
        }

        public void LoadData()
        {
            try
            {
                _log.Info("Loading data.");

                _settings = ApplicationSettings.Current.SysproInvoiceSettings ??
                            new SysproInvoiceSettings();

                SetupPersistence();
                SetupExport();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error setting up the invoice export.");
            }
        }

        #endregion

        #region ExportData

        private class ExportData
        {
            public int ErrorCount { get; set; }

            public int ExportCount { get; set; }

            public List<ExportDocument> Documents { get; set; }
        }

        #endregion

        #region ExportDocument

        private class ExportDocument
        {
            public XDocument Document { get; set; }

            public string TransmissionReference { get; set; }

            public string FileName { get; set; }

            public List<int> ExportedOrderIds { get; set; }
        }

        #endregion
    }
}