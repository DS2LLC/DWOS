using System.Collections.Generic;

namespace DWOS.Data.Datasets
{
    public partial class CustomersDataset
    {
        partial class CustomerRow
        {
            public bool HasBillingAddress =>
                !IsAddress1Null() && !IsCityNull() && !IsStateNull() && !IsZipNull();
        }

        public partial class CustomFieldDataTable
        {
            public List<int> CustomerLoaded { get; set; }

            /// <summary>
            /// Load the table by customer, if the customer has not already been loaded.
            /// </summary>
            /// <param name="customerId"></param>
            /// <returns></returns>
            public CustomFieldRow[] LoadByCustomer(int customerId)
            {
                if (this.CustomerLoaded == null)
                    CustomerLoaded = new List<int>();

                if (!CustomerLoaded.Contains(customerId))
                {
                    CustomerLoaded.Add(customerId);
                    using (var ta = new CustomersDatasetTableAdapters.CustomFieldTableAdapter() { ClearBeforeFill = false })
                    {
                        ta.FillByCustomer(this, customerId);
                    }
                }

                return this.Select("CustomerID = " + customerId.ToString(), "Name") as CustomFieldRow[];
            }
        }
    }
}

namespace DWOS.Data.Datasets.CustomersDatasetTableAdapters
{
    
    
    public partial class ContactTableAdapter {
    }
}

namespace DWOS.Data.CustomersDatasetTableAdapters {
    
    
    public partial class ContactAdditionalCustomerTableAdapter {
    }
}
