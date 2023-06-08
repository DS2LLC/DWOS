using DWOS.Data;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DWOS.UI.Utilities.Convertors
{
    /// <summary>
    /// Converts from an <see cref="OrderStatusData"/> instance to a string
    /// representation of the amount of time left to complete the
    /// current/next process.
    /// </summary>
    public class ProcessTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var or = value as OrderStatusData;
            if (string.IsNullOrEmpty(or?.RemainingTime))
            {
                return null;
            }

            // Remaining time is in minutes
            int remainingTime;
            int.TryParse(or.RemainingTime, out remainingTime);
            return DateUtilities.ToDifferenceShortHand(remainingTime);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }

        #endregion
    }
}
