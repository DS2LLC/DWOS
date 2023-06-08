using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DWOS.UI.Utilities.Convertors
{
    /// <summary>
    /// Converts to <see cref="Visibility.Collapsed"/> if the value is null or
    /// <see cref="DBNull.Value"/> or <see cref="string.Empty"/>;
    /// otherwise, converts to <see cref="Visibility.Visible"/>.
    /// </summary>
    public class ValueToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DBNull.Value)
            {
                return Visibility.Collapsed;
            }

            if (value is string valueString && string.IsNullOrEmpty(valueString))
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
