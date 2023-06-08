using System;
using System.Data;
using System.Windows.Data;

namespace DWOS.Dashboard.Production
{
    /// <summary>
    /// Converts a <see cref="DataRowView"/> or a <see cref="List{T}"/> of
    /// to a string representation of a count.
    /// </summary>
    /// <remarks>
    /// This differs from similar classes in that it rounds counts over 1000.
    /// Instances must have a "Count" column.
    /// </remarks>
    public class ProductionMarkerTextConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var rowView = value as DataRowView;
            if (rowView?.Row?["Count"] != null)
            {
                var count = System.Convert.ToInt32(rowView.Row["Count"]);
                if (count < 1000)
                {
                    return count;
                }
                else
                {
                    return (count / 1000.00).ToString("n1") + "K";
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


}
