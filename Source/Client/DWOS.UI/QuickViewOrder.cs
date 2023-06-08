using System;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.UI.Reports;
using DWOS.UI.Utilities;
using DWOS.Utilities.Validation;
using NLog;

namespace DWOS.UI
{
    public partial class QuickViewOrder: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        protected ValidatorManager _validators = new ValidatorManager();

        #endregion

        #region Properties

        public int OrderID { get; set; }

        #endregion

        #region Methods

        public QuickViewOrder()
        {
            this.InitializeComponent();
        }

        private void LoadData()
        {
            this.dsOrders.EnforceConstraints = false;
            this.dsOrders.PriceUnit.BeginLoadData();
            this.dsOrders.CustomerSummary.BeginLoadData();
            this.dsOrders.d_OrderStatus.BeginLoadData();
            this.dsOrders.d_Priority.BeginLoadData();
            this.dsOrders.UserSummary.BeginLoadData();
            this.dsOrders.CustomerShippingSummary.BeginLoadData();
            this.dsOrders.CustomerAddress.BeginLoadData();
            this.dsOrders.PartSummary.BeginLoadData();
            this.dsOrders.ProcessingLine.BeginLoadData();
            this.dsOrders.Media.BeginLoadData();
            this.dsOrders.Order_DocumentLink.BeginLoadData();
            this.dsOrders.OrderSerialNumber.BeginLoadData();
            this.dsOrders.WorkDescription.BeginLoadData();
            this.dsOrders.OrderWorkDescription.BeginLoadData();

            this.taPriceUnit.Fill(this.dsOrders.PriceUnit);
            this.taCustomerSummary.FillByOrder(this.dsOrders.CustomerSummary, this.OrderID);
            this.taOrderStatus.Fill(this.dsOrders.d_OrderStatus);
            this.taPriority.Fill(this.dsOrders.d_Priority);
            this.taUserSummary.Fill(this.dsOrders.UserSummary);
            this.taCustomerShippingSummary.FillByOrder(this.dsOrders.CustomerShippingSummary, this.OrderID);
            this.taCustomerAddress.FillByOrder(this.dsOrders.CustomerAddress, this.OrderID);
            this.taPartSummary.FillByOrder(this.dsOrders.PartSummary, this.OrderID);
            this.taProcessingLine.Fill(this.dsOrders.ProcessingLine);
            this.taMedia.FillByOrder(this.dsOrders.Media, this.OrderID);
            this.taOrder.FillByOrderID(this.dsOrders.Order, this.OrderID);
        	this.taOrderFees.FillByOrder(this.dsOrders.OrderFees, this.OrderID);
            this.taOrderProcesses.FillBy(this.dsOrders.OrderProcesses, this.OrderID);
            this.taOrder_Media.FillByOrder(this.dsOrders.Order_Media, this.OrderID);
            this.taOrderDocumentLink.FillByOrder(this.dsOrders.Order_DocumentLink, this.OrderID);
            this.taOrderFeeType.Fill(this.dsOrders.OrderFeeType);

            using(var taDepts = new d_DepartmentTableAdapter())
                taDepts.Fill(this.dsOrders.d_Department);
            using(var taWS = new d_WorkStatusTableAdapter())
                taWS.Fill(this.dsOrders.d_WorkStatus);

            this.taOrderSerialNumber.FillByOrder(this.dsOrders.OrderSerialNumber, this.OrderID);
            this.taOrderNote.FillByOrder(this.dsOrders.OrderNote, this.OrderID);

            this.taWorkDescription.Fill(this.dsOrders.WorkDescription);
            this.taOrderWorkDescription.FillByOrder(this.dsOrders.OrderWorkDescription, this.OrderID);

            this.dsOrders.PriceUnit.EndLoadData();
            this.dsOrders.CustomerSummary.EndLoadData();
            this.dsOrders.d_OrderStatus.EndLoadData();
            this.dsOrders.d_Priority.EndLoadData();
            this.dsOrders.UserSummary.EndLoadData();
            this.dsOrders.CustomerShippingSummary.EndLoadData();
            this.dsOrders.CustomerAddress.EndLoadData();
            this.dsOrders.PartSummary.EndLoadData();
            this.dsOrders.ProcessingLine.EndLoadData();
            this.dsOrders.Media.EndLoadData();
            this.dsOrders.Order_DocumentLink.EndLoadData();
            this.dsOrders.OrderSerialNumber.EndLoadData();
            this.dsOrders.WorkDescription.EndLoadData();
            this.dsOrders.OrderWorkDescription.EndLoadData();

            //Load Basic Order Info
            this.pnlOrderInfo.IsQuickView = true;
            this.pnlOrderInfo.ViewOnly = true;
            this.pnlOrderInfo.LoadData(this.dsOrders);
            this.pnlOrderInfo.ClearPartSummaryBeforeFill = true;
            
            //Load Order Processing Info
            this.pnlOrderProcessingInfo.ViewOnly = true;
            this.pnlOrderProcessingInfo.LoadData(this.dsOrders);

            // Load serial numbers
            pnlSerialNumbers.ViewOnly = true;
            pnlSerialNumbers.LoadData(dsOrders);
            ultraTabControl1.Tabs["SerialNumbers"].Visible =
                FieldUtilities.IsFieldEnabled("Order", "Serial Number")
                && ApplicationSettings.Current.SerialNumberEditorType == SerialNumberType.Advanced;

            // Load notes
            notesQuickViewPanel.LoadData(dsOrders);

            // Load validators - show/hide fields
            pnlOrderInfo.AddValidators(_validators, errorProvider);
        }

