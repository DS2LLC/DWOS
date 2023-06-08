using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using DWOS.Data;
using DWOS.Shared.Utilities;
using DWOS.UI.Properties;
using DWOS.UI.Utilities;

namespace DWOS.UI
{
    /// <summary>
    /// Converts an <see cref="OrderStatusData"/> instance to an image that
    /// represents its current action.
    /// </summary>
    public class ProcessActionsConverter : IValueConverter
    {
        #region Fields

        public enum WorkAction
        {
            None,
            Process,
            CheckIn,
            ControlInspection,
            FinalInspection,
            OrderReview,
            ImportExportReview,
        }

        private readonly BitmapImage _checkInImage;
        private readonly BitmapImage _controlInspectionImage;
        private readonly BitmapImage _finalInspectionImage;
        private readonly BitmapImage _orderReviewImage;
        private readonly BitmapImage _processImage;
        private readonly IDwosApplicationSettingsProvider _settingsProvider;

        #endregion

        #region IValueConverter Members

        public ProcessActionsConverter()
        {
            _settingsProvider = DependencyContainer.Resolve<IDwosApplicationSettingsProvider>();
            _processImage = Resources.Process_16.ToWpfImage();
            _checkInImage = Resources.Checked_32.ToWpfImage();
            _controlInspectionImage = Resources.Inspection_16.ToWpfImage();
            _finalInspectionImage = Resources.Certificate_16.ToWpfImage();
            _orderReviewImage = Resources.OrderReview_16.ToWpfImage();
        }

        public ProcessActionsConverter(IDwosApplicationSettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            _processImage = Resources.Process_16.ToWpfImage();
            _checkInImage = Resources.Checked_32.ToWpfImage();
            _controlInspectionImage = Resources.Inspection_16.ToWpfImage();
            _finalInspectionImage = Resources.Certificate_16.ToWpfImage();
            _orderReviewImage = Resources.OrderReview_16.ToWpfImage();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var or = value as OrderStatusData;

            if (or == null)
            {
                return null;
            }

            var workAction = or.InBatch ? WorkAction.None : GetNextAction(or, _settingsProvider.Settings);

            switch(workAction)
            {
                case WorkAction.Process:
                    return _processImage;
                case WorkAction.CheckIn:
                    return _checkInImage;
                case WorkAction.ControlInspection:
                    return _controlInspectionImage;
                case WorkAction.FinalInspection:
                    return _finalInspectionImage;
                case WorkAction.OrderReview:
                case WorkAction.ImportExportReview:
                    return _orderReviewImage;
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }

        public static WorkAction GetNextAction(OrderStatusData or, ApplicationSettings appSettings)
        {
            if(or.WorkStatus == appSettings.WorkStatusInProcess)
            {
                if(or.CurrentLocation == Settings.Default.CurrentDepartment)
                    return WorkAction.Process;
            }
            else if(or.WorkStatus == appSettings.WorkStatusChangingDepartment)
            {
                if (or.NextDept == Settings.Default.CurrentDepartment)
                    return WorkAction.CheckIn;
            }
            else if(or.WorkStatus == appSettings.WorkStatusPendingQI)
                return WorkAction.ControlInspection;
            else if(or.WorkStatus == appSettings.WorkStatusFinalInspection)
                return WorkAction.FinalInspection;
            else if(or.WorkStatus == appSettings.WorkStatusPendingOR)
                return WorkAction.OrderReview;
            else if(or.WorkStatus == appSettings.WorkStatusPendingImportExportReview)
                return WorkAction.ImportExportReview;

            return WorkAction.None;
        }

        #endregion
    }
}