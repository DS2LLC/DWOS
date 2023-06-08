namespace DWOS.UI.Sales.Models
{
    public class CustomerShipping
    {
        public int CustomerShippingId { get; }

        public string Name { get; }

        public bool IsDefault { get; }

        public CustomerShipping(int customerShippingId, string name, bool isDefault)
        {
            CustomerShippingId = customerShippingId;
            Name = name;
            IsDefault = isDefault;
        }
    }
}
