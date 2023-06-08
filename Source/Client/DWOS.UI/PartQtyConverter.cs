using System;
using System.Data;
using System.Globalization;
using System.Windows.Data;
using DWOS.Data;
using DWOS.Shared.Utilities;
using Infragistics.Windows.DataPresenter;

namespace DWOS.UI
{
    /// <summary>
    /// Converts a <see cref="DataRecord"/> or <see cref="OrderStatusData"/>
    /// instance to a part quantity string.
    /// </summary>
    [ValueConversion(typeof(DataRowView), typeof(string))]
    public class PartQtyConverter : IValueConverter
    {
        #region Fields

        private readonly IDwosApplicationSettingsProvider _settingsProvider;

        #endregion

        #region Methods

        public PartQtyConverter()
        {
            _settingsProvider = DependencyContainer.Resolve<IDwosApplicationSettingsProvider>();
        }

        public PartQtyConverter(IDwosApplicationSettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
        }

        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var partQuantity = value as int?;

            // UI passes DataRecord as parameter, but tests pass OrderStatusData.
            var order = (parameter as DataRecord)?.DataItem as OrderStatusData ?? parameter as OrderStatusData;

            var partProcessingCount = order?.PartProcessingCount;

            if (!partQuantity.HasValue)
            {
                return string.Empty;
            }

            if (!partProcessingCount.HasValue || !_settingsProvider.Settings.AllowPartialProcessLoads)
            {
                return partQuantity.Value;
            }

            return partProcessingCount.Value + " / " + partQuantity.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }

        #endregion
    }
}