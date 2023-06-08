using System;
using System.Data;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Shared;
using Infragistics.Win.UltraWinGrid;
using NLog;
using System.Linq;
using System.Drawing;

namespace DWOS.UI.Sales
{
    public partial class OrderFees: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private OrdersDataSet _dsOrders;
        private int _orderID;

        #endregion

        #region Properties

        public OrdersDataSet OrdersDataset
        {
            get { return this._dsOrders; }
            set { this._dsOrders = value; }
        }

        public int OrderID
        {
            get { return this._orderID; }
            set
            {
                this._orderID = value;
                Text += " - " + this._orderID;
            }
        }
        public bool IsQuickView { get; set; } = false;

        #endregion

        #region Methods

        public OrderFees()
        {
            this.InitializeComponent();
        }

        private decimal CalculateDefaultFee(string feeTypeID)
        {
            if(String.IsNullOrEmpty(feeTypeID))
                return 0;
            else
            {
                OrdersDataSet.OrderFeeTypeRow oftRow = this.dsOrders.OrderFeeType.FindByOrderFeeTypeID(feeTypeID);
                return oftRow != null ? oftRow.Price : 0;
            }
        }

       

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsQuickView)
                {
                    //release variable before closing to prevent them from being disposed because they are used on parent form
                    this.grdOrderFees.UpdateData();
                    this.bsOrderFee.EndEdit();
                    this.bsOrderFee = null;
                    this.dsOrders = null;
                    this._dsOrders = null;
                }

                Close();
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error closing form.", exc);
                _log.Fatal(exc, "Error closing form.");
            }
        }

        private void OrderFees_Load(object sender, EventArgs e)
        {
            //rebind datasources
            this.dsOrders = this._dsOrders;
            this.bsOrderFee.DataSource = this._dsOrders;
            this.bsOrderFee.Filter = "OrderID = " + this._orderID;
            this.grdOrderFees.DataBind();

            this.cboFeeType.DataSource = this.dsOrders.OrderFeeType;
            this.cboFeeType.DisplayMember = this.dsOrders.OrderFeeType.OrderFeeTypeIDColumn.ColumnName;
            this.cboFeeType.DataBind();

            //ensure an item is selected
            if(this.cboFeeType.Items.Count > 0)
                this.cboFeeType.SelectedIndex = 0;

            maskedEditorCurrency.InputMask = "{currency:-5." + ApplicationSettings.Current.PriceDecimalPlaces + "}";

            maskedEditorPercent.InputMask = string.Format("-nnn.{0} %",
                string.Concat(Enumerable.Repeat('n', ApplicationSettings.Current.PriceDecimalPlaces)));

            //Viewed from WIP order summary (i.e. QuickViewOrder dialog), so it's readonly view of fees
            grdOrderFees.Enabled = !IsQuickView;
            lblDefaultFees.Enabled = !IsQuickView;
        }

        private void grdOrderFees_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Appearance.ResetBackColor();
            e.Row.RowSelectorAppearance.ResetBackColor();
            var ofRow = ((DataRowView)e.Row.ListObject).Row as OrdersDataSet.OrderFeesRow;

            if(ofRow != null)
            {
                ofRow.OrderID = this.OrderID;
                if(this.cboFeeType.SelectedItem == null)
                    this.cboFeeType.SelectedIndex = 0;

                if(ofRow.IsNull(this.dsOrders.OrderFees.OrderFeeTypeIDColumn.ColumnName) || String.IsNullOrEmpty(ofRow.OrderFeeTypeID))
                    ofRow.OrderFeeTypeID = this.cboFeeType.SelectedItem.DisplayText;
                if(ofRow.IsNull(this.dsOrders.OrderFees.ChargeColumn.ColumnName))
                    ofRow.Charge = this.CalculateDefaultFee(ofRow.OrderFeeTypeID);

                // Configure row based on FeeType
                var oftRow = ofRow.OrderFeeTypeRow;
                if (oftRow != null)
                {
                    var feeType = oftRow.FeeType;
                    e.Row.Cells["FeeType"].SetValue(feeType, false);

                    if (feeType.EquivalentTo("Fixed"))
                    {
                        e.Row.Cells["Charge"].EditorComponent = this.maskedEditorCurrency;
                    }
                    else if (feeType.EquivalentTo("Percentage"))
                    {
                        e.Row.Cells["Charge"].EditorComponent = this.maskedEditorPercent;
                    }

                    // Show error if:
                    //    - discount is positive, -or-
                    //    - fee is negative
                    var isDiscount = oftRow.Price < 0;

                    if (isDiscount != ofRow.Charge < 0)
                    {
                        e.Row.Appearance.BackColor = Color.Red;
                        e.Row.RowSelectorAppearance.BackColor = Color.Red;
                    }
                }
            }
        }

        private void lblDefaultFees_Click(object sender, EventArgs e)
        {
            var currentOrder = dsOrders.Order.FindByOrderID(OrderID);
            if (ApplicationSettings.Current.ApplyDefaultFeesEnabled)
                OrderFeeTools.AddDefaultFees(ref currentOrder);
        }

        private void grdOrderFees_CellChange(object sender, CellEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Key == "OrderFeeTypeID" && e.Cell.Text != null)
                {
                    var feeType = this.dsOrders.OrderFeeType.FindByOrderFeeTypeID(e.Cell.Text);

                    if (feeType != null)
                    {
                        e.Cell.Row.Cells["Charge"].SetValue(feeType.Price, true);
                        e.Cell.Row.Cells["FeeType"].SetValue(feeType.FeeType, false);
                        e.Cell.Row.Update();
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error with order fees cell change.");
            }
        }

        private void grdOrderFees_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            this.grdOrderFees.DisplayLayout.Bands[0].Columns["Charge"].UseEditorMaskSettings = true;
            this.grdOrderFees.DisplayLayout.Bands[0].Columns["Charge"].PadChar = ' ';
            this.grdOrderFees.DisplayLayout.Bands[0].Columns["Charge"].PromptChar = '_';
            this.grdOrderFees.DisplayLayout.Bands[0].Columns["Charge"].MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.grdOrderFees.DisplayLayout.Bands[0].Columns["Charge"].MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
        }

        #endregion


        
    }
    static class OrderFeeTools
    {


        public static void DeleteDefaultFees(ref OrdersDataSet.OrderRow currentOrder)
        {
            try
            {
                var dsOrders = (OrdersDataSet)currentOrder.Table.DataSet;
                var fees     = dsOrders.OrderFees;
                fees.Rows.Clear();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error deleting default fees.");
            }
        }



            public static void AddDefaultFees(ref OrdersDataSet.OrderRow currentOrder)
        {
            try
            {
                if (currentOrder == null)
                    return;

                    var dsOrders = (OrdersDataSet)currentOrder.Table.DataSet;
                    // Add customer-default fees
                    foreach (var customerFee in currentOrder.CustomerSummaryRow.GetCustomerFeeRows())
                    {
                        string feeTypeId = customerFee.OrderFeeTypeID;
                        var feeType = dsOrders.OrderFeeType.FindByOrderFeeTypeID(feeTypeId);
                        var existingFee = currentOrder.GetOrderFeesRows().FirstOrDefault(fee => fee.OrderFeeTypeID == feeTypeId);

                        if (feeType != null && existingFee == null)
                        {
                            var feeRow = dsOrders.OrderFees.NewOrderFeesRow();
                            feeRow.OrderID = currentOrder.OrderID;
                            feeRow.Charge = customerFee.Charge;
                            feeRow.OrderFeeTypeID = feeType.OrderFeeTypeID;
                            dsOrders.OrderFees.AddOrderFeesRow(feeRow);
                        }
                    }

                    // Add system-default fees
                    if (!string.IsNullOrWhiteSpace(ApplicationSettings.Current.DefaultFees))
                    {
                        var defaultFees = ApplicationSettings.Current.DefaultFees.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var defaultFeeTypeId in defaultFees)
                        {
                            var feeTypeId = defaultFeeTypeId.Trim();
                            var feeType = dsOrders.OrderFeeType.FindByOrderFeeTypeID(feeTypeId);
                            var existingFee = currentOrder.GetOrderFeesRows().FirstOrDefault(fee => fee.OrderFeeTypeID == feeTypeId);

                            if (feeType != null && existingFee == null)
                            {
                                var feeRow = dsOrders.OrderFees.NewOrderFeesRow();
                                feeRow.OrderID = currentOrder.OrderID;
                                feeRow.Charge = feeType.Price;
                                feeRow.OrderFeeTypeID = feeType.OrderFeeTypeID;
                                dsOrders.OrderFees.AddOrderFeesRow(feeRow);
                            }
                        }
                    }


            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error adding default fees.");
            }
        }
    }
}