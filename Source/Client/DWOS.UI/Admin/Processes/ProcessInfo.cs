using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ProcessesDatasetTableAdapters;
using DWOS.Shared;
using DWOS.UI.Documents;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using DWOS.Utilities.Validation;
using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using RestSharp.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Interop;
using Infragistics.Win.UltraWinGrid;

namespace DWOS.UI.Admin.Processes
{
    public partial class ProcessInfo : DataPanel
    {
        #region Fields

        private const string STATUS_APPROVED = "Approved";
        private const string STATUS_PLANNED = "Planned";
        private const string STATUS_CLOSED = "Closed";

        private BindingSource _procAliasBS;
        private BindingSource _customerProcAliasBS;
        private ProcessDocumentLinkTableAdapter _taProcessDocumentLink;
        private ProcessAliasTableAdapter _taProcessAlias;
        private ProcessAliasDocumentLinkTableAdapter _taAliasDocumentLink;
        private ProcessStepsTableAdapter _taProcessSteps;
        private bool _isDisposing;
        private DWOS.Utilities.Validation.ValidatorManager _aliasValidators = new DWOS.Utilities.Validation.ValidatorManager();
        private UltraToolbarsManager _toolbarManager;
        private DWOS.Utilities.Validation.ValidatorManager _manager;
        private HashSet<int> _loadedProcesses = new HashSet<int>();
        private readonly BindingList<ProcessSuggestionItem> _suggestions =
            new BindingList<ProcessSuggestionItem>();

        private DisplayDisabledTooltips _displayDisabledTooltipsGeneral;

        #endregion

        #region Properties

