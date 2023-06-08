using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using DWOS.Utilities.Validation;
using DWOS.Data.Datasets;
using DWOS.Shared;
using DWOS.UI.Utilities;
using DWOS.UI.Properties;
using Infragistics.Win.UltraWinEditors;

namespace DWOS.UI.Sales
{
    public partial class OrderBulkFieldChange: Form
    {
        #region Fields

        private ValidatorManager _manager;
        private string _mediaFilePath;
        private OrdersDataSet.OrderRow[] _orders;

        #endregion

        #region Properties

        public bool POChanged
        {
            get { return this.chkPO.Checked; }
        }

        public bool OrderDateChanged
        {
            get { return this.chkOrderDate.Checked; }
        }

        public bool ShipDateChanged
        {
            get { return this.chkEstShipDate.Checked; }
        }

        public bool ReqDateChanged
        {
            get { return this.chkOrderRequiredDate.Checked; }
        }

        public bool CustomerWOChanged
        {
            get { return this.chkCustomerWO.Checked; }
        }

        public bool PriorityChanged
        {
            get { return this.chkPriority.Checked; }
        }

        public bool PODocChanged
        {
            get { return this.chkMedia.Checked; }
        }

        public bool PartQtyChanged
        {
            get { return this.chkPartQty.Checked; }
        }

        public bool UnitPriceChanged
        {
            get { return this.chkUnitPrice.Checked; }
        }

        public string POValue
        {
            get { return this.txtPONumber.Text; }
        }

        public DateTime OrderDateValue
        {
            get { return this.dtOrderDate.DateTime; }
        }

        public DateTime EstShipValue
        {
            get { return this.dtShipDate.DateTime; }
        }

        public DateTime ReqDateValue
        {
            get { return this.dtOrderRequiredDate.DateTime; }
        }

        public string CustomerWOValue
        {
            get { return this.cboCustomerWO.Text; }
        }

        public string PriorityValue
        {
            get { return this.cboPriority.Text; }
        }

        public string PODocFilePathValue
        {
            get { return this._mediaFilePath; }
        }

        public int PartQtyValue
        {
            get { return this.numPartQty.Value == null ? 1 : Convert.ToInt32(this.numPartQty.Value); }
        }

        public decimal UnitPriceValue
        {
            get { return Convert.ToDecimal(this.numUnitPrice.Value); }
        }

        #endregion

        #region Methods

        public OrderBulkFieldChange()
        {
            this.InitializeComponent();
        }

        public void LoadData(OrdersDataSet dsOrders, OrdersDataSet.OrderRow[] orders)
        {
            this._orders = orders;

            this.BindList(this.cboPriority, dsOrders.d_Priority, dsOrders.d_Priority.PriorityIDColumn.ColumnName, dsOrders.d_Priority.PriorityIDColumn.ColumnName);

            //set default values
            if(this._orders.Length > 0)
            {
                OrdersDataSet.OrderRow order = this._orders[0];

                if(!order.IsPurchaseOrderNull())
                    this.txtPONumber.Text = order.PurchaseOrder;
                if(!order.IsOrderDateNull())
                    this.dtOrderDate.Value = order.OrderDate;
                if (!order.IsEstShipDateNull())
                    this.dtShipDate.Value = order.EstShipDate;
                if(!order.IsRequiredDateNull())
                    this.dtOrderRequiredDate.Value = order.RequiredDate;
                if(!order.IsCustomerWONull())
                    this.cboCustomerWO.Text = order.CustomerWO;
                if(!order.IsPriorityNull())
                    this.cboPriority.Text = order.Priority;
                if(!order.IsPartQuantityNull())
                    this.numPartQty.Value = order.PartQuantity;
                if(!order.IsBasePriceNull())
                    this.numUnitPrice.Value = order.BasePrice;
            }

            this.AddValidators();
        }

