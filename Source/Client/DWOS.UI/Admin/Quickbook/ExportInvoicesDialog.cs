using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DWOS.QBExport;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.Data;
using DWOS.UI.Admin.Quickbook;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using NLog;

namespace DWOS.UI.Admin
{
    public partial class ExportInvoicesDialog: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private ExportInvoicesN _exportQB;
        private ExportCSV _exportCSV;

        #endregion

        #region Methods

        public ExportInvoicesDialog()
        {
            this.InitializeComponent();
        }

        private void InitialLoad()
        {
            this.ultraToolbarsManager1.Enabled = false;
            this.statusBar.Panels[1].Text = "";

            //Quickbooks or CSV based on settings
            this.Text = "Export to " + ApplicationSettings.Current.ExportInvoiceType;
            if (ApplicationSettings.Current.ExportInvoiceType.EquivalentTo(DWOS.UI.Admin.SettingsPanels.SettingsInvoiceInfo.enumInvoiceType.Quickbooks.ToString()))
            {
                this.ultraToolbarsManager1.Tools["Export"].SharedProps.ToolTipTextFormatted = "Export to " + ApplicationSettings.Current.ExportInvoiceType;
                this.ultraToolbarsManager1.Tools["Print"].SharedProps.Enabled = true;
                this.ultraToolbarsManager1.Tools["InvoiceStatusReport"].SharedProps.Enabled = true;

                this._exportQB = new ExportInvoicesN();
                this._exportQB.LoadData();
                this._exportQB.ProgessChanged += this._export_ProgessChanged;

                this.grdExport.DataMember = null;
                this.grdExport.DataSource = this._exportQB.OrderInvoices.OrderInvoice;

                this.statusBar.Panels[2].Text = this._exportQB.OrderInvoices.OrderInvoice.Count.ToString() + " Invoices";
            }
            else if (ApplicationSettings.Current.ExportInvoiceType.EquivalentTo(DWOS.UI.Admin.SettingsPanels.SettingsInvoiceInfo.enumInvoiceType.CSV.ToString()))
            {
                this.ultraToolbarsManager1.Tools["Export"].SharedProps.ToolTipTextFormatted = "Export to " + ApplicationSettings.Current.ExportInvoiceType;
                this.ultraToolbarsManager1.Tools["Print"].SharedProps.Enabled = false;
                this.ultraToolbarsManager1.Tools["InvoiceStatusReport"].SharedProps.Enabled = false;

                _exportCSV = new ExportCSV();
                _exportCSV.LoadData();

                //Load grid
                this.grdExport.DataMember = null;
                this.grdExport.DataSource = _exportCSV.OrderInvoices.OrderInvoice;

                this.statusBar.Panels[2].Text = _exportCSV.OrderInvoices.OrderInvoice.Count.ToString() + " Invoices";
            }

           
            this.ultraToolbarsManager1.Enabled = true;

            this.exportInvoiceSettings.Initialze(this.ultraToolbarsManager1.Tools["Settings2"] as PopupControlContainerTool);
        }

        private void PrintInvoiceReport()
        {
            try
            {
                if(this._exportQB != null && this._exportQB.OrderInvoices.OrderInvoice != null)
                {
                    var report = new ExportedInvoiceReport(this._exportQB.OrderInvoices.OrderInvoice);
                    report.DisplayReport();
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error creating exported invoice report.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void ExportInvoices()
        {
            int exportedCount = 0;

            try
            {
                using(new UsingWaitCursor(this))
                {
                    this.ultraToolbarsManager1.Tools["Export"].SharedProps.Enabled = false;

                    this.statusBar.Panels[0].ProgressBarInfo.Value = 0;
                    this.statusBar.Panels[1].Text                  = "Exporting Invoices...";

                    //Export to Quickbooks or CSV based on settings
                    if (ApplicationSettings.Current.ExportInvoiceType.EquivalentTo(DWOS.UI.Admin.SettingsPanels.SettingsInvoiceInfo.enumInvoiceType.Quickbooks.ToString()))
                    {
                        exportedCount = this._exportQB.Export();

                        this.statusBar.Panels[1].Text = String.Format("Exported {0} of {1} Invoices", exportedCount, this._exportQB.OrderInvoices.OrderInvoice.Count);
                        this.ultraToolbarsManager1.Tools["Print"].SharedProps.Enabled = true;

                        this.PrintInvoiceReport();
                    }
                    else if (ApplicationSettings.Current.ExportInvoiceType.EquivalentTo(DWOS.UI.Admin.SettingsPanels.SettingsInvoiceInfo.enumInvoiceType.CSV.ToString()))
                    {
                        _exportCSV.Export();
                        this.statusBar.Panels[1].Text = String.Format("Exported {0} of {1} Invoices", exportedCount, _exportCSV.OrderInvoices.OrderInvoice.Count);
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error exporting invoices.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void PrintInvoiceCompare()
        {
            try
            {
                using(new UsingWaitCursor(this))
                {
                    using(var compareDialog = new InvoiceCompareOptions())
                    {
                        if(compareDialog.ShowDialog(this) == DialogResult.OK)
                        {
                            var compare = new InvoiceComparison();
                            this.statusBar.Panels[0].ProgressBarInfo.Value = 0;
                            compare.ProgessChanged += this._export_ProgessChanged;

                            var invCompare = compare.CompareInvoices(compareDialog.dteFromDate.DateTime, compareDialog.dteToDate.DateTime);

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

                            excel.DisplayReport();
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error displaying invoice compare data.", exc);
            }
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

        private void DisposeMe()
        {
            if (_exportQB != null)
            {
                this._exportQB.ProgessChanged -= this._export_ProgessChanged;
                _exportQB.Dispose();
            }
            
            _exportQB = null;
        }

        #endregion

        #region Events

        private void _export_ProgessChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (IsDisposed) 
                    return;

                if(e.ProgressPercentage >= this.statusBar.Panels[0].ProgressBarInfo.Minimum && e.ProgressPercentage <= this.statusBar.Panels[0].ProgressBarInfo.Maximum)
                    this.statusBar.Panels[0].ProgressBarInfo.Value = e.ProgressPercentage;

                this.statusBar.Refresh();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error on progress changed.";
                _log.Error(errorMsg, exc);
            }
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
                    if(ApplicationSettings.Current.ExportInvoiceType == DWOS.UI.Admin.SettingsPanels.SettingsInvoiceInfo.enumInvoiceType.Quickbooks.ToString())
                        this.ExportInvoices();
                    else if (ApplicationSettings.Current.ExportInvoiceType == DWOS.UI.Admin.SettingsPanels.SettingsInvoiceInfo.enumInvoiceType.CSV.ToString())
                        this.ExportInvoices();
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
                    var orderId = Convert.ToInt32(e.Row.Cells["OrderID"].Value);
                    var partQty = Convert.ToInt32(e.Row.Cells["PartQuantity"].Value);
                    var cocPartQty = this.taOrderInvoice.GetCOCPartQuantity(orderId);

                    if(partQty != cocPartQty.GetValueOrDefault())
                    {
                        e.Row.Cells["PartQuantity"].Appearance = this.GetCellApperance("Red");
                        e.Row.Cells["Notes"].Value = "COC Quantity is set at " + cocPartQty.GetValueOrDefault();
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error("Error setting appearance of cell.", exc);
            }
        }

        #endregion
    }
}