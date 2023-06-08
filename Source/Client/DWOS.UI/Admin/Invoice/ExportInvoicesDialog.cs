using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DWOS.QBExport;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.Data.Invoice;
using DWOS.Data;
using DWOS.UI.Admin.Quickbook;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using NLog;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace DWOS.UI.Admin
{
    public partial class ExportInvoicesDialog: Form
    {
        #region Fields

        private const int PANEL_PROGRESS_BAR = 0;
        private const int PANEL_STATUS = 1;
        private const int PANEL_TOTAL = 2;

        private IInvoiceExporter _export;

        private bool _runningTask;

        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("ExportInvoices", new UltraGridBandSettings());

        #endregion

        #region Properties

        private static string DefaultStatusMessage
        {
            get
            {
                var updateIntervalMinutes = ApplicationSettings.Current.InvoiceIntervalMinutes;
                string msg;

                if (updateIntervalMinutes.HasValue)
                {
                    msg = updateIntervalMinutes > 1
                        ? $"Server automatically invoices orders every {updateIntervalMinutes} minutes."
                        : "Server automatically invoices orders every minute.";
                }
                else
                {
                    msg = string.Empty;
                }

                return msg;
            }
        }

        private int SelectedOrderCount
        {
            get
            {
                grdExport.UpdateData();

                return grdExport.Rows
                    .Count(row => !row.IsFilteredOut && ((row.Cells["Export"].Value as bool?) ?? false));
            }
        }

        #endregion

        #region Methods

        public ExportInvoicesDialog()
        {
            this.InitializeComponent();
        }

        private void InitialLoad()
        {
            try
            {
                this.ultraToolbarsManager1.Enabled = false;
                UpdateStatus(DefaultStatusMessage);

                //Quickbooks or CSV based on settings
                this.Text = "Export Invoices to " + ApplicationSettings.Current.InvoiceExportType.ToString();
                string invoiceText = string.Empty;
                if (ApplicationSettings.Current.InvoiceExportType == InvoiceType.Quickbooks)
                {
                    this.ultraToolbarsManager1.Tools["Export"].SharedProps.ToolTipTextFormatted = "Export invoices to " + ApplicationSettings.Current.InvoiceExportType.ToString();
                    this.ultraToolbarsManager1.Tools["Print"].SharedProps.Enabled = true;
                    this.ultraToolbarsManager1.Tools["InvoiceStatusReport"].SharedProps.Enabled = true;

                    if (ApplicationSettings.Current.InvoiceExportMaxErrors <= 0)
                        ApplicationSettings.Current.InvoiceExportMaxErrors = 1; // Should be at least 1, otherwise export will quit after the first order because the error count = max error count (0 = 0)

                    _export = new ExportInvoicesN(ApplicationSettings.Current.QuickBooksInvoiceMidpointRounding);

                }
                else if (ApplicationSettings.Current.InvoiceExportType == InvoiceType.CSV)
                {
                    this.ultraToolbarsManager1.Tools["Export"].SharedProps.ToolTipTextFormatted = "Export invoices to " + ApplicationSettings.Current.InvoiceExportType.ToString() + " file";
                    this.ultraToolbarsManager1.Tools["Print"].SharedProps.Enabled = false;
                    this.ultraToolbarsManager1.Tools["InvoiceStatusReport"].SharedProps.Enabled = false;

                    _export = new ExportCSV();
                }
                else if (ApplicationSettings.Current.InvoiceExportType == InvoiceType.Syspro)
                {
                    this.ultraToolbarsManager1.Tools["Export"].SharedProps.ToolTipTextFormatted = "Export invoices to SYSPRO (XML) file";
                    this.ultraToolbarsManager1.Tools["Print"].SharedProps.Enabled = false;
                    this.ultraToolbarsManager1.Tools["InvoiceStatusReport"].SharedProps.Enabled = false;

                    _export = new ExportSyspro(new SysproInvoicePersistence());
                }

                if (_export != null)
                {
                    _export.LoadData();
                    _export.ProgessChanged += this.Export_ProgessChanged;

                    // Remove orders that should not be invoiced
                    var filterExpression = ApplicationSettings.Current.InvoiceCheckTotal
                        ? "TotalPrice <= 0"
                        : "BasePrice <= 0";

                    var rowsToRemove = this._export.OrderInvoices.OrderInvoice.Select(filterExpression);
                    if (rowsToRemove.Length > 0)
                    {
                        foreach (var dataRow in rowsToRemove)
                        {
                            dataRow["Invoice"] = "ZERO";
                            dataRow["Export"] = false;
                            dataRow.Delete();
                        }
                        this._export.OrderInvoices.OrderInvoice.AcceptChanges();
                    }

                    //Load grid
                    this.grdExport.DataMember = null;
                    this.grdExport.DataSource = _export.OrderInvoices.OrderInvoice;
                    invoiceText = _export.OrderInvoices.OrderInvoice.Count.ToString() + " Invoices";
                }

                this.statusBar.Panels[PANEL_TOTAL].Text = invoiceText;
                grdExport.Update();
            }
            finally
            {
                this.ultraToolbarsManager1.Enabled = true;
            }
        }

        private void ExportInvoices()
        {
            try
            {
                using(new UsingWaitCursor(this))
                {
                    LockUI();

                    // Filtered-out rows should not be exported.
                    foreach (var filteredRow in grdExport.Rows.Where(r => r.IsFilteredOut))
                    {
                        filteredRow.Cells["Export"].Value = false;
                    }

                    UpdateProgressBar(0);
                    UpdateStatus("Exporting Invoices...");

                    //Force an update to ensure checked state for export is updated for all rows
                    this.grdExport.UpdateData();

                    //Export to Quickbooks or CSV based on settings
                    if (ApplicationSettings.Current.InvoiceExportType == InvoiceType.Quickbooks)
                    {
                        // QB Export can take a long time
                        var exportTask = Task<InvoiceResult>.Factory.StartNew(_export.Export);
                        exportTask.ContinueWith((originalTask) =>
                        {
                            if (originalTask.IsFaulted)
                            {
                                string errorMsg = "Error exporting invoices.";
                                foreach (var exc in originalTask.Exception.InnerExceptions)
                                {
                                    bool logError = !(exc is ExportConnectionException);
                                    ErrorMessageBox.ShowDialog(errorMsg, exc, logError);

                                    if (!logError)
                                    {
                                        LogManager.GetCurrentClassLogger().Warn(exc, errorMsg);
                                    }
                                }

                                UpdateStatus(errorMsg);
                            }
                            else
                            {
                                FinishExport(originalTask.Result);
                            }

                            UnlockUI();
                        }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

                        timerExport.Start();
                    }
                    else
                    {
                        var result = _export.Export();
                        FinishExport(result);
                        UnlockUI();
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error exporting invoices.";
                bool logError = !(exc is ExportConnectionException);
                ErrorMessageBox.ShowDialog(errorMsg, exc, logError);

                if (!logError)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, errorMsg);
                }

                UnlockUI();
            }
        }

        private void LockUI()
        {
            _runningTask = true;
            ultraToolbarsManager1.Tools["Export"].SharedProps.Enabled = false;
            ultraToolbarsManager1.Tools["Print"].SharedProps.Enabled = false;
            ultraToolbarsManager1.Tools["InvoiceStatusReport"].SharedProps.Enabled = false;
        }

        private void UnlockUI()
        {
            _runningTask = false;
            ultraToolbarsManager1.Tools["Export"].SharedProps.Enabled = true;
            ultraToolbarsManager1.Tools["Print"].SharedProps.Enabled = true;
            ultraToolbarsManager1.Tools["InvoiceStatusReport"].SharedProps.Enabled = true;
        }

        private void FinishExport(InvoiceResult exportResult)
        {
            try
            {
                timerExport.Stop();

                if (exportResult == null)
                {
                    return;
                }

                UpdateStatus(string.Format("Exported {0} of {1} Invoices", exportResult.ExportedCount, exportResult.TotalCount));
                UpdateProgressBar(this.statusBar.Panels[PANEL_PROGRESS_BAR].ProgressBarInfo.Maximum);

                if (!exportResult.Cancelled)
                {
                    this.PrintInvoiceReport();
                }

                //Show any issues encountered
                if (exportResult.ErrorCount > 0)
                {
                    grdExport.DisplayLayout.Bands[0].Columns["Issues"].Hidden = false;
                }

                //Hide the rows that export successfully
                var exportedOrderIDs = new List<int>(exportResult.ExportedOrderIDs ?? Enumerable.Empty<int>());
                foreach (var row in grdExport.Rows)
                {
                    int rowOrderID = Convert.ToInt32(row.Cells["OrderID"].Value);

                    if (exportedOrderIDs.Contains(rowOrderID))
                    {
                        row.Cells["Export"].Value = false;
                        row.Hidden = true;
                    }
                }

                this.ultraToolbarsManager1.Tools["Print"].SharedProps.Enabled = true;

                if (this.grdExport.Rows.Count > 0)
                {
                    this.ultraToolbarsManager1.Tools["Export"].SharedProps.Enabled = true;
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error finishing invoice export.";
                ErrorMessageBox.ShowDialog(errorMsg, exc, true);
            }
        }

        private void PrintInvoiceReport()
        {
            try
            {
                GetInvoiceReport()?.DisplayReport();
            }
            catch (Exception exc)
            {
                string errorMsg = "Error creating exported invoice report.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private IReport GetInvoiceReport()
        {
            IReport report = null;

            if (ApplicationSettings.Current.InvoiceExportType == InvoiceType.CSV)
            {
                var exportCSV = _export as ExportCSV;
                if (exportCSV?.ExportedInvoices != null && exportCSV?.FieldMapper?.InvoiceTokens != null)
                {
                    report = new ExportedCSVInvoiceReport(exportCSV.ExportedInvoices, exportCSV.FieldMapper.InvoiceTokens);
                }
            }
            else if (_export?.OrderInvoices?.OrderInvoice != null)
            {
                report = new ExportedInvoiceReport(_export.OrderInvoices.OrderInvoice);
            }

            return report;
        }

        private void PrintInvoiceCompare()
        {
            try
            {
                if (ApplicationSettings.Current.InvoiceExportType != InvoiceType.Quickbooks)
                {
                    return;
                }

                using(new UsingWaitCursor(this))
                {
                    using(var compareDialog = new InvoiceCompareOptions())
                    {
                        if(compareDialog.ShowDialog(this) == DialogResult.OK)
                        {
                            var fromDate = compareDialog.dteFromDate.DateTime;
                            var toDate = compareDialog.dteToDate.DateTime;

                            UpdateProgressBar(0);

                            // Invoice comparison can take a long time
                            LockUI();

                            var reportTask = Task<SimpleExcelReport>.Factory
                                .StartNew(() => CreateQBComparisonReport(fromDate, toDate));

                            reportTask.ContinueWith((originalTask) =>
                            {
                                if (originalTask.IsFaulted)
                                {
                                    foreach (var exc in originalTask.Exception.InnerExceptions)
                                    {
                                        ErrorMessageBox.ShowDialog("Error displaying invoice compare data.", exc);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        originalTask.Result.DisplayReport();
                                    }
                                    catch (Exception exc)
                                    {
                                        ErrorMessageBox.ShowDialog("Error displaying invoice compare data.", exc);
                                    }
                                }

                                UnlockUI();
                            }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error displaying invoice compare data.", exc);
                UnlockUI();
            }
        }

        private SimpleExcelReport CreateQBComparisonReport(DateTime fromDate, DateTime toDate)
        {
            var compare = new InvoiceComparison();
            compare.ProgessChanged += this.Export_ProgessChanged;

            var invCompare = compare.CompareInvoices(fromDate, toDate);

            var excel = new SimpleExcelReport("Invoices", invCompare);
            excel.GetColumnFormat = (col) =>
            {
                switch(col.ColumnName)
                {
                    case "QBPrice":
                    case "TotalPrice":
                    case "BasePrice":
                    case "Fees":
                        return ExcelBaseReport.MONEY_FORMAT;
                    case "OrderDate":
                    case "CompletedDate":
                        return ExcelBaseReport.DATE_FORMAT;
                }

                return "";
            };

            excel.GetColumnHidden = (col) =>
            {
                switch(col.ColumnName)
                {
                    case "BasePrice":
                    case "PriceUnit":
                    case "Fees":
                        return true;
                }

                return false;
            };

            return excel;
        }
        
        private Infragistics.Win.Appearance GetCellApperance(string colorName)
        {
            if(!this.grdExport.DisplayLayout.Appearances.Exists(colorName))
            {
                var app = this.grdExport.DisplayLayout.Appearances.Add(colorName);
                app.ForeColor = Color.FromName(colorName);
                return app;
            }

            return this.grdExport.DisplayLayout.Appearances[colorName];
        }

        private void UpdateStatus(string statusMsg)
        {
            try
            {
                statusBar.Panels[PANEL_STATUS].Text = statusMsg;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on status changed.");
            }
        }

        private void UpdateProgressBar(int progressPercentage)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke((Action<int>)UpdateProgressBar, progressPercentage);
                    return;
                }
                else if (IsDisposed || statusBar == null || statusBar.IsDisposed)
                {
                    return;
                }

                var progressBarInfo = statusBar.Panels[PANEL_PROGRESS_BAR].ProgressBarInfo;
                var minimum = progressBarInfo.Minimum;
                var maximum = progressBarInfo.Maximum;

                if (progressPercentage >= minimum && progressPercentage <= maximum)
                {
                    progressBarInfo.Value = progressPercentage;
                }

                statusBar.Refresh();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on progress changed.");
            }
        }

        private void DisposeMe()
        {
            if (_export != null)
            {
                _export.ProgessChanged -= Export_ProgessChanged;
                _export.Dispose();
                _export = null;
            }
        }

        #endregion

        #region Events

        private void Export_ProgessChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgressBar(e.ProgressPercentage);
        }

        private void ExportToQuickbooks_Load(object sender, EventArgs e)
        {
            try
            {
                this.InitialLoad();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error loading invoices to be exported to Quickbooks.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void ultraToolbarsManager1_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch(e.Tool.Key)
            {
                case "Export": // ButtonTool
                    string msg;
                    var selectedCount = SelectedOrderCount;

                    if (selectedCount == 1)
                    {
                        msg = "Would you like to export 1 invoice?";
                    }
                    else
                    {
                        msg = string.Format("Would you like to export {0} invoices?", selectedCount);
                    }

                    if (MessageBoxUtilities.ShowMessageBoxYesOrNo(msg, "Export Invoices") == DialogResult.Yes)
                    {
                        ExportInvoices();
                    }

                    break;

                case "Print": // ButtonTool
                    this.PrintInvoiceReport();
                    break;
                case "InvoiceStatusReport": // ButtonTool
                    this.PrintInvoiceCompare();
                    break;
            }
        }

        private void grdExport_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                if(!e.ReInitialize && e.Row.IsDataRow)
                {
                    if (e.Row.Band.Index == 0)
                    {
                        var orderId = Convert.ToInt32(e.Row.Cells["OrderID"].Value);

                        var partQty = 0;

                        if (e.Row.Cells["PartQuantity"].Value != DBNull.Value)
                        {
                            partQty = Convert.ToInt32(e.Row.Cells["PartQuantity"].Value);
                        }

                        var cocPartQty = this.taOrderInvoice.GetCOCPartQuantity(orderId);

                        if (partQty != cocPartQty.GetValueOrDefault())
                        {
                            e.Row.Cells["PartQuantity"].Appearance = this.GetCellApperance("Red");
                            e.Row.Cells["Issues"].Value = "COC Quantity is set at " + cocPartQty.GetValueOrDefault();
                        }

                        e.Row.Cells["Export"].IgnoreRowColActivation = true;
                        e.Row.Cells["Invoice"].IgnoreRowColActivation = true;
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting appearance of cell.");
            }
        }

        private void grdExport_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            const string currencyFormat = "$#,##0.00";

            try
            {
                grdExport.AfterColPosChanged -= grdExport_AfterColPosChanged;
                grdExport.AfterSortChange -= grdExport_AfterSortChange;

                //Hide the edit icon on left of row in grid
                e.Layout.RowSelectorImages.DataChangedImage = null;
                e.Layout.RowSelectorImages.ActiveAndDataChangedImage = null;

                //Hide all the columns
                foreach (var col in grdExport.DisplayLayout.Bands[0].Columns)
                    col.Hidden = true;

                grdExport.DisplayLayout.Bands[0].Columns["Export"].Header.CheckBoxAlignment = HeaderCheckBoxAlignment.Center;
                grdExport.DisplayLayout.Bands[0].Columns["Export"].Header.CheckBoxSynchronization = HeaderCheckBoxSynchronization.RowsCollection;
                grdExport.DisplayLayout.Bands[0].Columns["Export"].Header.CheckBoxVisibility = HeaderCheckBoxVisibility.WhenUsingCheckEditor;

                //Just the columns we need, set the order and add user friendly column names
                grdExport.DisplayLayout.Bands[0].Columns["Export"].Header.VisiblePosition = 0;
                grdExport.DisplayLayout.Bands[0].Columns["Export"].CellActivation = Activation.AllowEdit;
                grdExport.DisplayLayout.Bands[0].Columns["Export"].Hidden = false;

                grdExport.DisplayLayout.Bands[0].Columns["Invoice"].Header.VisiblePosition = 1;
                grdExport.DisplayLayout.Bands[0].Columns["Invoice"].Hidden = false;

                grdExport.DisplayLayout.Bands[0].Columns["SalesOrderID"].Header.VisiblePosition = 2;
                grdExport.DisplayLayout.Bands[0].Columns["SalesOrderID"].CellActivation = Activation.NoEdit;
                grdExport.DisplayLayout.Bands[0].Columns["SalesOrderID"].Hidden = false;
                grdExport.DisplayLayout.Bands[0].Columns["SalesOrderID"].Header.Caption = "Sales Order";

                grdExport.DisplayLayout.Bands[0].Columns["OrderID"].Header.VisiblePosition = 3;
                grdExport.DisplayLayout.Bands[0].Columns["OrderID"].CellActivation = Activation.NoEdit;
                grdExport.DisplayLayout.Bands[0].Columns["OrderID"].Hidden = false;
                grdExport.DisplayLayout.Bands[0].Columns["OrderID"].Header.Caption = "WO";

                grdExport.DisplayLayout.Bands[0].Columns["CustomerName"].Header.VisiblePosition = 4;
                grdExport.DisplayLayout.Bands[0].Columns["CustomerName"].CellActivation = Activation.NoEdit;
                grdExport.DisplayLayout.Bands[0].Columns["CustomerName"].Hidden = false;
                grdExport.DisplayLayout.Bands[0].Columns["CustomerName"].Header.Caption = "Customer";

                grdExport.DisplayLayout.Bands[0].Columns["PurchaseOrder"].Header.VisiblePosition = 5;
                grdExport.DisplayLayout.Bands[0].Columns["PurchaseOrder"].CellActivation = Activation.NoEdit;
                grdExport.DisplayLayout.Bands[0].Columns["PurchaseOrder"].Hidden = false;
                grdExport.DisplayLayout.Bands[0].Columns["PurchaseOrder"].Header.Caption = "PO";

                grdExport.DisplayLayout.Bands[0].Columns["CustomerWO"].Header.VisiblePosition = 6;
                grdExport.DisplayLayout.Bands[0].Columns["CustomerWO"].CellActivation = Activation.NoEdit;
                grdExport.DisplayLayout.Bands[0].Columns["CustomerWO"].Hidden = false;
                grdExport.DisplayLayout.Bands[0].Columns["CustomerWO"].Header.Caption = "Customer WO";

                grdExport.DisplayLayout.Bands[0].Columns["CompletedDate"].Header.VisiblePosition = 7;
                grdExport.DisplayLayout.Bands[0].Columns["CompletedDate"].CellActivation = Activation.NoEdit;
                grdExport.DisplayLayout.Bands[0].Columns["CompletedDate"].Hidden = false;
                grdExport.DisplayLayout.Bands[0].Columns["CompletedDate"].Header.Caption = "Completed Date";

                grdExport.DisplayLayout.Bands[0].Columns["Priority"].Header.VisiblePosition = 8;
                grdExport.DisplayLayout.Bands[0].Columns["Priority"].CellActivation = Activation.NoEdit;
                grdExport.DisplayLayout.Bands[0].Columns["Priority"].Hidden = false;

                grdExport.DisplayLayout.Bands[0].Columns["BasePrice"].Header.VisiblePosition = 9;
                grdExport.DisplayLayout.Bands[0].Columns["BasePrice"].CellActivation = Activation.NoEdit;
                grdExport.DisplayLayout.Bands[0].Columns["BasePrice"].Hidden = false;
                grdExport.DisplayLayout.Bands[0].Columns["BasePrice"].Header.Caption = "Base Price";

                grdExport.DisplayLayout.Bands[0].Columns["PartQuantity"].Header.VisiblePosition = 10;
                grdExport.DisplayLayout.Bands[0].Columns["PartQuantity"].CellActivation = Activation.NoEdit;
                grdExport.DisplayLayout.Bands[0].Columns["PartQuantity"].Hidden = false;
                grdExport.DisplayLayout.Bands[0].Columns["PartQuantity"].Header.Caption = "Quantity";

                grdExport.DisplayLayout.Bands[0].Columns["PartName"].Header.VisiblePosition = 11;
                grdExport.DisplayLayout.Bands[0].Columns["PartName"].CellActivation = Activation.NoEdit;
                grdExport.DisplayLayout.Bands[0].Columns["PartName"].Hidden = false;
                grdExport.DisplayLayout.Bands[0].Columns["PartName"].Header.Caption = "Part Name";

                grdExport.DisplayLayout.Bands[0].Columns["PartDesc"].Header.VisiblePosition = 12;
                grdExport.DisplayLayout.Bands[0].Columns["PartDesc"].CellActivation = Activation.NoEdit;
                grdExport.DisplayLayout.Bands[0].Columns["PartDesc"].Hidden = false;
                grdExport.DisplayLayout.Bands[0].Columns["PartDesc"].Header.Caption = "Part Desc.";

                grdExport.DisplayLayout.Bands[0].Columns["TotalPrice"].Header.VisiblePosition = 13;
                grdExport.DisplayLayout.Bands[0].Columns["TotalPrice"].CellActivation = Activation.NoEdit;
                grdExport.DisplayLayout.Bands[0].Columns["TotalPrice"].Hidden = false;
                grdExport.DisplayLayout.Bands[0].Columns["TotalPrice"].Header.Caption = "Total Price";

                grdExport.DisplayLayout.Bands[0].Columns["Processes"].Header.VisiblePosition = 14;
                grdExport.DisplayLayout.Bands[0].Columns["Processes"].CellActivation = Activation.NoEdit;
                grdExport.DisplayLayout.Bands[0].Columns["Processes"].Hidden = false;
                grdExport.DisplayLayout.Bands[0].Columns["Processes"].Header.Caption = "Processes";

                grdExport.DisplayLayout.Bands[0].Columns["ProcessAliases"].Header.VisiblePosition = 14;
                grdExport.DisplayLayout.Bands[0].Columns["ProcessAliases"].CellActivation = Activation.NoEdit;
                grdExport.DisplayLayout.Bands[0].Columns["ProcessAliases"].Hidden = false;
                grdExport.DisplayLayout.Bands[0].Columns["ProcessAliases"].Header.Caption = "Process Aliases";

                grdExport.DisplayLayout.Bands[0].Columns["Issues"].Header.VisiblePosition = 15;
                grdExport.DisplayLayout.Bands[0].Columns["Issues"].CellActivation = Activation.NoEdit;

                grdExport.DisplayLayout.Bands[0].Columns["BasePrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                grdExport.DisplayLayout.Bands[0].Columns["BasePrice"].Format = currencyFormat;

                grdExport.DisplayLayout.Bands[0].Columns["PartQuantity"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

                grdExport.DisplayLayout.Bands[0].Columns["TotalPrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                grdExport.DisplayLayout.Bands[0].Columns["TotalPrice"].Format = currencyFormat;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(grdExport.DisplayLayout.Bands[0]);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing export grid layout.");
            }
            finally
            {
                grdExport.AfterColPosChanged += grdExport_AfterColPosChanged;
                grdExport.AfterSortChange += grdExport_AfterSortChange;
            }
        }

        private void grdExport_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdExport.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column position in grid.");
            }
        }

        private void grdExport_AfterSortChange(object sender, BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdExport.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column sort in grid.");
            }
        }

        private void ExportInvoicesDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_runningTask)
            {
                e.Cancel = true;

                const string message = "Please wait for the export or invoice comparison " +
                    "to finish before closing this window.";

                const string title = "Export";

                MessageBoxUtilities.ShowMessageBoxWarn(message, title);
            }
        }

        private void timerExport_Tick(object sender, EventArgs e)
        {
            try
            {
                var progressBarInfo = statusBar.Panels[PANEL_PROGRESS_BAR].ProgressBarInfo;

                if (progressBarInfo.Value == 0)
                {
                    UpdateStatus("Export is taking longer than expected.");
                }

                timerExport.Stop();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error while showing export warning.");
            }
        }

        #endregion

    }
}