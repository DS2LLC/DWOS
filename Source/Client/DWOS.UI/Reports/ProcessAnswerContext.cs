using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Reports.ProcessPartsReportTableAdapters;
using DWOS.Reports;
using DWOS.UI.Utilities;
using NLog;
using ProcessPartsReport = DWOS.Data.Reports.ProcessPartsReport;

namespace DWOS.UI.Reports
{
    /// <summary>
    /// Data context for <see cref="ProcessAnswerReportOptions"/>.
    /// </summary>
    public class ProcessAnswerContext : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Occurs when the user accepts the dialog.
        /// </summary>
        public event EventHandler Accepted;

        /// <summary>
        /// Occurs when the UI needs to show an "add question group" dialog.
        /// </summary>
        public event EventHandler ShowQuestionGroupDialog;

        private DateTime? _fromDate;
        private DateTime? _toDate;
        private bool _allCustomersSelected = true;

        private ObservableCollection<object> _selectedProcesses =
            new ObservableCollection<object>();

        private ProcessQuestionGroup _selectedQuestionGroup;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly List<ProcessQuestionGroup> _addedGroups =
            new List<ProcessQuestionGroup>();

        private readonly List<ProcessQuestionGroup> _removedGroups =
            new List<ProcessQuestionGroup>();

        #endregion

        #region Properties

        public DateTime? FromDate
        {
            get => _fromDate;
            set
            {
                if (_fromDate != value)
                {
                    _fromDate = value;
                    OnPropertyChanged(nameof(FromDate));
                }
            }
        }

        public DateTime? ToDate
        {
            get => _toDate;
            set
            {
                if (_toDate != value)
                {
                    _toDate = value;
                    OnPropertyChanged(nameof(ToDate));
                }
            }
        }

        public ObservableCollection<Process> Processes { get; } = new ObservableCollection<Process>();

        /// <summary>
        /// Gets or sets the selected processes for this instance.
        /// </summary>
        /// <remarks>
        /// The Infragistics control used to show this requires this
        /// property have a setter and be of this type.
        /// </remarks>
        public ObservableCollection<object> SelectedProcesses
        {
            get => _selectedProcesses;
            set
            {
                // Binding requires setter
                if (_selectedProcesses != value)
                {
                    _selectedProcesses = value;
                    OnPropertyChanged(nameof(SelectedProcesses));

                    // Clear all process
                    ResetProcessGroups();

                    // Load all
                    foreach (var process in _selectedProcesses.OfType<Process>())
                    {
                        LoadSelectedProcessData(process);
                    }
                }
            }
        }

        public IEnumerable<Process> CastedSelectedProcesses =>
            SelectedProcesses.OfType<Process>();

        public ObservableCollection<ProcessQuestionGroup> QuestionGroups { get; } =
            new ObservableCollection<ProcessQuestionGroup>();

        public ProcessQuestionGroup SelectedQuestionGroup
        {
            get => _selectedQuestionGroup;
            set
            {
                if (_selectedQuestionGroup != value)
                {
                    _selectedQuestionGroup = value;
                    OnPropertyChanged(nameof(SelectedQuestionGroup));
                }
            }
        }

        public string QuestionGroupWarning
        {
            get
            {
                var allProcessIds = CastedSelectedProcesses.SelectMany(p => p.AllProcessIds);
                var questionGroupProcessIds = QuestionGroups
                    .Select(group => group.ProcessId)
                    .ToList();

                if (allProcessIds.Any(id => !questionGroupProcessIds.Contains(id)))
                {
                    return "You have not setup any groups for one or more revisions.";
                }

                return null;
            }
        }

        public bool AllCustomersSelected
        {
            get => _allCustomersSelected;
            set
            {
                if (_allCustomersSelected != value)
                {
                    _allCustomersSelected = value;
                    OnPropertyChanged(nameof(AllCustomersSelected));
                    OnPropertyChanged(nameof(IsCustomerSelectEnabled));
                }
            }
        }

        public bool IsCustomerSelectEnabled => !_allCustomersSelected;

        public ObservableCollection<CustomerViewModel> Customers { get; } =
            new ObservableCollection<CustomerViewModel>();

        public ICommand AddGroup { get; }

        public ICommand RemoveGroup { get; }

        public ICommand Accept { get; }

        #endregion

        #region Methods

        public ProcessAnswerContext()
        {
            var today = DateTime.Today;
            FromDate = today.StartOfDay();
            ToDate = today.EndOfDay();

            AddGroup = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(
                () =>
                {
                    ShowQuestionGroupDialog?.Invoke(this, EventArgs.Empty);
                });

            RemoveGroup = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(
                () =>
                {
                    QuestionGroups.Remove(_selectedQuestionGroup);
                },
                () => _selectedQuestionGroup != null);

            Accept = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(this.DoAccept, this.CanAccept);

            QuestionGroups.CollectionChanged += QuestionGroupsOnCollectionChanged;

            SelectedProcesses.CollectionChanged += SelectedProcesses_CollectionChanged;
        }

