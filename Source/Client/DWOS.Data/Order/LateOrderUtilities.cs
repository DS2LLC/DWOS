using DWOS.Data.Datasets;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DWOS.Data.Order
{
    /// <summary>
    /// Contains utility methods related to late orders.
    /// </summary>
    public static class LateOrderUtilities
    {
        /// <summary>
        /// Retrieves a list of IDs from the database for contacts receiving
        /// late order notifications.
        /// </summary>
        /// <param name="customerId">
        /// Customer ID to use when retrieving contacts.
        /// </param>
        /// <returns>
        /// A list of contact IDs.
        /// </returns>
        public static ICollection<int> GetContactIdsForNotification(int customerId)
        {
            var contactIds = new List<int>();

            // Add primary contacts for customer
            using (var dtContact = new CustomersDataset.ContactDataTable())
            {
                using (var taContact = new ContactTableAdapter())
                {
                    taContact.FillBy(dtContact, customerId);
                }

                contactIds.AddRange(dtContact
                    .Where(c => c.Active && c.LateOrderNotification)
                    .Select(c => c.ContactID));
            }

            // Add secondary contacts for customer
            if (ApplicationSettings.Current.AllowAdditionalCustomersForContacts)
            {
                // Load data
                using (var dsCustomer = new CustomersDataset { EnforceConstraints = false })
                {
                    using (var taContact = new ContactTableAdapter())
                    {
                        taContact.FillBySecondaryCustomer(dsCustomer.Contact, customerId);
                    }

                    using (var taAdditionalCustomer = new ContactAdditionalCustomerTableAdapter())
                    {
                        taAdditionalCustomer.FillBySecondaryCustomer(dsCustomer.ContactAdditionalCustomer, customerId);
                    }

                    using (var taCustomer = new CustomerTableAdapter { ClearBeforeFill = false })
                    {
                        foreach (var contactCustomerId in dsCustomer.Contact.Select(c => c.CustomerID))
                        {
                            taCustomer.FillBy(dsCustomer.Customer, contactCustomerId);
                        }
                    }

                    // Add contacts
                    var secondaryContactIds = dsCustomer.Contact
                        .Where(contact => contact.Active && contact.CustomerRow.Active && CanReceiveNotification(contact, customerId))
                        .Select(contact => contact.ContactID);

                    contactIds.AddRange(secondaryContactIds);
                }
            }

            return contactIds;
        }

        /// <summary>
        /// Returns a value that indicates if a contact is setup to receive
        /// late order notifications for a secondary customer.
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        private static bool CanReceiveNotification(CustomersDataset.ContactRow contact, int customerId)
        {
            var additionalCustomerItem = contact.GetContactAdditionalCustomerRows()
                .FirstOrDefault(addCustomer => addCustomer.CustomerID == customerId);

            return contact.LateOrderNotification
                && additionalCustomerItem != null
                && additionalCustomerItem.IncludeInLateOrderNotifications;
        }
    }
}
