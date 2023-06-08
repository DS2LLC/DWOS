using System;
using System.Globalization;
using System.Windows.Data;

namespace DWOS.Dashboard.Charts
{
    /// <summary>
    /// Converts a boolean value to its negative value.
    /// </summary>
    public class NegateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return value;
        }

    }
}
