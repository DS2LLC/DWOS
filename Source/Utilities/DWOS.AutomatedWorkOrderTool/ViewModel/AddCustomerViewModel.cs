using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.AutomatedWorkOrderTool.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class AddCustomerViewModel : ViewModelBase
    {
        #region Fields

        private CustomerViewModel _selectedCustomer;

        #endregion

        #region Properties

        public IDataManager DataManager { get; }
        public ICustomerManager CustomerManager { get; }
        public ObservableCollection<CustomerViewModel> Customers { get; } =
            new ObservableCollection<CustomerViewModel>();

        public CustomerViewModel SelectedCustomer
        {
            get => _selectedCustomer;
            set => Set(nameof(SelectedCustomer), ref _selectedCustomer, value);
        }

        #endregion

        #region Methods

        public AddCustomerViewModel(IDataManager dataManager, ICustomerManager customerManager, IMessenger messenger)
            : base(messenger)
        {
            DataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
            CustomerManager = customerManager ?? throw new ArgumentNullException(nameof(customerManager));
        }

        public void LoadData()
        {
            var existingCustomerIds = new HashSet<int>(CustomerManager.CurrentCustomers.Select(c => c.Id));

            Customers.Clear();

            var customersToShow = new List<CustomerViewModel>();
            using (var dataset = new AwotDataSet())
            {
                DataManager.LoadInitialData(dataset);

                foreach (var customerRow in dataset.Customer)
                {
                    var skipCustomer = !customerRow.Active ||
                                       customerRow.GetOSPFormatRows().Length > 0 ||
                                       existingCustomerIds.Contains(customerRow.CustomerID);

                    if (skipCustomer)
                    {
                        continue;
                    }

                    customersToShow.Add(CustomerViewModel.From(customerRow));
                }
            }

            var sortedCustomers = customersToShow.OrderBy(c => c.Name).ToList();
            foreach (var customer in sortedCustomers)
            {
                Customers.Add(customer);
            }

            SelectedCustomer = sortedCustomers.FirstOrDefault();
        }

        #endregion
    }
}
