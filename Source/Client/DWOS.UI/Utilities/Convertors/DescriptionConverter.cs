using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace DWOS.UI.Utilities.Convertors
{
    /// <summary>
    /// Converts from an enumerated value to a value provided by its
    /// <see cref="DescriptionAttribute"/>.
    /// </summary>
    public sealed class DescriptionConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var members = value.GetType().GetMember(value.ToString());

            string description = value.ToString();
            if (members.Length == 1)
            {
                var attrs = members[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    description = (attrs[0] as DescriptionAttribute).Description;
                }
            }

            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method is not currently needed.");
        }

        #endregion
    }
}
