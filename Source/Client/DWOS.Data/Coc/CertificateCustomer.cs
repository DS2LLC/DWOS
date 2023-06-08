namespace DWOS.Data.Coc
{
    public class CertificateCustomer
    {
        #region Properties

        public int CustomerId { get; }

        public string Name { get; }

        #endregion

        #region Methods

        public CertificateCustomer(int customerId, string name)
        {
            CustomerId = customerId;
            Name = name;
        }

        #endregion
    }
}
