using System;
using System.Windows;
using System.Windows.Controls;
using DWOS.Data;
using Infragistics.Windows.DataPresenter;
using NLog;

namespace DWOS.UI
{
    /// <summary>
    /// Selects the style for a process field.
    /// </summary>
    public class ProcessFieldStyleSelector : StyleSelector
    {
        private readonly Style _linkStyle;
        private readonly Style _standardStyle;

        public ProcessFieldStyleSelector(Style linkStyle, Style standardStyle)
        {
            _linkStyle = linkStyle;
            _standardStyle = standardStyle;
        }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            try
            {
                var cvp = container as CellValuePresenter;

                if (cvp?.Record == null || !cvp.Record.IsDataRecord)
                    return _standardStyle;

                var or = cvp.Record?.DataItem as OrderStatusData;

                if (or?.WorkStatus == ApplicationSettings.Current.WorkStatusInProcess)
                {
                    return _linkStyle;
                }

                return _standardStyle;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error in style selector.");
                return _standardStyle;
            }
        }
    }
}