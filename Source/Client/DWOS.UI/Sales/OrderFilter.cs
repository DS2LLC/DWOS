using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Datasets.PartsDatasetTableAdapters;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using NLog;

namespace DWOS.UI.Sales
{
    public partial class OrderFilter: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        private bool _checkingAllParts;

        #endregion

        #region Methods

        public OrderFilter()
        {
            this.InitializeComponent();
        }

        private void LoadCustomers()
        {
            Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter taCustomers = null;
            CustomersDataset.CustomerDataTable customers = null;

            try
            {
                customers = new CustomersDataset.CustomerDataTable();
                taCustomers = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter();
                taCustomers.Fill(customers);

                foreach(CustomersDataset.CustomerRow item in customers)
                    this.cboValueCustomer.Items.Add(item.CustomerID, item.Name);
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error loading customers.", exc);
            }
            finally
            {
                if(taCustomers != null)
                    taCustomers.Dispose();
                if(customers != null)
                    customers.Dispose();

                customers = null;
                taCustomers = null;
            }
        }

        private void LoadOrderStatus()
        {
            d_OrderStatusTableAdapter taOrderStatus = null;
            OrdersDataSet.d_OrderStatusDataTable orderStatus = null;

            try
            {
                orderStatus = new OrdersDataSet.d_OrderStatusDataTable();
                taOrderStatus = new d_OrderStatusTableAdapter();
                taOrderStatus.Fill(orderStatus);

                foreach(OrdersDataSet.d_OrderStatusRow item in orderStatus)
                    this.cboOrderStatus.Items.Add(item.OrderStatusID, item.OrderStatusID);

                //select Open Orders by default
                this.cboOrderStatus.Items[1].CheckState = CheckState.Checked;
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error loading order status.", exc);
            }
            finally
            {
                if(taOrderStatus != null)
                    taOrderStatus.Dispose();
                if(orderStatus != null)
                    orderStatus.Dispose();

                orderStatus = null;
                taOrderStatus = null;
            }
        }

