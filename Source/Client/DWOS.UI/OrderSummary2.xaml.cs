using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderStatusDataSetTableAdapters;
using DWOS.Data.Date;
using DWOS.Shared.Utilities;
using DWOS.UI.Properties;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using DWOS.UI.Utilities.Convertors;
using Infragistics.Windows.Controls;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.DataPresenter.Events;
using NLog;
using Infragistics.Windows.Editors;
using DWOS.Data.Order.Activity;
using DWOS.Reports.Reports;
using DWOS.Reports;

namespace DWOS.UI
{
    /// <summary>
    ///     Interaction logic for OrderSummary2.xaml
    /// </summary>
    public partial class OrderSummary2 : IOrderSummary, IReportTab
    {
        #region Fields

        private const int CurrentLayoutVersion = 20;
        private const string MyCustomerFilter = "<My Customers>";

        public const string DATA_TYPE = "ordersummary";
        public event EventHandler LayoutError;

        private bool _countAsPercent = false;
        private CountBy _countBy;
        private LateBy _lateBy;
        private readonly DaysLateCalculator _daysLateCalculator = new DaysLateCalculator(DependencyContainer.Resolve<IDateTimeNowProvider>());
        public event EventHandler AfterSelectedRowChanged;

        private enum CountBy { Order = 0, Part = 1 }
        private enum LateBy { Order = 0, Process = 1}

        #endregion

        #region Properties

        private string V19LayoutFilePath => FileSystem.UserAppDataPath() + "\\" + Key + "_" + "_v19.dat";

        private string LayoutFilePath => FileSystem.UserAppDataPath() + "\\" + Key + "_" + $"_v{CurrentLayoutVersion}.dat";

        public UserControl TabControl => this;

        public string TabName { get; set; }

        public string DataType => DATA_TYPE;

        public int SelectedWO
        {
            get
            {
                var row = CurrentRow;
                return row?.WO ?? -1;
            }
        }

        public string SelectedWorkStatus
        {
            get
            {
                var row = CurrentRow;
                return row?.WorkStatus;
            }
        }

        public bool? SelectedHoldStatus
        {
            get
            {
                var row = CurrentRow;
                return row?.Hold;
            }
        }

        public OrderType? SelectedOrderType
        {
            get
            {
                var row = CurrentRow;
                return (OrderType?)row?.OrderType;
            }
        }

        public string SelectedLocation
        {
            get
            {
                var row = CurrentRow;
                return row?.CurrentLocation;
            }
        }

        public int? SelectedLine
        {
            get
            {
                var row = CurrentRow;
                return row?.CurrentLine;
            }
        }

        public bool SelectedInBatch
        {
            get
            {
                var row = CurrentRow;
                return row?.InBatch ?? false;
            }
        }

        public int SelectedActiveTimerCount => CurrentRow?.ActiveTimerCount ?? 0;

        public string Key { get; set; }

        private OrderStatusData CurrentRow
        {
            get
            {
                if(grdOrders.SelectedItems.Records.Count == 1 && grdOrders.SelectedItems.Records[0].IsDataRecord)
                {
                    var dataRecord = grdOrders.SelectedItems.Records[0] as DataRecord;
                    if (dataRecord != null)
                    {
                        return (OrderStatusData)dataRecord.DataItem;
                    }
                }

                return null;
            }
        }

        #endregion

        #region Methods

        public OrderSummary2()
        {
            InitializeComponent();

            Key = RandomUtils.GetRandomString(6);
            TabName = "Orders";

            _lateBy = (LateBy)Settings.Default.SummaryLateBy;
            if (_lateBy == LateBy.Order)
                optLateByOrder.IsChecked = true;
            else
                optLateByProcess.IsChecked = true;

            _countBy = (CountBy)Settings.Default.SummaryCountBy;

            if (_countBy == CountBy.Order)
                optCountByOrder.IsChecked = true;
            else
                optCountByPart.IsChecked = true;
        }

