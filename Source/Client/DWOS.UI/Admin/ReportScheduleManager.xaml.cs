using DWOS.UI.Utilities;
using NLog;
using System;
using System.Windows;

namespace DWOS.UI.Admin
{
    /// <summary>
    /// Interaction logic for ReportScheduleManager.xaml, a dialog for
    /// managing report schedules.
    /// </summary>
    public partial class ReportScheduleManager
    {
        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ReportScheduleManager"/>class.
        /// </summary>
        public ReportScheduleManager()
        {
            InitializeComponent();
            Icon = Properties.Resources.Email_32.ToWpfImage();
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.LoadData();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, $"Error loading {nameof(ReportScheduleManager)}.");
            }
        }

        private void ViewModel_Accepted(object sender, EventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, $"Error accepting {nameof(ReportScheduleManager)}.");
            }
        }

        #endregion
    }
}
