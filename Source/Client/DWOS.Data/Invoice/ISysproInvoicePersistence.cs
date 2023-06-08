using System.Xml.Linq;

namespace DWOS.Data.Invoice
{
    /// <summary>
    /// Defines SYSPRO-related methods.
    /// </summary>
    public interface ISysproInvoicePersistence
    {
        /// <summary>
        /// Retrieves a file name using a transmission reference as a default.
        /// </summary>
        /// <param name="transmissionReference"></param>
        /// <returns></returns>
        string GetFileName(string transmissionReference);

        /// <summary>
        /// Retrieves a destination directory for SYSPRO invoices.
        /// </summary>
        /// <returns></returns>
        string GetDirectory();

        /// <summary>
        /// Saves the XML document to the file.
        /// </summary>
        /// <remarks>
        /// Implementers can show user-facing error messages here without
        /// having to implement them in <see cref="ExportSyspro"/>.
        /// </remarks>
        /// <param name="document"></param>
        /// <param name="fileName"></param>
        void Save(XDocument document, string fileName);
    }
}
