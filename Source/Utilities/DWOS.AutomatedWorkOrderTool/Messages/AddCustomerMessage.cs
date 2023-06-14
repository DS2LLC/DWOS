using System;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.AutomatedWorkOrderTool.Messages
{
    /// <summary>
    /// Request to add a customer to the list of customers.
    /// </summary>
    public class AddCustomerMessage : MessageBase
    {
        #region Properties

        public CustomerViewModel NewCustomer { get; }

        #endregion

        #region Methods

        public AddCustomerMessage(CustomerViewModel newCustomer)
        {
            NewCustomer = newCustomer ?? throw new ArgumentNullException(nameof(newCustomer));
        }

        #endregion
    }
}
