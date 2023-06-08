using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ProcessPackageDatasetTableAdapters;
using DWOS.Data.Order;
using DWOS.UI.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DWOS.UI.Admin.PartManagerPanels
{
    /// <summary>
    /// Manages part process data.
    /// </summary>
    internal sealed class PartProcessDataManager
    {
        #region Fields

        public const string COLUMN_DEPT = "Department"; 
        public const string COLUMN_ALIAS = "Alias";
        public const string COLUMN_PROCESS = "Process";
        public const string COLUMN_PART_PROCESS_ID = "PartProcessID";
        public const string COLUMN_STEP_ORDER = "StepOrder";
        public const string COLUMN_APPROVED = "Approved";
        public const string COLUMN_HAS_CLOSED_INSPECTIONS = "HasClosedInspections";
        public const string COLUMN_LOAD_CAPACITY_QUANTITY = "LoadCapacity_Quantity";
        public const string COLUMN_LOAD_CAPACITY_WEIGHT = "LoadCapacity_Weight";
        public const string COLUMN_LEAD_TIME_HOURS = "LeadTime_Hours";
        public const string COLUMN_LEAD_TIME_TYPE = "LeadTime_Type";
        public const string COLUMN_UNANSWERED_QUESTIONS = "UnansweredQuestions";
        public const string COLUMN_SHOWED_WARNING = "AlreadyShowedWarning";
        public const string COLUMN_PREFIX_PRICE = "Price_";

        private const int MAX_QUANTITY = 999999999;
        private const decimal MAX_WEIGHT = 999999.99999999M;

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

        public DataTable DataTable
        {
            get;
            private set;
        }

        public IEnumerable<PricePointData> PricePoints
        {
            get;
            private set;
        }

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
        public bool SyncPricePointData
        {
            get;
            set;
        }

        public IEnumerable<ProcessNote> NotesToShow
        {
            get
            {
                var processNotes = new List<ProcessNote>();

                foreach (var processPriceData in DataTable.Rows.OfType<DataRow>().Where(p => p != null && p.IsValidState()))
                {
                    int partProcessID = (processPriceData[COLUMN_PART_PROCESS_ID] as int?) ?? 0;
                    bool displayedWarning = (processPriceData[COLUMN_SHOWED_WARNING] as bool?) ?? false;

                    try
                    {
                        if (partProcessID < 0 && !displayedWarning)
                        {
                            using (var taPA = new Data.Datasets.ProcessesDatasetTableAdapters.ProcessAliasTableAdapter())
                            {
                                processPriceData[COLUMN_SHOWED_WARNING] = true;
                                var partProcess = Dataset.PartProcess.FindByPartProcessID(partProcessID);

                                if (partProcess != null)
                                {
                                    string notes = taPA.GetNotes(partProcess.ProcessAliasID);

                                    if (!string.IsNullOrEmpty(notes))
                                    {
                                        processNotes.Add(new ProcessNote(partProcessID, notes));
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        string errorMsg = "Error retrieving process notes for part process {0}";
                        LogManager.GetCurrentClassLogger().Error(exc, errorMsg, partProcessID);
                    }
                }

                return processNotes;
            }
        }

        public IPriceUnitPersistence PriceUnitPersistence { get; }

        #endregion

        #region Methods

        public PartProcessDataManager(PartsDataset dataset, IPriceUnitPersistence priceUnitPersistence, PartsDataset.PartRow currentRecord)
        {
            Dataset = dataset ?? throw new ArgumentNullException(nameof(dataset));
            PriceUnitPersistence = priceUnitPersistence ?? throw new ArgumentNullException(nameof(priceUnitPersistence));
            CurrentRecord = currentRecord ?? throw new ArgumentNullException(nameof(currentRecord));

            DataTable = new DataTable();

            if (ApplicationSettings.Current.PartPricingType == PricingType.Process)
            {
                PricePoints = GeneratePricePoints(currentRecord).OrderBy(p => p);
            }
            else
            {
                PricePoints = Enumerable.Empty<PricePointData>();
            }

            DataTable.Columns.Add(COLUMN_DEPT, typeof(string));
            DataTable.Columns.Add(COLUMN_ALIAS, typeof(string));
            DataTable.Columns.Add(COLUMN_PROCESS, typeof(string));
            DataTable.Columns.Add(COLUMN_PART_PROCESS_ID, typeof(int));
            DataTable.Columns.Add(COLUMN_STEP_ORDER, typeof(int));
            DataTable.Columns.Add(COLUMN_APPROVED, typeof(bool));
            DataTable.Columns.Add(COLUMN_HAS_CLOSED_INSPECTIONS, typeof(bool));
            DataTable.Columns.Add(COLUMN_UNANSWERED_QUESTIONS, typeof(int));
            DataTable.Columns.Add(COLUMN_SHOWED_WARNING, typeof(bool));
            DataTable.Columns.Add(COLUMN_LOAD_CAPACITY_QUANTITY, typeof(int));
            DataTable.Columns.Add(COLUMN_LOAD_CAPACITY_WEIGHT, typeof(decimal));
            DataTable.Columns.Add(COLUMN_LEAD_TIME_HOURS, typeof(decimal));
            DataTable.Columns.Add(COLUMN_LEAD_TIME_TYPE, typeof(string));

            foreach (var pricePoint in PricePoints)
            {
                DataTable.Columns.Add(TempTableColumnName(pricePoint), typeof(decimal));
            }

            using (var taPPA = new Data.Datasets.PartsDatasetTableAdapters.PartProcessAnswerTableAdapter())
            {
                foreach (var partProcess in CurrentRecord.GetPartProcessRows())
                {
                    var newRow = NewTempDataRow(partProcess, 0M, null);
                    newRow[COLUMN_UNANSWERED_QUESTIONS] = (taPPA.GetUnAnsweredQuestionCount(partProcess.PartProcessID) as int?) ?? 0;
                    DataTable.Rows.Add(newRow);
                }
            }
        }

        public void AddProcesses(List<SelectedProcess> processes)
        {
            SyncPricePointData = true;

            foreach (var selectedProcess in processes)
            {
                //create new data source
                var newPartProcess = this.Dataset.PartProcess.NewPartProcessRow();
                newPartProcess.ProcessID = selectedProcess.ProcessID;
                newPartProcess.PartID = CurrentRecord.PartID;
                newPartProcess.StepOrder = GetNextProcessStepOrder(CurrentRecord);
                newPartProcess.ProcessAliasID = selectedProcess.ProcessAliasID;

                this.Dataset.PartProcess.Rows.Add(newPartProcess);
                newPartProcess.EndEdit();

                var defaultPrice = newPartProcess.ProcessRow.IsPriceNull() ?
                    0M :
                    newPartProcess.ProcessRow.Price;

                var newRow = NewTempDataRow(newPartProcess,
                    defaultPrice,
                    ProcessData.From(newPartProcess.ProcessRow));

                DataTable.Rows.Add(newRow);
                SyncQuantity(newRow);
                SyncWeight(newRow);
            }
        }

        public void UpdatePrices(PricingStrategy pricingStrategy, decimal newTotal)
        {
            SyncPricePointData = true;

            if (DataTable.Rows.Count == 0)
            {
                return;
            }

            var priceDataRows = DataTable.Rows
                .OfType<DataRow>()
                .ToList();

            var matchingPricePoints = PricePoints
                .Where(point => OrderPrice.GetPricingStrategy(point.PriceUnit) == pricingStrategy);

            // Check prices against minimums
            bool hasValueBelowMinimum = false;
            foreach (var row in priceDataRows)
            {
                var partProcessID = (row[COLUMN_PART_PROCESS_ID] as int?) ?? 0;
                var partProcess = Dataset.PartProcess.FindByPartProcessID(partProcessID);
                var process = partProcess.ProcessRow;

                var minimumPrice = process.IsMinPriceNull() ? 0M : process.MinPrice;

                var newPrice = DefaultValueForSync(newTotal, partProcess);
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
                // Set prices
                foreach (var matchingPoint in matchingPricePoints)
                {
                    var totalFromProcesses = 0M;

                    string key = TempTableColumnName(matchingPoint);
                    foreach (var row in priceDataRows)
                    {
                        if (row == null)
                        {
                            continue;
                        }

                        var partProcessID = (row[COLUMN_PART_PROCESS_ID] as int?) ?? 0;
                        var partProcess = Dataset.PartProcess.FindByPartProcessID(partProcessID);
                        var processPrice = DefaultValueForSync(newTotal, partProcess);
                        totalFromProcesses += processPrice;
                        row[key] = processPrice;
                    }

                    // Ensure that the sum from processes equals the new total.
                    if (totalFromProcesses != newTotal)
                    {
                        var difference = newTotal - totalFromProcesses;

                        var lastPriceRow = priceDataRows
                            .LastOrDefault(p => Convert.ToDecimal(p[key]) > 0);

                        if (lastPriceRow != null)
                        {
                            lastPriceRow[key] = Convert.ToDecimal(lastPriceRow[key])
                                + difference;
                        }
                    }
                }
            }
        }

        public void UpdateWeights(decimal newPartWeight)
        {
            if (newPartWeight < 0M)
            {
                return;
            }

            foreach (var row in DataTable.Rows.OfType<DataRow>())
            {
                var qty = row[COLUMN_LOAD_CAPACITY_QUANTITY] as int?;

                if (!qty.HasValue)
                {
                    continue;
                }

                var newWeight = Math.Min(newPartWeight * qty.Value, MAX_WEIGHT);
                row[COLUMN_LOAD_CAPACITY_WEIGHT] = newWeight;

                var partProcess = GetPartProcess(row);

                if (partProcess != null)
                {
                    partProcess.LoadCapacityWeight = newWeight;
                }
            }
        }

        public void ClearWeights()
        {
            foreach (var row in DataTable.Rows.OfType<DataRow>())
            {
                row[COLUMN_LOAD_CAPACITY_WEIGHT] = DBNull.Value;

                var partProcess = GetPartProcess(row);

                if (partProcess != null)
                {
                    partProcess.SetLoadCapacityWeightNull();
                }
            }
        }

        public void SyncWeight(DataRow sourceRow)
        {
            if (sourceRow == null)
            {
                return;
            }

            var matchingPartProcess = GetPartProcess(sourceRow);

            var weight = sourceRow[COLUMN_LOAD_CAPACITY_WEIGHT] as decimal?;

            if (weight.HasValue)
            {
                matchingPartProcess.LoadCapacityWeight = weight.Value;
            }
            else
            {
                matchingPartProcess.SetLoadCapacityWeightNull();
            }
        }

        public void RefreshWeight(DataRow sourceRow)
        {
            if (CurrentRecord.IsWeightNull() || sourceRow == null)
            {
                return;
            }
            else if (sourceRow.IsNull(COLUMN_LOAD_CAPACITY_QUANTITY))
            {
                sourceRow[COLUMN_LOAD_CAPACITY_WEIGHT] = DBNull.Value;
                SyncWeight(sourceRow);
                return;
            }

            var partWeight = CurrentRecord.Weight;
            var qty = (sourceRow[COLUMN_LOAD_CAPACITY_QUANTITY] as int?) ?? 0;

            sourceRow[COLUMN_LOAD_CAPACITY_WEIGHT] = qty * partWeight;
            SyncWeight(sourceRow);
        }

        public void SyncQuantity(DataRow sourceRow)
        {
            if (sourceRow == null)
            {
                return;
            }

            var matchingPartProcess = GetPartProcess(sourceRow);

            var quantity = sourceRow[COLUMN_LOAD_CAPACITY_QUANTITY] as int?;

            if (quantity.HasValue)
            {
                matchingPartProcess.LoadCapacityQuantity = quantity.Value;
            }
            else
            {
                matchingPartProcess.SetLoadCapacityQuantityNull();
            }
        }

        public void SyncLeadTimeData(DataRow sourceRow)
        {
            if (sourceRow == null)
            {
                return;
            }

            var matchingPartProcess = GetPartProcess(sourceRow);

            // Hours
            var hours = sourceRow[COLUMN_LEAD_TIME_HOURS] as decimal?;

            if (hours.HasValue)
            {
                matchingPartProcess.LeadTimeHours = hours.Value;
            }
            else
            {
                matchingPartProcess.SetLeadTimeHoursNull();
            }

            // Type
            var type = sourceRow[COLUMN_LEAD_TIME_TYPE] as string;

            if (hours.HasValue && string.IsNullOrEmpty(type))
            {
                type = "load";
                sourceRow[COLUMN_LEAD_TIME_TYPE] = type;
            }
            else if (!hours.HasValue)
            {
                type = string.Empty;
                sourceRow[COLUMN_LEAD_TIME_TYPE] = type;
            }

            if (!string.IsNullOrEmpty(type))
            {
                matchingPartProcess.LeadTimeType = type;
            }
            else
            {
                matchingPartProcess.SetLeadTimeTypeNull();
            }
        }

        public void RefreshQuantity(DataRow sourceRow)
        {
            if (CurrentRecord.IsWeightNull() || sourceRow == null)
            {
                return;
            }
            else if (sourceRow.IsNull(COLUMN_LOAD_CAPACITY_WEIGHT))
            {
                sourceRow[COLUMN_LOAD_CAPACITY_QUANTITY] = DBNull.Value;
                SyncQuantity(sourceRow);
                return;
            }

            var partWeight = CurrentRecord.Weight;
            var weight = (sourceRow[COLUMN_LOAD_CAPACITY_WEIGHT] as decimal?) ?? 0;

            if (partWeight == 0)
            {
                sourceRow[COLUMN_LOAD_CAPACITY_QUANTITY] = 0;
            }
            else
            {
                sourceRow[COLUMN_LOAD_CAPACITY_QUANTITY] = Convert.ToInt32(weight / partWeight);
            }

            SyncQuantity(sourceRow);
        }

        public void MoveProcessDown(DataRow selectedProcess)
        {
            if (selectedProcess == null)
            {
                return;
            }

            int currentStepOrder = (selectedProcess[COLUMN_STEP_ORDER] as int?) ?? 0;

            foreach (var process in DataTable.Rows.OfType<DataRow>())
            {
                int processStepOrder = (process[COLUMN_STEP_ORDER] as int?) ?? 0;

                if (processStepOrder == currentStepOrder + 1)
                {
                    process[COLUMN_STEP_ORDER] = currentStepOrder;
                    break;
                }
            }

            selectedProcess[COLUMN_STEP_ORDER] = currentStepOrder + 1;
            FixStepOrder();
        }

        public void MoveProcessUp(DataRow selectedProcess)
        {
            if (selectedProcess == null)
            {
                return;
            }

            int currentStepOrder = (selectedProcess[COLUMN_STEP_ORDER] as int?) ?? 0;

            foreach (var process in DataTable.Rows.OfType<DataRow>())
            {
                int processStepOrder = (process[COLUMN_STEP_ORDER] as int?) ?? 0;

                if (processStepOrder == currentStepOrder - 1)
                {
                    process[COLUMN_STEP_ORDER] = currentStepOrder;
                    break;
                }
            }

            selectedProcess[COLUMN_STEP_ORDER] = currentStepOrder - 1;
            FixStepOrder();
        }

        public void FixStepOrder()
        {
            // Fix
            int currentStepOrder = 0;
            foreach (var process in DataTable.Rows.OfType<DataRow>().OrderBy(i => (i[COLUMN_STEP_ORDER] as int?) ?? 0))
            {
                currentStepOrder += 1;
                int stepOrder = (process[COLUMN_STEP_ORDER] as int?) ?? 0;

                if (stepOrder != currentStepOrder)
                {
                    process[COLUMN_STEP_ORDER] = currentStepOrder;
                }
            }

            // Sync
            foreach (var row in DataTable.Rows.OfType<DataRow>())
            {
                if (row == null)
                {
                    continue;
                }

                int partProcessID = (row[COLUMN_PART_PROCESS_ID] as int?) ?? 0;
                var partProcess = Dataset.PartProcess.FindByPartProcessID(partProcessID);

                if (partProcess != null)
                {
                    var stepOrder = row[COLUMN_STEP_ORDER] as int?;

                    if (stepOrder.HasValue)
                    {
                        partProcess.StepOrder = stepOrder.Value;
                    }
                    else
                    {
                        LogManager.GetCurrentClassLogger()
                            .Warn("Could not sync step order for PartProcess #{0}- no value provided", partProcessID);
                    }
                }
            }
        }

        public static string TempTableColumnName(PricePointData pricePoint)
        {
            if (pricePoint == null)
            {
                return null;
            }

            switch (OrderPrice.GetPriceByType(pricePoint.PriceUnit))
            {
                case PriceByType.Quantity:
                    return $"{COLUMN_PREFIX_PRICE}{pricePoint.PriceUnit}_{pricePoint.MinQuantity}_{pricePoint.MaxQuantity}";
                case PriceByType.Weight:
                    return $"{COLUMN_PREFIX_PRICE}{pricePoint.PriceUnit}_{pricePoint.MinWeight}_{pricePoint.MaxWeight}";
                default:
                    return $"{pricePoint.GetHashCode()}";
            }
        }

        public void SaveAsProcessPackage(string processPackageName)
        {
            ProcessPackageDataset dsProcesses = null;
            ProcessPackageTableAdapter taPP = null;
            ProcessPackage_ProcessesTableAdapter taPPP = null;

            try
            {
                if (string.IsNullOrEmpty(processPackageName))
                {
                    return;
                }

                dsProcesses = new ProcessPackageDataset();
                taPP = new ProcessPackageTableAdapter();
                taPPP = new ProcessPackage_ProcessesTableAdapter();

                dsProcesses.EnforceConstraints = false;
                ProcessPackageDataset.ProcessPackageRow processPackage = dsProcesses.ProcessPackage.AddProcessPackageRow(processPackageName);

                foreach (var item in DataTable.Rows.OfType<DataRow>())
                {
                    if (item == null)
                    {
                        continue;
                    }

                    int partProcessID = (item[COLUMN_PART_PROCESS_ID] as int?) ?? 0;
                    var partProcess = Dataset.PartProcess.FindByPartProcessID(partProcessID);

                    if (partProcess != null && partProcess.IsValidState())
                    {
                        ProcessPackageDataset.ProcessPackage_ProcessesRow processInPackage = dsProcesses.ProcessPackage_Processes.NewProcessPackage_ProcessesRow();
                        processInPackage.ProcessID = partProcess.ProcessID;
                        processInPackage.StepOrder = partProcess.StepOrder;
                        processInPackage.ProcessAliasID = partProcess.ProcessAliasID;
                        processInPackage.ProcessPackageRow = processPackage;

                        dsProcesses.ProcessPackage_Processes.AddProcessPackage_ProcessesRow(processInPackage);
                    }
                }

                using (var tm = new TableAdapterManager())
                {
                    tm.ProcessPackageTableAdapter = taPP;
                    tm.ProcessPackage_ProcessesTableAdapter = taPPP;

                    tm.UpdateAll(dsProcesses);
                }
            }
            finally
            {
                dsProcesses?.Dispose();
                taPP?.Dispose();
                taPPP?.Dispose();
            }
        }

        public void AddProcesses(IEnumerable<ProcessPackageDataset.ProcessPackage_ProcessesRow> processes)
        {
            SyncPricePointData = true;

            foreach (ProcessPackageDataset.ProcessPackage_ProcessesRow item in processes)
            {
                //create new data source
                var cr = this.Dataset.PartProcess.NewPartProcessRow();
                cr.ProcessID = item.ProcessID;
                cr.ProcessAliasID = item.ProcessAliasID;
                cr.PartID = CurrentRecord.PartID;
                cr.StepOrder = GetNextProcessStepOrder(CurrentRecord);

                this.Dataset.PartProcess.Rows.Add(cr);
                cr.EndEdit();

                //NOTE: Don't set 'UnAnsweredQuestions' to allow validator to check to see if there are any unaswered questions

                var defaultPrice = cr.ProcessRow.IsPriceNull() ?
                    0M :
                    cr.ProcessRow.Price;

                var newRow = NewTempDataRow(cr, defaultPrice, ProcessData.From(cr.ProcessRow));
                DataTable.Rows.Add(newRow);
                SyncQuantity(newRow);
                SyncWeight(newRow);
            }
        }

        public void SaveRow()
        {
            if (!SyncPricePointData)
            {
                return;
            }

            // Save price data
            // (The widget automatically changes PartProcess values.)
            foreach (var dataRow in DataTable.Rows.OfType<DataRow>())
            {
                if (dataRow == null)
                {
                    continue;
                }

                int partProcessID = (dataRow[COLUMN_PART_PROCESS_ID] as int?) ?? 0;
                var partProcess = Dataset.PartProcess.FindByPartProcessID(partProcessID);

                if (partProcess != null && partProcess.IsValidState())
                {
                    var existingVolumePriceData = Dataset.PartProcessVolumePrice.Where(i => i.IsValidState() && i.PartProcessID == partProcessID);

                    foreach (var pricePoint in PricePoints)
                    {
                        var existingPriceRow = existingVolumePriceData.FirstOrDefault(row => pricePoint.Matches(row));
                        var newAmount = (dataRow[TempTableColumnName(pricePoint)] as decimal?) ?? 0M;

                        if (existingPriceRow == null)
                        {
                            // Add
                            var newRow = Dataset.PartProcessVolumePrice.NewPartProcessVolumePriceRow();
                            newRow.Amount = newAmount;
                            newRow.PartProcessRow = partProcess;
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

                            Dataset.PartProcessVolumePrice.AddPartProcessVolumePriceRow(newRow);
                        }
                        else if (existingPriceRow != null)
                        {
                            existingPriceRow.Amount = newAmount;
                        }
                    }
                }
            }
        }

        public decimal GetTotal(PricePointData changedPricePoint)
        {
            return DataTable
                .Rows
                .OfType<DataRow>()
                .Sum(row => (row[TempTableColumnName(changedPricePoint)] as decimal?) ?? 0M);
        }

        public PartsDataset.PartProcessRow GetPartProcess(DataRow row)
        {
            var partProcessID = row[COLUMN_PART_PROCESS_ID] as int? ?? 0;
            return Dataset.PartProcess.FindByPartProcessID(partProcessID);
        }

        public static bool IsPriceColumn(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            else
            {
                return key.StartsWith(COLUMN_PREFIX_PRICE);
            }
        }

        public bool IsMissingPricingData(DataRow currentProcess)
        {
            if (currentProcess == null)
            {
                return false;
            }

            bool missingData = false;
            foreach (var pricePoint in PricePoints)
            {
                var tableColumn = TempTableColumnName(pricePoint);
                if (currentProcess.IsNull(tableColumn))
                {
                    missingData = true;
                    break;
                }

                var price = (currentProcess[tableColumn] as decimal?) ?? 0M;

                if (price == 0M)
                {
                    missingData = true;
                    break;
                }
            }

            return missingData;
        }

        private IEnumerable<PricePointData> GeneratePricePoints(PartsDataset.PartRow currentRecord)
        {
            bool hasExistingProcesses = currentRecord.GetPartProcessRows().Length > 0;
            bool hasExistingProcessPricing = false;

            if (hasExistingProcesses)
            {
                hasExistingProcessPricing = currentRecord
                    .GetPartProcessRows()
                    .Any(row => row.GetPartProcessVolumePriceRows().Length > 0);
            }

            if (hasExistingProcessPricing)
            {
                return GenerateFromExisting(currentRecord);
            }

            if (ApplicationSettings.Current.EnableVolumePricing)
            {
                var customerDefaults = GenerateFromCustomerDefaults(currentRecord.CustomerID);

                if (customerDefaults == null || customerDefaults.Count == 0)
                {
                    return GenerateFromSystemDefaults();
                }

                return customerDefaults;
            }

            // Create for non-volume pricing
            return PriceUnitPersistence.ActivePriceUnits
                .Select(PricePointData.Blank);
        }

        private List<PricePointData> GenerateFromExisting(PartsDataset.PartRow currentRecord)
        {
            var pricePoints = new List<PricePointData>();

            var pointsDictionary = new Dictionary<int, PricePointData>();

            foreach (var process in currentRecord.GetPartProcessRows())
            {
                foreach (var price in process.GetPartProcessVolumePriceRows())
                {
                    var pricePointData = PricePointData.From(price);
                    pointsDictionary[pricePointData.GetHashCode()] = pricePointData;
                }
            }

            pricePoints.AddRange(pointsDictionary.Values);

            if (pricePoints.All(point => OrderPrice.GetPriceByType(point.PriceUnit) == PriceByType.Quantity))
            {
                // Use weight-based price points from either the customer defaults or the system defaults
                var customerWeightDefaults = GenerateFromCustomerDefaults(currentRecord.CustomerID)
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

            return pricePoints;
        }

        public List<PricePointData> GenerateFromCustomerDefaults(int customerId)
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

                var activePriceUnits = PriceUnitPersistence
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

        public List<PricePointData> GenerateFromSystemDefaults()
        {
            var pricePoints = new List<PricePointData>();
            OrdersDataSet.PricePointDataTable dtPricePoint = null;
            OrdersDataSet.PricePointDetailDataTable dtPricePointDetail = null;

            try
            {
                dtPricePoint = new OrdersDataSet.PricePointDataTable();
                using (var taPrice = new Data.Datasets.OrdersDataSetTableAdapters.PricePointTableAdapter())
                {
                    taPrice.FillVolumeDefault(dtPricePoint);
                }

                int pricePointID = dtPricePoint.FirstOrDefault()?.PricePointID ?? 0;

                dtPricePointDetail = new OrdersDataSet.PricePointDetailDataTable();
                using (var taDetail = new Data.Datasets.OrdersDataSetTableAdapters.PricePointDetailTableAdapter())
                {
                    taDetail.FillByPricePoint(dtPricePointDetail, pricePointID);
                }

                var activePriceUnits = PriceUnitPersistence
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
            }
            finally
            {
                dtPricePoint?.Dispose();
                dtPricePointDetail?.Dispose();
            }

            return pricePoints;
        }

        private DataRow NewTempDataRow(PartsDataset.PartProcessRow newPartProcess, decimal defaultPrice, ProcessData defaultProcessData)
        {
            var defaultQty = defaultProcessData?.LoadQuantity;
            var defaultWeight = defaultProcessData?.LoadWeight;
            var defaultLeadTimeHours = defaultProcessData?.LeadTimeHours;
            var defaultLeadTimeType = defaultProcessData?.LeadTimeType;

            // If one is not present, try calculating the other
            if (CurrentRecord != null)
            {
                var partWeight = CurrentRecord.IsWeightNull() ?
                   0M :
                   CurrentRecord.Weight;

                if (defaultQty.HasValue && !defaultWeight.HasValue)
                {
                    defaultWeight = defaultQty.Value * partWeight;
                }
                else if (defaultWeight.HasValue && !defaultQty.HasValue)
                {
                    if (partWeight == 0)
                    {
                        defaultQty = 0;
                    }
                    else
                    {
                        defaultQty = Convert.ToInt32(defaultWeight.Value / partWeight);
                    }
                }
            }

            var newRow = DataTable.NewRow();


            using (var daProcesses = new Data.Datasets.PartsDatasetTableAdapters.ProcessTableAdapter())
            {
                PartsDataset.ProcessDataTable dtProcesses = new PartsDataset.ProcessDataTable();
                daProcesses.Fill(dtProcesses);
                PartsDataset.ProcessRow drProcess = dtProcesses.FindByProcessID(newPartProcess.ProcessID);

                newRow[COLUMN_DEPT] = (drProcess.Department == null) ? "":drProcess.Department;

            }
           



            newRow[COLUMN_ALIAS] = newPartProcess.ProcessAliasRow?.Name ?? newPartProcess.PartProcessID.ToString();

            if (newPartProcess.ProcessRow == null)
            {
                newRow[COLUMN_PROCESS] = newPartProcess.ProcessID;
            }
            else
            {
                newRow[COLUMN_PROCESS] = newPartProcess.ProcessRow.ProcessName;
            }

            newRow[COLUMN_PART_PROCESS_ID] = newPartProcess.PartProcessID;
            newRow[COLUMN_STEP_ORDER] = newPartProcess.StepOrder;

            // Load Capacity
            if (!newPartProcess.IsLoadCapacityQuantityNull())
            {
                newRow[COLUMN_LOAD_CAPACITY_QUANTITY] = newPartProcess.LoadCapacityQuantity;
            }
            else if (defaultQty.HasValue)
            {
                newRow[COLUMN_LOAD_CAPACITY_QUANTITY] = defaultQty.Value;
            }

            if (!newPartProcess.IsLoadCapacityWeightNull())
            {
                newRow[COLUMN_LOAD_CAPACITY_WEIGHT] = newPartProcess.LoadCapacityWeight;
            }
            else if (defaultWeight.HasValue)
            {
                newRow[COLUMN_LOAD_CAPACITY_WEIGHT] = defaultWeight;
            }

            // Lead Time
            if (!newPartProcess.IsLeadTimeHoursNull())
            {
                newRow[COLUMN_LEAD_TIME_HOURS] = newPartProcess.LeadTimeHours;
            }
            else if (defaultLeadTimeHours.HasValue)
            {
                newRow[COLUMN_LEAD_TIME_HOURS] = defaultLeadTimeHours;
            }

            if (!newPartProcess.IsLeadTimeTypeNull())
            {
                newRow[COLUMN_LEAD_TIME_TYPE] = newPartProcess.LeadTimeType;
            }
            else if (!string.IsNullOrEmpty(defaultLeadTimeType))
            {
                newRow[COLUMN_LEAD_TIME_TYPE] = defaultLeadTimeType;
            }

            foreach (var pricePoint in PricePoints)
            {
                // Find value
                var matchingRow = newPartProcess
                    .GetPartProcessVolumePriceRows()
                    .FirstOrDefault(row => pricePoint.Matches(row));

                newRow[TempTableColumnName(pricePoint)] = matchingRow?.Amount ?? defaultPrice;
            }

            bool isApproved = (newPartProcess.ProcessRow?.Active ?? false) &&
                (!newPartProcess.ProcessRow?.IsIsApprovedNull() ?? false) &&
                (newPartProcess.ProcessRow?.IsApproved ?? false);

            newRow[COLUMN_APPROVED] = isApproved;

            int inactiveInspectionCount;
            using (var taProcess = new Data.Datasets.PartsDatasetTableAdapters.ProcessTableAdapter())
            {
                inactiveInspectionCount = taProcess.GetInactiveInspectionCount(newPartProcess.ProcessID) ?? 0;
            }

            newRow[COLUMN_HAS_CLOSED_INSPECTIONS] = inactiveInspectionCount > 0;
            return newRow;
        }

        /// <summary>
        /// Returns a default value to use during sync with part each/lot price.
        /// </summary>
        /// <param name="newTotal"></param>
        /// <param name="partProcess"></param>
        /// <returns></returns>
        private decimal DefaultValueForSync(decimal newTotal, PartsDataset.PartProcessRow partProcess)
        {
            if (partProcess == null || partProcess.ProcessRow == null)
            {
                return 0M;
            }

            int priceDecimals = ApplicationSettings.Current.PriceDecimalPlaces;

            var processRow = partProcess.ProcessRow;

            var sumProcessTarget = CurrentRecord
                .GetPartProcessRows()
                .Sum(i => (i.ProcessRow == null || i.ProcessRow.IsPriceNull()) ? 0M : i.ProcessRow.Price);

            decimal newPrice;
            if (sumProcessTarget == 0M)
            {
                newPrice = Math.Round(newTotal / Convert.ToDecimal(CurrentRecord.GetPartProcessRows().Length),
                    priceDecimals,
                    MidpointRounding.AwayFromZero);
            }
            else
            {
                var processTargetPrice = processRow.IsPriceNull()
                    ? 0M
                    : processRow.Price;

                // Round calculation so that it uses the specified precision
                newPrice = Math.Round(newTotal * (processTargetPrice / sumProcessTarget),
                    priceDecimals,
                    MidpointRounding.AwayFromZero);
            }

            return newPrice;
        }

        private static int GetNextProcessStepOrder(PartsDataset.PartRow partRow)
        {
            var processes = partRow.GetPartProcessRows();
            var hasProcesses = processes == null || processes.Length == 0;

            return hasProcesses ? 1 : partRow.GetPartProcessRows().Max(p => p.StepOrder) + 1;
        }

        #endregion

        #region ProcessLoadCapacity

        private sealed class ProcessData
        {
            #region Properties

            public int? LoadQuantity
            {
                get;
                set;
            }

            public decimal? LoadWeight
            {
                get;
                set;
            }

            public decimal? LeadTimeHours
            {
                get;
                set;
            }

            public string LeadTimeType
            {
                get;
                set;
            }

            #endregion

            #region Methods

            public static ProcessData From(PartsDataset.ProcessRow process)
            {
                if (process == null)
                {
                    return new ProcessData();
                }

                int? quantity = null;
                decimal? weight = null;

                if (!process.IsLoadCapacityNull())
                {
                    var capacityType = process.IsLoadCapacityTypeNull() ?
                        string.Empty :
                        process.LoadCapacityType.ToUpper();

                    if (capacityType == "QUANTITY")
                    {
                        var roundedCapacity = Math.Round(process.LoadCapacity, MidpointRounding.AwayFromZero);
                        quantity = Math.Min(Convert.ToInt32(roundedCapacity), MAX_QUANTITY);
                    }
                    else
                    {
                        weight = Math.Min(process.LoadCapacity, MAX_WEIGHT);
                    }
                }

                // Lead time
                decimal? leadTimeHours = null;
                string leadTimeType = null;
                if (!process.IsLeadTimeHoursNull() && !process.IsLeadTimeTypeNull())
                {
                    leadTimeHours = process.LeadTimeHours;
                    leadTimeType = process.LeadTimeType;
                }

                return new ProcessData()
                {
                    LoadQuantity = quantity,
                    LoadWeight = weight,
                    LeadTimeHours = leadTimeHours,
                    LeadTimeType = leadTimeType
                };
            }

            #endregion
        }

        #endregion
    }
}
