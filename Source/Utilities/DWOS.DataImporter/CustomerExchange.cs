using DWOS.Shared.Utilities;
using Infragistics.Documents.Excel;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using DWOS.Data;
using DWOS.Data.Customer;

namespace DWOS.DataImporter
{
    public class CustomerExchange : DataExchange
    {
        #region Properties

        public Data.Datasets.CustomersDataset Dataset { get; set; }

        #endregion

        #region Methods

        public CustomerExchange(IUserNotifier notifier) :
            base(notifier)
        {

        }

        protected override void Export(string file)
        {
            this.Dataset = new Data.Datasets.CustomersDataset() {EnforceConstraints = false};

            Notifier.ShowNotification("Loading customers...");
            using(var ta = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter())
                ta.Fill(Dataset.Customer);
            Notifier.ShowNotification("Loaded {0} customers".FormatWith(Dataset.Customer.Count));

            var report = ExportToWorkbook(Dataset.Customer);
            report.Save(file);
            Notifier.ShowNotification("Exported file to {0}".FormatWith(file));

            if(System.IO.File.Exists(file))
                Process.Start(new ProcessStartInfo(file));
        }

        protected override void Import(string file)
        {
            Notifier.ShowNotification("Strarting customer import from file '{0}'.".FormatWith(file));

            this.Dataset = new Data.Datasets.CustomersDataset() { EnforceConstraints = false };

            using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter())
                ta.Fill(Dataset.Customer);

            using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.CustomerStatusTableAdapter())
            {
                ta.Fill(Dataset.CustomerStatus);
            }

            Notifier.ShowNotification("Found {0} customers currently in the database.".FormatWith(Dataset.Customer.Count));

            //load worksheet
            var workBook = Workbook.Load(file);
            var workSheet = workBook.Worksheets[0];
            
            //load field mappings
            var fieldMaps = FieldMapCollection.LoadFieldMappings(this.Dataset.Customer.TableName) ?? FieldMapCollection.CreateFieldMap(workSheet, this.Dataset.Customer);
            var customerNameMap = fieldMaps.FieldMaps.FirstOrDefault(fm => fm.ColumnName == Dataset.Customer.NameColumn.ColumnName);
            var mapper = new CustomerFieldMapper(Dataset.Customer, Dataset.CustomerStatus, Notifier);

            //foreach worksheet row, skipping header row
            foreach(var row in workSheet.Rows.Skip(1))
            {
                var customerName = FieldMapper.GetString(customerNameMap, row);

                //ensure has customer name
                if(String.IsNullOrWhiteSpace(customerName))
                {
                    _log.Info("Row {0} has no {1}, skipping row.", row.Index, customerNameMap.ColumnName);
                    continue;
                }

                //ensure customer does not already exist
                var foundRow = Dataset.Customer.FirstOrDefault(cr => cr.Name == customerName);

                if(foundRow != null)
                {
                    _log.Info("Customer '{0}' from row {1} already exists in database , skipping row.", foundRow.Name, row.Index);
                    continue;
                }

                //create new customer
                var customerRow = this.Dataset.Customer.NewCustomerRow();
                SetDefaults(customerRow);

                mapper.MapAll(fieldMaps.FieldMaps, row, customerRow);

                if (!IsValid(customerRow))
                {
                    Notifier.ShowNotification("Customer row is not valid with row index: " + row.Index);
                    continue;
                }
                else
                {
                    this.Dataset.Customer.AddCustomerRow(customerRow);

                    // Add customer address
                    var address = this.Dataset.CustomerAddress.NewCustomerAddressRow();
                    address.CustomerRow = customerRow;

                    if (!customerRow.IsAddress1Null())
                    {
                        address.Address1 = customerRow.Address1;
                    }

                    if (!customerRow.IsAddress2Null())
                    {
                        address.Address2 = customerRow.Address2;
                    }

                    if (!customerRow.IsCityNull())
                    {
                        address.City = customerRow.City;
                    }

                    if (!customerRow.IsStateNull())
                    {
                        address.State = customerRow.State;
                        address.CountryID = AddressUtilities.GetCountryId(customerRow.State);

                        address.RequireRepairStatement = customerRow.CountryID !=
                            ApplicationSettings.Current.CompanyCountry;
                    }
                    else
                    {
                        address.RequireRepairStatement = false;
                        address.CountryID = AddressUtilities.COUNTRY_ID_UNKNOWN;
                    }

                    if (!customerRow.IsZipNull())
                    {
                        address.Zip = customerRow.Zip;
                    }

                    address.IsDefault = true;
                    address.Name = customerRow.Name;

                    this.Dataset.CustomerAddress.AddCustomerAddressRow(address);
                }
            }

            var newCustomerCount = this.Dataset.Customer.Count(r => r.RowState == DataRowState.Added);

            if(newCustomerCount < 1)
            {
                Notifier.ShowNotification("No new customers found.");
                return;
            }

            if (MessageBox.Show("Are you sure you want to save the {0} customers to the database?".FormatWith(newCustomerCount), "IMPORT", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var taManager = new Data.Datasets.CustomersDatasetTableAdapters.TableAdapterManager())
                {
                    Notifier.ShowNotification("Saving {0} customers to the database.".FormatWith(newCustomerCount));

                    taManager.CustomerTableAdapter = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter();
                    taManager.CustomerAddressTableAdapter = new Data.Datasets.CustomersDatasetTableAdapters.CustomerAddressTableAdapter();
                    var updateCount = taManager.UpdateAll(this.Dataset);

                    Notifier.ShowNotification("Import completed {0} rows (should be 2 per customer).".FormatWith(updateCount));
                }
            }
            else
                Notifier.ShowNotification("Import canceled.");
        }

        private void SetDefaults(Data.Datasets.CustomersDataset.CustomerRow customer)
        {
            customer.Active = true;
            customer.LeadTime = 10;
            customer.PrintInvoice = false;
            customer.EmailInvoice = false;
            customer.OrderReview = true;
            customer.OrderPriority = "Normal";
            customer.InvoiceLevelID = "Default";
            customer.CountryID = AddressUtilities.COUNTRY_ID_UNKNOWN;
        }

        #endregion
    }
}
