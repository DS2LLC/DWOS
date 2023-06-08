using System;
using System.Globalization;
using System.Windows.Data;
using DWOS.Data.Datasets;

namespace DWOS.UI
{
    /// <summary>
    /// Converts an <see cref="int"/> representing an <see cref="OrderType"/>
    /// value to a display string.
    /// </summary>
    [ValueConversion(typeof(int), typeof(string))]
    public class OrderTypeToNameConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                var orderType = (OrderType)value;
                switch(orderType)
                {
                    case OrderType.ReworkExt:
                        return "External Rework";
                    case OrderType.ReworkInt:
                        return "Internal Rework";
                    case OrderType.ReworkHold:
                        return "Rework Hold";
                    default:
                        return orderType.ToString();
                }
            }

            return "NA";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }

        #endregion
    }
}