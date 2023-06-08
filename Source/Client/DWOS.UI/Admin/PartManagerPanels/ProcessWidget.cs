using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ProcessPackageDatasetTableAdapters;
using DWOS.Shared;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using DWOS.Utilities.Validation;
using Infragistics.Win.UltraMessageBox;
using Infragistics.Win.UltraWinGrid;
using NLog;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Interop;
using DWOS.UI.Admin.ProcessSuggestionPicker;
using DWOS.Data.Order;

namespace DWOS.UI.Admin.PartManagerPanels
{
    public partial class ProcessWidget : UserControl
    {
        #region Fields

        private const string DISPLAY_COLUMN_STATUS = "Status";
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public event EventHandler<PriceChangedEventArgs> PriceChanged;

        private ProcessPicker _processPicker;
        private PartProcessDataManager _dataManager;
        private ProcessGridSettings _gridSettings;
        private readonly GridSettingsPersistence<ProcessGridSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<ProcessGridSettings>("PartProcess", new ProcessGridSettings());

        /// <summary>
        /// Counter that controls when to unregister/register process change event handlers.
        /// </summary>
        /// <remarks>
        /// Fix for an issue where this control registered events multiple times.
        /// </remarks>
        private int _processChangeHandlerCounter;

        #endregion

        #region Properties

        public PartsDataset.PartRow CurrentRecord
        {
            get;
            private set;
        }

        public PartsDataset Dataset
        {
            get;
            private set;
        }

        public bool ViewOnly
        {
            get;
            set;
        }

        public bool HasPricePoints => _dataManager.PricePoints.Any();

        public bool ShowLoadCapacityWeight
        {
            get;
            set;
        }

        public bool CanCurrentUserViewPrices =>
            SecurityManager.Current.IsInRole("PartsManager") || SecurityManager.Current.IsInRole("OrderEntry");

        #endregion

        #region Methods

        public ProcessWidget()
        {
            InitializeComponent();
            ShowLoadCapacityWeight = true;
        }

        public void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            var idv                    = new ImageDisplayValidator(new UnansweredQuestionsValidator(grdProcess), errProvider);
            idv.Validator.ErrorMessage = "The process '{0}' may have unanswered questions that are required to be completed.\rPlease review the process questions.";

            // Validation style w/ UltraGrid looks bad
            idv.InvalidStyleSetName = string.Empty;
            idv.RequiredStyleSetName = string.Empty;

            manager.Add(idv);
        }

        public void LoadData(PartsDataset dataset)
        {
            Dataset = dataset;
            btnAddProcess.Enabled = !ViewOnly;
            btnDeleteProcess.Enabled = !ViewOnly;
            btnEditProcess.Enabled = !ViewOnly;
            btnUp.Enabled = !ViewOnly;
            btnDown.Enabled = !ViewOnly;
            btnAddProcessPackage.Enabled = !ViewOnly;
            btnSaveAsProcessPackage.Enabled = !ViewOnly;

            btnSettings.Visible = ApplicationSettings.Current.PartPricingType == PricingType.Process;
        }

        public void LoadRow(PartsDataset.PartRow part)
        {
            CurrentRecord = part;

            grdProcess.Selected.Rows.Clear();
            grdProcess.DataSource = null;
            _dataManager = null;

            if (CurrentRecord != null)
            {
                _dataManager = new PartProcessDataManager(Dataset, new PriceUnitPersistence(), CurrentRecord);
                grdProcess.DataSource = _dataManager.DataTable;
            }
        }

        public void SaveRow()
        {
            if (CurrentRecord == null || _dataManager == null)
            {
                return;
            }

            _dataManager.SaveRow();
        }

        public void RegisterCommands(ICommandManager commands)
        {
            commands.AddCommand("DeleteProcess", new DeleteProcessCommand(this));
            commands.AddCommand("EditProcess", new EditProcessCommand(this));
        }

        public void UpdatePrices(PricingStrategy pricingStrategy, decimal newTotal)
        {
            try
            {
                UnregisterProcessChangeHandlers();
                _dataManager.UpdatePrices(pricingStrategy, newTotal);
            }
            finally
            {
                RegisterProcessChangeHandlers();
            }
        }

        private void UnregisterProcessChangeHandlers()
        {
            if (_processChangeHandlerCounter == 0)
            {
                grdProcess.BeforeCellUpdate -= grdProcess_BeforeCellUpdate;
                grdProcess.AfterCellUpdate -= grdProcess_AfterCellUpdate;
            }

            _processChangeHandlerCounter++;
        }

        private void RegisterProcessChangeHandlers()
        {
            if (_processChangeHandlerCounter > 0)
            {
                _processChangeHandlerCounter--;
            }

            if (_processChangeHandlerCounter == 0)
            {
                grdProcess.BeforeCellUpdate += grdProcess_BeforeCellUpdate;
                grdProcess.AfterCellUpdate += grdProcess_AfterCellUpdate;
            }
        }

        public void UpdateWeights(decimal newPartWeight)
        {
            try
            {
                UnregisterProcessChangeHandlers();
                _dataManager.UpdateWeights(newPartWeight);
            }
            finally
            {
                RegisterProcessChangeHandlers();
            }
        }

        public void ClearWeights()
        {
            try
            {
                UnregisterProcessChangeHandlers();
                _dataManager.ClearWeights();
            }
            finally
            {
                RegisterProcessChangeHandlers();
            }
        }

