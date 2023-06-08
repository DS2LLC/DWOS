using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Order;
using DWOS.Shared.Utilities;
using DWOS.UI.Admin;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinGrid;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Interop;

namespace DWOS.UI.Sales
{
    public partial class QuoteProcessPriceWidget : UserControl, IQuoteProcessWidget
    {
        #region Fields

        private const string COLUMN_ALIAS = "Alias";
        private const string COLUMN_PROCESS = "Process";
        private const string COLUMN_QUOTE_PART_PROCESS_ID = "QuotePartProcessID";
        private const string COLUMN_STEP_ORDER = "StepOrder";
        private const string COLUMN_ACTIVE = "Active";
        private const string COLUMN_SHOW = "Show";
        private const string GRID_COLUMN_INACTIVE = "Inactive";

        public event EventHandler<PricePointChangedEventArgs> PricePointChanged;
        public event EventHandler PriceBucketsChanged;

        private bool _editable;
        private DataTable _dtProcessData;
        private IEnumerable<PricePointData> _pricePoints;
        private IPriceUnitPersistence _priceUnitPersistence;
        private ProcessPicker _processPicker;

        /// <summary>
        /// If true, syncs price point data when calling <see cref="SaveRow"/>.
        /// </summary>
        /// <remarks>
        /// Added in order to fix a bug where this control could save
        /// incomplete price point data if the first process has a blank
        /// price point.
        /// 
        /// Due to how DataTables work, deleting part process will delete
        /// pricing info regardless of this field's value.
        /// </remarks>
        private bool _syncPricePointData;

        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("Quote_Process", new UltraGridBandSettings());

        #endregion

        #region Properties

        public IEnumerable<PricePoint> PricePoints => _pricePoints
            .Select(point => PricePoint.FromQuantities(point.MinQuantity, point.MaxQuantity, point.PriceUnit));

        #endregion

        #region Methods

        public QuoteProcessPriceWidget()
        {
            InitializeComponent();
        }

        public void UpdatePrices(PricingStrategy pricingStrategy, decimal newTotal)
        {
            try
            {
                grdProcesses.BeforeCellUpdate -= grdProcesses_BeforeCellUpdate;
                grdProcesses.AfterCellUpdate -= grdProcesses_AfterCellUpdate;
                _syncPricePointData = true;

                if (_dtProcessData.Rows.Count == 0)
                {
                    return;
                }

                var matches = _pricePoints.Where(point => OrderPrice.GetPricingStrategy(point.PriceUnit) == pricingStrategy);

                // Check prices against minimums
                bool hasValueBelowMinimum = false;
                foreach (var row in _dtProcessData.Rows.OfType<DataRow>())
                {
                    var quotePartProcessID = (row[COLUMN_QUOTE_PART_PROCESS_ID] as int?) ?? 0;
                    var quotePartProcess = Dataset.QuotePart_Process.FindByQuotePartProcessID(quotePartProcessID);
                    var process = quotePartProcess.ProcessRow;

                    var minimumPrice = process.IsMinPriceNull() ? 0M : process.MinPrice;
                    var newPrice = DefaultValueForSync(newTotal, quotePartProcess);

                    if (newPrice < minimumPrice)
                    {
                        hasValueBelowMinimum = true;
                        break;
                    }
                }

                bool changePrices = true;
                if (hasValueBelowMinimum)
                {
                    const string msg = "The price that you entered will result in one or more processes being below the minimum price.\n" +
                        "Do you want to override all minimum prices?";

                    changePrices = MessageBoxUtilities.ShowMessageBoxYesOrNo(msg, "Parts Manager") == DialogResult.Yes;
                }

                if (changePrices)
                {
                    foreach (var matchingPoint in matches)
                    {
                        string key = TempTableColumnName(matchingPoint);
                        foreach (var row in _dtProcessData.Rows.OfType<DataRow>())
                        {
                            if (row == null)
                            {
                                continue;
                            }
                            var quotePartProcessID = (row[COLUMN_QUOTE_PART_PROCESS_ID] as int?) ?? 0;
                            var quotePartProcess = Dataset.QuotePart_Process.FindByQuotePartProcessID(quotePartProcessID);

                            row[key] = DefaultValueForSync(newTotal, quotePartProcess);
                        }
                    }
                }
            }
            finally
            {
                grdProcesses.BeforeCellUpdate += grdProcesses_BeforeCellUpdate;
                grdProcesses.AfterCellUpdate += grdProcesses_AfterCellUpdate;
            }
        }

        public QuotePartPrice GetPrice(int customerId, PriceByType priceByType, int currentQty, decimal currentTotalWeight)
        {
            PricePointData match;

            if (currentQty < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(currentQty));
            }

            if (currentTotalWeight < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(currentTotalWeight));
            }

