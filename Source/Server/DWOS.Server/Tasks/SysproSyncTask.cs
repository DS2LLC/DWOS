using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Xml.Linq;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderInvoiceDataSetTableAdapters;
using DWOS.Data.Invoice;
using Quartz;
using NLog;
using System.Threading.Tasks;

namespace DWOS.Server.Tasks
{
    [DisallowConcurrentExecution]
    internal class SysproSyncTask : IJob
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly XmlSerializer _errorSerializer = new XmlSerializer(typeof(SysproSyncError));

        #endregion

        #region Methods

        private void BeginProcessing()
        {
            _log.Info($"BEGIN: {nameof(SysproSyncTask)}");

            try
            {
                if (!ServerSettings.Default.SysproSettings.Enabled)
                {
                    return;
                }

                Sync();

                // Automated invoicing after sync
                // Otherwise, the sync would try to process the pending invoices
                if (ServerSettings.Default.SysproSettings.AutomaticInvoicing)
                {
                    AutomaticInvoice();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error running SYSPRO sync.");
            }

            _log.Info($"END: {nameof(SysproSyncTask)}");
        }

        private void Sync()
        {
            const string fileSearchPattern = "*.xml";
            OrderInvoiceDataSet.SysproInvoiceDataTable dtSyspro = null;
            SysproInvoiceTableAdapter taSyspro = null;

            try
            {
                dtSyspro = new OrderInvoiceDataSet.SysproInvoiceDataTable();
                taSyspro = new SysproInvoiceTableAdapter();

                taSyspro.FillByStatus(dtSyspro, SysproInvoiceStatus.Pending.ToString());

                var sysproSettings = ServerSettings.Default.SysproSettings;

                if (!Directory.Exists(sysproSettings.SaveDirectory))
                {
                    _log.Warn("SYSPRO save directory does not exist - skipping sync.");
                    return;
                }

                if (!Directory.Exists(sysproSettings.ErrorDirectory))
                {
                    _log.Warn("SYSPRO error directory does not exist - skipping sync.");
                    return;
                }

                // Failed invoice check
                var errorFiles = Directory.EnumerateFiles(sysproSettings.ErrorDirectory, fileSearchPattern).ToList();

                foreach (var errorFile in errorFiles)
                {
                    SysproSyncError errorData;
                    using (var errorFileStream = File.OpenRead(errorFile))
                    {
                        errorData = (SysproSyncError)_errorSerializer.Deserialize(errorFileStream);
                    }

                    var invoice = dtSyspro.FirstOrDefault(i => i.TransmissionReference == errorData.TransmissionReference);

                    if (invoice != null)
                    {
                        invoice.Status = SysproInvoiceStatus.Failed.ToString();
                        invoice.Message = errorData.ErrorMessage;
                    }

                    File.Delete(errorFile);
                }

                // Succeeded invoice check
                var invoiceFiles =
                    new HashSet<string>(
                        Directory.EnumerateFiles(sysproSettings.SaveDirectory, fileSearchPattern).Select(Path.GetFileName));

                var pendingTransactions = dtSyspro.Where(i => i.Status == SysproInvoiceStatus.Pending.ToString()).ToList();

                foreach (var pendingTransaction in pendingTransactions)
                {
                    if (invoiceFiles.Contains(pendingTransaction.FileName))
                    {
                        continue;
                    }

                    pendingTransaction.Status = SysproInvoiceStatus.Successful.ToString();
                }

                // Save changes
                taSyspro.Update(dtSyspro);
            }
            finally
            {
                dtSyspro?.Dispose();
                taSyspro?.Dispose();
            }
        }

        private void AutomaticInvoice()
        {
            var saveDirectory = ServerSettings.Default.SysproSettings.SaveDirectory;

            if (!Directory.Exists(saveDirectory))
            {
                _log.Warn("SYSPRO save directory does not exist.");
                return;
            }

            var export = new ExportSyspro(new AutoSysproInvoicePersistence(saveDirectory));
            export.LoadData();

            // Remove orders that should not be invoiced
            var filterExpression = ApplicationSettings.Current.InvoiceCheckTotal
                ? "TotalPrice <= 0"
                : "BasePrice <= 0";

            var rowsToRemove = export.OrderInvoices.OrderInvoice.Select(filterExpression);
            if (rowsToRemove.Length > 0)
            {
                foreach (var dataRow in rowsToRemove)
                {
                    dataRow.Delete();
                }

                export.OrderInvoices.OrderInvoice.AcceptChanges();
            }

            var rowsToExport = export.OrderInvoices.OrderInvoice.Select("Export = 1");
            if (rowsToExport.Length > 0)
            {
                export.Export();
            }
        }

        #endregion

        #region IJob Members

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await Task.Run(() => BeginProcessing()).ConfigureAwait(false);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error syncing SYSPRO data.");
            }
        }

        #endregion

        #region AutoSysproInvoicePersistence

        /// <summary>
        /// Implementation that does not show any dialogs.
        /// </summary>
        internal class AutoSysproInvoicePersistence : ISysproInvoicePersistence
        {
            public string SaveDirectory { get; }

            public AutoSysproInvoicePersistence(string saveDirectory)
            {
                SaveDirectory = saveDirectory;
            }

            public string GetFileName(string transmissionReference)
            {
                return Path.Combine(SaveDirectory, $"{transmissionReference}.xml");
            }

            public string GetDirectory() => SaveDirectory;

            public void Save(XDocument document, string fileName)
            {
                using (var file = File.Open(fileName, FileMode.CreateNew))
                {
                    document.Save(file);
                }
            }
        }

        #endregion
    }
}