        private void DoAccept()
        {
            try
            {
                if (SelectedProcesses.Count > 0)
                {
                    // Save last selection
                    ReportPreferences.LastProcessAnswerReportIds = CastedSelectedProcesses
                        .SelectMany(p => p.AllProcessIds)
                        .ToList();

                    ReportPreferences.Save();
                }

                SaveChanges();

                Accepted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error accepting Process Answer Report dialog.");
            }
        }

        private bool CanAccept()
        {
            return SelectedProcesses.Count > 0
                && Customers.Count > 0
                && (_allCustomersSelected || Customers.Any(c => c.IsSelected));
        }

        public void LoadData()
        {
            // Load Processes
            var processIdDict = new Dictionary<int, Process>();
            using (var dtProcesses = new ProcessesDataset.ProcessDataTable())
            {
                using (var taProcesses = new Data.Datasets.ProcessesDatasetTableAdapters.ProcessTableAdapter())
                {
                    taProcesses.Fill(dtProcesses);
                }

                Processes.Clear();

                foreach (var process in dtProcesses.Where(p => p.Active).OrderBy(p => p.Name))
                {
                    Process processItem = Process.From(process.ProcessID, dtProcesses);
                    Processes.Add(processItem);
                    processIdDict.Add(process.ProcessID, processItem);
                }
            }

            // Select the last selected process
            SelectedProcesses.Clear();

            foreach (var selectedProcessId in ReportPreferences.LastProcessAnswerReportIds)
            {
                processIdDict.TryGetValue(selectedProcessId, out var process);

                if (process != null)
                {
                    SelectedProcesses.Add(process);
                }
            }

            // Load Customers
            using (var dtCustomer = new ProcessesDataset.CustomerDataTable())
            {
                using (var taCustomer = new Data.Datasets.ProcessesDatasetTableAdapters.CustomerTableAdapter())
                {
                    taCustomer.Fill(dtCustomer);
                }

                Customers.Clear();

                foreach (var customerRow in dtCustomer.OrderBy(c => c.Name))
                {
                    var customer = new CustomerViewModel(customerRow.CustomerID, customerRow.Name);
                    Customers.Add(customer);
                }
            }
        }

        public void LoadSelectedProcessData(Process process)
        {
            if (process == null)
            {
                return;
            }

            ProcessQuestionGroupTableAdapter taQuestionGroup = null;
            ProcessQuestionSummaryTableAdapter taQuestionSummary = null;

            var dtQuestionGroup =
                new ProcessPartsReport.ProcessQuestionGroupDataTable();

            var dtQuestionSummary =
                new ProcessPartsReport.ProcessQuestionSummaryDataTable();

            try
            {
                QuestionGroups.CollectionChanged -= QuestionGroupsOnCollectionChanged;

                taQuestionGroup = new ProcessQuestionGroupTableAdapter();
                taQuestionSummary = new ProcessQuestionSummaryTableAdapter();

                SelectedQuestionGroup = null;

                foreach (var processRevision in process.AllRevisions)
                {
                    // Question Groups
                    taQuestionGroup.FillByProcess(dtQuestionGroup, processRevision.ProcessId);
                    taQuestionSummary.FillByProcess(dtQuestionSummary, processRevision.ProcessId);

                    foreach (var groupRow in dtQuestionGroup)
                    {
                        var questionsInGroup = dtQuestionSummary
                            .Where(q => q.ProcessStepID == groupRow.ProcessStepID)
                            .ToList();

                        var includeAll = questionsInGroup.All(q => !q.IncludeInProcessGroup); // Setup prior to 17.3.1 release

                        var questionGroup = new ProcessQuestionGroup
                        {
                            ProcessId = processRevision.ProcessId,
                            ProcessName = processRevision.Name,
                            ProcessStepId = groupRow.ProcessStepID,
                            ProcessStepOrder = groupRow.ProcessStepOrder,
                            ProcessStepName = groupRow.ProcessStepName,
                            ProcessQuestionId = groupRow.IsProcessQuestionIDNull()
                                ? (int?)null
                                : groupRow.ProcessQuestionID,
                            ProcessQuestionOrder = groupRow.IsProcessQuestionOrderNull()
                                ? (decimal?)null
                                : groupRow.ProcessQuestionOrder,
                            ProcessQuestionName = groupRow.IsProcessQuestionNameNull()
                                ? null
                                : groupRow.ProcessQuestionName,
                            Questions = questionsInGroup
                                .Select(q => new ProcessQuestionGroup.GroupQuestion
                                {
                                    QuestionId = q.ProcessQuestionID,
                                    Name = q.QuestionName,
                                    Include = q.IncludeInProcessGroup || q.IdentifiesProcessGroup || includeAll
                                }).ToList()
                        };

                        QuestionGroups.Add(questionGroup);

                        if (includeAll)
                        {
                            // Force dialog to save this group - otherwise,
                            // IncludeInProcessGroup will not be saved for use in
                            // the report
                            _addedGroups.Add(questionGroup);
                        }
                    }
                }
            }
            finally
            {
                QuestionGroups.CollectionChanged += QuestionGroupsOnCollectionChanged;
                taQuestionGroup?.Dispose();
                taQuestionSummary?.Dispose();

                dtQuestionGroup?.Dispose();
                dtQuestionSummary?.Dispose();
            }

        }

