using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.UI.Sales.Models;

namespace DWOS.UI.Sales
{
    public class CustomerFeeViewModel : Utilities.ViewModelBase
    {
        #region Fields

        private string _orderFeeTypeId;
        private OrderPrice.enumFeeType _feeType;
        private decimal _charge;
        private bool _isDiscount;

        #endregion

        #region Properties

        public CustomersDataset.CustomerFeeRow Row { get; }

        public string OrderFeeTypeId
        {
            get => _orderFeeTypeId;
            set => Set(nameof(OrderFeeTypeId), ref _orderFeeTypeId, value);
        }

        public OrderPrice.enumFeeType FeeType
        {
            get => _feeType;
            set => Set(nameof(FeeType), ref _feeType, value);
        }

        public decimal Charge
        {
            get => _charge;
            set => Set(nameof(Charge), ref _charge, value);
        }

        public bool IsDiscount
        {
            get => _isDiscount;
            set => Set(nameof(IsDiscount), ref _isDiscount, value);
        }

        #endregion

        #region Methods

        public CustomerFeeViewModel()
        {

        }

        private CustomerFeeViewModel(CustomersDataset.CustomerFeeRow row)
        {
            Row = row;
        }

        public static CustomerFeeViewModel From(OrderFeeType orderFeeType)
        {
            if (orderFeeType == null)
            {
                return null;
            }

            return new CustomerFeeViewModel
            {
                _orderFeeTypeId = orderFeeType.OrderFeeTypeId,
                _charge = orderFeeType.Price,
                _feeType = orderFeeType.FeeType,
                _isDiscount = orderFeeType.Price < 0
            };
        }

        public static CustomerFeeViewModel From(CustomersDataset.CustomerFeeRow row, OrderFeeType orderFeeType)
        {
            if (row == null || orderFeeType == null)
            {
                return null;
            }

            return new CustomerFeeViewModel(row)
            {
                _orderFeeTypeId = row.OrderFeeTypeID,
                _charge = row.Charge,
                _feeType = orderFeeType.FeeType,
                _isDiscount = orderFeeType.Price < 0
            };
        }

        public override string Validate(string propertyName)
        {
            if (propertyName == nameof(Charge))
            {
                var isNegative = _charge < 0;
                if (isNegative && !_isDiscount)
                {
                    return "Fee amount cannot be negative";
                }
                else if (!isNegative && _isDiscount)
                {
                    return "Discount amount cannot be positive.";
                }
            }

            return null;
        }

        public override string ValidateAll() =>
            Validate(nameof(Charge));

        #endregion
    }
}
