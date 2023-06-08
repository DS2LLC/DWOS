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
        private OrdersDataSet.OrderSummaryDataTable _orderDT;
        private int _userSelectedWO = -1;

        private string _SelectedDept;

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
                this._orderDT = this.taOrder.GetData();
                DataView dv = this._orderDT.DefaultView;
                dv.RowFilter = $"((CurrentLocation = '{_SelectedDept.ToString()}' ) and (WorkStatus = 'In Process' ))";

 
                this.cboOrder.DataSource = dv.OfType<DataRowView>().Select(dr => dr["OrderID"].ToString()).ToList(); 
                this.cboOrder.DataBind();
                  

                OrdersDataSet.OrderSummaryRow row = (OrdersDataSet.OrderSummaryRow)this.taOrder.GetDataBy(_userSelectedWO).Rows[0];

                _log.Info($"Racking Form Loaded with Order {row.OrderID.ToString()}.");


                this.lbCustomer.Text       = row.CustomerName;
                this.lbPart.Text           = row.PartName;
                this.lbPO.Text             = row.PurchaseOrder.ToString();
                this.numPartQty.Value      = row.PartQuantity;
                this.numPartQty.MaxValue   = row.PartQuantity;
                this.lbDueDate.Text        = row.RequiredDate.ToShortDateString();
                this.tePartNotes.Text      = (row.IsPartNotesNull())?"":row.PartNotes.ToString();
                this.txtOrderNotes.Text    = (row.IsCustomerNotesNull()) ? "":row.CustomerNotes.ToString();

                using (var taMedia = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter())
                {
                    this._mediaDT = taMedia.GetPartsWOMediaByOrder(_userSelectedWO);
                    DWOS.Data.Datasets.OrdersDataSet.MediaRow mr = (DWOS.Data.Datasets.OrdersDataSet.MediaRow)taMedia.GetPartsWOMediaByOrder(_userSelectedWO).Rows[0];
                    //dispose current image first
                    if (this.picPartImage.Image != null)
                    {
                        ((Image)this.picPartImage.Image).Dispose();
                        this.picPartImage.Image = null;
                    }

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
                var errorMsg = "Error loading data.";
                _log.Error(exc, errorMsg);
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
            OrdersDataSet.OrderSummaryRow row = _orderDT.FindByOrderID(Convert.ToInt32(cboOrder.SelectedItem.DataValue));

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
                this._orderDT = new OrdersDataSet.OrderSummaryDataTable();
                this._mediaDT = new OrdersDataSet.MediaDataTable();

                LoadData();
                this.cboOrder.SelectedText = _userSelectedWO.ToString(); 
                this.cboOrder.Focus();
            }
            catch (Exception exc)
            {
                string errorMsg = "Error loading part check in dialog.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }



        private void cboOrder_SelectionChanged(object sender, EventArgs e)
        {
            if(this.cboOrder.SelectedText != "")
                _userSelectedWO = Convert.ToInt32(this.cboOrder.SelectedText);
            LoadData();
        }
    }
}
