using System;
using System.Collections.Generic;
using System.Linq;
using DWOS.Data.Datasets;
using Infragistics.Documents.Excel;
using DWOS.Shared.Utilities;
using DWOS.Data.Customer;

namespace DWOS.DataImporter
{
    /// <summary>
    /// Maps customers from worksheets to the database.
    /// </summary>
    public sealed class CustomerFieldMapper : FieldMapper<Data.Datasets.CustomersDataset.CustomerRow>
    {
        #region Properties

        public Data.Datasets.CustomersDataset.CustomerDataTable Table
        {
            get;
            private set;
        }

        public CustomersDataset.CustomerStatusDataTable StatusTable { get; private set; }

        public IUserNotifier Notifier { get; private set; }

        #endregion

        #region Methods

        public CustomerFieldMapper(CustomersDataset.CustomerDataTable table, CustomersDataset.CustomerStatusDataTable statusTable, IUserNotifier notifier)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table", "table cannot be null");
            }

            if (statusTable == null)
            {
                throw new ArgumentNullException(nameof(statusTable));
            }

            if (notifier == null)
            {
                throw new ArgumentNullException(nameof(notifier));
            }

            Table = table;
            StatusTable = statusTable;
            Notifier = notifier;
        }

        public override void MapAll(IEnumerable<FieldMap> fieldMaps, WorksheetRow worksheetRow, CustomersDataset.CustomerRow dbRow)
        {
            foreach (var fieldMap in fieldMaps)
            {
                var column = Table.Columns[fieldMap.ColumnName];
                dbRow[column] = GetValue(fieldMap, worksheetRow, column.DataType, column.AllowDBNull);
            }

            dbRow.Name = dbRow.Name.Trim();

            if (!dbRow.IsAddress1Null())
            {
                dbRow.Address1 = dbRow.Address1.Trim();
            }

            if (!dbRow.IsAddress2Null())
            {
                dbRow.Address2 = dbRow.Address2.Trim();
            }

            if (!dbRow.IsCityNull())
            {
                dbRow.City = dbRow.City.Trim();
            }

            if (!dbRow.IsStateNull())
            {
                dbRow.State = dbRow.State
                    .Trim()
                    .ToUpper();

                dbRow.CountryID = AddressUtilities.GetCountryId(dbRow.State);
            }
            else
            {
                dbRow.CountryID = AddressUtilities.COUNTRY_ID_UNKNOWN;
            }

            if (!dbRow.IsZipNull())
            {
                dbRow.Zip = dbRow.Zip.Trim();
            }

            if (dbRow.IsCustomerStatusNull() || StatusTable.All(status => status.StatusID != dbRow.CustomerStatus))
            {
                Notifier.ShowNotification($"{dbRow.Name} has an invalid customer status. Using default.");
                dbRow.CustomerStatus = "Good Standing";
            }
        }

        #endregion
    }
}
