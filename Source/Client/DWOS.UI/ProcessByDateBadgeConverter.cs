using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DWOS.Data.Date;
using DWOS.Shared.Utilities;
using DWOS.UI.Properties;

namespace DWOS.UI
{
    /// <summary>
    /// Converts an <see cref="OrderStatusData"/> instance to a shape that
    /// represents the Process By date for its current process.
    /// </summary>
    public class ProcessByDateBadgeConverter : IValueConverter
    {
        #region Fields

        private static readonly Dictionary <int, SolidColorBrush> DayColorLookUp = new Dictionary <int, SolidColorBrush>();
        private static readonly SolidColorBrush Normal = new SolidColorBrush(Colors.Transparent);
        private readonly DaysLateCalculator _daysLateCalculator;

        #endregion

        #region Methods

        public ProcessByDateBadgeConverter()
        {
            _daysLateCalculator = new DaysLateCalculator(DependencyContainer.Resolve<IDateTimeNowProvider>());
        }

        public ProcessByDateBadgeConverter(IDateTimeNowProvider nowProvider)
        {
            if (nowProvider == null)
            {
                throw new ArgumentNullException(nameof(nowProvider));
            }

            _daysLateCalculator = new DaysLateCalculator(nowProvider);
        }

        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var or = value as OrderStatusData;

            if (or == null || !or.CurrentProcessDue.HasValue)
            {
                return null;
            }

            var daysTillLate = _daysLateCalculator.GetDaysLate(or.CurrentProcessDue.Value.Date);

            var rect = new System.Windows.Shapes.Rectangle {Fill = GetColor(daysTillLate), Width = 5};

            //do animations if enabled but highlight full row is not
            if(Settings.Default.WIPAnimations && !Settings.Default.WIPHighlightLateFullRow && daysTillLate < 0)
            {
                var animate = new ColorAnimation();
                animate.From = Colors.Transparent;
                animate.Duration = TimeSpan.FromSeconds(1);
                animate.RepeatBehavior = RepeatBehavior.Forever;
                animate.AutoReverse = true;
                rect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, animate);
            }

            return rect;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }

        public static void Initialize()
        {
            if (DayColorLookUp.Count >= 1)
            {
                return;
            }

            var late0Color = ColorConverter.ConvertFromString(Settings.Default.Late0Day.Name);
            if (late0Color != null)
                DayColorLookUp.Add(-1, new SolidColorBrush((Color) late0Color));

            var late1Color = ColorConverter.ConvertFromString(Settings.Default.Late1Day.Name);
            if (late1Color != null)
            {
                DayColorLookUp.Add(0, new SolidColorBrush((Color)late1Color));
            }

            var late2Color = ColorConverter.ConvertFromString(Settings.Default.Late2Day.Name);
            if (late2Color != null)
            {
                DayColorLookUp.Add(1, new SolidColorBrush((Color)late2Color));
            }
        }

        private static SolidColorBrush GetColor(int daysLate)
        {
            SolidColorBrush value;

            return DayColorLookUp.TryGetValue(daysLate, out value) ? value : Normal;
        }

        #endregion
    }
}