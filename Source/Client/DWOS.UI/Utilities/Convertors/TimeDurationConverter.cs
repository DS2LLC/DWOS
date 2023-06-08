using System;
using System.Globalization;
using System.Windows.Data;
using DWOS.Data;

namespace DWOS.UI.Utilities.Convertors
{
    /// <summary>
    /// Converts from an <see cref="int"/> representing minutes to a
    /// user-friendly string representation.
    /// </summary>
    [ValueConversion(typeof(int), typeof(String))]
    public class TimeDurationConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return DateUtilities.ToDifferenceShortHand((int)value);
            }

            return "NA";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }

            return DateUtilities.FromDifferenceShortHand(value.ToString());
        }

        #endregion
    }
}