        private void AddProcess()
        {
            if (CurrentRecord == null)
            {
                return;
            }

            _log.Info("Adding a new process to part: " + CurrentRecord.PartID);

            if (ApplicationSettings.Current.UseProcessSuggestions)
            {
                var processWindow = new SelectProcessesWindow();
                processWindow.LoadData(CurrentRecord.IsManufacturerIDNull() ? string.Empty : CurrentRecord.ManufacturerID);
                var helper = new WindowInteropHelper(processWindow) { Owner = DWOSApp.MainForm.Handle };

                if (processWindow.ShowDialog() ?? false)
                {
                    using (new UsingGridLoad(grdProcess))
                    {
                        _dataManager.AddProcesses(processWindow.GetProcesses());
                    }

                    this.DisplayProcessNotes();
                    OnAllPricesChanged();
                }

                GC.KeepAlive(helper);
            }
            else
            {
                //Create Add Process Dropdown Box
                if (this._processPicker == null)
                    _processPicker = new ProcessPicker();

                _processPicker.LoadCustomerAliases(CurrentRecord.CustomerID);

                if (_processPicker.ShowDialog(this) == DialogResult.OK)
                {
                    using (new UsingGridLoad(grdProcess))
                    {
                        _dataManager.AddProcesses(_processPicker.SelectedProcesses);
                    }

                    this.DisplayProcessNotes();
                    OnAllPricesChanged();
                }
            }
        }

        private void DisplayProcessNotes()
        {
            foreach (var processNote in _dataManager.NotesToShow)
            {
                if (string.IsNullOrEmpty(processNote.Notes))
                {
                    continue;
                }

                try // Continue showing process notes if one fails
                {
                    var messageinfo = new UltraMessageBoxInfo
                    {
                        Caption = About.ApplicationName,
                        Header = "Process Notes",
                        Icon = MessageBoxIcon.Warning,
                        ShowHelpButton = Infragistics.Win.DefaultableBoolean.False,
                        Text = processNote.Notes
                    };

                    this.messageBoxProcessNotes.ShowMessageBox(messageinfo);
                }
                catch (Exception exc)
                {
                    string errorMsg = "Error displaying process notes for part process {0}";
                    _log.Error(exc, errorMsg, processNote.PartProcessID);
                }
            }
        }

        private void MoveProcessUp()
        {
            var selectedRows = grdProcess.Selected.Rows;

            if (selectedRows == null || selectedRows.Count != 1 || !selectedRows[0].IsDataRow)
            {
                return;
            }

            DataRow selectedProcess = (selectedRows[0].ListObject as DataRowView)?.Row;
            if (selectedProcess != null)
            {
                _dataManager.MoveProcessUp(selectedProcess);
                RefreshSort();
            }
        }

        private void MoveProcessDown()
        {
            var selectedRows = grdProcess.Selected.Rows;

            if (selectedRows == null || selectedRows.Count != 1 || !selectedRows[0].IsDataRow)
            {
                return;
            }

            DataRow selectedProcess = (selectedRows[0].ListObject as DataRowView)?.Row;
            if (selectedProcess != null)
            {
                _dataManager.MoveProcessDown(selectedProcess);
                RefreshSort();
            }
        }

        private void RefreshSort()
        {
            var band = grdProcess.DisplayLayout.Bands[0];
            band.SortedColumns.Clear();
            band.SortedColumns.Add(PartProcessDataManager.COLUMN_STEP_ORDER, false);
        }

        private void AddProcessPackage()
        {
            if (CurrentRecord == null)
            {
                return;
            }

            int selectedProcessPackageID = -1;

            using (var processPackageManager = new ProcessPackages())
            {
                // If user selects a package, add processes from the package.
                if (processPackageManager.ShowDialog(this) == DialogResult.OK)
                {
                    selectedProcessPackageID = processPackageManager.SelectedProcessPackage?.ProcessPackageID ?? 0;
                }
            }

            if (selectedProcessPackageID > 0)
            {
                _log.Info("Adding a process package to part: " + CurrentRecord.PartID);

                using (var taPackageProcesses = new ProcessPackage_ProcessesTableAdapter())
                {
                    var dtPPP = new ProcessPackageDataset.ProcessPackage_ProcessesDataTable();
                    taPackageProcesses.FillByProcessPackageID(dtPPP, selectedProcessPackageID);

                    using (new UsingGridLoad(grdProcess))
                    {
                        _log.Info("Adding process package {0}.", selectedProcessPackageID);
                        _dataManager.AddProcesses(dtPPP);
                    }

                    this.DisplayProcessNotes();
                    OnAllPricesChanged();
                }
            }
        }

        private void SaveAsProcessPackage()
        {
            if (_dataManager.DataTable.Rows.Count == 0)
            {
                return;
            }

            using (var tb = new TextBoxForm())
            {
                tb.Text = "Process Package Name";
                tb.FormLabel.Text = "Process Package Name";
                tb.FormTextBox.Text = "New Process Package";
                tb.FormTextBox.MaxLength = 50;
                tb.FormTextBox.Focus();
                tb.FormTextBox.SelectAll();

                if (tb.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(tb.FormTextBox.Text))
                {
                    _dataManager.SaveAsProcessPackage(tb.FormTextBox.Text);
                }
            }
        }

