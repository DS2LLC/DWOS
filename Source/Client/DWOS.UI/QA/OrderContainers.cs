using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.UI.Reports;
using DWOS.UI.Utilities;
using DWOS.UI.Utilities.Scale;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Interop;

namespace DWOS.UI.QA
{
    public partial class OrderContainers : Form
    {
        #region Fields

        private const string NO_CONTAINER_SELECTED_ERROR = "Please select a container before weighing parts.";
        private const string ERROR_HEADER = "Containers";
        private const decimal MAX_WEIGHT = 999999.99999999M;
        private const int STATUS_FLYOUT_TIME_MS = 2000;
        private const int TOTAL_FLYOUT_TIME_MS = 6000;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly FlyoutManager _flyoutManager;
        private IScale _scale;

        private readonly Lazy<decimal> _partWeightLazy;

        private OrdersDataSet.OrderRow _order;
        private OrdersDataSet _dsOrders;

        public int OrderId { get; set; }

        private bool _isInReworkProcess;
        private bool _reworkedInOutsideProcessing;
        private decimal _tareWeight;
        private readonly GridSettingsPersistence<UltraGridBandSettings> _containerBandSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("OrderContainers_Container", new UltraGridBandSettings());

        private readonly GridSettingsPersistence<UltraGridBandSettings> _itemBandSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("OrderContainers_Item", new UltraGridBandSettings());

        #endregion

        #region Properties

        public bool PrintCurrentLabel
        {
            get
            {
                return chkPrintContainerLabels.Checked && chkPrintWOLabels.Checked;
            }
            set
            {
                chkPrintContainerLabels.Checked = value;
                chkPrintWOLabels.Checked = value;
            }
        }

        private string PrintPromptText
        {
            get
            {
                switch (CurrentLabelTypeRole)
                {
                    case LabelFactory.LabelTypeRole.Default:
                        return "Print Labels:";
                    case LabelFactory.LabelTypeRole.COC:
                        return "Print COC Labels:";
                    case LabelFactory.LabelTypeRole.Rework:
                        return "Print Rework Labels:";
                    case LabelFactory.LabelTypeRole.Hold:
                        return "Print Hold Labels:";
                    case LabelFactory.LabelTypeRole.OutsideProcessing:
                        return "Print Outside Processing Labels:";
                    case LabelFactory.LabelTypeRole.OutsideProcessingRework:
                        return "Print Outside Proc. Rework Labels:";
                    case LabelFactory.LabelTypeRole.ExternalRework:
                        return "Print External Rework Labels:";
                    default:
                        return "Print Unknown Labels:";
                }
            }
        }

        private LabelFactory.LabelType CurrentContainerLabel
        {
            get
            {
                return LabelFactory.GetLabelType(LabelFactory.LabelCategory.Container, CurrentLabelTypeRole)
                    .GetValueOrDefault(LabelFactory.LabelType.Container);
            }
        }

        private LabelFactory.LabelType CurrentOrderLabel
        {
            get
            {
                return LabelFactory.GetLabelType(LabelFactory.LabelCategory.WO, CurrentLabelTypeRole)
                    .GetValueOrDefault(LabelFactory.LabelType.Container);
            }
        }

        private LabelFactory.LabelTypeRole CurrentLabelTypeRole
        {
            get
            {
                LabelFactory.LabelTypeRole returnValue;

                if (_order.Hold)
                {
                    returnValue = LabelFactory.LabelTypeRole.Hold;
                }
                else if (_isInReworkProcess || _order.WorkStatus == ApplicationSettings.Current.WorkStatusPendingReworkPlanning)
                {
                    if (_reworkedInOutsideProcessing)
                    {
                        returnValue = LabelFactory.LabelTypeRole.OutsideProcessingRework;
                    }
                    else
                    {
                        returnValue = LabelFactory.LabelTypeRole.Rework;
                    }
                }
                else if (_order.WorkStatus == ApplicationSettings.Current.WorkStatusFinalInspection ||
                    _order.WorkStatus == ApplicationSettings.Current.WorkStatusShipping)
                {
                    returnValue = LabelFactory.LabelTypeRole.COC;
                }
                else if (_order.OrderType == (int)OrderType.ReworkExt)
                {
                    returnValue = LabelFactory.LabelTypeRole.ExternalRework;
                }
                else if (_order.CurrentLocation == ApplicationSettings.Current.DepartmentOutsideProcessing)
                {
                    returnValue = LabelFactory.LabelTypeRole.OutsideProcessing;
                }
                else
                {
                    returnValue = LabelFactory.LabelTypeRole.Default;
                }

                return returnValue;
            }
        }

