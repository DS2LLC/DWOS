using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using DWOS.UI.Utilities;

namespace DWOS.UI
{
    /// <summary>
    /// Converts an <see cref="OrderStatusData"/> to an image that indicates
    /// if it's in a batch.
    /// </summary>
    public class StatusImageConverter : IValueConverter
    {
        private Lazy<BitmapImage> _batchImageLazy = new Lazy<BitmapImage>(() => Properties.Resources.wizard.ToWpfImage());

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var or = value as OrderStatusData;

            if(or != null && or.InBatch)
                return _batchImageLazy.Value;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }

        #endregion
    }
}