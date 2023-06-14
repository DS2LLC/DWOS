using DWOS.Shared.Utilities;
using Infragistics.Documents.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using DWOS.Data;
using DWOS.Data.Datasets;

namespace DWOS.DataImporter
{
    public class PartExchange : DataExchange
    {
        #region Fields

        /// <summary>
        /// Maps customer names to customer IDs
        /// </summary>
        private readonly List<CustomerMap> _customerLookups =
            new List<CustomerMap>();

        /// <summary>
        /// Maps name to process & alias.
        /// </summary>
        private readonly List<ProcessMap> _processLookups =
            new List<ProcessMap>();

        #endregion

        #region Properties

        public Data.Datasets.PartsDataset Dataset { get; set; }

        #endregion

        #region Methods

        public PartExchange(IUserNotifier notifier) :
            base(notifier)
        {

        }

        protected override void Export(string file)
        {
            this.Dataset = new Data.Datasets.PartsDataset() { EnforceConstraints = false };

            Notifier.ShowNotification("Loading current parts...");
            
            using (var ta = new Data.Datasets.PartsDatasetTableAdapters.CustomerTableAdapter())
                ta.Fill(Dataset.Customer);
            using (var ta = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
                ta.Fill(Dataset.Part);

            Dataset.Part.Columns.Add("CustomerName", typeof(string));
            foreach(var part in Dataset.Part)
            {
                if (part.CustomerRow != null)
                    part["CustomerName"] = part.CustomerRow.Name;
            }

            Notifier.ShowNotification("Loaded {0} parts".FormatWith(Dataset.Part.Count));
            
            var report = ExportToWorkbook(Dataset.Part);
            report.Save(file);
            Notifier.ShowNotification("Exported file to {0}".FormatWith(file));

            if (System.IO.File.Exists(file))
                Process.Start(new ProcessStartInfo(file));
        }

        protected override void Import(string file)
        {
            Notifier.ShowNotification("Strarting part import from file '{0}'.".FormatWith(file));

            this.Dataset = new Data.Datasets.PartsDataset() { EnforceConstraints = false };

            Notifier.ShowNotification("Loading existing customers from database.");
            
            //load current customers
            using (var ta = new Data.Datasets.PartsDatasetTableAdapters.CustomerTableAdapter())
                ta.Fill(Dataset.Customer);
            using (var ta = new Data.Datasets.PartsDatasetTableAdapters.d_MaterialTableAdapter())
                ta.Fill(Dataset.d_Material);

            // load active parts
            using (var ta = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
            {
                ta.Fill(Dataset.Part);
            }

            // Load processes and aliases
            using (var ta = new Data.Datasets.PartsDatasetTableAdapters.ProcessTableAdapter())
            {
                ta.Fill(Dataset.Process);
            }

            using (var ta = new Data.Datasets.PartsDatasetTableAdapters.ProcessAliasTableAdapter())
            {
                ta.Fill(Dataset.ProcessAlias);
            }

            //load worksheet
            var workBook    = Workbook.Load(file);
            var workSheet   = workBook.Worksheets[0];

            //load field mappings
            var fieldMaps       = FieldMapCollection.LoadFieldMappings(this.Dataset.Part.TableName) ?? FieldMapCollection.CreateFieldMap(workSheet, this.Dataset.Part);

            if (!fieldMaps.FieldMaps.Any(w => w.ColumnName == "Name"))
            {
                string errorMessage = "Missing the following required columns: Name.";
                if (workBook.Worksheets.Count > 1)
                {
                    errorMessage += "\n\n" + "The import workbook has more than one worksheet." +
                        "\n" + "Please delete all worksheets except for the one you want to import.";
                }

                MessageBox.Show(errorMessage); 
                return;
            }

            var customerNameMap = FieldMap.CreateFieldMap(workSheet, "CustomerName");

            if(customerNameMap == null)
            {
                MessageBox.Show("Unable to find the required field 'CustomerName'");
                return;
            }

            var partNameMap = FieldMap.CreateFieldMap(workSheet, "Name");
            var partIdMap = FieldMap.CreateFieldMap(workSheet, "PartID");

            Notifier.ShowNotification("Beginning import of each row from the source data.");

            var mapper = new PartFieldMapper(this.Dataset.Part, this.Notifier);

            //foreach worksheet row, skipping header row
            foreach (var row in workSheet.Rows.Skip(1))
            {
                var failedAddingProcess = false;

                // Must have customer name
                var customerName = FieldMapper.GetString(customerNameMap, row);

                if (String.IsNullOrWhiteSpace(customerName))
                {
                    Notifier.ShowNotification("Row {0} has no {1}, skipping row.".FormatWith(row.Index, customerNameMap.ColumnName));
                    continue;
                }

                // Must have part name
                var partName = FieldMapper.GetString(partNameMap, row);

                if (string.IsNullOrEmpty(partName))
                {
                    Notifier.ShowNotification("Row {0} has no {1}, skipping row.".FormatWith(row.Index, partNameMap.ColumnName));
                    continue;
                }

                //get the name mapping to the customer id
                var customerLookup = _customerLookups.FirstOrDefault(w => w.Name == customerName);
                
                if (customerLookup == null)
                {
                    var foundCustomerRow = Dataset.Customer.FirstOrDefault(cr => cr.Name.Trim() == customerName.Trim());

                    //does not exist in Database
                    if(foundCustomerRow == null)
                    {
                        //lets map to an existing customer
                        var selectCustomer = new DWOS.DataImporter.Controls.SelectCustomer();
                        selectCustomer.LoadData(customerName, this.Dataset.Customer);
                        selectCustomer.ShowDialog();

                        if(selectCustomer.Status == Controls.SelectCustomer.ResultStatus.OK)
                            foundCustomerRow = selectCustomer.SelectedRow as Data.Datasets.PartsDataset.CustomerRow;
                        else if(selectCustomer.Status == Controls.SelectCustomer.ResultStatus.ABORT)
                        {
                            if(MessageBox.Show("Do you want to export the data already mapped?", "Export Data", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                Notifier.ShowNotification("Exporting existing mappings...");
                                break;
                            }
                            else
                            {
                                Notifier.ShowNotification("Aborting per user...");
                                return;
                            }
                        }
                    }

                    customerLookup = new CustomerMap() { ID = foundCustomerRow == null ? 0 : foundCustomerRow.CustomerID, Name = customerName };
                    _customerLookups.Add(customerLookup);
                }

                if(customerLookup.ID < 1)
                    continue;

                // Try to find matching active part
                PartsDataset.PartRow matchingPartRow;

                var partId = partIdMap == null
                    ? -1
                    : FieldMapper.GetInt32(partIdMap, row) ?? -1;

                if (partId >= 0)
                {
                    matchingPartRow = Dataset.Part
                        .FirstOrDefault(p => p.PartID == partId);
                }
                else
                {
                    var formattedPartName = partName.Trim().ToUpper();

                    matchingPartRow = Dataset.Part
                        .FirstOrDefault(p => p.CustomerID == customerLookup.ID && p.Name == formattedPartName &&
                                             p.RowState != DataRowState.Added);
                }

                if (matchingPartRow == null)
                {
                    // Add new part
                    PartsDataset.PartRow partRow = this.Dataset.Part.NewPartRow();
                    SetDefaults(partRow);

                    mapper.MapAll(fieldMaps.FieldMaps, row, partRow);

                    //if material is set but no matching row then add the material
                    if(!partRow.IsMaterialNull() && partRow.d_MaterialRow == null)
                        this.Dataset.d_Material.Addd_MaterialRow(partRow.Material, true);

                    //set customer ID
                    partRow["CustomerID"] = customerLookup.ID;

                    if (!IsValid(partRow))
                    {
                        Notifier.ShowNotification("Part row is not valid with row index: " + row.Index);
                    }
                    else
                    {
                        this.Dataset.Part.AddPartRow(partRow);
                        var processNames = mapper
                            .GetProcessNames(fieldMaps.UnmappedFields, row)
                            .ToList();

                        if (!AddProcesses(partRow, processNames))
                        {
                            Notifier.ShowNotification($"Skipped processes at row index: {row.Index}");
                            failedAddingProcess = true;
                        }
                    }
                }
                else
                {
                    // Modify existing part
                    Notifier.ShowNotification($"Found existing part - {partName}");

                    var previousEachPrice = matchingPartRow.IsEachPriceNull()
                        ? (decimal?) null
                        : matchingPartRow.EachPrice;

                    var previousLotPrice = matchingPartRow.IsLotPriceNull()
                        ? (decimal?) null
                        : matchingPartRow.LotPrice;

                    mapper.MapAll(fieldMaps.FieldMaps, row, matchingPartRow);

                    //if material is set but no matching row then add the material
                    if (!matchingPartRow.IsMaterialNull() && matchingPartRow.d_MaterialRow == null)
                        this.Dataset.d_Material.Addd_MaterialRow(matchingPartRow.Material, true);

                    if (!IsValid(matchingPartRow))
                    {
                        Notifier.ShowNotification("Part row is not valid with row index: " + row.Index);
                        matchingPartRow.RejectChanges();
                    }
                    else
                    {
                        var eachPrice = matchingPartRow.IsEachPriceNull()
                            ? (decimal?) null
                            : matchingPartRow.EachPrice;

                        var lotPrice = matchingPartRow.IsLotPriceNull()
                            ? (decimal?) null
                            : matchingPartRow.LotPrice;

                        if (previousEachPrice != eachPrice || previousLotPrice != lotPrice)
                        {
                            UpdateProcessPrices(matchingPartRow);
                        }

                        if (mapper.GetProcessNames(fieldMaps.UnmappedFields, row).Any())
                        {
                            Notifier.ShowNotification(
                                $"Cannot add processes for existing part with row index: {row.Index}");
                        }
                    }
                }

                if (failedAddingProcess)
                {
                    Notifier.ShowNotification("Aborting import - canceled out of process import");
                    return;
                }
            }

            var partCount = Dataset.Part.Count(p => p.RowState == DataRowState.Added || p.RowState == DataRowState.Modified);

            var messageBoxText = $"Are you sure you want to save the {partCount} parts to the database?";

            if(MessageBox.Show(messageBoxText, "IMPORT", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Notifier.ShowNotification($"Saving {partCount} parts to the database.");

                var newParts = Dataset.Part.Where(part => part.RowState == DataRowState.Added).ToList();
                var existingParts = Dataset.Part.Where(part => part.RowState == DataRowState.Modified).ToList();

                using(var taManager = new Data.Datasets.PartsDatasetTableAdapters.TableAdapterManager())
                {
                    taManager.d_MaterialTableAdapter = new Data.Datasets.PartsDatasetTableAdapters.d_MaterialTableAdapter();
                    taManager.PartTableAdapter = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter();
                    taManager.PartProcessVolumePriceTableAdapter = new Data.Datasets.PartsDatasetTableAdapters.PartProcessVolumePriceTableAdapter();
                    taManager.PartProcessTableAdapter = new Data.Datasets.PartsDatasetTableAdapters.PartProcessTableAdapter();
                    var updateCount = taManager.UpdateAll(this.Dataset);
                    UpdateHistory(newParts, existingParts);

                    Notifier.ShowNotification("Import completed {0} rows.".FormatWith(updateCount));
                }
            }
            else
                Notifier.ShowNotification("Import canceled.");
        }

        private bool AddProcesses(PartsDataset.PartRow partRow, IEnumerable<string> processNames)
        {
            var skippedAddingProcess = false;

            var processes = new List<ProcessMap>();

            var approvedAliasRows = Dataset.ProcessAlias
                .Where(alias => alias.ProcessRow.Active && !alias.ProcessRow.IsIsApprovedNull() && alias.ProcessRow.IsApproved)
                .ToList();

            foreach (var processName in processNames)
            {
                var processMatch = _processLookups.FirstOrDefault(map => map.Name == processName);

                if (processMatch == null)
                {
                    var aliasRow = approvedAliasRows
                        .FirstOrDefault(alias => alias.Name == processName);

                    if (aliasRow == null)
                    {
                        // Show dialog to match alias
                        var selectProcessAlias = new Controls.SelectProcessAlias();
                        selectProcessAlias.LoadData(processName, Dataset);

                        if (selectProcessAlias.ShowDialog() ?? false)
                        {
                            aliasRow = selectProcessAlias.SelectedProcessAliasRow;
                        }
                        else
                        {
                            skippedAddingProcess = true;
                            break;
                        }
                    }

                    if (aliasRow != null)
                    {
                        processMatch = new ProcessMap
                        {
                            Name = processName,
                            ProcessAliasId = aliasRow.ProcessAliasID,
                            ProcessId = aliasRow.ProcessID
                        };

                        _processLookups.Add(processMatch);
                    }
                }

                processes.Add(processMatch);
            }

            if (!skippedAddingProcess)
            {
                var stepOrder = 1;
                foreach (var process in processes)
                {
                    var processRow = Dataset.Process.FindByProcessID(process.ProcessId);
                    var processAliasRow = Dataset.ProcessAlias.FindByProcessAliasID(process.ProcessAliasId);

                    var partProcessRow = Dataset.PartProcess.NewPartProcessRow();
                    partProcessRow.PartRow = partRow;
                    partProcessRow.ProcessRow = processRow;
                    partProcessRow.ProcessAliasRow = processAliasRow;
                    partProcessRow.StepOrder = stepOrder;
                    Dataset.PartProcess.AddPartProcessRow(partProcessRow);

                    stepOrder++;
                }
            }

            return !skippedAddingProcess;
        }

        private void UpdateHistory(List<PartsDataset.PartRow> newParts, List<PartsDataset.PartRow> existingParts)
        {
            using (var taHistory = new Data.Datasets.OrderHistoryDataSetTableAdapters.PartHistoryTableAdapter())
            {
                foreach (var newPart in newParts ?? Enumerable.Empty<PartsDataset.PartRow>())
                {
                    taHistory.UpdatePartHistory(newPart.PartID,
                        "Data Migration",
                        "Imported new part.",
                        "Server");
                }

                foreach (var existingPart in existingParts ?? Enumerable.Empty<PartsDataset.PartRow>())
                {
                    taHistory.UpdatePartHistory(existingPart.PartID,
                        "Data Migration",
                        "Updated part during data migration.",
                        "Server");
                }
            }
        }

        private void UpdateProcessPrices(PartsDataset.PartRow part)
        {
            if (part == null)
            {
                return;
            }

            using (var taPartProcess = new Data.Datasets.PartsDatasetTableAdapters.PartProcessTableAdapter { ClearBeforeFill = false })
            {
                taPartProcess.FillByPart(Dataset.PartProcess, part.PartID);
            }

            using (var taPartProcessPrice = new Data.Datasets.PartsDatasetTableAdapters.PartProcessVolumePriceTableAdapter { ClearBeforeFill = false })
            {
                taPartProcessPrice.FillByPartID(Dataset.PartProcessVolumePrice, part.PartID);
            }

            var processPriceRows = part.GetPartProcessRows()
                .SelectMany(proc => proc.GetPartProcessVolumePriceRows())
                .ToList();

            if (processPriceRows.Count == 0)
            {
                // Part does not have existing process-level prices.
                return;
            }

            Notifier.ShowNotification($"Updating process-level prices for {part.Name}");

            var eachPricePoints = processPriceRows
                .Where(p => OrderPrice.GetPricingStrategy(OrderPrice.ParsePriceUnit(p.PriceUnit)) == PricingStrategy.Each)
                .GroupBy(p => p.IsMinValueNull() ? string.Empty : p.MinValue)
                .OrderBy(m => m.Key);

            var lotPricePoints = processPriceRows
                .Where(p => OrderPrice.GetPricingStrategy(OrderPrice.ParsePriceUnit(p.PriceUnit)) == PricingStrategy.Lot)
                .GroupBy(p => p.IsMinValueNull() ? string.Empty : p.MinValue)
                .OrderBy(m => m.Key);

            if (!part.IsEachPriceNull())
            {
                var eachPriceTotal = part.EachPrice;

                var primaryPricePoint = eachPricePoints.FirstOrDefault();

                var conversionFactor = 0M;
                if (primaryPricePoint != null)
                {
                    var oldTotal = primaryPricePoint.Sum(p => p.Amount);

                    if (oldTotal != 0M)
                    {
                        conversionFactor = eachPriceTotal / oldTotal;
                    }
                }

                foreach (var point in eachPricePoints)
                {
                    foreach (var processPrice in point)
                    {
                        processPrice.Amount = RoundPrice(processPrice.Amount * conversionFactor);
                    }
                }
            }

            if (!part.IsLotPriceNull())
            {
                var lotPriceTotal = part.LotPrice;

                var primaryPricePoint = lotPricePoints.FirstOrDefault();

                var conversionFactor = 0M;
                if (primaryPricePoint != null)
                {
                    var oldTotal = primaryPricePoint.Sum(p => p.Amount);

                    if (oldTotal != 0M)
                    {
                        conversionFactor = lotPriceTotal / oldTotal;
                    }
                }

                foreach (var point in lotPricePoints)
                {
                    foreach (var processPrice in point)
                    {
                        processPrice.Amount = RoundPrice(processPrice.Amount * conversionFactor);
                    }
                }
            }
        }

        private static decimal RoundPrice(decimal price) => Math.Round(price,
            ApplicationSettings.Current.PriceDecimalPlaces,
            MidpointRounding.AwayFromZero);

        private void SetDefaults(Data.Datasets.PartsDataset.PartRow part)
        {
            part.Active = true;
            part.Length = 0;
            part.Width = 0;
            part.Height = 0;
            part.SurfaceArea = 0;
            part.PartMarking = false;
            part.LastModified = DateTime.Now;
        }

        #endregion
    }
}