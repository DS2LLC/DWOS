using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace DWOS.Data
{
    /// <summary>
    /// Manages the field tokens and output for exporting DWOS invoices
    /// </summary>
    public class InvoiceFieldMapper
    {
        #region Fields

        private const string COMMA = ",";

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private List<InvoiceToken> _invoiceTokens = new List<InvoiceToken>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the invoice tokens for this instance.
        /// </summary>
        /// <remarks>
        /// The default value for this is generated from
        /// <see cref="ApplicationSettings.InvoiceExportTokens"/>.
        /// </remarks>
        public List<InvoiceToken> InvoiceTokens
        {
            get
            {
                if (_invoiceTokens == null)
                    _invoiceTokens = new List<InvoiceToken>();

                //sort by order 
                _invoiceTokens.Sort((t1, t2) => t1.Order.CompareTo(t2.Order));

                return _invoiceTokens;
            }

            set { _invoiceTokens = value; }
        }

        #endregion

        #region Methods

        public InvoiceFieldMapper()
        {
            this.ConstructFieldMapping();
        }

        private void ConstructFieldMapping()
        {
            try
            {
                //Get the field tokens which are comma separated and enclosed in <>
                string fieldTokens = ApplicationSettings.Current.InvoiceExportTokens;
                fieldTokens = fieldTokens.Replace(@"<", String.Empty);
                fieldTokens = fieldTokens.Replace(@">", String.Empty);
                string[] Tokens = fieldTokens.Split(new char[] { ',' });

                //Order is the sequence of tokens entered by user
                int order = 0;
                foreach (string fieldToken in Tokens)
                {
                    enumTokens tokenMatch;
                    string value = null;
                    string field = fieldToken;

                    //Check if token has literal value assigned
                    string[] keyvalue = fieldToken.Split(new char[] { ':' });
                    if (keyvalue.Length == 2)
                    {
                        field = keyvalue[0];
                        value = keyvalue[1];
                    }

                    bool match = Enum.TryParse<enumTokens>(field, out tokenMatch);
                    if (match)
                    {
                        var token = new InvoiceToken() { TokenName = tokenMatch, Value = value, IsSelected = true, Order = order++ };

                        switch (token.TokenName)
                        {
                            case enumTokens.CUSTOMERID:
                                token.Header = "Customer ID";
                                break;
                            case enumTokens.CUSTOMERNAME:
                                token.Header = "Customer Name";
                                break;
                            case enumTokens.INVOICEID:
                                token.Header = "Invoice ID";
                                break;
                            case enumTokens.WO:
                                token.Header = "Work Order";
                                break;
                            case enumTokens.DATECREATED:
                                token.Header = "Date Created";
                                break;
                            case enumTokens.DATEDUE:
                                token.Header = "Date Due";
                                break;
                            case enumTokens.SHIPDATE:
                                token.Header = "Ship Date";
                                break;
                            case enumTokens.TERMS:
                                token.Header = "Terms";
                                break;
                            case enumTokens.PO:
                                token.Header = "Purchase Order";
                                break;
                            case enumTokens.ARACCOUNT:
                                token.Header = "AR Account";
                                break;
                            case enumTokens.SALESACCOUNT:
                                token.Header = "Sales Account";
                                break;
                            case enumTokens.LINEITEM:
                                token.Header = "Line Item";
                                break;
                            case enumTokens.ITEM:
                                token.Header = "Item";
                                break;
                            case enumTokens.DESCRIPTION:
                                token.Header = "Description";
                                break;
                            case enumTokens.QUANTITY:
                                token.Header = "Quantity";
                                break;
                            case enumTokens.UNIT:
                                token.Header = "Unit";
                                break;
                            case enumTokens.UNITPRICE:
                                token.Header = "Unit Price";
                                break;
                            case enumTokens.EXTPRICE:
                                token.Header = "Ext Price";
                                break;
                            case enumTokens.SHIPPING:
                                token.Header = "Shipping";
                                break;
                            case enumTokens.TRACKINGNUMBER:
                                token.Header = "Tracking #";
                                break;
                            case enumTokens.PARTNAME:
                                token.Header = "Part Name";
                                break;
                            case enumTokens.PARTDESC:
                                token.Header = "Part Desc";
                                break;
                            case enumTokens.PROCESSES:
                                 token.Header = "Processes";
                                break;
                            case enumTokens.PROCESSALIASES:
                                token.Header = "Process Aliases";
                                break;
                            case enumTokens.PROCESSDESCRIPTION:
                                token.Header = "Process Descriptons";
                                break;
                            default:
                                break;
                        }

                        _invoiceTokens.Add(token);
                    }
                    else
                    {
                        _log.Info("No match found for field: " + fieldToken);
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error creating field mapping for export file.");
            }
        }

        /// <summary>
        /// Generates a CSV header.
        /// </summary>
        /// <returns></returns>
        public string ExportInvoiceHeader()
        {
            StringBuilder tokenizedHeader = new StringBuilder();
            for (int index = 0; index < this.InvoiceTokens.Count; index++)
            {
                tokenizedHeader.Append(this.InvoiceTokens[index].Header);
                if (index != this.InvoiceTokens.Count - 1)
                    tokenizedHeader.Append(COMMA);
            }

            return tokenizedHeader.ToString();
        }

        /// <summary>
        /// Generates CSV content from <see cref="InvoiceD.LineItems"/>.
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns>
        /// A collection of line item strings in CSV format
        /// </returns>
        public List<string> ExportInvoice(InvoiceD invoice)
        {
            List<string> lineItems = new List<string>();

            foreach (var lineItem in invoice.LineItems)
            {
                StringBuilder tokenizedInvoice = new StringBuilder();
                foreach (InvoiceToken token in this.InvoiceTokens)
                {
                    if (!token.IsSelected)
                        continue;

                    switch (token.TokenName)
                    {
                        case enumTokens.CUSTOMERID:
                            tokenizedInvoice.Append(invoice.CustomerID);
                            break;
                        case enumTokens.CUSTOMERNAME:
                            tokenizedInvoice.Append(invoice.CustomerName);
                            break;
                        case enumTokens.INVOICEID:
                            tokenizedInvoice.Append(invoice.InvoiceID);
                            break;
                        case enumTokens.WO:
                            tokenizedInvoice.Append(invoice.WO);
                            break;
                        case enumTokens.DATECREATED:
                            tokenizedInvoice.Append(invoice.DateCreated);
                            break;
                        case enumTokens.DATEDUE:
                            tokenizedInvoice.Append(invoice.DueDate);
                            break;
                        case enumTokens.SHIPDATE:
                            tokenizedInvoice.Append(invoice.ShipDate);
                            break;
                        case enumTokens.TERMS:
                            tokenizedInvoice.Append(invoice.Terms);
                            break;
                        case enumTokens.PO:
                            tokenizedInvoice.Append(invoice.PO);
                            break;
                        case enumTokens.ARACCOUNT:
                            tokenizedInvoice.Append(token.Value);
                            break;
                        case enumTokens.SALESACCOUNT:
                            tokenizedInvoice.Append(lineItem.SalesAccount);
                            break;
                        case enumTokens.LINEITEM:
                            tokenizedInvoice.Append(lineItem.LineItem);
                            break;
                        case enumTokens.ITEM:
                            tokenizedInvoice.Append(invoice.Item);
                            break;
                        case enumTokens.DESCRIPTION:
                            tokenizedInvoice.Append(lineItem.Description);
                            break;
                        case enumTokens.QUANTITY:
                            tokenizedInvoice.Append(invoice.Quantity);
                            break;
                        case enumTokens.UNIT:
                            tokenizedInvoice.Append(invoice.PriceUnit);
                            break;
                        case enumTokens.UNITPRICE:
                            tokenizedInvoice.Append(lineItem.BasePrice);
                            break;
                        case enumTokens.EXTPRICE:
                            tokenizedInvoice.Append(lineItem.ExtPrice);
                            break;
                        case enumTokens.SHIPPING:
                            tokenizedInvoice.Append(invoice.Shipping);
                            break;
                        case enumTokens.TRACKINGNUMBER:
                            tokenizedInvoice.Append(invoice.TrackingNumber);
                            break;
                        case enumTokens.PARTNAME:
                            tokenizedInvoice.Append(invoice.PartName);
                            break;
                        case enumTokens.PARTDESC:
                            tokenizedInvoice.Append(invoice.PartDesc);
                            break;
                        case enumTokens.PROCESSES:
                            tokenizedInvoice.Append(invoice.Processes);
                            break;
                        case enumTokens.PROCESSALIASES:
                            tokenizedInvoice.Append(invoice.ProcessAliases);
                            break;
                        case enumTokens.PROCESSDESCRIPTION:
                            tokenizedInvoice.Append(invoice.ProcessDesc);
                            break;
                        default:
                            break;
                    }

                    tokenizedInvoice.Append(COMMA);
                }

                //Remove the last comma
                if (tokenizedInvoice.Length > 0)
                    tokenizedInvoice = tokenizedInvoice.Remove(tokenizedInvoice.Length - 1, 1);

                lineItems.Add(tokenizedInvoice.ToString());
            }

            return lineItems;
        }

        /// <summary>
        /// Generates CSV content from <see cref="InvoiceD.FeeItems"/>.
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns>
        /// A collection of fee item strings in CSV format
        /// </returns>
        public List<string> ExportInvoiceFees(InvoiceD invoice)
        {
            List<string> fees = new List<string>();

            foreach (InvoiceFeeItem fee in invoice.FeeItems)
            {
                StringBuilder tokenizedfee = new StringBuilder();

                foreach (InvoiceToken token in this.InvoiceTokens)
                {
                    if (!token.IsSelected)
                        continue;

                    switch (token.TokenName)
                    {
                        case enumTokens.CUSTOMERID:
                            tokenizedfee.Append(invoice.CustomerID);
                            break;
                        case enumTokens.CUSTOMERNAME:
                            tokenizedfee.Append(invoice.CustomerName);
                            break;
                        case enumTokens.INVOICEID:
                            tokenizedfee.Append(invoice.InvoiceID);
                            break;
                        case enumTokens.WO:
                            tokenizedfee.Append(invoice.WO);
                            break;
                        case enumTokens.DATECREATED:
                            tokenizedfee.Append(invoice.DateCreated);
                            break;
                        case enumTokens.DATEDUE:
                            tokenizedfee.Append(invoice.DueDate);
                            break;
                        case enumTokens.SHIPDATE:
                            tokenizedfee.Append(invoice.ShipDate);
                            break;
                        case enumTokens.TERMS:
                            tokenizedfee.Append(invoice.Terms);
                            break;
                        case enumTokens.PO:
                            tokenizedfee.Append(invoice.PO);
                            break;
                        case enumTokens.ARACCOUNT:
                            tokenizedfee.Append(token.Value);
                            break;
                        case enumTokens.SALESACCOUNT:
                            tokenizedfee.Append(fee.SalesAccount);
                            break;
                        case enumTokens.LINEITEM:
                            tokenizedfee.Append(fee.LineItemNum);
                            break;
                        case enumTokens.ITEM:
                            tokenizedfee.Append(fee.Item);
                            break;
                        case enumTokens.DESCRIPTION:
                            tokenizedfee.Append(fee.Description);
                            break;
                        case enumTokens.QUANTITY:
                            tokenizedfee.Append(fee.Quantity);
                            break;
                        case enumTokens.UNIT:
                            tokenizedfee.Append(fee.FeeType);
                            break;
                        case enumTokens.UNITPRICE:
                            tokenizedfee.Append(fee.Amount);
                            break;
                        case enumTokens.EXTPRICE:
                            tokenizedfee.Append(fee.ExtAmount);
                            break;
                        case enumTokens.SHIPPING:
                            tokenizedfee.Append("");
                            break;
                        case enumTokens.TRACKINGNUMBER:
                            tokenizedfee.Append("");
                            break;
                        case enumTokens.PARTNAME:
                            tokenizedfee.Append("");
                            break;
                        case enumTokens.PARTDESC:
                            tokenizedfee.Append("");
                            break;
                        case enumTokens.PROCESSES:
                            tokenizedfee.Append("");
                            break;
                        case enumTokens.PROCESSALIASES:
                            tokenizedfee.Append("");
                            break;
                        case enumTokens.PROCESSDESCRIPTION:
                            tokenizedfee.Append("");
                            break;
                        default:
                            break;
                    }

                    tokenizedfee.Append(COMMA);
                }

                //Remove the last comma
                if (tokenizedfee.Length > 0)
                    tokenizedfee = tokenizedfee.Remove(tokenizedfee.Length - 1, 1);

                fees.Add(tokenizedfee.ToString());
            }

            return fees;
        }

        #endregion

        #region enumTokens

        /// <summary>
        /// Represents a CSV token.
        /// </summary>
        public enum enumTokens
        {
            /// <summary>
            /// Customer ID
            /// </summary>
            CUSTOMERID,

            /// <summary>
            /// Customer Name
            /// </summary>
            CUSTOMERNAME,

            /// <summary>
            /// Invoice ID
            /// </summary>
            INVOICEID,

            /// <summary>
            /// Work Order
            /// </summary>
            WO,

            /// <summary>
            /// Date Created
            /// </summary>
            DATECREATED,

            /// <summary>
            /// Date Due
            /// </summary>
            DATEDUE,

            /// <summary>
            /// Ship Date
            /// </summary>
            SHIPDATE,

            /// <summary>
            /// Terms
            /// </summary>
            TERMS,

            /// <summary>
            /// Purchase Account
            /// </summary>
            PO,

            /// <summary>
            /// AR Account
            /// </summary>
            ARACCOUNT,

            /// <summary>
            /// Sales Account
            /// </summary>
            SALESACCOUNT,

            /// <summary>
            /// Line Item
            /// </summary>
            LINEITEM,

            /// <summary>
            /// Item
            /// </summary>
            ITEM,

            /// <summary>
            /// Description
            /// </summary>
            DESCRIPTION,

            /// <summary>
            /// Quantity
            /// </summary>
            QUANTITY,

            /// <summary>
            /// Unit
            /// </summary>
            UNIT,

            /// <summary>
            /// Unit Price
            /// </summary>
            UNITPRICE,

            /// <summary>
            /// Ext. Price
            /// </summary>
            EXTPRICE,

            /// <summary>
            /// Shipping
            /// </summary>
            SHIPPING,

            /// <summary>
            /// Part Name
            /// </summary>
            TRACKINGNUMBER,

            /// <summary>
            /// Tracking Number
            /// </summary>
            PARTNAME,

            /// <summary>
            /// Part Description
            /// </summary>
            PARTDESC,

            /// <summary>
            /// Order Status
            /// </summary>

            FEES,

            /// <summary>
            /// Processes
            /// </summary>
            PROCESSES,

            /// <summary>
            /// Process Aliases
            /// </summary>
            PROCESSALIASES,

            /// <summary>
            /// Process Description
            /// </summary>
            PROCESSDESCRIPTION,


        };

        #endregion
    }
}
