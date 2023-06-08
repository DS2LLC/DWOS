using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DWOS.UI.Utilities;
using DWOS.Data.Datasets;
using Infragistics.Win;
using Infragistics.Shared;
using Infragistics.Win.UltraWinGrid;
using NLog;

namespace DWOS.UI.Sales
{
    public partial class ReceivingOrderImport: Form
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #region Properties

        public int? SelectedReceivingOrder
        {
            get
            {
                if(this.grdImport.Selected.Rows.Count == 1)
                    return Convert.ToInt32(this.grdImport.Selected.Rows[0].Cells["ReceivingID"].Value);

                return null;
            }
        }

        public int? AssignToSalesOrderID
        {
            get
            {
                if (this.grdImport.Selected.Rows.Count == 1)
                    return Convert.ToInt32(this.grdImport.Selected.Rows[0].Cells["SalesOrderID"].Value);

                return null;
            }
        }

        public int? NodeSalesOrderID
        {
            get;
            set;
        }

        /// <summary>
        ///   Gets or sets the used reciving orders. These are orders already in use so they will be hidden.
        /// </summary>
        /// <value> The used reciving orders. </value>
        public List<int> UsedReceivingOrders { get; set; }

        public bool IsRework
        {
            get { return this.chkIsRework.Checked; }
        }

        #endregion

        #region Methods

        public ReceivingOrderImport()
        {
            this.InitializeComponent();
        }

        private void LoadData()
        {
            try
            {


                var loadedCustomerIDs = new HashSet<int>();
                this.taReceivingSummary.FillOpenRecOrders(this.partsDataset1.ReceivingSummary);

                //Add available sales orders for each customer
                foreach (PartsDataset.ReceivingSummaryRow row in this.partsDataset1.ReceivingSummary)
                {
                    UltraGridBand band = this.grdImport.DisplayLayout.Bands[0];

                    foreach (UltraGridRow rw in band.GetRowEnumerator(GridRowType.DataRow))
                    {
                        //Create a default entry item
                        ValueList valueList = new ValueList();
                        valueList.ValueListItems.Add("-1", "<None>");

                        //Get any sales orders for this customer that user may want to assign this import order to
                        var customerID = Convert.ToInt32(rw.Cells["CustomerID"].Value);

                        if (!loadedCustomerIDs.Contains(customerID))
                        {
                            this.taSalesOrder.FillByCustomerID(ordersDataSet.SalesOrder, customerID);
                            loadedCustomerIDs.Add(customerID);
                        }

                        var salesOrders = this.ordersDataSet.SalesOrder
                            .Where(s => s.CustomerID == customerID);

                        foreach (OrdersDataSet.SalesOrderRow salesOrder in salesOrders)
                        {
                            valueList.ValueListItems.Add(salesOrder.SalesOrderID, salesOrder.SalesOrderID.ToString());

                            //Select the sales order for the node that the user has selected in OE
                            if (this.NodeSalesOrderID.GetValueOrDefault() == salesOrder.SalesOrderID)
                                rw.Cells["SalesOrderID"].Value = salesOrder.SalesOrderID;
                        }

                        rw.Cells["SalesOrderID"].ValueList = valueList;
                        rw.Cells["SalesOrderID"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;

                        //Select <none> if no sales order association
                        if (rw.Cells["SalesOrderID"].Value == null)
                            rw.Cells["SalesOrderID"].Value = -1;
                    }
                }

                if (this.grdImport.Rows.Count > 0)
                    this.grdImport.Rows[0].Selected = true;
            }
            catch (Exception exc)
            {
                string errorMsg = "Error loading Recieving data.";
                _log.Error(exc, errorMsg);
            }
        }

        private void SaveData()
        {
            try
            {
                DataTable dtDeleted = this.partsDataset1.ReceivingSummary.GetChanges(DataRowState.Deleted);

                if(dtDeleted != null && dtDeleted.Rows.Count > 0)
                {
                    foreach(DataRow item in dtDeleted.Rows)
                    {
                        item.RejectChanges();
                        //mark each order recipet that is marked delted with a -1 entry to simulate deletion
                        this.taReceivingSummary.UpdateOrderID(-1, Convert.ToInt32(item[this.partsDataset1.ReceivingSummary.ReceivingIDColumn.ColumnName]));
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error deleting receiving summary rows.";
                _log.Error(exc, errorMsg);
            }
        }

        #endregion

        #region Events

        private void dropDown_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                if(e.Row.IsDataRow)
                {
                    int id = Convert.ToInt32(e.Row.Cells["ReceivingID"].Value);

                    //if row is already in use then do not show
                    if(this.UsedReceivingOrders != null && this.UsedReceivingOrders.Contains(id))
                        e.Row.Hidden = true;
                    else
                    {
                        int v = Convert.ToInt32(e.Row.Cells["ProcessCount"].Value);

                        if(v > 0)
                            e.Row.Appearance.BackColor = Color.LightGreen;
                        else
                            e.Row.Appearance.BackColor = Color.Salmon;
                    }

                    e.Row.Cells["SalesOrderID"].IgnoreRowColActivation = true;
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error updating row color.";
                _log.Error(exc, errorMsg);
            }
        }

        private void grdImport_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.RowSelectorImages.DataChangedImage = null;
            e.Layout.RowSelectorImages.ActiveAndDataChangedImage = null;

            //if(this.grdImport.Rows.Count > 0)
            //    this.grdImport.Rows[0].Selected = true;
            //grdImport.UseAppStyling = false;
            //grdImport.DisplayLayout.DefaultSelectedBackColor = Color.Transparent;
            //grdImport.DisplayLayout.SelectionOverlayColor = Color.Transparent;
        }

        private void ReceivingOrderImport_Load(object sender, EventArgs e)
        {
            if(DesignMode)
                return;

            this.LoadData();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.SaveData();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void grdImport_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            //btnOK.Enabled = grdImport.Selected.Rows.Count == 1;
        }

        private void grdImport_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            var itemsAsString = new StringBuilder();
            foreach(UltraGridRow r in e.Rows)
            {
                itemsAsString.Append(r.Cells["ReceivingID"].Value);
                itemsAsString.Append("|");
            }

            using(var frm = new UserEventLog{Operation = "Delete", Form = "RecOrderImport", UserID = SecurityManager.Current.UserID, UserName = SecurityManager.Current.UserName, TransactionDetails = itemsAsString.ToString()})
            {
                bool deleteOK = frm.ShowDialog(this) == DialogResult.OK;
                e.Cancel = !deleteOK;
            }

            e.DisplayPromptMsg = false;
        }

        private void grdImport_DoubleClickRow(object sender, DoubleClickRowEventArgs e) { btnOK_Click(this, EventArgs.Empty); }

        #endregion
    }
}