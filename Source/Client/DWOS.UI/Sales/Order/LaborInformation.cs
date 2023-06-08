using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace DWOS.UI.Sales.Order
{
    public partial class LaborInformation : DataPanel
    {
        #region Fields

        private const string ERROR_END_TIME = "If EndTime is present, cannot set null value. If it's absent, cannot set non-null value.";

        private BindingList<ILaborInfo> _laborInfo;
        private BatchProcessesTableAdapter _taBatchProcesses;
        private BatchQuantityInfoTableAdapter _taBatchQuantity;
        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("OrderEntry_LaborInformation", new UltraGridBandSettings());

        #endregion

        #region Properties

        public OrdersDataSet Dataset
        {
            get { return _dataset as OrdersDataSet; }
            set { _dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.Order.OrderIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public LaborInformation()
        {
            InitializeComponent();
            _laborInfo = new BindingList<ILaborInfo>();
        }

        public void LoadData(OrdersDataSet dataset, BatchProcessesTableAdapter taBatchProcesses)
        {
            _taBatchQuantity = new BatchQuantityInfoTableAdapter();
            _taBatchProcesses = taBatchProcesses;
            _dataset = dataset;
            bsData.DataSource = dataset;
            bsData.DataMember = dataset.Order.TableName;
            grdLabor.DataSource = _laborInfo;
            _panelLoaded = true;
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            base.BeforeMoveToNewRecord(id);
            txtTotalProcess.Clear();
            txtTotalInspection.Clear();

            _laborInfo.Clear();
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            OrdersDataSet.BatchQuantityInfoDataTable dtBatchQuantityInfo = null;
            try
            {
                base.AfterMovedToNewRecord(id);
                var currentOrder = CurrentRecord as OrdersDataSet.OrderRow;

                if (currentOrder == null)
                {
                    return;
                }

                dtBatchQuantityInfo = new OrdersDataSet.BatchQuantityInfoDataTable();
                _taBatchQuantity.FillByOrder(dtBatchQuantityInfo, currentOrder.OrderID);

                // Order labor - in-process
                foreach (var orderProcess in currentOrder.GetOrderProcessesRows().OrderBy(row => row.StepOrder))
                {
                    var laborTimes = orderProcess.GetOrderProcessesOperatorRows()
                        .SelectMany(row => row.GetLaborTimeRows());

                    foreach (var time in laborTimes)
                    {
                        _laborInfo.Add(new OrderProcessLabor(time));
                    }
                }

                // Order labor - out-of-process
                foreach (var time in currentOrder.GetOrderOperatorRows().SelectMany(op => op.GetOrderOperatorTimeRows()))
                {
                    _laborInfo.Add(new OrderLabor(time));
                }

                // Batch labor
                foreach (var batchID in currentOrder.GetBatchOrderRows().Select(i => i.BatchID).Distinct())
                {
                    var batch = Dataset.Batch.FindByBatchID(batchID);

                    if (batch == null)
                    {
                        continue;
                    }

                    var qtyData = dtBatchQuantityInfo.FirstOrDefault(row => row.BatchID == batchID);

                    int orderQuantity = 0;
                    int batchQuantity = 0;

                    if (qtyData != null)
                    {
                        batchQuantity = qtyData.BatchQuantity;
                        orderQuantity = qtyData.OrderQuantity;
                    }

                    // Batch labor - in-process
                    foreach (var batchProcess in batch.GetBatchProcessesRows())
                    {
                        string processName = _taBatchProcesses.GetProcessName(batchProcess.BatchProcessID);

                        var laborTimes = batchProcess.GetBatchProcessesOperatorRows()
                            .SelectMany(row => row.GetLaborTimeRows());

                        foreach (var time in laborTimes)
                        {
                            _laborInfo.Add(new BatchProcessLabor(time, processName, orderQuantity, batchQuantity));
                        }
                    }

                    // Batch labor - out-of-process
                    foreach (var time in batch.GetBatchOperatorRows().SelectMany(op => op.GetBatchOperatorTimeRows()))
                    {
                        _laborInfo.Add(new BatchLabor(time, orderQuantity, batchQuantity));
                    }
                }

                RefreshTotals();
            }
            finally
            {
                dtBatchQuantityInfo?.Dispose();
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            _taBatchQuantity?.Dispose();

            _taBatchProcesses = null;
            _taBatchQuantity = null;
        }

        private void RefreshTotals()
        {
            DateTime now = DateTime.Now;
            var totalProcessMinutes = 0d;
            var totalInspectionMinutes = 0d;
            var totalMiscMinutes = 0d;

            foreach (var time in _laborInfo)
            {
                var duration = time.CalculateDuration(now);
                if (time.WorkStatus == ApplicationSettings.Current.WorkStatusInProcess)
                {
                    totalProcessMinutes += duration;
                }
                else if (time.WorkStatus == ApplicationSettings.Current.WorkStatusPendingQI)
                {
                    totalInspectionMinutes += duration;
                }
                else
                {
                    totalMiscMinutes += duration;
                }
            }

            txtTotalProcess.Text = GetDurationString(totalProcessMinutes);
            txtTotalInspection.Text = GetDurationString(totalInspectionMinutes);
            txtTotalOther.Text = GetDurationString(totalMiscMinutes);

            double totalMinutes = totalProcessMinutes + totalInspectionMinutes + totalMiscMinutes;
            txtTotalAll.Text = GetDurationString(totalMinutes);
        }

        private static string GetDurationString(double minutes)
        {
            if (minutes == 0d)
            {
                return string.Empty;
            }
            else
            {
                return DateUtilities.ToDifferenceShortHand(Convert.ToInt32(minutes));
            }
        }

        #endregion

        #region Events

        private void grdLabor_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            const string timeFormat = "hh:mm tt";
            const string timeMaskInput = "{LOC} yyyy/mm/dd hh:mm tt";

            try
            {
                grdLabor.AfterColPosChanged -= grdLabor_AfterColPosChanged;
                grdLabor.AfterSortChange -= grdLabor_AfterSortChange;

                var band = grdLabor.DisplayLayout.Bands[0];
                band.Columns[nameof(ILaborInfo.WorkStatus)].Hidden = true;

                band.Columns[nameof(ILaborInfo.StartTime)].Format = timeFormat;
                band.Columns[nameof(ILaborInfo.StartTime)].MaskInput = timeMaskInput;
                band.Columns[nameof(ILaborInfo.StartTime)].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTime;

                band.Columns[nameof(ILaborInfo.EndTime)].Format = timeFormat;
                band.Columns[nameof(ILaborInfo.EndTime)].MaskInput = timeMaskInput;
                band.Columns[nameof(ILaborInfo.EndTime)].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTime;

                band.Columns[nameof(ILaborInfo.Duration)].Format = "#,##0";
                band.Columns[nameof(ILaborInfo.Duration)].MaskInput = "n,nnn";

                if (!SecurityManager.Current.IsInRole("OrderProcess.Edit"))
                {
                    grdLabor.Enabled = false;
                }
                else if (!SecurityManager.Current.IsInRole("OrderProcess.EditTime"))
                {
                    band.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
                }

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(band);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing layout.");
            }
            finally
            {
                grdLabor.AfterColPosChanged += grdLabor_AfterColPosChanged;
                grdLabor.AfterSortChange += grdLabor_AfterSortChange;
            }
        }

        private void grdLabor_AfterColPosChanged(object sender, Infragistics.Win.UltraWinGrid.AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdLabor.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing column position in grid.");
            }
        }

        private void grdLabor_AfterSortChange(object sender, Infragistics.Win.UltraWinGrid.BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdLabor.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing column position in grid.");
            }
        }

        private void grdLabor_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            try
            {
                if (e.Row.Cells["EndTime"].Value == null)
                {
                    e.Row.Cells["EndTime"].Activation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                    e.Row.Cells[nameof(ILaborInfo.Duration)].Activation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing labor record.");
            }
        }
        private void grdLabor_BeforeCellUpdate(object sender, Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventArgs e)
        {
            try
            {
                const string dateFormat = "yyyy/MM/dd hh:mm tt";
                var laborInfo = e.Cell.Row.ListObject as ILaborInfo;

                if (laborInfo == null)
                {
                    return;
                }

                bool validationError = false;

                if (e.Cell.Column.Key == nameof(ILaborInfo.StartTime))
                {
                    // Edited StartTime
                    var maximumValue = (laborInfo.EndTime ?? DateTime.Now);
                    DateTime? newStartTime = e.NewValue as DateTime?;

                    if (!newStartTime.HasValue)
                    {
                        MessageBoxUtilities.ShowMessageBoxOK("Time-In is required.", "Order Entry");
                        validationError = true;
                    }
                    else if (newStartTime.Value > maximumValue)
                    {
                        string msg = string.Format("Time-In cannot exceed {0}.",
                            maximumValue.ToString(dateFormat));

                        MessageBoxUtilities.ShowMessageBoxOK(msg, "Order Entry");
                        validationError = true;
                    }
                }
                else if (e.Cell.Column.Key == nameof(ILaborInfo.EndTime))
                {
                    // Edited EndTime
                    var minimumValue = laborInfo.StartTime;
                    DateTime? newEndTime = e.NewValue as DateTime?;

                    if (!newEndTime.HasValue)
                    {
                        MessageBoxUtilities.ShowMessageBoxOK("Time-Out is required.", "Order Entry");
                        validationError = true;
                    }
                    else if (newEndTime.Value < minimumValue)
                    {
                        string msg = string.Format("Time-Out cannot be below {0}.",
                            minimumValue.ToString(dateFormat));

                        MessageBoxUtilities.ShowMessageBoxOK(msg, "Order Entry");
                        validationError = true;
                    }
                }

                e.Cancel = validationError;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error while checking changes to labor durations.");
            }
        }

        private void grdLabor_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            try
            {
                RefreshTotals();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error refreshing labor duration totals.");
            }
        }


        private void grdLabor_Error(object sender, Infragistics.Win.UltraWinGrid.ErrorEventArgs e)
        {
            try
            {
                // If the error was caused by user setting an invalid value,
                // revert back to the original value to avoid an infinite
                // error loop.

                var cell = e.DataErrorInfo?.Cell;

                if (cell == null)
                {
                    return;
                }

                cell.Value = e.DataErrorInfo.Cell.OriginalValue;
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error canceling update for labor grid error.");
            }
        }

        #endregion

        #region ILaborInfoItem

        private interface ILaborInfo
        {
            string WorkStatus { get; }

            string Name { get; }

            DateTime StartTime { get; set; }

            DateTime? EndTime { get; set; }

            string Process { get; }

            string Status { get; }

            bool IsBatched { get; }

            int? Quantity { get; }

            double? Duration { get; set; }

            double CalculateDuration(DateTime defaultEndTime);
        }

        #endregion

        #region OrderProcessLabor

        private sealed class OrderProcessLabor : ILaborInfo
        {
            #region Properties

            public OrdersDataSet.LaborTimeRow LaborTimeRow
            {
                get;
            }

            #endregion

            #region Methods

            public OrderProcessLabor(OrdersDataSet.LaborTimeRow laborTime)
            {
                if (laborTime == null)
                {
                    throw new ArgumentNullException(nameof(laborTime));
                }
                else if (laborTime.IsOrderProcessesOperatorIDNull())
                {
                    string errorMsg = string.Format("Please use {0} for batch-related labor items.",
                        nameof(BatchProcessLabor));

                    throw new ArgumentException(errorMsg, nameof(laborTime));
                }

                LaborTimeRow = laborTime;
            }

            #endregion

            #region ILaborInfo Members

            public string WorkStatus => LaborTimeRow.WorkStatus;

            public string Name
            {
                get
                {
                    var orderProcessesOperator = LaborTimeRow.OrderProcessesOperatorRow;

                    if (orderProcessesOperator != null)
                    {
                        return orderProcessesOperator.UserSummaryRow?.Name ??
                            orderProcessesOperator.UserID.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public DateTime StartTime
            {
                get
                {
                    return LaborTimeRow.StartTime;
                }
                set
                {
                    LaborTimeRow.StartTime = value;
                }
            }

            public DateTime? EndTime
            {
                get
                {
                    if (LaborTimeRow.IsEndTimeNull())
                    {
                        return null;
                    }
                    else
                    {
                        return LaborTimeRow.EndTime;
                    }
                }
                set
                {
                    if (value.HasValue)
                    {
                        LaborTimeRow.EndTime = value.Value;
                    }
                    else
                    {
                        LaborTimeRow.SetEndTimeNull();
                    }
                }
            }

            public string Process
            {
                get
                {
                    return LaborTimeRow.OrderProcessesOperatorRow?
                        .OrderProcessesRow?
                        .ProcessAliasSummaryRow?
                        .Name;
                }
            }

            public string Status
            {
                get
                {
                    var workStatus = LaborTimeRow.WorkStatus;

                    if (workStatus == ApplicationSettings.Current.WorkStatusInProcess)
                    {
                        return "Processing";
                    }
                    else if (workStatus == ApplicationSettings.Current.WorkStatusPendingQI)
                    {
                        return "Inspecting";
                    }
                    else
                    {
                        return workStatus;
                    }
                }
            }

            public bool IsBatched
            {
                get
                {
                    return false;
                }
            }

            public int? Quantity
            {
                get
                {
                    var orderProcessesRow = LaborTimeRow.OrderProcessesOperatorRow?.OrderProcessesRow;

                    if (orderProcessesRow == null || orderProcessesRow.IsPartCountNull())
                    {
                        return null;
                    }

                    return orderProcessesRow.PartCount;
                }
            }

            public double? Duration
            {
                get
                {
                    var endTime = EndTime;

                    if (!endTime.HasValue)
                    {
                        return null;
                    }

                    return (endTime.Value - StartTime).TotalMinutes;
                }
                set
                {
                    var previousValue = Duration;

                    if (previousValue.HasValue != value.HasValue)
                    {
                        throw new InvalidOperationException(ERROR_END_TIME);
                    }
                    else if (previousValue == value)
                    {
                        return;
                    }

                    EndTime = StartTime.AddMinutes(value.Value);
                }
            }

            public double CalculateDuration(DateTime defaultEndTime)
            {
                return ((EndTime ?? defaultEndTime)- StartTime).TotalMinutes;
            }

            #endregion
        }

        #endregion

        #region OrderLabor

        private sealed class OrderLabor : ILaborInfo
        {
            #region Properties

            public OrdersDataSet.OrderOperatorTimeRow TimeRow { get; }

            #endregion

            #region Methods

            public OrderLabor(OrdersDataSet.OrderOperatorTimeRow timeRow)
            {
                TimeRow = timeRow ?? throw new ArgumentNullException(nameof(timeRow));
            }

            #endregion

            #region ILaborInfo Members

            public string WorkStatus => TimeRow.WorkStatus;

            public string Name
            {
                get
                {
                    var orderOperator = TimeRow.OrderOperatorRow;

                    if (orderOperator != null)
                    {
                        return orderOperator.UserSummaryRow?.Name ??
                            orderOperator.UserID.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public DateTime StartTime
            {
                get => TimeRow.StartTime;
                set => TimeRow.StartTime = value;
            }

            public DateTime? EndTime
            {
                get
                {
                    if (TimeRow.IsEndTimeNull())
                    {
                        return null;
                    }
                    else
                    {
                        return TimeRow.EndTime;
                    }
                }
                set
                {
                    if (value.HasValue)
                    {
                        TimeRow.EndTime = value.Value;
                    }
                    else
                    {
                        TimeRow.SetEndTimeNull();
                    }
                }
            }

            public string Process => "N/A";

            public string Status => TimeRow.WorkStatus;

            public bool IsBatched => false;

            public int? Quantity
            {
                get
                {
                    var orderRow = TimeRow.OrderOperatorRow?.OrderRow;

                    return (orderRow == null || orderRow.IsPartQuantityNull())
                        ? (int?)null
                        : orderRow.PartQuantity;
                }
            }

            public double? Duration
            {
                get
                {
                    var endTime = EndTime;

                    if (!endTime.HasValue)
                    {
                        return null;
                    }

                    return (endTime.Value - StartTime).TotalMinutes;
                }
                set
                {
                    var previousValue = Duration;

                    if (previousValue.HasValue != value.HasValue)
                    {
                        throw new InvalidOperationException(ERROR_END_TIME);
                    }
                    else if (previousValue == value)
                    {
                        return;
                    }

                    EndTime = StartTime.AddMinutes(value.Value);
                }
            }

            public double CalculateDuration(DateTime defaultEndTime)
            {
                return ((EndTime ?? defaultEndTime) - StartTime).TotalMinutes;
            }

            #endregion
        }

        #endregion

        #region BatchProcessLabor

        private sealed class BatchProcessLabor : ILaborInfo
        {
            #region Properties

            public OrdersDataSet.LaborTimeRow LaborTimeRow
            {
                get;
            }

            public int OrderQuantity
            {
                get;
                private set;
            }

            public int BatchQuantity
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public BatchProcessLabor(OrdersDataSet.LaborTimeRow laborTime, string process, int orderQuantity, int batchQuantity)
            {
                if (laborTime == null)
                {
                    throw new ArgumentNullException(nameof(laborTime));
                }
                else if (laborTime.IsBatchProcessesOperatorIDNull())
                {
                    string errorMsg = string.Format("Please use {0} for non-batch labor items.",
                        nameof(OrderProcessLabor));

                    throw new ArgumentException(errorMsg, nameof(laborTime));
                }

                LaborTimeRow = laborTime;
                Process = process;
                OrderQuantity = orderQuantity;
                BatchQuantity = batchQuantity;
            }

            #endregion

            #region ILaborInfo Members

            public string WorkStatus => LaborTimeRow.WorkStatus;

            public string Name
            {
                get
                {
                    var batchProcessesOperator = LaborTimeRow.BatchProcessesOperatorRow;

                    if (batchProcessesOperator != null)
                    {
                        return batchProcessesOperator.UserSummaryRow?.Name ??
                            batchProcessesOperator.UserID.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public DateTime StartTime
            {
                get
                {
                    return LaborTimeRow.StartTime;
                }
                set
                {
                    LaborTimeRow.StartTime = value;
                }
            }

            public DateTime? EndTime
            {
                get
                {
                    if (LaborTimeRow.IsEndTimeNull())
                    {
                        return null;
                    }
                    else
                    {
                        return LaborTimeRow.EndTime;
                    }
                }
                set
                {
                    if (value.HasValue)
                    {
                        LaborTimeRow.EndTime = value.Value;
                    }
                    else
                    {
                        LaborTimeRow.SetEndTimeNull();
                    }
                }
            }

            public string Process
            {
                get;
                private set;
            }

            public string Status
            {
                get
                {
                    var workStatus = LaborTimeRow.WorkStatus;

                    if (workStatus == ApplicationSettings.Current.WorkStatusInProcess)
                    {
                        return "Processing";
                    }
                    else if (workStatus == ApplicationSettings.Current.WorkStatusPendingQI)
                    {
                        return "Inspecting";
                    }
                    else
                    {
                        return workStatus;
                    }
                }
            }

            public bool IsBatched
            {
                get
                {
                    return true;
                }
            }

            public int? Quantity => BatchQuantity;

            public double? Duration
            {
                get
                {
                    var endTime = EndTime;

                    if (!endTime.HasValue || BatchQuantity == 0)
                    {
                        return null;
                    }

                    var totalDuration = (endTime.Value - StartTime).TotalMinutes;
                    var orderDuration = totalDuration * (Convert.ToDouble(OrderQuantity) / Convert.ToDouble(BatchQuantity));
                    return orderDuration;
                }
                set
                {
                    var previousValue = Duration;

                    if (previousValue.HasValue != value.HasValue)
                    {
                        throw new InvalidOperationException(ERROR_END_TIME);
                    }
                    else if (previousValue == value)
                    {
                        return;
                    }

                    EndTime = StartTime.AddMinutes(value.Value);
                }
            }

            public double CalculateDuration(DateTime defaultEndTime)
            {
                if (OrderQuantity == 0 || BatchQuantity == 0)
                {
                    return 0;
                }

                var endTime = EndTime ?? defaultEndTime;

                var totalDuration = (endTime - StartTime).TotalMinutes;
                var orderDuration = totalDuration * (Convert.ToDouble(OrderQuantity) / Convert.ToDouble(BatchQuantity));
                return orderDuration;
            }

            #endregion
        }

        #endregion

        #region BatchLabor

        private class BatchLabor : ILaborInfo
        {
            #region Properties

            public OrdersDataSet.BatchOperatorTimeRow TimeRow { get; }

            public int OrderQuantity { get; }

            public int BatchQuantity { get; }

            #endregion

            #region Methods

            public BatchLabor(OrdersDataSet.BatchOperatorTimeRow timeRow, int orderQuantity, int batchQuantity)
            {
                TimeRow = timeRow ?? throw new ArgumentNullException(nameof(timeRow));
                OrderQuantity = orderQuantity;
                BatchQuantity = batchQuantity;
            }

            #endregion

            #region ILaborInfo Members

            public string WorkStatus => TimeRow.WorkStatus;

            public string Name
            {
                get
                {
                    var batchOperator = TimeRow.BatchOperatorRow;

                    if (batchOperator != null)
                    {
                        return batchOperator.UserSummaryRow?.Name ??
                            batchOperator.UserID.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public DateTime StartTime
            {
                get => TimeRow.StartTime;
                set => TimeRow.StartTime = value;
            }

            public DateTime? EndTime
            {
                get
                {
                    if (TimeRow.IsEndTimeNull())
                    {
                        return null;
                    }
                    else
                    {
                        return TimeRow.EndTime;
                    }
                }
                set
                {
                    if (value.HasValue)
                    {
                        TimeRow.EndTime = value.Value;
                    }
                    else
                    {
                        TimeRow.SetEndTimeNull();
                    }
                }
            }

            public string Process => "N/A";

            public string Status => TimeRow.WorkStatus;

            public bool IsBatched => true;

            public int? Quantity => BatchQuantity;

            public double? Duration
            {
                get
                {
                    var endTime = EndTime;

                    if (!endTime.HasValue || BatchQuantity == 0)
                    {
                        return null;
                    }

                    var totalDuration = (endTime.Value - StartTime).TotalMinutes;
                    return totalDuration * (Convert.ToDouble(OrderQuantity) / Convert.ToDouble(BatchQuantity));
                }
                set
                {
                    var previousValue = Duration;

                    if (previousValue.HasValue != value.HasValue)
                    {
                        throw new InvalidOperationException(ERROR_END_TIME);
                    }
                    else if (previousValue == value)
                    {
                        return;
                    }

                    EndTime = StartTime.AddMinutes(value.Value);
                }
            }

            public double CalculateDuration(DateTime defaultEndTime)
            {
                if (OrderQuantity == 0 || BatchQuantity == 0)
                {
                    return 0;
                }

                var endTime = EndTime ?? defaultEndTime;

                var totalDuration = (endTime - StartTime).TotalMinutes;
                return totalDuration * (Convert.ToDouble(OrderQuantity) / Convert.ToDouble(BatchQuantity));
            }

            #endregion
        }

        #endregion
    }
}
