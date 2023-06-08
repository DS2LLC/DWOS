using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Shared.Wizard;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using NLog;

namespace DWOS.UI.Admin.FieldMigration
{
    public class FieldMigrationController : WizardController
    {
        #region Fields

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public override string WizardTitle => "Field Migration Wizard";

        public string CustomFieldName { get; set; }

        public bool IsMigrationFinished { get; private set; }

        #endregion

        #region Methods

        public WizardDialog NewDialog()
        {
            var wizard = new WizardDialog();
            var panels = new List<IWizardPanel>
            {
                new FieldSelectPanel(),
                new CustomFieldSelectPanel(),
                new MigratePanel(),
                new SummaryPanel()
            };

            wizard.InitializeWizard(this, panels);
            return wizard;
        }

        public MigrationSummary GetUpdateCounts()
        {
            var orderCount = GetUpdateOrders(CustomFieldName).Count;

            var customerCount = GetUpdateCustomerIds(CustomFieldName)
                .Count;

            return new MigrationSummary(orderCount, customerCount);
        }

        private static List<OrdersDataSet.OrderRow> GetUpdateOrders(string customFieldName)
        {
            if (string.IsNullOrEmpty(customFieldName))
            {
                return new List<OrdersDataSet.OrderRow>();
            }

            OrderTableAdapter taOrders = null;
            OrderProductClassTableAdapter taProductClass = null;

            try
            {
                taOrders = new OrderTableAdapter();
                taProductClass = new OrderProductClassTableAdapter();

                var dtOrders = taOrders
                    .GetOrdersWithCustomField(customFieldName);

                var orders = new List<OrdersDataSet.OrderRow>(dtOrders.Count);

                foreach (var order in dtOrders)
                {
                    if (!taProductClass.HasProductClass(order.OrderID).GetValueOrDefault())
                    {
                        orders.Add(order);
                    }
                }

                return orders;
            }
            finally
            {
                taOrders?.Dispose();
                taProductClass?.Dispose();
            }
        }

        private static List<int> GetUpdateCustomerIds(string customFieldName)
        {
            Data.Datasets.CustomersDatasetTableAdapters.CustomFieldTableAdapter taField = null;
            Data.Datasets.CustomersDatasetTableAdapters.Customer_FieldsTableAdapter taCustomerFields = null;

            try
            {
                taField = new Data.Datasets.CustomersDatasetTableAdapters.CustomFieldTableAdapter();
                taCustomerFields = new Data.Datasets.CustomersDatasetTableAdapters.Customer_FieldsTableAdapter();
                var dtCustomFields = taField.GetByName(customFieldName);

                var customFields = dtCustomFields
                    .ToList();

                var customerIds = new List<int>();
                foreach (var customField in customFields)
                {
                    var customerId = customField.CustomerID;

                    if (!customerIds.Contains(customerId) && taCustomerFields.GetField(customerId, "Product Class").Count == 0)
                    {
                        customerIds.Add(customerId);
                    }
                }

                return customerIds;
            }
            finally
            {
                taField?.Dispose();
                taCustomerFields?.Dispose();
            }
        }

        public void Update()
        {
            IsMigrationFinished = false;
            AddProductClasses(CustomFieldName);
            AddProductClassFields(CustomFieldName);
            HideCustomFields(CustomFieldName);
            IsMigrationFinished = true;
        }

        private static void AddProductClasses(string customFieldName)
        {
            var orders = GetUpdateOrders(customFieldName);
            if (orders == null || string.IsNullOrEmpty(customFieldName))
            {
                return;
            }

            OrderCustomFieldsTableAdapter taOrderCustomFields = null;
            OrderProductClassTableAdapter taOrderProductClass = null;
            OrdersDataSet.OrderProductClassDataTable dtOrderProductClass = null;

            try
            {
                taOrderCustomFields = new OrderCustomFieldsTableAdapter();
                taOrderProductClass = new OrderProductClassTableAdapter();
                dtOrderProductClass = new OrdersDataSet.OrderProductClassDataTable();

                // Add a product class for each order
                foreach (var order in orders)
                {
                    var fieldValue = taOrderCustomFields.GetValue(order.OrderID, customFieldName);

                    if (string.IsNullOrEmpty(fieldValue))
                    {
                        continue;
                    }

                    var productClassRow = dtOrderProductClass.NewOrderProductClassRow();
                    productClassRow.OrderID = order.OrderID;
                    productClassRow.ProductClass = fieldValue;
                    dtOrderProductClass.AddOrderProductClassRow(productClassRow);
                }

                taOrderProductClass.Update(dtOrderProductClass);
            }
            finally
            {
                taOrderCustomFields?.Dispose();
                taOrderProductClass?.Dispose();
                dtOrderProductClass?.Dispose();
            }
        }

