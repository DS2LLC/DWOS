using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DWOS.UI.Admin.Processes.ViewModels
{
    /// <summary>
    /// View Model for <see cref="ProcessImportDepartmentDialog"/>.
    /// </summary>
    internal class ProcessImportDepartmentViewModel : ViewModelBase
    {
        #region Fields

        public event EventHandler Accepted;

        private string _processName;
        private string _importedDepartment;
        private string _selectedDepartment;
        private bool _createNewDepartment;

        #endregion

        #region Properties

        public ICommand Accept { get; }

        public string ProcessName
        {
            get => _processName;
            private set => Set(nameof(ProcessName), ref _processName, value);
        }

        public string ImportedDepartment
        {
            get => _importedDepartment;
            private set => Set(nameof(ImportedDepartment), ref _importedDepartment, value);
        }

        public ObservableCollection<string> Departments { get; } =
            new ObservableCollection<string>();

        public string SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(nameof(SelectedDepartment), ref _selectedDepartment, value);
        }

        public bool CreateNewDepartment
        {
            get => _createNewDepartment;
            set
            {
                if (Set(nameof(CreateNewDepartment), ref _createNewDepartment, value))
                {
                    RaisePropertyChanged(nameof(UseExistingDepartment));
                }
            }
        }

        public bool UseExistingDepartment => !_createNewDepartment;

        #endregion

        #region Methods

        public ProcessImportDepartmentViewModel()
        {
            Accept = new RelayCommand(
                () => Accepted?.Invoke(this, EventArgs.Empty),
                () => !string.IsNullOrEmpty(_selectedDepartment));
        }

        public void LoadData(string processName, string importedDepartment, IEnumerable<string> departments)
        {
            ProcessName = processName;
            ImportedDepartment = importedDepartment;

            SelectedDepartment = null;
            Departments.Clear();

            foreach (var dept in departments.OrderBy(d => d))
            {
                Departments.Add(dept);
            }

            // Try to select first existing department that starts with the
            // same letter as the imported department.
            if (!string.IsNullOrEmpty(importedDepartment))
            {
                var firstCharacter = importedDepartment.Substring(0, 1);
                SelectedDepartment = Departments.FirstOrDefault(dept => dept.StartsWith(firstCharacter))
                    ?? Departments.FirstOrDefault();
            }
            else
            {
                SelectedDepartment = Departments.FirstOrDefault();
            }
        }

        #endregion
    }
}
