using Infragistics.Controls.Editors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DWOS.UI.Utilities.Convertors
{
    /// <summary>
    /// Converts between <see cref="bool"/> and
    /// <see cref="CustomValueEnteredActions"/> values.
    /// </summary>
    public class BoolToCustomValueEnteredActionConverter : IValueConverter
    {
        public CustomValueEnteredActions TrueAction { get; set; } =
            CustomValueEnteredActions.Add;

        public CustomValueEnteredActions FalseAction { get; set; } =
            CustomValueEnteredActions.Ignore;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueAsBool = value as bool?;

            return valueAsBool ?? false
                ? TrueAction
                : FalseAction;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueAsAction = value as CustomValueEnteredActions?;

            return valueAsAction == TrueAction;
        }
    }
}