        private void UpdatePartNumbersByCustomer()
        {
            PartsDataset.PartDataTable parts = null;
            PartTableAdapter taParts = null;

            try
            {
                if(this.chkPart.Enabled)
                {
                    this.cboValuePartNumber.Items.Clear();

                    if(this.cboValueCustomer.CheckedItems.Count == 1 && !this._checkingAllParts)
                    {
                        parts = new PartsDataset.PartDataTable();
                        taParts = new PartTableAdapter();

                        parts.Constraints.Clear();
                        parts.Columns.Remove("CustomerID");

                        taParts.FillByCustomer(parts, Convert.ToInt32(this.cboValueCustomer.CheckedItems[0].DataValue), true);

                        foreach(PartsDataset.PartRow item in parts)
                            this.cboValuePartNumber.Items.Add(item.PartID, item.Name);
                    }
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error loading parts based on selected customer.", exc);
            }
            finally
            {
                if(parts != null)
                    parts.Dispose();
                if(taParts != null)
                    taParts.Dispose();

                taParts = null;
                parts = null;
            }
        }

        private string GetSQLClause()
        {
            var sb = new StringBuilder();

            //Order ID / WO
            if(this.chkWO.Checked && this.numValueWO.Value != null)
            {
                sb.Append(this.dsOrders.Order.OrderIDColumn.ColumnName);
                sb.Append(" " + this.cboOpWO.Text + " ");
                sb.Append(Data.Datasets.Utilities.SqlBless(this.numValueWO.Value.ToString()));
            }

            //Order Status
            if(this.chkOrderStatus.Checked && this.cboOrderStatus.CheckedItems.Count > 0)
            {
                if(sb.Length > 0)
                    sb.Append(" AND ");

                sb.Append(this.dsOrders.Order.StatusColumn.ColumnName);

                string status = this.GetSelectedValues(this.cboOrderStatus);
                if(status.Contains(',')) //selected more than one
                {
                    sb.Append(" IN (");
                    sb.Append(status);
                    sb.Append(")");
                }
                else //else selected just one
                {
                    sb.Append(" = ");
                    sb.Append(status);
                }
            }

            //Purchase Order / PO
            if(this.chkPO.Checked && !string.IsNullOrEmpty(this.txtValuePO.Text))
            {
                if(sb.Length > 0)
                    sb.Append(" AND ");

                sb.Append(this.dsOrders.Order.PurchaseOrderColumn.ColumnName);
                sb.Append(" LIKE '%");
                sb.Append(Data.Datasets.Utilities.SqlBless(this.txtValuePO.Text));
                sb.Append("%'");
            }

            //Customer
            if(this.chkCustomer.Checked && this.cboValueCustomer.CheckedItems.Count > 0)
            {
                if(sb.Length > 0)
                    sb.Append(" AND ");

                sb.Append("Customer.Name");

                string customers = this.GetSelectedValues(this.cboValueCustomer);
                if(customers.Contains(',')) //selected more than one
                {
                    sb.Append(" IN (");
                    sb.Append(customers);
                    sb.Append(")");
                }
                else //else selected just one
                {
                    sb.Append(" = ");
                    sb.Append(customers);
                }
            }

            //Part Name
            if(this.chkPart.Checked && !string.IsNullOrEmpty(this.cboValuePartNumber.Text))
            {
                if(sb.Length > 0)
                    sb.Append(" AND ");

                sb.Append("Part.Name");
                string parts = this.GetSelectedValues(this.cboValuePartNumber); // Calls CleanSQLValue
                if(parts.Contains(',')) //selected more than one
                {
                    sb.Append(" IN (");
                    sb.Append(parts);
                    sb.Append(")");
                }
                else //else selected just one
                {
                    sb.Append(" LIKE '%");
                    sb.Append(parts);
                    sb.Append("%'");
                }
            }

            //Due Date
            if(this.chkDueDate.Checked && this.dteValueDueDate.Value != null)
            {
                if(sb.Length > 0)
                    sb.Append(" AND ");

                // = :> est ship date >= '2/1/2015' AND est ship date <= '2/1/2015'
                //Get only the Date Part not the time
                sb.Append(this.dsOrders.Order.EstShipDateColumn.ColumnName);

                if(this.cboOpDueDate.Text == "=")
                {
                    sb.Append(" >= '");
                    sb.Append(this.dteValueDueDate.DateTime.StartOfDay());
                    sb.Append("'");

                    sb.Append(" AND ");
                    sb.Append(this.dsOrders.Order.EstShipDateColumn.ColumnName);
                    sb.Append(" <= '");
                    sb.Append(this.dteValueDueDate.DateTime.EndOfDay());
                    sb.Append("'");
                }
                else // > OR <
                {
                    sb.Append(" " + this.cboOpDueDate.Text + " '");
                    sb.Append(this.cboOpDueDate.Text == ">" ? this.dteValueDueDate.DateTime.StartOfDay() : this.dteValueDueDate.DateTime.EndOfDay());
                    sb.Append("'");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the SQL-escaped version of the editor's text.
        /// </summary>
        /// <param name="cbo"></param>
        /// <returns></returns>
        private string GetSelectedValues(UltraComboEditor cbo)
        {
            var sb = new StringBuilder();

            //if only one item selected and nothing else is typed into the box
            if(cbo.DropDownStyle == DropDownStyle.DropDown && cbo.CheckedItems.Count == 1 && cbo.CheckedItems[0].DisplayText == cbo.Text)
                sb.Append(Data.Datasets.Utilities.SqlBless(cbo.CheckedItems[0].DisplayText));
            else if(cbo.CheckedItems.Count > 0)
            {
                foreach(ValueListItem item in cbo.CheckedItems)
                {
                    if(sb.Length > 0)
                        sb.Append(" , ");

                    sb.Append("'" + Data.Datasets.Utilities.SqlBless(item.DisplayText) + "'");
                }
            }

            string value = sb.ToString();

            //if is a drop down and text is specifed that is not in the existing list
            if(cbo.DropDownStyle == DropDownStyle.DropDown && !String.IsNullOrEmpty(cbo.Text) && (value == null || !value.Contains(cbo.Text)))
            {
                if(!String.IsNullOrEmpty(value))
                    value += (", '" + Data.Datasets.Utilities.SqlBless(cbo.Text) + "'");
                else
                    value += (Data.Datasets.Utilities.SqlBless(cbo.Text));
            }

            return value;
        }

        #endregion

        #region Events

        private void OrderFilter_Load(object sender, EventArgs e)
        {
            _log.Info("Loading Order Filter.");
            this.LoadOrderStatus();
            this.LoadCustomers();
            this.dteValueDueDate.DateTime = DateTime.Now;
        }

        private void cboValueCustomer_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                if(e.Button != null)
                {
                    CheckState checkAll = e.Button.Key == "CheckAll" ? CheckState.Checked : CheckState.Unchecked;
                    this._checkingAllParts = true;

                    ICheckedItemList itemList = this.cboValueCustomer.Items.ValueList;
                    for(int i = 0; i < this.cboValueCustomer.Items.Count; i++)
                        itemList.SetCheckState(i, checkAll);

                    this._checkingAllParts = false;
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error updating check state of customers.", exc);
            }
        }

        private void cboValuePartNumber_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                if(e.Button != null)
                {
                    CheckState checkAll = e.Button.Key == "CheckAll" ? CheckState.Checked : CheckState.Unchecked;

                    if(this.cboValuePartNumber.Items.Count > 0)
                    {
                        ICheckedItemList itemList = this.cboValuePartNumber.Items.ValueList;
                        for(int i = 0; i < this.cboValuePartNumber.Items.Count; i++)
                            itemList.SetCheckState(i, checkAll);
                    }
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error updating check state of part numbers.", exc);
            }
        }

        private void cboValueCustomer_ValueChanged(object sender, EventArgs e)
        {
            this.UpdatePartNumbersByCustomer();
        }

        private void chkWO_CheckedChanged(object sender, EventArgs e)
        {
            this.cboOpWO.Enabled = this.numValueWO.Enabled = this.chkWO.Checked;
        }

        private void chkPO_CheckedChanged(object sender, EventArgs e)
        {
            this.cboOpPO.Enabled = this.txtValuePO.Enabled = this.chkPO.Checked;
        }

        private void chkCustomer_CheckedChanged(object sender, EventArgs e)
        {
            this.cboOpCustomer.Enabled = this.cboValueCustomer.Enabled = this.chkCustomer.Checked;

            if(!this.cboOpCustomer.Enabled)
                this.cboValueCustomer_EditorButtonClick(this, new EditorButtonEventArgs(this.cboValueCustomer.ButtonsRight["CheckAll"], null));
        }

        private void chkPart_CheckedChanged(object sender, EventArgs e)
        {
            this.cboOpPart.Enabled = this.cboValuePartNumber.Enabled = this.chkPart.Checked;

            this.UpdatePartNumbersByCustomer();
        }

        private void chkDueDate_CheckedChanged(object sender, EventArgs e)
        {
            this.cboOpDueDate.Enabled = this.dteValueDueDate.Enabled = this.chkDueDate.Checked;
        }

        private void chkOrderStatus_CheckedChanged(object sender, EventArgs e)
        {
            this.cboOrderStatus.Enabled = this.chkOrderStatus.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = this.GetSQLClause();

                if(String.IsNullOrEmpty(sql))
                    sql = " 1=1 ";

                _log.Debug("Order Filter Where clause: " + sql);

                this.dsOrders.EnforceConstraints = false;
                this.taOrders.Fill(this.dsOrders.Order, sql);

                if(this.dsOrders.Order.Count < 1)
                {
                    MessageBox.Show("No orders selected.", About.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.None;
                }
                else
                {
                    var os = new OrderStatusReport(this.dsOrders.Order);
                    os.DisplayReport();
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error displaying order status report.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion
    }
}