using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.AutomatedWorkOrderTool.ViewModel;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    internal class CustomerManager : ICustomerManager
    {
        #region Fields

        private readonly List<CustomerViewModel> _customers =
            new List<CustomerViewModel>();

        #endregion

        #region Properties

        public IDataManager DataManager { get; }

        #endregion

        #region Methods

        public CustomerManager(IDataManager dataManager)
        {
            if (dataManager != null)
            {
                DataManager = dataManager;
            }
        }

        #endregion

        #region ICustomerManager Members

        public IEnumerable<CustomerViewModel> CurrentCustomers => _customers;

        public void Load()
        {
            _customers.Clear();
            using (var dsAwot = new AwotDataSet())
            {
                DataManager.LoadInitialData(dsAwot);

                _customers.AddRange(dsAwot.Customer.Where(c => c.GetOSPFormatRows().Length > 0).Select(CustomerViewModel.From));
            }
        }

        public void Add(CustomerViewModel customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            _customers.Add(customer);
        }

        public IEnumerable<CustomField> GetCustomFields(CustomerViewModel customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            using (var dtCustomField = new AwotDataSet.CustomFieldDataTable())
            {
                DataManager.LoadCustomFields(dtCustomField, customer.Id);

                return dtCustomField
                    .Select(row => new CustomField
                    {
                        CustomFieldId = row.CustomFieldID,
                        Name = row.Name,
                        Description = row.IsDescriptionNull() ? null : row.Description,
                        IsRequired = row.Required,
                        IsVisible = row.IsVisible,
                        DefaultValue = row.IsDefaultValueNull() ? null : row.DefaultValue
                    })
                    .ToList();
            }
        }

        public bool UsesOrderReview(CustomerViewModel customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.CustomerSummaryTableAdapter())
            {
                return ta.GetOrderReview(customer.Id) ?? false;
            }
        }

        public string GetDefaultValue(CustomerViewModel customer, string field)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            if (string.IsNullOrEmpty(field))
            {
                throw new ArgumentNullException(nameof(field));
            }

            using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.Customer_FieldsTableAdapter())
                return ta.GetDefaultValue(field, customer.Id);
        }

        #endregion
    }
}
