using System.Collections.Generic;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.AutomatedWorkOrderTool.ViewModel;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    /// <summary>
    /// Manages <see cref="CustomerViewModel"/> instances.
    /// </summary>
    public interface ICustomerManager
    {
        /// <summary>
        /// Gets a collection representing the current list of customers.
        /// </summary>
        /// <remarks>
        /// This list may contain customers that have not been saved; this
        /// allows users to add customers then manufacturers in two steps.
        /// </remarks>
        IEnumerable<CustomerViewModel> CurrentCustomers { get; }

        /// <summary>
        /// Loads customers from persistence.
        /// </summary>
        void Load();

        /// <summary>
        /// Adds a customer to this instance.
        /// </summary>
        /// <param name="newCustomer">
        /// Instance representing a newly-added customer.
        /// </param>
        void Add(CustomerViewModel newCustomer);

        IEnumerable<CustomField> GetCustomFields(CustomerViewModel customer);

        bool UsesOrderReview(CustomerViewModel customer);

        string GetDefaultValue(CustomerViewModel customer, string field);
    }
}
