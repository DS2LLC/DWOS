using System;
using System.Globalization;
using System.Windows.Data;
using DWOS.AutomatedWorkOrderTool.ViewModel;

namespace DWOS.AutomatedWorkOrderTool.Converters
{
    public class ImportSummaryImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ImportSummaryItem.ItemType type))
            {
                return null;
            }

            switch (type)
            {
                case ImportSummaryItem.ItemType.Error:
                    return "/Images/error_32.png";
                default:
                    return "/Images/info_32.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
