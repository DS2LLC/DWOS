using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace DWOS.UI.Utilities.Convertors
{
    /// <summary>
    /// Converts a <see cref="bool"/> to a <see cref="ScrollBarVisibility"/> value.
    /// </summary>
    public class BoolToScrollBarVisibilityConverter : IValueConverter
    {
        #region  Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool showScrollbar)
            {
                return showScrollbar ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Only need one-way conversion
            return null;
        }

        #endregion
    }
}