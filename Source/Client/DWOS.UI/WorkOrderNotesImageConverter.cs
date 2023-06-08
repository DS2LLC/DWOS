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
    /// Converts an <see cref="OrderStatusData"/> to an image of a note if
    /// the order has a note.
    /// </summary>
    public class WorkOrderNotesImageConverter : IValueConverter
    {
        private static readonly Dictionary <string, BitmapImage> Images = new Dictionary <string, BitmapImage>();

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var or = value as OrderStatusData;

            if (or == null)
            {
                return null;
            }

            return or.OrderNoteCount > 0
                ? GetImage("note")
                : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }

        private static BitmapImage GetImage(string name)
        {
            if (Images.ContainsKey(name))
            {
                return Images[name];
            }

            Bitmap img = null;

            switch(name)
            {
                case "note":
                    img = Properties.Resources.Note_16;
                    break;
            }

            if(img != null)
                Images.Add(name, img.ToWpfImage());

            return Images.ContainsKey(name) ? Images[name] : null;
        }

        #endregion
    }
}