        #endregion

        #region Methods

        public OrderContainers()
        {
            InitializeComponent();

            _partWeightLazy = new Lazy<decimal>(() =>
            {
                using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.PartSummaryTableAdapter())
                {
                    return ta.GetPartWeightByOrder(OrderId).GetValueOrDefault();
                }
            });

            _flyoutManager = new FlyoutManager(this)
            {
                FlyoutSize = new Size(300, 100)
            };
        }

        public void LoadData(int orderId)
        {
            OrderId = orderId;
            _dsOrders = new OrdersDataSet() { EnforceConstraints = false };

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderTableAdapter())
            {
                ta.FillByOrderID(_dsOrders.Order, orderId);
            }

            using (var ta = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.OrderContainersTableAdapter())
                ta.FillByOrder(_dsOrders.OrderContainers, orderId);

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderSerialNumberTableAdapter())
            {
                ta.FillByOrder(_dsOrders.OrderSerialNumber, orderId);
            }

            using (var taShipmentPackageType = new Data.Datasets.OrdersDataSetTableAdapters.ShipmentPackageTypeTableAdapter())
            {
                taShipmentPackageType.Fill(_dsOrders.ShipmentPackageType);
            }

            using (var taContainerItem = new Data.Datasets.OrdersDataSetTableAdapters.OrderContainerItemTableAdapter())
            {
                taContainerItem.FillByOrder(_dsOrders.OrderContainerItem, orderId);
            }

            _order = _dsOrders.Order.FirstOrDefault();

            using (var taProcess = new Data.Datasets.OrdersDataSetTableAdapters.OrderProcessesTableAdapter())
            {
                var currentProcess = taProcess.GetCurrentProcess(orderId).FirstOrDefault();
                _isInReworkProcess = currentProcess?.OrderProcessType == (int)OrderProcessType.Rework;
            }

            using (var taCustomer = new Data.Datasets.OrdersDataSetTableAdapters.CustomerSummaryTableAdapter())
            {
                taCustomer.FillByOrder(_dsOrders.CustomerSummary, orderId);
            }

            // Determine value of _reworkedInOutsideProcessing
            using (var taInternalRework = new Data.Datasets.OrdersDataSetTableAdapters.InternalReworkTableAdapter())
            {
                OrdersDataSet.InternalReworkRow internalRework = null;
                taInternalRework.FillByOriginalOrderID(_dsOrders.InternalRework, orderId);
                internalRework = _dsOrders.InternalRework
                    .OrderByDescending(ir => ir.DateCreated)
                    .FirstOrDefault(ir => ir.OriginalOrderID == orderId && ir.Active);

                if (internalRework == null)
                {
                    taInternalRework.FillByReworkOrderID(_dsOrders.InternalRework, orderId);
                    internalRework = _dsOrders.InternalRework
                        .OrderByDescending(ir => ir.DateCreated)
                        .FirstOrDefault(ir => !ir.IsReworkOrderIDNull() && ir.ReworkOrderID == orderId && ir.Active);
                }

                if (internalRework != null)
                {
                    var internalReworkProcessDept = taInternalRework
                        .GetDepartmentForProcessAlias(internalRework.InternalReworkID);

                    _reworkedInOutsideProcessing = internalReworkProcessDept == ApplicationSettings.Current.DepartmentOutsideProcessing;
                }
            }

            bsData.DataSource = _dsOrders.OrderContainers;
            this.grdContainers.DataSource = bsData;
            lblPrintPrompt.Text = PrintPromptText;
        }

        public OrdersDataSet.OrderContainersRow AddContainer()
        {
            const int partsPerContainer = 1;
            var containers = new List<OrdersDataSet.OrderContainersRow>();

            var rowVw = bsData.AddNew() as DataRowView;

            var cr = rowVw.Row as OrdersDataSet.OrderContainersRow;
            cr.OrderID = OrderId;
            cr.PartQuantity = partsPerContainer;
            cr.IsActive = true;
            cr.Weight = partsPerContainer * _partWeightLazy.Value;
            cr.ShipmentPackageTypeID = 1;
            cr.EndEdit();

            // Fix bug where clicking 'OK' after adding a row would
            // not save the row.
            _dsOrders.OrderContainers.AddOrderContainersRow(cr);

            return cr;
        }

        private void SaveData()
        {
            grdContainers.UpdateData();

            using (var taManager = new Data.Datasets.OrdersDataSetTableAdapters.TableAdapterManager())
            {
                taManager.OrderContainersTableAdapter = new Data.Datasets.OrdersDataSetTableAdapters.OrderContainersTableAdapter();
                taManager.OrderContainerItemTableAdapter = new Data.Datasets.OrdersDataSetTableAdapters.OrderContainerItemTableAdapter();
                taManager.UpdateAll(_dsOrders);
            }
        }

        public void PrintLabels()
        {
            if (chkPrintContainerLabels.Checked)
            {
                CreateContainerLabel().PrintReport();
            }

            if (chkPrintWOLabels.Checked)
            {
                CreateOrderLabel().PrintReport();
            }
        }

        private IReport CreateContainerLabel()
        {
            switch (CurrentContainerLabel)
            {
                case LabelFactory.LabelType.Container:
                    return new ContainerLabelReport() { OrderID = OrderId };
                case LabelFactory.LabelType.COCContainer:
                    return new COCContainerLabelReport() { OrderId = OrderId };
                case LabelFactory.LabelType.ReworkContainer:
                    return new ReworkContainerLabelReport(OrderId);
                case LabelFactory.LabelType.HoldContainer:
                    return new HoldContainerLabelReport(OrderId);
                case LabelFactory.LabelType.OutsideProcessingContainer:
                    return new OutsideProcessingContainerLabelReport() { OrderID = OrderId };
                case LabelFactory.LabelType.OutsideProcessingReworkContainer:
                    return new OutsideReworkContainerLabelReport(OrderId);
                case LabelFactory.LabelType.ExternalReworkContainer:
                    return new ExternalReworkContainerLabelReport() { OrderID = OrderId };
                default:
                    return null;
            }
        }

        private IReport CreateOrderLabel()
        {
            switch (CurrentOrderLabel)
            {
                case LabelFactory.LabelType.WO:
                    return new WorkOrderLabelReport() { Order = _order };
                case LabelFactory.LabelType.COC:
                    return new COCLabelReport() { OrderId = OrderId };
                case LabelFactory.LabelType.Rework:
                    return new ReworkLabelReport(_order, ReworkLabelReport.ReportLabelType.Rework);
                case LabelFactory.LabelType.Hold:
                    return new ReworkLabelReport(_order, ReworkLabelReport.ReportLabelType.Hold);
                case LabelFactory.LabelType.OutsideProcessing:
                    return new OutsideProcessingLabelReport(_order);
                case LabelFactory.LabelType.OutsideProcessingRework:
                    return new ReworkLabelReport(_order, ReworkLabelReport.ReportLabelType.OutsideProcessingRework);
                case LabelFactory.LabelType.ExternalRework:
                    return new ExternalReworkLabelReport(_order);
                default:
                    return null;
            }
        }

        private void LoadScale(IScale scale)
        {
            if (this._scale != null)
            {
                this._scale.Dispose();
            }

            this._scale = scale;

            if (this._scale != null)
            {
                this._scale.Open();

                if (_scale.IsOpen)
                {
                    this._scale.ScaleDataReceived += _scale_ScaleDataReceived;
                }
                else
                {
                    var msg = $"Cannot open port { _scale.PortName } for the weight scale. Please change scale settings and try again.";

                    MessageBoxUtilities.ShowMessageBoxWarn(msg, ERROR_HEADER);
                    _scale = null;
                }
            }

            this.btnWeigh.Enabled = this._scale != null;
            this.btnZero.Enabled = this._scale != null;
            this.btnTare.Enabled = this._scale != null;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);

            if (this._scale != null)
            {
                _scale.Dispose();
            }
        }

        private void UpdateContainerWeight(decimal grossWeight)
        {
            Infragistics.Win.UltraWinGrid.UltraGridRow selectedRow = null;
            if (this.grdContainers.Selected.Rows.Count > 0)
            {
                selectedRow = this.grdContainers.Selected.Rows[0];
            }

            OrdersDataSet.OrderContainersRow selectedContainer = null;
            if (selectedRow != null && selectedRow.IsDataRow)
            {
                var dataRowView = selectedRow.ListObject as DataRowView;
                selectedContainer = dataRowView?.Row as OrdersDataSet.OrderContainersRow;
            }

            if (selectedContainer == null)
            {
                MessageBoxUtilities.ShowMessageBoxWarn(NO_CONTAINER_SELECTED_ERROR, ERROR_HEADER);
                return;
            }

            var partWeight = _partWeightLazy.Value;

            var weightFormat = $"0.{"0".Repeat(ApplicationSettings.Current.WeightDecimalPlaces)}";

            _flyoutManager.DisplayFlyout("Weight Scale",
                $"Gross:{grossWeight.ToString(weightFormat)} lbs.\n" +
                $"Manual Tare: {_tareWeight.ToString(weightFormat)} lbs.\n" +
                $"Part Weight: {partWeight.ToString(weightFormat)} lbs.",
                timeMillseconds: TOTAL_FLYOUT_TIME_MS);

            var containerQuantity = 0;

            // Try to determine pieces from net/gross weight and part weight
            var scaleWeight = grossWeight - _tareWeight;

            if (partWeight > 0M)
            {
                containerQuantity = Convert.ToInt32(scaleWeight / partWeight);
            }

            if (containerQuantity > 0)
            {
                selectedContainer.PartQuantity = containerQuantity;
            }
            else
            {
                _logger.Info("Cannot set container quantity below 1");
            }

            if (grossWeight > 0)
            {
                selectedContainer.Weight = grossWeight;
            }
            else
            {
                _logger.Info("Cannot set container quantity to 0 or a negative number.");
            }
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                SaveData();
                PrintLabels();
                DialogResult = DialogResult.OK;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error on closing containers dialog.");
            }
        }

        private void btnAddProcess_Click(object sender, EventArgs e)
        {
            try
            {
                AddContainer();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on add another container.");
            }
        }

        private void btnRemoveProcess_Click(object sender, EventArgs e)
        {
            try
            {
                var rowsToDelete = new List<DataRow>();

                foreach (var row in this.grdContainers.Selected.Rows)
                {
                    if (row.IsDataRow)
                    {
                        var dataRow = ((DataRowView)row.ListObject).Row;

                        if (dataRow != null)
                            rowsToDelete.Add(dataRow);
                    }
                }

                foreach (var delete in rowsToDelete)
                    delete.Delete();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error removing the selected containers.");
            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdContainers.Selected.Rows.Count == 0)
                {
                    return;
                }

                var firstSelectedRow = grdContainers.Selected.Rows[0];

                if (firstSelectedRow.IsDataRow)
                {
                    var container = (firstSelectedRow.ListObject as DataRowView)?.Row as OrdersDataSet.OrderContainersRow;

                    if (container != null)
                    {
                        _dsOrders.OrderContainerItem.AddOrderContainerItemRow(container, 1);
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error removing the selected containers.");
            }
        }

        private void btnScaleOptions_Click(object sender, EventArgs e)
        {
            bool dialogResult = false;
            try
            {
                var window = new ScaleOptions();
                var helper = new WindowInteropHelper(window) { Owner = DWOSApp.MainForm.Handle };
                dialogResult = window.ShowDialog().GetValueOrDefault();
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error while changing scale options.", exc);
            }

            try
            {
                if (dialogResult)
                {
                    LoadScale(ScaleFactory.NewScale());
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error while changing scales.", exc);
            }
        }

        private void OrderContainers_Load(object sender, EventArgs e)
        {
            try
            {
                LoadScale(ScaleFactory.NewScale());
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error while changing scales.", exc);
            }
        }

        private void _scale_ScaleDataReceived(object sender, ScaleDataReceivedEventArgs e)
        {
            try
            {
                this.scaleDataNotReceivedTimer.Stop();

                _logger.Info("Successfully received data from scale.");

                var grossWeight = 0M;
                if (e.Data.GrossWeight.HasValue)
                {
                    grossWeight = Math.Min(e.Data.GrossWeight.Value, MAX_WEIGHT);
                }

                Action<decimal> updateData = UpdateContainerWeight;
                Invoke(updateData, grossWeight);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error while processing scale data.");
            }
        }

        private void btnWeigh_Click(object sender, EventArgs e)
        {
            if (_scale == null)
            {
                return;
            }

            if (this.grdContainers.Selected.Rows.Count == 0)
            {
                MessageBoxUtilities.ShowMessageBoxWarn(NO_CONTAINER_SELECTED_ERROR, ERROR_HEADER);
                return;
            }

            var tareDialog = new TareWeightDialog
            {
                TareWeight = _tareWeight
            };

            var helper = new WindowInteropHelper(tareDialog) { Owner = Handle };

            if (tareDialog.ShowDialog() ?? false)
            {
                _tareWeight = tareDialog.TareWeight;
                this._scale.Print();
                this.scaleDataNotReceivedTimer.Start();

                _flyoutManager.DisplayFlyout("Weight Scale",
                    "Sent 'weigh' to weight scale.",
                    timeMillseconds: STATUS_FLYOUT_TIME_MS);
            }

            GC.KeepAlive(helper);
        }

        private void btnZero_Click(object sender, EventArgs e)
        {
            this._scale?.Zero();
            _flyoutManager.DisplayFlyout("Weight Scale",
                "Sent 'zero' to weight scale.",
                timeMillseconds: STATUS_FLYOUT_TIME_MS);
        }

        private void btnTare_Click(object sender, EventArgs e)
        {
            this._scale?.Tare();
            _flyoutManager.DisplayFlyout("Weight Scale",
                "Sent 'tare' to weight scale.",
                timeMillseconds: STATUS_FLYOUT_TIME_MS);
        }

        private void scaleDataNotReceivedTimer_Tick(object sender, EventArgs e)
        {
            this.scaleDataNotReceivedTimer.Stop();

            _flyoutManager.DisplayFlyout("Weight Scale",
                "Did not receive data from scale.\nPlease check your computer's connection to the scale and try again.",
                true);
        }

        private void grdContainers_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                grdContainers.AfterColPosChanged -= grdContainers_AfterColPosChanged;
                grdContainers.AfterSortChange -= grdContainers_AfterSortChange;

                var layout = grdContainers.DisplayLayout;
                var primaryBand = layout.Bands[0];
                var itemBand = layout.Bands[1];

                primaryBand.Columns["Weight"].Format = string.Format("###,##0.0{0}",
                    string.Concat(Enumerable.Repeat("#", ApplicationSettings.Current.WeightDecimalPlaces - 1)));

                primaryBand.Columns["Weight"].MaskInput = string.Format("nnnn.{0}",
                    string.Concat(Enumerable.Repeat("n", ApplicationSettings.Current.WeightDecimalPlaces)));

                if (!layout.ValueLists.Exists("ShipmentPackageType"))
                {
                    var shipmentPackageTypeList = layout.ValueLists.Add("ShipmentPackageType");
                    foreach (var typeRow in _dsOrders.ShipmentPackageType.OrderBy(type => Name))
                    {
                        shipmentPackageTypeList.ValueListItems.Add(typeRow.ShipmentPackageTypeID, typeRow.Name);
                    }
                }

                primaryBand.Columns["ShipmentPackageTypeID"].ValueList = layout.ValueLists["ShipmentPackageType"];

                itemBand.Columns["ShipmentPackageTypeID"].ValueList = layout.ValueLists["ShipmentPackageType"];

                // Load settings
                _containerBandSettingsPersistence.LoadSettings().ApplyTo(primaryBand);
                _itemBandSettingsPersistence.LoadSettings().ApplyTo(itemBand);

            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing main containers grid.");
            }
            finally
            {
                grdContainers.AfterColPosChanged += grdContainers_AfterColPosChanged;
                grdContainers.AfterSortChange += grdContainers_AfterSortChange;
            }
        }

        private void grdContainers_AfterColPosChanged(object sender, Infragistics.Win.UltraWinGrid.AfterColPosChangedEventArgs e)
        {
            try
            {
                var layout = grdContainers.DisplayLayout;
                var primaryBand = layout.Bands[0];
                var itemBand = layout.Bands[1];

                // Save only the band that changed
                if (e.ColumnHeaders.Any(header => header.Band == primaryBand))
                {
                    var primarySettings = new UltraGridBandSettings();
                    primarySettings.RetrieveSettingsFrom(primaryBand);
                    _containerBandSettingsPersistence.SaveSettings(primarySettings);
                }

                if (e.ColumnHeaders.Any(header => header.Band == itemBand))
                {
                    var itemSettings = new UltraGridBandSettings();
                    itemSettings.RetrieveSettingsFrom(itemBand);
                    _itemBandSettingsPersistence.SaveSettings(itemSettings);
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error changing column position in grid.");
            }
        }

        private void grdContainers_AfterSortChange(object sender, Infragistics.Win.UltraWinGrid.BandEventArgs e)
        {
            try
            {
                var layout = grdContainers.DisplayLayout;
                var primaryBand = layout.Bands[0];
                var itemBand = layout.Bands[1];

                // Save only the band that changed
                if (e.Band == primaryBand)
                {
                    var primarySettings = new UltraGridBandSettings();
                    primarySettings.RetrieveSettingsFrom(primaryBand);
                    _containerBandSettingsPersistence.SaveSettings(primarySettings);
                }
                else if (e.Band == itemBand)
                {
                    var itemSettings = new UltraGridBandSettings();
                    itemSettings.RetrieveSettingsFrom(itemBand);
                    _itemBandSettingsPersistence.SaveSettings(itemSettings);
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error changing column position in grid.");
            }
        }

        private void grdContainers_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            try
            {
                e.Row.ExpandAll();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing row.");
            }
        }

        private void pnlWeigh_MouseHover(object sender, EventArgs e)
        {
            try
            {
                // Show tooltip for disabled control
                if (btnWeigh.Enabled)
                {
                    return;
                }

                ultraToolTipManager1.ShowToolTip(btnWeigh);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error showing weigh tooltip");
            }
        }

        private void pnlZero_MouseHover(object sender, EventArgs e)
        {
            try
            {
                // Show tooltip for disabled control
                if (btnZero.Enabled)
                {
                    return;
                }

                ultraToolTipManager1.ShowToolTip(btnZero);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error showing zero tooltip");
            }
        }

        private void pnlTare_MouseHover(object sender, EventArgs e)
        {
            try
            {
                // Show tooltip for disabled control
                if (btnTare.Enabled)
                {
                    return;
                }

                ultraToolTipManager1.ShowToolTip(btnTare);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error showing tare tooltip");
            }
        }

        private void pnlWeigh_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                // Hide tooltip for disabled control
                if (btnWeigh.Enabled || !ultraToolTipManager1.IsToolTipVisible(btnWeigh))
                {
                    return;
                }

                ultraToolTipManager1.HideToolTip();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error hiding weigh tooltip");
            }
        }

        private void pnlZero_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                // Hide tooltip for disabled control
                if (btnZero.Enabled || !ultraToolTipManager1.IsToolTipVisible(btnZero))
                {
                    return;
                }

                ultraToolTipManager1.HideToolTip();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error hiding zero tooltip");
            }
        }

        private void pnlTare_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                // Hide tooltip for disabled control
                if (btnTare.Enabled || !ultraToolTipManager1.IsToolTipVisible(btnTare))
                {
                    return;
                }

                ultraToolTipManager1.HideToolTip();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error hiding tare tooltip");
            }
        }

        private void Print_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                btnPreview.Enabled = chkPrintContainerLabels.Checked ||
                    chkPrintWOLabels.Checked;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc,
                    "Error changing checkbox that determines what labels to print.");
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                const string saveBeforePreviewMsg = "Would you like to save your changes?\n" +
                    "(If you choose not to save, labels may show outdated information.)";

                const string saveBeforePreviewHeader = "Containers";

                var hasUnsavedChanges = this._dsOrders.OrderContainers
                    .Any(r => r.RowState == DataRowState.Deleted || r.RowState == DataRowState.Modified || r.RowState == DataRowState.Added);

                if (hasUnsavedChanges)
                {
                    var dialogResult = MessageBoxUtilities.ShowMessageBoxYesOrNo(
                        saveBeforePreviewMsg,
                        saveBeforePreviewHeader);

                    if (dialogResult == DialogResult.Yes)
                    {
                        SaveData();
                    }
                }

                if (chkPrintContainerLabels.Checked)
                {
                    CreateContainerLabel().DisplayReport();
                }

                if (chkPrintWOLabels.Checked)
                {
                    CreateOrderLabel().DisplayReport();
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error while previewing labels.", exc);
            }
        }


        #endregion

    }
}
