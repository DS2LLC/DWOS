using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using DWOS.Data.Datasets;
using DWOS.Shared.Utilities;
using Infragistics.Documents.Excel;

namespace DWOS.DataImporter
{
    /// <summary>
    /// Imports and exports contacts.
    /// </summary>
    public class ContactExchange : DataExchange
    {
        #region Methods

        public ContactExchange(IUserNotifier notifier)
            : base(notifier)
        {
        }

        protected override void Export(string file)
        {
            using (var dataset = new CustomersDataset() {EnforceConstraints = false})
            {
                Notifier.ShowNotification("Loading current contacts...");

                using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter())
                {
                    ta.Fill(dataset.Customer);
                }
                using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.ContactTableAdapter())
                {
                    ta.Fill(dataset.Contact);
                }

                dataset.Contact.Columns.Add("CustomerName", typeof(string));

                foreach (var contactRow in dataset.Contact)
                {
                    if (contactRow.CustomerRow != null)
                    {
                        contactRow["CustomerName"] = contactRow.CustomerRow.Name;
                    }
                }

                Notifier.ShowNotification("Loaded {0} contacts".FormatWith(dataset.Contact.Count));

                var report = ExportToWorkbook(dataset.Contact);
                report.Save(file);
            }

            Notifier.ShowNotification("Exported file to {0}".FormatWith(file));

            if (File.Exists(file))
            {
                Process.Start(new ProcessStartInfo(file));
            }
        }

        protected override void Import(string file)
        {
            Notifier.ShowNotification($"Starting contact import from file '{file}'.");
            var dataset = new CustomersDataset {EnforceConstraints = false};

            Notifier.ShowNotification("Loading existing customers from database.");

            // Load current customers
            using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.CustomerTableAdapter())
            {
                ta.Fill(dataset.Customer);
            }

            //load worksheet
            var workBook    = Workbook.Load(file);
            var workSheet   = workBook.Worksheets[0];

            //load field mappings
            var fieldMaps = FieldMapCollection.LoadFieldMappings(dataset.Contact.TableName) ??
                FieldMapCollection.CreateFieldMap(workSheet, dataset.Contact);

            if (fieldMaps.FieldMaps.All(w => w.ColumnName != "Name"))
            {
                var errorMessage = "Missing the following required columns: Name.";
                if (workBook.Worksheets.Count > 1)
                {
                    errorMessage += "\n\n" + "The import workbook has more than one worksheet." +
                        "\n" + "Please delete all worksheets except for the one you want to import.";
                }

                MessageBox.Show(errorMessage); 
                return;
            }

            var customerNameMap = FieldMap.CreateFieldMap(workSheet, "CustomerName");

            if(customerNameMap == null)
            {
                MessageBox.Show("Unable to find the required field 'CustomerName'");
                return;
            }

            Notifier.ShowNotification("Beginning import of each row from the source data.");

            var customerLookups = new List<CustomerMap>(); //customer id to customer name
            var mapper = new ContactFieldMapper(dataset.Contact, Notifier);

            foreach (var worksheetRow in workSheet.Rows.Skip(1))
            {
                var customerName = FieldMapper.GetString(customerNameMap, worksheetRow);

                //ensure has customer name
                if (string.IsNullOrWhiteSpace(customerName))
                {
                    Notifier.ShowNotification($"Row {worksheetRow.Index} has no customer name, skipping row.");
                    continue;
                }

                //get the name mapping to the customer id
                var customerLookup = customerLookups.FirstOrDefault(w => w.Name == customerName);

                if (customerLookup == null)
                {
                    var foundRow = dataset.Customer.FirstOrDefault(cr => cr.Name.Trim() == customerName.Trim());

                    //does not exist in Database
                    if(foundRow == null)
                    {
                        //lets map to an existing customer
                        var selectCustomer = new Controls.SelectCustomer();
                        selectCustomer.LoadData(customerName, dataset.Customer);
                        selectCustomer.ShowDialog();

                        if (selectCustomer.Status == Controls.SelectCustomer.ResultStatus.OK)
                        {
                            foundRow = selectCustomer.SelectedRow as CustomersDataset.CustomerRow;
                        }
                        else if (selectCustomer.Status == Controls.SelectCustomer.ResultStatus.ABORT)
                        {
                            var exportDataPrompt = MessageBox.Show("Do you want to export the data already mapped?", "Export Data",
                                MessageBoxButton.YesNo);

                            if (exportDataPrompt == MessageBoxResult.Yes)
                            {
                                Notifier.ShowNotification("Exporting existing mappings...");
                                break;
                            }

                            Notifier.ShowNotification("Aborting per user...");
                            return;
                        }
                    }

                    customerLookup = new CustomerMap
                    {
                        ID = foundRow?.CustomerID ?? 0,
                        Name = customerName
                    };
                    customerLookups.Add(customerLookup);
                }

                if (customerLookup.ID < 1)
                {
                    continue;
                }

                var contactRow = dataset.Contact.NewContactRow();
                SetDefaults(contactRow);
                mapper.MapAll(fieldMaps.FieldMaps, worksheetRow, contactRow);

                contactRow.CustomerID = customerLookup.ID;

                if (!IsValid(contactRow))
                {
                    Notifier.ShowNotification($"Contact row is not valid with row index: {worksheetRow.Index}");
                }
                else
                {
                    dataset.Contact.AddContactRow(contactRow);
                }
            }

            if (dataset.Contact.Any(d => d.IsNameNull()))
            {
                Notifier.ShowNotification("Warning: One or more contacts does not have a name.");
            }

            var confirmPromptResult = MessageBox.Show(
                $"Are you sure you want to save {dataset.Contact.Count} contacts to the database?",
                "IMPORT", MessageBoxButton.YesNo);

            if (confirmPromptResult == MessageBoxResult.Yes)
            {
                using (var taContact = new Data.Datasets.CustomersDatasetTableAdapters.ContactTableAdapter())
                {
                    Notifier.ShowNotification($"Saving {dataset.Contact.Count} contacts to the database.");

                    var updateCount = taContact.Update(dataset.Contact);
                    Notifier.ShowNotification($"Import completed {updateCount}.");
                }
            }
            else
            {
                Notifier.ShowNotification("Import canceled.");
            }
        }

        private void SetDefaults(CustomersDataset.ContactRow contactRow)
        {
            contactRow.Active = true;
            contactRow.PortalAuthorized = false;
            contactRow.ShippingNotification = false;
        }

        #endregion
    }
}
