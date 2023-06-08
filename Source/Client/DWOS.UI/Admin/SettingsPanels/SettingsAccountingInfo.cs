using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Interop;

namespace DWOS.UI.Admin.SettingsPanels
{
    public partial class SettingsAccountingInfo: UserControl, ISettingsPanel
    {
        #region Fields

        private const string GENERAL_PART = "General Part";
        private const string QUICKBOOKS_DESCRIPTION_TEMPLATE = "QuickBooks_Description";

        private SysproInvoiceSettings _sysproSettings;
        private readonly DisplayDisabledTooltips _displayTooltipsMain;
        private readonly DisplayDisabledTooltips _displayTooltipsType;
        private readonly DisplayDisabledTooltips _displayTooltipsPart;

        /// <summary>
        /// DataTable that persists templates.
        /// </summary>
        private readonly ApplicationSettingsDataSet.TemplatesDataTable _dtTemplates;

        #endregion

        #region Properties
        public bool CanDock
        {
            get { return false; }
        }

        public string PanelKey
        {
            get { return "Accounting"; }
        }

        #endregion

        #region Methods
        
        public SettingsAccountingInfo()
        {
            this.InitializeComponent();
            _dtTemplates = new ApplicationSettingsDataSet.TemplatesDataTable();
            _displayTooltipsMain = new DisplayDisabledTooltips(ultraGroupBox1, ultraToolTipManager1);
            _displayTooltipsType = new DisplayDisabledTooltips(ultraGroupBox2, ultraToolTipManager1);
            _displayTooltipsPart = new DisplayDisabledTooltips(ultraGroupBox3, ultraToolTipManager1);
        }

        public bool Editable
        {
            get { return SecurityManager.Current.IsInRole("ApplicationSettings.Edit"); }
        }

        public void LoadData()
        {
            Enabled = this.Editable;

            this.LoadInvoiceLevels();
            this.LoadInvoiceTypes();
            this.LoadRoundingTypes();
            this.LoadOtherColumnValues();
            
            this.txtExportPartItemName.Text = ApplicationSettings.Current.InvoicePartItemName;
            if(string.IsNullOrEmpty(this.txtExportPartItemName.Text))
                this.txtExportPartItemName.Text = GENERAL_PART;

            this.txtExportPartItemCode.Text = ApplicationSettings.Current.InvoicePartItemCode;
            if (string.IsNullOrEmpty(this.txtExportPartItemCode.Text))
                this.txtExportPartItemCode.Text = GENERAL_PART;

            this.chkIndexSOInvoices.Checked = ApplicationSettings.Current.IndexSOInvoices;
            this.chkCalcUnitPrice.Checked = ApplicationSettings.Current.InvoiceCalcUnitPrice;
            this.chkHeaders.Checked = ApplicationSettings.Current.InvoiceHeaderRow;
            this.txtExportTokens.Text = ApplicationSettings.Current.InvoiceExportTokens;
            this.txtConnectionName.Text = ApplicationSettings.Current.QBConnectionString;
            this.txtTransactionClass.Text = ApplicationSettings.Current.QBClass;
            this.txtCustomerWO.Text = ApplicationSettings.Current.InvoiceCustomerWOField;
            this.txtTrackingNumber.Text = ApplicationSettings.Current.InvoiceTrackingNumberField;
            this.txtWOPrefix.Text = ApplicationSettings.Current.InvoiceWorkOrderPrefix;
            this.txtSOPrefix.Text = ApplicationSettings.Current.InvoiceSalesOrderPrefix;
            this.txtPackagePrefix.Text = ApplicationSettings.Current.InvoicePackagePrefix;
            this.numMaxExport.Value = ApplicationSettings.Current.InvoiceMaxBatchExport;
            this.numMaxErrors.Value = ApplicationSettings.Current.InvoiceExportMaxErrors;
            this.numPriceDecimals.Value = ApplicationSettings.Current.PriceDecimalPlaces;
            this.chkCheckTotalPrice.Checked = ApplicationSettings.Current.InvoiceCheckTotal;

            InvoiceType exportType = ApplicationSettings.Current.InvoiceExportType;
            chkExportCSV.Checked = exportType == InvoiceType.CSV;
            chkExportQuickbooks.Checked = exportType == InvoiceType.Quickbooks;
            chkExportSyspro.Checked = exportType == InvoiceType.Syspro;

            chkExportSyspro.Enabled = ApplicationSettings.Current.SysproIntegrationEnabled;

            var selectedLevelItem = this.cboInvoiceLevel.FindItemByValue<InvoiceLevelType>(v => v == ApplicationSettings.Current.InvoiceLevel);
            if (selectedLevelItem != null)
                this.cboInvoiceLevel.SelectedItem = selectedLevelItem;
            else
                this.cboInvoiceLevel.SelectedIndex = 0;

            var selectedTypeItem = this.cboInvoiceType.FindItemByValue<InvoiceLineItemType>(
                v => v == ApplicationSettings.Current.InvoiceLineItemType);

            if (selectedTypeItem != null)
                this.cboInvoiceType.SelectedItem = selectedTypeItem;
            else
                this.cboInvoiceType.SelectedIndex = 0;


            var selectedOther1Item = this.cboOther1.FindItemByValue<InvoiceItemType>(v => v == ApplicationSettings.Current.InvoiceItem1);
            if (selectedOther1Item != null)
                this.cboOther1.SelectedItem = selectedOther1Item;
            else
                this.cboOther1.SelectedIndex = 0;

            var selectedOther2Item = this.cboOther2.FindItemByValue<InvoiceItemType>(v => v == ApplicationSettings.Current.InvoiceItem2);
            if (selectedOther2Item != null)
                this.cboOther2.SelectedItem = selectedOther2Item;
            else
                this.cboOther2.SelectedIndex = 0;

            var selectedRoundingType = cboQuickbooksRounding
                .FindItemByValue<MidpointRounding>(v => v == ApplicationSettings.Current.QuickBooksInvoiceMidpointRounding);

            if (selectedRoundingType != null)
            {
                cboQuickbooksRounding.SelectedItem = selectedRoundingType;
            }
            else
            {
                cboQuickbooksRounding.SelectedIndex = 0;
            }

            _sysproSettings = ApplicationSettings.Current.SysproInvoiceSettings?.Copy();

            // Load QuickBooks description template
            using (var taTemplate = new TemplatesTableAdapter { ClearBeforeFill = false })
            {
                taTemplate.FillById(_dtTemplates, QUICKBOOKS_DESCRIPTION_TEMPLATE);
            }
        }

