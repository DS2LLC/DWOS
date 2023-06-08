using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Shared;
using DWOS.UI.Properties;
using DWOS.UI.QA;
using DWOS.UI.Utilities;
using DWOS.Utilities.Validation;
using DWOS.Data;

namespace DWOS.UI.Sales
{
    public partial class OrderProcessingInfo : DataPanel
    {
        #region Properties

        public OrdersDataSet Dataset
        {
            get { return base._dataset as OrdersDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.Order.OrderIDColumn.ColumnName; }
        }

        public bool ViewOnly { get; set; }

        #endregion

        #region Methods

        public OrderProcessingInfo()
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                // Setup dependency injection to prevent design-time errors.
                DesignTime.DesignTimeUtilities.SetupDependencyInjection();
            }

            InitializeComponent();
        }

        public void LoadData(OrdersDataSet dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.Order.TableName;

            base.BindValue(this.cboLocation, Dataset.Order.CurrentLocationColumn.ColumnName, true);
            base.BindValue(this.cboWorkStatus, Dataset.Order.WorkStatusColumn.ColumnName, true);
            base.BindValue(this.cboProcessingLine, Dataset.Order.CurrentLineColumn.ColumnName, true);

            base.BindList(this.cboLocation, Dataset.d_Department, Dataset.d_Department.DepartmentIDColumn.ColumnName, Dataset.d_Department.DepartmentIDColumn.ColumnName);
            base.BindList(this.cboWorkStatus, Dataset.d_WorkStatus, Dataset.d_WorkStatus.WorkStatusIDColumn.ColumnName, Dataset.d_WorkStatus.WorkStatusIDColumn.ColumnName);
            base.BindList(this.cboProcessingLine, Dataset.ProcessingLine, Dataset.ProcessingLine.ProcessingLineIDColumn.ColumnName, Dataset.ProcessingLine.NameColumn.ColumnName);

            this.processEditor.LoadData(dataset, false);

            if(!SecurityManager.Current.IsInRole("OrderProcess.Edit") || ViewOnly)
            {
                cboLocation.Enabled = false;
                cboWorkStatus.Enabled = false;

                this.processEditor.ViewOnly = true;
            }

            if (ViewOnly)
            {
                cboProcessingLine.Enabled = false;
            }

            if (!ApplicationSettings.Current.MultipleLinesEnabled)
            {
                lblProcessingLine.Visible = false;
                cboProcessingLine.Visible = false;
            }

            base._panelLoaded = true;
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider) { manager.Add(new ImageDisplayValidator(new EndDateValidator(this.processEditor, this), errProvider)); }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            //Load data for this node
            LoadNode(Convert.ToInt32(id));
        }

        private void LoadNode(int orderID)
        {
            try
            {
                var order = Dataset.Order.FindByOrderID(orderID);

                if(order == null)
                    return;

                using(var taPS = new ProcessAliasSummaryTableAdapter {ClearBeforeFill = false})
                    taPS.FillByOrder(Dataset.ProcessAliasSummary, order.OrderID);

                this.processEditor.LoadProcesses(order.OrderID);
                this.processEditor.ViewOnly = order.Status != Settings.Default.OrderStatusOpen ||
                    !SecurityManager.Current.IsInRole("OrderProcess.Edit") ||
                    ViewOnly;

                UpdateProcessingLineFilter();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error loading order processing info";
                _log.Error(exc, errorMsg);
            }
        }

        private void UpdateProcessingLineFilter(bool updateValue = false)
        {
            var currentStatus = cboWorkStatus.SelectedItem?.DataValue as string;
            var currentLineId = cboProcessingLine.SelectedItem?.DataValue as int?;
            var currentOrder = CurrentRecord as OrdersDataSet.OrderRow;

            string department;

            if (currentStatus == ApplicationSettings.Current.WorkStatusChangingDepartment && currentOrder != null)
            {
                // Department of next process -or- the department currently listed
                department = currentOrder.GetOrderProcessesRows().OrderBy(op => op.StepOrder).FirstOrDefault(op => op.IsEndDateNull())?.Department ??
                             cboLocation.SelectedItem?.DataValue as string;
            }
            else
            {
                department = cboLocation.SelectedItem?.DataValue as string;
            }

            string processingLineFilter;
            if (string.IsNullOrWhiteSpace(department))
            {
                // Show no lines - should not happen under normal circumstances
                processingLineFilter = "1 = 0";
            }
            else if (!currentLineId.HasValue || updateValue)
            {
                // Show lines for the current department
                processingLineFilter = $"DepartmentID IS NULL OR DepartmentID = '{department}'";
            }
            else
            {
                // Show current line and lines for the current department
                processingLineFilter = $"DepartmentID IS NULL OR DepartmentID = '{department}' OR ProcessingLineID = {currentLineId}";
            }

            UpdateFilter(cboProcessingLine, processingLineFilter);

            if (updateValue)
            {
                cboProcessingLine.DataBindings[0].WriteValue();
            }
        }

        #endregion

        #region Events

        private void OrderProcessingInfo_Resize(object sender, EventArgs e)
        {
            if((grpData.Width > Width) || (grpData.Height > Height))
                grpData.Dock = DockStyle.None;
            else
                grpData.Dock = DockStyle.Fill;
        }

        private void cboProcessingLine_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                var currentOrder = CurrentRecord as OrdersDataSet.OrderRow;

                if (currentOrder == null)
                {
                    return;
                }

                currentOrder.SetCurrentLineNull();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error deleting current line from order.");
            }
        }

        private void cboLocation_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (_recordLoading)
                {
                    return;
                }

                UpdateProcessingLineFilter(true);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing department for order.");
            }
        }

        private void cboWorkStatus_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (_recordLoading)
                {
                    return;
                }

                UpdateProcessingLineFilter(true);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing department for order.");
            }
        }

        #endregion

        private class EndDateValidator : ControlValidatorBase
        {
            #region Fields

            private OrderProcessingInfo _orderInfo;
            private List <int> _ordersAlreadyReminded = new List <int>();
            private Dictionary <int, bool> _requiresPO = new Dictionary <int, bool>();

            #endregion

            #region Methods

            public EndDateValidator(OrderProcessEditor control, OrderProcessingInfo orderInfo) : base(control) { this._orderInfo = orderInfo; }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                try
                {
                    if(Control != null && Control.Enabled)
                    {
                        var order = this._orderInfo.CurrentRecord as OrdersDataSet.OrderRow;

                        //if it is a new order and haven't asked before
                        if(order != null && !order.IsEstShipDateNull() && order.GetOrderProcessesRows().Length > 0)
                        {
                            var maxDate = DateTime.MinValue;

                            foreach(var op in order.GetOrderProcessesRows())
                            {
                                if(!op.IsEstEndDateNull() && op.EstEndDate > maxDate)
                                    maxDate = op.EstEndDate;
                            }

                            if(maxDate > DateTime.MinValue && !this._ordersAlreadyReminded.Contains(order.OrderID))
                            {
                                this._ordersAlreadyReminded.Add(order.OrderID);

                                //if last process date is greater then est ship date
                                if (maxDate.Date > order.EstShipDate.Date)
                                {
                                    var errorMsg = $"The last process date '{maxDate:d}' " +
                                        $"is after the estimated ship date of " +
                                        $"'{order.EstShipDate:d}'.\n\n" +
                                        $"Please update the order's estimated ship date.";

                                    MessageBoxUtilities.ShowMessageBoxWarn(errorMsg, "Est. Ship Date Exceeded");
                                }
                            }
                        }
                    }

                    //passed
                    e.Cancel = false;
                    FireAfterValidation(true, "");
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error determining if there is a scanned PO required.";
                    ErrorMessageBox.ShowDialog(errorMsg, exc);
                }
            }

            public override void Dispose()
            {
                this._requiresPO = null;
                this._ordersAlreadyReminded = null;
                this._orderInfo = null;
                base.Dispose();
            }

            #endregion
        }


    }
}