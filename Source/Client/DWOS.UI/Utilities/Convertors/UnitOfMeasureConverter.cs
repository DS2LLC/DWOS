using System;
using System.Windows.Data;

namespace DWOS.UI.Utilities.Convertors
{
    /// <summary>
    /// Converts from an <see cref="UnitOfMeasure"/> value to a string
    /// representation of it using an 'is squared' parameter.
    /// </summary>
    /// <remarks>
    /// The parameter is a string - when it equals <c>"squared"</c> (using
    /// case-insensitive comparison), this converter adds a squared indicator
    /// to its output.
    /// </remarks>
    public class UnitOfMeasureConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var unitOfMeasure = value as UnitOfMeasure?;

            string result = string.Empty;

            bool isSquared = parameter != null && string.Equals(parameter.ToString(), "SQUARED", StringComparison.OrdinalIgnoreCase);

            if (unitOfMeasure.HasValue)
            {
                switch (unitOfMeasure.Value)
                {
                    case UnitOfMeasure.Inch:
                        result = "in";
                        break;
                    case UnitOfMeasure.Feet:
                        result = "ft";
                        break;
                    case UnitOfMeasure.Pound:
                        result = "lb";
                        break;
                }

                if (!string.IsNullOrEmpty(result) && isSquared)
                {
                    result += "²";
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
