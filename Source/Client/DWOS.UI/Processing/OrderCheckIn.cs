using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Shared;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;
using Infragistics.Win;
using NLog;

namespace DWOS.UI
{
    public partial class OrderCheckIn : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private OrderProcessingDataSet.MediaDataTable _mediaDT;
        private OrderProcessingDataSet.OrderWorkStatusSummaryDataTable _orderSummaryDT;
        private int _userSelectedWO = -1;

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
                if(this.cboOrder.SelectedItem != null)
                    return Convert.ToInt32(this.cboOrder.SelectedItem.DataValue);

                return -1;
            }
            set { this._userSelectedWO = value; }
        }

        #endregion

        #region Methods

        public OrderCheckIn()
        {
            InitializeComponent();

            new SecurityFormWatcher(this, this.btnCancel);
        }

        private void LoadData()
        {
            try
            {
                this.txtDepartment.Text = Settings.Default.CurrentDepartment;

                //load all orders that are changing departments
                this.taOrderWorkStatusSummary.FillBy(this._orderSummaryDT,  ApplicationSettings.Current.WorkStatusChangingDepartment);
                this.cboOrder.DataSource = this._orderSummaryDT;
                this.cboOrder.DataBind();

                if(this._userSelectedWO > 0)
                {
                    ValueListItem item = this.cboOrder.FindItemByValue <int>(i => i == this._userSelectedWO);

                    if(item != null)
                        this.cboOrder.SelectedItem = item;
                }
            }
            catch(Exception exc)
            {
                var errorMsg = "Error loading data.";
                _log.Error(exc, errorMsg);
            }
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if(this.cboOrder.SelectedItem == null)
                    MessageBoxUtilities.ShowMessageBoxOKCancel("No valid order selected to check in.", "Order Check In");
                else
                {
                    //get selected work order
                    var or = ((DataRowView) this.cboOrder.SelectedItem.ListObject).Row as OrderProcessingDataSet.OrderWorkStatusSummaryRow;
                    _userSelectedWO = or.OrderID;

                    var checkIn = new OrderCheckInController(_userSelectedWO);
                    var checkInValid = checkIn.IsValid(Settings.Default.CurrentDepartment, Settings.Default.CurrentLine);

                    if(!checkInValid.Response)
                        MessageBoxUtilities.ShowMessageBoxWarn(checkInValid.Description, "Order Check In");
                    else
                    {
                        var checkInResponse = checkIn.CheckIn(Settings.Default.CurrentDepartment, SecurityManager.Current.UserID);

                        if(checkInResponse.Response)
                            this.Close();
                        else
                            MessageBoxUtilities.ShowMessageBoxWarn(checkInResponse.Description, "Order Check In");
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving part check in.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void PartCheckIn_Load(object sender, EventArgs e)
        {
            try
            {
                this._orderSummaryDT = new OrderProcessingDataSet.OrderWorkStatusSummaryDataTable();
                this._mediaDT = new OrderProcessingDataSet.MediaDataTable();

                LoadData();
                this.cboOrder.Focus();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error loading part check in dialog.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e) { Close(); }

        private void cboOrder_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.cboOrder.Text = this.cboOrder.Text.Replace("~", "");
                
                if(this.cboOrder.SelectedItem != null)
                {
                    var or = ((DataRowView) this.cboOrder.SelectedItem.ListObject).Row as OrderProcessingDataSet.OrderWorkStatusSummaryRow;

                    //dispose current image first
                    if(this.picPartImage.Image != null)
                    {
                        ((Image) this.picPartImage.Image).Dispose();
                        this.picPartImage.Image = null;
                    }

                    //get image from media id
                    this.taMedia.FillDefaultWithoutMedia(this._mediaDT, or.PartID);
                    var medias = this._mediaDT.ToArray();

                    if(medias.Length > 0)
                        this.picPartImage.Image = MediaUtilities.GetThumbnail(medias[0].MediaID, medias[0].FileExtension);

                    //if image not set
                    if(this.picPartImage.Image == null)
                        this.picPartImage.Image = Properties.Resources.NoImage;
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error displaying part image.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void cboOrder_TextChanged(object sender, EventArgs e)
        {
            this.cboOrder.Appearance.BorderColor = this.cboOrder.SelectedItem == null ? Color.Red : Color.Green;

            try
            {
                if(this.cboOrder.SelectedItem == null)
                {
                    if(this.picPartImage.Image != null)
                    {
                        ((Image) this.picPartImage.Image).Dispose();
                        this.picPartImage.Image = null;
                    }
                }
            }
            catch(Exception exc)
            {
                _log.Warn(exc, "Error removing selected picture.");
            }
        }

        #endregion
    }
}