        private void SyncDialogResult(IEnumerable<Sales.IPricePointDialogItem> dialogPricePoints)
        {
            if (dialogPricePoints == null)
            {
                return;
            }

            SaveRow(); // sync all changes that were made before opening dialog

            var existingPriceRows = new List<PartsDataset.PartProcessVolumePriceRow>();
            foreach (var process in CurrentRecord.GetPartProcessRows())
            {
                existingPriceRows.AddRange(process.GetPartProcessVolumePriceRows().Where(i => i.IsValidState()));
            }

            var sortedDialogPoints = dialogPricePoints
                .OrderBy(point => point)
                .ToList();

            foreach (var process in CurrentRecord.GetPartProcessRows())
            {
                var existingAmountsForProcess = new LinkedList<decimal>(existingPriceRows
                   .Where(row => row.PartProcessID == process.PartProcessID)
                   .Select(row => row.Amount)
                   .OrderByDescending(d => d));

                decimal defaultAmount = existingAmountsForProcess.Count > 0 ?
                    existingAmountsForProcess.Last.Value :
                    0M;

                foreach (var dialogPoint in sortedDialogPoints)
                {
                    var minValue = dialogPoint.MinValueString;
                    var maxValue = dialogPoint.MaxValueString;
                    string priceUnit = dialogPoint.CalculateBy.ToString();

                    var newPriceRow = Dataset.PartProcessVolumePrice.NewPartProcessVolumePriceRow();
                    if (!string.IsNullOrEmpty(minValue))
                    {
                        newPriceRow.MinValue = minValue;
                    }
                    else
                    {
                        newPriceRow.SetMinValueNull();
                    }

                    if (!string.IsNullOrEmpty(maxValue))
                    {
                        newPriceRow.MaxValue = maxValue;
                    }
                    else
                    {
                        newPriceRow.SetMaxValueNull();
                    }

                    newPriceRow.PriceUnit = priceUnit;
                    newPriceRow.PartProcessRow = process;

                    if (existingAmountsForProcess.Count > 0)
                    {
                        newPriceRow.Amount = existingAmountsForProcess.First.Value;
                        existingAmountsForProcess.RemoveFirst();
                    }
                    else
                    {
                        newPriceRow.Amount = defaultAmount;
                    }

                    Dataset.PartProcessVolumePrice.AddPartProcessVolumePriceRow(newPriceRow);
                }
            }

            foreach (var existingPrice in existingPriceRows)
            {
                existingPrice.Delete();
            }

            LoadRow(CurrentRecord);
        }

        private void RefreshGridSettings()
        {
            if (_gridSettings == null)
            {
                return;
            }

            _gridSettings.Columns.Clear();
            _gridSettings.HiddenColumns.Clear();

            var band = grdProcess.DisplayLayout.Bands[0];

            foreach (var column in band.Columns)
            {
                if (column.ExcludeFromColumnChooser == ExcludeFromColumnChooser.True && column.Hidden)
                {
                    continue;
                }

                if (column.Hidden)
                {
                    _gridSettings.HiddenColumns.Add(column.Key);
                }
                else
                {
                    _gridSettings.HiddenColumns.Remove(column.Key);
                    _gridSettings.Columns.Add(column.Key, new UltraGridBandSettings.ColumnSettings
                    {
                        Order = column.Header.VisiblePosition,
                        Width = column.Width
                    });
                }
            }

            if (!CanCurrentUserViewPrices)
            {
                _log.Debug("Current user is not setup to view prices - skipping HidePrices refresh.");
            }
            else if (_dataManager.PricePoints.Any()) // otherwise, does not have group
            {
                _gridSettings.HidePrices = true;

                foreach (var pricePoint in _dataManager.PricePoints)
                {
                    string columnName = PartProcessDataManager.TempTableColumnName(pricePoint);
                    if (!band.Columns[columnName].Hidden)
                    {
                        _gridSettings.HidePrices = false;
                        break;
                    }
                }
            }
            else
            {
                _gridSettings.HidePrices = false;
            }

            if (_gridSettings.HidePrices)
            {
                foreach (var pricePoint in _dataManager.PricePoints)
                {
                    string columnKey = PartProcessDataManager.TempTableColumnName(pricePoint);
                    _gridSettings.HiddenColumns.Remove(columnKey);
                }

                var remainingColumns = _gridSettings
                    .HiddenColumns
                    .Where(c => PartProcessDataManager.IsPriceColumn(c))
                    .ToList();

                foreach (var columnKey in remainingColumns)
                {
                    _gridSettings.HiddenColumns.Remove(columnKey);
                }
            }
        }

        private void LoadGridSettings()
        {
            if (_gridSettings == null)
            {
                _gridSettings = _gridSettingsPersistence.LoadSettings();
            }

            var band = grdProcess.DisplayLayout.Bands[0];

            foreach (var columnKey in _gridSettings.HiddenColumns)
            {
                if (band.Columns.Exists(columnKey))
                {
                    band.Columns[columnKey].Hidden = true;
                }
            }

            if (_gridSettings.HidePrices)
            {
                foreach (var pricePoint in _dataManager.PricePoints)
                {
                    string columnName = PartProcessDataManager.TempTableColumnName(pricePoint);
                    band.Columns[columnName].Hidden = true;
                }
            }

            foreach (var columnKeyValue in _gridSettings.Columns)
            {
                if (band.Columns.Exists(columnKeyValue.Key))
                {
                    var column = band.Columns[columnKeyValue.Key];
                    column.Header.VisiblePosition = columnKeyValue.Value.Order;
                    column.Width = columnKeyValue.Value.Width;
                }
            }
        }

        private static PricePointConversion ConvertPricePoints(IEnumerable<PricePointData> pricePoints)
        {
            var quantityItems = new List<Sales.PricePointEditContext.QuantityPricePointItem>();
            var weightItems = new List<Sales.PricePointEditContext.WeightPricePointItem>();

            foreach (var pricePoint in pricePoints)
            {
                switch (OrderPrice.GetPriceByType(pricePoint.PriceUnit))
                {
                    case PriceByType.Quantity:
                        quantityItems.Add(new Sales.PricePointEditContext.QuantityPricePointItem(pricePoint.PriceUnit,
                            pricePoint.MinQuantity ?? 0,
                            pricePoint.MaxQuantity));
                        break;
                    case PriceByType.Weight:
                        weightItems.Add(new Sales.PricePointEditContext.WeightPricePointItem(pricePoint.PriceUnit,
                            pricePoint.MinWeight ?? 0M,
                            pricePoint.MaxWeight));
                        break;
                    default:
                        // Unsupported price by type
                        break;
                }
            }

            return new PricePointConversion
            {
                QuantityItems = quantityItems,
                WeightItems = weightItems
            };
        }

