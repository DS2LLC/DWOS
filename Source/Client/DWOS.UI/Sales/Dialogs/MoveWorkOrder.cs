using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Order;
using DWOS.UI;
using NLog;
using DWOS.Data.Datasets;
using Infragistics.Win.UltraWinTree;
using Infragistics.Win.UltraDataGridView;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;


namespace DWOS.UI.Sales.Dialogs
{
    public partial class MoveWorkOrder : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private OrdersDataSet.OrderRow _OrderRow;

        private OrdersDataSet.SalesOrderDataTable _dtSalesOrders;
        #endregion

        #region Properties
        public OrdersDataSet.OrderRow orderRow
        {
            set { _OrderRow = value; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveWorkOrder"/> class.
        /// </summary>
        public MoveWorkOrder()
        {
            InitializeComponent();
            ugSalesOrders.DisplayLayout.Bands[0].Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
        }

        /// <summary>
        /// Handles the Load event of the MoveWorkOrder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void MoveWorkOrder_Load(object sender, EventArgs e)
        {
            //get the current order
            try
            {
                string customerName = "";
                var dtCustomerSummary = new OrdersDataSet.ContactSummaryDataTable();
                using (var taCustomerSummary = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.CustomerSummaryTableAdapter())
                {
                    customerName = taCustomerSummary.GetCustomerName(_OrderRow.CustomerID);
                }

                string PartName = "";
                var dtPartSummary = new OrdersDataSet.PartSummaryDataTable();
                using (var taPartSummary = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.PartSummaryTableAdapter())
                {
                    PartName = taPartSummary.GetPartNameByOrder(_OrderRow.OrderID);
                }


                lbWorkOrder.Text = _OrderRow.OrderID.ToString();

                if (!_OrderRow.IsSalesOrderIDNull())
                    lbSalesOrder.Text = _OrderRow.SalesOrderID.ToString();

                if (!_OrderRow.IsPurchaseOrderNull())
                    lbPO.Text = _OrderRow.PurchaseOrder.ToString();

                if (!_OrderRow.IsPartIDNull())
                    lbPart.Text = _OrderRow.PartID.ToString();

                if (!_OrderRow.IsCustomerIDNull())
                    gbSalesOrders.Text = $"Sales Orders for Customer {_OrderRow.CustomerID}";



                _dtSalesOrders = new OrdersDataSet.SalesOrderDataTable();

                if (!_OrderRow.IsPurchaseOrderNull()) 
                {
                    _dtSalesOrders.DefaultView.RowFilter = $"PurchaseOrder = '{_OrderRow.PurchaseOrder}'";
                }
                else
                {
                    _dtSalesOrders.DefaultView.RowFilter = $"PurchaseOrder Is Null";
                }

                FillGrid();
                ugSalesOrders.Selected.Rows.Clear();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error loading move command.");
            }

        }

        /// <summary>
        /// Fills the ugSalesOrder grid control in Move Work Order dialog
        /// </summary>
        private void FillGrid()
        {
            using (var taSalesOrders = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.SalesOrderTableAdapter())
            {

                taSalesOrders.FillByCustomerID(_dtSalesOrders, _OrderRow.CustomerID);
                List<SOData> soStringArrays = new List<SOData>();
                soStringArrays.Add(new SOData() { SalesOrder = "None", PO = (!_OrderRow.IsPurchaseOrderNull()) ? _OrderRow.PurchaseOrder : "None", Created = "", CreatedBy = "" });
                
                foreach (System.Data.DataRowView soRow in _dtSalesOrders.DefaultView)
                {
                    string po = "";
                    try { po = soRow["PurchaseOrder"].ToString(); }
                    catch (Exception) { po = "None"; }
                    soStringArrays.Add(new SOData() { SalesOrder = soRow["SalesOrderID"].ToString(), PO = po, Created = soRow["OrderDate"].ToString(), CreatedBy = "Admin" });

                }
                ugSalesOrders.DataSource = soStringArrays;
                
                //rename Columns
                ugSalesOrders.DisplayLayout.Bands[0].Columns[0].Header.Caption = "Sales Order";
                ugSalesOrders.DisplayLayout.Bands[0].Columns[1].Header.Caption = "Purchase Order";
                ugSalesOrders.DisplayLayout.Bands[0].Columns[2].Header.Caption = "Date Created";
                ugSalesOrders.DisplayLayout.Bands[0].Columns[3].Header.Caption = "Created By";

                //disable editing
                ugSalesOrders.ActiveCell = null;
                //_dtSalesOrders.DefaultView.RowFilter = $"PurchaseOrder = '{_OrderRow.PurchaseOrder}'";
            }

        }

        /// <summary>Handles the Click event of the btnOK control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            string PO = "";

            if (ugSalesOrders.Selected.Rows.Count > 0)
            {
               
                PO = ugSalesOrders.Selected.Rows[0].Cells["PO"].Value.ToString();

                _OrderRow.PurchaseOrder = (_OrderRow.PurchaseOrder != PO) ? PO : _OrderRow.PurchaseOrder;


                if (ugSalesOrders.Selected.Rows[0].Cells[0].Value.ToString() == "None")
                    _OrderRow.SetSalesOrderIDNull();
                else
                    _OrderRow.SalesOrderID = Convert.ToInt32(ugSalesOrders.Selected.Rows[0].Cells["SalesOrder"].Value);


            }
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        /// <summary>Handles the CheckedChanged event of the cbShowPO control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cbShowPO_CheckedChanged(object sender, EventArgs e)
        {
           if(_dtSalesOrders != null)
            {
                if (((UltraCheckEditor)sender).Checked)
                    _dtSalesOrders.DefaultView.RowFilter = "";
                else
                    _dtSalesOrders.DefaultView.RowFilter = (!_OrderRow.IsPurchaseOrderNull()) ? $"PurchaseOrder = '{_OrderRow.PurchaseOrder}'" : $"PurchaseOrder Is Null";
                FillGrid();

            }

        }

    }


    class SOData
    {
        public string SalesOrder { get; set; }
        public string PO { get; set; }
        public string Created { get; set; }
        public string CreatedBy { get; set; }
    }
}
