using DWOS.Data;
using DWOS.Shared;
using DWOS.UI.Utilities;
using Infragistics.Win;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DWOS.UI.Admin
{

    public partial class OrderFeeManager: Form
    {
        #region Methods

        public OrderFeeManager()
        {
            this.InitializeComponent();
        }

        private void LoadData()
        {
            this.dsOrders.EnforceConstraints = false;
            this.dsOrders.OrderFeeType.BeginLoadData();

            this.taOrderFeeType.Fill(this.dsOrders.OrderFeeType);

            this.dsOrders.OrderFeeType.EndLoadData();

            //Create the valuelist of fee types
            this.taFeeType.Fill(this.dsOrders.d_FeeType);

            var list = DataUtilities.CreateValueList(this.dsOrders.d_FeeType,
                this.dsOrders.d_FeeType.FeeTypeIDColumn.ColumnName,
                this.dsOrders.d_FeeType.FeeTypeIDColumn.ColumnName,
                null);

            list.SortStyle = ValueListSortStyle.Ascending;

            var feeTypeColumn = grdFees.DisplayLayout.Bands[0].Columns["FeeType"];
            feeTypeColumn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            feeTypeColumn.ValueList = list;
            feeTypeColumn.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
        }

        private bool SaveData()
        {
            try
            {
                this.grdFees.UpdateData();
                this.taOrderFeeType.Update(this.dsOrders.OrderFeeType);

                return true;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving data.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);

                return false;
            }
        }

        #endregion

        #region Events

        private void FeeManager_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadData();
            }
            catch (Exception exc)
            {
                string errorMsg = "Error loading order fee manager.";
                NLog.LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if(this.SaveData())
                    Close();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving data.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void grdFees_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
        {
            foreach (var row in e.Rows)
            {
                var feeTypeID = row.Cells["OrderFeeTypeID"].Text;

                int usageCount;
                using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderFeeTypeTableAdapter())
                {
                    usageCount = ta.GetUsageCount(feeTypeID) ?? 0;
                }

                if (usageCount > 0)
                {
                    e.Cancel = true;
                    MessageBoxUtilities.ShowMessageBoxWarn(
                        "Fee is in-use. Remove the fee from all orders and customers before deleting.",
                        "Unable to delete fee");
                }
            }
        }

        private void grdFees_AfterRowInsert(object sender, Infragistics.Win.UltraWinGrid.RowEventArgs e)
        {
            if(e.Row.IsDataRow && (String.IsNullOrEmpty(e.Row.Cells["OrderFeeTypeID"].Text) || String.IsNullOrEmpty(e.Row.Cells["OrderFeeTypeID"].Value.ToString())))
            {
                var rnd = new Random();
                e.Row.Cells["OrderFeeTypeID"].Value = "New Fee " + rnd.Next(1000, 9999);
            }

            if (e.Row.IsDataRow && String.IsNullOrEmpty(e.Row.Cells["Price"].Text))
            {
                e.Row.Cells["Price"].Value = 1;
            }

            if (e.Row.IsDataRow && String.IsNullOrEmpty(e.Row.Cells["FeeType"].Text))
            {
                e.Row.Cells["FeeType"].Value = OrderPrice.enumFeeType.Fixed.ToString();
            }
        }

        private void grdFees_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            var feeRow = ((DataRowView)e.Row.ListObject).Row as Data.Datasets.OrdersDataSet.OrderFeeTypeRow;

            string feeType = feeRow?.FeeType ?? string.Empty;

            if (feeType.EquivalentTo(OrderPrice.enumFeeType.Fixed.ToString()))
            {
                e.Row.Cells["Price"].EditorComponent = this.maskedEditorCurrency;
            }
            else if (feeType.EquivalentTo(OrderPrice.enumFeeType.Percentage.ToString()))
            {
                e.Row.Cells["Price"].EditorComponent = this.maskedEditorPercent;
            }

            // Highlight discounts in a different color.
            if (feeRow.Price < 0)
            {
                e.Row.Appearance.BackColor = e.Row.IsAlternate
                    ? Color.Goldenrod
                    : Color.LightGoldenrodYellow;
            }
            else
            {
                e.Row.Appearance.ResetBackColor();
            }
        }

        private void grdFees_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            this.grdFees.DisplayLayout.Bands[0].Columns["Price"].UseEditorMaskSettings = true;
            this.grdFees.DisplayLayout.Bands[0].Columns["Price"].PadChar = ' ';
            this.grdFees.DisplayLayout.Bands[0].Columns["Price"].PromptChar = '_';
            this.grdFees.DisplayLayout.Bands[0].Columns["Price"].MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.grdFees.DisplayLayout.Bands[0].Columns["Price"].MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
        }
        private void grdFees_BeforeCellUpdate(object sender, Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventArgs e)
        {
            if (e.Cell.Column.Key != "OrderFeeTypeID")
            {
                return;
            }

            bool duplicateIdExists = this.grdFees.Rows
                .OfType<Infragistics.Win.UltraWinGrid.UltraGridRow>()
                .Any(row => Convert.ToString(row.Cells["OrderFeeTypeID"].Value) == Convert.ToString(e.NewValue));

            if (duplicateIdExists)
            {
                string message = "The fee name \"{0}\" is being used.".FormatWith(e.NewValue);
                string header = "Unable to rename fee";
                MessageBoxUtilities.ShowMessageBoxWarn(message, header);
                e.Cancel = true;
            }

        }
        #endregion
    }
}