        private static void AddProductClassFields(string customFieldName)
        {
            var customerIds = GetUpdateCustomerIds(customFieldName);
            if (customerIds == null)
            {
                return;
            }

            Data.Datasets.CustomersDatasetTableAdapters.CustomFieldTableAdapter taCustomField = null;
            Data.Datasets.CustomersDatasetTableAdapters.Customer_FieldsTableAdapter taCustomerFields = null;
            Data.Datasets.CustomersDatasetTableAdapters.FieldsTableAdapter taFields = null;

            try
            {
                taCustomField = new Data.Datasets.CustomersDatasetTableAdapters.CustomFieldTableAdapter();
                taCustomerFields = new Data.Datasets.CustomersDatasetTableAdapters.Customer_FieldsTableAdapter { ClearBeforeFill = false };
                taFields = new Data.Datasets.CustomersDatasetTableAdapters.FieldsTableAdapter();

                var systemField = taFields.GetData().FirstOrDefault(f => f.Name == "Product Class");

                if (systemField == null)
                {
                    Log.Error("Field not found - Product Class");
                    return;
                }

                var dtCustomerFields = new CustomersDataset.Customer_FieldsDataTable();

                // Get field
                foreach (var customerId in customerIds)
                {
                    var field = taCustomField.GetByCustomer(customerId).FirstOrDefault(f => f.Name == customFieldName);

                    if (field == null)
                    {
                        continue;
                    }

                    taCustomerFields.FillField(dtCustomerFields, customerId, customFieldName);
                    var customerField = dtCustomerFields.FirstOrDefault(f => f.CustomerID == customerId);

                    if (customerField == null)
                    {
                        // Create new one
                        customerField = dtCustomerFields.NewCustomer_FieldsRow();
                        customerField.CustomerID = customerId;
                        customerField.FieldID = systemField.FieldID;

                        customerField.Required = field.Required;

                        if (!field.IsDefaultValueNull())
                        {
                            customerField.DefaultVaue = field.DefaultValue;
                        }

                        dtCustomerFields.AddCustomer_FieldsRow(customerField);
                    }
                    else
                    {
                        // This case shouldn't typically happen.
                        customerField.Required = field.Required;

                        if (!field.IsDefaultValueNull())
                        {
                            customerField.DefaultVaue = field.DefaultValue;
                        }
                    }
                }

                taCustomerFields.Update(dtCustomerFields);
            }
            finally
            {
                taCustomField?.Dispose();
                taCustomerFields?.Dispose();
                taFields?.Dispose();
            }

        }

        private static void HideCustomFields(string customFieldName)
        {
            if (string.IsNullOrEmpty(customFieldName))
            {
                return;
            }

            Data.Datasets.CustomersDatasetTableAdapters.CustomFieldTableAdapter taField = null;

            try
            {
                taField = new Data.Datasets.CustomersDatasetTableAdapters.CustomFieldTableAdapter();
                var dtCustomFields = taField.GetByName(customFieldName);

                foreach (var customField in dtCustomFields)
                {
                    customField.IsVisible = false;
                    customField.Required = false;
                }

                taField.Update(dtCustomFields);
            }
            finally
            {
                taField?.Dispose();
            }
        }

        public override void Finished()
        {

        }

        #endregion

        #region MigrationSummary

        public class MigrationSummary
        {
            #region Properties

            public int Orders { get; }

            public int Customers { get; }

            #endregion

            #region Methods

            public MigrationSummary(int orders, int customers)
            {
                Orders = orders;
                Customers = customers;
            }

            #endregion
        }
        #endregion
    }
}
