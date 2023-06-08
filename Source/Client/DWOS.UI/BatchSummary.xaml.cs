using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Shared.Utilities;
using DWOS.UI.Utilities;
using DWOS.UI.Utilities.Convertors;
using Infragistics.Windows.Controls;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.DataPresenter.Events;
using Infragistics.Windows.Editors;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DWOS.UI
{
    /// <summary>
    ///     Interaction logic for BatchSummary.xaml
    /// </summary>
    public partial class BatchSummary : IBatchSummary
    {
        #region Fields

        public const string DATA_TYPE = "batchsummary";
        public event EventHandler LayoutError;
        private const int Version = 21;

        public event EventHandler AfterSelectedRowChanged;

        #endregion

        #region Properties

        private string V20LayoutFilePath => FileSystem.UserAppDataPath() + "\\" + Key + "_" + "_v20.dat";

        private string LayoutFilePath => FileSystem.UserAppDataPath() + "\\" + Key + "_" + $"_v{Version}.dat";

        public UserControl TabControl => this;

        public string TabName { get; set; }

        public string DataType => DATA_TYPE;

        public int SelectedBatch
        {
            get
            {
                var row = SelectedBatchRow;
                return row?.BatchID ?? -1;
            }
        }

        public string SelectedWorkStatus
        {
            get
            {
                var row = SelectedBatchRow;
                return row?.WorkStatus;
            }
        }

        public string SelectedLocation
        {
            get
            {
                var row = SelectedBatchRow;
                return row?.CurrentLocation;
            }
        }

        public int? SelectedLine
        {
            get
            {
                var row = SelectedBatchRow;
                return row.CurrentLineID;
            }
        }

        public int SelectedActiveTimerCount => SelectedBatchRow?.ActiveTimerCount ?? 0;

        public string Key { get; set; }

        private BatchStatusData SelectedBatchRow
        {
            get
            {
                if (grdOrders.SelectedItems.Records.Count == 1 && grdOrders.SelectedItems.Records[0].IsDataRecord)
                {
                    var dataRecord = grdOrders.SelectedItems.Records[0] as DataRecord;

                    if (dataRecord != null)
                    {
                        return dataRecord.DataItem as BatchStatusData;
                    }
                }

                return null;
            }
        }

        private BatchOrderStatusData SelectedOrderRow
        {
            get
            {
                if (grdOrders.SelectedItems.Records.Count == 1 && grdOrders.SelectedItems.Records[0].IsDataRecord)
                {
                    var dataRecord = grdOrders.SelectedItems.Records[0] as DataRecord;
                    if (dataRecord != null)
                    {
                        return dataRecord.DataItem as BatchOrderStatusData;
                    }
                }

                return null;
            }
        }

        #endregion

        #region Methods

        public BatchSummary()
        {
            InitializeComponent();

            Key = RandomUtils.GetRandomString(6);
            TabName = "Orders";
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

            grdOrders.DataSource = data.Batches;
        }

        public void RefreshData(WipData data)
        {
            var lastSelectedBatch = SelectedBatch;
            grdOrders.DataSource = data.Batches;

            //Force resort of the grid since it will not catch the changes to the data and the sorted fields will be incorrect
            var temp = new FieldSortDescription[grdOrders.DefaultFieldLayout.SortedFields.Count];
            grdOrders.DefaultFieldLayout.SortedFields.CopyTo(temp, 0);
            grdOrders.DefaultFieldLayout.SortedFields.Clear();

            foreach (var fieldSortDescription in temp)
                grdOrders.DefaultFieldLayout.SortedFields.Add(fieldSortDescription);

            if (lastSelectedBatch > 0)
            {
                SelectBatch(lastSelectedBatch);
            }
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

                if (!File.Exists(filePath) && File.Exists(V20LayoutFilePath))
                {
                    UpgradeLayoutFile(V20LayoutFilePath);
                }

                if (File.Exists(filePath))
                {
                    // Try and load the customization file
                    if (!LoadCustomization(filePath))
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
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading layout.");
            }
        }

        private void Setup()
        {
            grdOrders.Background = null;

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
                var filePath = LayoutFilePath;

                using(var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    grdOrders.SaveCustomizations(fs);
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error saving layout.");
            }
        }

        public void SelectBatch(int batchId)
        {
            try
            {
                if (!grdOrders.Records.Any())
                {
                    return;
                }

                grdOrders.ActiveRecord = null;
                grdOrders.SelectedItems.Records.Clear();

                if (batchId > 0)
                {
                    var isGrouped = grdOrders.Records.Any((r) => r is GroupByRecord);

                    var records = isGrouped
                        ? grdOrders.Records.OfType<GroupByRecord>().SelectMany(r => r.ChildRecords)
                        : grdOrders.Records;

                    var orderRecord = records.OfType<DataRecord>().FirstOrDefault(
                        (record) =>
                        {
                            var batchStatus = record.DataItem as BatchStatusData;
                            return batchStatus != null &&
                                   batchStatus.BatchID == batchId;
                        });

                    if (orderRecord != null)
                    {
                        grdOrders.SelectedItems.Records.Add(orderRecord);
                        grdOrders.ActiveRecord = orderRecord;
                        grdOrders.BringRecordIntoView(orderRecord);
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error selecting order");
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
                Version = Version
            };
        }

        private static void ShowLoginWarningMessage()
        {
            MessageBoxUtilities.ShowMessageBoxWarn(
                "You must be logged-in to view this information.",
                "Batches");
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ProcessByDateBadgeConverter.Initialize();
        }

        private void grdOrders_SelectedItemsChanged(object sender, SelectedItemsChangedEventArgs e)
        {
            AfterSelectedRowChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Field_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var cvp = sender as CellValuePresenter;
                if (cvp != null && cvp.Field.Name == "PartName")
                {
                    e.Handled = true;

                    cvp.Record.Select();

                    if (!SecurityManager.Current.IsValidUser)
                    {
                        ShowLoginWarningMessage();
                        return;
                    }

                    var order = SelectedOrderRow;

                    if (order != null)
                    {
                        using (var p = new QuickViewPart())
                        {
                            p.PartID = order.PartID;
                            p.ShowDialog();
                        }
                    }
                }
                else if (cvp != null && cvp.Field.Name == "OrderID")
                {
                    e.Handled = true;

                    cvp.Record.Select();

                    if (!SecurityManager.Current.IsValidUser)
                    {
                        ShowLoginWarningMessage();
                        return;
                    }

                    var order = SelectedOrderRow;

                    if (order != null)
                    {
                        using (var o = new QuickViewOrder())
                        {
                            o.OrderID = order.OrderID;
                            o.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error clicking a field in Batch Summary.");
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