        public void SaveData()
        {
            ApplicationSettings.Current.IndexSOInvoices = this.chkIndexSOInvoices.Checked;
            ApplicationSettings.Current.InvoicePartItemName = this.txtExportPartItemName.Text;
            ApplicationSettings.Current.InvoicePartItemCode = this.txtExportPartItemCode.Text;
            ApplicationSettings.Current.InvoiceCalcUnitPrice = this.chkCalcUnitPrice.Checked;
            ApplicationSettings.Current.InvoiceHeaderRow = this.chkHeaders.Checked;
            ApplicationSettings.Current.InvoiceExportTokens = this.txtExportTokens.Text;
            ApplicationSettings.Current.QBConnectionString = this.txtConnectionName.Text;
            ApplicationSettings.Current.QBClass = this.txtTransactionClass.Text;
            ApplicationSettings.Current.InvoiceCustomerWOField = this.txtCustomerWO.Text;
            ApplicationSettings.Current.InvoiceTrackingNumberField = this.txtTrackingNumber.Text;
            ApplicationSettings.Current.InvoiceWorkOrderPrefix = this.txtWOPrefix.Text;
            ApplicationSettings.Current.InvoiceSalesOrderPrefix = this.txtSOPrefix.Text ;
            ApplicationSettings.Current.InvoicePackagePrefix = this.txtPackagePrefix.Text;
            ApplicationSettings.Current.InvoiceMaxBatchExport = Convert.ToInt32(this.numMaxExport.Value);
            ApplicationSettings.Current.InvoiceExportMaxErrors = Convert.ToInt32(this.numMaxErrors.Value);
            ApplicationSettings.Current.PriceDecimalPlaces = Convert.ToInt32(this.numPriceDecimals.Value);
            ApplicationSettings.Current.InvoiceCheckTotal = this.chkCheckTotalPrice.Checked;

            if (chkExportQuickbooks.Checked)
            {
                ApplicationSettings.Current.InvoiceExportType = InvoiceType.Quickbooks;
            }
            else if (chkExportSyspro.Checked)
            {
                ApplicationSettings.Current.InvoiceExportType = InvoiceType.Syspro;
            }
            else
            {
                ApplicationSettings.Current.InvoiceExportType = InvoiceType.CSV;
            }

            ApplicationSettings.Current.SysproInvoiceSettings = _sysproSettings;

            ApplicationSettings.Current.InvoiceLevel = this.cboInvoiceLevel.SelectedItem == null ?
                InvoiceLevelType.WorkOrder :
                (this.cboInvoiceLevel.SelectedItem.DataValue as InvoiceLevelType?) ?? InvoiceLevelType.WorkOrder;

            ApplicationSettings.Current.InvoiceLineItemType = this.cboInvoiceType.SelectedItem == null ?
                InvoiceLineItemType.Part :
                (this.cboInvoiceType.SelectedItem.DataValue as InvoiceLineItemType?) ?? InvoiceLineItemType.Part;

            ApplicationSettings.Current.InvoiceItem1 = this.cboOther1.SelectedItem == null ?
                InvoiceItemType.None :
                (this.cboOther1.SelectedItem.DataValue as InvoiceItemType?) ?? InvoiceItemType.None;

            ApplicationSettings.Current.InvoiceItem2 = this.cboOther2.SelectedItem == null ?
                InvoiceItemType.None :
                (this.cboOther2.SelectedItem.DataValue as InvoiceItemType?) ?? InvoiceItemType.None;

            ApplicationSettings.Current.QuickBooksInvoiceMidpointRounding = cboQuickbooksRounding.SelectedItem?.DataValue as MidpointRounding? ?? MidpointRounding.ToEven;

            // Save changes to template
            using (var ta = new TemplatesTableAdapter())
            {
                ta.Update(_dtTemplates);
            }
        }
        
