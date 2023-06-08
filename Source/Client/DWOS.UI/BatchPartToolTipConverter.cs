using System;
using System.Windows.Data;

namespace DWOS.UI
{
    /// <summary>
    /// Converts a <see cref="BatchOrderStatusData"/> instance to a part.
    /// </summary>
    public sealed class BatchPartToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var batchOrder = value as BatchOrderStatusData;

            if (batchOrder == null)
            {
                return null;
            }

            var partTip = new QuickPartTip();
            partTip.LoadPart(batchOrder.OrderID);

            return partTip;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}