            if (currentQty == 0)
            {
                match = _pricePoints.FirstOrDefault();
            }
            else if (_pricePoints.Any(p => p.MinQuantity.HasValue || p.MinWeight.HasValue))
            {
                // Use VDP pricing
                switch (priceByType)
                {
                    case PriceByType.Quantity:
                        match = _pricePoints
                            .Where(point => OrderPrice.GetPriceByType(point.PriceUnit) == PriceByType.Quantity)
                            .FirstOrDefault(p => currentQty >= (p.MinQuantity ?? 0) && currentQty <= (p.MaxQuantity ?? int.MaxValue));
                        break;
                    case PriceByType.Weight:
                        match = _pricePoints
                            .Where(point => OrderPrice.GetPriceByType(point.PriceUnit) == PriceByType.Weight)
                            .FirstOrDefault(p => currentTotalWeight >= (p.MinWeight ?? 0) && currentTotalWeight <= (p.MaxWeight ?? decimal.MaxValue));
                        break;
                    default:
                        match = null;
                        break;
                }
            }
            else
            {
                var pricePoint = _priceUnitPersistence.DeterminePriceUnit(customerId, currentQty, currentTotalWeight, priceByType);
                match = _pricePoints.FirstOrDefault(p => p.PriceUnit == pricePoint);
            }

