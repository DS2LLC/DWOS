namespace DWOS.UI.Sales.Models
{
    public class CustomerAddress
    {
        public int CustomerAddressId { get; }

        public string Name { get; }

        public int CountryId { get; }

        public bool IsDefault { get; }

        public CustomerAddress(int customerAddressId, string name, int countryId, bool isDefault)
        {
            CustomerAddressId = customerAddressId;
            Name = name;
            CountryId = countryId;
            IsDefault = isDefault;
        }
    }
}
