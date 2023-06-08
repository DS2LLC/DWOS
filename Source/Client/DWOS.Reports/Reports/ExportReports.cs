using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using DWOS.Data;
using DWOS.Data.Datasets;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using System.Threading;

namespace DWOS.Reports
{
    public class ExportedInvoiceReport : ExcelBaseReport
    {
        #region Fields

        public const string COL_IMPORTED = "Imported";
        public const string COL_NOTES = "Issues";
        private readonly OrderInvoiceDataSet.OrderInvoiceDataTable _invoice;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Exported Invoices"; }
        }

        #endregion

        #region Methods

        public ExportedInvoiceReport(OrderInvoiceDataSet.OrderInvoiceDataTable invoice)
        {
            this._invoice = invoice; 
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            decimal totalPriceSummary = 0;
            int totalOrderCount = 0;

            SetupReportInfo();

            var wks = CreateWorksheet(Title);
            int rowIndex = this.AddHeaderRow(wks);
            
            foreach (OrderInvoiceDataSet.OrderInvoiceRow item in this._invoice)
            {
                if (!item.IsNull(COL_IMPORTED))
                {
                    ++totalOrderCount;
                    totalPriceSummary += AddOrderRow(item, wks, ref rowIndex);
                }
            }

            AddSummaryRow(totalPriceSummary, totalOrderCount, wks, ref rowIndex);
        }

        private decimal AddOrderRow(OrderInvoiceDataSet.OrderInvoiceRow orderInvoice, Worksheet wks, ref int rowIndex)
        {
            decimal price = 0;
            int colIndex = 0;

            try
            {
                string imported = orderInvoice.IsNull(COL_IMPORTED) ? "-" : (Convert.ToBoolean(orderInvoice[COL_IMPORTED]) ? "Yes" : "No");

                WorksheetCell cell;
                if (imported == "No")
                {
                    cell = CreateCell(wks, rowIndex, colIndex++, imported, horizontalAlignment: HorizontalCellAlignment.Center);
                    cell.CellFormat.Fill = CellFill.CreateSolidFill(System.Drawing.Color.Red);
                }
                else if (imported == "Yes")
                {
                    cell = CreateCell(wks, rowIndex, colIndex++, imported, horizontalAlignment: HorizontalCellAlignment.Center);
                    cell.CellFormat.Fill = CellFill.CreateSolidFill(System.Drawing.Color.LightGreen);
                }
                else
                {
                    cell = CreateCell(wks, rowIndex, colIndex++, imported, horizontalAlignment: HorizontalCellAlignment.Center);
                    cell.CellFormat.Fill = CellFill.CreateSolidFill(System.Drawing.Color.LightBlue);
                }

                //WO
                CreateCell(wks, rowIndex, colIndex++, orderInvoice.OrderID, horizontalAlignment: HorizontalCellAlignment.Left);

                //PO
                CreateCell(wks, rowIndex, colIndex++, orderInvoice.IsPurchaseOrderNull() ? "NA" : orderInvoice.PurchaseOrder, horizontalAlignment: HorizontalCellAlignment.Left);

                //DWOS Customer Name
                CreateCell(wks, rowIndex, colIndex++, orderInvoice.CustomerName, horizontalAlignment: HorizontalCellAlignment.Left);

                //QB Invoice
                CreateCell(wks, rowIndex, colIndex++, orderInvoice.IsInvoiceNull() ? "None" : orderInvoice.Invoice, horizontalAlignment: HorizontalCellAlignment.Left);

                //Created Date
                CreateCell(wks, rowIndex, colIndex++, orderInvoice.IsOrderDateNull() ? "NA" : orderInvoice.OrderDate.ToShortDateString(), horizontalAlignment: HorizontalCellAlignment.Left);

                //Required Date
                CreateCell(wks, rowIndex, colIndex++, orderInvoice.IsRequiredDateNull() ? "NA" : orderInvoice.RequiredDate.ToShortDateString(), horizontalAlignment: HorizontalCellAlignment.Left);

                //Completed Date
                CreateCell(wks, rowIndex, colIndex++, orderInvoice.IsCompletedDateNull() ? "NA" : orderInvoice.CompletedDate.ToShortDateString(), horizontalAlignment: HorizontalCellAlignment.Left);

                //Priority
                CreateCell(wks, rowIndex, colIndex++, orderInvoice.IsPriorityNull() ? "NA" : orderInvoice.Priority, horizontalAlignment: HorizontalCellAlignment.Center);

                //Total Price
                if (orderInvoice.IsBasePriceNull() || orderInvoice.IsPriceUnitNull() || orderInvoice.IsPartQuantityNull())
                {
                    CreateCell(wks, rowIndex, colIndex++, "UnKnown", horizontalAlignment: HorizontalCellAlignment.Left);
                    cell.CellFormat.Fill = CellFill.CreateSolidFill(System.Drawing.Color.Red);
                }
                else
                {
                    decimal weight = orderInvoice.IsWeightNull() ? 0M : orderInvoice.Weight;
                    decimal fees = OrderPrice.CalculateFees(orderInvoice, orderInvoice.BasePrice);
                    decimal tmpPrice = OrderPrice.CalculatePrice(orderInvoice.BasePrice, orderInvoice.PriceUnit, fees, orderInvoice.PartQuantity, weight);
                    cell = CreateCell(wks, rowIndex, colIndex++, tmpPrice, horizontalAlignment: HorizontalCellAlignment.Right, cellFormat: MONEY_FORMAT);
                    
                    if (imported == "Yes")
                        price = tmpPrice;
                    else
                        price = 0;
                }

                if (!orderInvoice.IsNull(COL_NOTES))
                {
                    string notes = orderInvoice[COL_NOTES].ToString();
                    CreateCell(wks, rowIndex, colIndex++, notes, horizontalAlignment: HorizontalCellAlignment.Left);
                    cell.CellFormat.Fill = CellFill.CreateSolidFill(System.Drawing.Color.Red);
                }
                else
                {
                    CreateCell(wks, rowIndex, colIndex++, string.Empty, horizontalAlignment: HorizontalCellAlignment.Left);
                }

                rowIndex++;

                return price;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding invoice row to report.");
                return 0;
            }
        }