        public void DisplayReport()
        {
            try
            {
                var filteredRecords = grdOrders.RecordManager
                    .GetFilteredInDataRecords()
                    .Select(d => d.DataItem)
                    .OfType<OrderStatusData>()
                    .ToList();

                if (filteredRecords.Count == 0)
                {
                    return;
                }

                // Retrieve visible columns (name -> report column map)
                var visibleColumns = new Dictionary<string, ExcelReport.Column>();
                foreach (var field in grdOrders.FieldLayouts[0].Fields)
                {
                    if (field.Visibility != Visibility.Visible)
                    {
                        continue;
                    }

                    var memberName = field.Name;
                    var columnLabel = field.Label?.ToString();

                    if (string.IsNullOrEmpty(columnLabel))
                    {
                        columnLabel = memberName;
                    }

                    visibleColumns.Add(memberName, new ExcelReport.Column(columnLabel));
                }

                // Get order of columns using data table (which was the source of
                // the column order in the original table).
                List<string> columnOrder;
                using (var dtOrderStatus = new OrderStatusDataSet.OrderStatusDataTable())
                {
                    columnOrder = dtOrderStatus.Columns
                        .OfType<DataColumn>()
                        .Select(c => c.ColumnName)
                        .ToList();
                }

                // Add custom fields
                using (var taField = new CustomFieldNameTableAdapter())
                {
                    using (var dbFields = taField.GetData())
                    {
                        columnOrder.AddRange(dbFields.Select(f => f.FormattedName));
                    }
                }

                using (var taField = new PartLevelCustomFieldNameTableAdapter())
                {
                    using (var dbFields = taField.GetData())
                    {
                        columnOrder.AddRange(dbFields.Select(f => f.FormattedName));
                    }
                }

                // Add other columns to the end that are populated after calling
                // table adapter's FillForClientDisplay method.
                columnOrder.Add("Operators");
                columnOrder.Add("SerialNumbers");
                columnOrder.Add("ProductClass");
                columnOrder.Add("CurrentLineString");

                // Get ordered lists of data members and report columns.
                var memberOrder = new List<string>();
                var reportColumns = new List<ExcelReport.Column>();

                foreach (var column in columnOrder)
                {
                    ExcelReport.Column reportColumn;
                    if (!visibleColumns.TryGetValue(column, out reportColumn))
                    {
                        continue;
                    }

                    memberOrder.Add(column);
                    reportColumns.Add(reportColumn);
                }

                // Create report rows
                var reportRows = new List<ExcelReport.Row>();

                foreach (var record in filteredRecords)
                {
                    var cells = memberOrder
                        .Select(member => record[member])
                        .ToList();

                    reportRows.Add(new ExcelReport.Row { Cells = cells });
                }

                // Create report
                var report = new ExcelReport("WIP", new List<ExcelReport.ReportTable>
                {
                    new ExcelReport.ReportTable
                    {
                        Columns = reportColumns,
                        Name = "OrderStatus",
                        Rows = reportRows,
                        IncludeCompanyHeader = false,
                        FormattingOptions = new ExcelReport.TableFormattingOptions
                        {
                            BorderAroundCells = true
                        }
                    }
                });

                report.DisplayReport();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error displaying grid WIP report.");
            }
        }

        public DwosTabData Export()
        {
            return new DwosTabData
            {
                Name = TabName,
                DataType = DataType,
                Key = Key,
                Layout = grdOrders.SaveCustomizations(),
                Version = CurrentLayoutVersion
            };
        }

