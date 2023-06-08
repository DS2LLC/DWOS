using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Data;

namespace DWOS.Dashboard.Charts
{
    /// <summary>
    /// Converts a <see cref="DataRowView"/> or a <see cref="List{T}"/> of
    /// <see cref="DataRowView"/> instances to a string representation of
    /// a count.
    /// </summary>
    /// <remarks>
    /// Instances must have a "Count" column.
    /// </remarks>
    public class OrderValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is DataRowView)
                {
                    return "[" + ((DataRowView)value).Row["Count"] + "]";
                }

                if (value is List<object>) //OTHERS
                {
                    var rows = ((List<object>)value).OfType<DataRowView>();
                    return "[" + rows.Sum(r => System.Convert.ToInt32(r["Count"])) + "]";
                }

                return "";
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error in late value convertor.");
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