        private void BindList(UltraComboEditor cbo, DataTable dt, string valueMember, string displayMember)
        {
            cbo.DataSource = dt.DefaultView;
            cbo.DisplayMember = displayMember;
            cbo.ValueMember = valueMember;
        }

        private void AddValidators()
        {
            this._manager = new ValidatorManager();
            this._manager.Add(new ImageDisplayValidator(new DateTimeValidator(this.dtOrderDate, "Order date required."), this.errorProvider));
            this._manager.Add(new ImageDisplayValidator(new DateTimeValidator(this.dtShipDate, "Estimated ship date."), this.errorProvider));
            this._manager.Add(new ImageDisplayValidator(new DateTimeValidator(this.dtOrderRequiredDate, "Required date is required."), this.errorProvider));
            this._manager.Add(new ImageDisplayValidator(new NumericControlValidator(this.numPartQty){MinimumValue = 1}, this.errorProvider));
            this._manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboPriority, "Priority required."), this.errorProvider));
            this._manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtPONumber, "PO Number required."){DefaultValue = "<PO>"}, this.errorProvider));
            this._manager.Add(new ImageDisplayValidator(new CurrencyControlValidator(this.numUnitPrice), this.errorProvider));
        }

        #endregion

        #region Events

        private void chkPO_AfterCheckStateChanged(object sender, EventArgs e)
        {
            this.txtPONumber.Enabled = this.chkPO.Checked;
        }

        private void chkOrderDate_AfterCheckStateChanged(object sender, EventArgs e)
        {
            this.dtOrderDate.Enabled = this.chkOrderDate.Checked;
        }

        private void chkEstShipDate_AfterCheckStateChanged(object sender, EventArgs e)
        {
            this.dtShipDate.Enabled = this.chkEstShipDate.Checked;
        }

        private void chkOrderRequiredDate_AfterCheckStateChanged(object sender, EventArgs e)
        {
            this.dtOrderRequiredDate.Enabled = this.chkOrderRequiredDate.Checked;
        }

        private void chkCustomerWO_AfterCheckStateChanged(object sender, EventArgs e)
        {
            this.cboCustomerWO.Enabled = this.chkCustomerWO.Enabled;
        }

        private void ultraCheckEditor7_AfterCheckStateChanged(object sender, EventArgs e)
        {
            this.cboPriority.Enabled = this.chkPriority.Checked;
        }

        private void chkMedia_CheckedChanged(object sender, EventArgs e)
        {
            this.txtMedia.Enabled = this.chkMedia.Checked;
        }

        private void chkPartQty_CheckedChanged(object sender, EventArgs e)
        {
            this.numPartQty.Enabled = this.chkPartQty.Checked;
        }

        private void chkUnitPrice_CheckedChanged(object sender, EventArgs e)
        {
            this.numUnitPrice.Enabled = this.chkUnitPrice.Checked;
        }

        private void txtMedia_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                switch(e.Button.Key)
                {
                    case "Add":
                        using(var fd = new OpenFileDialog())
                        {
                            fd.InitialDirectory = Settings.Default.ScannedPODirectory;
                            fd.Filter = "Any Media (*.*)|*.*";

                            if(fd.ShowDialog(ActiveForm) == DialogResult.OK)
                            {
                                this._mediaFilePath = fd.FileName;
                                this.txtMedia.Text = Path.GetFileNameWithoutExtension(this._mediaFilePath);
                            }
                        }
                        break;
                    case "Delete":
                        this._mediaFilePath = null;
                        this.txtMedia.Text = null;
                        break;
                    case "View": //view the media
                        if(this.txtMedia.Text != null && this._mediaFilePath != null)
                        {
                            DWOS.Shared.Utilities.FileLauncher.New()
                                .HandleErrorsWith((exception, errorMsg) => MessageBoxUtilities.ShowMessageBoxWarn(errorMsg, "View Media"))
                                .Launch(this._mediaFilePath);
                        }
                        break;
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error with media.", exc);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(this._manager.ValidateControls())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        #endregion
    }
}