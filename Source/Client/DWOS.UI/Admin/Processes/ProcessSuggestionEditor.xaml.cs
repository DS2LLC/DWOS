using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using NLog;

namespace DWOS.UI.Admin.Processes
{
    /// <summary>
    ///     Interaction logic for ProcessSuggestionEditor.xaml
    /// </summary>
    public partial class ProcessSuggestionEditor
    {
        #region  Fields

        public enum SuggestionType
        {
            Pre,
            Post
        }

        #endregion

        #region  Properties

        public string PrimaryProcessName
        {
            get => ViewModel?.PrimaryProcessName;
            set
            {
                var vm = ViewModel;

                if(vm != null)
                    vm.PrimaryProcessName = value;
            }
        }

        public string SelectedConditionOperator
        {
            get
            {
                var vm = ViewModel;

                if(string.IsNullOrWhiteSpace(vm?.SelectedConditionType.Value))
                {
                    return null;
                }

                return vm.SelectedConditionOperator;
            }
        }

        public string SelectedConditionType =>
            ViewModel?.SelectedConditionType.Value;

        public string SelectedConditionValue
        {
            get
            {
                var vm = ViewModel;

                if(string.IsNullOrWhiteSpace(vm?.SelectedConditionType.Value))
                {
                    return null;
                }

                return vm.ConditionValue;
            }
        }

        public Process SelectedProcess =>
            ViewModel?.SelectedProcess;

        public ProcessAlias SelectedProcessAlias =>
            ViewModel?.SelectedProcessAlias;

        public SuggestionType SelectedSuggestionType =>
            ViewModel?.SelectedSuggestionType ?? SuggestionType.Pre;

        private EditorDataContext ViewModel =>
            DataContext as EditorDataContext;

        #endregion

        #region  Methods

        public ProcessSuggestionEditor()
        {
            InitializeComponent();
            DataContext = new EditorDataContext();
            Icon = Properties.Resources.Process_32.ToWpfImage();
        }

        public void LoadData(int processId, ProcessesDataset dsProcesses, List<string> manufacturers)
        {
            ViewModel?.LoadData(processId, dsProcesses, manufacturers);
        }

        public void LoadSuggestion(ProcessesDataset.ProcessSuggestionRow row)
        {
            ViewModel?.LoadSuggestion(row);
        }

        #endregion

        #region Events

        private void ProcessSuggestionEditor_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if(vm != null)
                {
                    vm.Completed += VmOnCompleted;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading process suggestion editor.");
            }
        }