        private static string GetCaption(PriceUnitPersistence priceUnitPersistence, PricePointData pricePoint)
        {
            string caption;

            var priceByType = OrderPrice.GetPriceByType(pricePoint.PriceUnit);

            // Assumption - if MinQuantity is not present, neither is MaxQuantity
            if (priceByType == PriceByType.Quantity)
            {
                if (pricePoint.MinQuantity.HasValue && pricePoint.MaxQuantity.HasValue)
                {
                    caption = $"{priceUnitPersistence.GetDisplayText(pricePoint.PriceUnit)}({pricePoint.MinQuantity}-{pricePoint.MaxQuantity})";
                }
                else if (pricePoint.MinQuantity.HasValue)
                {
                    caption = $"{priceUnitPersistence.GetDisplayText(pricePoint.PriceUnit)} ({pricePoint.MinQuantity}+)";
                }
                else
                {
                    caption = $"{priceUnitPersistence.GetDisplayText(pricePoint.PriceUnit)}";
                }
            }
            else if (priceByType == PriceByType.Weight)
            {
                if (pricePoint.MinWeight.HasValue && pricePoint.MaxWeight.HasValue)
                {
                    caption = $"{priceUnitPersistence.GetDisplayText(pricePoint.PriceUnit)}({pricePoint.MinWeight}-{pricePoint.MaxWeight} lbs.)";
                }
                else if (pricePoint.MinWeight.HasValue)
                {
                    caption = $"{priceUnitPersistence.GetDisplayText(pricePoint.PriceUnit)} ({pricePoint.MinWeight}+ lbs.)";
                }
                else
                {
                    caption = $"{priceUnitPersistence.GetDisplayText(pricePoint.PriceUnit)}";
                }
            }
            else
            {
                caption = $"{priceUnitPersistence.GetDisplayText(pricePoint.PriceUnit)}";
            }

            return caption;
        }