            if (match == null)
            {
                return null;
            }
            else
            {
                return new QuotePartPrice(match.PriceUnit, TotalFor(match));
            }
        }

        /// <summary>
        /// Returns a default value to use during sync with quote part's each/lot price.
        /// </summary>
        /// <param name="newTotal"></param>
        /// <param name="partProcess"></param>
        /// <returns></returns>
        private decimal DefaultValueForSync(decimal newTotal, QuoteDataSet.QuotePart_ProcessRow quotePartProcess)
        {
            int rounding = ApplicationSettings.Current.PriceDecimalPlaces;
            if (quotePartProcess == null || quotePartProcess.ProcessRow == null)
            {
                return 0M;
            }
            var processRow = quotePartProcess.ProcessRow;
            var processTarget = processRow.IsPriceNull() ? 0M : processRow.Price;
            var sumProcessTarget = CurrentRecord
                .GetQuotePart_ProcessRows()
                .Sum(i => (i.ProcessRow?.IsPriceNull() ?? true) ? 0M : i.ProcessRow.Price);

            decimal newPrice;
            if (sumProcessTarget == 0M)
            {
                newPrice = newTotal / Convert.ToDecimal(CurrentRecord.GetQuotePart_ProcessRows().Length);
            }
            else
            {
                // Round calculation because it won't be entirely accurate
                // Example:
                // Process 1 has Target Price of $2.
                // Process 2 has a Target Price of $1
                // The new total is $6, so Process 1 can be $1.99~ before rounding.
                newPrice = Math.Round(newTotal * (processTarget / sumProcessTarget),
                    rounding,
                    MidpointRounding.AwayFromZero);
            }
            return newPrice;
        }

        private void AddNewProcess()
        {
            if (CurrentRecord == null)
            {
                return;
            }

            if (_processPicker == null)
            {
                _processPicker = new ProcessPicker(true);
            }

            _processPicker.LoadCustomerAliases(CurrentRecord.QuoteRow.CustomerID);

            if (_processPicker.ShowDialog() == DialogResult.OK)
            {
                _syncPricePointData = true;

                foreach (var process in _processPicker.SelectedProcesses)
                {
                    var nextStepOrder = this.GetNextProcessStepOrder(CurrentRecord);

                    var partProcessRow = this.Dataset.QuotePart_Process.NewQuotePart_ProcessRow();
                    partProcessRow.QuotePartRow = CurrentRecord;
                    partProcessRow.ProcessID = process.ProcessID; // Process may not be loaded
                    partProcessRow.StepOrder = nextStepOrder;
                    partProcessRow.ProcessAliasID = process.ProcessAliasID;
                    partProcessRow.ShowOnQuote = true;

                    this.Dataset.QuotePart_Process.AddQuotePart_ProcessRow(partProcessRow);

                    var defaultPrice = partProcessRow.ProcessRow.IsPriceNull() ?
                        0M :
                        partProcessRow.ProcessRow.Price;

                    _dtProcessData.Rows.Add(NewTempDataRow(partProcessRow, defaultPrice));
                }
            }
        }

        private int GetNextProcessStepOrder(QuoteDataSet.QuotePartRow quotePart)
        {
            var quotePartProcessRows = quotePart.GetQuotePart_ProcessRows();

            if (quotePartProcessRows.Count() > 0)
            {
                return quotePartProcessRows.Max(qpqpr => qpqpr.StepOrder) + 1;
            }
            else
            {
                return 1;
            }
        }

        private void DeleteCurrentSelection()
        {
            var selectedRows = grdProcesses.Selected.Rows;

            if (selectedRows.Count == 0)
            {
                return;
            }

            using (new UsingGridLoad(grdProcesses))
            {
                foreach (var row in selectedRows)
                {
                    if (!row.IsDataRow)
                    {
                        continue;
                    }

                    var selectedProcessPrice = (row.ListObject as DataRowView).Row;
                    QuoteDataSet.QuotePart_ProcessRow selectedQuotePartProcess = null;

                    if (selectedProcessPrice != null)
                    {
                        int quotePartProcessID = (selectedProcessPrice[COLUMN_QUOTE_PART_PROCESS_ID] as int?) ?? 0;
                        selectedQuotePartProcess = Dataset.QuotePart_Process.FindByQuotePartProcessID(quotePartProcessID);
                        selectedProcessPrice.Delete();
                    }

                    if (selectedQuotePartProcess != null)
                    {
                        selectedQuotePartProcess.Delete();
                    }
                }
            }

            this.UpdateButtonEnabledStates();
            this.UpdateProcessStepOrders();
        }

        /// <summary>
        /// Updates StepOrder for processes based on the current grid order.
        /// </summary>
        private void UpdateProcessStepOrders()
        {
            int currentStepOrder = 0;

            foreach (var row in grdProcesses.Rows.Where(r => r.IsDataRow))
            {
                var priceData = (row.ListObject as DataRowView).Row;

                if (priceData == null)
                {
                    continue;
                }

                // Fix
                currentStepOrder += 1;
                int stepOrder = (priceData[COLUMN_STEP_ORDER] as int?) ?? 0;

                if (stepOrder != currentStepOrder)
                {
                    priceData[COLUMN_STEP_ORDER] = currentStepOrder;
                }

                // Sync
                int quotePartProcessID = (priceData[COLUMN_QUOTE_PART_PROCESS_ID] as int?) ?? 0;
                var quotePartProcess = Dataset.QuotePart_Process.FindByQuotePartProcessID(quotePartProcessID);

                if (quotePartProcess != null && quotePartProcess.StepOrder != currentStepOrder)
                {
                    quotePartProcess.StepOrder = currentStepOrder;
                }
            }
        }

        private void OnPriceChanged(PricePointData changedPricePoint)
        {
            if (changedPricePoint == null)
            {
                return;
            }

            decimal total = TotalFor(changedPricePoint);

            var pricePoint = PricePoint.FromQuantities(
                changedPricePoint.MinQuantity,
                changedPricePoint.MaxQuantity,
                changedPricePoint.PriceUnit);

            var args = new PricePointChangedEventArgs(pricePoint, total);
            PricePointChanged?.Invoke(this, args);
        }

        private decimal TotalFor(PricePointData pricePoint)
        {
            if (pricePoint == null)
            {
                return 0M;
            }

            string columnKey = TempTableColumnName(pricePoint);

            decimal total = 0M;
            foreach (var row in _dtProcessData.Rows.OfType<DataRow>())
            {
                total += (row[columnKey] as decimal?) ?? 0M;
            }

            return total;
        }

        private void OnAllPricesChanged()
        {
            foreach (var pricePoint in _pricePoints)
            {
                OnPriceChanged(pricePoint);
            }
        }

        private IEnumerable<PricePointData> GeneratePricePoints(QuoteDataSet.QuotePartRow currentRecord)
        {
            OrdersDataSet.PricePointDataTable dtPricePoint = null;
            OrdersDataSet.PricePointDetailDataTable dtPricePointDetail = null;

            try
            {
                bool hasExistingProcesses = currentRecord.GetQuotePart_ProcessRows().Length > 0;
                bool hasExistingProcessPricing = false;

                if (hasExistingProcesses)
                {
                    hasExistingProcessPricing = currentRecord
                        .GetQuotePart_ProcessRows()
                        .Any(row => row.GetQuotePartProcessPriceRows().Length > 0);
                }

                var pricePoints = new List<PricePointData>();

                if (hasExistingProcessPricing)
                {
                    var pointsDictionary = new Dictionary<int, PricePointData>();

                    foreach (var process in currentRecord.GetQuotePart_ProcessRows())
                    {
                        foreach (var price in process.GetQuotePartProcessPriceRows())
                        {
                            var pricePointData = PricePointData.From(price);
                            pointsDictionary[pricePointData.GetHashCode()] = pricePointData;
                        }
                    }

                    pricePoints.AddRange(pointsDictionary.Values);

                    if (pricePoints.All(point => OrderPrice.GetPriceByType(point.PriceUnit) == PriceByType.Quantity))
                    {
                        // Use weight-based price points from either the customer defaults or the system defaults
                        var customerWeightDefaults = GenerateFromCustomerDefaults(currentRecord.QuoteRow.CustomerID)
                            ?.Where(point => OrderPrice.GetPriceByType(point.PriceUnit) == PriceByType.Weight)
                            .ToList();

                        if (customerWeightDefaults != null && customerWeightDefaults.Count > 0)
                        {
                            pricePoints.AddRange(customerWeightDefaults);
                        }
                        else
                        {
                            pricePoints.AddRange(GenerateFromSystemDefaults().Where(point => OrderPrice.GetPriceByType(point.PriceUnit) == PriceByType.Weight));
                        }
                    }
                }
                else if (ApplicationSettings.Current.EnableVolumePricing)
                {
                    var customerDefaults = GenerateFromCustomerDefaults(currentRecord.QuoteRow.CustomerID);

                    if (customerDefaults != null && customerDefaults.Count > 0)
                    {
                        pricePoints.AddRange(customerDefaults);
                    }
                    else
                    {
                        pricePoints.AddRange(GenerateFromSystemDefaults());
                    }
                }
                else
                {
                    // Non-volume pricing
                    var priceByType = (PriceByType)Enum.Parse(typeof(PriceByType), currentRecord.PriceBy);

                    pricePoints.AddRange(_priceUnitPersistence.ActivePriceUnits.Select(PricePointData.Blank));
                }

                return pricePoints;
            }
            finally
            {
                dtPricePoint?.Dispose();
                dtPricePointDetail?.Dispose();
            }
        }

        private List<PricePointData> GenerateFromCustomerDefaults(int customerId)
        {
            var pricePoints = new List<PricePointData>();
            OrdersDataSet.CustomerPricePointDataTable dtCustomerPricePoint = null;
            OrdersDataSet.CustomerPricePointDetailDataTable dtCustomerPricePointDetail = null;

            try
            {
                dtCustomerPricePoint = new OrdersDataSet.CustomerPricePointDataTable();
                using (var taPrice = new Data.Datasets.OrdersDataSetTableAdapters.CustomerPricePointTableAdapter())
                {
                    taPrice.FillByVolume(dtCustomerPricePoint, customerId);
                }

                var pricePointRow = dtCustomerPricePoint.FirstOrDefault();

                if (pricePointRow == null)
                {
                    return null;
                }

                var pricePointId = pricePointRow.CustomerPricePointID;

                dtCustomerPricePointDetail = new OrdersDataSet.CustomerPricePointDetailDataTable();
                using (var taDetail = new Data.Datasets.OrdersDataSetTableAdapters.CustomerPricePointDetailTableAdapter())
                {
                    taDetail.FillByPricePoint(dtCustomerPricePointDetail, pricePointId);
                }

                var activePriceUnits = _priceUnitPersistence
                    .ActivePriceUnits
                    .ToList();

                foreach (var detailRow in dtCustomerPricePointDetail)
                {
                    var pricePoint = PricePointData.From(detailRow);

                    if (activePriceUnits.Contains(pricePoint.PriceUnit))
                    {
                        pricePoints.Add(pricePoint);
                    }
                }
            }
            finally
            {
                dtCustomerPricePoint?.Dispose();
                dtCustomerPricePointDetail?.Dispose();
            }

            if (pricePoints.All(point => OrderPrice.GetPriceByType(point.PriceUnit) == PriceByType.Quantity))
            {
                // Use system-default weight price points
                pricePoints.AddRange(GenerateFromSystemDefaults().Where(point => OrderPrice.GetPriceByType(point.PriceUnit) == PriceByType.Weight));
            }

            return pricePoints;
        }

        private IEnumerable<PricePointData> GenerateFromSystemDefaults()
        {
            // Create from price points - volume defaults
            var pricePoints = new List<PricePointData>();
            var dtPricePoint = new OrdersDataSet.PricePointDataTable();
            using (var taPrice = new Data.Datasets.OrdersDataSetTableAdapters.PricePointTableAdapter())
            {
                taPrice.FillVolumeDefault(dtPricePoint);
            }

            int pricePointID = dtPricePoint.FirstOrDefault()?.PricePointID ?? 0;

            var dtPricePointDetail = new OrdersDataSet.PricePointDetailDataTable();
            using (var taDetail = new Data.Datasets.OrdersDataSetTableAdapters.PricePointDetailTableAdapter())
            {
                taDetail.FillByPricePoint(dtPricePointDetail, pricePointID);
            }

            var activePriceUnits = _priceUnitPersistence
                .ActivePriceUnits
                .ToList();

            foreach (var detailRow in dtPricePointDetail)
            {
                var pricePoint = PricePointData.From(detailRow);

                if (activePriceUnits.Contains(pricePoint.PriceUnit))
                {
                    pricePoints.Add(pricePoint);
                }
            }

            return pricePoints;
        }

        private static DataTable NewTempTable(IEnumerable<PricePointData> pricePoints)
        {
            if (pricePoints == null)
            {
                throw new ArgumentNullException(nameof(pricePoints), "pricePoints cannot be null");
            }

            var _dtProcessData = new DataTable();
            _dtProcessData.Columns.Add(COLUMN_ALIAS, typeof(string));
            _dtProcessData.Columns.Add(COLUMN_PROCESS, typeof(string));
            _dtProcessData.Columns.Add(COLUMN_QUOTE_PART_PROCESS_ID, typeof(int));
            _dtProcessData.Columns.Add(COLUMN_STEP_ORDER, typeof(int));
            _dtProcessData.Columns.Add(COLUMN_ACTIVE, typeof(bool));
            _dtProcessData.Columns.Add(COLUMN_SHOW, typeof(bool));

            foreach (var pricePoint in pricePoints)
            {
                _dtProcessData.Columns.Add(TempTableColumnName(pricePoint), typeof(decimal));
            }

            return _dtProcessData;
        }

        private DataRow NewTempDataRow(QuoteDataSet.QuotePart_ProcessRow quotePartProcess, decimal defaultPrice)
        {
            DataRow newRow = _dtProcessData.NewRow();
            newRow[COLUMN_ALIAS] = quotePartProcess.ProcessAliasRow?.Name ?? quotePartProcess.QuotePartProcessID.ToString();

            if (quotePartProcess.ProcessRow == null || quotePartProcess.ProcessRow.IsProcessNameNull())
            {
                newRow[COLUMN_PROCESS] = quotePartProcess.ProcessID;
            }
            else
            {
                newRow[COLUMN_PROCESS] = quotePartProcess.ProcessRow.ProcessName;
            }

            newRow[COLUMN_QUOTE_PART_PROCESS_ID] = quotePartProcess.QuotePartProcessID.ToString();
            newRow[COLUMN_STEP_ORDER] = quotePartProcess.StepOrder;
            newRow[COLUMN_ACTIVE] = quotePartProcess.ProcessRow?.Active ?? false;
            newRow[COLUMN_SHOW] = quotePartProcess.IsShowOnQuoteNull() ? true : quotePartProcess.ShowOnQuote;

            foreach (var pricePoint in _pricePoints)
            {
                // Find value
                var matchingRow = quotePartProcess
                    .GetQuotePartProcessPriceRows()
                    .FirstOrDefault(row => pricePoint.Matches(row));

                newRow[TempTableColumnName(pricePoint)] = matchingRow?.Amount ?? defaultPrice;
            }

            return newRow;
        }

        private static string TempTableColumnName(PricePointData pricePoint)
        {
            if (pricePoint == null)
            {
                return null;
            }

            return string.Format("Price_{0}", pricePoint.GetHashCode());
        }

        private string GetCaption(PricePointData pricePoint)
        {
            string caption;

            var priceByType = OrderPrice.GetPriceByType(pricePoint.PriceUnit);
            string displayText = _priceUnitPersistence.GetDisplayText(pricePoint.PriceUnit);

            if (priceByType == PriceByType.Quantity)
            {
                // Assumption - if MinQuantity is not present, neither is MaxQuantity
                if (pricePoint.MinQuantity.HasValue && pricePoint.MaxQuantity.HasValue)
                {
                    caption = $"{displayText}({pricePoint.MinQuantity}-{pricePoint.MaxQuantity})";
                }
                else if (pricePoint.MinQuantity.HasValue)
                {
                    caption = $"{displayText} ({pricePoint.MinQuantity}+)";
                }
                else
                {
                    caption = displayText;
                }
            }
            else if (priceByType == PriceByType.Weight)
            {
                if (pricePoint.MinWeight.HasValue && pricePoint.MaxWeight.HasValue)
                {
                    caption = $"{displayText}({pricePoint.MinWeight}-{pricePoint.MaxWeight} lbs.)";
                }
                else if (pricePoint.MinWeight.HasValue)
                {
                    caption = $"{displayText} ({pricePoint.MinWeight}+ lbs.)";
                }
                else
                {
                    caption = displayText;
                }
            }
            else
            {
                caption = displayText;
            }

            return caption;
        }

        private void SyncDialogResult(IEnumerable<IPricePointDialogItem> dialogPricePoints)
        {
            if (dialogPricePoints == null || CurrentRecord == null)
            {
                return;
            }

            SaveRow(); // sync all changes that were made before opening dialog

            var existingPriceRows = new List<QuoteDataSet.QuotePartProcessPriceRow>();

            foreach (var process in CurrentRecord.GetQuotePart_ProcessRows())
            {
                existingPriceRows.AddRange(process.GetQuotePartProcessPriceRows().Where(i => i.IsValidState()));
            }

            var sortedDialogPoints = dialogPricePoints.OrderBy(point => point);

            foreach (var process in CurrentRecord.GetQuotePart_ProcessRows())
            {
                var existingAmountsForProcess = new LinkedList<decimal>(existingPriceRows
                    .Where(row => row.QuotePartProcessID == process.QuotePartProcessID)
                    .Select(row => row.Amount)
                    .OrderByDescending(d => d));

                decimal defaultAmount = existingAmountsForProcess.Count > 0 ?
                    existingAmountsForProcess.Last.Value :
                    0M;

                foreach (var dialogPoint in sortedDialogPoints)
                {
                    string priceUnit = dialogPoint.CalculateBy.ToString();

                    var newPriceRow = Dataset.QuotePartProcessPrice.NewQuotePartProcessPriceRow();

                    if (!string.IsNullOrEmpty(dialogPoint.MinValueString))
                    {
                        newPriceRow.MinValue = dialogPoint.MinValueString;
                    }

                    if (!string.IsNullOrEmpty(dialogPoint.MaxValueString))
                    {
                        newPriceRow.MaxValue = dialogPoint.MaxValueString;
                    }

                    newPriceRow.PriceUnit = priceUnit;
                    newPriceRow.QuotePart_ProcessRow = process;

                    if (existingAmountsForProcess.Count > 0)
                    {
                        newPriceRow.Amount = existingAmountsForProcess.First.Value;
                        existingAmountsForProcess.RemoveFirst();
                    }
                    else
                    {
                        newPriceRow.Amount = defaultAmount;
                    }

                    Dataset.QuotePartProcessPrice.AddQuotePartProcessPriceRow(newPriceRow);
                }
            }

            foreach (var existingPrice in existingPriceRows)
            {
                existingPrice.Delete();
            }

            LoadRow(CurrentRecord);
            PriceBucketsChanged?.Invoke(this, new EventArgs());
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

        #endregion

        #region Events

        private void btnAddProcess_Click(object sender, EventArgs e)
        {
            try
            {
                AddNewProcess();
                UpdateButtonEnabledStates();
                OnAllPricesChanged();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error on add process click.");
            }
        }

        private void btnDeleteProcess_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteCurrentSelection();
                OnAllPricesChanged();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error deleting nodes.");
            }
        }

        private void grdProcesses_AfterSelectChange(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
        {
            UpdateButtonEnabledStates();
        }

        private void grdProcesses_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            const string currencyFormat = "{0:C}";

            try
            {
                grdProcesses.AfterColPosChanged -= grdProcesses_AfterColPosChanged;
                grdProcesses.AfterSortChange -= grdProcesses_AfterSortChange;

                var band = grdProcesses.DisplayLayout.Bands[0];

                foreach (var pricePoint in _pricePoints)
                {
                    string columnName = TempTableColumnName(pricePoint);
                    string caption = GetCaption(pricePoint);

                    var totalSummary = band.Summaries.Add(SummaryType.Sum, band.Columns[columnName]);
                    totalSummary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
                    totalSummary.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    totalSummary.Appearance.ForeColor = Color.Black;
                    totalSummary.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
                    totalSummary.DisplayFormat = currencyFormat;

                    band.Columns[columnName].Format = string.Format("$#,##0.{0}",
                        string.Concat(Enumerable.Repeat('#', ApplicationSettings.Current.PriceDecimalPlaces)));

                    band.Columns[columnName].MaskInput = "{currency:6." + ApplicationSettings.Current.PriceDecimalPlaces + "}";

                    band.Columns[columnName].MinValue = -214748.3648M;
                    band.Columns[columnName].MaxValue = 214748.3647M;

                    band.Columns[columnName].Header.Caption = caption;
                }

                band.SortedColumns.Add(COLUMN_STEP_ORDER, false);

                band.Columns[COLUMN_STEP_ORDER].Header.Caption = string.Empty;
                band.Columns[COLUMN_STEP_ORDER].MaxWidth = 40;
                band.Columns[COLUMN_STEP_ORDER].MinWidth = 40;

                band.Columns[COLUMN_QUOTE_PART_PROCESS_ID].Hidden = true;
                band.Columns[COLUMN_ACTIVE].Hidden = true;

                band.Columns[COLUMN_ALIAS].CellActivation = Activation.NoEdit;
                band.Columns[COLUMN_PROCESS].CellActivation = Activation.NoEdit;

                band.Columns[COLUMN_ALIAS].TabStop = false;
                band.Columns[COLUMN_PROCESS].TabStop = false;

                band.Columns[COLUMN_SHOW].CellActivation = Activation.AllowEdit;

                var inactiveImgColumn = band.Columns.Add(GRID_COLUMN_INACTIVE, string.Empty);
                inactiveImgColumn.DataType = typeof(Image);
                inactiveImgColumn.MaxWidth = 28;
                inactiveImgColumn.MinWidth = 28;
                inactiveImgColumn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                inactiveImgColumn.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                inactiveImgColumn.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                inactiveImgColumn.TabStop = false;

                band.Columns[COLUMN_STEP_ORDER].Header.VisiblePosition = 0;
                inactiveImgColumn.Header.VisiblePosition = 1;
                band.Columns[COLUMN_ALIAS].Header.VisiblePosition = 2;
                band.Columns[COLUMN_PROCESS].Header.VisiblePosition = 3;
                band.Columns[COLUMN_SHOW].Header.VisiblePosition = 4;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(band);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing grid layout.");
            }
            finally
            {
                grdProcesses.AfterColPosChanged += grdProcesses_AfterColPosChanged;
                grdProcesses.AfterSortChange += grdProcesses_AfterSortChange;
            }
        }

        private void grdProcesses_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdProcesses.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column position in grid.");
            }
        }

        private void grdProcesses_AfterSortChange(object sender, BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdProcesses.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column sort in grid.");
            }
        }

        private void grdProcesses_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (!e.Row.IsDataRow)
            {
                return;
            }

            if (e.Row.Cells.Exists(GRID_COLUMN_INACTIVE))
            {
                DataRow currentProcess = (e.Row.ListObject as DataRowView)?.Row;

                bool isActive = (currentProcess[COLUMN_ACTIVE] as bool?) ?? false;

                try
                {
                    grdProcesses.BeforeCellUpdate -= grdProcesses_BeforeCellUpdate;
                    grdProcesses.AfterCellUpdate -= grdProcesses_AfterCellUpdate;

                    if (isActive)
                    {
                        e.Row.Cells[GRID_COLUMN_INACTIVE].Value = null;
                    }
                    else
                    {
                        e.Row.Cells[GRID_COLUMN_INACTIVE].Value = Properties.Resources.RoundDashRed_32;
                    }
                }
                finally
                {
                    grdProcesses.BeforeCellUpdate += grdProcesses_BeforeCellUpdate;
                    grdProcesses.AfterCellUpdate += grdProcesses_AfterCellUpdate;
                }
            }
        }

        private void grdProcesses_SelectionDrag(object sender, CancelEventArgs e)
        {
            grdProcesses.DoDragDrop(grdProcesses.Selected.Rows, DragDropEffects.Move);
        }

        private void grdProcesses_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move; // is this necessary?
            var senderGrid = sender as UltraGrid;

            if (senderGrid == null)
            {
                return;
            }

            var pointInGrid = senderGrid.PointToClient(new Point(e.X, e.Y));

            if (pointInGrid.Y < 20)
            {
                grdProcesses.ActiveRowScrollRegion.Scroll(RowScrollAction.LineUp);
            }
            else if (pointInGrid.Y > (senderGrid.Height - 20))
            {
                grdProcesses.ActiveRowScrollRegion.Scroll(RowScrollAction.LineDown);
            }
        }

        private void grdProcesses_DragDrop(object sender, DragEventArgs e)
        {
            var pointInGrid = grdProcesses.PointToClient(new Point(e.X, e.Y));
            var elementOver = grdProcesses.DisplayLayout.UIElement.ElementFromPoint(pointInGrid);

            if (elementOver == null)
            {
                return;
            }

            var rowOver = elementOver.GetContext(typeof(UltraGridRow), true) as UltraGridRow;

            SelectedRowsCollection selectedRows = null;
            if (rowOver != null)
            {
                selectedRows = e.Data.GetData(typeof(SelectedRowsCollection)) as SelectedRowsCollection;
            }

            if (selectedRows != null)
            {
                foreach (var selectedRow in selectedRows)
                {
                    grdProcesses.Rows.Move(selectedRow, rowOver.Index);
                }
            }

            UpdateProcessStepOrders();

            // Refresh sort
            var band = grdProcesses.DisplayLayout.Bands[0];
            band.SortedColumns.Clear();
            band.SortedColumns.Add(COLUMN_STEP_ORDER, false);
        }

        private void grdProcesses_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            try
            {
                // Check against minimum price
                if (e.Cell.Row.IsDataRow)
                {
                    var matchingTempRow = (e.Cell.Row.ListObject as DataRowView)?.Row;
                    var matchingQuotePartProcess = this.Dataset.QuotePart_Process
                        .FindByQuotePartProcessID(matchingTempRow[COLUMN_QUOTE_PART_PROCESS_ID] as int? ?? 0);
                    var process = matchingQuotePartProcess.ProcessRow;
                    var newValue = (e.NewValue as decimal?) ?? 0M;
                    var minimumValue = process.IsMinPriceNull() ? 0M : process.MinPrice;

                    if (newValue < minimumValue)
                    {
                        const string msgFormat = "The price you entered ({0}) is less than the process's minimum price.\n" +
                            "Do you want to override the minimum price of {1}?";

                        string msg = string.Format(msgFormat,
                            newValue.ToString(OrderPrice.CurrencyFormatString),
                            minimumValue.ToString(OrderPrice.CurrencyFormatString));

                        e.Cancel = MessageBoxUtilities.ShowMessageBoxYesOrNo(msg, "Quote Manager") != DialogResult.Yes;
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error before updating per-process price.");
            }
        }

        private void grdProcesses_AfterCellUpdate(object sender, CellEventArgs e)
        {
            try
            {
                _syncPricePointData = true;
                var columnKey = e.Cell.Column.Key;

                foreach (var pricePoint in _pricePoints)
                {
                    if (TempTableColumnName(pricePoint) == columnKey)
                    {
                        OnPriceChanged(pricePoint);
                        break;
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error updating per-process price.");
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                var usesVolumePricing = _pricePoints.All(point => point.MinQuantity.HasValue || point.MinWeight.HasValue);

                if (!usesVolumePricing && !ApplicationSettings.Current.EnableVolumePricing)
                {
                    const string msg = "Unable to show pricing options for this part.\n" +
                        "Please enable volume pricing discounts and try again.";

                    MessageBoxUtilities.ShowMessageBoxOK(msg, "Quote Manager");
                    return;
                }
                else if (CurrentRecord.GetQuotePart_ProcessRows().Length == 0)
                {
                    const string msg = "Please add a process before configuring pricing options.";

                    MessageBoxUtilities.ShowMessageBoxOK(msg, "Quote Manager");
                    return;
                }

                // Prepare dialog data
                IEnumerable<PricePointEditContext.QuantityPricePointItem> quantityItems;
                IEnumerable<PricePointEditContext.WeightPricePointItem> weightItems;
                if (usesVolumePricing)
                {
                    (quantityItems, weightItems) = ConvertPricePoints(_pricePoints);
                }
                else
                {
                    var customerPricePoints = GenerateFromCustomerDefaults(CurrentRecord.QuoteRow.CustomerID);

                    if (customerPricePoints == null || customerPricePoints.Count == 0)
                    {
                        (quantityItems, weightItems) = ConvertPricePoints(GenerateFromSystemDefaults());
                    }
                    else
                    {
                        (quantityItems, weightItems) = ConvertPricePoints(customerPricePoints);
                    }
                }

                var windowContext = PricePointEditContext.From(CurrentRecord.Name,
                    quantityItems,
                    weightItems);

                var window = new PricePointDialog(windowContext);
                var helper = new WindowInteropHelper(window) { Owner = DWOSApp.MainForm.Handle };
                if (window.ShowDialog().GetValueOrDefault(false))
                {
                    SyncDialogResult(window.PricePoints);
                }

                GC.KeepAlive(helper);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error showing quote process price settings.");
            }
        }

        #endregion

        #region IQuoteProcessWidget Members

        public QuoteDataSet.QuotePartRow CurrentRecord
        {
            get;
            private set;
        }

        public QuoteDataSet Dataset
        {
            get;
            private set;
        }

        public bool Editable
        {
            get
            {
                return _editable;
            }
            set
            {
                _editable = value;
                UpdateButtonEnabledStates();
            }
        }

        public void LoadData(QuoteDataSet dataset, bool panelIsEditable)
        {
            Dataset = dataset;
            _editable = panelIsEditable;
            _priceUnitPersistence = new PriceUnitPersistence();
            UpdateButtonEnabledStates();
        }

        public void LoadRow(QuoteDataSet.QuotePartRow quotePart)
        {
            CurrentRecord = quotePart;
            grdProcesses.Selected.Rows.Clear();
            grdProcesses.DataSource = null;
            _pricePoints = null;
            _dtProcessData = null;
            _syncPricePointData = false;

            if (CurrentRecord != null)
            {
                _pricePoints = GeneratePricePoints(CurrentRecord).OrderBy(p => p);
                _dtProcessData = NewTempTable(_pricePoints);

                foreach (var quotePartProcess in CurrentRecord.GetQuotePart_ProcessRows())
                {
                    _dtProcessData.Rows.Add(NewTempDataRow(quotePartProcess, 0M));
                }
            }

            grdProcesses.DataSource = _dtProcessData;
        }

        public void SaveRow()
        {
            if (CurrentRecord == null || !_syncPricePointData)
            {
                return;
            }

            // Save price data
            // (This widget automatically changes PartProcess values.)
            foreach (var dataRow in _dtProcessData.Rows.OfType<DataRow>())
            {
                if (dataRow == null)
                {
                    continue;
                }

                int quotePartProcessID = (dataRow[COLUMN_QUOTE_PART_PROCESS_ID] as int?) ?? 0;
                var quotePartProcess = Dataset.QuotePart_Process.FindByQuotePartProcessID(quotePartProcessID);

                //Handle price changes
                if (quotePartProcess != null && quotePartProcess.IsValidState())
                {
                    var existingPriceData = Dataset.QuotePartProcessPrice.Where(i => i.IsValidState() && i.QuotePartProcessID == quotePartProcessID);

                    foreach (var pricePoint in _pricePoints)
                    {
                        var existingPriceRow = existingPriceData.FirstOrDefault(row => pricePoint.Matches(row));
                        var newAmount = (dataRow[TempTableColumnName(pricePoint)] as decimal?) ?? 0M;

                        if (existingPriceRow == null)
                        {
                            // Add
                            var newRow = Dataset.QuotePartProcessPrice.NewQuotePartProcessPriceRow();
                            newRow.Amount = newAmount;
                            newRow.QuotePart_ProcessRow = quotePartProcess;
                            newRow.PriceUnit = pricePoint.PriceUnit.ToString();

                            if (pricePoint.MinQuantity.HasValue)
                            {
                                newRow.MinValue = pricePoint.MinQuantity.Value.ToString();
                            }
                            else if (pricePoint.MinWeight.HasValue)
                            {
                                newRow.MinValue = pricePoint.MinWeight.Value.ToString();
                            }
                            else
                            {
                                newRow.SetMinValueNull();
                            }

                            if (pricePoint.MaxQuantity.HasValue)
                            {
                                newRow.MaxValue = pricePoint.MaxQuantity.Value.ToString();
                            }
                            else if (pricePoint.MaxWeight.HasValue)
                            {
                                newRow.MaxValue = pricePoint.MaxWeight.Value.ToString();
                            }
                            else
                            {
                                newRow.SetMaxValueNull();
                            }

                            Dataset.QuotePartProcessPrice.AddQuotePartProcessPriceRow(newRow);
                        }
                        else if (existingPriceRow != null)
                        {
                            existingPriceRow.Amount = newAmount;
                        }
                    }
                }

                //Handle show on quote state changes
                if (quotePartProcess != null)
                {
                    bool showOnQuote = (bool)dataRow[COLUMN_SHOW];
                    quotePartProcess.ShowOnQuote = showOnQuote;
                }
            }
        }

        public void UpdateButtonEnabledStates()
        {
            bool processSelected = grdProcesses.Selected.Rows.Count > 0;

            btnAddProcess.Enabled = _editable;
            btnDeleteProcess.Enabled = _editable && processSelected;
        }

        #endregion

        #region QuotePartPrice

        public sealed class QuotePartPrice
        {
            #region Properties

            public OrderPrice.enumPriceUnit PriceUnit
            {
                get;
                private set;
            }

            public decimal Amount
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public QuotePartPrice(OrderPrice.enumPriceUnit priceUnit, decimal amount)
            {
                PriceUnit = priceUnit;
                Amount = amount;
            }

            #endregion
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
