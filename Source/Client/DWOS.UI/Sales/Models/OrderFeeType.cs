using DWOS.Data;

namespace DWOS.UI.Sales.Models
{
    public class OrderFeeType
    {
        #region Properties

        public string OrderFeeTypeId { get; }

        public OrderPrice.enumFeeType FeeType { get; }

        public decimal Price { get; }

        public bool IsDefault { get; }

        #endregion

        #region Methods

        public OrderFeeType(string orderFeeTypeId, OrderPrice.enumFeeType feeType, decimal price, bool isDefault)
        {
            OrderFeeTypeId = orderFeeTypeId;
            FeeType = feeType;
            Price = price;
            IsDefault = isDefault;
        }

        #endregion
    }
}