        private void ProcessSuggestionEditor_OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if(vm != null)
                {
                    vm.Completed -= VmOnCompleted;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error unloading process suggestion editor.");
            }
        }

        private void VmOnCompleted(object sender, EventArgs eventArgs)
        {
            DialogResult = true;
        }

        #endregion

        #region ConditionType

        public class ConditionType
        {
            #region  Properties

            public string DisplayText { get; }

            public string Value { get; }

            #endregion

            #region  Methods

            public ConditionType(string displayText, string value)
            {
                DisplayText = displayText;
                Value = value;
            }

            #endregion
        }

        #endregion

        #region Department

        public class Department
        {
            #region  Properties

            public string DepartmentId { get; }

            public List<Process> Processes { get; }

            #endregion

            #region  Methods

            public Department(string departmentID, List<Process> processes)
            {
                DepartmentId = departmentID;
                Processes = processes;
            }

            public override string ToString()
            {
                return DepartmentId;
            }

            #endregion
        }

        #endregion

        #region EditorDataContext

        private class EditorDataContext : INotifyPropertyChanged
        {
            #region  Fields

            private const string OPERATOR_EQUALS = "=";
            private string _conditionValue;
            private string _primaryProcessName;
            private string _selectedConditionOperator = OPERATOR_EQUALS;
            private ConditionType _selectedConditionType;
            private Department _selectedDepartment;
            private Process _selectedProcess;
            private ProcessAlias _selectedProcessAlias;
            private SuggestionType _selectedSuggestionType = SuggestionType.Pre;

            #endregion

            #region  Properties

            public ICommand Accept { get; }

            public List<string> ConditionOperators { get; } =
                new List<string>
                {
                    OPERATOR_EQUALS
                };

            public List<ConditionType> ConditionTypes { get; } =
                new List<ConditionType>
                {
                    new ConditionType("(Any)", null),
                    new ConditionType("Manufacturer", "Manufacturer")
                };

            public string ConditionValue
            {
                get => _conditionValue;
                set
                {
                    if(_conditionValue != value)
                    {
                        _conditionValue = value;
                        OnNotifyPropertyChanged(nameof(ConditionValue));
                    }
                }
            }

            public ObservableCollection<string> ConditionValueSuggestions { get; } =
                new ObservableCollection<string>();

            public ObservableCollection<Department> Departments { get; } =
                new ObservableCollection<Department>();

            public bool HasConditionType => !string.IsNullOrWhiteSpace(_selectedConditionType?.Value);

            public string PrimaryProcessName
            {
                get => _primaryProcessName;
                set
                {
                    if(_primaryProcessName != value)
                    {
                        _primaryProcessName = value;
                        OnNotifyPropertyChanged(nameof(PrimaryProcessName));
                    }
                }
            }

            public string SelectedConditionOperator
            {
                get => _selectedConditionOperator;
                set
                {
                    if(_selectedConditionOperator != value)
                    {
                        _selectedConditionOperator = value;
                        OnNotifyPropertyChanged(nameof(SelectedConditionOperator));
                    }
                }
            }

            public ConditionType SelectedConditionType
            {
                get => _selectedConditionType;
                set
                {
                    if(_selectedConditionType != value)
                    {
                        _selectedConditionType = value;
                        OnNotifyPropertyChanged(nameof(SelectedConditionType));
                        OnNotifyPropertyChanged(nameof(HasConditionType));

                        if(string.IsNullOrWhiteSpace(value?.Value))
                        {
                            // Reset other condition fields
                            SelectedConditionOperator = OPERATOR_EQUALS;
                            ConditionValue = null;
                        }
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
                        OnNotifyPropertyChanged(nameof(SelectedDepartment));

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
                        OnNotifyPropertyChanged(nameof(SelectedProcess));

                        SelectedProcessAlias = value?.ProcessAliases.FirstOrDefault();
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
                        OnNotifyPropertyChanged(nameof(SelectedProcessAlias));
                    }
                }
            }


            public SuggestionType SelectedSuggestionType
            {
                get => _selectedSuggestionType;
                set
                {
                    if(_selectedSuggestionType != value)
                    {
                        _selectedSuggestionType = value;
                        OnNotifyPropertyChanged(nameof(SelectedSuggestionType));
                    }
                }
            }

            public List<SuggestionType> SuggestionTypes { get; } =
                new List<SuggestionType>
                {
                    SuggestionType.Pre,
                    SuggestionType.Post
                };

            #endregion

            #region  Methods

            public EditorDataContext()
            {
                Accept = new RelayCommand(DoAccept, CanAccept);
                _selectedConditionType = ConditionTypes.FirstOrDefault();
            }

            public void LoadData(int processId, ProcessesDataset dsProcesses, List<string> manufacturers)
            {
                // Clear existing elements (if any)
                SelectedDepartment = null;
                Departments.Clear();

                if(dsProcesses == null)
                    return;

                // Load Departments
                foreach(var departmentRow in dsProcesses.d_Department.OrderBy(dept => dept.DepartmentID))
                {
                    var activeProcesses = departmentRow
                        .GetProcessRows()
                        .Where(p => p.Active && p.ProcessID != processId)
                        .OrderBy(p => p.Name)
                        .ToList();

                    if(activeProcesses.Count == 0)
                        continue;

                    var processes = new List<Process>();
                    foreach(var processRow in departmentRow.GetProcessRows().Where(p => p.Active).OrderBy(p => p.Name))
                    {
                        if (processRow.ProcessID == processId)
                        {
                            // Do not allow users to add a suggestion for the current process
                            continue;
                        }

                        var processAliases = new List<ProcessAlias>();

                        foreach(var processAliasRow in processRow.GetProcessAliasRows().OrderBy(alias => alias.Name))
                            processAliases.Add(new ProcessAlias(processAliasRow.ProcessAliasID, processAliasRow.Name));

                        processes.Add(new Process(processRow.ProcessID, processRow.Name, processAliases));
                    }

                    Departments.Add(new Department(departmentRow.DepartmentID, processes));
                }

                // Selecting a department also selects a process
                SelectedDepartment = Departments.FirstOrDefault();

                // Load value suggestions - for now, these are only manufacturers
                if(manufacturers != null)
                {
                    foreach(var manufacturer in manufacturers.OrderBy(m => m))
                    {
                        ConditionValueSuggestions.Add(manufacturer);
                    }
                }
            }

            public void LoadSuggestion(ProcessesDataset.ProcessSuggestionRow row)
            {
                if(row == null)
                {
                    return;
                }

                // Assumptions:
                //    - Data is already loaded
                //    - This is a valid row with a process and alias

                var suggestedProcess = row.ProcessRowByFK_ProcessSuggestion_Process_Suggested;
                var suggestedAlias = row.ProcessAliasRow;

                SelectedSuggestionType = (SuggestionType) Enum.Parse(typeof(SuggestionType), row.Type);
                SelectedDepartment = Departments.FirstOrDefault(d => d.DepartmentId == suggestedProcess.Department);
                SelectedProcess = _selectedDepartment?.Processes.FirstOrDefault(p => p.ProcessId == suggestedProcess.ProcessID);
                SelectedProcessAlias = _selectedProcess?.ProcessAliases.FirstOrDefault(alias => alias.ProcessAliasId == suggestedAlias.ProcessAliasID);

                if(row.IsConditionTypeNull())
                {
                    SelectedConditionType = ConditionTypes.FirstOrDefault(ct => string.IsNullOrWhiteSpace(ct.Value));
                    SelectedConditionOperator = OPERATOR_EQUALS;
                    ConditionValue = null;
                }
                else
                {
                    SelectedConditionType = ConditionTypes.FirstOrDefault(ct => ct.Value == row.ConditionType);

                    var rowConditionOperator = row.IsConditionOperatorNull() ? null : row.ConditionOperator;
                    SelectedConditionOperator = ConditionOperators.FirstOrDefault(op => op == rowConditionOperator);
                    ConditionValue = row.IsConditionValueNull() ? null : row.ConditionValue;
                }
            }

            private bool CanAccept()
            {
                return _selectedProcess != null && _selectedProcessAlias != null;
            }

            private void DoAccept()
            {
                Completed?.Invoke(this, EventArgs.Empty);
            }

            private void OnNotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            public event EventHandler Completed;
        }

        #endregion

        #region Process

        public class Process
        {
            #region  Properties

            public string Name { get; }

            public List<ProcessAlias> ProcessAliases { get; }

            public int ProcessId { get; }

            #endregion

            #region  Methods

            public Process(int processId, string name, List<ProcessAlias> processAliases)
            {
                ProcessId = processId;
                Name = name;
                ProcessAliases = processAliases;
            }

            public override string ToString()
            {
                return Name;
            }

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

            public ProcessAlias(int processAliasId, string name)
            {
                ProcessAliasId = processAliasId;
                Name = name;
            }

            public override string ToString()
            {
                return Name;
            }

            #endregion
        }

        #endregion
    }
}