        private void LoadInvoiceLevels()
        {
            this.cboInvoiceLevel.Items.Clear();

            this.cboInvoiceLevel.Items.Add(InvoiceLevelType.SalesOrder, InvoiceLevelType.SalesOrder.ToString());
            var defaultItem = this.cboInvoiceLevel.Items.Add(InvoiceLevelType.WorkOrder, InvoiceLevelType.WorkOrder.ToString());
            this.cboInvoiceLevel.Items.Add(InvoiceLevelType.Package, InvoiceLevelType.Package.ToString());

            this.cboInvoiceLevel.SelectedItem = defaultItem;
        }

        private void LoadInvoiceTypes()
        {
            cboInvoiceType.Items.Clear();
            var defaultItem = cboInvoiceType.Items.Add(InvoiceLineItemType.Part, "Part");
            cboInvoiceType.Items.Add(InvoiceLineItemType.Department, "Department");
            cboInvoiceType.Items.Add(InvoiceLineItemType.ProductClass, "Product Class");

            cboInvoiceType.SelectedItem = defaultItem;
        }

        private void LoadRoundingTypes()
        {
            cboQuickbooksRounding.Items.Clear();
            var defaultItem = cboQuickbooksRounding.Items.Add(MidpointRounding.ToEven, "Banker's Rounding");
            cboQuickbooksRounding.Items.Add(MidpointRounding.AwayFromZero, "Away From Zero");
            cboQuickbooksRounding.SelectedItem = defaultItem;
        }

        private void LoadOtherColumnValues()
        {
            var comboBoxes = new List<Infragistics.Win.UltraWinEditors.UltraComboEditor>
            {
                this.cboOther1,
                this.cboOther2
            };

            foreach (var cbo in comboBoxes)
            {
                cbo.Items.Clear();
                cbo.Items.Add(InvoiceItemType.CustomerWO, InvoiceItemType.CustomerWO.ToString());
                cbo.Items.Add(InvoiceItemType.WO, InvoiceItemType.WO.ToString());
                cbo.Items.Add(InvoiceItemType.TrackingNumber, InvoiceItemType.TrackingNumber.ToString());
                cbo.Items.Add(InvoiceItemType.Weight, InvoiceItemType.Weight.ToString());
                cbo.Items.Add(InvoiceItemType.PO, InvoiceItemType.PO.ToString());
                cbo.Items.Add(InvoiceItemType.PackingSlip, InvoiceItemType.PackingSlip.ToString());
                var defaultItem = cbo.Items.Add(InvoiceItemType.None, "<" + InvoiceItemType.None.ToString() + ">");
                cbo.SelectedItem = defaultItem;
            }
        }

