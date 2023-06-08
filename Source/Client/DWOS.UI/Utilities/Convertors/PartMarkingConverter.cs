using DWOS.Data;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DWOS.UI.Utilities.Convertors
{
    /// <summary>
    /// Converts strings to 'part marking' strings.
    /// </summary>
    public sealed class PartMarkingConverter : IValueConverter
    {
        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString().ToPartMarkingString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}
