using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using NLog;

namespace DWOS.UI.Sales
{
    public partial class OrderSearch : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public Data.Datasets.OrderStatusDataSet.OrderSearchRow SelectedOrder { get; private set; }

        #endregion

        #region Methods

        public OrderSearch()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            cboQuoteSearchField.Items.Add(Data.Datasets.OrderStatusDataSetTableAdapters.OrderSearchTableAdapter.OrderSearchField.All, "-All-");
            cboQuoteSearchField.Items.Add(Data.Datasets.OrderStatusDataSetTableAdapters.OrderSearchTableAdapter.OrderSearchField.OrderID, "WO");
            cboQuoteSearchField.Items.Add(Data.Datasets.OrderStatusDataSetTableAdapters.OrderSearchTableAdapter.OrderSearchField.CurrentLocation, "Location");
            cboQuoteSearchField.Items.Add(Data.Datasets.OrderStatusDataSetTableAdapters.OrderSearchTableAdapter.OrderSearchField.CustomerName, "Customer");
            cboQuoteSearchField.Items.Add(Data.Datasets.OrderStatusDataSetTableAdapters.OrderSearchTableAdapter.OrderSearchField.PartName, "Part");
            cboQuoteSearchField.Items.Add(Data.Datasets.OrderStatusDataSetTableAdapters.OrderSearchTableAdapter.OrderSearchField.PurchaseOrder, "PO");
            cboQuoteSearchField.Items.Add(Data.Datasets.OrderStatusDataSetTableAdapters.OrderSearchTableAdapter.OrderSearchField.Status, "Status");
            cboQuoteSearchField.Items.Add(Data.Datasets.OrderStatusDataSetTableAdapters.OrderSearchTableAdapter.OrderSearchField.WorkStatus, "Work Status");
            cboQuoteSearchField.Items.Add(Data.Datasets.OrderStatusDataSetTableAdapters.OrderSearchTableAdapter.OrderSearchField.SerialNumber, "Serial Number");

            cboQuoteSearchField.SelectedIndex = 0;
        }

        private void Search(Data.Datasets.OrderStatusDataSetTableAdapters.OrderSearchTableAdapter.OrderSearchField searchField, string search)
        {
            try
            {
                using(new Utilities.UsingWaitCursor(this))
                {
                    taOrderSearch.FillBySearch(dsOrderStatus.OrderSearch, searchField, search, chkActiveOnly.Checked);

                    grdParts.PerformAction(UltraGridAction.ToggleRowSel);
                    lblRecordCount.Text = "Record Count: " + grdParts.Rows.GetFilteredInNonGroupByRows().Length;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error searching for parts.");
            }
        }

        #endregion

        #region Events

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search((Data.Datasets.OrderStatusDataSetTableAdapters.OrderSearchTableAdapter.OrderSearchField)cboQuoteSearchField.SelectedItem.DataValue, txtSearch.Text);
        }

        private void btnGoTo_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdParts.Selected.Rows.Count > 0)
                {
                    var row = grdParts.Selected.Rows[0];
                    if (row != null && row.IsDataRow && row.Band.Columns.Exists("OrderID"))
                    {
                        this.SelectedOrder = ((DataRowView)row.ListObject).Row as Data.Datasets.OrderStatusDataSet.OrderSearchRow;
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error in go to part.");
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Return)
                btnSearch.PerformClick();
        }

        private void QuoteSearch_Load(object sender, EventArgs e) { LoadData(); }

        private void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdParts.Selected.Rows.Count > 0)
                {
                    var row = grdParts.Selected.Rows[0];

                    if (row != null && row.IsDataRow && row.Band.Columns.Exists("OrderID"))
                    {
                        var orderID = Convert.ToInt32(row.GetCellValue("OrderID"));

                        using (var p = new QuickViewOrder())
                        {
                            p.OrderID = orderID;
                            p.ShowDialog(this);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error showing part quick view.");
            }
        }

        #endregion
    }
}