        private void OnProcessUnselected(Process process)
        {
            try
            {
                QuestionGroups.CollectionChanged -= QuestionGroupsOnCollectionChanged;

                var addedGroups = new List<ProcessQuestionGroup>();
                var removedGroups = new List<ProcessQuestionGroup>();
                var groups = new List<ProcessQuestionGroup>();
                foreach (var processId in process.AllProcessIds)
                {
                    addedGroups.AddRange(_addedGroups.Where(g => g.ProcessId == processId));
                    removedGroups.AddRange(_removedGroups.Where(g => g.ProcessId == processId));
                    groups.AddRange(QuestionGroups.Where(g => g.ProcessId == processId));
                }

                foreach (var addedGroup in addedGroups)
                {
                    _addedGroups.Remove(addedGroup);
                }

                foreach (var removedGroup in removedGroups)
                {
                    _removedGroups.Remove(removedGroup);
                }

                foreach (var group in groups)
                {
                    QuestionGroups.Remove(group);
                }
            }
            finally
            {
                QuestionGroups.CollectionChanged += QuestionGroupsOnCollectionChanged;
            }
        }

        private void ResetProcessGroups()
        {
            try
            {
                QuestionGroups.CollectionChanged -= QuestionGroupsOnCollectionChanged;
                _addedGroups.Clear();
                _removedGroups.Clear();
            }
            finally
            {
                QuestionGroups.CollectionChanged += QuestionGroupsOnCollectionChanged;
            }
        }

        private void SaveChanges()
        {
            ProcessQuestionGroupTableAdapter taQuestionGroup = null;
            ProcessQuestionSummaryTableAdapter taQuestionSummary = null;
            try
            {
                taQuestionGroup = new ProcessQuestionGroupTableAdapter();
                taQuestionSummary = new ProcessQuestionSummaryTableAdapter();

                var processesString = string.Join(", ",
                    CastedSelectedProcesses.Select(p => p.ProcessId));

                _logger.Info($"Saving Process Question Group changes for Processes: {processesString}");

                // Save removed groups
                foreach (var removedGroup in _removedGroups)
                {
                    if (removedGroup.ProcessQuestionId.HasValue)
                    {
                        var questionIdRemoved = removedGroup.ProcessQuestionId.Value;
                        taQuestionGroup.UpdateGroupIdentity(questionIdRemoved, false);
                    }

                    foreach (var question in removedGroup.Questions)
                    {
                        taQuestionSummary.UpdateIncludeInGroup(question.QuestionId, false);
                    }
                }

                // Save added groups
                foreach (var addedGroup in _addedGroups)
                {
                    if (addedGroup.ProcessQuestionId.HasValue)
                    {
                        var questionIdAdded = addedGroup.ProcessQuestionId.Value;
                        taQuestionGroup.UpdateGroupIdentity(questionIdAdded, true);
                    }

                    foreach (var question in addedGroup.Questions)
                    {
                        taQuestionSummary.UpdateIncludeInGroup(question.QuestionId, question.Include);
                    }
                }

                _logger.Info($"Successfully saved Process Question Group changes for Processes: {processesString}");
            }
            finally
            {
                taQuestionGroup?.Dispose();
                taQuestionSummary?.Dispose();
            }
        }

        public ProcessAnswerReport GenerateReport()
        {
            return new ProcessAnswerReport
            {
                ProcessIds = CastedSelectedProcesses
                    .SelectMany(p => p.AllProcessIds)
                    .ToList(),

                CustomerIds = _allCustomersSelected
                    ? null
                    : Customers
                        .Where(c => c.IsSelected)
                        .Select(c => c.CustomerId)
                        .ToList(),

                FromDate = FromDate?.StartOfDay() ?? DateTime.MinValue,
                ToDate = ToDate?.EndOfDay() ?? DateTime.MaxValue,
            };
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region Events

        private void QuestionGroupsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var addNewItems = false;
            var removeOldItems = false;

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    addNewItems = true;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    removeOldItems = true;
                    break;
                case NotifyCollectionChangedAction.Replace:
                    addNewItems = true;
                    removeOldItems = true;
                    break;
                case NotifyCollectionChangedAction.Reset:
                    removeOldItems = true;
                    break;
            }

