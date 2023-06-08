using System;
using System.ComponentModel;
using DWOS.Data.Datasets;

namespace DWOS.Data.Invoice
{
    /// <summary>
    /// Defines invoice-related functionality.
    /// </summary>
    public interface IInvoiceExporter : IDisposable
    {
        /// <summary>
        /// Occurs when this instance exports an invoice.
        /// </summary>
        event ProgressChangedEventHandler ProgessChanged;

        /// <summary>
        /// Gets the dataset for this instance.
        /// </summary>
        OrderInvoiceDataSet OrderInvoices { get; }

        /// <summary>
        /// Loads order data into <see cref="OrderInvoices"/>.
        /// </summary>
        void LoadData();

        /// <summary>
        /// Exports selected invoices.
        /// </summary>
        /// <returns></returns>
        InvoiceResult Export();
    }
}
