using System;
using System.Collections.Generic;
using DWOS.Data.Datasets;
using DWOS.Shared.Utilities;
using Infragistics.Documents.Excel;

namespace DWOS.DataImporter
{
    /// <summary>
    /// Maps contacts from worksheets to the database.
    /// </summary>
    public sealed class ContactFieldMapper : FieldMapper<CustomersDataset.ContactRow>
    {
        #region Properties

        /// <summary>
        /// Gets the contact table for this instance.
        /// </summary>
        public CustomersDataset.ContactDataTable ContactTable { get; }

        /// <summary>
        /// Gets the notifier for this instance.
        /// </summary>
        public IUserNotifier Notifier { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ContactFieldMapper"/> class.
        /// </summary>
        /// <param name="contactTable"></param>
        /// <param name="notifier"></param>
        public ContactFieldMapper(CustomersDataset.ContactDataTable contactTable, IUserNotifier notifier)
        {
            if (contactTable == null)
            {
                throw new ArgumentNullException(nameof(contactTable));
            }

            if (notifier == null)
            {
                throw new ArgumentNullException(nameof(notifier));
            }

            ContactTable = contactTable;
            Notifier = notifier;
        }

        public override void MapAll(IEnumerable<FieldMap> fieldMaps, WorksheetRow worksheetRow, CustomersDataset.ContactRow dbRow)
        {
            foreach (var fieldMap in fieldMaps)
            {
                var column = ContactTable.Columns[fieldMap.ColumnName];
                dbRow[column] = GetValue(fieldMap, worksheetRow, column.DataType, column.AllowDBNull);
            }

            if (!dbRow.IsNameNull())
            {
                dbRow.Name = dbRow.Name.Trim();
            }

            if (!dbRow.IsEmailAddressNull())
            {
                dbRow.EmailAddress = dbRow.EmailAddress.Trim();
            }

            if (dbRow.ShippingNotification && dbRow.IsEmailAddressNull())
            {
                var contactName = dbRow.IsNameNull() ? string.Empty : dbRow.Name;
                dbRow.ShippingNotification = false;
                var errorMsg = $"{contactName} had Shipping Notifications " +
                    "enabled but did not have an email address. Disabling " +
                    "shipping notifications for this contact.";

                Notifier.ShowNotification(errorMsg);
            }
        }

        #endregion
    }
}
