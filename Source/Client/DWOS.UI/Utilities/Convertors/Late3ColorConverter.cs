using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using DWOS.Data;
using DWOS.Data.Date;
using DWOS.Shared.Utilities;
using DWOS.UI.Properties;

namespace DWOS.UI.Utilities.Convertors
{
    /// <summary>
    /// Converts from a <see cref="OrderStatusData"/> to a
    /// <see cref="SolidColorBrush"/> that indicates how late it is.
    /// </summary>
    public class Late3ColorConverter : IValueConverter
    {
        #region Fields

        private static int _dayLookUpCreatedOn = -1;
        private static Dictionary<DateTime, int> _dayLateLookup = new Dictionary<DateTime, int>();
        private static Dictionary<int, SolidColorBrush> _dayColorLookUp = new Dictionary<int, SolidColorBrush>();
        private static SolidColorBrush _normal = new SolidColorBrush(Colors.Transparent);
        private readonly DaysLateCalculator _daysLateCalculator;
        private readonly IDwosApplicationSettingsProvider _settingsProvider;

        #endregion

        #region Methods

        public Late3ColorConverter()
        {
            _daysLateCalculator = new DaysLateCalculator(DependencyContainer.Resolve<IDateTimeNowProvider>());
            _settingsProvider = DependencyContainer.Resolve<IDwosApplicationSettingsProvider>();
        }

        public Late3ColorConverter(IDateTimeNowProvider nowProvider,
            IDwosApplicationSettingsProvider settingsProvider)
        {
            if (nowProvider == null)
            {
                throw new ArgumentNullException(nameof(nowProvider));
            }

            _daysLateCalculator = new DaysLateCalculator(nowProvider);
            _settingsProvider = settingsProvider ?? throw new ArgumentNullException();
        }

        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is OrderStatusData statusData) || !statusData.EstShipDate.HasValue)
            {
                return null;
            }

            if (statusData.Hold && !_settingsProvider.Settings.IncludeHoldsInLateOrders)
            {
                return null;
            }

            var daysTillLate = _daysLateCalculator.GetDaysLate(statusData.EstShipDate.Value);

            if (daysTillLate < 0) //aka late
            {
                SolidColorBrush value1;

                return _dayColorLookUp.TryGetValue(-1, out value1) ? value1 : _normal;
            }

            if (daysTillLate <= 2)
            {
                SolidColorBrush value1;

                return _dayColorLookUp.TryGetValue(daysTillLate, out value1) ? value1 : _normal;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }

        public static void Initialize()
        {
            if (_dayLookUpCreatedOn != DateTime.Now.Day)
            {
                _dayLookUpCreatedOn = DateTime.Now.Day;
                _dayLateLookup.Clear();
            }

            if (_dayColorLookUp.Count < 1)
            {
                _dayColorLookUp.Add(-1, new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.Late0Day.Name)));
                _dayColorLookUp.Add(0, new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.Late1Day.Name)));
                _dayColorLookUp.Add(1, new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.Late2Day.Name)));
                _dayColorLookUp.Add(2, new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.Late3Day.Name)));
            }
        }

        #endregion
    }
}