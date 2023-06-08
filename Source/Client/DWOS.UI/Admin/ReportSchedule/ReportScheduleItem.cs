using DWOS.Data.Datasets;
using DWOS.UI.Utilities;

namespace DWOS.UI.Admin.ReportSchedule
{
    /// <summary>
    /// Represents a single report schedule item.
    /// </summary>
    public class ReportScheduleItem : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets the report type row for this instance.
        /// </summary>
        public ApplicationSettingsDataSet.ReportTypeRow ReportTypeRow { get; }

        /// <summary>
        /// Gets the report name for this instance.
        /// </summary>
        public string ReportName => ReportTypeRow.DisplayName;

        /// <summary>
        /// Gets or sets the default schedule for this instance.
        /// </summary>
        public string Schedule
        {
            get => ReportTypeRow.IsDefaultScheduleNull() ? string.Empty : ReportTypeRow.DefaultSchedule;
            set
            {
                if (string.IsNullOrEmpty(value) && !ReportTypeRow.IsDefaultScheduleNull())
                {
                    ReportTypeRow.SetDefaultScheduleNull();
                    RaisePropertyChanged(nameof(Schedule));
                }
                else if (ReportTypeRow.IsDefaultScheduleNull() || value != ReportTypeRow.DefaultSchedule)
                {
                    ReportTypeRow.DefaultSchedule = value;
                    RaisePropertyChanged(nameof(Schedule));
                }
            }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportScheduleItem"/> class.
        /// </summary>
        /// <param name="reportTypeRow"></param>
        public ReportScheduleItem(ApplicationSettingsDataSet.ReportTypeRow reportTypeRow)
        {
            ReportTypeRow = reportTypeRow;
        }

        public override string Validate(string propertyName)
        {
            if (propertyName == nameof(Schedule))
            {
                var isScheduleValid = !ReportTypeRow.IsDefaultScheduleNull()
                    && !string.IsNullOrEmpty(ReportTypeRow.DefaultSchedule);

                if (!isScheduleValid)
                {
                    return "Schedule is Invalid";
                }
            }

            return string.Empty;
        }

        public override string ValidateAll() =>
            Validate(nameof(Schedule));

        #endregion
    }
}