        public ProcessesDataset Dataset
        {
            get { return base._dataset as ProcessesDataset; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return this.Dataset.Process.ProcessIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public ProcessInfo()
        {
            this.InitializeComponent();
            _displayDisabledTooltipsGeneral = new DisplayDisabledTooltips(ultraTabPageControl1, tipManager);

            // default value of $0 is invalid when there is no unit of measure selected
            curMaterialCost.ValueObject = null;
            curMaterialCost.ValueChanged += curMaterialCost_ValueChanged;

            // same goes for lead time and its type
            numLeadTime.Value = null;
            numLeadTime.ValueChanged += numLeadTime_ValueChanged;
        }

        public void LoadData(ProcessesDataset dataset, ProcessStepsTableAdapter taProcessSteps)
        {
            this.Dataset = dataset;
            this._taProcessSteps = taProcessSteps;
            bsData.DataSource = this.Dataset;
            bsData.DataMember = this.Dataset.Process.TableName;

            //bind column to control
            base.BindValue(this.txtProcessName, this.Dataset.Process.NameColumn.ColumnName);
            base.BindValue(this.txtProcessDesc, this.Dataset.Process.DescriptionColumn.ColumnName);
            base.BindValue(this.cboProcessRev, this.Dataset.Process.RevisionColumn.ColumnName, true);

            base.BindValue(this.dtModDate, this.Dataset.Process.ModifiedDateColumn.ColumnName);
            base.BindValue(this.cboProcessStepDept, this.Dataset.Process.DepartmentColumn.ColumnName, true); //CR594
            base.BindValue(this.cboProcessCategory, this.Dataset.Process.CategoryColumn.ColumnName);
            base.BindValue(this.txtShortCode, this.Dataset.Process.ProcessCodeColumn.ColumnName);
            base.BindValue(this.optPaperless, this.Dataset.Process.IsPaperlessColumn.ColumnName);

            base.BindValue(this.txtFrozenBy, this.Dataset.Process.FrozenByColumn.ColumnName);
            base.BindValue(this.txtFrozenDate, this.Dataset.Process.FrozenDateColumn.ColumnName);
            base.BindValue(this.numProcessMinPrice, this.Dataset.Process.MinPriceColumn.ColumnName);
            base.BindValue(this.numProcessPrice, this.Dataset.Process.PriceColumn.ColumnName);
            base.BindValue(this.curMaterialCost, this.Dataset.Process.MaterialUnitCostColumn.ColumnName);
            base.BindValue(this.numLeadTime, this.Dataset.Process.LeadTimeHoursColumn.ColumnName);
            base.BindValue(this.curBurdenRate, this.Dataset.Process.BurdenCostRateColumn.ColumnName);

            base.BindValue(this.numLoadCapacity, this.Dataset.Process.LoadCapacityColumn.ColumnName);
            base.BindValue(this.cboLoadCapacityType, this.Dataset.Process.LoadCapacityTypeColumn.ColumnName);


            // Creating drop down for material unit cost
            var materialCostDropDown = new UltraComboEditor()
            {
                DropDownStyle = DropDownStyle.DropDownList
            };

            materialCostDropDown.Items.Add("lb", "per lb.");
            materialCostDropDown.Items.Add("in²", "per in²");
            materialCostDropDown.ValueChanged += MaterialCostDropDown_ValueChanged;
            (this.curMaterialCost.ButtonsRight[0] as DropDownEditorButton).Control = materialCostDropDown;
            base.BindValue(materialCostDropDown, this.Dataset.Process.MaterialUnitColumn.ColumnName, DataSourceUpdateMode.OnPropertyChanged);

            // Creating drop down for lead time
            var leadTimeDropDown = new UltraComboEditor()
            {
                DropDownStyle = DropDownStyle.DropDownList
            };

            leadTimeDropDown.Items.Add("load", "per load");
            leadTimeDropDown.Items.Add("piece", "per piece");
            leadTimeDropDown.ValueChanged += LeadTimeDropDown_ValueChanged;
            (numLeadTime.ButtonsRight[0] as DropDownEditorButton).Control = leadTimeDropDown;
            BindValue(leadTimeDropDown, Dataset.Process.LeadTimeTypeColumn.ColumnName, DataSourceUpdateMode.OnPropertyChanged);

            //bind lists
            base.BindList(this.cboProcessRev, this.Dataset.Revisions, this.Dataset.Revisions.RevisionColumn.ColumnName, this.Dataset.Revisions.RevisionColumn.ColumnName);
            base.BindList(this.cboProcessStepDept, this.Dataset.d_Department, this.Dataset.d_Department.DepartmentIDColumn.ColumnName, this.Dataset.d_Department.DepartmentIDColumn.ColumnName);
            base.BindList(this.cboProcessCategory, this.Dataset.d_ProcessCategory, this.Dataset.d_ProcessCategory.ProcessCategoryColumn.ColumnName, this.Dataset.d_ProcessCategory.ProcessCategoryColumn.ColumnName);

            var currencyInputMask = "{currency:4." + ApplicationSettings.Current.PriceDecimalPlaces + "}";
            this.numProcessMinPrice.MaskInput = currencyInputMask;
            this.numProcessPrice.MaskInput = currencyInputMask;
            this.curMaterialCost.MaskInput = currencyInputMask;
            this.curBurdenRate.MaskInput = currencyInputMask;

            // --------  Process Aliases  ---------------
            this._procAliasBS = new BindingSource();
            this._procAliasBS.DataSource = this.Dataset;
            this._procAliasBS.DataMember = this.Dataset.ProcessAlias.TableName;
            this._procAliasBS.Filter = "1 = 0";

            txtProcessAliasName.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(txtProcessAliasName), this._procAliasBS, this.Dataset.ProcessAlias.NameColumn.ColumnName, true));
            txtPopupNote.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(txtPopupNote), this._procAliasBS, this.Dataset.ProcessAlias.PopUpNotesColumn.ColumnName, true));
            txtTravelerNote.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(txtTravelerNote), this._procAliasBS, this.Dataset.ProcessAlias.TravelerNotesColumn.ColumnName, true));

            txtProcessAliasName.MaxLength = this.Dataset.ProcessAlias.NameColumn.MaxLength;
            txtPopupNote.MaxLength = this.Dataset.ProcessAlias.PopUpNotesColumn.MaxLength;
            txtTravelerNote.MaxLength = this.Dataset.ProcessAlias.TravelerNotesColumn.MaxLength;

            // --------  Customer Process Aliases  ---------------
            this._customerProcAliasBS = new BindingSource();
            this._customerProcAliasBS.DataSource = this.Dataset;
            this._customerProcAliasBS.DataMember = this.Dataset.CustomerProcessAlias.TableName;
            this._customerProcAliasBS.Filter = "1 = 0";

            txtCustomerProcessAliasName.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(txtCustomerProcessAliasName), this._customerProcAliasBS, this.Dataset.CustomerProcessAlias.NameColumn.ColumnName, true));
            txtCustomerProcessAliasName.MaxLength = this.Dataset.CustomerProcessAlias.NameColumn.MaxLength;

            cboCustomer.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(cboCustomer), this._customerProcAliasBS, this.Dataset.CustomerProcessAlias.CustomerIDColumn.ColumnName, true));

            cboCustomer.DataSource = this.Dataset.Customer.DefaultView;
            cboCustomer.DisplayMember = this.Dataset.Customer.NameColumn.ColumnName;
            cboCustomer.ValueMember = this.Dataset.Customer.CustomerIDColumn.ColumnName;

            pnlCustomerProcessAlias.Dock = DockStyle.Fill;
            pnlProcessAlias.Dock = DockStyle.Fill;

            docLinkManagerProcessAlias.InitializeData(LinkType.ProcessAlias, this.Dataset.ProcessAlias, this.Dataset.ProcessAliasDocumentLink);
            docLinkManagerProcess.InitializeData(LinkType.Process, this.Dataset.Process, this.Dataset.ProcessDocumentLink);
            base._panelLoaded = true;

            //Time Constraints
            bsConstraints.Filter = "0 = 1"; //initial do not load anything
            bsConstraints.DataSource = this.Dataset.ProcessRequisite;
            grdConstraints.DataSource = bsConstraints;

            // Load capacity
            pnlLoadCapacity.Visible = ApplicationSettings.Current.UseLoadCapacity;
            UpdateLoadCapacity();

            // Process suggestions
            grdSuggestions.DataSource = _suggestions;

            // Product classes
            if (!FieldUtilities.IsFieldEnabled("Order", "Product Class"))
            {
                btnProcessProductClass.Visible = false;
            }

            // Lead time
            if (!ApplicationSettings.Current.UsingLeadTimeScheduling)
            {
                lblLeadTime.Visible = false;
                numLeadTime.Visible = false;
            }
        }

        internal void LoadCommand(CommandManager cmdManager, UltraToolbarsManager toolbarManager)
        {
            _toolbarManager = toolbarManager;
            cmdManager.AddCommand("btnAddCustomerProcessAlias", new AddCustomerProcessAliasCommand(toolbarManager.Tools["AddCustomerAlias"], tvwProcessAliases, this));
            var dc = cmdManager.AddCommand("btnDeleteCustomerAlias", new DeleteCommand(toolbarManager.Tools["DeleteAlias"], tvwProcessAliases, null)) as DeleteCommand;
            cmdManager.AddCommand("btnAddProcessAlias", new AddProcessAliasCommand(toolbarManager.Tools["AddProcessAlias"], tvwProcessAliases, this));

            dc.CancelCommand += (di) =>
            {
                //if deleting a process alias node
                if (di.Any(n => n is ProcessAliasNode))
                {
                    //if selected the remaining steps
                    var aliases = di.OfType<ProcessAliasNode>().ToList();
                    if (aliases[0].Parent.Nodes.Count <= aliases.Count)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn("Process must contain at least one alias.", "Delete Process");
                        return true;
                    }
                }

                return false;
            };
            dc.ItemDeleted += (s, e) => UpdateAliasTabText();

            cmdManager.AddCommand("AddConstraint", new AddConstraintCommand(toolbarManager.Tools["AddConstraint"], this));
            cmdManager.AddCommand("DeleteConstraint", new DeleteConstraintCommand(toolbarManager.Tools["DeleteConstraint"], this));

            base.Commands.AddCommand("btnAddInspection", new AddInspectionCommand(this.btnAddInspection, this.lvwInspections, this));
            base.Commands.AddCommand("DeleteProcess", new DeleteInspectionCommand(this.btnRemoveInspection, this.lvwInspections, this));
            base.Commands.AddCommand("MoveProcessStepDownCommand", new MoveInspectionStepDownCommand(this.btnInspectionDown, this.lvwInspections, this));
            base.Commands.AddCommand("MoveProcessStepUpCommand", new MoveInspectionStepUpCommand(this.btnInspectionUp, this.lvwInspections, this));
            base.Commands.AddCommand("CheckInspectionCommand", new CheckInspectionCommand(this.btnInspectionCheck, this.lvwInspections, this));

            Commands.AddCommand("btnAddSuggestion", new AddSuggestionCommand(btnAddSuggestion, this));
            Commands.AddCommand("btnDeleteSuggestion", new DeleteSuggestionCommand(btnDeleteSuggestion, this));
            Commands.AddCommand("btnEditSuggestion", new EditSuggestionCommand(btnEditSuggestion, this));
            Commands.AddCommand("btnImportSuggestions", new ImportSuggestionsCommand(btnImportSuggestions, this));
        }

        public override void AddValidators(DWOS.Utilities.Validation.ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtProcessName, "Process name required.") { DefaultValue = "Process XXX" }, errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtProcessDesc, "Process description required."), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboProcessRev, "Process Revision required."), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboProcessStepDept, "Process Department required.") { PreserveWhitespace = true }, errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtCustomerProcessAliasName, "Customer alias name required."), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtProcessAliasName, "Process alias name required."), errProvider));
            manager.Add(new ImageDisplayValidator(new DepartmentValidator(this.cboProcessStepDept, this), errProvider));
            manager.Add(new ImageDisplayValidator(new MaterialCostValidator(this.curMaterialCost) { IsRequired = false }, errProvider));
            manager.Add(new ImageDisplayValidator(new LeadTimeValidator(numLeadTime) { IsRequired = false }, errProvider));

            _aliasValidators.Add(new ImageDisplayValidator(new TextControlValidator(this.txtCustomerProcessAliasName, "Customer alias name required."), errProvider) { Tab = tabProcessInfo.Tabs["Aliases"] });
            _aliasValidators.Add(new ImageDisplayValidator(new TextControlValidator(this.txtProcessAliasName, "Process alias name required."), errProvider) { Tab = tabProcessInfo.Tabs["Aliases"] });

            var tab = tabProcessInfo.Tabs[1];
            manager.Add(new TooltipDisplayValidator(tipManager, null) { RequiredStyleSetName = null, InvalidStyleSetName = null, Validator = new ProcessAliasesValiditor(tab, this.tvwProcessAliases, this) { ErrorMessage = "You must have at least 1 alias defined for each processes." } });
            _manager = manager;
        }

        public ProcessesDataset.ProcessRow AddProcessRow()
        {
            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as ProcessesDataset.ProcessRow;
            cr.Name = "Process XXX";
            cr.ModifiedDate = DateTime.Now;
            cr.Revision = "<None>";
            cr.Department = ApplicationSettings.Current.DepartmentSales;
            cr.d_ProcessCategoryRow = this.Dataset.d_ProcessCategory.FirstOrDefault();
            cr.IsPaperless = true;
            cr.IsApproved = false;

            return cr;
        }

        public void Freeze(string frozenBy)
        {
            var process = this.CurrentRecord as ProcessesDataset.ProcessRow;

            if (process != null)
            {
                process.Frozen = true;
                process.FrozenDate = DateTime.Now;
                process.FrozenBy = frozenBy;

                UpdateFrozenImage(true);

                //lock the panel down to prevent changes
                this.Editable = false;
            }
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            base.BeforeMoveToNewRecord(id);
            curMaterialCost.ValueChanged -= curMaterialCost_ValueChanged;
            numLeadTime.ValueChanged -= numLeadTime_ValueChanged;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            try
            {
                base.AfterMovedToNewRecord(id);

                tabProcessInfo.SelectedTab = tabProcessInfo.Tabs[0];

                LoadProcessAliases();

                docLinkManagerProcess.ClearLinks();

                var process = this.CurrentRecord as ProcessesDataset.ProcessRow;

                if (process != null)
                {
                    if (this._taProcessDocumentLink == null)
                    {
                        this._taProcessDocumentLink = new ProcessDocumentLinkTableAdapter()
                        {
                            ClearBeforeFill = false
                        };
                    }

                    if (this._taAliasDocumentLink == null)
                    {
                        this._taAliasDocumentLink = new ProcessAliasDocumentLinkTableAdapter()
                        {
                            ClearBeforeFill = false
                        };
                    }

                    if (!_loadedProcesses.Contains(process.ProcessID))
                    {
                        _taProcessDocumentLink.FillByProcess(Dataset.ProcessDocumentLink,
                            process.ProcessID);

                        _taAliasDocumentLink.FillByProcess(Dataset.ProcessAliasDocumentLink,
                            process.ProcessID);

                        _loadedProcesses.Add(process.ProcessID);
                    }

                    docLinkManagerProcess.LoadLinks(process, process.GetProcessDocumentLinkRows());
                }

                UpdateFrozenImage(process != null && process.Frozen);

                //Load data for this node
                LoadInspections(this.CurrentRecord as Data.Datasets.ProcessesDataset.ProcessRow);

                //Load only requisites for this process id
                bsConstraints.Filter = "ParentProcessID = " + id;

                UpdateConstraintsTabText();

                cboStatus.Value = GetProcessStatusString(process);

                SetLoadCapacityVariance(process);

                RefreshMaterialCostText();
                RefreshLeadTimeText();

                UpdateRequisitesList(process);
                UpdateSuggestions(process);
                UpdateProductClassText(process);
            }
            finally
            {
                curMaterialCost.ValueChanged += curMaterialCost_ValueChanged;
                numLeadTime.ValueChanged += numLeadTime_ValueChanged;
            }
        }

        private void LoadProcessAliases()
        {
            tvwProcessAliases.Nodes.Clear();

            if (CurrentRecord != null)
            {
                var processRow = this.CurrentRecord as ProcessesDataset.ProcessRow;

                if (processRow != null)
                {
                    var processAliases = processRow.GetProcessAliasRows();

                    var rootNode = new ProcessAliasRootNode();
                    tvwProcessAliases.Nodes.Add(rootNode);

                    foreach (var par in processAliases)
                    {
                        var processAliaseNode = new ProcessAliasNode(par);
                        rootNode.Nodes.Add(processAliaseNode);

                        foreach (var cpa in par.GetCustomerProcessAliasRows())
                        {
                            processAliaseNode.Nodes.Add(new CustomerProcessAliasNode(cpa));
                        }
                    };

                    rootNode.ExpandAll();
                    rootNode.Select();
                    LoadNode(rootNode);
                }
            }

            UpdateAliasTabText();
        }

        private void LoadNode(UltraTreeNode node)
        {
            pnlCustomerProcessAlias.Enabled = false;
            pnlProcessAlias.Enabled = false;

            pnlCustomerProcessAlias.Visible = false;
            pnlProcessAlias.Visible = false;

            if (node is ProcessAliasNode)
            {
                docLinkManagerProcessAlias.ClearLinks();

                var paNode = node as ProcessAliasNode;

                if (!paNode.UsageCount.HasValue)
                {
                    if (this._taProcessAlias == null)
                    {
                        this._taProcessAlias = new ProcessAliasTableAdapter();
                    }

                    paNode.UsageCount = Convert.ToInt32(this._taProcessAlias.GetAliasUsageCount(paNode.DataRow.ProcessAliasID));
                }

                docLinkManagerProcessAlias.LoadLinks(paNode.DataRow, paNode.DataRow.GetProcessAliasDocumentLinkRows());

                this._procAliasBS.Filter = this.Dataset.ProcessAlias.ProcessAliasIDColumn.ColumnName + " = " + paNode.DataRow.ProcessAliasID;
                pnlProcessAlias.Enabled = true;
                pnlProcessAlias.Visible = true;
            }
            else if (node is CustomerProcessAliasNode)
            {
                var cpaNode = node as CustomerProcessAliasNode;
                this._customerProcAliasBS.Filter = this.Dataset.CustomerProcessAlias.CustomerProcessAliasIDColumn.ColumnName + " = " + cpaNode.DataRow.CustomerProcessAliasID;
                pnlCustomerProcessAlias.Enabled = true;
                pnlCustomerProcessAlias.Visible = true;
            }
        }

        private void EndProcessAliasEdit()
        {
            _customerProcAliasBS.EndEdit();

            if (this._customerProcAliasBS.Current is DataRowView && ((DataRowView)this._customerProcAliasBS.Current).IsEdit)
                ((DataRowView)this._customerProcAliasBS.Current).EndEdit();

            _procAliasBS.EndEdit();

            if (this._procAliasBS.Current is DataRowView && ((DataRowView)this._procAliasBS.Current).IsEdit)
                ((DataRowView)this._procAliasBS.Current).EndEdit();
        }

        private void AddProcessAlias()
        {
            if (CurrentRecord is ProcessesDataset.ProcessRow)
            {
                if (this.IsValidControls())
                {
                    var processRow = this.CurrentRecord as ProcessesDataset.ProcessRow;

                    if (processRow.Name != null)
                    {
                        var par = this.Dataset.ProcessAlias.AddProcessAliasRow(processRow.Name, processRow, null, null);

                        var processAliaseNode = new ProcessAliasNode(par);
                        tvwProcessAliases.Nodes[0].Nodes.Add(processAliaseNode);
                        processAliaseNode.Select();
                        LoadNode(processAliaseNode);
                    }
                }
            }

            UpdateAliasTabText();
        }

        private void AddCustomerProcessAlias()
        {
            var paNode = tvwProcessAliases.SelectedNode<ProcessAliasNode>();

            if (paNode != null && this.IsValidControls())
            {
                var cpar = this.Dataset.CustomerProcessAlias.AddCustomerProcessAliasRow(paNode.DataRow, this.Dataset.Customer[0], paNode.DataRow.Name);

                var customerProcessAliasNode = new CustomerProcessAliasNode(cpar);
                paNode.Nodes.Add(customerProcessAliasNode);
                customerProcessAliasNode.Select();
                LoadNode(customerProcessAliasNode);
            }
        }

        private void AddProcessConstraint(int processId = 0)
        {
            if (CurrentRecord is ProcessesDataset.ProcessRow)
            {
                var processRow = this.CurrentRecord as ProcessesDataset.ProcessRow;
                var childRow = processRow;

                if (processId > 0)
                {
                    var foundRow = this.Dataset.Process.FindByProcessID(processId);
                    if (foundRow != null)
                        childRow = foundRow;
                }

                var contraintRow = this.Dataset.ProcessRequisite.AddProcessRequisiteRow(processRow, childRow, 1);
            }
        }

        public override void EndEditing()
        {
            EndProcessAliasEdit();

            bsConstraints.EndEdit();

            base.EndEditing();
        }

        private bool IsValidControls()
        {
            bool success = true;

            if (this._isDisposing)
                return false;

            if (this.tvwProcessAliases.SelectedNodes.Count > 0 && this.tvwProcessAliases.SelectedNodes[0].Control != null)
            {
                success = this._aliasValidators.ValidateControls();
            }

            return success;
        }

        private void DisposeMe()
        {
            if (this.DesignMode)
                return;

            if (_aliasValidators != null)
            {
                _aliasValidators.Dispose();
                _aliasValidators = null;
            }

            _procAliasBS = null;
            _customerProcAliasBS = null;
            _taProcessDocumentLink = null;
            _taProcessAlias = null;
            _taAliasDocumentLink = null;
            _toolbarManager = null;
            _displayDisabledTooltipsGeneral?.Dispose();
        }

        private void LoadInspections(Data.Datasets.ProcessesDataset.ProcessRow process)
        {
            lvwInspections.Items.Clear();

            foreach (var processInspectionsRow in process.GetProcessInspectionsRows())
            {
                lvwInspections.Items.Add(new InspectionItem(processInspectionsRow));
            }

            UpdateInspectionsTabText();

        }

        private void ResynchInspectionStepOrder()
        {
            List<InspectionItem> processes = this.lvwInspections.Items.OfType<InspectionItem>().OrderBy(pi => pi.StepOrder).ToList();
            int index = 1;

            foreach (var InspectionItem in processes)
            {
                InspectionItem.StepOrder = index;
                index++;
            }

            this.lvwInspections.Items.RefreshSort(true);
        }

        private void UpdateAliasTabText()
        {
            int count = 0;
            tvwProcessAliases.Nodes.ForEachNode(un =>
                                                {
                                                    if (un is ProcessAliasNode)
                                                        count++;
                                                });
            tabProcessInfo.Tabs["Aliases"].Text = "Aliases (" + count + ")";
        }

        private void UpdateInspectionsTabText()
        {
            tabProcessInfo.Tabs["Inspections"].Text = "Inspections (" + lvwInspections.Items.Count + ")";
        }

        private void UpdateConstraintsTabText()
        {
            var constraintCount = grdConstraints.Rows.Count;
            tabProcessInfo.Tabs["Time"].Text = string.Format("Constraints ({0})", constraintCount);
        }

        private void UpdateSuggestionsTab()
        {
            var suggestionsTab = tabProcessInfo.Tabs["Suggestions"];
            suggestionsTab.Visible = ApplicationSettings.Current.UseProcessSuggestions;
            suggestionsTab.Text = $"Suggestions ({grdSuggestions.Rows.Count})";
        }

        private void AddProcessCategories()
        {
            try
            {
                var categoriesInUse = new List<string>();

                categoriesInUse.AddRange(Dataset.Process
                    .Where(proc => proc.IsValidState())
                    .Select(proc => proc.Category)
                    .Distinct());

                var currentCategory = this.cboProcessCategory.Value?.ToString();

                if (!string.IsNullOrEmpty(currentCategory))
                {
                    categoriesInUse.Add(currentCategory);
                }

                using (var settings = new Schedule.ProcessLeadTimeSettings())
                {
                    settings.DoNotAllowDeletionOf(categoriesInUse);
                    settings.ShowDialog();
                }

                using (var taCategory = new Data.Datasets.ProcessesDatasetTableAdapters.d_ProcessCategoryTableAdapter())
                    taCategory.Fill(this.Dataset.d_ProcessCategory);

                //Reselect the process since the reload reset the change
                cboProcessCategory.DataBindings[0].ReadValue();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error editing process categories.");
            }
        }

        private void UpdateFrozenImage(bool isFrozen)
        {
            picFrozen.Image = isFrozen ? imagelistLocks.Images["Lock"] : imagelistLocks.Images["Unlock"];
        }

        private static string GetProcessStatusString(ProcessesDataset.ProcessRow process)
        {
            string dropdownValue;
            if(process == null)
            {
                dropdownValue = STATUS_CLOSED;
            }
            else
            {
                bool isActive = process.Active;
                bool isApproved = !process.IsIsApprovedNull() && process.IsApproved;

                if(isActive && isApproved)
                {
                    dropdownValue = STATUS_APPROVED;
                }
                else
                {
                    dropdownValue = isActive ? STATUS_PLANNED : STATUS_CLOSED;
                }
            }
            return dropdownValue;
        }

        private void SetLoadCapacityVariance(ProcessesDataset.ProcessRow process)
        {
            try
            {
                numLoadCapacityVariance.ValueChanged -= numLoadCapacityVariance_ValueChanged;

                if (process == null || process.IsLoadCapacityVarianceNull())
                {
                    numLoadCapacityVariance.Value = null;
                }
                else
                {
                    // Convert to percent representation
                    numLoadCapacityVariance.Value = process.LoadCapacityVariance * 100M;
                }
            }
            finally
            {
                numLoadCapacityVariance.ValueChanged += numLoadCapacityVariance_ValueChanged;
            }
        }

        private void UpdateLoadCapacity()
        {
            const string quantityCapacityType = "QUANTITY";
            const decimal maxWeight = 9999.99999999M;

            string capacityType = (cboLoadCapacityType.Value?.ToString() ?? string.Empty).ToUpperInvariant();

            string maskInput;
            if (capacityType == quantityCapacityType)
            {
                maskInput = "nnn,nnn,nnn";
            }
            else // Weight or default
            {
                maskInput = string.Format("nnnn.{0} lbs",
                    string.Concat(Enumerable.Repeat("n", ApplicationSettings.Current.WeightDecimalPlaces)));
            }

            numLoadCapacity.MaskInput = maskInput;

            var process = CurrentRecord as ProcessesDataset.ProcessRow;

            if (process != null && !process.IsLoadCapacityNull())
            {
                var currentCapacity = process.LoadCapacity;
                if (capacityType != quantityCapacityType && currentCapacity > maxWeight)
                {
                    process.LoadCapacity = maxWeight;
                    numLoadCapacity.DataBindings[0].ReadValue();
                }
            }
        }

        private void RefreshMaterialCostText()
        {
            var dropDownEditorButton = curMaterialCost.ButtonsRight[0] as DropDownEditorButton;
            var dropDown = dropDownEditorButton.Control as UltraComboEditor;
            var currentProcess = CurrentRecord as ProcessesDataset.ProcessRow;

            string displayText;
            if (dropDown.Visible)
            {
                // dropDownEditorButton.Control has a more recent value
                // than currentProcess.MaterialUnit.
                displayText = dropDown.SelectedItem.DisplayText;
            }
            else if (currentProcess != null)
            {
                // Control text is empty due to how binding works w/ non-visible controls.
                var materialUnitValue = currentProcess.IsMaterialUnitNull() ?
                    string.Empty :
                    currentProcess.MaterialUnit;

                var itemForValue = dropDown.FindItemByValue<string>(v => v == materialUnitValue);
                displayText = itemForValue?.DisplayText;
            }
            else
            {
                displayText = null;
            }

            dropDownEditorButton.Text = displayText;
        }

        private void RefreshLeadTimeText()
        {
            var dropDownEditorButton = numLeadTime.ButtonsRight[0] as DropDownEditorButton;
            var dropDown = dropDownEditorButton.Control as UltraComboEditor;
            var currentProcess = CurrentRecord as ProcessesDataset.ProcessRow;

            string displayText;
            if (dropDown.Visible)
            {
                // dropDownEditorButton.Control has a more recent value
                // than currentProcess.LeadTimeType.
                displayText = dropDown.SelectedItem.DisplayText;
            }
            else if (currentProcess != null)
            {
                // Control text is empty due to how binding works w/ non-visible controls.
                var leadTimeTypeValue = currentProcess.IsLeadTimeTypeNull() ?
                    string.Empty :
                    currentProcess.LeadTimeType;

                var itemForValue = dropDown.FindItemByValue<string>(v => v == leadTimeTypeValue);
                displayText = itemForValue?.DisplayText;
            }
            else
            {
                displayText = null;
            }

            dropDownEditorButton.Text = displayText;
        }

        private void UpdateRequisitesList(ProcessesDataset.ProcessRow parentProcess)
        {
            var childProcesses = parentProcess.GetProcessRequisiteRowsByFK_ProcessRequisite_ProcessParent().ToList();

            var processColumn = grdConstraints.DisplayLayout.Bands[0].Columns["ChildProcessID"];
            var valueList = new ValueList();

            foreach (var process in Dataset.Process.Where(p => p.IsValidState()))
            {
                // Originally, the value list would only contain Active
                // processes because it was populated when Process Manager
                // initially loaded.
                if (process.Active || childProcesses.Any(p => p.ChildProcessID == process.ProcessID))
                {
                    valueList.ValueListItems.Add(process.ProcessID, process.Name);
                }
            }

            valueList.SortStyle = ValueListSortStyle.Ascending;
            processColumn.ValueList = valueList;
            processColumn.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Disallow;
        }

        private void UpdateSuggestions(ProcessesDataset.ProcessRow process)
        {
            _suggestions.Clear();

            if (process == null)
            {
                return;
            }

            var suggestions = process
                .GetProcessSuggestionRowsByFK_ProcessSuggestion_Process_Primary()
                .OrderBy(p => p.ProcessRowByFK_ProcessSuggestion_Process_Suggested?.Name);

            foreach (var suggestionRow in suggestions)
            {
                _suggestions.Add(new ProcessSuggestionItem(suggestionRow));
            }

            UpdateSuggestionsTab();
        }

        private void AddSuggestion(ProcessesDataset.ProcessRow process, ProcessesDataset.ProcessAliasRow processAlias, string suggestionType, string conditionType, string conditionOperator, string conditionValue)
        {
            if (!(CurrentRecord is ProcessesDataset.ProcessRow currentProcess) || process == null || processAlias == null)
            {
                return;
            }

            var processSuggestion = Dataset.ProcessSuggestion.NewProcessSuggestionRow();
            processSuggestion.PrimaryProcessID = currentProcess.ProcessID;
            processSuggestion.SuggestedProcessID = process.ProcessID;
            processSuggestion.SuggestedProcessAliasID = processAlias.ProcessAliasID;
            processSuggestion.Type = suggestionType;

            if (!string.IsNullOrWhiteSpace(conditionType))
            {
                processSuggestion.ConditionType = conditionType;
            }

            if (!string.IsNullOrWhiteSpace(conditionOperator))
            {
                processSuggestion.ConditionOperator = conditionOperator;
            }

            if (!string.IsNullOrWhiteSpace(conditionValue))
            {
                processSuggestion.ConditionValue = conditionValue;
            }

            Dataset.ProcessSuggestion.AddProcessSuggestionRow(processSuggestion);
            _suggestions.Add(new ProcessSuggestionItem(processSuggestion));

            UpdateSuggestionsTab();
        }

        private List<string> GetManufacturers()
        {
            using (var dtManufacturer = new PartsDataset.d_ManufacturerDataTable())
            {
                using (var taManufacturer = new Data.Datasets.PartsDatasetTableAdapters.d_ManufacturerTableAdapter())
                {
                    taManufacturer.Fill(dtManufacturer);
                }

                return dtManufacturer
                    .Select(manufacturer => manufacturer.ManufacturerID)
                    .ToList();
            }
        }

        private void UpdateProductClassText(ProcessesDataset.ProcessRow processRow)
        {
            var processProductClasses = processRow?.GetProcessProductClassRows();
            if (processRow == null)
            {
                btnProcessProductClass.Text = "Product Classes...";
            }
            else
            {
                btnProcessProductClass.Text = $"Product Classes ({processProductClasses.Length})...";
            }
        }

        #endregion

        #region Events

        private void tvwProcessAliases_BeforeSelect(object sender, BeforeSelectEventArgs e)
        {
            try
            {
                //if a node is selected then validate it before moving to the next node
                if (this.tvwProcessAliases.SelectedNodes.Count > 0)
                {
                    if (!this.IsValidControls())
                        e.Cancel = true;

                    foreach (var dn in this.tvwProcessAliases.SelectedNodes.OfType<IDataNode>())
                    {
                        if (dn.IsRowValid)
                        {
                            dn.UpdateNodeUI();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error processing selection.", exc);
            }
        }

        private void tvwProcessAliases_AfterSelect(object sender, SelectEventArgs e)
        {
            if (e.NewSelections.Count == 1)
                LoadNode(e.NewSelections[0]);
        }

        private void txtCustomerProcessAliasName_Leave(object sender, EventArgs e)
        {
            var node = tvwProcessAliases.SelectedNode<CustomerProcessAliasNode>();
            if (node != null)
            {
                txtCustomerProcessAliasName.DataBindings[0].WriteValue();
                node.UpdateNodeUI();
            }
        }

        private void txtProcessAliasName_Leave(object sender, EventArgs e)
        {
            if (txtProcessAliasName.Text != "")
            {
                var node = tvwProcessAliases.SelectedNode<ProcessAliasNode>();
                if (node != null)
                {
                    txtProcessAliasName.DataBindings[0].WriteValue();
                    node.UpdateNodeUI();
                }
            }
        }

        private void tabProcessInfo_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {
            if (_toolbarManager != null)
            {
                //if on Alias tab then show the context tab for aliases
                var aliasContextTabVisible = e.Tab != null && e.Tab.Key == "Aliases";
                _toolbarManager.Ribbon.ContextualTabGroups["aliasContext"].Visible = aliasContextTabVisible;

                if (aliasContextTabVisible)
                    _toolbarManager.Ribbon.SelectedTab = _toolbarManager.Ribbon.Tabs["aliasRibbon"];

                //if on Time Constraint tab then show the context tab for time constraints
                var timeContextTabVisible = e.Tab != null && e.Tab.Key == "Time";
                _toolbarManager.Ribbon.ContextualTabGroups["constraintsContext"].Visible = timeContextTabVisible;

                if (timeContextTabVisible)
                    _toolbarManager.Ribbon.SelectedTab = _toolbarManager.Ribbon.Tabs["constraintRibbon"];
            }
        }

        private void cboProcessCategory_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            AddProcessCategories();
        }


        private void cboStatus_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cboStatus.Value != null)
            {
                string currentValue = cboStatus.Value.ToString();

                var process = CurrentRecord as ProcessesDataset.ProcessRow;
                if (process != null)
                {
                    switch (currentValue)
                    {
                        case "Approved":
                            process.IsApproved = true;
                            process.Active = true;
                            break;
                        case "Planned":
                            process.IsApproved = false;
                            process.Active = true;
                            break;
                        case "Closed":
                            process.IsApproved = false;
                            process.Active = false;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void numLoadCapacityVariance_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                var process = CurrentRecord as ProcessesDataset.ProcessRow;

                if (process == null)
                {
                    return;
                }

                var variancePercent = numLoadCapacityVariance.Value as decimal?;

                if (variancePercent.HasValue)
                {
                    // Convert from percentage representation
                    process.LoadCapacityVariance = variancePercent.Value / 100M;
                }
                else
                {
                    process.SetLoadCapacityVarianceNull();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing load capacity variance.");
            }
        }

        private void cboLoadCapacityType_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateLoadCapacity();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing load capacity type.");
            }
        }

        private void MaterialCostDropDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                RefreshMaterialCostText();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing value for material unit.");
            }
        }

        private void LeadTimeDropDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                RefreshLeadTimeText();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing value for lead time type.");
            }
        }

        private void curMaterialCost_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                curMaterialCost.Validated -= curMaterialCost_ValidatedAfterChange; // Do not register more than once

                var dropDownButton = curMaterialCost.ButtonsRight[0] as DropDownEditorButton;
                var dropDown = dropDownButton.Control as UltraComboEditor;

                if (curMaterialCost.Value != 0M && string.IsNullOrEmpty(dropDownButton.Text))
                {
                    // Values do not update properly when updating from this handler.
                    // Wait until after validation before changing the value.
                    curMaterialCost.Validated += curMaterialCost_ValidatedAfterChange;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing material cost.");
            }
        }

        private void numLeadTime_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                numLeadTime.Validated -= numLeadTime_ValidatedAfterChange; // Do not register more than once

                var dropDownButton = numLeadTime.ButtonsRight[0] as DropDownEditorButton;
                var dropDown = dropDownButton.Control as UltraComboEditor;

                var currentLeadTime = numLeadTime.Value as decimal? ?? 0M;
                if (currentLeadTime != 0M && string.IsNullOrEmpty(dropDownButton.Text))
                {
                    // Values do not update properly when updating from this handler.
                    // Wait until after validation before changing the value.
                    numLeadTime.Validated += numLeadTime_ValidatedAfterChange;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing material cost.");
            }
        }

        /// <summary>
        /// Assigns a default unit of measure to the current process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void curMaterialCost_ValidatedAfterChange(object sender, EventArgs e)
        {
            const string defaultUnitOfMeasure = "in²";

            try
            {
                curMaterialCost.Validated -= curMaterialCost_ValidatedAfterChange;

                var currentProcess = CurrentRecord as ProcessesDataset.ProcessRow;

                if (currentProcess == null)
                {
                    return;
                }

                var dropDownButton = curMaterialCost.ButtonsRight[0] as DropDownEditorButton;
                var dropDown = dropDownButton.Control as UltraComboEditor;

                currentProcess.MaterialUnit = defaultUnitOfMeasure;
                dropDown.Value = dropDown.FindItemByValue<string>(v => v == defaultUnitOfMeasure);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after validating material cost.");
            }
        }

        /// <summary>
        /// Assigns a default lead time type to the current process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numLeadTime_ValidatedAfterChange(object sender, EventArgs e)
        {
            const string defaultLeadTimeType = "load";

            try
            {
                numLeadTime.Validated -= numLeadTime_ValidatedAfterChange;

                var currentProcess = CurrentRecord as ProcessesDataset.ProcessRow;

                if (currentProcess == null)
                {
                    return;
                }

                var dropDownButton = numLeadTime.ButtonsRight[0] as DropDownEditorButton;
                var dropDown = dropDownButton.Control as UltraComboEditor;

                currentProcess.LeadTimeType = defaultLeadTimeType;
                dropDown.Value = dropDown.FindItemByValue<string>(v => v == defaultLeadTimeType);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error after validating material cost.");
            }
        }

        private void grdConstraints_Error(object sender, ErrorEventArgs e)
        {
            // If user enters an incorrect process name, revert the change.
            if (e.ErrorType == ErrorType.Data)
            {
                grdConstraints.ActiveCell.SetValue(grdConstraints.ActiveCell.OriginalValue, false);
            }
        }

        private void grdSuggestions_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                var gridRow = e.Row;

                if (gridRow == null)
                {
                    return;
                }

                // Set row color based on its status
                var status = gridRow.Cells[nameof(ProcessSuggestionItem.Status)].Value.ToString();

                if (status == STATUS_CLOSED)
                {
                    gridRow.Appearance.ForeColor = Color.Red;
                    gridRow.Appearance.FontData.Bold = DefaultableBoolean.True;
                }
                else if (status == STATUS_PLANNED)
                {
                    gridRow.Appearance.ForeColor = Color.DarkGoldenrod;
                    gridRow.Appearance.FontData.Bold = DefaultableBoolean.True;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error loading row for suggestions.");
            }

        }

        private void btnProcessProductClass_Click(object sender, EventArgs e)
        {
            try
            {
                var processRow = CurrentRecord as ProcessesDataset.ProcessRow;
                if (processRow == null)
                {
                    return;
                }

                var dialog = new ProcessProductClassEditor();
                dialog.Load(Dataset, processRow);
                new WindowInteropHelper(dialog) { Owner = Handle };

                if (dialog.ShowDialog() ?? false)
                {
                    dialog.ApplyChanges();

                    // Add global product classes for any new product classes added here
                    foreach (var processProductClass in processRow.GetProcessProductClassRows())
                    {
                        var addProductClass = !processProductClass.IsProductClassNull() &&
                            Dataset.ProductClass.All(pc => pc.Name != processProductClass.ProductClass);

                        if (addProductClass)
                        {
                            Dataset.ProductClass.AddProductClassRow(processProductClass.ProductClass);
                        }
                    }

                    UpdateProductClassText(processRow);
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error showing process product class dialog", exc);
            }
        }

        #endregion

        #region Process Aliases Validator

        public class ProcessAliasesValiditor : ControlValidatorBase
        {
            #region Fields

            private UltraTree _tree;
            private ProcessInfo _processInfo;
            #endregion

            #region Methods

            public ProcessAliasesValiditor(Infragistics.Win.UltraWinTabControl.UltraTab tab, UltraTree tree, ProcessInfo processInfo) : base(tab.TabPage)
            {
                _tree = tree;
                _processInfo = processInfo;
            }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                if (_tree != null && _tree.Enabled)
                {
                    //has to have at least 1 process alias node
                    var node = _tree.FindNode(n => n is ProcessAliasNode) as ProcessAliasNode;

                    if (node == null)
                    {
                        if (_processInfo.txtProcessName.Text.HasValue())
                        {
                            _processInfo.AddProcessAlias();
                            //MessageBoxUtilities.ShowMessageBoxWarn("Process must contain at least 1 process alias.", "Process Alias");
                            //e.Cancel = true;
                            //FireAfterValidation(false, _errMsg);
                            return;
                        }
                    }
                }

                //passed
                e.Cancel = false;
                FireAfterValidation(true, String.Empty);
            }

            #endregion
        }

        #endregion

        #region DepartmentValidator

        private sealed class DepartmentValidator : ControlValidatorBase
        {
            #region Fields

            private Lazy<string> _lazyDepartmentIdNone;

            #endregion

            #region Properties

            public ProcessInfo ProcessInfo
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public DepartmentValidator(Infragistics.Win.UltraWinEditors.UltraComboEditor control, ProcessInfo processInfo)
                : base(control)
            {
                ProcessInfo = processInfo;

                _lazyDepartmentIdNone = new Lazy<string>(
                    () =>
                    {
                        return ProcessInfo.Dataset.d_Department
                            .FirstOrDefault(row => row.IsValidState() && !row.IsSystemNameNull() && row.SystemName == "None")
                            ?.DepartmentID;
                    });
            }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                var cboDept = Control as Infragistics.Win.UltraWinEditors.UltraComboEditor;

                if (cboDept == null)
                {
                    return;
                }

                string status = Convert.ToString(ProcessInfo.cboStatus.Value);

                if (status == STATUS_APPROVED)
                {
                    if (Convert.ToString(cboDept.Value) == _lazyDepartmentIdNone.Value)
                    {
                        string errorMsg = string.Format("{0} is not a valid department for approved processes.",
                            cboDept.Value);

                        e.Cancel = true;
                        FireAfterValidation(false, errorMsg);
                        return;
                    }
                }

                e.Cancel = false;
                FireAfterValidation(true, string.Empty);
            }

            #endregion
        }

        #endregion

        #region MaterialCostValidator

        internal sealed class MaterialCostValidator : ControlValidatorBase
        {
            #region Methods

            public MaterialCostValidator(Control control)
                : base(control)
            {
            }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                var curMaterialCost = Control as UltraCurrencyEditor;

                if (curMaterialCost == null || curMaterialCost.ButtonsRight.Count == 0)
                {
                    return;
                }

                var dropDownButton = curMaterialCost.ButtonsRight[0] as DropDownEditorButton;

                var unitHasValue = !string.IsNullOrEmpty(dropDownButton.Text);
                var costHasValue = curMaterialCost.ValueObject != null;

                if (costHasValue && !unitHasValue)
                {
                    e.Cancel = true;
                    FireAfterValidation(false, "Material cost requires a value and a unit of measure.");
                }

                else
                {
                    e.Cancel = false;
                    FireAfterValidation(true, string.Empty);
                }
            }

            #endregion
        }

        #endregion

        #region LeadTimeValidator

        internal sealed class LeadTimeValidator : ControlValidatorBase
        {
            #region Methods

            public LeadTimeValidator(Control control)
                : base(control)
            {
            }

            internal override void ValidateControl(object sender, CancelEventArgs e)
            {
                var numLeadTime = Control as UltraNumericEditor;

                if (numLeadTime == null || numLeadTime.ButtonsRight.Count == 0)
                {
                    return;
                }

                var dropDownButton = numLeadTime.ButtonsRight[0] as DropDownEditorButton;

                var typeHasValue = !string.IsNullOrEmpty(dropDownButton.Text);
                var timeHasValue = numLeadTime.Value != null && numLeadTime.Value != DBNull.Value;

                if (timeHasValue && !typeHasValue)
                {
                    e.Cancel = true;
                    FireAfterValidation(false, "Lead time requires a value and a type.");
                }

                else
                {
                    e.Cancel = false;
                    FireAfterValidation(true, string.Empty);
                }
            }

            #endregion
        }

        #endregion

        #region Nodes

        #region Nested type: ProcessesRootNode

        internal class ProcessAliasRootNode : UltraTreeNode
        {
            #region Methods

            public ProcessAliasRootNode()
                : base("ROOT", "Aliases")
            {
                LeftImages.Add(Properties.Resources.Processes_32);
            }

            #endregion
        }

        #endregion

        #region Nested type: ProcessAliasNode

        internal class ProcessAliasNode : DataNode<ProcessesDataset.ProcessAliasRow>
        {
            #region Fields

            public const string KEY_PREFIX = "PRA";

            #endregion

            #region Properties

            public bool IsDataLoaded { get; set; }

            public int? UsageCount { get; set; }

            #endregion

            #region Methods

            public ProcessAliasNode(ProcessesDataset.ProcessAliasRow cr)
                : base(cr, cr.ProcessAliasID.ToString(), KEY_PREFIX, cr.Name)
            {
                LeftImages.Add(Properties.Resources.Process_16);
                this.UpdateNodeUI();
            }

            public override bool CanDelete =>
                UsageCount.HasValue && UsageCount < 1;

            public override void UpdateNodeUI()
            {
                try
                {
                    if (this.IsRowValid)
                        this.Text = this.DataRow.Name;
                }
                catch (Exception exc)
                {
                    NLog.LogManager.GetCurrentClassLogger().Info(exc, "Error updating node ui.");
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: CustomerProcessAliasNode

        internal class CustomerProcessAliasNode : DataNode<ProcessesDataset.CustomerProcessAliasRow>
        {
            #region Fields

            public const string KEY_PREFIX = "CPRA";
            public int UsageCount = -1;

            #endregion

            #region Properties

            #endregion

            #region Methods

            public CustomerProcessAliasNode(ProcessesDataset.CustomerProcessAliasRow cr)
                : base(cr, cr.CustomerProcessAliasID.ToString(), KEY_PREFIX, cr.Name)
            {
                LeftImages.Add(Properties.Resources.Customer);
                this.UpdateNodeUI();
            }

            public override bool CanDelete
            {
                get { return this.UsageCount < 1; }
            }

            public override void UpdateNodeUI()
            {
                this.Text = this.DataRow.Name;
            }

            #endregion
        }

        #endregion

        #endregion

        #region Commands

        private class AddProcessAliasCommand : TreeNodeCommandBase
        {
            #region Fields

            private ProcessInfo _processInfo;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return true; }
            }

            #endregion

            #region Methods

            public AddProcessAliasCommand(ToolBase tool, UltraTree toc, ProcessInfo processInfo)
                : base(tool)
            {
                base.TreeView = toc;
                _processInfo = processInfo;
            }

            public override void OnClick()
            {
                try
                {
                    _processInfo.EndProcessAliasEdit();
                    _processInfo.AddProcessAlias();
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error adding new process alias.", exc);
                }

            }

            public override void Dispose()
            {
                _processInfo = null;
                base.Dispose();
            }

            #endregion
        }

        private class AddCustomerProcessAliasCommand : TreeNodeCommandBase
        {
            #region Fields

            private ProcessInfo _processInfo;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return base.TreeView.SelectedNode<ProcessAliasNode>() != null; }
            }

            #endregion

            #region Methods

            public AddCustomerProcessAliasCommand(ToolBase tool, UltraTree toc, ProcessInfo processInfo)
                : base(tool)
            {
                base.TreeView = toc;
                _processInfo = processInfo;
            }

            public override void OnClick()
            {
                try
                {
                    _processInfo.EndProcessAliasEdit();
                    _processInfo.AddCustomerProcessAlias();
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error adding new process alias.", exc);
                }

            }

            public override void Dispose()
            {
                _processInfo = null;
                base.Dispose();
            }

            #endregion
        }

        private class ListViewCommand : CommandBase
        {
            #region Fields

            protected UltraListView _listView;
            protected ProcessInfo _processInfo;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _processInfo.Editable; }
            }

            #endregion

            #region Methods

            public ListViewCommand(UltraButton tool, UltraListView listView, ProcessInfo processInfo)
                : base(tool)
            {
                _processInfo = processInfo;
                _listView = listView;
                _listView.ItemSelectionChanged += _listView_ItemSelectionChanged;
                _processInfo.EditableStatusChanged += _processInfo_EditableStatusChanged;
                base.Refresh();
            }

            public override void Dispose()
            {
                if (_listView != null)
                    _listView.ItemSelectionChanged -= _listView_ItemSelectionChanged;

                _listView = null;

                if (_processInfo != null)
                    _processInfo.EditableStatusChanged -= _processInfo_EditableStatusChanged;
                _processInfo = null;

                base.Dispose();
            }

            #endregion

            #region Events

            private void _listView_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e) { base.Refresh(); }

            private void _processInfo_EditableStatusChanged(object sender, EventArgs e) { Refresh(); }

            #endregion
        }

        private class AddInspectionCommand : ListViewCommand
        {
            #region Methods

            public AddInspectionCommand(UltraButton tool, UltraListView listView, ProcessInfo processInfo)
                : base(tool, listView, processInfo)
            {

            }

            public override void OnClick()
            {
                try
                {
                    var processRow = this._processInfo.CurrentRecord as Data.Datasets.ProcessesDataset.ProcessRow;

                    if (processRow != null)
                    {
                        _log.Info("Adding a new inspection to process: " + processRow.ProcessID);

                        using (var cbo = new ComboBoxForm())
                        {
                            cbo.Text = "Add Inspection";
                            cbo.FormLabel.Text = "Inspection:";
                            cbo.ComboBox.DropDownStyle = DropDownStyle.DropDownList;
                            cbo.ComboBox.ValueMember = _processInfo.Dataset.PartInspectionType.PartInspectionTypeIDColumn.ColumnName;
                            cbo.ComboBox.DisplayMember = _processInfo.Dataset.PartInspectionType.NameColumn.ColumnName;

                            var partInspectionView = _processInfo.Dataset.PartInspectionType.AsDataView();
                            partInspectionView.RowFilter = "Active = 1";
                            partInspectionView.Sort = "Name ASC";
                            cbo.ComboBox.DataSource = partInspectionView;
                            cbo.ComboBox.DataBind();

                            if (cbo.ComboBox.Items.Count > 0)
                                cbo.ComboBox.SelectedIndex = 0;

                            if (cbo.ShowDialog(_processInfo) == DialogResult.OK)
                            {
                                if (cbo.ComboBox.SelectedItem != null)
                                {
                                    var inspectionType = ((DataRowView)cbo.ComboBox.SelectedItem.ListObject).Row as Data.Datasets.ProcessesDataset.PartInspectionTypeRow;
                                    var lastStep = this._listView.Items.Count < 1 ? 0 : this._listView.Items.OfType<InspectionItem>().Max(pi => pi.StepOrder);

                                    //create new data source
                                    var inspection = this._processInfo.Dataset.ProcessInspections.NewRow() as Data.Datasets.ProcessesDataset.ProcessInspectionsRow;
                                    inspection.PartInspectionTypeID = inspectionType.PartInspectionTypeID;
                                    inspection.StepOrder = lastStep + 1;
                                    inspection.ProcessID = processRow.ProcessID;
                                    inspection.COCData = false;

                                    this._processInfo.Dataset.ProcessInspections.Rows.Add(inspection);

                                    //create new ui nodes
                                    this._listView.Items.Add(new InspectionItem(inspection));
                                }
                            }
                        }
                    }

                    _processInfo.UpdateInspectionsTabText();
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error adding the process to the part.", exc);
                }
            }

            #endregion
        }

        private class DeleteInspectionCommand : ListViewCommand
        {
            #region Properties

            public override bool Enabled
            {
                get { return _processInfo.Editable && this._listView.SelectedItems.Count == 1 && this._listView.SelectedItems[0] is InspectionItem && ((InspectionItem)this._listView.SelectedItems[0]).ProcessInspection != null; }
            }

            #endregion

            #region Methods

            public DeleteInspectionCommand(UltraButton tool, UltraListView listView, ProcessInfo processInfo)
                : base(tool, listView, processInfo)
            {

            }

            public override void OnClick()
            {
                try
                {
                    var processRow = this._processInfo.CurrentRecord as Data.Datasets.ProcessesDataset.ProcessRow;

                    if (processRow != null)
                    {
                        _log.Info("Deleting inspection from order: " + processRow.ProcessID);

                        if (this._listView.SelectedItems.Count == 1 && this._listView.SelectedItems[0] is InspectionItem)
                        {
                            var pi = this._listView.SelectedItems[0] as InspectionItem;
                            pi.ProcessInspection.Delete();
                            _listView.Items.Remove(pi);
                            _processInfo.ResynchInspectionStepOrder();
                        }
                    }

                    _processInfo.UpdateInspectionsTabText();
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error deleting the process from the order.", exc);
                }
            }

            #endregion
        }

        private class MoveInspectionStepDownCommand : ListViewCommand
        {
            #region Properties

            public override bool Enabled
            {
                get
                {
                    //if not last one
                    if (_processInfo.Editable && this._listView.SelectedItems.Count == 1 && this._listView.SelectedItems[0] is InspectionItem)
                    {
                        var pi = this._listView.SelectedItems[0] as InspectionItem;
                        return pi.Index < this._listView.Items.Count - 1;
                    }

                    return false;
                }
            }

            #endregion

            #region Methods

            public MoveInspectionStepDownCommand(UltraButton tool, UltraListView listView, ProcessInfo processInfo)
                : base(tool, listView, processInfo)
            {
            }

            public override void OnClick()
            {
                try
                {
                    var orderRow = this._processInfo.CurrentRecord as Data.Datasets.ProcessesDataset.ProcessRow;

                    if (orderRow != null)
                    {
                        //Suspend the sorting until the StepOrder is reassigned 
                        using (new UsingListViewLoad(this._listView))
                        {
                            _log.Info("Move inspection down in process: " + orderRow.ProcessID);

                            List<InspectionItem> orderProcesses = this._listView.Items.OfType<InspectionItem>().Where(pi => pi.ProcessInspection != null).OrderBy(pi => pi.StepOrder).ToList();
                            var currentProcess = this._listView.SelectedItems[0] as InspectionItem;
                            bool foundProcess = false;

                            foreach (InspectionItem orderProcess in orderProcesses)
                            {
                                if (orderProcess.ProcessInspection.ProcessInspectionID == currentProcess.ProcessInspection.ProcessInspectionID)
                                {
                                    //move this process down one
                                    currentProcess.StepOrder += 1;
                                    foundProcess = true;

                                    continue;
                                }

                                if (foundProcess)
                                {
                                    //move next process up one
                                    orderProcess.StepOrder -= 1;
                                    this._listView.SelectedItems.Clear();
                                    this._listView.SelectedItems.Add(this._listView.ActiveItem);

                                    break;
                                }
                            }
                        }

                        this._listView.Items.RefreshSort(true);
                        _processInfo.ResynchInspectionStepOrder();
                        _processInfo.UpdateInspectionsTabText();
                        base.Refresh();
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error moving process up.", exc);
                }
            }

            #endregion
        }

        private class MoveInspectionStepUpCommand : ListViewCommand
        {
            #region Properties

            public override bool Enabled
            {
                get
                {
                    //if not first one then can move up
                    return _processInfo.Editable && this._listView.SelectedItems.Count == 1 && this._listView.SelectedItems[0] is InspectionItem && ((InspectionItem)this._listView.SelectedItems[0]).Index >= 1;
                }
            }

            #endregion

            #region Methods

            public MoveInspectionStepUpCommand(UltraButton tool, UltraListView listView, ProcessInfo processInfo)
                : base(tool, listView, processInfo)
            {
            }

            public override void OnClick()
            {
                try
                {
                    var orderRow = this._processInfo.CurrentRecord as Data.Datasets.ProcessesDataset.ProcessRow;

                    if (orderRow != null)
                    {
                        //Suspend the sorting until the StepOrder is reassigned 
                        using (new UsingListViewLoad(this._listView))
                        {
                            _log.Info("Move inspection up in process: " + orderRow.ProcessID);

                            List<InspectionItem> orderProcesses = this._listView.Items.OfType<InspectionItem>().Where(pi => pi.ProcessInspection != null).OrderByDescending(pi => pi.StepOrder).ToList();
                            if (this._listView.SelectedItems.Count > 0)
                            {
                                var currentProcess = this._listView.SelectedItems[0] as InspectionItem;

                                bool foundProcess = false;

                                //for each process in reverse order
                                foreach (InspectionItem orderProcess in orderProcesses)
                                {
                                    if (orderProcess.ProcessInspection.ProcessInspectionID == currentProcess.ProcessInspection.ProcessInspectionID)
                                    {
                                        //move this process up one
                                        currentProcess.StepOrder -= 1;
                                        foundProcess = true;

                                        continue;
                                    }

                                    if (foundProcess)
                                    {
                                        //move next process down one
                                        orderProcess.StepOrder += 1;
                                        this._listView.SelectedItems.Clear();
                                        this._listView.SelectedItems.Add(this._listView.ActiveItem);

                                        break;
                                    }
                                }
                            }
                        }

                        this._listView.Items.RefreshSort(true);
                        _processInfo.ResynchInspectionStepOrder();
                        _processInfo.UpdateInspectionsTabText();
                        base.Refresh();
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error moving process up.", exc);
                }
            }

            #endregion
        }

        private class CheckInspectionCommand : ListViewCommand
        {
            #region Properties

            public override bool Enabled
            {
                get { return this._listView.SelectedItems.Count > 0; }
            }

            #endregion

            #region Methods

            public CheckInspectionCommand(UltraButton tool, UltraListView listView, ProcessInfo processInfo)
                : base(tool, listView, processInfo)
            {

            }

            public override void OnClick()
            {
                try
                {
                    var inspections = this._listView.SelectedItems.OfType<InspectionItem>();

                    foreach (var i in inspections)
                    {
                        i.ToggleCOC();
                    }

                    _listView.Refresh();
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error adding the process to the part.", exc);
                }
            }

            #endregion
        }

        private class AddConstraintCommand : CommandBase
        {
            #region Fields

            private ProcessInfo _processInfo;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return true; }
            }

            #endregion

            #region Methods

            public AddConstraintCommand(ToolBase tool, ProcessInfo processInfo)
                : base(tool)
            {
                _processInfo = processInfo;
                _processInfo.MovedToNewRecord += _processInfo_MovedToNewRecord;
                _processInfo.EditableStatusChanged += _processInfo_EditableStatusChanged;
                base.Refresh();
            }


            public override void OnClick()
            {
                using (var frmProcesses = new ProcessPicker())
                {
                    //display Add Process dialog
                    if (frmProcesses.ShowDialog() == DialogResult.OK && frmProcesses.SelectedProcesses.Count > 0)
                    {
                        foreach (var selectedProcess in frmProcesses.SelectedProcesses)
                        {
                            if (selectedProcess.ProcessID > 0)
                            {
                                _processInfo.AddProcessConstraint(selectedProcess.ProcessID);
                                _processInfo.UpdateConstraintsTabText();
                            }
                        }
                    }
                }


            }

            public override void Dispose()
            {
                if (_processInfo != null)
                {
                    _processInfo.EditableStatusChanged -= _processInfo_EditableStatusChanged;
                    _processInfo.MovedToNewRecord -= _processInfo_MovedToNewRecord;
                }

                _processInfo = null;

                base.Dispose();
            }

            #endregion

            #region Events

            private void _processInfo_EditableStatusChanged(object sender, EventArgs e) { Refresh(); }

            private void _processInfo_MovedToNewRecord(object sender, EventArgs e)
            {
                Refresh();
            }

            #endregion
        }

        private class DeleteConstraintCommand : CommandBase
        {
            #region Fields

            private ProcessInfo _processInfo;

            #endregion

            #region Properties

            public override bool Enabled
            {
                get { return _processInfo.grdConstraints.Enabled && _processInfo.grdConstraints.Selected.Rows.Count > 0; }
            }

            #endregion

            #region Methods

            public DeleteConstraintCommand(ToolBase tool, ProcessInfo processInfo)
                : base(tool)
            {
                _processInfo = processInfo;
                _processInfo.MovedToNewRecord += _processInfo_MovedToNewRecord;
                _processInfo.grdConstraints.AfterSelectChange += grdConstraints_AfterSelectChange;
                base.Refresh();
            }

            public override void OnClick()
            {
                _processInfo.grdConstraints.DeleteSelectedRows();
                _processInfo.UpdateConstraintsTabText();
            }

            public override void Dispose()
            {
                if (_processInfo != null)
                {
                    _processInfo.MovedToNewRecord -= _processInfo_MovedToNewRecord;
                    _processInfo.grdConstraints.AfterSelectChange -= grdConstraints_AfterSelectChange;
                }

                _processInfo = null;

                base.Dispose();
            }

            #endregion

            #region Events

            private void _processInfo_MovedToNewRecord(object sender, EventArgs e)
            {
                Refresh();
            }

            private void grdConstraints_AfterSelectChange(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
            {
                this.Refresh();
            }

            #endregion
        }

        #endregion

        #region AddSuggestionCommand

        private class AddSuggestionCommand : CommandBase
        {
            #region Fields

            private ProcessInfo _processInfo;

            #endregion

            #region Properties

            public override bool Enabled => true;

            #endregion

            #region Methods

            public AddSuggestionCommand(UltraButton tool, ProcessInfo processInfo)
                : base(tool)
            {
                _processInfo = processInfo;
                _processInfo.MovedToNewRecord += _processInfo_MovedToNewRecord;
                _processInfo.EditableStatusChanged += _processInfo_EditableStatusChanged;
                Refresh();
            }

            public override void OnClick()
            {
                var primaryProcess = _processInfo.CurrentRecord as ProcessesDataset.ProcessRow;
                var editorWindow = new ProcessSuggestionEditor();
                editorWindow.LoadData(primaryProcess?.ProcessID ?? 0, _processInfo.Dataset, _processInfo.GetManufacturers());
                editorWindow.PrimaryProcessName = (_processInfo.CurrentRecord as ProcessesDataset.ProcessRow)?.Name;

                var helper = new WindowInteropHelper(editorWindow) {Owner = DWOSApp.MainForm.Handle};

                if(editorWindow.ShowDialog() ?? false)
                {
                    var processId = editorWindow.SelectedProcess?.ProcessId ?? 0;
                    var processAliasId = editorWindow.SelectedProcessAlias?.ProcessAliasId ?? 0;

                    var matchingProcess = _processInfo.Dataset.Process.FindByProcessID(processId);
                    var matchingProcessAlias = _processInfo.Dataset.ProcessAlias.FindByProcessAliasID(processAliasId);

                    if(matchingProcess != null && matchingProcessAlias != null && matchingProcessAlias.ProcessID == processId)
                    {
                        _processInfo.AddSuggestion(matchingProcess,
                            matchingProcessAlias,
                            editorWindow.SelectedSuggestionType.ToString(),
                            editorWindow.SelectedConditionType,
                            editorWindow.SelectedConditionOperator,
                            editorWindow.SelectedConditionValue);
                    }
                    else
                    {
                        var errorMsg = matchingProcess == null || matchingProcessAlias == null
                            ? "Process Suggestion: Expected non-null process/alias value."
                            : "Process Suggestion: Alias is not one of the process's aliases.";

                        _log.Warn(errorMsg);
                    }
                }

                GC.KeepAlive(helper);
            }

            public override void Dispose()
            {
                if(_processInfo != null)
                {
                    _processInfo.EditableStatusChanged -= _processInfo_EditableStatusChanged;
                    _processInfo.MovedToNewRecord -= _processInfo_MovedToNewRecord;
                }

                _processInfo = null;

                base.Dispose();
            }

            #endregion

            #region Events

            private void _processInfo_EditableStatusChanged(object sender, EventArgs e) =>
                Refresh();

            private void _processInfo_MovedToNewRecord(object sender, EventArgs e) =>
                Refresh();

            #endregion
        }

        #endregion

        #region DeleteSuggestionCommand

        private class DeleteSuggestionCommand : CommandBase
        {
            #region Fields

            private ProcessInfo _processInfo;

            #endregion

            #region Properties

            public override bool Enabled => _processInfo.grdSuggestions.Enabled
                && _processInfo.grdSuggestions.Selected.Rows.Count > 0;

            #endregion

            #region Methods

            public DeleteSuggestionCommand(UltraButton tool, ProcessInfo processInfo)
                : base(tool)
            {
                _processInfo = processInfo;
                _processInfo.MovedToNewRecord += _processInfo_MovedToNewRecord;
                _processInfo.grdSuggestions.AfterSelectChange += grdSuggestions_AfterSelectChange;
                Refresh();
            }

            public override void OnClick()
            {
                if (_processInfo.grdSuggestions.ActiveRow?.ListObject is ProcessSuggestionItem suggestion)
                {
                    _processInfo._suggestions.Remove(suggestion);
                    suggestion.Row.Delete();
                    _processInfo.UpdateSuggestionsTab();
                }
            }

            public override void Dispose()
            {
                if (_processInfo != null)
                {
                    _processInfo.MovedToNewRecord -= _processInfo_MovedToNewRecord;
                    _processInfo.grdSuggestions.AfterSelectChange -= grdSuggestions_AfterSelectChange;
                }

                _processInfo = null;

                base.Dispose();
            }

            #endregion

            #region Events

            private void _processInfo_MovedToNewRecord(object sender, EventArgs e)
            {
                Refresh();
            }

            private void grdSuggestions_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
            {
                Refresh();
            }

            #endregion
        }

        #endregion

        #region EditSuggestionCommand

        private class EditSuggestionCommand : CommandBase
        {
            #region Fields

            private ProcessInfo _processInfo;

            #endregion

            #region Properties

            public override bool Enabled => _processInfo.grdSuggestions.Enabled
                && _processInfo.grdSuggestions.Selected.Rows.Count > 0;

            #endregion

            #region Methods

            public EditSuggestionCommand(UltraButton tool, ProcessInfo processInfo)
                : base(tool)
            {
                _processInfo = processInfo;
                _processInfo.MovedToNewRecord += _processInfo_MovedToNewRecord;
                _processInfo.grdSuggestions.AfterSelectChange += grdSuggestions_AfterSelectChange;
                Refresh();
            }

            public override void OnClick()
            {
                if (!(_processInfo.grdSuggestions.ActiveRow?.ListObject is ProcessSuggestionItem suggestion))
                {
                    return;
                }

                var primaryProcess = _processInfo.CurrentRecord as ProcessesDataset.ProcessRow;
                var editorWindow = new ProcessSuggestionEditor();
                editorWindow.LoadData(primaryProcess?.ProcessID ?? 0, _processInfo.Dataset, _processInfo.GetManufacturers());
                editorWindow.PrimaryProcessName = (_processInfo.CurrentRecord as ProcessesDataset.ProcessRow)?.Name;
                editorWindow.LoadSuggestion(suggestion.Row);

                var helper = new WindowInteropHelper(editorWindow) {Owner = DWOSApp.MainForm.Handle};

                if(editorWindow.ShowDialog() ?? false)
                {
                    var processId = editorWindow.SelectedProcess?.ProcessId ?? 0;
                    var processAliasId = editorWindow.SelectedProcessAlias?.ProcessAliasId ?? 0;

                    var matchingProcess = _processInfo.Dataset.Process.FindByProcessID(processId);
                    var matchingProcessAlias = _processInfo.Dataset.ProcessAlias.FindByProcessAliasID(processAliasId);

                    if(matchingProcess != null && matchingProcessAlias != null && matchingProcessAlias.ProcessID == processId)
                    {
                        suggestion.Row.ProcessRowByFK_ProcessSuggestion_Process_Suggested = matchingProcess;
                        suggestion.Row.ProcessAliasRow = matchingProcessAlias;
                        suggestion.Row.Type = editorWindow.SelectedSuggestionType.ToString();
                        suggestion.Row.ConditionType = editorWindow.SelectedConditionType;
                        suggestion.Row.ConditionOperator = editorWindow.SelectedConditionOperator;
                        suggestion.Row.ConditionValue = editorWindow.SelectedConditionValue;

                        suggestion.RaiseRowUpdate();

                        _processInfo.UpdateSuggestionsTab();
                    }
                    else
                    {
                        var errorMsg = matchingProcess == null || matchingProcessAlias == null
                            ? "Process Suggestion: Expected non-null process/alias value."
                            : "Process Suggestion: Alias is not one of the process's aliases.";

                        _log.Warn(errorMsg);
                    }
                }

                GC.KeepAlive(helper);
            }

            public override void Dispose()
            {
                if (_processInfo != null)
                {
                    _processInfo.MovedToNewRecord -= _processInfo_MovedToNewRecord;
                    _processInfo.grdSuggestions.AfterSelectChange -= grdSuggestions_AfterSelectChange;
                }

                _processInfo = null;

                base.Dispose();
            }

            #endregion

            #region Events

            private void _processInfo_MovedToNewRecord(object sender, EventArgs e)
            {
                Refresh();
            }

            private void grdSuggestions_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
            {
                Refresh();
            }

            #endregion
        }

        #endregion

        #region ImportSuggestionsCommand

        private class ImportSuggestionsCommand : CommandBase
        {
            #region Fields

            private ProcessInfo _processInfo;

            #endregion

            #region Properties

            public override bool Enabled => true;

            #endregion

            #region Methods

            public ImportSuggestionsCommand(UltraButton tool, ProcessInfo processInfo)
                : base(tool)
            {
                _processInfo = processInfo;
                _processInfo.MovedToNewRecord += _processInfo_MovedToNewRecord;
                _processInfo.EditableStatusChanged += _processInfo_EditableStatusChanged;
                Refresh();
            }

            public override void OnClick()
            {
                if (!(_processInfo.CurrentRecord is ProcessesDataset.ProcessRow primaryProcess))
                {
                    return;
                }

                var importWindow = new SuggestionImport();

                importWindow.LoadData(primaryProcess.ProcessID, primaryProcess.Name);

                var helper = new WindowInteropHelper(importWindow) { Owner = DWOSApp.MainForm.Handle };

                if (importWindow.ShowDialog() ?? false)
                {
                    foreach (var process in importWindow.GetSelectedProcesses())
                    {
                        var processRow = _processInfo.Dataset.Process.FindByProcessID(process.ProcessId);
                        var processAliasRow = _processInfo.Dataset.ProcessAlias.FindByProcessAliasID(process.ProcessAliasId);

                        if (processRow == null)
                        {
                            // Should not normally happen - cannot typically remove in-use process
                            _log.Warn($"Could not find process for suggested process ID {process.ProcessId}");
                        }
                        else if (processAliasRow == null)
                        {
                            // Should not normally happen - cannot typically remove in-use alias
                            _log.Warn($"Could not find process for suggested process alias ID {process.ProcessAliasId}");
                        }
                        else if (!processRow.Active)
                        {
                            // User selected an inactive process.
                            MessageBoxUtilities.ShowMessageBoxWarn(
                                $"Cannot add process {processRow.Name} - it is inactive.",
                                "Process Manager");
                        }
                        else
                        {
                            _processInfo.AddSuggestion(processRow,
                                processAliasRow,
                                process.Type,
                                null,
                                null,
                                null);
                        }
                    }
                }

                GC.KeepAlive(helper);
            }

            public override void Dispose()
            {
                if(_processInfo != null)
                {
                    _processInfo.EditableStatusChanged -= _processInfo_EditableStatusChanged;
                    _processInfo.MovedToNewRecord -= _processInfo_MovedToNewRecord;
                }

                _processInfo = null;

                base.Dispose();
            }

            #endregion

            #region Events

            private void _processInfo_EditableStatusChanged(object sender, EventArgs e) =>
                Refresh();

            private void _processInfo_MovedToNewRecord(object sender, EventArgs e) =>
                Refresh();

            #endregion
        }

        #endregion

        #region InspectionItem

        private class InspectionItem : UltraListViewItem
        {
            public InspectionItem(Data.Datasets.ProcessesDataset.ProcessInspectionsRow processInspection)
                : base(processInspection.StepOrder, ItemArguments(processInspection))
            {
                this.ProcessInspection = processInspection;

                Appearance.Image = processInspection.PartInspectionTypeRow.Active
                    ? Properties.Resources.Inspection_16
                    : Properties.Resources.RoundDashRed_32;
            }

            public Data.Datasets.ProcessesDataset.ProcessInspectionsRow ProcessInspection { get; private set; }

            public int StepOrder
            {
                get
                {
                    return ProcessInspection.StepOrder;
                }
                set
                {
                    Value = value;
                    ProcessInspection.StepOrder = value;
                }
            }

            public void ToggleCOC()
            {
                this.SubItems["COC"].Value = this.ProcessInspection.COCData = !this.ProcessInspection.COCData;
            }

            private static object[] ItemArguments(ProcessesDataset.ProcessInspectionsRow processInspection) =>
                new object[]
                {
                    processInspection.PartInspectionTypeRow.Name,
                    processInspection.COCData,
                    processInspection.PartInspectionTypeRow.IsRevisionNull() ? string.Empty : processInspection.PartInspectionTypeRow.Revision
                };
        }

        #endregion

        #region InspectionItemSorter

        /// <summary>
        /// Customer sorter to sort part process nodes by there step order
        /// </summary>
        internal class InspectionItemSorter : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                if (x == null || y == null)
                    return 0;

                if (x is InspectionItem && y is InspectionItem)
                    return ((InspectionItem)x).StepOrder.CompareTo(((InspectionItem)y).StepOrder);
                else
                    return x.ToString().CompareTo(y.ToString());
            }

            #endregion
        }

        #endregion

        #region ProcessSuggestionItem

        private class ProcessSuggestionItem : INotifyPropertyChanged
        {
            #region Properties

            public ProcessesDataset.ProcessSuggestionRow Row { get; }

            public string Type =>
                Row.Type;

            public string DepartmentId =>
                Row.ProcessRowByFK_ProcessSuggestion_Process_Suggested?.Department ?? "N/A";

            public string ProcessName =>
                Row.ProcessRowByFK_ProcessSuggestion_Process_Suggested?.Name ?? Row.SuggestedProcessID.ToString();

            public string ProcessAliasName =>
                Row.ProcessAliasRow?.Name ?? Row.SuggestedProcessAliasID.ToString();

            public string Status =>
                GetProcessStatusString(Row.ProcessRowByFK_ProcessSuggestion_Process_Suggested);

            public string Condition
            {
                get
                {
                    if (Row.IsConditionTypeNull())
                    {
                        return "(Any)";
                    }
                    if (Row.IsConditionOperatorNull())
                    {
                        return "Invalid";
                    }

                    var rowConditionValue = Row.IsConditionValueNull() ? null : Row.ConditionValue;
                    return $"{Row.ConditionType} {Row.ConditionOperator} '{rowConditionValue }'";
                }
            }

            #endregion

            #region Methods

            public ProcessSuggestionItem(ProcessesDataset.ProcessSuggestionRow row)
            {
                Row = row ?? throw new ArgumentNullException(nameof(row));
            }

            /// <summary>
            /// Raises <see cref="PropertyChanged"/> after updating the
            /// suggestion's row.
            /// </summary>
            public void RaiseRowUpdate()
            {
                OnPropertyChanged(nameof(Type));
                OnPropertyChanged(nameof(DepartmentId));
                OnPropertyChanged(nameof(ProcessName));
                OnPropertyChanged(nameof(ProcessAliasName));
                OnPropertyChanged(nameof(Condition));
                OnPropertyChanged(nameof(Status));
            }

            private void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));

            #endregion

            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        }

        #endregion
    }
}