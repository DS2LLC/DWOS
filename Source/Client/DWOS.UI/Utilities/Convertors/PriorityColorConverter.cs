using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace DWOS.UI.Utilities.Convertors
{
    /// <summary>
    /// Converts from an <see cref="OrderStatusData"/> instance to a
    /// <see cref="System.Windows.Media.Brush"/> instance that indicates
    /// its priority.
    /// </summary>
    public class PriorityColorConverter : IValueConverter
    {
        #region Fields

        private static Dictionary<string, System.Windows.Media.Brush> _priorityColors = null;
        private static object _lock = new Object();
        private static System.Windows.Media.Brush DefaultColor = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF444444"));

        #endregion

        #region Methods

        private static void LoadData()
        {
            if (_priorityColors != null)
                return;

            _priorityColors = new Dictionary<string, System.Windows.Media.Brush>();

            var priorityTable = new Data.Datasets.OrdersDataSet.d_PriorityDataTable();

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.d_PriorityTableAdapter())
                ta.Fill(priorityTable);

            foreach (var priorityRow in priorityTable)
            {
                if (!priorityRow.IsColorNull())
                    _priorityColors.Add(priorityRow.PriorityID,new System.Windows.Media.SolidColorBrush( (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(priorityRow.Color.ToString())));
            }

            priorityTable.Dispose();
            priorityTable = null;
        }

        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (_priorityColors == null)
            {
                lock (_lock)
                    LoadData();
            }

            if (value is string && _priorityColors.ContainsKey(value.ToString()))
                return _priorityColors[value.ToString()];

            var or = value as OrderStatusData;
            if (or != null && _priorityColors.ContainsKey(or.Priority))
            {
                return _priorityColors[or.Priority];
            }

            return DefaultColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }

        #endregion
    }
}
