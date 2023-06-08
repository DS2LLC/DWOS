using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using DWOS.Data;
using Infragistics.Windows.DataPresenter;

namespace DWOS.UI
{
    /// <summary>
    /// Selects a style for an <see cref="OrderStatusData"/> record depending
    /// on its late status.
    /// </summary>
    public class LateStyleSelector : StyleSelector
    {
        private readonly Style _normalStyle;
        private readonly Style _lateStyle;
        private readonly IDwosApplicationSettingsProvider _settingsProvider;
            
        public LateStyleSelector(Style normalStyle, Style lateStyle,
            IDwosApplicationSettingsProvider settingsProvider)
        {
            _normalStyle = normalStyle;
            _lateStyle = lateStyle;
            _settingsProvider = settingsProvider
                ?? throw new ArgumentNullException(nameof(settingsProvider));
        }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var cvp = container as DataRecordCellArea;

            if (cvp?.Record == null || !cvp.Record.IsDataRecord || cvp.Record.DataItem == null)
                return _normalStyle;


            if (!(cvp.Record.DataItem is OrderStatusData or))
            {
                return _normalStyle;
            }

            if (or.Hold && !_settingsProvider.Settings.IncludeHoldsInLateOrders)
            {
                return _normalStyle;
            }

            var isShipDateLate = or.EstShipDate.HasValue && or.EstShipDate.Value.Date < DateTime.Now.Date;
            var isLateProcess = or.CurrentProcessDue.HasValue && or.CurrentProcessDue.Value.Date < DateTime.Now.Date;

            if (isShipDateLate || isLateProcess)
                return _lateStyle;

            return _normalStyle;
        }
    }
}