        private void AddSummaryRow(decimal ordersPriceSumary, int orderCount, Worksheet wks, ref int rowIndex)
        {
            CreateCell(wks, rowIndex++, 0, "Total Orders: " + orderCount.ToString(), horizontalAlignment: HorizontalCellAlignment.Left);
            CreateCell(wks, rowIndex++, 0, "Total Successfully Imported: " + ordersPriceSumary.ToString(OrderPrice.CurrencyFormatString), horizontalAlignment: HorizontalCellAlignment.Left);
        }

        private int AddHeaderRow(Worksheet wks)
        {
            var invoiceHeader = ApplicationSettings.Current.InvoiceExportType == InvoiceType.Quickbooks
                ? "QB Invoice"
                : "Invoice";

            //Add the company info
            var rowIndex = base.AddCompanyHeaderRows(wks, 4, String.Empty) + 2;
            int colIndex = 0;

            CreateHeaderCell(wks, rowIndex, colIndex++, "Imported", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "WO", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "PO", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "DWOS Customer", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, invoiceHeader, 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Created Date", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Required Date", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Completed Date", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Priority", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Total Price", 20);
            CreateHeaderCell(wks, rowIndex, colIndex++, "Issues", 75);

            return ++rowIndex;
        }

        #endregion
    }

    public class ExportedCSVInvoiceReport : ExcelBaseReport
    {
        #region Fields

        public const string COL_EXPORTED = "Exported";
        public const string COL_ISSUES = "Issues";
        private List<InvoiceToken> _tokens;
        private List<InvoiceD> _invoices;

        #endregion

        #region Properties

        public override string Title
        {
            get { return "Exported Invoices"; }
        }

        #endregion

        #region Methods

        public ExportedCSVInvoiceReport(List<InvoiceD> invoices, List<InvoiceToken> tokens) 
        {
            this._invoices = invoices;
            this._tokens = tokens;
        }

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            decimal totalPriceSummary = 0;
            int totalExported = 0;

            SetupReportInfo();

            var wks = CreateWorksheet(Title);
            int rowIndex = this.AddHeader(wks);

            foreach (var invoice in this._invoices)
            {
                //Just those that were actually exported
                if (invoice.Exported)
                {
                    totalPriceSummary += AddInvoice(invoice, wks, ref rowIndex);
                    totalExported++;
                }
            }

            AddSummaryRow(totalExported, totalPriceSummary, wks, ref rowIndex);
        }

