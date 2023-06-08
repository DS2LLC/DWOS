using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DWOS.UI
{
    /// <summary>
    /// Used with <see cref="WorkOrderImageConverter"/> to show/hide an image
    /// for an <see cref="OrderStatusData"/> instance.
    /// </summary>
    public class WorkOrderImageVisibilityConverter : IValueConverter
    {
        private static readonly Dictionary <string, BitmapImage> Images = new Dictionary <string, BitmapImage>();

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var or = value as OrderStatusData;

            if (or == null)
            {
                return Visibility.Collapsed;
            }

            switch(or.OrderType)
            {
                case 1:
                    return or.Hold ? Visibility.Visible : Visibility.Collapsed;
                case 3: //OrderType.ReworkExt
                case 4: //OrderType.ReworkInt
                case 5: //OrderType.ReworkHold
                case 6: //OrderType.Lost
                case 7: //OrderType.Quarantine
                    return Visibility.Visible;
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }

        #endregion
    }
}