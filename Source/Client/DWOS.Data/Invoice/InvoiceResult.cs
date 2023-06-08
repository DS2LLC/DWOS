using System.Collections.Generic;

namespace DWOS.Data.Invoice
{
    /// <summary>
    /// Represents the results of invoice export.
    /// </summary>
    public sealed class InvoiceResult
    {
        /// <summary>
        /// Gets or sets the number of exported invoices.
        /// </summary>
        public int ExportedCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of invoices that were unsuccessfully exported.
        /// </summary>
        public int ErrorCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total number of invoices.
        /// </summary>
        public int TotalCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of exported work orders.
        /// </summary>
        public List<int> ExportedOrderIDs
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating cancellation.
        /// </summary>
        public bool Cancelled
        {
            get;
            set;
        }
    }
}
