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
    /// Converts an <see cref="OrderStatusData"/> instance to a type that
    /// represents its status or type.
    /// </summary>
    public class WorkOrderImageConverter : IValueConverter
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

            switch(or.OrderType)
            {
                case 1:
                    return or.Hold ? GetImage("hold") : null;
                case 3: //OrderType.ReworkExt
                    return GetImage("extRework");
                case 4: //OrderType.ReworkInt
                    return GetImage("intRework");
                case 5: //OrderType.ReworkHold
                    return GetImage("hold");
                case 6: //OrderType.Lost
                    return GetImage("lost");
                case 7: //OrderType.Quarantine
                    return GetImage("quarantine");
                default:
                    return null;
            }
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
                case "lost":
                    img = Properties.Resources.Lost_16;
                    break;
                case "hold":
                    img = Properties.Resources.Hold_16;
                    break;
                case "extRework":
                    img = Properties.Resources.Repair_Blue_16;
                    break;
                case "intRework":
                    img = Properties.Resources.Repair_16;
                    break;
                case "quarantine":
                    img = Properties.Resources.Quarantine_16;
                    break;
            }

            if(img != null)
                Images.Add(name, img.ToWpfImage());

            return Images.ContainsKey(name) ? Images[name] : null;
        }

        #endregion
    }
}