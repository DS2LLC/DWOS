using System;
using System.Collections.Generic;

namespace DWOS.UI.Sales.Models
{
    public class CustomerSummary
    {
        #region Properties

        public int CustomerId { get; }

        public string Name { get; }

        public int LeadTime { get; set; }

        public decimal? DefaultPriceLot { get; set; }

        public string DefaultPurchaseOrder { get; set; }

        public string DefaultCustomerWorkOrder { get; set; }

        public bool RequiresCustomerWorkOrder { get; set; }

        public bool RequiresPurchaseOrder { get; set; }

        public bool RequiresDocument { get; set; }

        public List<CustomField> Fields { get; }

        public List<Part> Parts { get; }

        public List<CustomerShipping> ShippingMethods { get; }

        public List<CustomerAddress> ShippingAddresses { get; }

        #endregion

        #region Methods

        public CustomerSummary(int customerId, string name, int leadTime,
            List<CustomField> fields, List<Part> parts,
            List<CustomerShipping> shippingMethods,
            List<CustomerAddress> shippingAddresses)
        {
            CustomerId = customerId;
            Name = name;
            LeadTime = leadTime;
            Fields = fields ?? throw new ArgumentNullException(nameof(fields));
            Parts = parts ?? throw new ArgumentNullException(nameof(parts));
            ShippingMethods = shippingMethods ?? throw new ArgumentNullException(nameof(shippingMethods));
            ShippingAddresses = shippingAddresses ?? throw new ArgumentNullException(nameof(shippingAddresses));
        }

        #endregion
    }
}