        public void Initialize(WipData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            grdOrders.FieldSettings.AllowEdit = false;
            grdOrders.FieldSettings.AllowRecordFiltering = true;
            grdOrders.FieldSettings.SummaryUIType = SummaryUIType.MultiSelect;
            grdOrders.FieldSettings.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.InGroupByRecords;
            grdOrders.FieldSettings.LabelClickAction = LabelClickAction.SortByMultipleFields;
            grdOrders.FieldSettings.CellClickAction = CellClickAction.SelectRecord;

            grdOrders.DataSource = data.Orders;
            grdOrders.FieldLayouts[0].Fields["Priority"].Settings.SortComparer = new PrioritySortComparer();
            grdOrders.FieldLayouts[0].Fields["CurrentProcess"].Settings.CellValuePresenterStyleSelector = new ProcessFieldStyleSelector((Style)grdOrders.Resources["processLinkFieldStyle"], (Style)grdOrders.Resources["processFieldStyle"]);

            if (Settings.Default.WIPHighlightLateFullRow)
            {
                var settingsProvider = DependencyContainer.Resolve<IDwosApplicationSettingsProvider>();
                LateStyleSelector styleSelector;
                if (Settings.Default.WIPAnimations)
                {
                    styleSelector = new LateStyleSelector(
                        (Style)grdOrders.Resources["normalRecordStyle"],
                        (Style)grdOrders.Resources["lateAnimatedRecordStyle"],
                        settingsProvider);
                }
                else
                {
                    styleSelector = new LateStyleSelector(
                        (Style)grdOrders.Resources["normalRecordStyle"],
                        (Style)grdOrders.Resources["lateRecordStyle"],
                        settingsProvider);
                }

                grdOrders.FieldLayoutSettings.DataRecordCellAreaStyleSelector = styleSelector;
            }
            else
                grdOrders.FieldLayoutSettings.DataRecordCellAreaStyle = (Style)grdOrders.Resources["normalRecordStyle"];

            grdOrders.RecordFilterDropDownPopulating += grdOrders_RecordFilterDropDownPopulating;
            grdOrders.RecordFilterChanged += grdOrders_RecordFilterChanged;
            RefreshSettings();
        }

        public void RefreshData(WipData data)
        {
            var scrollVerticalOffset = grdOrders.ScrollInfo?.VerticalOffset;
            var scrollHorizontalOffset = grdOrders.ScrollInfo?.HorizontalOffset;

            var lastSelectedOrderId = SelectedWO;
            grdOrders.DataSource = data.Orders;
            UpdateStats();
            RefreshFields();

            //Force resort of the grid since it will not catch the changes to the data and the sorted fields will be incorrect
            var temp = new FieldSortDescription[grdOrders.DefaultFieldLayout.SortedFields.Count];
            grdOrders.DefaultFieldLayout.SortedFields.CopyTo(temp, 0);
            grdOrders.DefaultFieldLayout.SortedFields.Clear();

            foreach (FieldSortDescription sortDescription in temp)
                grdOrders.DefaultFieldLayout.SortedFields.Add(sortDescription);

            if (lastSelectedOrderId > 0)
            {
                SelectWO(lastSelectedOrderId);
            }
            else
            {
                // Hack/workaround for issue where scroll reset did not work
                // without a selected work order.
                var firstRecord = grdOrders.Records.FirstOrDefault();
                if (firstRecord != null)
                {
                    grdOrders.BringRecordIntoView(firstRecord);
                }
            }

            // Ensure that scroll position is the same after refresh
            if (scrollVerticalOffset.HasValue)
            {
                grdOrders.ScrollInfo.SetVerticalOffset(scrollVerticalOffset.Value);
            }

            if (scrollHorizontalOffset.HasValue)
            {
                grdOrders.ScrollInfo.SetHorizontalOffset(scrollHorizontalOffset.Value);
            }
        }

        public void RefreshSettings()
        {
            ApplicationSettings appSettings = ApplicationSettings.Current;

            // Update styling for CurrentProcessDue
            if (appSettings.SchedulingEnabled && appSettings.SchedulerType == SchedulerType.ProcessLeadTimeHour)
            {
                grdOrders.FieldLayouts[0].Fields["CurrentProcessDue"].Format = "MM/dd/yyyy h:mm tt";
            }
            else
            {
                grdOrders.FieldLayouts[0].Fields["CurrentProcessDue"].Format = "MM/dd/yyyy";
            }

            // Rerun stats - may change depending on options
            UpdateStats();

            // HACK - Refresh & run style selector again for 'include on-hold
            // orders in late orders' option
            grdOrders.RecordContainerGenerationMode = ItemContainerGenerationMode.Virtualize;
            grdOrders.RecordContainerGenerationMode = ItemContainerGenerationMode.Recycle;
        }

