using System;
using System.Globalization;
using System.Windows.Data;

namespace DWOS.UI.Utilities.Convertors
{
    /// <summary>
    /// Converts between <see cref="bool"/> and <see cref="string"/> values.
    /// </summary>
    public sealed class BoolToStringConverter : IValueConverter
    {
        #region Properties

        /// <summary>
        /// Gets or sets the string value to use for true.
        /// </summary>
        public string TrueValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the string value to use for false or in case of
        /// conversion failure.
        /// </summary>
        public string FalseValue
        {
            get;
            set;
        }

        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueAsBool = (value as bool?) ?? false;

            return valueAsBool ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueAsString = value as string;

            return valueAsString == TrueValue;
        }

        #endregion
    }
}
