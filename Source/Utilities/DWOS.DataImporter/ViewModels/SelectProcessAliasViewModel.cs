using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DWOS.Data.Datasets;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace DWOS.DataImporter.ViewModels
{
    public class SelectProcessAliasViewModel : ViewModelBase
    {
        #region Fields

        public event EventHandler Accepted;
        private string _sourceProcessName;
        private PartsDataset.ProcessRow _selectedProcess;
        private PartsDataset.ProcessAliasRow _selectedProcessAlias;

        #endregion

        #region Properties

        public string SourceProcessName
        {
            get => _sourceProcessName;
            internal set => Set(nameof(SourceProcessName), ref _sourceProcessName, value);
        }

        public ObservableCollection<PartsDataset.ProcessRow> Processes { get; } =
            new ObservableCollection<PartsDataset.ProcessRow>();

        public PartsDataset.ProcessRow SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                if (Set(nameof(SelectedProcess), ref _selectedProcess, value))
                {
                    // Set process aliases
                    SelectedProcessAlias = null;
                    ProcessAliases.Clear();

                    if (_selectedProcess != null)
                    {
                        foreach (var aliasRow in _selectedProcess.GetProcessAliasRows().OrderBy(alias => alias.Name))
                        {
                            ProcessAliases.Add(aliasRow);
                        }
                    }
                }
            }
        }

        public ObservableCollection<PartsDataset.ProcessAliasRow> ProcessAliases { get; } =
            new ObservableCollection<PartsDataset.ProcessAliasRow>();

        public PartsDataset.ProcessAliasRow SelectedProcessAlias
        {
            get => _selectedProcessAlias;
            set => Set(nameof(SelectedProcessAlias), ref _selectedProcessAlias, value);
        }

        public ICommand Accept { get; }

        #endregion

        #region Methods

        public SelectProcessAliasViewModel()
        {
            Accept = new RelayCommand(DoAccept, CanAccept);
        }

        private void DoAccept() =>
            Accepted?.Invoke(this, EventArgs.Empty);

        private bool CanAccept() =>
            _selectedProcessAlias != null;

        public void LoadData(string processName, PartsDataset dataset)
        {
            SourceProcessName = processName;

            // Show approved processes
            Processes.Clear();

            var processRows = dataset.Process
                .Where(p => p.Active && !p.IsIsApprovedNull() && p.IsApproved)
                .OrderBy(p => p.ProcessName);

            foreach (var processRow in processRows)
            {
                Processes.Add(processRow);
            }

            // Select best process match (if available)
            ProcessAliases.Clear();

            SelectedProcess = Processes.FirstOrDefault(p => p.Name == processName);
        }

        #endregion
    }
}
