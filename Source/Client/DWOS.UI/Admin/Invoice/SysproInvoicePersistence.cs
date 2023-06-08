using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using DWOS.Data;
using DWOS.Data.Invoice;
using DWOS.UI.Utilities;

namespace DWOS.UI.Admin
{
    internal class SysproInvoicePersistence : ISysproInvoicePersistence
    {
        public string GetFileName(string transmissionReference)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = @"Export Invoices to XML file",
                Filter = @"XML files (*.xml)|*.xml",
                InitialDirectory = UserSettings.Default.LastInvoiceDirectory,
                FileName = $"{transmissionReference}.xml"
            };

            string fileName = null;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog.FileName;

                UserSettings.Default.LastInvoiceDirectory = Path.GetDirectoryName(fileName);
                UserSettings.Default.Save();
            }

            return fileName;
        }

        public string GetDirectory()
        {
            var directoryDialog = new FolderBrowserDialog
            {
                SelectedPath = UserSettings.Default.LastInvoiceDirectory,
                Description = @"Select a folder for SYSPRO invoices"
            };

            string directory = null;
            if (directoryDialog.ShowDialog() == DialogResult.OK)
            {
                directory = directoryDialog.SelectedPath;
                UserSettings.Default.LastInvoiceDirectory = directory;
                UserSettings.Default.Save();
            }

            return directory;
        }

        public void Save(XDocument document, string fileName)
        {
            try
            {
                using (var file = File.Open(fileName, FileMode.CreateNew))
                {
                    document.Save(file);
                }
            }
            catch (IOException)
            {
                MessageBoxUtilities.ShowMessageBoxWarn("File already exists.",
                    "File Error",
                    fileName);

                throw;
            }
        }
    }
}