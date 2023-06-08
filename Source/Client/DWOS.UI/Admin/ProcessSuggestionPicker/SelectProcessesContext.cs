using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ProcessesDatasetTableAdapters;
using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;

namespace DWOS.UI.Admin.ProcessSuggestionPicker
{
    public class SelectProcessesContext : INotifyPropertyChanged
    {
        #region  Fields

        public enum DialogStatus
        {
            PrimaryProcess,
            Suggestions
        }

        private string _manufacturer;
        private Department _selectedDepartment;
        private Process _selectedProcess;
        private ProcessAlias _selectedProcessAlias;
        private DialogStatus _status;

        #endregion

        #region  Properties

        public bool CanContinue => _status != DialogStatus.Suggestions;

        public bool CanFinish => _status == DialogStatus.Suggestions;

        public ICommand Continue { get; }

        public ObservableCollection<Department> Departments { get; } =
            new ObservableCollection<Department>();

        public ICommand Finish { get; }

        public bool HasNoSuggestion => Suggestions.Count == 0;

        public bool HasSuggestions => Suggestions.Count > 0;

        public string Manufacturer
        {
            get => _manufacturer;
            set
            {
                if(_manufacturer != value)
                {
                    _manufacturer = value;
                    OnPropertyChanged(nameof(Manufacturer));
                }
            }
        }

        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                if(_selectedDepartment != value)
                {
                    _selectedDepartment = value;
                    OnPropertyChanged(nameof(SelectedDepartment));

                    SelectedProcess = value?.Processes.FirstOrDefault();
                }
            }
        }

        public Process SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                if(_selectedProcess != value)
                {
                    _selectedProcess = value;
                    OnPropertyChanged(nameof(SelectedProcess));

                    SelectedProcessAlias = value?.Aliases.FirstOrDefault();
                }
            }
        }

        public ProcessAlias SelectedProcessAlias
        {
            get => _selectedProcessAlias;
            set
            {
                if(_selectedProcessAlias != value)
                {
                    _selectedProcessAlias = value;
                    OnPropertyChanged(nameof(SelectedProcessAlias));
                }
            }
        }

        public bool ShowPrimaryProcess =>
            _status == DialogStatus.PrimaryProcess;

        public bool ShowSuggestions =>
            _status == DialogStatus.Suggestions;

        public DialogStatus Status
        {
            get => _status;
            private set
            {
                if(_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                    OnPropertyChanged(nameof(CanContinue));
                    OnPropertyChanged(nameof(CanFinish));
                    OnPropertyChanged(nameof(ShowPrimaryProcess));
                    OnPropertyChanged(nameof(ShowSuggestions));
                }
            }
        }

        public BindingList<ProcessSuggestion> Suggestions { get; } =
            new BindingList<ProcessSuggestion>();

        #endregion

        #region  Methods

        public SelectProcessesContext()
        {
            Continue = new RelayCommand(DoContinue, () => CanContinue);
            Finish = new RelayCommand(DoFinish, () => CanFinish);
        }

        public List<SelectedProcess> GetProcessSelection()
        {
            if(_selectedDepartment == null || _selectedProcess == null || _selectedProcessAlias == null)
            {
                return new List<SelectedProcess>();
            }

            var selectedProcesses = new List<SelectedProcess>();

            // Pre-Processes
            var preProcesses = Suggestions.Where(s => s.IsChecked && s.Type.Trim().ToUpper() == "PRE");
            foreach(var preProcess in preProcesses)
            {
                selectedProcesses.Add(new SelectedProcess
                                      {
                                          ProcessID = preProcess.ProcessId,
                                          ProcessAliasID = preProcess.ProcessAliasId,
                                          AliasName = preProcess.ProcessAliasName,
                                          Department = preProcess.DepartmentName
                                      });
            }

            // Primary Process
            selectedProcesses.Add(new SelectedProcess
                                  {
                                      ProcessID = _selectedProcess.ProcessId,
                                      ProcessAliasID = _selectedProcessAlias.ProcessAliasId,
                                      AliasName = _selectedProcessAlias.Name,
                                      Department = _selectedDepartment.Name
                                  });

            // Post-Processes
            var postProcesses = Suggestions.Where(s => s.IsChecked && s.Type.Trim().ToUpper() == "POST");
            foreach(var postProcess in postProcesses)
            {
                selectedProcesses.Add(new SelectedProcess
                                      {
                                          ProcessID = postProcess.ProcessId,
                                          ProcessAliasID = postProcess.ProcessAliasId,
                                          AliasName = postProcess.ProcessAliasName,
                                          Department = postProcess.DepartmentName
                                      });
            }

            return selectedProcesses;
        }

        public void LoadData(string manufacturer)
        {
            Manufacturer = manufacturer;
            Departments.Clear();
            SelectedDepartment = null;

            Suggestions.Clear();
            OnPropertyChanged(nameof(HasSuggestions));
            OnPropertyChanged(nameof(HasNoSuggestion));

            using(var dsProcess = new ProcessesDataset())
            {
                using (new UsingDataSetLoad(dsProcess))
                {
                    using(var taDept = new d_DepartmentTableAdapter())
                    {
                        taDept.Fill(dsProcess.d_Department);
                    }

                    using(var taProcess = new ProcessTableAdapter())
                    {
                        taProcess.FillBy(dsProcess.Process, true, true); // active & approved
                    }

                    using(var taProcessAlias = new ProcessAliasTableAdapter())
                    {
                        taProcessAlias.FillBy(dsProcess.ProcessAlias, true, true); // active & approved
                    }

                    using(var taProcessSuggestion = new ProcessSuggestionTableAdapter {ClearBeforeFill = false})
                    {
                        foreach(var processRow in dsProcess.Process)
                        {
                            taProcessSuggestion.FillApprovedByPrimaryProcess(dsProcess.ProcessSuggestion, processRow.ProcessID);
                        }
                    }
                }

                foreach(var deptRow in dsProcess.d_Department.OrderBy(d => d.DepartmentID))
                {
                    if(deptRow.GetProcessRows().Length == 0)
                    {
                        continue;
                    }

                    Departments.Add(Department.From(deptRow));
                }

                SelectedDepartment = Departments.FirstOrDefault();
            }
        }

        private void DoContinue()
        {
            if(!CanContinue)
            {
                return;
            }

            Suggestions.Clear();

            foreach(var suggestion in _selectedProcess.Suggestions.Where(IncludeSuggestion))
            {
                Suggestions.Add(suggestion);
            }

            SuggestionsSortChanged?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(HasSuggestions));
            OnPropertyChanged(nameof(HasNoSuggestion));
            Status = DialogStatus.Suggestions;

            bool IncludeSuggestion(ProcessSuggestion suggestedProcess)
            {
                if(suggestedProcess == null)
                {
                    return false;
                }

                switch (suggestedProcess.Condition.Type?.ToUpperInvariant())
                {
                    case "MANUFACTURER":
                        // Assumption: Operator is always '='.
                        return (_manufacturer ?? string.Empty) == (suggestedProcess.Condition.Value ?? string.Empty);
                    default:
                        return true;
                }
            }
        }

        private void DoFinish()
        {
            if(!CanFinish)
            {
                return;
            }

            DialogExit?.Invoke(this, EventArgs.Empty);
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Condition

        public class Condition
        {
            #region  Properties

            public string Operator { get; }

            public string Type { get; }

            public string Value { get; }

            #endregion

            #region  Methods

            public Condition(string type, string op, string value)
            {
                Type = type;
                Operator = op;
                Value = value;
            }

            public static Condition From(ProcessesDataset.ProcessSuggestionRow suggestionRow)
            {
                if(suggestionRow == null)
                {
                    return null;
                }

                var conditionType = suggestionRow.IsConditionTypeNull() ? null : suggestionRow.ConditionType;
                var conditionOperator = suggestionRow.IsConditionOperatorNull() ? null : suggestionRow.ConditionOperator;
                var conditionValue = suggestionRow.IsConditionValueNull() ? null : suggestionRow.ConditionValue;

                return new Condition(conditionType, conditionOperator, conditionValue);
            }

            #endregion
        }

        #endregion

        #region Department

        public class Department
        {
            #region  Properties

            public string Name { get; }

            public List<Process> Processes { get; }

            #endregion

            #region  Methods

            private Department(string name, List<Process> processes)
            {
                Name = name;
                Processes = processes;
            }

            public static Department From(ProcessesDataset.d_DepartmentRow deptRow)
            {
                if(deptRow == null)
                {
                    return null;
                }

                var processes = new List<Process>();
                foreach(var processRow in deptRow.GetProcessRows().OrderBy(p => p.Name))
                {
                    processes.Add(Process.From(processRow));
                }

                return new Department(deptRow.DepartmentID, processes);
            }

            public override string ToString() =>
                Name;

            #endregion
        }

        #endregion

        #region Process

        public class Process
        {
            #region  Properties

            public List<ProcessAlias> Aliases { get; }

            public string Name { get; }

            public int ProcessId { get; }

            public List<ProcessSuggestion> Suggestions { get; }

            #endregion

            #region  Methods

            private Process(int processId, string name, List<ProcessAlias> aliases, List<ProcessSuggestion> suggestions)
            {
                ProcessId = processId;
                Name = name;
                Aliases = aliases;
                Suggestions = suggestions;
            }

            public static Process From(ProcessesDataset.ProcessRow processRow)
            {
                if(processRow == null)
                {
                    return null;
                }

                var aliases = new List<ProcessAlias>();

                foreach(var alias in processRow.GetProcessAliasRows().OrderBy(alias => alias.Name))
                {
                    aliases.Add(ProcessAlias.From(alias));
                }

                var suggestions = new List<ProcessSuggestion>();

                var processSuggestionRows = processRow
                    .GetProcessSuggestionRowsByFK_ProcessSuggestion_Process_Primary();

                foreach(var suggestionRow in processSuggestionRows)
                {
                    var skip = suggestionRow.ProcessRowByFK_ProcessSuggestion_Process_Primary == null ||
                               suggestionRow.ProcessAliasRow == null;

                    if(skip)
                    {
                        continue;
                    }

                    suggestions.Add(ProcessSuggestion.From(suggestionRow));
                }

                return new Process(processRow.ProcessID, processRow.Name, aliases, suggestions);
            }

            public override string ToString() =>
                Name;

            #endregion
        }

        #endregion

        #region ProcessAlias

        public class ProcessAlias
        {
            #region  Properties

            public string Name { get; }

            public int ProcessAliasId { get; }

            #endregion

            #region  Methods

            private ProcessAlias(int processAliasID, string name)
            {
                ProcessAliasId = processAliasID;
                Name = name;
            }

            public static ProcessAlias From(ProcessesDataset.ProcessAliasRow processAliasRow)
            {
                if(processAliasRow == null)
                {
                    return null;
                }

                return new ProcessAlias(processAliasRow.ProcessAliasID, processAliasRow.Name);
            }

            public override string ToString() =>
                Name;

            #endregion
        }

        #endregion

        #region ProcessSuggestion

        public class ProcessSuggestion : INotifyPropertyChanged
        {
            #region  Fields

            private bool _isChecked;

            #endregion

            #region  Properties

            public Condition Condition { get; private set; }

            public string DepartmentName { get; private set; }

            public string Description { get; private set; }

            public bool IsChecked
            {
                get => _isChecked;
                set
                {
                    if(_isChecked != value)
                    {
                        _isChecked = value;
                        OnPropertyChanged(nameof(IsChecked));
                    }
                }
            }

            public int ProcessAliasId { get; private set; }

            public string ProcessAliasName { get; private set; }

            public int ProcessId { get; private set; }

            public string ProcessName { get; private set; }

            public string Type { get; private set; }

            #endregion

            #region  Methods

            private ProcessSuggestion()
            {
            }

            public static ProcessSuggestion From(ProcessesDataset.ProcessSuggestionRow suggestionRow)
            {
                if(suggestionRow == null)
                {
                    return null;
                }

                var suggestedProcessRow = suggestionRow.ProcessRowByFK_ProcessSuggestion_Process_Suggested;
                var suggestedAliasRow = suggestionRow.ProcessAliasRow;

                var description = suggestedProcessRow == null || suggestedProcessRow.IsDescriptionNull()
                    ? string.Empty
                    : suggestedProcessRow.Description;

                return new ProcessSuggestion
                       {
                           IsChecked = true,
                           Type = suggestionRow.Type,
                           ProcessId = suggestionRow.SuggestedProcessID,
                           ProcessAliasId = suggestionRow.SuggestedProcessAliasID,
                           DepartmentName = suggestedProcessRow?.Department,
                           ProcessName = suggestedProcessRow?.Name,
                           Description = description,
                           ProcessAliasName = suggestedAliasRow?.Name,
                           Condition = Condition.From(suggestionRow)
                       };
            }

            private void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        public event EventHandler SuggestionsSortChanged;

        public event EventHandler DialogExit;
    }
}