        private void HandleGridChange()
        {
            try
            {
                RefreshGridSettings();

                if (_gridSettings != null)
                {
                    _gridSettingsPersistence.SaveSettings(_gridSettings);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after showing/hiding/moving a column.");
            }
        }

        #endregion

        #region Events

        private void btnUp_Click(object sender, EventArgs e)
        {
            try
            {
                MoveProcessUp();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error moving process up.");
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            try
            {
                MoveProcessDown();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error moving process down.");
            }
        }

        private void btnAddProcess_Click(object sender, EventArgs e)
        {
            try
            {
                AddProcess();
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error adding the process to the part.", exc);
            }
        }
        private void btnAddProcessPackage_Click(object sender, EventArgs e)
        {
            try
            {
                AddProcessPackage();
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error adding the process to the part.", exc);
            }
        }

        private void btnSaveAsProcessPackage_Click(object sender, EventArgs e)
        {
            try
            {
                SaveAsProcessPackage();
            }
            catch (Exception exc)
            {
                string errorMsg = "Error saving process package.";
                _log.Error(exc, errorMsg);
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                if (ApplicationSettings.Current.PartPricingType != PricingType.Process)
                {
                    return;
                }

                var usesVolumePricing = _dataManager
                    .PricePoints
                    .All(point => point.MinQuantity.HasValue || point.MinWeight.HasValue);

                if (!usesVolumePricing && !ApplicationSettings.Current.EnableVolumePricing)
                {
                    const string msg = "Unable to show pricing options for this part.\n" +
                        "Please enable volume pricing discounts and try again.";

                    MessageBoxUtilities.ShowMessageBoxOK(msg, "Parts Manager");
                    return;
                }

                if (CurrentRecord.GetPartProcessRows().Length == 0)
                {
                    const string msg = "Please add a process before configuring pricing options.";

                    MessageBoxUtilities.ShowMessageBoxOK(msg, "Parts Manager");
                    return;
                }

                // Prepare dialog data
                IEnumerable<Sales.PricePointEditContext.QuantityPricePointItem> quantityItems;
                IEnumerable<Sales.PricePointEditContext.WeightPricePointItem> weightItems;
                if (usesVolumePricing)
                {
                    (quantityItems, weightItems) = ConvertPricePoints(_dataManager.PricePoints);
                }
                else
                {
                    var customerPricePoints = _dataManager.GenerateFromCustomerDefaults(CurrentRecord.CustomerID);

                    if (customerPricePoints == null || customerPricePoints.Count == 0)
                    {
                        (quantityItems, weightItems) = ConvertPricePoints(_dataManager.GenerateFromSystemDefaults());
                    }
                    else
                    {
                        (quantityItems, weightItems) = ConvertPricePoints(customerPricePoints);
                    }
                }

                // Show dialog
                var window = new Sales.PricePointDialog(Sales.PricePointEditContext.From(CurrentRecord.Name, quantityItems, weightItems));
                var helper = new WindowInteropHelper(window) { Owner = DWOSApp.MainForm.Handle };

                if (window.ShowDialog().GetValueOrDefault(false))
                {
                    SyncDialogResult(window.PricePoints);
                }

                GC.KeepAlive(helper);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error showing part process price settings.");
            }
        }

        private void grdProcess_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                grdProcess.AfterColPosChanged -= grdProcess_AfterColPosChanged;
                grdProcess.AfterSortChange -= grdProcess_AfterSortChange;

                var currencyFormat = $"{{0:C{ApplicationSettings.Current.PriceDecimalPlaces}}}";

                var displayLoadCapacity = ApplicationSettings.Current.UseLoadCapacity;
                var displayLeadTime = ApplicationSettings.Current.UsingLeadTimeScheduling;

                var displayLayout = grdProcess.DisplayLayout;
                if (!displayLayout.ValueLists.Exists("LeadTimeType"))
                {
                    var shipmentPackageTypeList = displayLayout.ValueLists.Add("LeadTimeType");
                    shipmentPackageTypeList.ValueListItems.Add("load", "per load");
                    shipmentPackageTypeList.ValueListItems.Add("piece", "per piece");
                }

                var band = grdProcess.DisplayLayout.Bands[0];

                band.SortedColumns.Add(PartProcessDataManager.COLUMN_STEP_ORDER, false);

                band.Columns[PartProcessDataManager.COLUMN_PART_PROCESS_ID].Hidden = true;
                band.Columns[PartProcessDataManager.COLUMN_APPROVED].Hidden = true;
                band.Columns[PartProcessDataManager.COLUMN_HAS_CLOSED_INSPECTIONS].Hidden = true;
                band.Columns[PartProcessDataManager.COLUMN_UNANSWERED_QUESTIONS].Hidden = true;
                band.Columns[PartProcessDataManager.COLUMN_SHOWED_WARNING].Hidden = true;

                band.Columns[PartProcessDataManager.COLUMN_STEP_ORDER].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                band.Columns[PartProcessDataManager.COLUMN_PART_PROCESS_ID].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                band.Columns[PartProcessDataManager.COLUMN_APPROVED].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                band.Columns[PartProcessDataManager.COLUMN_HAS_CLOSED_INSPECTIONS].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                band.Columns[PartProcessDataManager.COLUMN_UNANSWERED_QUESTIONS].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                band.Columns[PartProcessDataManager.COLUMN_SHOWED_WARNING].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;

                band.Columns[PartProcessDataManager.COLUMN_STEP_ORDER].Header.Caption = string.Empty;

                band.Columns[PartProcessDataManager.COLUMN_ALIAS].Header.Caption = "Alias";
                band.Columns[PartProcessDataManager.COLUMN_ALIAS].CellActivation = Activation.NoEdit;
                band.Columns[PartProcessDataManager.COLUMN_ALIAS].TabStop = false;
                band.Columns[PartProcessDataManager.COLUMN_ALIAS].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;

                band.Columns[PartProcessDataManager.COLUMN_PROCESS].Header.Caption = "Process";
                band.Columns[PartProcessDataManager.COLUMN_PROCESS].CellActivation = Activation.NoEdit;
                band.Columns[PartProcessDataManager.COLUMN_PROCESS].TabStop = false;

                // Load Capacity
                band.Columns[PartProcessDataManager.COLUMN_LOAD_CAPACITY_QUANTITY].Hidden = !displayLoadCapacity;
                band.Columns[PartProcessDataManager.COLUMN_LOAD_CAPACITY_QUANTITY].MaskInput = "nnn,nnn,nnn";
                band.Columns[PartProcessDataManager.COLUMN_LOAD_CAPACITY_QUANTITY].MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiteralsWithPadding;
                band.Columns[PartProcessDataManager.COLUMN_LOAD_CAPACITY_QUANTITY].MinValue = 0;
                band.Columns[PartProcessDataManager.COLUMN_LOAD_CAPACITY_QUANTITY].Header.Caption = "Capacity Qty.";

                band.Columns[PartProcessDataManager.COLUMN_LOAD_CAPACITY_WEIGHT].Hidden = !displayLoadCapacity || !ShowLoadCapacityWeight;
                band.Columns[PartProcessDataManager.COLUMN_LOAD_CAPACITY_WEIGHT].MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiteralsWithPadding;
                band.Columns[PartProcessDataManager.COLUMN_LOAD_CAPACITY_WEIGHT].MaskInput = string.Format("nnnn.{0} lbs",
                        string.Concat(Enumerable.Repeat("n", ApplicationSettings.Current.WeightDecimalPlaces)));

                if (displayLoadCapacity)
                {
                    band.Columns[PartProcessDataManager.COLUMN_LOAD_CAPACITY_QUANTITY].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                    band.Columns[PartProcessDataManager.COLUMN_LOAD_CAPACITY_WEIGHT].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                }
                else
                {
                    band.Columns[PartProcessDataManager.COLUMN_LOAD_CAPACITY_QUANTITY].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                    band.Columns[PartProcessDataManager.COLUMN_LOAD_CAPACITY_WEIGHT].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                }

                band.Columns[PartProcessDataManager.COLUMN_LOAD_CAPACITY_WEIGHT].MinValue = 0M;
                band.Columns[PartProcessDataManager.COLUMN_LOAD_CAPACITY_WEIGHT].Header.Caption = "Capacity Weight";

                // Lead Time
                band.Columns[PartProcessDataManager.COLUMN_LEAD_TIME_HOURS].Hidden = !displayLeadTime;
                band.Columns[PartProcessDataManager.COLUMN_LEAD_TIME_HOURS].MaskInput = "nnn.nnnn";
                band.Columns[PartProcessDataManager.COLUMN_LEAD_TIME_HOURS].MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiteralsWithPadding;
                band.Columns[PartProcessDataManager.COLUMN_LEAD_TIME_HOURS].MinValue = 0;
                band.Columns[PartProcessDataManager.COLUMN_LEAD_TIME_HOURS].MaxValue = 999.9999M;
                band.Columns[PartProcessDataManager.COLUMN_LEAD_TIME_HOURS].Header.Caption = "Lead Time Hours";

                band.Columns[PartProcessDataManager.COLUMN_LEAD_TIME_TYPE].Hidden = !displayLeadTime;
                band.Columns[PartProcessDataManager.COLUMN_LEAD_TIME_TYPE].Header.Caption = "Lead Time Type";
                band.Columns[PartProcessDataManager.COLUMN_LEAD_TIME_TYPE].ValueList = band.Layout.ValueLists["LeadTimeType"];
                band.Columns[PartProcessDataManager.COLUMN_LEAD_TIME_TYPE].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;

                if (displayLeadTime)
                {
                    band.Columns[PartProcessDataManager.COLUMN_LEAD_TIME_HOURS].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                    band.Columns[PartProcessDataManager.COLUMN_LEAD_TIME_TYPE].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                }
                else
                {
                    band.Columns[PartProcessDataManager.COLUMN_LEAD_TIME_HOURS].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                    band.Columns[PartProcessDataManager.COLUMN_LEAD_TIME_TYPE].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                }

                // Status
                var unapprovedImgColumn = band.Columns.Add(DISPLAY_COLUMN_STATUS, string.Empty);
                unapprovedImgColumn.DataType = typeof(Image);
                unapprovedImgColumn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                unapprovedImgColumn.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                unapprovedImgColumn.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                unapprovedImgColumn.TabStop = false;
                unapprovedImgColumn.ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;

                // Setup default positions for columns
                unapprovedImgColumn.Header.VisiblePosition = 0;
                band.Columns[PartProcessDataManager.COLUMN_STEP_ORDER].Header.VisiblePosition = 1;
                band.Columns[PartProcessDataManager.COLUMN_ALIAS].Header.VisiblePosition = 2;
                band.Columns[PartProcessDataManager.COLUMN_PROCESS].Header.VisiblePosition = 3;

                // Setup price point columns
                if (_dataManager.PricePoints.Any())
                {
                    var displayPrices = CanCurrentUserViewPrices;

                    var priceUnitPersistence = new PriceUnitPersistence();

                    foreach (var pricePoint in _dataManager.PricePoints)
                    {
                        string columnName = PartProcessDataManager.TempTableColumnName(pricePoint);
                        string caption = GetCaption(priceUnitPersistence, pricePoint);
                        var pricePointColumn = band.Columns[columnName];
                        var totalSummary = band.Summaries.Add(SummaryType.Sum, pricePointColumn);
                        totalSummary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
                        totalSummary.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                        totalSummary.Appearance.ForeColor = Color.Black;
                        totalSummary.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
                        totalSummary.DisplayFormat = currencyFormat;

                        pricePointColumn.Format = $"$#,##0.{string.Concat(Enumerable.Repeat('#', ApplicationSettings.Current.PriceDecimalPlaces))}";

                        pricePointColumn.MaskInput = "{currency:6." + ApplicationSettings.Current.PriceDecimalPlaces + "}";

                        pricePointColumn.MinValue = -214748.3648M;
                        pricePointColumn.MaxValue = 214748.3647M;

                        pricePointColumn.Header.Caption = caption;

                        if (!displayPrices)
                        {
                            pricePointColumn.ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                            pricePointColumn.Hidden = true;
                        }
                    }
                }

                LoadGridSettings();
            }
            finally
            {
                grdProcess.AfterColPosChanged += grdProcess_AfterColPosChanged;
                grdProcess.AfterSortChange += grdProcess_AfterSortChange;
            }
        }

