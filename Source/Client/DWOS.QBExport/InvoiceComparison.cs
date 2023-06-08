using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderInvoiceDataSetTableAdapters;
using NLog;
using nsoftware.InQB;
using System;
using System.ComponentModel;

namespace DWOS.QBExport
{
    /// <summary>
    /// Compares persisted orders against QuickBooks invoices.
    /// </summary>
    public class InvoiceComparison
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public event ProgressChangedEventHandler ProgessChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Compares invoices for orders completed in the given date range.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public OrderInvoiceDataSet.InvoiceCompareDataTable CompareInvoices(DateTime fromDate, DateTime toDate)
        {
            using(var ta = new InvoiceCompareTableAdapter())
            {
                fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
                toDate   = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

                OrderInvoiceDataSet.InvoiceCompareDataTable invoiceCompare = ta.GetData(fromDate, toDate);

                _log.Info("Loading data. Found {0} invoices.".FormatWith(invoiceCompare.Count));

                Objsearch search = null;
                try
                {
                    search = new Objsearch();
                    search.QueryType          = ObjsearchQueryTypes.qtInvoiceSearch;
                    search.QBConnectionString = ApplicationSettings.Current.QBConnectionString;
                    search.QBXMLVersion = Properties.Settings.Default.QBXMLVersion;
                    search.OpenQBConnection();

                    var invoice         = new Invoice();
                    double invoiceIndex = 0;
                    double invoiceCount = invoiceCompare.Rows.Count;

                    foreach(OrderInvoiceDataSet.InvoiceCompareRow invRow in invoiceCompare)
                    {
                        this.ProgessChanged?.Invoke(this, new ProgressChangedEventArgs(Convert.ToInt32((++invoiceIndex / invoiceCount) * 100), null));

                        if(!string.IsNullOrWhiteSpace(invRow.Invoice))
                        {
                            try
                            {
                                search.Reset();
                                search.QueryType = ObjsearchQueryTypes.qtInvoiceSearch;
                                search.SearchCriteria.RefNumber = invRow.Invoice;
                                search.Search();
                            }
                            catch(Exception exc)
                            {
                                _log.Debug(exc, "Error finding invoice {0}.".FormatWith(invRow.Invoice));
                                continue;
                            }

                            _log.Info("Finding invoice {0} and found {1} invoices.".FormatWith(invRow.Invoice, search.Results.Count));

                            foreach(ObjSearchResult t in search.Results)
                            {
                                invoice.Reset();
                                invoice.QBResponseAggregate = t.Aggregate;

                                //Determine Status
                                if(invoice.IsPaid)
                                    invRow.QBStatus = "Paid";
                                else if(invoice.IsPending)
                                    invRow.QBStatus = "Pending";
                                else if(invoice.IsToBePrinted)
                                    invRow.QBStatus = "Not Printed";
                                else
                                {
                                    string toBeEmaild = invoice.Config("IsToBeEmailed");
                                    if(!string.IsNullOrWhiteSpace(toBeEmaild) && (toBeEmaild == "1" || toBeEmaild.ToLower() == "true"))
                                        invRow.QBStatus = "To Be Emailed";
                                    else
                                        invRow.QBStatus = "Unknown";
                                }

                                //Determine QB Price
                                invRow.QBPrice = string.IsNullOrWhiteSpace(invoice.Subtotal) ? 0 : Convert.ToDouble(invoice.Subtotal);

                                //Determine DWOS Price
                                var weight = invRow.IsWeightNull() ? 0M : invRow.Weight;
                                invRow.TotalPrice = (double)OrderPrice.CalculatePrice(invRow.BasePrice, invRow.PriceUnit, 0M, invRow.PartQuantity, weight);

                                if(!invRow.IsFeesNull())
                                    invRow.TotalPrice = invRow.TotalPrice + (double)invRow.Fees;
                            }
                        }
                    }

                }
                finally
                {
                    search?.CloseQBConnection();
                    search?.Dispose();
                }

                return invoiceCompare;
            }
        }

        #endregion
    }
}
