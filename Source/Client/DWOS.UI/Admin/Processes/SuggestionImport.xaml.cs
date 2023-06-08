using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.PartsDatasetTableAdapters;
using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using NLog;

namespace DWOS.UI.Admin.Processes
{
    /// <summary>
    ///     Interaction logic for SuggestionImport.xaml
    /// </summary>
    public partial class SuggestionImport
    {
        #region  Properties

        private DialogContext ViewModel =>
            DataContext as DialogContext;

        #endregion

        #region  Methods

        public SuggestionImport()
        {
            InitializeComponent();
            DataContext = new DialogContext();
        }

        public List<Process> GetSelectedProcesses()
        {
            var allPartProcesses = ViewModel?.SelectedPart?.Processes;

            if(allPartProcesses == null)
            {
                return new List<Process>();
            }

            return allPartProcesses
                .Where(p => p.IncludeProcess)
                .ToList();
        }

        public void LoadData(int processId, string name)
        {
            ViewModel?.LoadData(processId, name);
        }

        #endregion

        #region Events

        private void SuggestionImport_OnLoaded(object sender, RoutedEventArgs e)
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
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading suggestions import dialog.");
            }
        }

        private void SuggestionImport_OnUnloaded(object sender, RoutedEventArgs e)
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
                LogManager.GetCurrentClassLogger().Error(exc, "Error unloading suggestions import dialog.");
            }
        }

        private void VmOnCompleted(object sender, EventArgs eventArgs)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error completing suggestions import dialog.");
            }
        }

        #endregion

        #region Customer

        public class Customer
        {
            #region  Properties

            public int CustomerId { get; }

            public bool IsActive { get; }

            public string Name { get; }

            public List<Part> Parts { get; } =
                new List<Part>();

            #endregion

            #region  Methods

            private Customer(int customerId, string name, bool isActive)
            {
                CustomerId = customerId;
                Name = name;
                IsActive = isActive;
            }

            public static Customer From(PartsDataset.CustomerRow customerRow)
            {
                if(customerRow == null)
                {
                    return null;
                }

                return new Customer(customerRow.CustomerID, customerRow.Name, customerRow.Active);
            }

            public override string ToString() =>
                Name;

            #endregion
        }

        #endregion

        #region DialogContext

        private class DialogContext : INotifyPropertyChanged
        {
            #region  Fields

            private string _processName;
            private Customer _selectedCustomer;
            private Part _selectedPart;

            #endregion

            #region  Properties

            public ICommand Accept { get; }

            public ObservableCollection<Customer> Customers { get; } =
                new ObservableCollection<Customer>();

            public string ProcessName
            {
                get => _processName;
                set
                {
                    if(_processName != value)
                    {
                        _processName = value;
                        OnPropertyChanged(nameof(ProcessName));
                    }
                }
            }

            public Customer SelectedCustomer
            {
                get => _selectedCustomer;
                set
                {
                    if(_selectedCustomer != value)
                    {
                        _selectedCustomer = value;
                        OnPropertyChanged(nameof(SelectedCustomer));

                        SelectedPart = value?.Parts.FirstOrDefault();
                    }
                }
            }

            public Part SelectedPart
            {
                get => _selectedPart;
                set
                {
                    if(_selectedPart != value)
                    {
                        _selectedPart = value;
                        OnPropertyChanged(nameof(SelectedPart));
                    }
                }
            }

            #endregion

            #region  Methods

            public DialogContext()
            {
                Accept = new RelayCommand(DoAccept, CanAccept);
            }

            public void LoadData(int processId, string processName)
            {
                ProcessName = processName;

                var customerDict = new Dictionary<int, Customer>();
                using(var dsParts = new PartsDataset {EnforceConstraints = false})
                {
                    using(var taPart = new PartTableAdapter())
                    {
                        taPart.FillActiveByProcess(dsParts.Part, processId);
                    }

                    var taCustomer = new CustomerTableAdapter {ClearBeforeFill = false};
                    var taPartProcess = new PartProcessTableAdapter {ClearBeforeFill = false};
                    var taProcess = new ProcessTableAdapter {ClearBeforeFill = false};
                    var taProcessAlias = new ProcessAliasTableAdapter {ClearBeforeFill = false};

                    try
                    {
                        foreach(var partRow in dsParts.Part.OrderBy(p => p.Name))
                        {
                            if(!customerDict.TryGetValue(partRow.CustomerID, out var customer))
                            {
                                taCustomer.FillByCustomer(dsParts.Customer, partRow.CustomerID);
                                customer = Customer.From(dsParts.Customer.FindByCustomerID(partRow.CustomerID));
                                customerDict[partRow.CustomerID] = customer;
                            }

                            if(customer == null || !customer.IsActive)
                            {
                                // Skip inactive or invalid customers
                                continue;
                            }

                            taPartProcess.FillByPart(dsParts.PartProcess, partRow.PartID);
                            taProcess.FillByPartID(dsParts.Process, partRow.PartID);
                            taProcessAlias.FillBy(dsParts.ProcessAlias, partRow.PartID);

                            customer.Parts.Add(Part.From(partRow, processId));
                        }
                    }
                    finally
                    {
                        taCustomer.Dispose();
                        taPartProcess.Dispose();
                        taProcess.Dispose();
                        taProcessAlias.Dispose();
                    }
                }

                foreach(var activeCustomer in customerDict.Values.Where(c => c.IsActive).OrderBy(c => c.Name))
                {
                    Customers.Add(activeCustomer);
                }

                SelectedCustomer = Customers.FirstOrDefault();
            }

            private bool CanAccept() =>
                _selectedCustomer != null && _selectedPart != null && _selectedPart.Processes.Any(p => p.IncludeProcess);

            private void DoAccept()
            {
                Completed?.Invoke(this, EventArgs.Empty);
            }

            private void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            public event EventHandler Completed;
        }

        #endregion

        #region Part

        public class Part
        {
            #region  Properties

            public string Name { get; }
            public int PartId { get; }

            public List<Process> Processes { get; }

            #endregion

            #region  Methods

            private Part(int partId, string name, List<Process> processes)
            {
                PartId = partId;
                Name = name;
                Processes = processes;
            }

            public static Part From(PartsDataset.PartRow partRow, int primaryProcessId)
            {
                if(partRow == null)
                {
                    return null;
                }

                var processes = new List<Process>();

                var type = "Pre";
                foreach(var processRow in partRow.GetPartProcessRows().OrderBy(p => p.StepOrder))
                {
                    if(processRow.ProcessID == primaryProcessId)
                    {
                        type = "Post";
                        continue;
                    }

                    processes.Add(Process.From(processRow, type));
                }

                return new Part(partRow.PartID, partRow.Name, processes);
            }

            public override string ToString() =>
                Name;

            #endregion
        }

        #endregion

        #region Process

        public class Process : INotifyPropertyChanged
        {
            #region  Fields

            private bool _includeProcess;

            #endregion

            #region  Properties

            public bool IncludeProcess
            {
                get => _includeProcess;
                set
                {
                    if(_includeProcess != value)
                    {
                        _includeProcess = value;
                        OnPropertyChanged(nameof(IncludeProcess));
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

            private Process()
            {
            }

            public static Process From(PartsDataset.PartProcessRow processRow, string type)
            {
                if(processRow == null || string.IsNullOrEmpty(type))
                {
                    return null;
                }

                return new Process
                       {
                           ProcessId = processRow.ProcessID,
                           ProcessName = processRow.ProcessRow?.ProcessName ?? "N/A",
                           ProcessAliasId = processRow.ProcessAliasID,
                           ProcessAliasName = processRow.ProcessAliasRow?.Name ?? "N/A",
                           Type = type
                       };
            }

            private void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion
    }
}