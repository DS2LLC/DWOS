namespace DWOS.UI.Sales.Models
{
    public class CustomerFee
    {
        #region Properties

        public int CustomerFeeId { get; }

        public string OrderFeeTypeId { get; }

        public decimal Charge { get; }

        #endregion

        #region Methods

        public CustomerFee(int customerFeeId, string orderFeeTypeId, decimal charge)
        {
            CustomerFeeId = customerFeeId;
            OrderFeeTypeId = orderFeeTypeId;
            Charge = charge;
        }

        #endregion
    }
}
