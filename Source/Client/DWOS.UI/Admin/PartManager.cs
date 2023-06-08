using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.PartsDatasetTableAdapters;
using DWOS.UI.Admin.PartManagerPanels;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Admin.Templates;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using NLog;

namespace DWOS.UI.Admin
{
    public partial class PartManager : DataEditorBase
    {
        #region Fields

        private readonly Dictionary<int, PartLoaded> _customersLoaded = new Dictionary<int, PartLoaded>();
        private int _customerFilterID = -1;
        private int _lastSelectedPartID = -1;
        private int _partProcessChangeCount;
        private readonly HashSet<int> _partsWithUpgradedProcesses = new HashSet<int>();

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the currently filtered customer ID.
        /// </summary>
        /// <value> The current customer ID. </value>
        internal int CurrentCustomerID
        {
            get
            {
                if (CustomerFilter.SelectedItem is ValueListItem)
                    return Convert.ToInt32(((ValueListItem)CustomerFilter.SelectedItem).DataValue);

                return -1;
            }
            set
            {
                ValueListItem item = CustomerFilter.FindItemByValue(vli => Convert.ToInt32(vli.DataValue) == value);

                if (item != null)
                {
                    if (CustomerFilter.SelectedItem == item)
                        CustomerFilter.ResetSelectedIndex();

                    CustomerFilter.SelectedItem = item;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the customer filter ID.
        /// </summary>
        /// <value> The customer filter ID. </value>
        public int CustomerFilterID
        {
            get { return this._customerFilterID; }
            set { this._customerFilterID = value; }
        }

        /// <summary>
        ///     Gets or sets the selected part ID.
        /// </summary>
        /// <value> The selected part ID. </value>
        public int SelectedPartID
        {
            get { return this._lastSelectedPartID; }
            set { this._lastSelectedPartID = value; }
        }

        /// <summary>
        ///     Gets the number of part processes changed.
        /// </summary>
        /// <value> The part processes changed. </value>
        public int PartProcessesChanged
        {
            get { return this._partProcessChangeCount; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to allow selected part to be in active.
        /// </summary>
        /// <value> <c>true</c> if [allow in active selected part]; otherwise, <c>false</c> . </value>
        public bool AllowInActiveSelectedPart { get; set; }

        public string InitialPartFilterText { get; set; }

        private bool ShowInactive
        {
            get { return ((StateButtonTool)base.toolbarManager.Tools["DisplayInactive"]).Checked; }
            set { ((StateButtonTool)base.toolbarManager.Tools["DisplayInactive"]).Checked = value; }
        }

        private ComboBoxTool CustomerFilter
        {
            get { return base.toolbarManager.Tools["CustomerFilter"] as ComboBoxTool; }
        }

        /// <summary>
        ///     Gets a value indicating whether any of the data was saved back to the DB.
        /// </summary>
        /// <value><c>true</c> if [data changed]; otherwise, <c>false</c>.</value>
        public bool DataChanged { get; private set; }

        private int StatusBarPartCount
        {
            set { this.ultraStatusBar1.Panels["PartCount"].Text = "Parts: " + value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if process prices should be
        /// automatically synced.
        /// </summary>
        public bool AutoSyncProcessPrices
        {
            get; set;
        }


        public int QuotePartID { get; set; }
        #endregion

        #region Methods

        public PartManager()
        {
            InitializeComponent();

            base.AddDataPanel(this.dpPartInfo);
            base.AddDataPanel(this.dpPartMarkInfo);
            InitializePanels();

            AllowInActiveSelectedPart = true;
            tvwTOC.Override.SortComparer = new PartManagerNodeSorter();
            tvwTOC.Override.SelectionType = SelectType.Extended;
        }

        private void LoadData()
        {
            this.dsParts.EnforceConstraints = false;
            this.dsParts.Part.BeginLoadData();
            this.dsParts.Part_Media.BeginLoadData();
            this.dsParts.Media.BeginLoadData();
            this.dsParts.d_Material.BeginLoadData();
            this.dsParts.Customer.BeginLoadData();
            this.dsParts.PartProcess.BeginLoadData();
            this.dsParts.d_Airframe.BeginLoadData();
            dsParts.PartLevelCustomField.BeginLoadData();
            dsParts.Lists.BeginLoadData();
            dsParts.ListValues.BeginLoadData();

            this.taAirframe.Fill(this.dsParts.d_Airframe);
            this.taManufacturer.Fill(this.dsParts.d_Manufacturer);
            this.taMaterials.Fill(this.dsParts.d_Material);
            this.taCustomer.FillByActive(this.dsParts.Customer);
            this.taPriceUnit.Fill(this.dsParts.PriceUnit);
            this.taProcessAlias.Fill(this.dsParts.ProcessAlias);

            //fill all process names
            using (var ta = new ProcessTableAdapter())
                ta.Fill(this.dsParts.Process);

            taPartLevelCustomField.Fill(dsParts.PartLevelCustomField);
            taLists.Fill(dsParts.Lists);
            taListValues.Fill(dsParts.ListValues);

            this.dsParts.d_Airframe.EndLoadData();
            this.dsParts.Part.EndLoadData();
            this.dsParts.Part_Media.EndLoadData();
            this.dsParts.Media.EndLoadData();
            this.dsParts.d_Material.EndLoadData();
            this.dsParts.Customer.EndLoadData();
            this.dsParts.PartProcess.EndLoadData();
            dsParts.PartLevelCustomField.EndLoadData();
            dsParts.Lists.EndLoadData();
            dsParts.ListValues.EndLoadData();
            this.dsParts.EnforceConstraints = true;

            this.dpPartInfo.LoadData(this.dsParts);
            this.dpPartMarkInfo.LoadData(this.dsParts);
            this.dsParts.PartProcess.PartProcessRowDeleting += PartProcess_PartProcessRowDeleting;

            DataUtilities.LoadComboBox(CustomerFilter, this.dsParts.Customer, this.dsParts.Customer.CustomerIDColumn.ColumnName, this.dsParts.Customer.NameColumn.ColumnName);
        }

        private void LoadData(int customerID)
        {
            using (new UsingWaitCursor(this))
            {
                PartLoaded partLoaded;

                if (this._customersLoaded.ContainsKey(customerID))
                    partLoaded = this._customersLoaded[customerID];
                else
                {
                    partLoaded = new PartLoaded { CustomerID = customerID, ActiveLoaded = false, InActiveLoaded = false };
                    this._customersLoaded.Add(customerID, partLoaded);
                }

                //if active not loaded
                if (!partLoaded.ActiveLoaded)
                {
                    _log.Debug("Load ACTIVE data for customer: " + customerID);

                    this.dsParts.EnforceConstraints = false;
                    this.dsParts.Part.BeginLoadData();

                    this.taManager.PartTableAdapter.FillByCustomer(this.dsParts.Part, customerID, true);

                    this.dsParts.Part.EndLoadData();
                    this.dsParts.EnforceConstraints = true;

                    partLoaded.ActiveLoaded = true;
                }

                //if showing inactive and inactive not loaded
                if (ShowInactive && !partLoaded.InActiveLoaded)
                {
                    _log.Debug("Load INACTIVE data for customer: " + customerID);

                    this.dsParts.EnforceConstraints = false;
                    this.dsParts.Part.BeginLoadData();

                    this.taManager.PartTableAdapter.FillByCustomer(this.dsParts.Part, customerID, false);

                    this.dsParts.Part.EndLoadData();
                    this.dsParts.EnforceConstraints = true;

                    partLoaded.InActiveLoaded = true;
                }
            }
        }

        private void LoadTOC()
        {
            using (new UsingWaitCursor(this))
            {
                using (new UsingTreeLoad(tvwTOC))
                {
                    tvwTOC.Nodes.DisposeAll(true);
                    tvwTOC.Nodes.Clear();

                    this.txtNodeFilter.Text = null;

                    var rootNode = new PartsRootNode(this);
                    tvwTOC.Nodes.Add(rootNode);
                    rootNode.Expanded = true;

                    if (CustomerFilter.SelectedItem != null)
                    {
                        var customerID = CustomerFilter.SelectedItemDataValue<int>();
                        if (customerID > 0)
                        {
                            PartsDataset.CustomerRow cr = this.dsParts.Customer.FindByCustomerID(customerID);

                            if (cr != null)
                            {
                                //load part for the customer
                                LoadData(cr.CustomerID);

                                string filter = this.dsParts.Part.CustomerIDColumn.ColumnName + " = " + cr.CustomerID;
                                bool showInactive = ShowInactive;
                                var rows = this.dsParts.Part.Select(filter, this.dsParts.Part.NameColumn.ColumnName) as PartsDataset.PartRow[];

                                _log.Info("Loading nodes " + rows.Length);
                                foreach (var pr in rows)
                                    rootNode.Nodes.Add(new PartNode(pr, showInactive));

                                StatusBarPartCount = rows.Length;
                            }
                        }
                    }

                    rootNode.Select();
                }
            }
        }

        protected override void ReloadTOC() { LoadTOC(); }

        protected override bool SaveData()
        {
            try
            {
                base.EndAllEdits();

                //track number of changes to the part's processes
                DataTable dtChanges = this.dsParts.PartProcess.GetChanges();
                if (dtChanges != null)
                    this._partProcessChangeCount += dtChanges.Rows.Count;

                // Retrieve added & changed rows for later use
                var addedPartRows = DataUtilities
                    .GetRowsByRowState(dsParts.Part, DataRowState.Added)
                    .OfType<PartsDataset.PartRow>();

                var modifiedParts = DataUtilities.GetRowsAndValuesByRowState(dsParts.Part, DataRowState.Modified);
                    modifiedParts.Keys.Select(d => (PartsDataset.PartRow)d).Where(p => !p.IsCreatedInReceivingNull() && p.CreatedInReceiving).ForEach(pr => pr.CreatedInReceiving = false); //remove flag

                var addedProcesses = DataUtilities
                    .GetRowsByRowState(dsParts.PartProcess, DataRowState.Added)
                    .OfType<PartsDataset.PartProcessRow>();

                var modifiedProcesses = DataUtilities.GetRowsAndValuesByRowState(dsParts.PartProcess, DataRowState.Modified);

                var deletedProcesses = DataUtilities
                    .GetRowsAndValuesByRowState(dsParts.PartProcess, DataRowState.Deleted);

                var addedPartMarks = DataUtilities
                    .GetRowsByRowState(dsParts.Part_PartMarking, DataRowState.Added)
                    .OfType<PartsDataset.Part_PartMarkingRow>();

                var modifiedPartMarks = DataUtilities
                    .GetRowsByRowState(dsParts.Part_PartMarking, DataRowState.Modified)
                    .OfType<PartsDataset.Part_PartMarkingRow>();

                var deletedPartMarks = DataUtilities
                    .GetRowsAndValuesByRowState(dsParts.Part_PartMarking, DataRowState.Deleted);

                var partsWithAddedPricing = DataUtilities
                    .GetRowsByRowState(dsParts.PartProcessVolumePrice, DataRowState.Added)
                    .OfType<PartsDataset.PartProcessVolumePriceRow>()
                    .Select(volumePrice => volumePrice.PartProcessRow?.PartID ?? -1)
                    .ToList();

                var partsWithModifiedPricing = DataUtilities
                    .GetRowsByRowState(dsParts.PartProcessVolumePrice, DataRowState.Modified)
                    .OfType<PartsDataset.PartProcessVolumePriceRow>()
                    .Select(volumePrice => volumePrice.PartProcessRow?.PartID ?? -1)
                    .ToList();

                var partsWithVolumePriceChanges = new HashSet<int>(partsWithAddedPricing.Concat(partsWithModifiedPricing));

                var deletedPricing = DataUtilities
                    .GetRowsAndValuesByRowState(dsParts.Part_PartMarking, DataRowState.Deleted);

                var modifiedCustomFields = DataUtilities
                    .GetRowsAndValuesByRowState(dsParts.PartCustomFields, DataRowState.Modified);

                foreach (var deletedPriceEntry in deletedPricing)
                {
                    // Handle deleted price points just in case one exists
                    var partProcessId = deletedPriceEntry.Value[dsParts.PartProcessVolumePrice.PartProcessIDColumn.Ordinal] as int? ?? 0;
                    var partProcess = dsParts.PartProcess.FindByPartProcessID(partProcessId);

                    if (partProcess == null)
                    {
                        // Part process was deleted
                        continue;
                    }

                    partsWithVolumePriceChanges.Add(partProcess.PartID);
                }

                // Save changes
                var changes = UpdateDatabase();

                if (!changes.HasValue)
                {
                    _log.Info("User declined to merge changes.");
                    MessageBoxUtilities.ShowMessageBoxOK("Data was not saved.", "Order Entry");
                    return false;
                }

                //track any changes to data
                DataChanged = DataChanged || changes > 0;

                // Update part history
                using (var taHistory = new Data.Datasets.OrderHistoryDataSetTableAdapters.PartHistoryTableAdapter())
                {
                    // New parts
                    foreach (var newPartRow in addedPartRows)
                    {
                        taHistory.UpdatePartHistory(newPartRow.PartID,
                            Text,
                            newPartRow.IsParentIDNull() ? "Part created." : "Part revised.",
                            SecurityManager.Current.UserName);
                    }

                    // Modified parts
                    foreach(var modifiedPartEntry in modifiedParts)
                    {
                        if (!(modifiedPartEntry.Key is PartsDataset.PartRow modifiedPartRow))
                        {
                            continue;
                        }

                        var newValues = modifiedPartRow.ItemArray;

                        for (var i = 0; i < newValues.Length; i++)
                        {
                            if (Equals(newValues[i], modifiedPartEntry.Value[i]))
                            {
                                // Value was unchanged
                                continue;
                            }

                            var updateMsg = $"{modifiedPartRow.Table.Columns[i].ColumnName} " +
                                            $"value changed from {modifiedPartEntry.Value[i] ?? "NULL"} to " +
                                            $"{newValues[i] ?? "NULL"}";

                            taHistory.UpdatePartHistory(modifiedPartRow.PartID,
                                Text,
                                updateMsg,
                                SecurityManager.Current.UserName);
                        }
                    }


                    // Upgraded processes
                    foreach (var partId in _partsWithUpgradedProcesses)
                    {
                        if (dsParts.Part.FindByPartID(partId) == null)
                        {
                            // Part was deleted
                            continue;
                        }

                        taHistory.UpdatePartHistory(partId,
                            Text,
                            "Updated out-of-date processes for part.",
                            SecurityManager.Current.UserName);
                    }

                    _partsWithUpgradedProcesses.Clear();

                    // New processes
                    foreach (var newProcessRow in addedProcesses)
                    {
                        taHistory.UpdatePartHistory(newProcessRow.PartID,
                            Text,
                            $"Part process {newProcessRow.ProcessAliasRow?.Name ?? newProcessRow.ProcessAliasID.ToString()} added.",
                            SecurityManager.Current.UserName);
                    }

                    // Modified processes
                    foreach (var modifiedProcessEntry in modifiedProcesses)
                    {
                        if (!(modifiedProcessEntry.Key is PartsDataset.PartProcessRow modifiedProcessRow))
                        {
                            continue;
                        }

                        var newValues = modifiedProcessRow.ItemArray;

                        for (var i = 0; i < newValues.Length; i++)
                        {
                            if (Equals(newValues[i], modifiedProcessEntry.Value[i]))
                            {
                                // Value was unchanged
                                continue;
                            }

                            var updateMsg = "For process {0}; {1} value changed from {2} to {3}".FormatWith(
                                modifiedProcessRow.ProcessAliasRow?.Name ?? modifiedProcessRow.ProcessAliasID.ToString(),
                                modifiedProcessRow.Table.Columns[i].ColumnName,
                                modifiedProcessEntry.Value[i] ?? "NULL",
                                newValues[i] ?? "NULL");

                            taHistory.UpdatePartHistory(modifiedProcessRow.PartID,
                                Text,
                                updateMsg,
                                SecurityManager.Current.UserName);
                        }
                    }

                    // Deleted processes
                    foreach (var deletedProcessEntry in deletedProcesses)
                    {
                        var partId = deletedProcessEntry.Value[dsParts.PartProcess.PartIDColumn.Ordinal] as int? ?? 0;
                        var partRow = dsParts.Part.FindByPartID(partId);

                        var processAliasId = deletedProcessEntry.Value[dsParts.PartProcess.ProcessAliasIDColumn.Ordinal] as int? ?? 0;
                        var aliasRow = dsParts.ProcessAlias.FindByProcessAliasID(processAliasId);

                        if (partRow == null)
                        {
                            // Part was likely deleted
                            continue;
                        }

                        taHistory.UpdatePartHistory(partRow.PartID,
                            Text,
                            $"Removed process {aliasRow?.Name ?? processAliasId.ToString()} from part.",
                            SecurityManager.Current.UserName);
                    }

                    // Part mark changes
                    foreach (var newPartMarkRow in addedPartMarks)
                    {
                        taHistory.UpdatePartHistory(newPartMarkRow.PartID,
                            Text,
                            "Added part mark.",
                            SecurityManager.Current.UserName);
                    }

                    foreach (var newPartMarkRow in modifiedPartMarks)
                    {
                        taHistory.UpdatePartHistory(newPartMarkRow.PartID,
                            Text,
                            "Changed part mark.",
                            SecurityManager.Current.UserName);
                    }

                    foreach (var deletedPartMarkEntry in deletedPartMarks)
                    {
                        var partId = deletedPartMarkEntry.Value[dsParts.Part_PartMarking.PartIDColumn.Ordinal] as int? ?? 0;
                        var partRow = dsParts.Part.FindByPartID(partId);

                        if (partRow == null)
                        {
                            // Part was likely deleted
                            continue;
                        }

                        taHistory.UpdatePartHistory(partRow.PartID,
                            Text,
                            $"Removed part mark.",
                            SecurityManager.Current.UserName);
                    }

                    // Volume pricing changes
                    foreach (var partId in partsWithVolumePriceChanges)
                    {
                        var partRow = dsParts.Part.FindByPartID(partId);
                        if (partRow == null)
                        {
                            // Part with that ID doesn't exist. It's probably a
                            // new part.
                            continue;
                        }

                        taHistory.UpdatePartHistory(partRow.PartID,
                            Text,
                            "Changed per-process price data.",
                            SecurityManager.Current.UserName);
                    }

                    // Custom field changes
                    foreach (var modifiedCustomField in modifiedCustomFields)
                    {
                        if (!(modifiedCustomField.Key is PartsDataset.PartCustomFieldsRow modifiedRow))
                        {
                            continue;
                        }

                        var originalValue = modifiedCustomField.Value[dsParts.PartCustomFields.ValueColumn.Ordinal];
                        var newValue = modifiedRow.ItemArray[dsParts.PartCustomFields.ValueColumn.Ordinal];

                        if (Equals(originalValue, newValue))
                        {
                            continue;
                        }

                        var updateMsg = $"Custom field {modifiedRow.PartLevelCustomFieldID} " +
                                        $"value changed from `{originalValue}` to `{newValue}`";

                        taHistory.UpdatePartHistory(modifiedRow.PartID,
                            Text,
                            updateMsg,
                            SecurityManager.Current.UserName);
                    }
                }

                return true;
            }
            catch (Exception exc)
            {
                _log.Warn("DataSet Errors: " + dsParts.GetDataErrors());
                ErrorMessageBox.ShowDialog("Error saving data.", exc);

                // If any errors occur while PartManager is saving data, they
                // are likely major issues that cannot be corrected.
                MessageBoxUtilities.ShowMessageBoxError($"There was an error saving your data. Please close {Text} and try again.",
                    Text);

                return false;
            }
        }

        private int? UpdateDatabase()
        {
            while (true)
            {
                try
                {
                    return taManager.UpdateAll(dsParts);
                }
                catch (DBConcurrencyException exc)
                {
                    var knownTables = new List<string>
                    {
                        dsParts.Part.TableName,
                        dsParts.PartProcess.TableName,
                        dsParts.PartProcessVolumePrice.TableName
                    };

                    var rowsWithErrors = new DataRow[exc.RowCount];
                    exc.CopyToRows(rowsWithErrors);
                    if (rowsWithErrors.Any(s => !knownTables.Contains(s.Table.TableName)))
                    {
                        // Cannot handle error
                        _log.Warn($"Cannot handle concurrency error outside of {dsParts.Part.TableName}.");
                        throw;
                    }

                    _log.Warn(exc, "Concurrency error while saving data.");
                    const string promptText = "There are some changes on the " +
                        "server that need to be merged in order to save your " +
                        "changes.\n\nMerge changes?";

                    var prompt = MessageBoxUtilities.ShowMessageBoxYesOrNo(promptText, Text);
                    if (prompt != DialogResult.Yes)
                    {
                        return null;
                    }

                    var parts = rowsWithErrors.OfType<PartsDataset.PartRow>().ToList();
                    var partProcesses = rowsWithErrors.OfType<PartsDataset.PartProcessRow>().ToList();
                    var partVolumePrices = rowsWithErrors.OfType<PartsDataset.PartProcessVolumePriceRow>().ToList();

                    MergeParts(parts);
                    MergePartProcesses(partProcesses);
                    MergePartProcessVolumePrices(partVolumePrices);

                    var partIdCount = parts.Select(p => p.PartID)
                        .Concat(partProcesses.Select(p => p.PartID))
                        .Concat(partVolumePrices.Select(p => p.PartProcessRow.PartID))
                        .Distinct()
                        .Count();

                    var successText = $"Successfully merged your changes for {partIdCount} {(partIdCount == 1 ? "part" : "parts")}.";
                    MessageBoxUtilities.ShowMessageBoxOK(successText, Text);
                }
            }
        }

        private void MergeParts(ICollection<PartsDataset.PartRow> conflictedRows)
        {
            if (!conflictedRows.Any())
            {
                return;
            }

            using (var dsPartsTemp = new PartsDataset.PartDataTable())
            {
                foreach (var row in conflictedRows)
                {
                    taPart.FillByPartID(dsPartsTemp, row.PartID);
                }

                var mergeLogger = new MergeLogger(dsParts.Part, dsPartsTemp, dsParts.Part.PartIDColumn.ColumnName);
                mergeLogger.LogValues();

                dsParts.Part.Merge(dsPartsTemp, true, MissingSchemaAction.Error);
            }
        }

        private void MergePartProcesses(ICollection<PartsDataset.PartProcessRow> conflictedRows)
        {
            if (!conflictedRows.Any())
            {
                return;
            }

            using (var dsPartProcessTemp = new PartsDataset.PartProcessDataTable())
            {
                foreach (var row in conflictedRows)
                {
                    taPartProcess.FillById(dsPartProcessTemp, row.PartProcessID);
                }

                var mergeLogger = new MergeLogger(dsParts.PartProcess, dsPartProcessTemp,
                    dsParts.PartProcess.PartProcessIDColumn.ColumnName);

                mergeLogger.LogValues();

                dsParts.PartProcess.Merge(dsPartProcessTemp, true, MissingSchemaAction.Error);
            }
        }

        private void MergePartProcessVolumePrices(ICollection<PartsDataset.PartProcessVolumePriceRow> conflictedRows)
        {
            if (!conflictedRows.Any())
            {
                return;
            }

            using (var dsPartsVolumeTemp = new PartsDataset.PartProcessVolumePriceDataTable())
            {
                foreach (var row in conflictedRows)
                {
                    taPartProcessVolumePrice.FillById(dsPartsVolumeTemp, row.PartProcessVolumePriceID);
                }

                var mergeLogger = new MergeLogger(dsParts.PartProcessVolumePrice, dsPartsVolumeTemp,
                    dsParts.PartProcessVolumePrice.PartProcessVolumePriceIDColumn.ColumnName);

                mergeLogger.LogValues();

                dsParts.PartProcessVolumePrice.Merge(dsPartsVolumeTemp, true, MissingSchemaAction.Error);
            }
        }

        private void LoadCommands()
        {
            if (SecurityManager.Current.IsInRole("PartManager.Edit"))
            {
                var add = base.Commands.AddCommand("Add", new AddCommand(toolbarManager.Tools["Add"], tvwTOC)) as AddCommand;
                add.AddNode = AddNode;

                base.Commands.AddCommand("History", new HistoryCommand(toolbarManager.Tools["History"], tvwTOC));
                base.Commands.AddCommand("Delete", new DeleteCommand(toolbarManager.Tools["Delete"], tvwTOC, this));
                base.Commands.AddCommand("Copy", new CopyPasteCommand(toolbarManager.Tools["Copy"], tvwTOC));

                var rc = base.Commands.AddCommand("Revise", new ReviseCommand(toolbarManager.Tools["Revise"], tvwTOC)) as ReviseCommand;
                rc.ReviseNode += (s, rce) => RevisePart(rce.Item);

                var up = base.Commands.AddCommand("UpdateProcesses", new UpdateProcessesCommand(toolbarManager.Tools["UpdateProcesses"], tvwTOC)) as UpdateProcessesCommand;
                up.UpgradeProcesses += (s, rce) => UpgradeProcesses(rce.Item);

                base.Commands.AddCommand("Print", new PrintNodeCommand(toolbarManager.Tools["Print"], tvwTOC));

                var inactive = base.Commands.AddCommand("DisplayInActive", new DisplayInactiveCommand(toolbarManager.Tools["DisplayInActive"], tvwTOC)) as DisplayInactiveCommand;
                inactive.ToolClicked += TogglePartActiveStatus;

                base.Commands.AddCommand("Search", new SearchCommand(toolbarManager.Tools["Search"], tvwTOC, this));
                base.Commands.AddCommand("CopyToCustomer", new CopyToCommand(toolbarManager.Tools["CopyToCustomer"], tvwTOC, this));
                base.Commands.AddCommand("SaveTemplate", new SaveTemplateCommand(toolbarManager.Tools["SaveTemplate"], tvwTOC, this.dsParts.Media, this));
                base.Commands.AddCommand("ApplyTemplate", new ApplyTemplateCommand(toolbarManager.Tools["ApplyTemplate"], tvwTOC, this));

                base.Commands.AddCommand("TemplateManager", new TemplateManagerCommand(toolbarManager.Tools["TemplateManager"], this));

                base.Commands.AddCommand("CreateFromQuote", new CreatePartFromQuoteCommand(toolbarManager.Tools["CreateFromQuote"], this));
            }

            if (ApplicationSettings.Current.PartMarkingEnabled)
            {
                if (SecurityManager.Current.IsInRole("PartManager.Edit"))
                {
                    var partMarkCmd = Commands.AddCommand("PartMark", new AddPartMarkCommand(toolbarManager.Tools["PartMark"], tvwTOC)) as AddPartMarkCommand;
                    if (partMarkCmd != null)
                    {
                        partMarkCmd.AddPartMark += PartMarkCmdOnAddPartMark;
                    }
                }
            }
            else
            {
                // Hide
                toolbarManager.Tools["PartMark"].SharedProps.Visible = false;
            }
        }

        private PartsDataset.PartRow AddPart(PartsRootNode rn)
        {
            _log.Info("Adding a part.");

            //create new data source
            PartsDataset.PartRow cr = this.dpPartInfo.AddPart(CurrentCustomerID);

            //if there is a value in the filter then set it to the new parts name
            if (!String.IsNullOrEmpty(this.txtNodeFilter.Text))
                cr.Name = this.txtNodeFilter.Text;

            //create new ui nodes
            var cn = new PartNode(cr, ShowInactive);
            rn.Nodes.Add(cn);
            cn.Select();

            return cr;
        }

        /// <summary>
        ///     Revises the part by creating new part based on existing one.
        /// </summary>
        /// <param name="partNode"> The rn. </param>
        private void RevisePart(PartNode partNode)
        {
            _log.Info("Revising a part.");

            //copy and paste to create new part
            CopyCommand.CopyNode(partNode);
            var revisedNode = PasteCommand.PasteNode(tvwTOC.Nodes[0] as ICopyPasteNode) as PartNode; //past to root node

            if (revisedNode != null)
            {
                //set old part as inactive
                partNode.DataRow.Active = false;

                //create new revision and set new part as active
                //revisedNode.DataRow.Revision	= DateTime.Now.ToShortDateString(); -- [05.20.11 Dont set date by default]
                revisedNode.DataRow.Active = true;
                revisedNode.DataRow.ParentID = partNode.DataRow.PartID; //ensure relation is set to track history

                UpgradeProcesses(revisedNode);

                //select old part then re-select to force refresh of UI
                partNode.UpdateNodeUI();
                revisedNode.Select();
            }
        }

        private void UpgradeProcesses(PartNode partNode)
        {
            if (partNode == null)
            {
                return;
            }

            try
            {
                _log.Info("Upgrading a part's processes.");

                var revisedProcesses = FindUpdatedRevisedPartProcesses(partNode);

                if (revisedProcesses.Count > 0)
                {
                    _partsWithUpgradedProcesses.Add(partNode.DataRow.PartID);
                }

                foreach (var process in revisedProcesses)
                {
                    //find part process row to see if we need to replace it
                    PartsDataset.PartProcessRow existingPartProcess = Array.Find(partNode.DataRow.GetPartProcessRows(), p => p == process.Key);
                    if (existingPartProcess != null)
                    {
                        // Create price rows
                        var newPriceRows = new List<PartsDataset.PartProcessVolumePriceRow>();
                        foreach (var existingPrice in existingPartProcess.GetPartProcessVolumePriceRows())
                        {
                            var newPrice = dsParts.PartProcessVolumePrice.NewPartProcessVolumePriceRow();
                            newPrice.Amount = existingPrice.Amount;
                            newPrice.PriceUnit = existingPrice.PriceUnit;

                            if (!existingPrice.IsMinValueNull())
                            {
                                newPrice.MinValue = existingPrice.MinValue;
                            }

                            if (!existingPrice.IsMaxValueNull())
                            {
                                newPrice.MaxValue = existingPrice.MaxValue;
                            }

                            newPriceRows.Add(newPrice);
                        }

                        int stepOrder = existingPartProcess.StepOrder;

                        int? loadCapacityQuantity = null;
                        if (!existingPartProcess.IsLoadCapacityQuantityNull())
                        {
                            loadCapacityQuantity = existingPartProcess.LoadCapacityQuantity;
                        }

                        decimal? loadCapacityWeight = null;
                        if (!existingPartProcess.IsLoadCapacityWeightNull())
                        {
                            loadCapacityWeight = existingPartProcess.LoadCapacityWeight;
                        }

                        // Delete existing part process
                        existingPartProcess.BeginEdit();
                        existingPartProcess.Delete();
                        existingPartProcess.EndEdit();

                        //add new par process relating the part to the new process alias
                        var newPartProcess = this.dsParts.PartProcess.NewPartProcessRow();
                        newPartProcess.PartRow = partNode.DataRow;
                        newPartProcess.ProcessRow = process.Value.ProcessRow;
                        newPartProcess.StepOrder = stepOrder;
                        newPartProcess.ProcessAliasRow = process.Value;

                        if (loadCapacityQuantity.HasValue)
                        {
                            newPartProcess.LoadCapacityQuantity = loadCapacityQuantity.Value;
                        }

                        if (loadCapacityWeight.HasValue)
                        {
                            newPartProcess.LoadCapacityWeight = loadCapacityWeight.Value;
                        }

                        this.dsParts.PartProcess.AddPartProcessRow(newPartProcess);

                        // Add new price rows
                        foreach (var newPrice in newPriceRows)
                        {
                            newPrice.PartProcessRow = newPartProcess;
                            dsParts.PartProcessVolumePrice.AddPartProcessVolumePriceRow(newPrice);
                        }
                    }
                }

                // Show warnings for processes with inactive inspections
                foreach (var partProcess in partNode.DataRow.GetPartProcessRows())
                {
                    var inactiveInspectionCount = taProcess.GetInactiveInspectionCount(partProcess.ProcessID) ?? 0;

                    if (inactiveInspectionCount > 0)
                    {
                        var partProcessName = partProcess.ProcessAliasRow?.Name ?? partProcess.PartProcessID.ToString();

                        MessageBoxUtilities.ShowMessageBoxWarn(
                            $"Process '{partProcessName}' has an inactive inspection.\n" +
                            "Please revise the process in Process Manager.",
                            Text);
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error updating revised part.";
                _log.Error(exc, errorMsg);
            }

            partNode.Select();
        }

        private Dictionary<PartsDataset.PartProcessRow, PartsDataset.ProcessAliasRow> FindUpdatedRevisedPartProcesses(PartNode partNode)
        {
            try
            {
                var revisedProcesses = new Dictionary<PartsDataset.PartProcessRow, PartsDataset.ProcessAliasRow>();

                //get all part processes
                var processes = partNode.DataRow.GetPartProcessRows().Where(p => !p.ProcessRow.Active);

                //for each inactive process get new process
                foreach (var partProcess in processes)
                {
                    PartsDataset.ProcessRow bestFitProcess = null;
                    PartsDataset.ProcessAliasRow bestFitAlias = null;

                    //attempt to find latest revised process
                    using (var taProcess = new ProcessTableAdapter())
                    {
                        object parent = taProcess.Get_RevisedProcess(partProcess.ProcessID);
                        if (parent is int && ((int)parent) > 0)
                        {
                            PartsDataset.ProcessRow found = this.dsParts.Process.FindByProcessID((int)parent);

                            if (found != null && found.ProcessID != partProcess.ProcessID)
                                bestFitProcess = found;
                        }
                    }

                    //if found a revised process, but no alias then find the alias
                    if (bestFitProcess != null && bestFitAlias == null)
                    {
                        //see if any aliases with same name
                        foreach (var alias in bestFitProcess.GetProcessAliasRows())
                        {
                            //if they have the same name
                            if (alias.Name == partProcess.ProcessAliasRow.Name)
                                bestFitAlias = alias;
                        }
                    }

                    //if found alternative then replase it
                    if (bestFitAlias != null)
                        revisedProcesses.Add(partProcess, bestFitAlias);
                }

                return revisedProcesses;
            }
            catch (Exception exc)
            {
                string errorMsg = "Error revising part processes";
                _log.Error(exc, errorMsg);
                return new Dictionary<PartsDataset.PartProcessRow, PartsDataset.ProcessAliasRow>();
            }
        }

        protected override void LoadNode(UltraTreeNode node)
        {
            this.dpPartInfo.CurrentPartUsageCount = -1;

            if (node is PartNode)
            {
                var partNode = (PartNode)node;

                //update part usage count
                if (partNode.PartUsageCount < 0) //if not set then set it now
                {
                    object count = this.taPart.PartUsageCount(partNode.DataRow.PartID);
                    partNode.PartUsageCount = count == null || count == DBNull.Value ? 0 : Convert.ToInt32(count);
                }

                //dynamiclly load children
                partNode.LoadChildrenNodes(this);

                this.dpPartInfo.CurrentPartUsageCount = partNode.PartUsageCount;
                this.dpPartInfo.Editable = partNode.PartUsageCount < 1;

                DisplayPanel(this.dpPartInfo);
                this.dpPartInfo.MoveToRecord(partNode.ID);

                if (this.AutoSyncProcessPrices)
                {
                    this.AutoSyncProcessPrices = false;

                    var result = MessageBoxUtilities.ShowMessageBoxYesOrNo("Would you like to update process prices?",
                        "Update Prices");

                    if (result == DialogResult.Yes)
                    {
                        this.dpPartInfo.UpdateProcessPrices();
                    }
                }
            }
            else if (node is PartMarkNode partMarkNode)
            {
                dpPartMarkInfo.MoveToRecord(partMarkNode.ID);
                DisplayPanel(dpPartMarkInfo);
            }
            else
            {
                DisplayPanel(null);
            }
        }

        protected override void SaveSelectedNode()
        {
            //save selection as customerID|partNodeKey
            if (tvwTOC.SelectedNodes.Count > 0)
                Properties.Settings.Default.LastSelectedPart = CurrentCustomerID + "|" + tvwTOC.SelectedNodes[0].Key;
        }

        private void FilterTree(string partName)
        {
            try
            {
                if (tvwTOC.Nodes.Count == 1)
                {
                    using (new UsingWaitCursor(this))
                    {
                        using (new UsingTimeMe("loading filtered parts."))
                        {
                            using (new UsingTreeLoad(tvwTOC))
                            {
                                if (partName != null)
                                    partName = partName.ToUpper();

                                UltraTreeNode rootNode = tvwTOC.Nodes[0];
                                rootNode.Nodes.Clear();

                                this.taManager.PartTableAdapter.FillByPartName(this.dsParts.Part, CurrentCustomerID, "%" + partName + "%");
                                string filter = this.dsParts.Part.CustomerIDColumn.ColumnName +
                                    " = " +
                                    CurrentCustomerID +
                                    " AND Name LIKE '%" +
                                    DataUtilities.PrepareForRowFilterLike(partName) +
                                    "%'";

                                var rows = this.dsParts.Part.Select(filter, this.dsParts.Part.NameColumn.ColumnName) as PartsDataset.PartRow[];

                                foreach (var pr in rows)
                                {
                                    var showInactive = ShowInactive || !string.IsNullOrEmpty(partName);
                                    rootNode.Nodes.Add(new PartNode(pr, showInactive));
                                }

                                StatusBarPartCount = rows.Length;

                                rootNode.Select();

                                if (!string.IsNullOrEmpty(partName))
                                    this.txtNodeFilter.Appearance.BorderColor = Color.Red;
                                else
                                    this.txtNodeFilter.Appearance.ResetBorderColor();
                            }
                        }
                    }
                }
                else
                    this.txtNodeFilter.Appearance.ResetBorderColor();
            }
            catch (Exception exc)
            {
                string errorMsg = "Error updating in line filter.";
                _log.Error(exc, errorMsg);
            }
        }

        private void TogglePartActiveStatus()
        {
            try
            {
                //redo filter since reload will clear it
                var filter = this.txtNodeFilter.Text;
                var currentCustomerId = this.CurrentCustomerID;

                ReloadTOC();

                if (ShowInactive)
                {
                    _loadingData = true;

                    this.taCustomer.Fill(this.dsParts.Customer);
                    DataUtilities.LoadComboBox(CustomerFilter, this.dsParts.Customer, this.dsParts.Customer.CustomerIDColumn.ColumnName, this.dsParts.Customer.NameColumn.ColumnName);
                    this.CurrentCustomerID = currentCustomerId;

                    _loadingData = false;
                }

                this.txtNodeFilter.Text = filter;

                if (!String.IsNullOrWhiteSpace(this.txtNodeFilter.Text))
                    FilterTree(this.txtNodeFilter.Text);
            }
            catch (Exception exc)
            {
                string errorMsg = "Error updating visibility of inactive parts.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        protected override void OnDispose()
        {
            try
            {
                this.dsParts.PartProcess.PartProcessRowDeleting -= PartProcess_PartProcessRowDeleting;
            }
            catch (Exception exc)
            {
                string errorMsg = "Error on Part Manager form close.";
                _log.Error(exc, errorMsg);
            }

            base.OnDispose();
        }

        internal void GoToPart(int partID)
        {
            var partNode = tvwTOC.Nodes.FindNode<PartNode>(pn => pn.DataRow.PartID == partID);

            if (partNode != null)
                partNode.Select();
        }

        private PartsDataset.PartRow AddPart()
        {
            try
            {
                if (IsValidControls())
                {
                    _validators.Enabled = false;

                    PartsDataset.PartRow partRow = null;
                    var root = tvwTOC.Nodes[0] as PartsRootNode;
                    if (root != null)
                        partRow = AddPart(root);

                    _validators.Enabled = true;

                    return partRow;
                }

                return null;
            }
            catch (Exception exc)
            {
                string errorMsg = "Error adding part.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
                return null;
            }
        }

        public PartsDataset.PartRow CreatePartFromQuote(Data.Datasets.QuoteDataSet.QuotePartSearchRow qpSearchRow, int? quotePartId = null)
        {
            PartsDataset.PartRow newPart = null;
            var qpId = quotePartId ?? qpSearchRow.QuotePartID;
            if (qpId > 0)
            {
                _log.Info("Adding a part from a quote.");

                newPart = this.dpPartInfo.AddPartFromQuote(qpId);
                this.CurrentCustomerID = newPart.CustomerID; //forces refresh, if diff customer then will load this node

                //Check to see if we need to revise any parts
                var oldPart = this.tvwTOC.Nodes[0].Nodes.FindNode<PartNode>(n => n.IsRowValid && n.DataRow.Active && n.DataRow.Name == newPart.Name && n.DataRow.PartID != newPart.PartID);
                if (oldPart != null)
                {
                    var result = MessageBoxUtilities.ShowMessageBoxYesOrNo(
                        $"A part with the name {newPart.Name} already exists for {newPart.CustomerRow.Name}. " +
                        $"Would you like to revise the existing part and set the imported part as the active part?",
                        "Existing Part Conflict");
                    if (result == DialogResult.Yes)
                    {
                        //Set our revision properties.
                        oldPart.DataRow.Active = false;
                        newPart.ParentID = oldPart.DataRow.PartID;
                    }

                }

                //find the node
                var node = this.tvwTOC.Nodes[0].Nodes.FindNode<PartNode>(n => n.IsRowValid && n.DataRow.PartID == newPart.PartID);
                if (node == null)
                {
                    node = new PartNode(newPart, this.ShowInactive);
                    this.tvwTOC.Nodes[0].Nodes.Add(node);
                }

                node.Select();
            }

            return newPart;
        }

        private void AddPartMarking()
        {
            if (!IsValidControls())
            {
                return;
            }

            _validators.Enabled = false;
            try
            {
                var selectedNode = tvwTOC.SelectedNodes.OfType<PartNode>().FirstOrDefault();

                if (selectedNode == null || !selectedNode.IsRowValid || selectedNode.DataRow.GetPart_PartMarkingRows().Length > 0)
                {
                    return;
                }

                _log.Info("Adding a new part mark.");

                var newPartMark = dsParts.Part_PartMarking.NewPart_PartMarkingRow();
                newPartMark.PartRow = selectedNode.DataRow;
                newPartMark.ProcessSpec = "<DEFAULT>";
                dsParts.Part_PartMarking.AddPart_PartMarkingRow(newPartMark);

                var markNode = new PartMarkNode(newPartMark);
                selectedNode.Nodes.Add(markNode);
                markNode.Select();
            }
            finally
            {
                _validators.Enabled = true;
            }
        }

        #endregion

        #region Events

        private void Part_Load(object sender, EventArgs e)
        {
            _loadingData = true; //prevent TOC from loading

            try
            {
                using (new UsingWaitCursor())
                {
                    if (DesignMode)
                        return;

                    SuspendLayout();

                    LoadCommands();
                    this.dpPartInfo.BringToFront();

                    Application.DoEvents();

                    LoadData();
                    LoadValidators();

                    CustomerFilter.ToolValueChanged += CustomerFilter_ToolValueChanged;

                    _loadingData = false;
                    splitContainer1.Panel2.Enabled = SecurityManager.Current.IsInRole("PartManager.Edit");

                    //if searching within a specified customer than set that now and prevent user from changing customer
                    if (this._customerFilterID >= 0)
                    {
                        CustomerFilter.SharedProps.Enabled = false;
                        CurrentCustomerID = this._customerFilterID; //change currently selected customer to get data to load

                        //if unable to select customer then customer must be InActive
                        if (CurrentCustomerID != _customerFilterID)
                        {
                            //get the inactive customer and refresh the combo box list
                            taCustomer.FillByCustomer(this.dsParts.Customer, _customerFilterID);
                            DataUtilities.LoadComboBox(CustomerFilter, this.dsParts.Customer, this.dsParts.Customer.CustomerIDColumn.ColumnName, this.dsParts.Customer.NameColumn.ColumnName);
                            CurrentCustomerID = this._customerFilterID;
                        }

                        if (this._lastSelectedPartID > 0)
                        {
                            string key = PartNode.CreateKey(PartNode.KEY_PREFIX, this._lastSelectedPartID.ToString());
                            var pn = tvwTOC.GetNodeByKey(key) as PartNode;

                            if (pn == null)
                            {
                                bool? isActive = this.taPart.IsPartActive(this._lastSelectedPartID);
                                if (isActive.HasValue && !isActive.Value)
                                {
                                    //show inactive, then reset customer id filter as showInActive resets it
                                    ShowInactive = true;
                                    this.CurrentCustomerID = this.CustomerFilterID;

                                    pn = tvwTOC.GetNodeByKey(key) as PartNode;
                                }
                            }

                            if (pn != null)
                                pn.Select();
                        }
                        else if (!String.IsNullOrEmpty(InitialPartFilterText))
                        {
                            this.txtNodeFilter.Text = InitialPartFilterText;
                            FilterTree(this.txtNodeFilter.Text);
                        }
                    }
                    else if (this._lastSelectedPartID > 0) //else if just wanting to navigate to a part by default
                    {
                        int custID = this.taPart.GetCustomerID(this._lastSelectedPartID).GetValueOrDefault(0);
                        if (custID > 0)
                        {
                            CurrentCustomerID = custID; //change currently selected customer to get data to load

                            string key = PartNode.CreateKey(PartNode.KEY_PREFIX, this._lastSelectedPartID.ToString());
                            var pn = tvwTOC.GetNodeByKey(key) as PartNode;

                            if (pn != null)
                                pn.Select();
                        }
                    }
                    else if (!string.IsNullOrEmpty(Properties.Settings.Default.LastSelectedPart))
                    {
                        //renew last selection
                        string[] ids = Properties.Settings.Default.LastSelectedPart.Split('|');
                        if (ids.Length == 2)
                        {
                            int customerID;
                            if (Int32.TryParse(ids[0], out customerID))
                            {
                                CurrentCustomerID = customerID;
                                base.RestoreLastSelectedNode(ids[1]);
                            }
                        }
                    }
                    else if (CustomerFilter.ValueList.ValueListItems.Count > 0)
                        CustomerFilter.SelectedIndex = 0;

                    //if nothing was loaded in the TOC then reselect customer
                    if (tvwTOC.Nodes.Count == 0)
                    {
                        CustomerFilter.ResetSelectedIndex();
                        if (CustomerFilter.ValueList.ValueListItems.Count > 0)
                            CustomerFilter.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception exc)
            {
                _log.Warn("DataSet Errors: " + this.dsParts.GetDataErrors());
                splitContainer1.Enabled = false;
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
            }
            finally
            {
                ResumeLayout();
            }
        }

        private void PartProcess_PartProcessRowDeleting(object sender, PartsDataset.PartProcessRowChangeEvent e)
        {
            try
            {
                //Must ensure
                this.taProcessAnswer.ClearBeforeFill = false;

                DataRow[] rowss = this.dsParts.PartProcessAnswer.Select(this.dsParts.PartProcessAnswer.PartProcessIDColumn.ColumnName + " = " + e.Row.PartProcessID, null, DataViewRowState.CurrentRows);
                if (rowss.Length == 0)
                {
                    this.taProcessAnswer.FillBy(this.dsParts.PartProcessAnswer, e.Row.PartProcessID);
                    rowss = this.dsParts.PartProcessAnswer.Select(this.dsParts.PartProcessAnswer.PartProcessIDColumn.ColumnName + " = " + e.Row.PartProcessID);
                }

                foreach (var ppaRow in rowss)
                    ppaRow.Delete();
            }
            catch (Exception exc)
            {
                string errorMsg = "Error deleting process row.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        protected override void btnOK_Click(object sender, EventArgs e)
        {
            PartsDataset.PartRow currentRow = null;
            this._lastSelectedPartID = -1;

            try
            {
                if (tvwTOC.SelectedNodes.Count == 1 && tvwTOC.SelectedNodes[0] is PartNode)
                    currentRow = ((PartNode)tvwTOC.SelectedNodes[0]).DataRow;

                if (IsValidControls())
                {
                    if (SaveData())
                    {
                        //check after save to get updated part id
                        if (currentRow != null)
                        {
                            this._lastSelectedPartID = currentRow.PartID;

                            //if not allowed to select an inactive part
                            if (!AllowInActiveSelectedPart && !currentRow.Active)
                            {
                                MessageBoxUtilities.ShowMessageBoxOKCancel("An inactive part is not allowed to be selected. Please select an active part.", "Inactive Part Selected");
                                DialogResult = DialogResult.None;
                                return;
                            }
                        }

                        SaveSelectedNode();
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error closing form.", exc);
                _log.Fatal(exc, "Error closing form.");
            }
        }

        private void CustomerFilter_ToolValueChanged(object sender, ToolEventArgs e)
        {
            try
            {
                using (new UsingWaitCursor(this))
                {
                    if (!_loadingData && CustomerFilter.SelectedItem != null)
                    {
                        Refresh();
                        ReloadTOC();
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error after customer list value changed.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void AddNode(object sender, EventArgs e) { AddPart(); }

        private void txtNodeFilter_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                if (!_loadingData)
                {
                    if (e.Button.Key == "GO")
                        FilterTree(this.txtNodeFilter.Text);

                    else if (e.Button.Key == "Reset")
                    {
                        this.txtNodeFilter.Text = null;
                        FilterTree(null);
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error updating filter.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void txtNodeFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!_loadingData)
                {
                    if (e.KeyChar == '\r')
                    {
                        e.Handled = true;
                        FilterTree(this.txtNodeFilter.Text);
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error updating filter.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void PartMarkCmdOnAddPartMark(object sender, EventArgs eventArgs)
        {
            try
            {
                AddPartMarking();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error adding part marking to part");
            }
        }

        #endregion

        #region Nodes

        #region Nested type: IHistoryNode

        private interface IHistoryNode
        {
            IItemHistoryLoader GetHistoryLoader();
        }

        #endregion

        #region Nested type: PartNode

        private class PartNode : DataNode<PartsDataset.PartRow>, IHistoryNode, IReportNode
        {
            #region Fields

            private static Image _partImage;

            #endregion

            #region Properties

            public const string KEY_PREFIX = "PA";
            public int PartUsageCount = -1;

            public bool IsDataLoaded { get; set; }

            #endregion

            #region Methods

            public PartNode(PartsDataset.PartRow cr, bool showInActive) : base(cr, cr.PartID.ToString(), KEY_PREFIX, cr.Name)
            {
                if (_partImage == null)
                    _partImage = Properties.Resources.Part_16;

                LeftImages.Add(_partImage);

                UpdateVisibility(showInActive);
                UpdateNodeUI();
            }

            public override bool CanDelete
            {
                get
                {
                    //can't delete if this part is used by orders
                    return this.PartUsageCount < 1;
                }
            }

            public override string ClipboardDataFormat
            {
                get { return GetType().FullName; }
            }

            internal void LoadChildrenNodes(PartManager pm)
            {
                if (!IsDataLoaded)
                {
                    using (new UsingTimeMe("Loading Nodes for part " + DataRow.PartID))
                    {
                        pm.taManager.MediaTableAdapter.FillByPartIDWithoutMedia(pm.dsParts.Media, DataRow.PartID);
                        pm.taManager.Part_MediaTableAdapter.FillByPartID(pm.dsParts.Part_Media, DataRow.PartID);
                        pm.taManager.PartProcessTableAdapter.FillByPart(pm.dsParts.PartProcess, DataRow.PartID);
                        pm.taManager.PartProcessVolumePriceTableAdapter.FillByPartID(pm.dsParts.PartProcessVolumePrice, DataRow.PartID);
                        pm.taManager.PartAreaTableAdapter.FillByPartID(pm.dsParts.PartArea, DataRow.PartID);
                        pm.taManager.PartAreaDimensionTableAdapter.FillByPartID(pm.dsParts.PartAreaDimension, DataRow.PartID);
                        pm.taManager.Part_DocumentLinkTableAdapter.FillByPartID(pm.dsParts.Part_DocumentLink, DataRow.PartID);
                        pm.taManager.Part_PartMarkingTableAdapter.FillByPartID(pm.dsParts.Part_PartMarking, DataRow.PartID);

                        foreach (var partMark in DataRow.GetPart_PartMarkingRows())
                        {
                            Nodes.Add(new PartMarkNode(partMark));
                        }

                        pm.taManager.PartCustomFieldsTableAdapter.FillByPartID(pm.dsParts.PartCustomFields, DataRow.PartID);

                        IsDataLoaded = true;
                    }
                }
            }

            public override void UpdateNodeUI()
            {
                if (base.DataRow == null)
                    return;

                Text = base.DataRow.Name;

                if (base.DataRow.Active)
                {
                    Override.NodeAppearance.FontData.Italic = DefaultableBoolean.False;
                    Override.NodeAppearance.ForeColor = Color.Black;
                }
                else
                {
                    Override.NodeAppearance.FontData.Italic = DefaultableBoolean.True;
                    Override.NodeAppearance.ForeColor = Color.Salmon;
                }
            }

            /// <summary>
            ///     Updates the visibility of the node based on whether or not is should show inactive nodes.
            /// </summary>
            /// <param name="showInActive"> if set to <c>true</c> [show in active]. </param>
            public void UpdateVisibility(bool showInActive) { Visible = base.DataRow.Active || showInActive; }

            #endregion

            #region IHistoryNode Members

            public IItemHistoryLoader GetHistoryLoader() { return new PartHistoryLoader(DataRow.PartID); }

            #endregion

            #region IReportNode Members

            public IReport CreateReport(string reportType)
            {
                try
                {
                    return new PartSummaryReport(DataRow);
                }
                catch (Exception exc)
                {
                    string errorMsg = "Error displaying report.";
                    LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
                    return null;
                }
            }

            public string[] ReportTypes() { return new[] { "Part Summary Report" }; }

            #endregion
        }

        #endregion

        #region Nested type: PartMarkNode

        private class PartMarkNode : DataNode<PartsDataset.Part_PartMarkingRow>
        {
            private const string KEY_PREFIX = "PM";
            public const string TITLE = "Part Marking";

            public PartMarkNode(PartsDataset.Part_PartMarkingRow cr) :
                base(cr, cr.Part_PartMarkingID.ToString(), KEY_PREFIX, TITLE)
            {
                LeftImages.Add(Properties.Resources.Tag_16);
            }
        }

        #endregion

        #region Nested type: PartsRootNode

        private class PartsRootNode : UltraTreeNode, ICopyPasteNode
        {
            #region Fields

            private PartManager _parts;

            #endregion

            #region Methods

            public PartsRootNode(PartManager parts) : base("ROOT", "Parts")
            {
                LeftImages.Add(Properties.Resources.Part_16);
                this._parts = parts;
            }

            public override void Dispose()
            {
                this._parts = null;
                base.Dispose();
            }

            #endregion

            #region ICopyPasteNode Members

            public UltraTreeNode PasteData(string format, DataRowProxy proxy)
            {
                if (proxy == null)
                {
                    return null;
                }

                if (_parts == null)
                {
                    LogManager.GetCurrentClassLogger().Warn("Node was disposed - skipping paste");
                    return null;
                }

                var relationsToDelete = new HashSet<string>
                {
                    // Manually copied later
                    "FK_Part_Media_Part",

                    // Manually copied later because PartID is part a primary key
                    "FK_PartCustomFields_Part",
                };

                //remove any media relations
                var deleteProxies = new List<DataRowProxy>();
                foreach (var proxyChild in proxy.ChildProxies)
                {
                    if (relationsToDelete.Contains(proxyChild.ParentRelation))
                    {
                        deleteProxies.Add(proxyChild);
                    }
                }

                //delete removed items from proxy
                foreach (var delete in deleteProxies)
                    proxy.ChildProxies.Remove(delete);

                //copy node and update customer id
                var dr = DataNode<DataRow>.AddPastedDataRows(proxy, this._parts.dsParts.Part) as PartsDataset.PartRow;
                dr.CustomerID = this._parts.CurrentCustomerID;
                var copiedPart = new PartNode(dr, true);
                base.Nodes.Add(copiedPart);

                try
                {
                    // Tie back relations after the parts PartID has been updated to -XX.
                    foreach (var delete in deleteProxies)
                    {
                        if (delete.ParentRelation == "FK_Part_Media_Part")
                        {
                            int mediaID = Convert.ToInt32(delete.ItemArray[1]);
                            bool defaultMedia = Convert.ToBoolean(delete.ItemArray[2]);
                            PartsDataset.MediaRow mr = this._parts.dsParts.Media.FindByMediaID(mediaID);
                            this._parts.dsParts.Part_Media.AddPart_MediaRow(dr, mr, defaultMedia);
                        }
                        else if (delete.ParentRelation == "FK_PartCustomFields_Part")
                        {
                            var partCustomFieldRow = _parts.dsParts.PartCustomFields.NewPartCustomFieldsRow();
                            partCustomFieldRow.PartRow = dr;
                            partCustomFieldRow.PartLevelCustomFieldID = Convert.ToInt32(
                                delete.ItemArray[_parts.dsParts.PartCustomFields.PartLevelCustomFieldIDColumn.Ordinal]);

                            partCustomFieldRow.Value = Convert.ToString(
                                delete.ItemArray[_parts.dsParts.PartCustomFields.ValueColumn.Ordinal]);

                            _parts.dsParts.PartCustomFields.AddPartCustomFieldsRow(partCustomFieldRow);
                        }
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error copying rows.");
                }

                copiedPart.Control.ActiveNode = copiedPart;
                copiedPart.Select();

                return copiedPart;
            }

            public bool CanPasteData(string format) { return format == typeof(PartNode).FullName; }

            public string ClipboardDataFormat
            {
                get { return null; }
            }

            #endregion
        }

        #endregion

        #endregion

        #region Commands

        #region Nested type: AddCommand

        private class AddCommand : TreeNodeCommandBase
        {
            #region Fields

            public EventHandler AddNode;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return true; }
            }

            #endregion

            #region Methods

            public AddCommand(ToolBase tool, UltraTree toc) : base(tool) { base.TreeView = toc; }

            public override void OnClick()
            {
                if (this.AddNode != null)
                    this.AddNode(this, EventArgs.Empty);
            }

            #endregion
        }

        #endregion

        #region Nested type: AddPartMarkCommand

        private class AddPartMarkCommand : TreeNodeCommandBase
        {
            #region Fields

            public event EventHandler AddPartMark;

            #endregion

            #region Properties

            public override bool Enabled => _node is PartNode partNode &&
                                            partNode.IsRowValid &&
                                            partNode.DataRow.GetPart_PartMarkingRows().Length == 0;

            #endregion

            #region Methods

            public AddPartMarkCommand(ToolBase tool, UltraTree toc) : base(tool)
            {
                base.TreeView = toc;
            }

            public override void OnClick()
            {
                AddPartMark?.Invoke(this, EventArgs.Empty);
            }

            #endregion
        }

        #endregion

        #region Nested type: DisplayInactiveCommand

        private class DisplayInactiveCommand : TreeNodeCommandBase
        {
            #region Fields

            public Action ToolClicked;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.TreeView != null && base.TreeView.Nodes.Count > 0; }
            }

            #endregion

            #region Methods

            public DisplayInactiveCommand(ToolBase tool, UltraTree toc) : base(tool) { base.TreeView = toc; }

            public override void OnClick()
            {
                try
                {
                    if(this.ToolClicked != null)
                        this.ToolClicked();
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error updating visibility of nodes.");
                }
            }

            public override void Dispose()
            {
                this.ToolClicked = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region Nested type: HistoryCommand

        private class HistoryCommand : TreeNodeCommandBase
        {
            #region Properties

            public override bool Enabled => _node is PartNode;

            #endregion

            #region Methods

            public HistoryCommand(ToolBase tool, UltraTree toc) : base(tool) { TreeView = toc; }

            public override void OnClick()
            {
                try
                {
                    if(_node is PartNode partNode)
                    {
                        using(var frm = new PartHistory())
                        {
                            frm.PartId = partNode.DataRow.PartID;
                            frm.PartName = partNode.DataRow.Name;
                            frm.ShowDialog(ActiveForm);
                        }
                    }
                }
                catch(Exception exc)
                {
                    string errorMsg = "Error loading history viewer.";
                    _log.Error(exc, errorMsg);
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: ReviseCommand

        private class ReviseCommand : TreeNodeCommandBase
        {
            #region Fields

            public event EventHandlerTemplate <object, PartNode> ReviseNode;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _node is PartNode; }
            }

            #endregion

            #region Methods

            public ReviseCommand(ToolBase tool, UltraTree toc) : base(tool) { base.TreeView = toc; }

            public override void OnClick()
            {
                //check to see if part should be revised
                if(_node is PartNode)
                {
                    if(((PartNode) _node).PartUsageCount < 1)
                    {
                        if(MessageBoxUtilities.ShowMessageBoxYesOrNo("The current part is not being used by any order, are you sure you want to revise it?", "Revise Part") == DialogResult.No)
                            return;
                    }
                }

                UltraTreeNode currentNode = base._node;

                //test to see if we can change selection
                UltraTreeNode root = base.TreeView.Nodes[0];
                root.Select();

                if(base.TreeView.SelectedNodes.Count == 1 && base.TreeView.SelectedNodes[0] == root)
                {
                    base.OnAfterSelect(currentNode);

                    if(Enabled)
                    {
                        if(ReviseNode != null && _node is PartNode)
                            ReviseNode(this, new EventArgsTemplate <PartNode>(_node as PartNode));
                    }
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: SearchCommand

        private class SearchCommand : TreeNodeCommandBase
        {
            #region Fields

            private PartManager _partManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.TreeView != null && base.TreeView.Nodes.Count > 0; }
            }

            #endregion

            #region Methods

            public SearchCommand(ToolBase tool, UltraTree toc, PartManager partManager) : base(tool)
            {
                base.TreeView = toc;
                this._partManager = partManager;
            }

            public override void OnClick()
            {
                try
                {
                    using(var frm = new PartSearch())
                    {
                        if(frm.ShowDialog(this._partManager) == DialogResult.OK)
                        {
                            var selectedPart = frm.SelectedPart;
                            
                            if (selectedPart != null)
                            {
                                if (!selectedPart.Active)
                                    this._partManager.ShowInactive = selectedPart.Active;

                                this._partManager.CurrentCustomerID = this._partManager.taPart.GetCustomerID(selectedPart.PartID).Value;
                                this._partManager.GoToPart(selectedPart.PartID);
                            }
                        }
                    }
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error updating visibility of nodes.");
                }
            }

            public override void Dispose()
            {
                this._partManager = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region Nested type: CopyToCommand

        private class CopyToCommand : TreeNodeCommandBase
        {
            #region Fields

            private PartManager _partManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _node is PartNode; }
            }

            #endregion

            #region Methods

            public CopyToCommand(ToolBase tool, UltraTree toc, PartManager partManager) : base(tool)
            {
                base.TreeView = toc;
                this._partManager = partManager;
            }

            public override void OnClick()
            {
                try
                {
                    var partNode = _node as PartNode;

                    using(var cbo = new ComboBoxForm())
                    {
                        cbo.Text = "Copy To Customer";
                        cbo.FormLabel.Text = "Customer:";
                        cbo.ComboBox.DropDownStyle = DropDownStyle.DropDownList;

                        cbo.ComboBox.ValueMember = this._partManager.dsParts.Customer.CustomerIDColumn.ColumnName;
                        cbo.ComboBox.DisplayMember = this._partManager.dsParts.Customer.NameColumn.ColumnName;
                        cbo.ComboBox.DataSource = this._partManager.dsParts.Customer.OrderBy(c => c.Name).ToList();
                        cbo.ComboBox.DataBind();

                        if(cbo.ComboBox.Items.Count > 0)
                            cbo.ComboBox.SelectedIndex = 0;

                        if(cbo.ShowDialog(this._partManager) == DialogResult.OK && cbo.ComboBox.SelectedItem != null)
                        {
                            TreeView.SelectedNodes.Clear(); // ends edits for the part to copy

                            var partProxy = CopyCommand.CopyRows(partNode.DataRow);
                            var partType = partNode.ClipboardDataFormat;

                            if(partProxy != null)
                            {
                                this._partManager.CurrentCustomerID = Convert.ToInt32(cbo.ComboBox.SelectedItem.DataValue);

                                var root = this._partManager.tvwTOC.Nodes[0] as PartsRootNode;
                                var newPart = root.PasteData(partType, partProxy) as PartNode;
                                this._partManager.GoToPart(newPart.DataRow.PartID);
                            }
                        }
                    }
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error updating visibility of nodes.");
                }
            }

            public override void Dispose()
            {
                this._partManager = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion

        #region Nested type: ReviseCommand

        private class UpdateProcessesCommand : TreeNodeCommandBase
        {
            #region Fields

            public event EventHandlerTemplate <object, PartNode> UpgradeProcesses;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _node is PartNode; }
            }

            #endregion

            #region Methods

            public UpdateProcessesCommand(ToolBase tool, UltraTree toc) : base(tool) { base.TreeView = toc; }

            public override void OnClick()
            {
                UltraTreeNode currentNode = base._node;

                //test to see if we can change selection
                UltraTreeNode root = base.TreeView.Nodes[0];
                root.Select();

                if(base.TreeView.SelectedNodes.Count == 1 && base.TreeView.SelectedNodes[0] == root)
                {
                    base.OnAfterSelect(currentNode);

                    if(Enabled)
                    {
                        if(UpgradeProcesses != null && _node is PartNode)
                            UpgradeProcesses(this, new EventArgsTemplate <PartNode>(_node as PartNode));
                    }
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: SaveTemplateCommand

        private class SaveTemplateCommand : TreeNodeCommandBase
        {
            #region Fields

            private PartsDataset.MediaDataTable _mediaTable;
            private PartManager _partManager;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.TreeView.SelectedNodesOfType <PartNode>().Count > 0; }
            }

            #endregion

            #region Methods

            public SaveTemplateCommand(ToolBase tool, UltraTree toc, PartsDataset.MediaDataTable media, PartManager partManager) : base(tool)
            {
                base.TreeView = toc;
                this._mediaTable = media;
                this._partManager = partManager;
            }

            public override void OnClick()
            {
                try
                {
                    var selectedNodes = base.TreeView.SelectedNodes;

                    if(selectedNodes.Count > 0)
                    {
                        var templates = new List<PartsDataset.MediaRow>();
                        foreach (var node in selectedNodes.OfType<PartNode>())
                        {
                            // Save a new template for the part, but only if it does not have unsaved media.
                            if (node.DataRow.GetPart_MediaRows().Any(m => m.MediaID < 0))
                            {
                                MessageBoxUtilities.ShowMessageBoxWarn(
                                    $"Please click Apply before creating a template from  '{node.DataRow.Name}'.",
                                    "Part Manager");
                            }
                            else
                            {
                                templates.Add(SaveTemplate(node));
                            }
                        }

                        var updateCount = _partManager.taManager.MediaTableAdapter.Update2(templates);

                        if (updateCount > 0)
                        {
                            MessageBoxUtilities.ShowMessageBoxOK(
                                $"{updateCount} part {(updateCount != 1 ? "templates have" : "template has")} been saved successfully.",
                                updateCount != 1 ? "Templates Saved" : "Template Saved");
                        }
                    }
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error saving part template.");
                }
            }

            private PartsDataset.MediaRow SaveTemplate(PartNode part)
            {
                try
                {
                    if (part == null)
                    {
                        return null;
                    }

                    var template = PartTemplate.CreateTemplate(part.DataRow);

                    //add to media table
                    var bytes = PartTemplate.SaveTemplate(template);
                    return _mediaTable.AddMediaRow(part.DataRow.Name, part.DataRow.Name, TemplateManager.PART_TEMPLATE_FILE_TYPE, bytes);
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error saving part template.");
                    return null;
                }
            }

            public override void Dispose()
            {
                this._partManager = null;
                this._mediaTable = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion Nested type: SaveTemplateCommand

        #region Nested type: ApplyTemplateCommand

        private class ApplyTemplateCommand : TreeNodeCommandBase
        {
            #region Fields

            private const string PART_TEMPLATE_FILE_TYPE = "PartTemp";

            private PartManager _partManager;
            private MediaTableAdapter _taMedia;
            private ProcessTableAdapter _taProcess;
            private ProcessAliasTableAdapter _taProcessAlias;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get { return true; }
            }

            private MediaTableAdapter MediaAdapter
            {
                get { return this._taMedia ?? (this._taMedia = new MediaTableAdapter()); }
            }

            private ProcessAliasTableAdapter ProcessAliasAdapter
            {
                get { return this._taProcessAlias ?? (this._taProcessAlias = new ProcessAliasTableAdapter()); }
            }

            private ProcessTableAdapter ProcessAdapter
            {
                get { return this._taProcess ?? (this._taProcess = new ProcessTableAdapter()); }
            }

            #endregion

            #region Methods

            public ApplyTemplateCommand(ToolBase tool, UltraTree toc, PartManager partManager) : base(tool)
            {
                base.TreeView = toc;
                this._partManager = partManager;

                //load all part templates from media table
                MediaAdapter.ClearBeforeFill = false;
                MediaAdapter.FillByFileTypeWithoutMedia(this._partManager.dsParts.Media, PART_TEMPLATE_FILE_TYPE);

                ProcessAliasAdapter.ClearBeforeFill = false;
                ProcessAliasAdapter.Fill(this._partManager.dsParts.ProcessAlias);

                ProcessAdapter.ClearBeforeFill = false;
                ProcessAdapter.Fill(this._partManager.dsParts.Process);
            }

            public override void OnClick()
            {
                try
                {
                    if(this._partManager.IsValidControls())
                    {
                        using(var templateManager = new TemplateManager())
                        {
                            //if there is a value in the filter then set it to the new parts name
                            if(!String.IsNullOrEmpty(this._partManager.txtNodeFilter.Text))
                                templateManager.FilterText = this._partManager.txtNodeFilter.Text;

                            if(templateManager.ShowDialog() == DialogResult.OK && templateManager.SelectedTemplate != null)
                            {
                                CreatePart(templateManager.SelectedTemplate);

                                // Reload the node to ensure all fields are populated
                                this._partManager.LoadNode(this._partManager.tvwTOC.SelectedNodes[0]);
                            }
                        }
                    }
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error applying part template.");
                }
            }

            private void CreatePart(PartTemplate template)
            {
                //create new data source
                var partRow = this._partManager.AddPart();
                partRow.Name = template.Name;
                partRow.AssemblyNumber = template.Assembly;
                partRow.Revision = template.Revision;
                partRow.CustomerID = this._partManager.CurrentCustomerID;
                partRow.Active = template.IsActive;
                partRow.ManufacturerID = template.Manufacturer;
                partRow.Airframe = template.Model;
                partRow.ShapeType = template.Shape;
                partRow.SurfaceArea = template.SurfaceArea;
                partRow.Length = template.Length;
                partRow.Width = template.Width;
                partRow.Height = template.Height;
                partRow.LotPrice = template.LotPrice;
                partRow.EachPrice = template.EachPrice;
                partRow.Notes = template.Notes;

                //ensure exists
                var materialRow = this._partManager.dsParts.d_Material.FindByMaterialID(template.Material);
                if(materialRow != null)
                    partRow.Material = materialRow.MaterialID;

                partRow.PartMarking = template.PartMarking;

                // Set the partShapeWidget values
                this._partManager.dpPartInfo.LoadPartShapeWidget(partRow);

                AddMedia(partRow, template.Media);
                AddProcess(partRow, template.ProcessAliasIDs);
            }

            /// <summary>
            ///     Adds the process.
            /// </summary>
            /// <param name="partRow">The part row.</param>
            /// <param name="processAliasIDs">The process alias ids.</param>
            private void AddProcess(PartsDataset.PartRow partRow, List <int> processAliasIDs)
            {
                try
                {
                    if(partRow != null)
                    {
                        foreach(var processAliasID in processAliasIDs)
                        {
                            var processAliasRow = this._partManager.dsParts.ProcessAlias.FirstOrDefault(p => p.ProcessAliasID == processAliasID);

                            if(processAliasRow != null)
                            {
                                var stepOrder = this._partManager.dpPartInfo.GetNextProcessStepOrder(partRow);

                                var newPartProcess = this._partManager.dsParts.PartProcess.NewPartProcessRow();
                                newPartProcess.PartRow = partRow;
                                newPartProcess.ProcessRow = processAliasRow.ProcessRow;
                                newPartProcess.StepOrder = stepOrder;
                                newPartProcess.ProcessAliasRow = processAliasRow;

                                this._partManager.dsParts.PartProcess.AddPartProcessRow(newPartProcess);
                            }
                        }
                    }
                }
                catch(Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error adding the process to the part.", exc);
                }
            }

            /// <summary>
            ///     Adds the media.
            /// </summary>
            /// <param name="partRow">The part row.</param>
            /// <param name="partMedia">The part media.</param>
            private void AddMedia(PartsDataset.PartRow partRow, List <PartTemplate.MediaTemplate> partMedia)
            {
                foreach(var media in partMedia)
                {
                    var mediaRow = this._partManager.dsParts.Media.FindByMediaID(media.MediaId);

                    if(mediaRow == null)
                    {
                        MediaAdapter.FillByMediaIdWithoutMedia(this._partManager.dsParts.Media, media.MediaId);
                        mediaRow = this._partManager.dsParts.Media.FindByMediaID(media.MediaId);
                    }

                    if (mediaRow != null)
                    {
                        // Add media to part
                        this._partManager.dsParts.Part_Media.AddPart_MediaRow(partRow, mediaRow, media.IsDefault);
                    }
                    else
                    {
                        _log.Warn($"Skipped media #{media.MediaId} when copying from template to part.");
                    }
                }
            }

            public override void Dispose()
            {
                this._partManager = null;
                this._taMedia = null;
                this._taProcessAlias = null;
                this._taProcess = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion Nested type: ApplyTemplateCommand

        #region Nested type: TemplateManagerCommand

        private class TemplateManagerCommand : CommandBase
        {
            #region Fields

            private PartManager _partManager;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get { return true; }
            }

            #endregion

            #region Methods

            public TemplateManagerCommand(ToolBase tool, PartManager partManager) : base(tool)
            {
                this._partManager = partManager;
                Refresh();
            }

            public override void OnClick()
            {
                try
                {
                    if(this._partManager.IsValidControls())
                    {
                        using(var templateManager = new TemplateManager {IsManagerMode = true})
                            templateManager.ShowDialog();
                    }
                }
                catch(Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error applying part template.");
                }
            }

            public override void Dispose()
            {
                this._partManager = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion Nested type: TemplateManagerCommand

        #region Nested type: CreatePartFromQuoteCommand

        private class CreatePartFromQuoteCommand : CommandBase
        {
            #region Fields

            private PartManager _partManager;

            private int _QuotePartID;

            #endregion Fields

            #region Properties

            public override bool Enabled
            {
                get { return true; }
            }

            #endregion

            #region Methods

            public CreatePartFromQuoteCommand(ToolBase tool, PartManager partManager)
                : base(tool)
            {
                this._partManager = partManager;
                Refresh();
            }

            public void CreatePartFromQuotePart()
            {
                if (_QuotePartID > 0)
                {
                    _log.Info("Adding a part from a quote.");

                    var newPart = _partManager.dpPartInfo.AddPartFromQuote(_QuotePartID);
                    _partManager.CurrentCustomerID = newPart.CustomerID; //forces refresh, if diff customer then will load this node

                    //Check to see if we need to revise any parts
                    var oldPart = _partManager.tvwTOC.Nodes[0].Nodes.FindNode<PartNode>(n => n.IsRowValid && n.DataRow.Active && n.DataRow.Name == newPart.Name && n.DataRow.PartID != newPart.PartID);
                    if (oldPart != null)
                    {
                        var result = MessageBoxUtilities.ShowMessageBoxYesOrNo(
                            $"A part with the name {newPart.Name} already exists for {newPart.CustomerRow.Name}. " +
                            $"Would you like to revise the existing part and set the imported part as the active part?",
                            "Existing Part Conflict");
                        if (result == DialogResult.Yes)
                        {
                            //Set our revision properties.
                            oldPart.DataRow.Active = false;
                            newPart.ParentID = oldPart.DataRow.PartID;
                        }

                    }

                    //find the node
                    var node = _partManager.tvwTOC.Nodes[0].Nodes.FindNode<PartNode>(n => n.IsRowValid && n.DataRow.Active && n.DataRow.PartID == newPart.PartID);
                    if (node == null)
                    {
                        node = new PartNode(newPart, _partManager.ShowInactive);
                        _partManager.tvwTOC.Nodes[0].Nodes.Add(node);
                    }

                    node.Select();


                }

            }

            public override void OnClick()
            {
                try
                {
                    if (this._partManager.IsValidControls())
                    {
                        using(var quoteSearch = new QuoteSearch())
                        {
                            if(quoteSearch.ShowDialog() == DialogResult.OK)
                            {
                                if(quoteSearch.SelectedQuotePart != null)
                                {

                                    _QuotePartID = quoteSearch.SelectedQuotePart.QuotePartID;
                                    CreatePartFromQuotePart();
  
                                }
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error creating a part from a quote.");
                }
            }

            public override void Dispose()
            {
                this._partManager = null;
                base.Dispose();
            }

            #endregion
        }

        #endregion Nested type: CreatePartFromQuoteCommand

        #endregion

        #region PartLoaded

        /// <summary>
        ///     Represents information about parts that were loaded by customer and active and inactive.
        /// </summary>
        private class PartLoaded
        {
            public int CustomerID { get; set; }
            public bool ActiveLoaded { get; set; }
            public bool InActiveLoaded { get; set; }
        }

        #endregion

        #region Nested type: PartManagerNodeSorter

        public class PartManagerNodeSorter : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                if(x is PartNode && y is PartNode)
                {
                    PartsDataset.PartRow partX = ((PartNode) x).DataRow;
                    PartsDataset.PartRow partY = ((PartNode) y).DataRow;

                    //else compare the last modified dates if there names are the same
                    if(partX.Name == partY.Name && !partX.IsLastModifiedNull() && !partY.IsLastModifiedNull())
                        return partY.LastModified.CompareTo(partX.LastModified);

                    //else just compare ASC
                    return partX.Name.CompareTo(partY.Name);
                }
                    //else if (x is ISortByDate && y is ISortByDate)
                    //    return ((ISortByDate)x).SortByDate.GetValueOrDefault().CompareTo(((ISortByDate)y).SortByDate.GetValueOrDefault());
                if(x is UltraTreeNode && y is UltraTreeNode)
                    return ((UltraTreeNode) x).Text.CompareTo(((UltraTreeNode) y).Text);
                return 0;
            }

            #endregion
        }

        #endregion
    }
}