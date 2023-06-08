using System;
using System.Globalization;
using System.Windows.Data;
using DWOS.Data;
using DWOS.Data.Date;
using DWOS.Shared.Utilities;

namespace DWOS.UI.Utilities.Convertors
{
    /// <summary>
    /// Converts from a <see cref="DateTime"/> value to a user-friendly
    /// representation of the difference between it and
    /// <see cref="DateTime.Now"/>.
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class TimeToTimeDifferenceConverter : IValueConverter
    {
        #region Fields

        private readonly IDateTimeNowProvider _nowProvider;

        #endregion

        #region Methods

        public TimeToTimeDifferenceConverter()
        {
            _nowProvider = DependencyContainer.Resolve<IDateTimeNowProvider>();
        }

        public TimeToTimeDifferenceConverter(IDateTimeNowProvider nowProvider)
        {
            _nowProvider = nowProvider ?? throw new ArgumentNullException();
        }

        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                DateTime tempQualifier = ((DateTime)value);
                var minutes = System.Convert.ToInt32(_nowProvider.Now.Subtract(tempQualifier).TotalMinutes);
                return DateUtilities.ToDifferenceShortHand(minutes);
            }

            return "NA";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }

        #endregion
    }
}