        private void RefreshFields()
        {
            const int customFieldWidth = 100;

            var gridFields = grdOrders.DefaultFieldLayout.Fields;

            List<OrderStatusDataSet.ICustomFieldNameRow> dbFields =
                new List<OrderStatusDataSet.ICustomFieldNameRow>();

            using (var taField = new CustomFieldNameTableAdapter())
            {
                dbFields.AddRange(taField.GetData());
            }

            using (var taField = new PartLevelCustomFieldNameTableAdapter())
            {
                dbFields.AddRange(taField.GetData());
            }

            foreach (var dbField in dbFields)
            {
                var fieldName = dbField.FormattedName;

                if (gridFields.Any(i => i.Name == fieldName))
                {
                    continue;
                }

                gridFields.Add(new TextField()
                {
                    Name = fieldName,
                    BindingType = BindingType.Unbound,
                    AlternateBinding = new Binding("[" + fieldName + "]"),
                    Label = dbField.Name,
                    Visibility = Visibility.Collapsed,
                    Width = new FieldLength(customFieldWidth),
                    TextWrapping = TextWrapping.WrapWithOverflow
                });
            }
        }

        public void ApplyDefaultSort()
        {
            grdOrders.FieldLayouts[0].SortedFields.Clear();

            grdOrders.FieldLayouts[0].SortedFields.Add(new FieldSortDescription {Direction = ListSortDirection.Descending, FieldName = "Priority"});
            grdOrders.FieldLayouts[0].SortedFields.Add(new FieldSortDescription {Direction = ListSortDirection.Descending, FieldName = "CurrentProcess"});
            grdOrders.FieldLayouts[0].SortedFields.Add(new FieldSortDescription {Direction = ListSortDirection.Descending, FieldName = "EstShipDate"});

            grdOrders.FieldLayouts[0].RecordFilters.Clear();

            //update current locations
            var filter = new RecordFilter {FieldName = "CurrentLocation"};
            filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.Equals, Settings.Default.CurrentDepartment));
            grdOrders.FieldLayouts[0].RecordFilters.Add(filter);

            //if not sales or shipping then also filters orders that are "In Process"
            if(Settings.Default.CurrentDepartment != ApplicationSettings.Current.DepartmentSales && Settings.Default.CurrentDepartment != ApplicationSettings.Current.DepartmentShipping)
            {
                var statusFilter = new RecordFilter {FieldName = "WorkStatus"};
                statusFilter.Conditions.Add(new ComparisonCondition(ComparisonOperator.Equals,  ApplicationSettings.Current.WorkStatusInProcess));
                grdOrders.FieldLayouts[0].RecordFilters.Add(statusFilter);
            }

