using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Data.Order;
using DWOS.Data.Utilities;
using DWOS.Shared;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;
using DWOS.UI.Utilities.PartMarking;
using Infragistics.Win;
using NLog;

namespace DWOS.UI.PartMarking
{
    public partial class PartMarkingProcessing : UserControl
    {
        #region Fields

        private readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private OrdersDataSet.OrderRow _order;
        private PartsDataset.PartRow _part;
        private IPartMarkDevice _partMarker;
        private CustomerSummaryTableAdapter _taCustomers = new CustomerSummaryTableAdapter();
        private int _lastLineCount = -1;

        #endregion

        #region Properties

        public int SelectedOrder { get; set; }

        private PartMarkFontSize SelectedFontSize =>
            (cboFont.SelectedItem?.DataValue as PartMarkFontSize?) ?? PartMarkFontSize.Medium;

        private MultiStroke SelectedMultiStroke =>
            (MultiStroke) Enum.Parse(typeof(MultiStroke), this.cboMultiStroke.Text);

        private int CurrentLineCount
        {
            get
            {
                if(!string.IsNullOrEmpty(txtLine4.Text))
                    return 4;
                if (!string.IsNullOrEmpty(txtLine3.Text))
                    return 3;
                if (!string.IsNullOrEmpty(txtLine2.Text))
                    return 2;
                
                return 1;
            }
        }
        
        #endregion

        #region Methods

        public PartMarkingProcessing()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            //fill orders
            using(var taOrder = new OrderTableAdapter())
            {
                var dtOrders = new OrdersDataSet.OrderDataTable();
                taOrder.FillByStatus(dtOrders, Settings.Default.OrderStatusOpen, new DateTime(2000, 1, 1));

                this.cboWorkOrder.DataSource = dtOrders.DefaultView;
                this.cboWorkOrder.DisplayMember = dtOrders.OrderIDColumn.ColumnName;
                this.cboWorkOrder.ValueMember = dtOrders.OrderIDColumn.ColumnName;
            }

            var selectedStroke = cboMultiStroke.FindItemByValue<string>(v => v == Settings.Default.PartMarkSelectedStroke.ToString());
            if (selectedStroke != null)
                cboMultiStroke.SelectedItem = selectedStroke;
        }

