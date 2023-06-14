using System;
using System.Globalization;
using System.Windows.Data;

namespace DWOS.AutomatedWorkOrderTool.Converters
{
    public class BoolToTextConverter : IValueConverter
    {
        #region Properties

        public string TrueValue { get; set; }

        public string FalseValue { get; set; }

        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool valueAsBool))
            {
                return null;
            }

            return valueAsBool ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Only converts one-way
            throw new NotImplementedException();
        }

        #endregion
    }
}