        private void grdProcess_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                if (!e.Row.IsDataRow)
                {
                    return;
                }

                DataRow currentProcess = (e.Row.ListObject as DataRowView)?.Row;

                if (currentProcess == null)
                {
                    return;
                }

                UnregisterProcessChangeHandlers();

                if (e.Row.Cells.Exists(DISPLAY_COLUMN_STATUS))
                {
                    var isApproved = (currentProcess[PartProcessDataManager.COLUMN_APPROVED] as bool?) ?? false;
                    var hasInactiveInspections = currentProcess[PartProcessDataManager.COLUMN_HAS_CLOSED_INSPECTIONS] as bool? ?? false;

                    if (!isApproved || hasInactiveInspections)
                    {
                        e.Row.Cells[DISPLAY_COLUMN_STATUS].Value = Properties.Resources.RoundDashRed_32;
                    }
                    else if (_dataManager.IsMissingPricingData(currentProcess))
                    {
                        e.Row.Cells[DISPLAY_COLUMN_STATUS].Value = Properties.Resources.Warning_16;
                    }
                    else
                    {
                        e.Row.Cells[DISPLAY_COLUMN_STATUS].Value = Properties.Resources.Process_16;
                    }
                }

                var hasCellsForUnansweredStyling = e.Row.Cells.Exists(PartProcessDataManager.COLUMN_UNANSWERED_QUESTIONS) &&
                    e.Row.Cells.Exists(PartProcessDataManager.COLUMN_ALIAS) &&
                    e.Row.Cells.Exists(PartProcessDataManager.COLUMN_PROCESS);