        private void UpdateExportTypeEnabled()
        {
            txtExportTokens.Enabled = chkExportCSV.Checked;
            chkCalcUnitPrice.Enabled = chkExportCSV.Checked;
            chkHeaders.Enabled = chkExportCSV.Checked;

            txtConnectionName.Enabled = chkExportQuickbooks.Checked;
            txtTransactionClass.Enabled = chkExportQuickbooks.Checked;
            cboQuickbooksRounding.Enabled = chkExportQuickbooks.Checked;
            btnQuickBooksTemplate.Enabled = chkExportQuickbooks.Checked;

            cboInvoiceType.Enabled = !chkExportSyspro.Checked;
            btnSyspro.Enabled = chkExportSyspro.Checked;
        }

        private ApplicationSettingsDataSet.TemplatesRow CreateQuickBooksDescriptionTemplate()
        {
            return _dtTemplates.AddTemplatesRow(
                QUICKBOOKS_DESCRIPTION_TEMPLATE,
                "%PART%",
                "Template to use for the QuickBooks description field.",
                "%PART%, %PROCESSES%, %WO%, %DESCRIPTION%");
        }

        #endregion

        #region Events

        private void chkExportCSV_CheckedChanged(object sender, EventArgs e)
        {
            chkExportQuickbooks.CheckedChanged -= chkExportQuickbooks_CheckedChanged;
            chkExportSyspro.CheckedChanged -= chkExportSyspro_CheckedChanged;

            chkExportQuickbooks.Checked = !chkExportCSV.Checked;
            chkExportSyspro.Checked = false;
            UpdateExportTypeEnabled();

            chkExportQuickbooks.CheckedChanged += chkExportQuickbooks_CheckedChanged;
            chkExportSyspro.CheckedChanged += chkExportSyspro_CheckedChanged;
        }

        private void chkExportQuickbooks_CheckedChanged(object sender, EventArgs e)
        {
            chkExportCSV.CheckedChanged -= chkExportCSV_CheckedChanged;
            chkExportSyspro.CheckedChanged -= chkExportSyspro_CheckedChanged;

            chkExportCSV.Checked = !chkExportQuickbooks.Checked;
            chkExportSyspro.Checked = false;
            UpdateExportTypeEnabled();

            chkExportCSV.CheckedChanged += chkExportCSV_CheckedChanged;
            chkExportSyspro.CheckedChanged += chkExportSyspro_CheckedChanged;
        }

        private void chkExportSyspro_CheckedChanged(object sender, EventArgs e)
        {
            chkExportQuickbooks.CheckedChanged -= chkExportQuickbooks_CheckedChanged;
            chkExportCSV.CheckedChanged -= chkExportCSV_CheckedChanged;

            chkExportCSV.Checked = !chkExportSyspro.Checked;
            chkExportQuickbooks.Checked = false;
            UpdateExportTypeEnabled();

            chkExportQuickbooks.CheckedChanged += chkExportQuickbooks_CheckedChanged;
            chkExportCSV.CheckedChanged += chkExportCSV_CheckedChanged;
        }

        private void btnSyspro_Click(object sender, EventArgs e)
        {
            var window = new SysproSettingsDialog();
            window.Load(_sysproSettings ?? new SysproInvoiceSettings());
            var helper = new WindowInteropHelper(window) { Owner = Handle };

            if (window.ShowDialog() ?? false)
            {
                _sysproSettings = window.Settings;
            }

            GC.KeepAlive(helper);
        }


        private void btnQuickBooksTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                var window = new PlainTextTemplateDialog();

                var templateRow = _dtTemplates.FindByTemplateID(QUICKBOOKS_DESCRIPTION_TEMPLATE)
                    ?? CreateQuickBooksDescriptionTemplate();

                if (templateRow == null)
                {
                    LogManager.GetCurrentClassLogger()
                        .Error("Could not edit existing template or create new one.");

                    return;
                }

                window.Load("QuickBooks Description Template",
                    templateRow.Template,
                    templateRow.IsTokensNull() ? string.Empty : templateRow.Tokens);

                var helper = new WindowInteropHelper(window)
                {
                    Owner = Handle
                };

                if (window.ShowDialog() ?? false)
                {
                    templateRow.Template = window.TemplateContent;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error in QuickBooks template editor.");
            }
        }

        #endregion
    }
}