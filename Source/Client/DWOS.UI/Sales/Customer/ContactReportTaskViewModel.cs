using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Windows.Input;

namespace DWOS.UI.Sales.Customer
{
    /// <summary>
    /// View Model for <see cref="ContactReportTaskEditor"/>.
    /// </summary>
    public class ContactReportTaskViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// Occurs when the user accepts the dialog.
        /// </summary>
        public event EventHandler Accepted;

        private string _reportName;
        private string _schedule;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the report name for this instance.
        /// </summary>
        public string ReportName
        {
            get => _reportName;
            set => Set(nameof(ReportName), ref _reportName, value);
        }

        /// <summary>
        /// Gets or sets the schedule for this instance.
        /// </summary>
        public string Schedule
        {
            get => _schedule;
            set => Set(nameof(Schedule), ref _schedule, value);
        }

        /// <summary>
        /// The accept command.
        /// </summary>
        public ICommand Accept { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ContactReportTaskViewModel" /> class.
        /// </summary>
        public ContactReportTaskViewModel()
        {
            Accept = new RelayCommand(DoAccept, CanAccept);
        }

        private void DoAccept()
        {
            Accepted?.Invoke(this, EventArgs.Empty);
        }

        private bool CanAccept() => string.IsNullOrEmpty(ValidateAll());

        public override string Validate(string propertyName)
        {
            if (propertyName == nameof(Schedule) && string.IsNullOrEmpty(_schedule))
            {
                return "Schedule is required.";
            }

            return string.Empty;
        }

        public override string ValidateAll() =>
            Validate(nameof(Schedule));

        #endregion
    }
}