                if (hasCellsForUnansweredStyling)
                {
                    int unanswered = (currentProcess[PartProcessDataManager.COLUMN_UNANSWERED_QUESTIONS] as int?) ?? 0;

                    if (unanswered == 0)
                    {
                        e.Row.Cells[PartProcessDataManager.COLUMN_ALIAS].Appearance.ForeColor = Color.Black;
                        e.Row.Cells[PartProcessDataManager.COLUMN_PROCESS].Appearance.ForeColor = Color.Black;
                    }
                    else
                    {
                        e.Row.Cells[PartProcessDataManager.COLUMN_ALIAS].Appearance.ForeColor = Color.Red;
                        e.Row.Cells[PartProcessDataManager.COLUMN_PROCESS].Appearance.ForeColor = Color.Red;
                    }
                }
            }
            finally
            {
                RegisterProcessChangeHandlers();
            }
        }

        private void grdProcess_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e) =>
            HandleGridChange();

        private void grdProcess_AfterSortChange(object sender, BandEventArgs e) =>
            HandleGridChange();

        private void grdProcess_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            try
            {
                if (!e.Cell.Row.IsDataRow)
                {
                    return;
                }

                string columnKey = e.Cell.Column.Key;

                if (PartProcessDataManager.IsPriceColumn(columnKey))
                {
                    // Check against minimum price
                    var matchingTempRow = (e.Cell.Row.ListObject as DataRowView)?.Row;
                    var matchingPartProcess = this._dataManager.GetPartProcess(matchingTempRow);
                    var process = matchingPartProcess.ProcessRow;
                    var newValue = (e.NewValue as decimal?) ?? 0M;
                    var minimumValue = process.IsMinPriceNull() ? 0M : process.MinPrice;

                    if (newValue < minimumValue)
                    {
                        const string msgFormat = "The price you entered ({0}) is less than the process's minimum price.\n" +
                            "Do you want to override the minimum price of {1}?";

                        string msg = string.Format(msgFormat,
                            newValue.ToString(OrderPrice.CurrencyFormatString),
                            minimumValue.ToString(OrderPrice.CurrencyFormatString));

                        e.Cancel = MessageBoxUtilities.ShowMessageBoxYesOrNo(msg, "Parts Manager") != DialogResult.Yes;
                    }
                }
                else if (columnKey == PartProcessDataManager.COLUMN_LOAD_CAPACITY_WEIGHT && CurrentRecord != null && CurrentRecord.IsWeightNull())
                {
                    // Prevent edits to load capacity weight when part weight is null
                    var msg = "Please enter a part weight before entering load capacity weights.";
                    MessageBoxUtilities.ShowMessageBoxWarn(msg, "Parts Manager");
                    e.Cancel = true;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error before updating per-process price.");
            }
        }

        private void grdProcess_AfterCellUpdate(object sender, CellEventArgs e)
        {
            bool removedListeners = false;

            try
            {
                if (!e.Cell.Row.IsDataRow)
                {
                    return;
                }

                var columnKey = e.Cell.Column.Key;

                if (PartProcessDataManager.IsPriceColumn(columnKey))
                {
                    // Sync price data
                    _dataManager.SyncPricePointData = true;
                    PricePointData match = null;

                    foreach (var pricePoint in _dataManager.PricePoints)
                    {
                        if (PartProcessDataManager.TempTableColumnName(pricePoint) == columnKey)
                        {
                            match = pricePoint;
                            break;
                        }
                    }

                    // Only trigger if match is the first price point among points
                    // with its strategy.
                    if (match != null)
                    {
                        var matchPriceStrategy = OrderPrice.GetPricingStrategy(match.PriceUnit);

                        PricePointData firstPricePoint = _dataManager
                            .PricePoints
                            .Where(point => OrderPrice.GetPricingStrategy(point.PriceUnit) == matchPriceStrategy)
                            .OrderBy(i => i)
                            .FirstOrDefault();

                        if (match.Equals(firstPricePoint))
                        {
                            OnPriceChanged(match);
                        }
                    }
                }
                else if (columnKey == PartProcessDataManager.COLUMN_LOAD_CAPACITY_QUANTITY)
                {
                    removedListeners = true;
                    UnregisterProcessChangeHandlers();

                    var sourceRow = (e.Cell.Row.ListObject as DataRowView)?.Row;
                    _dataManager.SyncQuantity(sourceRow);
                    _dataManager.RefreshWeight(sourceRow);
                }
                else if (columnKey == PartProcessDataManager.COLUMN_LOAD_CAPACITY_WEIGHT)
                {
                    removedListeners = true;
                    UnregisterProcessChangeHandlers();

                    var sourceRow = (e.Cell.Row.ListObject as DataRowView)?.Row;
                    _dataManager.SyncWeight(sourceRow);
                    _dataManager.RefreshQuantity(sourceRow);
                }
                else if (columnKey == PartProcessDataManager.COLUMN_LEAD_TIME_HOURS || columnKey == PartProcessDataManager.COLUMN_LEAD_TIME_TYPE)
                {
                    removedListeners = true;
                    UnregisterProcessChangeHandlers();

                    var sourceRow = (e.Cell.Row.ListObject as DataRowView)?.Row;
                    _dataManager.SyncLeadTimeData(sourceRow);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating per-process price.");
            }
            finally
            {
                if (removedListeners)
                {
                    RegisterProcessChangeHandlers();
                }
            }
        }

        private void OnPriceChanged(PricePointData changedPricePoint)
        {
            if (changedPricePoint == null)
            {
                return;
            }

            var total = _dataManager.GetTotal(changedPricePoint);

            var handler = PriceChanged;
            var args = new PriceChangedEventArgs(
                OrderPrice.GetPricingStrategy(changedPricePoint.PriceUnit),
                total);

            handler?.Invoke(this, args);

        }

        private void OnAllPricesChanged()
        {
            // Trigger change event using the first price point for each price unit.
            var firstEachPricePoint = _dataManager
                .PricePoints
                .Where(point => point.PriceUnit == OrderPrice.enumPriceUnit.Each)
                .OrderBy(point => (point.MinQuantity ?? 0))
                .FirstOrDefault();

            var firstLotPricePoint = _dataManager
                .PricePoints
                .Where(point => point.PriceUnit == OrderPrice.enumPriceUnit.Lot)
                .OrderBy(point => (point.MinQuantity ?? 0))
                .FirstOrDefault();

            OnPriceChanged(firstEachPricePoint);
            OnPriceChanged(firstLotPricePoint);
        }

        #endregion

        #region DeleteProcessCommand

        private sealed class DeleteProcessCommand : CommandBase
        {
            #region Fields

            private bool _enabled;

            #endregion

            #region Properties

            public override bool Enabled => _enabled;

            public ProcessWidget Instance
            {
                get;
            }

            #endregion

            #region Methods

            public DeleteProcessCommand(ProcessWidget instance)
                : base(instance.btnDeleteProcess)
            {
                Instance = instance;
                Instance.grdProcess.AfterSelectChange += GrdProcess_AfterSelectChange;
                Refresh();
            }

            public override void OnClick()
            {
                var selectedRows = Instance.grdProcess.Selected.Rows;

                if (selectedRows == null || selectedRows.Count != 1 || !selectedRows[0].IsDataRow)
                {
                    return;
                }

                DataRow selectedProcessData = (selectedRows[0].ListObject as DataRowView)?.Row;

                PartsDataset.PartProcessRow selectedProcess = null;

                if (selectedProcessData != null)
                {
                    selectedProcess = Instance._dataManager.GetPartProcess(selectedProcessData);
                    selectedProcessData.Delete();
                }

                if (selectedProcess != null && selectedProcess.IsValidState())
                {
                    selectedProcess.Delete();
                    Instance.grdProcess.Selected.Rows.Clear();
                    _enabled = false;
                    Refresh();
                }

                Instance._dataManager.FixStepOrder();
                Instance.OnAllPricesChanged();
            }

            #endregion

            #region Events

            private void GrdProcess_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
            {
                _enabled = e.Type == typeof(UltraGridRow) && Instance.grdProcess.Selected.Rows.Count == 1;
                Refresh();
            }

            #endregion
        }

        #endregion

        #region EditProcessCommand

        private sealed class EditProcessCommand : CommandBase
        {
            #region Fields

            private bool _enabled;

            #endregion

            #region Properties

            public override bool Enabled => _enabled;

            public ProcessWidget Instance
            {
                get;
            }

            #endregion

            #region Methods

            public EditProcessCommand(ProcessWidget instance)
                : base(instance.btnEditProcess)
            {
                Instance = instance;
                Instance.grdProcess.AfterSelectChange += GrdProcess_AfterSelectChange;
                Refresh();
            }

            public override void OnClick()
            {
                var selectedRows = Instance.grdProcess.Selected.Rows;

                if (selectedRows == null || selectedRows.Count != 1 || !selectedRows[0].IsDataRow)
                {
                    return;
                }

                DataRow selectedProcessData = (selectedRows[0].ListObject as DataRowView)?.Row;

                PartsDataset.PartProcessRow selectedProcess = null;

                if (selectedProcessData != null)
                {
                    selectedProcess = Instance._dataManager.GetPartProcess(selectedProcessData);
                }

                if (selectedProcess != null && selectedProcess.IsValidState())
                {
                    using (var ppa = new PartProcessAnswers())
                    {
                        ppa.CurrentPartProcess = selectedProcess;
                        ppa.PartsDataset = Instance.Dataset;
                        ppa.ShowDialog(Form.ActiveForm);

                        selectedProcessData[PartProcessDataManager.COLUMN_UNANSWERED_QUESTIONS] = ppa.UnAnsweredQuestions;
                    }
                }
            }

            #endregion

            #region Events

            private void GrdProcess_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
            {
                _enabled = e.Type == typeof(UltraGridRow) && Instance.grdProcess.Selected.Rows.Count == 1;
                Refresh();
            }

            #endregion
        }


        #endregion

        #region UnansweredQuestionsValidator

        private sealed class UnansweredQuestionsValidator : ControlValidatorBase
        {
            public UnansweredQuestionsValidator(UltraGrid processPriceGrid)
                : base(processPriceGrid)
            {

            }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                if (!(Control is UltraGrid processPriceGrid) || !processPriceGrid.Enabled)
                {
                    e.Cancel = false;
                    FireAfterValidation(true, String.Empty);
                    return;
                }

                bool failedValidation = false;
                List<string> failedProcessNames = new List<string>();
                foreach (var row in processPriceGrid.Rows.Where(i => i.IsDataRow))
                {
                    DataRow rowProcess = (row.ListObject as DataRowView)?.Row;
                    if (rowProcess == null)
                    {
                        continue;
                    }

                    int unansweredQuestions = (rowProcess[PartProcessDataManager.COLUMN_UNANSWERED_QUESTIONS] as int?) ?? 0;

                    if (unansweredQuestions != 0)
                    {
                        failedValidation = true;
                        failedProcessNames.Add(rowProcess[PartProcessDataManager.COLUMN_ALIAS]?.ToString() ?? string.Empty);
                    }
                }

                e.Cancel = failedValidation;

                if (failedValidation)
                {
                    FireAfterValidation(false, String.Format(ErrorMessage, String.Concat(failedProcessNames)));
                }
                else
                {
                    FireAfterValidation(true, String.Empty);
                }
            }
        }

        #endregion

        #region GridInfo

        public sealed class ProcessGridSettings : UltraGridBandSettings
        {
            public bool HidePrices { get; set; }
        }

        #endregion

        #region PricePointConversion

        private class PricePointConversion
        {
            public IEnumerable<Sales.PricePointEditContext.QuantityPricePointItem> QuantityItems { get; set; }

            public IEnumerable<Sales.PricePointEditContext.WeightPricePointItem> WeightItems { get; set; }

            /// <summary>
            /// Deconstructs this instance - for use with tuple deconstruction syntax.
            /// </summary>
            /// <param name="quantityItems"></param>
            /// <param name="weightItems"></param>
            public void Deconstruct(out IEnumerable<Sales.PricePointEditContext.QuantityPricePointItem> quantityItems,
                out IEnumerable<Sales.PricePointEditContext.WeightPricePointItem> weightItems)
            {
                quantityItems = QuantityItems;
                weightItems = WeightItems;
            }
        }

        #endregion
    }
}
