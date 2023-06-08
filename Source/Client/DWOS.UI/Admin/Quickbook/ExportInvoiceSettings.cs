using System;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;

namespace DWOS.UI.Admin.Quickbook
{
    public partial class ExportInvoiceSettings: SettingsPanelBase, ISettingsPanel
    {
        #region Fields

        private bool _settingsLoaded;

        #endregion

        #region Methods

        public bool CanDock
        {
            get { return false; }
        }

        public ExportInvoiceSettings()
        {
            this.InitializeComponent();

            Enabled = false;
        }
        
        protected override void SaveSettings()
        {
            if(this._settingsLoaded)
            {
                //QBExport.Properties.Settings.Default.QBConnectionString = this.txtConnectionName.Text;
                //QBExport.Properties.Settings.Default.CustomerWOField = this.txtCustomerWO.Text;
                //QBExport.Properties.Settings.Default.TrackingNumberField = this.txtTrackingNumber.Text;
                //QBExport.Properties. ApplicationSettings.Current.WorkOrderPrefix = this.txtWOPrefix.Text;
                //QBExport.Properties.Settings.Default.QBClass = this.txtTransactionClass.Text;
                //QBExport.Properties.Settings.Default.MaxBatchExport = Convert.ToInt32(this.numMaxExport.Value);
                //QBExport.Properties.Settings.Default.Save();

                ApplicationSettings.Current.QBConnectionString = this.txtConnectionName.Text;
                ApplicationSettings.Current.InvoiceCustomerWOField = this.txtCustomerWO.Text;
                ApplicationSettings.Current.InvoiceTrackingNumberField = this.txtTrackingNumber.Text;
                ApplicationSettings.Current.InvoiceWorkOrderPrefix = this.txtWOPrefix.Text;
                ApplicationSettings.Current.QBClass = this.txtTransactionClass.Text;
                ApplicationSettings.Current.InvoiceMaxBatchExport = Convert.ToInt32(this.numMaxExport.Value);
                ApplicationSettings.Current.Save();
            }
        }

        protected override void LoadSettings()
        {
            if(!this._settingsLoaded)
            {
                Enabled = this.Editable;

                //this.txtConnectionName.Text = QBExport.Properties.Settings.Default.QBConnectionString;
                //this.txtCustomerWO.Text = QBExport.Properties.Settings.Default.CustomerWOField;
                //this.txtTrackingNumber.Text = QBExport.Properties.Settings.Default.TrackingNumberField;
                //this.txtWOPrefix.Text = QBExport.Properties. ApplicationSettings.Current.WorkOrderPrefix;
                //this.txtTransactionClass.Text = QBExport.Properties.Settings.Default.QBClass;
                //this.numMaxExport.Value = QBExport.Properties.Settings.Default.MaxBatchExport;

                this.txtConnectionName.Text = ApplicationSettings.Current.QBConnectionString;
                this.txtCustomerWO.Text = ApplicationSettings.Current.InvoiceCustomerWOField;
                this.txtTrackingNumber.Text = ApplicationSettings.Current.InvoiceTrackingNumberField;
                this.txtWOPrefix.Text = ApplicationSettings.Current.InvoiceWorkOrderPrefix;
                this.txtTransactionClass.Text = ApplicationSettings.Current.QBClass;
                this.numMaxExport.Value = ApplicationSettings.Current.InvoiceMaxBatchExport;

                this._settingsLoaded = true;
            }
        }
        

        #endregion

        #region ISettingsPanel Members

        public string PanelKey
        {
            get { return "QuickBooks"; }
        }

        public bool Editable
        {
            get { return SecurityManager.Current.IsValidUser; }
        }

        public void LoadData()
        {
            this.LoadSettings();
        }

        public void SaveData()
        {
            this.SaveSettings();
        }

        #endregion
    }
}