        private void LoadOrder(OrdersDataSet.OrderRow order)
        {
            try
            {
                ResetMessage();

                if(order == null)
                {
                    this._order = null;
                    return;
                }

                this._order = order;

                this.txtCustomerName.Text = this._taCustomers.GetCustomerName(order.CustomerID);
                this.txtCustomerWO.Text = order.IsCustomerWONull() ? "None" : order.CustomerWO;
                this.lblUserStatus.Text = "Order " + this._order.OrderID + " loaded.";

                using(var taPart = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
                {
                    var partDT = new PartsDataset.PartDataTable();
                    taPart.FillByPartID(partDT, this._order.PartID);

                    this._part = partDT[0];
                    this.txtPartName.Text = this._part.Name;
                    this.txtPartRev.Text = this._part.IsRevisionNull() ? "None" : this._part.Revision;

                    LoadPartMessage();
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error loading order.";
                Logger.Error(exc, errorMsg);
            }
        }

        private void LoadPartMessage()
        {
            ResetMessage();

            if(this._order == null)
                return;

            using(var taOPM = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderPartMarkTableAdapter())
            {
                var orderPMTable = new OrderProcessingDataSet.OrderPartMarkDataTable();
                taOPM.Fill(orderPMTable, this._order.OrderID);

                OrderProcessingDataSet.OrderPartMarkRow orderPMRow = orderPMTable.FirstOrDefault();

                //if there is a order template then use that
                if(orderPMRow != null)
                {
                    if(!orderPMRow.IsLine1Null())
                        this.txtLine1.Text = Interperter.Interpert(orderPMRow.Line1, this._part, this._order);
                    if(!orderPMRow.IsLine2Null())
                        this.txtLine2.Text = Interperter.Interpert(orderPMRow.Line2, this._part, this._order);
                    if(!orderPMRow.IsLine3Null())
                        this.txtLine3.Text = Interperter.Interpert(orderPMRow.Line3, this._part, this._order);
                    if(!orderPMRow.IsLine4Null())
                        this.txtLine4.Text = Interperter.Interpert(orderPMRow.Line4, this._part, this._order);
                }
                else
                {
                    //else if there is a part mark template then use that
                    using(var taPM = new PartMarkingTableAdapter())
                    {
                        var pmDT = taPM.GetDataByPart(_order.PartID);

                        if(pmDT.Count > 0)
                        {
                            if(!pmDT[0].IsDef1Null())
                                this.txtLine1.Text = Interperter.Interpert(pmDT[0].Def1, this._part, this._order);
                            if(!pmDT[0].IsDef2Null())
                                this.txtLine2.Text = Interperter.Interpert(pmDT[0].Def2, this._part, this._order);
                            if(!pmDT[0].IsDef3Null())
                                this.txtLine3.Text = Interperter.Interpert(pmDT[0].Def3, this._part, this._order);
                            if(!pmDT[0].IsDef4Null())
                                this.txtLine4.Text = Interperter.Interpert(pmDT[0].Def4, this._part, this._order);
                        }
                    }
                }
            }
        }

        public void SendMessagesToPartMarker()
        {
            try
            {
                if (_partMarker == null)
                {
                    MessageBoxUtilities.ShowMessageBoxWarn("Please close Part Marking and try again.", "Part Marking");
                    return;
                }

                Logger.Info(_order != null ? $"Sending part marking data for WO {_order.OrderID}" : "Sending part marking data for unknown WO.");

                //Update Admin to not overflow max length
                if (txtSendMessage.Text != null && txtSendMessage.Text.Split(Environment.NewLine).Length > 10)
                    txtSendMessage.Clear();
                if (txtMessageRec.Text != null && txtMessageRec.Text.Split(Environment.NewLine).Length > 10)
                    txtMessageRec.Clear();

                this._partMarker.ClearBuffer();

                int lineCount = 0;

                if(!string.IsNullOrEmpty(this.txtLine4.Text))
                    lineCount = 4;
                else if(!string.IsNullOrEmpty(this.txtLine3.Text))
                    lineCount = 3;
                else if(!string.IsNullOrEmpty(this.txtLine2.Text))
                    lineCount = 2;
                else if(!string.IsNullOrEmpty(this.txtLine1.Text))
                    lineCount = 1;

                var msgs = new string[lineCount];

                if(lineCount >= 1)
                    msgs[0] = this.txtLine1.Text ?? string.Empty;
                if(lineCount >= 2)
                    msgs[1] = this.txtLine2.Text ?? string.Empty;
                if(lineCount >= 3)
                    msgs[2] = this.txtLine3.Text ?? string.Empty;
                if(lineCount >= 4)
                    msgs[3] = this.txtLine4.Text ?? string.Empty;

                this._partMarker.SetFont(lineCount, this.SelectedMultiStroke, this.SelectedFontSize);
                this._partMarker.WriteText(msgs);

                this.cboWorkOrder.Focus();

                OnMessageSent("-- Send Completed --");
            }
            catch(Exception exc)
            {
                HandlePartMarkException(exc, "Error sending message to part marker.");
            }
        }

        public void SaveData()
        {
            try
            {
                if (this._order == null)
                {
                    return;
                }

                //update orders part mark information
                using(var taOrderPM = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderPartMarkTableAdapter())
                {
                    var orderPartMark = new OrderProcessingDataSet.OrderPartMarkDataTable();
                    taOrderPM.Fill(orderPartMark, _order.OrderID);

                    var orderPartMarkRow = orderPartMark.FirstOrDefault();

                    if (orderPartMarkRow == null)
                    {
                        orderPartMarkRow = orderPartMark.NewOrderPartMarkRow();
                        orderPartMarkRow.OrderID = _order.OrderID;
                    }

                    orderPartMarkRow.Line1 = this.txtLine1.Text;
                    orderPartMarkRow.Line2 = this.txtLine2.Text;
                    orderPartMarkRow.Line3 = this.txtLine3.Text;
                    orderPartMarkRow.Line4 = this.txtLine4.Text;
                    orderPartMarkRow.PartMarkedDate = DateTime.Now;

                    taOrderPM.Update(orderPartMarkRow);
                }

                if(this._order.WorkStatus ==  ApplicationSettings.Current.WorkStatusPartMarking)
                {
                    var workStatus = OrderUtilities.WorkStatusAfterPartMark(_order.RequireCoc);
                    var location = OrderUtilities.LocationAfterPartMark(_order.RequireCoc);

                    using(var taOrderSummary = new Data.Datasets.OrderProcessingDataSetTableAdapters.OrderSummaryTableAdapter())
                    {
                        taOrderSummary.UpdateWorkStatus(workStatus, _order.OrderID);
                        taOrderSummary.UpdateOrderLocation(location, _order.OrderID); 
                    }

                    this.lblUserStatus.Text = $"Order {this._order.OrderID} moved to {location} for {workStatus}.";
                    OrderHistoryDataSet.UpdateOrderHistory(this._order.OrderID, "Part Marking", $"Order part marked and moved to {location} for {workStatus}.", SecurityManager.Current.UserName);
                    TimeCollectionUtilities.StopAllOrderTimers(_order.OrderID);
                }
                else
                {
                    OrderHistoryDataSet.UpdateOrderHistory(this._order.OrderID, "Part Marking", "Order part marked, but was not in the correct department.", SecurityManager.Current.UserName);
                    this.lblUserStatus.Text = "Order " + this._order.OrderID + " loaded, but work status was not updated.";
                }
            }
            catch(Exception exc)
            {
                Logger.Error(exc, "Error saving part marking data.");
            }
        }

        private void ResetMessage()
        {
            this.txtLine1.ResetText();
            this.txtLine2.ResetText();
            this.txtLine3.ResetText();
            this.txtLine4.ResetText();
        }

        public void CloseConnection()
        {
            try
            {
                if (this._partMarker != null)
                {
                    this._partMarker.Close();
                    PartMarkSettingsFactory.Save(_partMarker.Settings);
                }

                Settings.Default.PartMarkSelectedFont = this.SelectedFontSize.ToString();
                Settings.Default.PartMarkSelectedStroke = this.SelectedMultiStroke.ToString();
            }
            catch(Exception exc)
            {
                HandlePartMarkException(exc, "Error going to keyboard mode on part marking close.");
            }
        }

        private void DisposeMe()
        {
            this._taCustomers = null;
            this._order = null;
            this._part = null;
        }

        public void ToggleDebugMode()
        {
            try
            {
                ultraTabControl1.Tabs[1].Visible = !ultraTabControl1.Tabs[1].Visible;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting tab visibility.");
            }
        }

        private void OnMessageSent(string message)
        {
            if(this.InvokeRequired)
                this.BeginInvoke(new Action <string>(OnMessageSent), message);
            else
                txtSendMessage.AppendText(message + Environment.NewLine);
        }

        private void OnMessageReceived(string message, bool isError)
        {
            if(this.InvokeRequired)
                this.BeginInvoke(new Action <string, bool>(OnMessageReceived), message, isError);
            else
                this.txtMessageRec.AppendText(message + Environment.NewLine);
        }

        private void UpdateAvailableFonts()
        {
            try
            {
                //if line count changed then reload available font sizes
                if (_partMarker?.Settings == null || this.CurrentLineCount == _lastLineCount)
                {
                    return;
                }

                var availableFontSizes = new List<PartMarkFontSize>();
                _lastLineCount = this.CurrentLineCount;

                picLine1.Appearance.AlphaLevel = _lastLineCount >= 1 ? (short)0 : (short)50;
                picLine2.Appearance.AlphaLevel = _lastLineCount >= 2 ? (short)0 : (short)50;
                picLine3.Appearance.AlphaLevel = _lastLineCount >= 3 ? (short)0 : (short)50;
                picLine4.Appearance.AlphaLevel = _lastLineCount >= 4 ? (short)0 : (short)50;

                foreach (var fs in _partMarker.Settings.FontSizes)
                {
                    if (!availableFontSizes.Contains(fs.FontSize) && fs.Matrices.Any(pm => pm.NumberOfLines == _lastLineCount))
                        availableFontSizes.Add(fs.FontSize);
                }

                cboFont.Items.Clear();

                foreach (var availableFontSize in availableFontSizes)
                    cboFont.Items.Add(availableFontSize, availableFontSize.ToString());

                var selectedFont = cboFont.FindItemByValue<PartMarkFontSize>(v => v.ToString() == Settings.Default.PartMarkSelectedFont);
                if (selectedFont != null)
                    cboFont.SelectedItem = selectedFont;
                else if(cboFont.Items.Count > 0)
                    cboFont.SelectedIndex = 0;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating font size list.");
            }
        }

        private void HandlePartMarkException(Exception exc, string errorMsg)
        {
            if (exc is IOException && exc.Message.Contains("does not exist"))
            {
                Logger.Warn(exc, errorMsg);

                MessageBoxUtilities.ShowMessageBoxError(
                    $"{errorMsg}\nPrinter is not connected.",
                    "Part Marking");

                return;
            }

            if (exc is UnauthorizedAccessException)
            {
                Logger.Warn(exc, errorMsg);

                MessageBoxUtilities.ShowMessageBoxError(
                    $"{errorMsg}\nUnable to access printer.",
                    "Part Marking");

                return;
            }

            ErrorMessageBox.ShowDialog(errorMsg, exc);
        }

        private void LoadPartMarker()
        {
            _partMarker = PartMarkPrinterFactory.NewPrinter(PartMarkSettingsFactory.Load());
            _partMarker.ReceivedMessages += OnMessageReceived;
            _partMarker.SentMessages += OnMessageSent;
            try
            {
                _partMarker.Open();
            }
            catch (Exception)
            {
                _partMarker = null;
                throw;
            }
        }

        #endregion

        #region Events

        private void PartMarkingProcessing_Load(object sender, EventArgs e)
        {
            try
            {
                if(DesignMode)
                    return;

                using(new UsingWaitCursor())
                {
                    LoadPartMarker();

                    LoadData();
                    UpdateAvailableFonts();

                    if(this.SelectedOrder > 0)
                    {
                        ValueListItem item = this.cboWorkOrder.FindItemByValue <int>(i => i == this.SelectedOrder);

                        if(item != null)
                            this.cboWorkOrder.SelectedItem = item;
                    }
                }

                this.cboWorkOrder.Focus();
            }
            catch(Exception exc)
            {
                HandlePartMarkException(exc, "Error opening part marking");
            }
        }

        private void cboWorkOrder_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                ResetMessage();
                this._order = null;

                this.txtPartName.ResetText();
                this.txtCustomerName.ResetText();
                this.txtCustomerWO.ResetText();
                this.txtPartRev.ResetText();

                if(this.cboWorkOrder.SelectedItem != null)
                {
                    var order = ((DataRowView) this.cboWorkOrder.SelectedItem.ListObject).Row as OrdersDataSet.OrderRow;

                    if(order != null)
                    {
                        if(order.WorkStatus !=  ApplicationSettings.Current.WorkStatusPartMarking)
                            MessageBoxUtilities.ShowMessageBoxWarn("Order " + order.OrderID + " work status is not currently set to " +  ApplicationSettings.Current.WorkStatusPartMarking + ".", "Incorrect Order Status");

                        LoadOrder(order);
                        this.cboWorkOrder.Appearance.BorderColor = Color.Green;
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error on work order selection change.";
                Logger.Error(exc, errorMsg);
            }
        }

        private void cboWorkOrder_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.cboWorkOrder.Appearance.BorderColor = this.cboWorkOrder.SelectedItem == null ? Color.Red : (((OrdersDataSet.OrderRow) ((DataRowView) this.cboWorkOrder.SelectedItem.ListObject).Row).WorkStatus ==  ApplicationSettings.Current.WorkStatusPartMarking ? Color.Green : Color.Yellow);

                if(this.cboWorkOrder.SelectedItem == null)
                    ResetMessage();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error changing selected order.";
                Logger.Error(exc, errorMsg);
                ResetMessage();
            }
        }

        private void txtLine_TextChanged(object sender, EventArgs e)
        {
            UpdateAvailableFonts();
        }

        #endregion
    }
}