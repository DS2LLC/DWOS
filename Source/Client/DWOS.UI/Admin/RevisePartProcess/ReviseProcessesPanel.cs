using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ProcessesDatasetTableAdapters;
using DWOS.Shared;
using DWOS.Shared.Wizard;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Admin.RevisePartProcess
{
    public partial class ReviseProcessesPanel : UserControl, IWizardPanel
    {
        #region Fields

        private RevisePartProcessController _controller;
        private readonly ProcessesData _processesData = new ProcessesData();
        private readonly GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("ReviseProcesses", new UltraGridBandSettings());

        #endregion

        #region Methods

        public ReviseProcessesPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region IWizardPanel Members

        public string Title => "Outdated Processes";

        public string SubTitle => string.Empty;

        public Action<IWizardPanel> OnValidStateChanged { get; set; }

        public Control PanelControl => this;

        public bool IsValid { get; private set; }

        public void Initialize(WizardController controller)
        {
            _controller = controller as RevisePartProcessController;
        }

        public void OnFinished()
        {
            _controller = null;
            _processesData.Dispose();
        }

        public void OnMoveFrom()
        {
        }

        public void OnMoveTo()
        {
            _processesData.LoadData();
            grdProcesses.DataSource = _processesData.Processes;

            if (_processesData.Processes.Count == 0)
            {
                // Skip process revision
                btnRevise.Enabled = false;
                IsValid = true;
                OnValidStateChanged?.Invoke(this);
            }
        }

        #endregion

        #region Events

        private void btnRevise_Click(object sender, EventArgs e)
        {
            try
            {
                btnRevise.Enabled = false;
                _controller.Processes = _processesData.Revise();
                progressBar.Value = 100;
                IsValid = true;
                OnValidStateChanged?.Invoke(this);
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error revising processes.", exc);
            }
        }


        private void grdProcesses_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                grdProcesses.AfterColPosChanged -= grdProcesses_AfterColPosChanged;
                grdProcesses.AfterSortChange -= grdProcesses_AfterSortChange;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(grdProcesses.DisplayLayout.Bands[0]);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing export grid layout.");
            }
            finally
            {
                grdProcesses.AfterColPosChanged += grdProcesses_AfterColPosChanged;
                grdProcesses.AfterSortChange += grdProcesses_AfterSortChange;
            }
        }

        private void grdProcesses_AfterColPosChanged(object sender, Infragistics.Win.UltraWinGrid.AfterColPosChangedEventArgs e)
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

        private void grdProcesses_AfterSortChange(object sender, Infragistics.Win.UltraWinGrid.BandEventArgs e)
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

        #endregion

        #region ProcessesData

        private class ProcessesData : IDisposable
        {
            #region Fields

            private readonly ProcessesDataset _dsProcesses = new ProcessesDataset();
            private readonly TableAdapterManager _taManager;
            private readonly PartInspectionTypeTableAdapter _taInspectionTypes = new PartInspectionTypeTableAdapter();
            private bool _isLoaded;

            #endregion

            #region Properties

            public List<ProcessData> Processes { get; } = new List<ProcessData>();

            #endregion

            public ProcessesData()
            {
                _taManager = new TableAdapterManager
                {
                    d_DepartmentTableAdapter = new d_DepartmentTableAdapter { ClearBeforeFill = false },
                    ProcessTableAdapter = new ProcessTableAdapter { ClearBeforeFill = false },
                    ProcessRequisiteTableAdapter = new ProcessRequisiteTableAdapter { ClearBeforeFill = false },
                    ProcessAliasTableAdapter = new ProcessAliasTableAdapter { ClearBeforeFill = false },
                    CustomerProcessAliasTableAdapter = new CustomerProcessAliasTableAdapter { ClearBeforeFill = false },
                    ProcessInspectionsTableAdapter = new ProcessInspectionsTableAdapter { ClearBeforeFill = false },
                    ProcessStepsTableAdapter = new ProcessStepsTableAdapter { ClearBeforeFill = false },
                    ProcessStepDocumentLinkTableAdapter = new ProcessStepDocumentLinkTableAdapter { ClearBeforeFill = false },
                    ProcessStepConditionTableAdapter = new ProcessStepConditionTableAdapter { ClearBeforeFill = false },
                    ProcessQuestionTableAdapter = new ProcessQuestionTableAdapter { ClearBeforeFill = false },
                    ProcessQuestionFieldTableAdapter = new ProcessQuestionFieldTableAdapter { ClearBeforeFill = false },
                    ProcessAliasDocumentLinkTableAdapter = new ProcessAliasDocumentLinkTableAdapter { ClearBeforeFill = false },
                    ProcessDocumentLinkTableAdapter = new ProcessDocumentLinkTableAdapter { ClearBeforeFill = false },
                    ProcessProductClassTableAdapter = new ProcessProductClassTableAdapter {  ClearBeforeFill = false },
                    d_ProcessCategoryTableAdapter = new d_ProcessCategoryTableAdapter { ClearBeforeFill = false },
                    ListsTableAdapter =  new ListsTableAdapter { ClearBeforeFill = false }
                };
            }

            public void LoadData()
            {
                if (_isLoaded)
                {
                    return;
                }

                try
                {
                    using (new UsingDataSetLoad(_dsProcesses))
                    {
                        using (var taCustomer = new CustomerTableAdapter())
                        {
                            taCustomer.Fill(_dsProcesses.Customer);
                        }

                        _taInspectionTypes.Fill(_dsProcesses.PartInspectionType);
                        _taManager.ListsTableAdapter.Fill(_dsProcesses.Lists);

                        using (var taUnits = new NumericUnitsTableAdapter())
                        {
                            taUnits.Fill(_dsProcesses.NumericUnits);
                        }

                        _taManager.d_DepartmentTableAdapter.Fill(_dsProcesses.d_Department);
                        _taManager.ProcessTableAdapter.FillActiveWithRevisedInspections(_dsProcesses.Process);
                        _taManager.d_ProcessCategoryTableAdapter.Fill(_dsProcesses.d_ProcessCategory);

                        var outdatedProcesses = _dsProcesses
                            .Process
                            .ToList();

                        foreach (var process in outdatedProcesses)
                        {
                            Processes.Add(ProcessData.From(process));

                            _taManager.ProcessAliasTableAdapter.FillByProcess(_dsProcesses.ProcessAlias, process.ProcessID);
                            _taManager.CustomerProcessAliasTableAdapter.FillByProcess(_dsProcesses.CustomerProcessAlias, process.ProcessID);
                            _taManager.ProcessInspectionsTableAdapter.FillByProcess(_dsProcesses.ProcessInspections, process.ProcessID);
                            _taManager.ProcessStepsTableAdapter.FillBy(_dsProcesses.ProcessSteps, process.ProcessID);
                            _taManager.ProcessStepDocumentLinkTableAdapter.FillByProcess(_dsProcesses.ProcessStepDocumentLink, process.ProcessID);
                            _taManager.ProcessStepConditionTableAdapter.FillBy(_dsProcesses.ProcessStepCondition, process.ProcessID);
                            _taManager.ProcessQuestionTableAdapter.FillBy(_dsProcesses.ProcessQuestion, process.ProcessID);
                            _taManager.ProcessQuestionFieldTableAdapter.FillByProcess(_dsProcesses.ProcessQuestionField, process.ProcessID);
                            _taManager.ProcessAliasDocumentLinkTableAdapter.FillByProcess(_dsProcesses.ProcessAliasDocumentLink, process.ProcessID);
                            _taManager.ProcessDocumentLinkTableAdapter.FillByProcess(_dsProcesses.ProcessDocumentLink, process.ProcessID);
                            _taManager.ProcessRequisiteTableAdapter.FillByProcess(_dsProcesses.ProcessRequisite, process.ProcessID);
                            _taManager.ProcessProductClassTableAdapter.FillByProcess(_dsProcesses.ProcessProductClass, process.ProcessID);

                            // Load processes used in any constraints -
                            // otherwise, this crashes with a
                            // constraints -related error
                            foreach (var constraint in process.GetProcessRequisiteRowsByFK_ProcessRequisite_ProcessParent())
                            {
                                if (_dsProcesses.Process.FindByProcessID(constraint.ChildProcessID) == null)
                                {
                                    _taManager.ProcessTableAdapter.FillByProcess(_dsProcesses.Process, constraint.ChildProcessID);
                                }
                            }

                            foreach (var constraint in process.GetProcessRequisiteRowsByFK_ProcessRequisite_ProcessChild())
                            {
                                if (_dsProcesses.Process.FindByProcessID(constraint.ParentProcessID) == null)
                                {
                                    _taManager.ProcessTableAdapter.FillByProcess(_dsProcesses.Process, constraint.ParentProcessID);
                                }
                            }
                        }

                        // Fix For VSTS #28133
                        // If any condition happens to point to a question
                        // that don't belong to the process and is not
                        // loaded, a constraints error breaks functionality.
                        var invalidConditions = _dsProcesses.ProcessStepCondition
                            .Where(psc => !psc.IsProcessQuestionIdNull() && psc.ProcessQuestionRow == null)
                            .ToList();

                        foreach (var invalidCondition in invalidConditions)
                        {
                            invalidCondition.Delete();
                            invalidCondition.AcceptChanges();
                        }
                    }
                }
                catch (ConstraintException)
                {
                    LogManager.GetCurrentClassLogger().Warn("DataSet Errors: " + _dsProcesses.GetDataErrors());
                    throw;
                }

                _isLoaded = true;
            }

            public RevisionResults<ProcessData> Revise()
            {
                try
                {
                    foreach (var process in Processes)
                    {
                        ReviseProcess(_dsProcesses.Process.FindByProcessID(process.ProcessId), process.NextRevision);
                    }

                    _taManager.UpdateAll(_dsProcesses);

                    // Either all updates fail or none of them do
                    return new RevisionResults<ProcessData>
                    {
                        Failure = new List<ProcessData>(),
                        Success = Processes
                    };

                }
                catch (System.Data.SqlClient.SqlException)
                {
                    LogManager.GetCurrentClassLogger().Warn("DataSet Errors: " + _dsProcesses.GetDataErrors());
                    throw;
                }
            }

            private void ReviseProcess(ProcessesDataset.ProcessRow processRow, string revision)
            {
                if (processRow == null)
                {
                    return;
                }

                // Copy process
                var disallowedRelations = new List<string>
                {
                    // Delete double relationship "FK_ProcessStepCondition_ProcessSteps"; keep the other side "FK_ProcessQuestion_ProcessStepCondition"
                    "FK_ProcessQuestion_ProcessStepCondition",

                    // Manually copy requisites
                    "FK_ProcessRequisite_ProcessChild",
                    "FK_ProcessRequisite_ProcessParent",

                    // Fix for VSTS #23006 - will set parent relationship later
                    "FK_Process_Process"
                };

                var revisionProxy = CopyCommand.CopyRows(processRow);

                revisionProxy.Remove(w => disallowedRelations.Contains(w.ParentRelation));

                List<DataRowProxyResults> results = new List<DataRowProxyResults>();
                var processRevision = DataNode<DataRow>.AddPastedDataRows(revisionProxy, _dsProcesses.Process, results) as ProcessesDataset.ProcessRow;

                if (processRevision != null)
                {
                    processRow.Active = false;
                    foreach (var processStep in processRevision.GetProcessStepsRows())
                    {
                        foreach (var condition in processStep.GetProcessStepConditionRows())
                        {
                            if (condition.IsProcessQuestionIdNull())
                            {
                                continue;
                            }

                            //need to point back to new processquestionid, not new one
                            var original = results.FirstOrDefault(c => c.Proxy.OriginalPrimaryKey == condition.ProcessQuestionId.ToString());

                            if (original != null)
                                condition.ProcessQuestionRow = original.Row as ProcessesDataset.ProcessQuestionRow;
                        }
                    }

                    // Revise
                    processRevision.Revision = revision;
                    processRevision.Active = true;
                    processRevision.ParentID = processRow.ProcessID;
                    processRevision.SetFrozenByNull();
                    processRevision.SetFrozenDateNull();
                    processRevision.Frozen = false;

                    // Update revised inspections
                    foreach (var processInspection in processRevision.GetProcessInspectionsRows())
                    {
                        if (processInspection.PartInspectionTypeRow.Active)
                        {
                            continue;
                        }

                        var revisedPartInspectionTypeId = _taInspectionTypes.GetRevisedPartInspectionType(processInspection.PartInspectionTypeID);

                        if (revisedPartInspectionTypeId.HasValue)
                        {
                            processInspection.PartInspectionTypeID = revisedPartInspectionTypeId.Value;
                        }
                        else
                        {
                            LogManager.GetCurrentClassLogger().Warn(
                                $"Could not find active revision for PartInspectionType {processInspection.PartInspectionTypeID}");
                        }
                    }

                    // Update requisites
                    foreach (var childConstraint in processRow.GetProcessRequisiteRowsByFK_ProcessRequisite_ProcessParent())
                    {
                        // processRow is parent
                        var newConstraint = _dsProcesses.ProcessRequisite.NewProcessRequisiteRow();
                        newConstraint.ParentProcessID = processRevision.ProcessID;
                        newConstraint.ChildProcessID = childConstraint.ChildProcessID;
                        newConstraint.Hours = childConstraint.Hours;
                        _dsProcesses.ProcessRequisite.AddProcessRequisiteRow(newConstraint);
                    }

                    foreach (var parentConstraint in processRow.GetProcessRequisiteRowsByFK_ProcessRequisite_ProcessChild())
                    {
                        // processRow is child
                        var newConstraint = _dsProcesses.ProcessRequisite.NewProcessRequisiteRow();
                        newConstraint.ParentProcessID = parentConstraint.ParentProcessID;
                        newConstraint.ChildProcessID = processRevision.ProcessID;
                        newConstraint.Hours = parentConstraint.Hours;
                        _dsProcesses.ProcessRequisite.AddProcessRequisiteRow(newConstraint);
                    }
                }
            }

            #region IDisposable Members

            public void Dispose()
            {
                _taManager.Dispose();
                _dsProcesses.Dispose();
                _taInspectionTypes.Dispose();
            }

            #endregion
        }

        #endregion
    }
}
