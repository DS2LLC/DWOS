using DWOS.Data.Datasets;
using DWOS.Utilities.Validation;
using System.Windows.Forms;

namespace DWOS.UI.Sales.Order
{
    public interface ISerialNumberWidget
    {
        bool ReadOnly { get; set; }

        void AddValidators(ValidatorManager manager, ErrorProvider errProvider);

        void LoadData(OrdersDataSet dataset);

        void LoadOrder(int orderId);

        void LoadOrder(OrdersDataSet.OrderRow row);

        void SaveRow();
    }
}