        private void UpdateOrderInfoAutoScrollSize()
        {
            ultraTabPageControl1.AutoScrollMinSize = pnlOrderInfo.MinimumSize;
        }

        #endregion

        #region Events

        private void QuickViewPart_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                this.LoadData();
                
				this.pnlOrderInfo.MoveToRecord(this.OrderID);
                this.pnlOrderInfo.Editable = false;
                UpdateOrderInfoAutoScrollSize();
                pnlOrderInfo.MinimumSizeChanged += pnlOrderInfo_MinimumSizeChanged;

				this.pnlOrderProcessingInfo.MoveToRecord(this.OrderID);
                this.pnlOrderProcessingInfo.Editable = false;

                pnlSerialNumbers.MoveToRecord(OrderID);
                pnlSerialNumbers.Editable = false;

                notesQuickViewPanel.MoveToRecord(OrderID);
                notesQuickViewPanel.Editable = false;

                btnPrint.Enabled = SecurityManager.Current.IsInRole("OrderEntry");
            }
            catch(Exception exc)
            {
                _log.Warn("DataSet Errors: " + this.dsOrders.GetDataErrors());
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!SecurityManager.Current.IsInRole("OrderEntry"))
                {
                    return;
                }

                string selectedReport = null;
                bool quickPrint = false;

                using (var cbo = new ComboBoxForm())
                {
                    cbo.chkOption.Visible = true;
                    cbo.chkOption.Text    = "Quick Print";
                    cbo.chkOption.Checked = UserSettings.Default.QuickPrint;
                    string[] reportTypes = new[] { "Work Order Traveler", "Work Order Summary" };


                    cbo.Text = "Reports";
                    foreach(var reportType in reportTypes)
                        cbo.ComboBox.Items.Add(reportType);

                    cbo.ComboBox.SelectedIndex = 0;
                    cbo.FormLabel.Text = "Report Type:";

                    if (cbo.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                    {
                        selectedReport  = cbo.ComboBox.Text;
                        quickPrint      = cbo.chkOption.Checked;
                        UserSettings.Default.QuickPrint = quickPrint;
                        UserSettings.Default.Save();
                    }
                }

                if (selectedReport == "Work Order Traveler")
                {
                    var psr = new WorkOrderTravelerReport(this.dsOrders.Order.FindByOrderID(this.OrderID));
                    if(quickPrint)
                        psr.PrintReport();
                    else
                        psr.DisplayReport();
                }
                else if (selectedReport == "Work Order Summary")
                {
                    using (var psr = new WorkOrderSummaryReport(this.dsOrders.Order.FindByOrderID(this.OrderID)))
                    {
                        if (quickPrint)
                            psr.PrintReport();
                        else
                            psr.DisplayReport();
                    }

                    if (ApplicationSettings.Current.PrintSummariesForRejoinedOrders)
                    {
                        foreach (var rejoinedOrderId in taOrder.GetRejoinedOrderIds(OrderID))
                        {
                            using (var psr = new WorkOrderSummaryReportWrapper(rejoinedOrderId, true))
                            {
                                if (quickPrint)
                                {
                                    psr.PrintReport();
                                }
                                else
                                {
                                    psr.DisplayReport();
                                }
                            }
                        }
                    }

                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error displaying report.";
                _log.Error(exc, errorMsg);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                //Update any changes that may have occurred for media (i.e. docs)
                if (SecurityManager.Current.IsInRole("Documents"))
                {
                    pnlOrderInfo.EndEditing();
                    var result = taManager.UpdateAll(dsOrders);
                }
            }
            catch (Exception exc)
            {
                _log.Warn("DataSet Errors: " + this.dsOrders.GetDataErrors());
                ErrorMessageBox.ShowDialog("Error updating .", exc);
            }

        }

        private void pnlOrderInfo_MinimumSizeChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateOrderInfoAutoScrollSize();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error handling Order Info min. size change." );
            }
        }

        #endregion
    }
}