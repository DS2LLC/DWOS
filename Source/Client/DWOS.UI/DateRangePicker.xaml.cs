using System;
using System.Windows;
using DWOS.Data;
using NLog;

namespace DWOS.UI
{
    /// <summary>
    /// Interaction logic for DateRangePicker.xaml
    /// </summary>
    public partial class DateRangePicker
    {
        public static readonly DependencyProperty FromDateProperty =
            DependencyProperty.Register(nameof(FromDate), typeof(DateTime?), typeof(DateRangePicker),
                new FrameworkPropertyMetadata {BindsTwoWayByDefault = true});

        public static readonly DependencyProperty ToDateProperty =
            DependencyProperty.Register(nameof(ToDate), typeof(DateTime?), typeof(DateRangePicker),
                new FrameworkPropertyMetadata {BindsTwoWayByDefault = true});

        public DateRangePicker()
        {
            InitializeComponent();
        }

        public DateTime? FromDate
        {
            get => GetValue(FromDateProperty) as DateTime?;
            set => SetValue(FromDateProperty, value);
        }

        public DateTime? ToDate
        {
            get => GetValue(ToDateProperty) as DateTime?;
            set => SetValue(ToDateProperty, value);
        }

        private void TodayClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var today = DateTime.Today;

                FromDate = today;
                ToDate = today;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing date.");
            }
        }

        private void LastMonthClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var lastMonth = DateTime.Now.AddMonths(-1);

                FromDate = DateUtilities.GetFirstDayOfMonth(lastMonth);
                ToDate = DateUtilities.GetLastDayOfMonth(lastMonth);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing date.");
            }
        }

        private void YearToDateClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var now = DateTime.Now;

                FromDate = DateUtilities.GetFirstDayOfYear(now);
                ToDate = now;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing date.");
            }
        }
    }
}
