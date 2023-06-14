using System;
using System.Globalization;
using System.Windows.Data;
using DWOS.AutomatedWorkOrderTool.Model;

namespace DWOS.AutomatedWorkOrderTool.Converters
{
    public class OrderStatusConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ShippingManifestOrder.OrderStatus status))
            {
                return null;
            }

            switch (status)
            {
                case ShippingManifestOrder.OrderStatus.New:
                    return "OK";
                case ShippingManifestOrder.OrderStatus.NewWithoutExistingOrders:
                    return "New Part";
                case ShippingManifestOrder.OrderStatus.Invalid:
                    return "Not Valid";
                case ShippingManifestOrder.OrderStatus.Existing:
                    return "Existing Order";
                default:
                    return "Unknown";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
