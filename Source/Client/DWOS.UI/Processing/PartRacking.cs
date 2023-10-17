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
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;
using DWOS.UI.Reports;
using DWOS.Utilities.Validation;
using NLog;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win;

namespace DWOS.UI.Processing
{
   
    public partial class PartRacking : Form
    {
        #region Fields
        private OrdersDataSet.MediaDataTable _mediaDT;
        private OrdersDataSet.RackOrdersDataTable _rackDT;
        private int _userSelectedWO = -1;

        private string _SelectedDept;

        private BindingSource _RackOrderBS;

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion
        #region Properties

        /// <summary>
        ///     Gets the selected work order.
        /// </summary>
        /// <value> The work order. </value>
        public int WorkOrder
        {
            get
            {
                if (this.cboOrder.SelectedItem != null)
                    return Convert.ToInt32(this.cboOrder.SelectedItem.DataValue);
                return -1;
            }
            set { this._userSelectedWO = value; }
        }

        public string FormSelectedDeptMain { set { _SelectedDept = value; } }
        
        #endregion

        private void LoadData()
        {
            try
            {
                //load all orders that are changing departments
                using (var taRackOrders = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.RackOrdersTableAdapter())
                {
                    taRackOrders.Fill(this._rackDT, _SelectedDept.ToString());
                    _RackOrderBS = new BindingSource();
                    _RackOrderBS.DataSource = _rackDT.DefaultView;
                }

                //fill the orderID combobox
                List<String> orders =  _rackDT.Select(dr => dr["OrderID"].ToString()).ToList();
                foreach (string o in orders)
                    this.cboOrder.Items.Add(o.ToString());


                //this.cboOrder.DataBindings.Add("Text", _RackOrderBS, "OrderID");
                this.lbCustomer.DataBindings.Add("Text", _RackOrderBS, "CustomerName");
                this.lbPart.DataBindings.Add("Text", _RackOrderBS, "PartName");
                this.lbPO.DataBindings.Add("Text", _RackOrderBS, "PurchaseOrder");
                this.numPartQty.DataBindings.Add("Value", _RackOrderBS, "PartQuantity",false);
                this.dtDueDate.DataBindings.Add("Value", _RackOrderBS, "RequiredDate");
                this.tePartNotes.DataBindings.Add("Text", _RackOrderBS, "PartNotes");
                this.txtOrderNotes.DataBindings.Add("Text", _RackOrderBS, "CustomerNotes");
      

            }
            catch (Exception exc)
            {
                var errorMsg = "Error loading data.";
                _log.Error(exc, errorMsg);
            }
        }

        private void LoadMedia(int OrderID)
        {
            try
            {
                using (var taMedia = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter())
                {
                    this._mediaDT = taMedia.GetPartsWOMediaByOrder(OrderID);

                    //dispose current image first
                    if (this.picPartImage.Image != null)
                    {
                        ((Image)this.picPartImage.Image).Dispose();
                        this.picPartImage.Image = null;
                    }

                    if (_mediaDT.Rows.Count==0)
                        return;

                    DWOS.Data.Datasets.OrdersDataSet.MediaRow mr = (DWOS.Data.Datasets.OrdersDataSet.MediaRow)taMedia.GetPartsWOMediaByOrder(OrderID).Rows[0];

                    //get image from media id
                    this._mediaDT = taMedia.GetPartsWOMediaByOrder(_userSelectedWO);
                    //this.taMedia.FillDefaultWithoutMedia(this._mediaDT, row.PartID);
                    var medias = this._mediaDT.ToArray();

                    if (medias.Length > 0)
                        this.picPartImage.Image = MediaUtilities.GetImage(medias[0].MediaID, medias[0].FileExtension);

                    //if image not set
                    if (this.picPartImage.Image == null)
                        this.picPartImage.Image = Properties.Resources.NoImage;
                    //FormLoad = false;

                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error Loading Media Record.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void moveToOrder(int OrderID)
        {
            Int32 index = _RackOrderBS.Find("OrderID", OrderID);
            try
            {
                if (index >= 0)
                {
                    _RackOrderBS.Position = index;
                    this.numPartQty.MaxValue = this.numPartQty.Value;
                    LoadMedia(OrderID);
                }



            }
            catch (Exception exc)
            {
                string errorMsg = "Error Moving to Selected Rack Order Record.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }


        }

        public PartRacking()
        {
            InitializeComponent();
        }

        private void btnCLose_Click(object sender, EventArgs e)
        {
            try
            {
                using (new UsingWaitCursor(this))
                {

                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error saving racking data.", exc);
            }
        }


        private void numPrintQty_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
        
            // Labels use a different default printer than other reports.
            var labelPrinterName = !string.IsNullOrEmpty(UserSettings.Default.ShippingLabelPrinterName)
                ? UserSettings.Default.ShippingLabelPrinterName
                : PrinterUtilities.SelectPrinterNameDialog(Utilities.PrinterType.Label);

            var rptPartRacking = new PartRackingLabelReport();
            rptPartRacking.LabelCount = Convert.ToInt32(numPrintQty.Value);
            OrdersDataSet.RackOrdersRow row = _rackDT.FindByOrderID(Convert.ToInt32(cboOrder.SelectedItem.DataValue));

            //update value from form
            row.SetField<int>("PartQuantity", (Int32)numPartQty.Value);

            rptPartRacking.Order = row;

            if (cbPrintPreview.Checked){rptPartRacking.DisplayReport();}

            rptPartRacking.PrintLabel(labelPrinterName);
        }

        private void PartRacking_Load(object sender, EventArgs e)
        {
            try
            {
                this.Width = 550;
                this.Height = 550;
                this._rackDT = new OrdersDataSet.RackOrdersDataTable();
                this._mediaDT = new OrdersDataSet.MediaDataTable();

                LoadData();

                moveToOrder(_userSelectedWO);
                int OrdItem = cboOrder.Items.ValueList.FindString(_userSelectedWO.ToString());
                if(OrdItem > 0)
                    cboOrder.SelectedIndex = OrdItem;
            }
            catch (Exception exc)
            {
                string errorMsg = "Error loading part check in dialog.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }



        private void cboOrder_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if(this.cboOrder.SelectedText != "")
                { 
                    //need to set the Max Value high so selected record can set value if higher that previously set.
                    this.numPartQty.MaxValue = 100000; 
                    moveToOrder(Convert.ToInt32(this.cboOrder.SelectedText));
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error Selecting Rack Order.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }

        }
    }
}
