using System;
using System.Globalization;
using System.Windows.Data;

namespace DWOS.UI
{
    /// <summary>
    /// Converts an <see cref="OrderStatusData"/> instance to a tooltip.
    /// </summary>
    public class PartToolTipConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var or = value as OrderStatusData;
            if (or == null)
            {
                return null;
            }
            var partTip = new QuickPartTip();
            partTip.LoadPart(or.WO);
            return partTip;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }

        #endregion
    }
}