        private decimal AddInvoice(InvoiceD invoice, Worksheet wks, ref int rowIndex)
        {
            decimal price = 0;
            int colIndex = 0;

            try
            {
                //Need to do this in token order as specified by user in CSV export
                foreach (var lineItem in invoice.LineItems)
                {
                    colIndex = 0;
                    foreach (var token in _tokens)
                    {
                        if (!token.IsSelected)
                            continue;

                        switch (token.TokenName)
                        {
                            case InvoiceFieldMapper.enumTokens.CUSTOMERID:
                                CreateCell(wks, rowIndex, colIndex++, invoice.CustomerID, horizontalAlignment: HorizontalCellAlignment.Center);
                                break;
                            case InvoiceFieldMapper.enumTokens.CUSTOMERNAME:
                                CreateCell(wks, rowIndex, colIndex++, invoice.CustomerName, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.INVOICEID:
                                CreateCell(wks, rowIndex, colIndex++, invoice.InvoiceID, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.WO:
                                CreateCell(wks, rowIndex, colIndex++, invoice.WO, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.DATECREATED:
                                CreateCell(wks, rowIndex, colIndex++, invoice.DateCreated, horizontalAlignment: HorizontalCellAlignment.Left, cellFormat: DATE_FORMAT);
                                break;
                            case InvoiceFieldMapper.enumTokens.DATEDUE:
                                CreateCell(wks, rowIndex, colIndex++, invoice.DueDate, horizontalAlignment: HorizontalCellAlignment.Center, cellFormat: DATE_FORMAT);
                                break;
                            case InvoiceFieldMapper.enumTokens.SHIPDATE:
                                CreateCell(wks, rowIndex, colIndex++, invoice.ShipDate, horizontalAlignment: HorizontalCellAlignment.Left, cellFormat: DATE_FORMAT);
                                break;
                            case InvoiceFieldMapper.enumTokens.TERMS:
                                CreateCell(wks, rowIndex, colIndex++, invoice.Terms, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.PO:
                                CreateCell(wks, rowIndex, colIndex++, invoice.PO, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.ARACCOUNT:
                                CreateCell(wks, rowIndex, colIndex++, token.Value, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.SALESACCOUNT:
                                CreateCell(wks, rowIndex, colIndex++, lineItem.SalesAccount, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.LINEITEM:
                                CreateCell(wks, rowIndex, colIndex++, lineItem.LineItem, horizontalAlignment: HorizontalCellAlignment.Center, cellFormat: NUMBER_FORMAT);
                                break;
                            case InvoiceFieldMapper.enumTokens.ITEM:
                                CreateCell(wks, rowIndex, colIndex++, invoice.Item, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.DESCRIPTION:
                                CreateCell(wks, rowIndex, colIndex++, lineItem.Description, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.QUANTITY:
                                CreateCell(wks, rowIndex, colIndex++, invoice.Quantity, horizontalAlignment: HorizontalCellAlignment.Right, cellFormat: NUMBER_FORMAT);
                                break;
                            case InvoiceFieldMapper.enumTokens.UNIT:
                                CreateCell(wks, rowIndex, colIndex++, invoice.PriceUnit, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.UNITPRICE:
                                CreateCell(wks, rowIndex, colIndex++, lineItem.BasePrice, horizontalAlignment: HorizontalCellAlignment.Right, cellFormat: MONEY_FORMAT);
                                break;
                            case InvoiceFieldMapper.enumTokens.EXTPRICE:
                                CreateCell(wks, rowIndex, colIndex++, lineItem.ExtPrice, horizontalAlignment: HorizontalCellAlignment.Right, cellFormat: MONEY_FORMAT);
                                break;
                            case InvoiceFieldMapper.enumTokens.SHIPPING:
                                CreateCell(wks, rowIndex, colIndex++, invoice.Shipping, horizontalAlignment: HorizontalCellAlignment.Center);
                                break;
                            case InvoiceFieldMapper.enumTokens.TRACKINGNUMBER:
                                CreateCell(wks, rowIndex, colIndex++, invoice.TrackingNumber, horizontalAlignment: HorizontalCellAlignment.Center);
                                break;
                            case InvoiceFieldMapper.enumTokens.PARTNAME:
                                CreateCell(wks, rowIndex, colIndex++, invoice.PartName, horizontalAlignment: HorizontalCellAlignment.Center);
                                break;
                            case InvoiceFieldMapper.enumTokens.PARTDESC:
                                CreateCell(wks, rowIndex, colIndex++, invoice.PartDesc, horizontalAlignment: HorizontalCellAlignment.Center);
                                break;
                            case InvoiceFieldMapper.enumTokens.PROCESSES:
                                CreateCell(wks, rowIndex, colIndex++, invoice.Processes, horizontalAlignment: HorizontalCellAlignment.Center);
                                break;
                            case InvoiceFieldMapper.enumTokens.PROCESSALIASES:
                                CreateCell(wks, rowIndex, colIndex++, invoice.ProcessAliases, horizontalAlignment: HorizontalCellAlignment.Center);
                                break;
                            case InvoiceFieldMapper.enumTokens.PROCESSDESCRIPTION:
                                CreateCell(wks, rowIndex, colIndex++, invoice.ProcessDesc, horizontalAlignment: HorizontalCellAlignment.Center);
                                break;
                            default:
                                break;
                        }
                    }

                    CreateCell(wks, rowIndex, colIndex++, invoice.Issues, horizontalAlignment: HorizontalCellAlignment.Center);
                    rowIndex++;
                }

                //Add invoice fees
                price = invoice.LineItems.Sum(lineItem => Convert.ToDecimal(lineItem.ExtPrice.Replace("$", String.Empty)));
                price += this.AddInvoiceFee(invoice, wks, ref rowIndex);

                return price;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding invoice row to report.");
                return 0;
            }
        }

        private decimal AddInvoiceFee(InvoiceD invoice, Worksheet wks, ref int rowIndex)
        {
            decimal price = 0;
            int colIndex = 0;

            try
            {
                foreach (InvoiceFeeItem fee in invoice.FeeItems)
                {
                    //CreateCell(wks, rowIndex, 0, invoice.Exported.ToString(), horizontalAlignment: HorizontalCellAlignment.Center);

                    //Reset to first column
                    colIndex = 0;

                    //Need to do this in token order as specified by user in CSV export
                    foreach (var token in _tokens)
                    {
                        if (!token.IsSelected)
                            continue;

                        switch (token.TokenName)
                        {
                            case InvoiceFieldMapper.enumTokens.CUSTOMERID:
                                CreateCell(wks, rowIndex, colIndex++, invoice.CustomerID, horizontalAlignment: HorizontalCellAlignment.Center);
                                break;
                            case InvoiceFieldMapper.enumTokens.CUSTOMERNAME:
                                CreateCell(wks, rowIndex, colIndex++, invoice.CustomerName, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.INVOICEID:
                                CreateCell(wks, rowIndex, colIndex++, invoice.InvoiceID, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.WO:
                                CreateCell(wks, rowIndex, colIndex++, invoice.WO, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.DATECREATED:
                                CreateCell(wks, rowIndex, colIndex++, invoice.DateCreated, horizontalAlignment: HorizontalCellAlignment.Left, cellFormat: DATE_FORMAT);
                                break;
                            case InvoiceFieldMapper.enumTokens.DATEDUE:
                                CreateCell(wks, rowIndex, colIndex++, invoice.DueDate, horizontalAlignment: HorizontalCellAlignment.Center, cellFormat: DATE_FORMAT);
                                break;
                            case InvoiceFieldMapper.enumTokens.SHIPDATE:
                                CreateCell(wks, rowIndex, colIndex++, invoice.ShipDate, horizontalAlignment: HorizontalCellAlignment.Left, cellFormat: DATE_FORMAT);
                                break;
                            case InvoiceFieldMapper.enumTokens.TERMS:
                                CreateCell(wks, rowIndex, colIndex++, invoice.Terms, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.PO:
                                CreateCell(wks, rowIndex, colIndex++, invoice.PO, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.ARACCOUNT:
                                CreateCell(wks, rowIndex, colIndex++, token.Value, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.SALESACCOUNT:
                                CreateCell(wks, rowIndex, colIndex++, fee.SalesAccount, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.LINEITEM:
                                CreateCell(wks, rowIndex, colIndex++, fee.LineItemNum, horizontalAlignment: HorizontalCellAlignment.Center, cellFormat: NUMBER_FORMAT);
                                break;
                            case InvoiceFieldMapper.enumTokens.ITEM:
                                CreateCell(wks, rowIndex, colIndex++, fee.Item, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.DESCRIPTION:
                                CreateCell(wks, rowIndex, colIndex++, fee.Description, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.QUANTITY:
                                CreateCell(wks, rowIndex, colIndex++, fee.Quantity, horizontalAlignment: HorizontalCellAlignment.Right, cellFormat: NUMBER_FORMAT);
                                break;
                            case InvoiceFieldMapper.enumTokens.UNIT:
                                CreateCell(wks, rowIndex, colIndex++, fee.FeeType, horizontalAlignment: HorizontalCellAlignment.Left);
                                break;
                            case InvoiceFieldMapper.enumTokens.UNITPRICE:
                                CreateCell(wks, rowIndex, colIndex++, fee.Amount, horizontalAlignment: HorizontalCellAlignment.Right, cellFormat: MONEY_FORMAT);
                                break;
                            case InvoiceFieldMapper.enumTokens.EXTPRICE:
                                CreateCell(wks, rowIndex, colIndex++, fee.ExtAmount, horizontalAlignment: HorizontalCellAlignment.Right, cellFormat: MONEY_FORMAT);
                                break;
                            case InvoiceFieldMapper.enumTokens.SHIPPING:
                                CreateCell(wks, rowIndex, colIndex++, invoice.Shipping, horizontalAlignment: HorizontalCellAlignment.Center);
                                break;
                            case InvoiceFieldMapper.enumTokens.TRACKINGNUMBER:
                                CreateCell(wks, rowIndex, colIndex++, invoice.TrackingNumber, horizontalAlignment: HorizontalCellAlignment.Center);
                                break;
                            case InvoiceFieldMapper.enumTokens.PARTNAME:
                                CreateCell(wks, rowIndex, colIndex++, "", horizontalAlignment: HorizontalCellAlignment.Center);
                                break;
                            case InvoiceFieldMapper.enumTokens.PARTDESC:
                                CreateCell(wks, rowIndex, colIndex++, "", horizontalAlignment: HorizontalCellAlignment.Center);
                                break;
                            case InvoiceFieldMapper.enumTokens.PROCESSES:
                                CreateCell(wks, rowIndex, colIndex++, "", horizontalAlignment: HorizontalCellAlignment.Center);
                                break;
                            case InvoiceFieldMapper.enumTokens.PROCESSALIASES:
                                CreateCell(wks, rowIndex, colIndex++, "", horizontalAlignment: HorizontalCellAlignment.Center);
                                break;
                            case InvoiceFieldMapper.enumTokens.PROCESSDESCRIPTION:
                                CreateCell(wks, rowIndex, colIndex++, "", horizontalAlignment: HorizontalCellAlignment.Center);
                                break;
                            default:
                                break;
                        }
                    }

                    CreateCell(wks, rowIndex, colIndex++, invoice.Issues, horizontalAlignment: HorizontalCellAlignment.Center);
                    rowIndex++;
                    price += Convert.ToDecimal(fee.ExtAmount.Replace("$", String.Empty));
                }

                return price;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding invoice row to report.");
                return 0;
            }
        }

        private void AddSummaryRow(int orderCount, decimal totalPrice,  Worksheet wks, ref int rowIndex)
        {
            var cell = CreateCell(wks, ++rowIndex, 0, "Total Orders Exported: " + orderCount, horizontalAlignment: HorizontalCellAlignment.Left, borderStyle: CellBorderLineStyle.None);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            cell = CreateCell(wks, ++rowIndex, 0, "Total Price: " + String.Format("{0:C}", totalPrice), horizontalAlignment: HorizontalCellAlignment.Left, borderStyle: CellBorderLineStyle.None);
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
        }

        private int AddHeader(Worksheet wks)
        {
            //Add the company info
            var rowIndex = base.AddCompanyHeaderRows(wks, 4, String.Empty) + 2;
            int colIndex = 0;

            //CreateHeaderCell(wks, rowIndex, colIndex++, "Exported", 20);

            //Add the header based on the tokens specified by user
            foreach(var token in _tokens)
            {
                if(token.IsSelected)
                    CreateHeaderCell(wks, rowIndex, colIndex++, token.Header, 20);
            }

            CreateHeaderCell(wks, rowIndex, colIndex++, "Issues", 20);

            return ++rowIndex;
        }

        #endregion
    }
}