            if (addNewItems)
            {
                var newGroups = args.NewItems?.OfType<ProcessQuestionGroup>() ?? Enumerable.Empty<ProcessQuestionGroup>();
                foreach (var newGroup in newGroups)
                {
                    _addedGroups.Add(newGroup);
                    _removedGroups.Remove(g => g.ProcessQuestionId == newGroup.ProcessQuestionId);
                }
            }

            if (removeOldItems)
            {
                var removedGroups = args.OldItems?.OfType<ProcessQuestionGroup>() ?? Enumerable.Empty<ProcessQuestionGroup>();
                foreach (var removedGroup in removedGroups)
                {
                    _removedGroups.Add(removedGroup);
                    _addedGroups.Remove(g => g.ProcessQuestionId == removedGroup.ProcessQuestionId);
                }
            }

            // Check for processes that are missing groups
            OnPropertyChanged(nameof(QuestionGroupWarning));
        }

        private void SelectedProcesses_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    // Clear all process
                    ResetProcessGroups();

                    // Load all
                    foreach (var process in CastedSelectedProcesses)
                    {
                        LoadSelectedProcessData(process);
                    }

                    return;
                }
                var oldProcesses = e.OldItems?.OfType<Process>()
                    ?? Enumerable.Empty<Process>();

                foreach (var oldProcess in oldProcesses)
                {
                    OnProcessUnselected(oldProcess);
                }

                var newProcesses = e.NewItems?.OfType<Process>()
                    ?? Enumerable.Empty<Process>();

                foreach (var newProcess in newProcesses)
                {
                    LoadSelectedProcessData(newProcess);
                }

                // Refresh warning related to process data
                OnPropertyChanged(nameof(QuestionGroupWarning));
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error handling process selection change event.");
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Process

        public class Process
        {
            #region Properties

            public int ProcessId { get; set; }

            public string Name { get; set; }

            public Process PreviousRevision { get; set; }

            public IEnumerable<int> AllProcessIds
            {
                get
                {
                    var prevRevision = PreviousRevision;

                    var processIds = new List<int> { ProcessId };

                    while (prevRevision != null)
                    {
                        processIds.Add(prevRevision.ProcessId);
                        prevRevision = prevRevision.PreviousRevision;
                    }

                    return processIds;
                }
            }

            public IEnumerable<Process> AllRevisions
            {
                get
                {
                    yield return this;

                    var prevRevision = PreviousRevision;

                    while (prevRevision != null)
                    {
                        yield return prevRevision;
                        prevRevision = prevRevision.PreviousRevision;
                    }
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Returns a new <see cref="Process"/> instance using an ID and a
            /// data table.
            /// </summary>
            /// <remarks>
            /// This fetches the process row from <paramref name="dtProcess"/>
            /// every time because the process row's parent object is null
            /// even when the ParentID isn't.
            /// </remarks>
            /// <param name="processId"></param>
            /// <param name="dtProcess"></param>
            /// <returns></returns>
            public static Process From(int processId, ProcessesDataset.ProcessDataTable dtProcess)
            {
                if (dtProcess == null)
                {
                    throw new ArgumentNullException(nameof(dtProcess));
                }

                var process = dtProcess.FindByProcessID(processId);

                if (process == null)
                {
                    return null;
                }

                // Assumption: Process's parent/previous revision was loaded.
                return new Process
                {
                    ProcessId = process.ProcessID,
                    Name = process.IsRevisionNull()
                            ? process.Name
                            : $"{process.Name} Rev. {process.Revision}",
                    PreviousRevision = process.IsParentIDNull()
                        ? null
                        : From(process.ParentID, dtProcess)
                };
            }

            #endregion
        }

        #endregion

        #region CustomerViewModel

        public class CustomerViewModel : ViewModelBase
        {
            #region Fields

            private bool _isSelected;

            #endregion

            #region Properties

            public int CustomerId { get; }

            public string Name { get; }

            public bool IsSelected
            {
                get => _isSelected;
                set => Set(nameof(IsSelected), ref _isSelected, value);
            }

            #endregion

            #region Methods

            public CustomerViewModel(int customerId, string name)
            {
                CustomerId = customerId;
                Name = name;
            }

            #endregion
        }

        #endregion
    }
}