            UpdateStats();
        }

        private bool LoadCustomization(string filePath)
        {
            bool success;

            try
            {
                string errorMsg;
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    grdOrders.LoadCustomizations(fs, out errorMsg);
                }

                if (!string.IsNullOrEmpty(errorMsg))
                {
                    LogManager.GetCurrentClassLogger().Warn("Error loading layout file {0}:\n{1}", filePath, errorMsg);
                    LayoutError?.Invoke(this, EventArgs.Empty);
                }

                success = string.IsNullOrEmpty(errorMsg);
            }
            catch (System.Xml.XmlException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error loading layout file {0}", filePath);
                LayoutError?.Invoke(this, EventArgs.Empty);
                success = false;
            }
            catch (InvalidOperationException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error loading layout file {0}", filePath);
                LayoutError?.Invoke(this, EventArgs.Empty);
                success = false;
            }

            return success;
        }

        public void LoadLayout()
        {
            try
            {
                var filePath = LayoutFilePath;

                if (!File.Exists(filePath) && File.Exists(V19LayoutFilePath))
                {
                    UpgradeLayoutFile(V19LayoutFilePath);
                }

                if (File.Exists(filePath))
                {
                    // Try and load the customization file
                    if(!LoadCustomization(filePath))
                    {
                        // Corrupt customization file, delete and save a new default one
                        LogManager.GetCurrentClassLogger().Info("Customization file is not in a valid xml format, {0}", filePath);
                        File.Copy(filePath, filePath + ".bak", true);
                        File.Delete(filePath);
                        grdOrders.ClearCustomizations(CustomizationType.All); // This doesn't really seem to clear the customization
                        SaveLayout();

                        // Reload the newly saved file
                        LoadCustomization(filePath);
                    }
                }
                else
                {
                    LogManager.GetCurrentClassLogger().Info("Customization file does not exist: {0}", filePath);
                }

                Setup();
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading layout.");
            }
        }

        private void Setup()
        {
            grdOrders.Background = null;

            //else set the background, since it is based on customer logo not assigned by default
            var companyLogoPath = ApplicationSettings.Current.CompanyLogoImagePath;

            if(!string.IsNullOrWhiteSpace(companyLogoPath) && File.Exists(companyLogoPath))
            {
                var image = new BitmapImage(new Uri(companyLogoPath, UriKind.Absolute));
                var brush = new ImageBrush(image)
                {
                    Opacity = .25,
                    Stretch = Stretch.None
                };
                brush.Freeze();
                grdOrders.Background = brush;
            }
        }

        public void LoadLayout(string content)
        {
            // Reminder - Might need to upgrade contents if from a previous version
            string errorMsg;
            grdOrders.LoadCustomizations(content,  out errorMsg);

            if (!string.IsNullOrEmpty(errorMsg))
            {
                LogManager.GetCurrentClassLogger().Warn("Error loading layout:\n{0}", errorMsg);
                LayoutError?.Invoke(this, EventArgs.Empty);
            }

            Setup();
        }

        private void UpgradeLayoutFile(string previousLayoutPath)
        {
            var content = File.ReadAllText(previousLayoutPath);

            content = content
                // Fix 'cannot resize' issue
                .Replace("cellWidthAuto=\"true\"", string.Empty)
                .Replace("labelWidthAuto=\"true\"", string.Empty);

            File.WriteAllText(LayoutFilePath, content);
            File.Delete(previousLayoutPath);
        }

        public void SaveLayout()
        {
            try
            {
                Settings.Default.SummaryLateBy = (int)_lateBy;
                Settings.Default.SummaryCountBy = (int)_countBy;

                var filePath = LayoutFilePath;

                using(var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    grdOrders.SaveCustomizations(fs);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error saving layout.");
            }
        }

        public List <int> GetFilteredOrders()
        {
            var orders = new List <int>();

            foreach(var record in grdOrders.RecordManager.GetFilteredInDataRecords())
            {
                if(record.IsDataRecord && record.DataItem is OrderStatusData)
                {
                    orders.Add(((OrderStatusData)record.DataItem).WO);
                }
            }

            return orders;
        }

        private void FilterByProcess(string currentProcess)
        {
            grdOrders.FieldLayouts[0].RecordFilters.Clear();

            var statusFilter = new RecordFilter {FieldName = "WorkStatus"};
            statusFilter.Conditions.Add(new ComparisonCondition(ComparisonOperator.Equals,  ApplicationSettings.Current.WorkStatusInProcess));
            grdOrders.FieldLayouts[0].RecordFilters.Add(statusFilter);

            var processFilter = new RecordFilter {FieldName = "CurrentProcess"};
            processFilter.Conditions.Add(new ComparisonCondition(ComparisonOperator.Equals, currentProcess));
            grdOrders.FieldLayouts[0].RecordFilters.Add(processFilter);
        }

        private void UpdateStats()
        {
            if(!statExpander.IsExpanded)
                return;

            LogManager.GetCurrentClassLogger().Debug("Running stats.");
            ApplicationSettings appSettings = ApplicationSettings.Current;

            grpLateOrder.Header = (_countBy + "S LATE").ToUpper();
            grpOrdersBytType.Header = (_countBy + "S BY TYPE").ToUpper();
            grpOrdersToProcess.Header = (_countBy + "S DUE TODAY").ToUpper();

            var counts = new OrderSummaryCounts();

            foreach(var record in grdOrders.RecordManager.GetFilteredInDataRecords())
            {
                if (!record.IsDataRecord || !(record.DataItem is OrderStatusData))
                {
                    continue;
                }

                var order = record.DataItem as OrderStatusData;

                var count = 1;
                if (_countBy == CountBy.Part)
                {
                    count = order.PartQuantity ?? 0;
                }

                counts.TotalCount += count;

                var skipLateCount = order.Hold && !appSettings.IncludeHoldsInLateOrders;

                if (!skipLateCount)
                {
                    if (_lateBy == LateBy.Process)
                    {
                        if (order.CurrentProcessDue.HasValue)
                        {
                            var daysTillLate = _daysLateCalculator.GetDaysLate(order.CurrentProcessDue.Value.Date);

                            if(daysTillLate < 0) //aka late
                                counts.LateCount += count;
                            else if(daysTillLate == 0)
                                counts.Day1Count += count;
                            else if(daysTillLate == 1)
                                counts.Day2Count += count;
                            else if(daysTillLate == 2)
                                counts.Day3Count += count;

                            if(daysTillLate <= 0)
                                counts.DueTodayCount += count;
                        }
                    }
                    else
                    {
                        if(order.EstShipDate.HasValue)
                        {
                            var daysTillLate = _daysLateCalculator.GetDaysLate(order.EstShipDate.Value.Date);

                            if(daysTillLate < 0) //aka late
                                counts.LateCount += count;
                            else if(daysTillLate == 0)
                                counts.Day1Count += count;
                            else if(daysTillLate == 1)
                                counts.Day2Count += count;
                            else if(daysTillLate == 2)
                                counts.Day3Count += count;

                            if(daysTillLate <= 0)
                                counts.DueTodayCount += count;
                        }
                    }
                }

                var ot = (OrderType) order.OrderType;

                switch(ot)
                {
                    case OrderType.ReworkExt:
                        counts.ExtReworkCount += count;
                        break;
                    case OrderType.ReworkInt:
                        counts.IntReworkCount += count;
                        break;
                    case OrderType.ReworkHold:
                        counts.HoldCount += count;
                        break;
                    case OrderType.Quarantine:
                        counts.QuarantineCount += count;
                        break;
                }

                //if did not already count as hold because of order type and order is on hold then count now
                if(order.Hold && ot != OrderType.ReworkHold)
                    counts.HoldCount += count;
            }

            txtLate.Text = _countAsPercent ? counts.LateCountPercent.ToString("P0") : counts.LateCount.ToString("N0");
            txtDue1Day.Text = _countAsPercent ? counts.Day1CountPercent.ToString("P0") : counts.Day1Count.ToString("N0");
            txtDue2Day.Text = _countAsPercent ? counts.Day2CountPercent.ToString("P0") : counts.Day2Count.ToString("N0");
            txtDue3Day.Text = _countAsPercent ? counts.Day3CountPercent.ToString("P0") : counts.Day3Count.ToString("N0");

            txtQuarantine.Text = _countAsPercent ? counts.QuarantineCountPercent.ToString("P0") : counts.QuarantineCount.ToString("N0");
            txtInternalRework.Text = _countAsPercent ? counts.IntReworkCountPercent.ToString("P0") : counts.IntReworkCount.ToString("N0");
            txtExternalRework.Text = _countAsPercent ? counts.ExtReworkCountPercent.ToString("P0") : counts.ExtReworkCount.ToString("N0");
            txtHold.Text = _countAsPercent ? counts.HoldCountPercent.ToString("P0") : counts.HoldCount.ToString("N0");

            lblToProcess.Content = counts.DueTodayCount.ToString("N0");
        }

        public void SelectWO(int orderId)
        {
            try
            {
                if (!grdOrders.Records.Any())
                {
                    return;
                }

                grdOrders.ActiveRecord = null;
                grdOrders.SelectedItems.Records.Clear();

                if(orderId > 0)
                {
                    var orderRow = grdOrders.Records.OfType<DataRecord>()
                        .FirstOrDefault(r => (r.DataItem as OrderStatusData)?.WO == orderId);

                    if(orderRow != null)
                    {
                        grdOrders.SelectedItems.Records.Add(orderRow);
                        grdOrders.ActiveRecord = orderRow;
                        grdOrders.BringRecordIntoView(orderRow);
                    }
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error selecting order");
            }
        }

        private static void ShowLoginWarningMessage()
        {
            MessageBoxUtilities.ShowMessageBoxWarn(
                "You must be logged-in to view this information.",
                "Orders");
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Late3ColorConverter.Initialize();
            ProcessByDateBadgeConverter.Initialize();
        }

        private void grdOrders_SelectedItemsChanged(object sender, SelectedItemsChangedEventArgs e)
        {
            AfterSelectedRowChanged?.Invoke(this, EventArgs.Empty);
        }

        private void grdOrders_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ClickCount > 1)
                {
                    return;
                }

                var source = e.OriginalSource as DependencyObject;
                if(source == null)
                    return;

                var cvp = Infragistics.Windows.Utilities.GetAncestorFromType(source, typeof(CellValuePresenter), true) as CellValuePresenter;
                if(cvp?.Record == null || !cvp.Record.IsDataRecord)
                    return;

                //if clicked on filter image then filter by process
                var sourceAsFrameworkElement = source as FrameworkElement;

                if(sourceAsFrameworkElement?.Name == "filterByProcess")
                {
                    var order = cvp.Record.DataItem as OrderStatusData;

                    if(!string.IsNullOrEmpty(order?.CurrentProcess))
                        FilterByProcess(order.CurrentProcess);

                    return;
                }

                if(cvp.Field.Name == "Part")
                {
                    cvp.Record.Select();
                    e.Handled = true;

                    if (!SecurityManager.Current.IsValidUser)
                    {
                        ShowLoginWarningMessage();
                        return;
                    }

                    using(var p = new QuickViewPart())
                    {
                        using(var ta = new OrderStatusTableAdapter())
                        {
                            int? partId = ta.GetPartID(SelectedWO);

                            if(partId.HasValue)
                            {
                                p.PartID = partId.Value;
                                p.ShowDialog();
                            }
                        }
                    }
                }
                else if(cvp.Field.Name == "WO")
                {
                    cvp.Record.Select();
                    e.Handled = true;

                    if (!SecurityManager.Current.IsValidUser)
                    {
                        ShowLoginWarningMessage();
                        return;
                    }

                    using(var o = new QuickViewOrder())
                    {
                        if(SelectedWO > 0)
                        {
                            o.OrderID = SelectedWO;
                            o.ShowDialog();
                        }
                    }
                }
                else if(cvp.Field.Name == "CurrentProcess")
                {
                    var order = cvp.Record.DataItem as OrderStatusData;

                    if (order?.WorkStatus == ApplicationSettings.Current.WorkStatusInProcess)
                    {
                        cvp.Record.Select();
                        e.Handled = true;

                        if (!SecurityManager.Current.IsValidUser)
                        {
                            ShowLoginWarningMessage();
                            return;
                        }

                        if(SelectedWO > 0 && cvp.Value.ToString() != "NA")
                        {
                            var activity = new ProcessingActivity(SelectedWO,
                                new ActivityUser(SecurityManager.Current.UserID, Settings.Default.CurrentDepartment,
                                    Settings.Default.CurrentLine));

                            activity.Initialize();

                            using(var frm = new OrderProcessing2(activity) {Mode = OrderProcessing2.ProcessingMode.ViewOnly})
                                frm.ShowDialog();
                        }
                    }
                }
                else if(cvp.Field.Name == "WorkStatus" && source is Image)
                {
                    var order = cvp.Record.DataItem as OrderStatusData;

                    //wait for the the row select to occur before we call this
                    Application.Current.Sleep(o =>
                    {
                        var selectedOrder = o as OrderStatusData;

                        if (selectedOrder == null)
                        {
                            return;
                        }

                        if (!SecurityManager.Current.IsValidUser)
                        {
                            ShowLoginWarningMessage();
                            return;
                        }

                        var workStatus = ProcessActionsConverter.GetNextAction(selectedOrder, ApplicationSettings.Current);
                        ICommandBase cmd = null;

                        switch(workStatus)
                        {
                            case ProcessActionsConverter.WorkAction.Process:
                                if(!selectedOrder.InBatch)
                                    cmd = DWOSApp.MainForm.Commands.FindCommand <OrderProcessingCommand>();
                                else
                                    cmd = DWOSApp.MainForm.Commands.FindCommand <BatchProcessingCommand>();
                                break;
                            case ProcessActionsConverter.WorkAction.CheckIn:
                                cmd = DWOSApp.MainForm.Commands.FindCommand <PartCheckInCommand>();
                                break;
                            case ProcessActionsConverter.WorkAction.ControlInspection:
                                cmd = DWOSApp.MainForm.Commands.FindCommand <PartInspectionCommand>();
                                break;
                            case ProcessActionsConverter.WorkAction.FinalInspection:
                                cmd = DWOSApp.MainForm.Commands.FindCommand <COCCommand>();
                                break;
                            case ProcessActionsConverter.WorkAction.OrderReview:
                                cmd = DWOSApp.MainForm.Commands.FindCommand <OrderReviewCommand>();
                                break;
                            case ProcessActionsConverter.WorkAction.ImportExportReview:
                                cmd = DWOSApp.MainForm.Commands.FindCommand<ImportExportReviewCommand>();
                                break;
                        }

                        if(cmd is CommandBase && cmd.Refresh())
                            ((CommandBase) cmd).PerformOnClick();
                    }, order, .5);
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error displaying quick preview.");
            }
        }

        private void grdOrders_RecordFilterChanged(object sender, RecordFilterChangedEventArgs e) { UpdateStats(); }

        private void grdOrders_RecordFilterDropDownPopulating(object sender, RecordFilterDropDownPopulatingEventArgs e)
        {
            //Set custom filter command if on the customer column
            if(e.Field.Name == "Customer")
                e.DropDownItems.Add(new FilterDropDownItem(new CustomerFilterCommand(), MyCustomerFilter));
        }

        private void statExpander_Expanded(object sender, RoutedEventArgs e) { UpdateStats(); }

        private void optLateByOrder_Checked(object sender, RoutedEventArgs e)
        {
            if(_lateBy != LateBy.Order)
            {
                _lateBy = LateBy.Order;
                UpdateStats();
            }
        }

        private void optLateByProcess_Checked(object sender, RoutedEventArgs e)
        {
            if(_lateBy != LateBy.Process)
            {
                _lateBy = LateBy.Process;
                UpdateStats();
            }
        }

        private void optCountByOrder_Checked(object sender, RoutedEventArgs e)
        {
            if(_countBy != CountBy.Order)
            {
                _countBy = CountBy.Order;
                UpdateStats();
            }
        }

        private void optCountByProcess_Checked(object sender, RoutedEventArgs e)
        {
            if(_countBy != CountBy.Part)
            {
                _countBy = CountBy.Part;
                UpdateStats();
            }
        }

        private void grdOrders_CellChanged(object sender, CellChangedEventArgs e)
        {
            try
            {
                // Workaround  for TFS #10692
                var filterCell = e.Cell as FilterCell;

                if (filterCell == null)
                {
                    return;
                }

                var specialConditionValues = new List<string>()
                {
                    "(Blanks)",
                    "(NonBlanks)",
                    "(Custom)",
                    string.Empty // Custom or blank
                };

                var currentConditions = filterCell.RecordFilter
                    .Conditions
                    .OfType<ComparisonCondition>()
                    .ToList();

                var currentConditionIsSpecial = currentConditions.Count == 1 &&
                    specialConditionValues.Contains(currentConditions.First()?.Value?.ToString());

                var currentConditionIsCustom = currentConditions.Count > 1;

                // newCondition is trimmed because "(Custom)" may have spaces behind it.
                var newCondition = ((e.Editor as XamComboEditor)?.SelectedItem?.ToString() ?? string.Empty)
                    .Trim();

                var newConditionIsNormal = !specialConditionValues.Contains(newCondition);

                if ((currentConditionIsSpecial || currentConditionIsCustom) && newConditionIsNormal)
                {
                    foreach (var condition in currentConditions)
                    {
                        filterCell.RecordFilter.Conditions.Remove(condition);
                    }

                    filterCell.RecordFilter.Conditions.Add(new ComparisonCondition(ComparisonOperator.Equals, newCondition));
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing field filter.");
            }
        }

        #endregion
    }
}