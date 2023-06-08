using DWOS.UI.Utilities;
using NLog;
using System;

namespace DWOS.UI.Sales.Customer
{
    /// <summary>
    /// Interaction logic for ContactReportTaskEditor.xaml, allows users to
    /// edit report task schedules for contact notifications.
    /// </summary>
    public partial class ContactReportTaskEditor
    {
        #region Properties

        /// <summary>
        /// Gets the current schedule for this instance.
        /// </summary>
        public string Schedule => ViewModel.Schedule;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ContactReportTaskEditor"/> class.
        /// </summary>
        public ContactReportTaskEditor()
        {
            InitializeComponent();
            Icon = Properties.Resources.Email_32.ToWpfImage();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ContactReportTaskEditor"/> class.
        /// </summary>
        /// <param name="reportName">
        /// The report's name.
        /// </param>
        /// <param name="schedule">
        /// The report task's original schedule.
        /// </param>
        public ContactReportTaskEditor(string reportName, string schedule)
            : this()
        {
            ViewModel.ReportName = reportName;
            ViewModel.Schedule = schedule;
        }

        #endregion

        #region Events

        private void ViewModel_Accepted(object sender, System.EventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, $"Error accepting {nameof(ContactReportTaskEditor)}");
            }
        }

        #endregion
    }
}
