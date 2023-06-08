using CronExpressionDescriptor;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Interaction logic for ReportScheduleEditor.xaml, a user control for
    /// editing cron schedules.
    /// </summary>
    public partial class ReportScheduleEditor
    {
        #region  Fields

        /// <summary>
        /// Dependency property for <see cref="ReportName"/>.
        /// </summary>
        public static readonly DependencyProperty ReportNameProperty = DependencyProperty.Register(
            nameof(ReportName), typeof(string), typeof(ReportScheduleEditor));

        /// <summary>
        /// Dependency property for <see cref="Schedule"/>.
        /// </summary>
        public static readonly DependencyProperty ScheduleProperty = DependencyProperty.Register(
            nameof(Schedule), typeof(string), typeof(ReportScheduleEditor),
            new FrameworkPropertyMetadata { PropertyChangedCallback = Schedule_PropertyChanged, BindsTwoWayByDefault = true });

        /// <summary>
        /// Dependency property for <see cref="ScheduleDescription"/>.
        /// </summary>
        public static readonly DependencyProperty ScheduleDescriptionProperty = DependencyProperty.Register(
            nameof(ScheduleDescription), typeof(string), typeof(ReportScheduleEditor),
            new FrameworkPropertyMetadata { PropertyChangedCallback = Schedule_PropertyChanged, BindsTwoWayByDefault = true });

        /// <summary>
        /// Dependency property for <see cref="Seconds"/>.
        /// </summary>
        public static readonly DependencyProperty SecondsProperty = DependencyProperty.Register(
            nameof(Seconds), typeof(string), typeof(ReportScheduleEditor),
            new FrameworkPropertyMetadata { PropertyChangedCallback = Field_PropertyChanged, BindsTwoWayByDefault = true });

        /// <summary>
        /// Dependency property for <see cref="Minutes"/>.
        /// </summary>
        public static readonly DependencyProperty MinutesProperty = DependencyProperty.Register(
            nameof(Minutes), typeof(string), typeof(ReportScheduleEditor),
            new FrameworkPropertyMetadata { PropertyChangedCallback = Field_PropertyChanged, BindsTwoWayByDefault = true });

        /// <summary>
        /// Dependency property for <see cref="Hours"/>.
        /// </summary>
        public static readonly DependencyProperty HoursProperty = DependencyProperty.Register(
            nameof(Hours), typeof(string), typeof(ReportScheduleEditor),
            new FrameworkPropertyMetadata { PropertyChangedCallback = Field_PropertyChanged, BindsTwoWayByDefault = true });

        /// <summary>
        /// Dependency property for <see cref="DayOfMonth"/>.
        /// </summary>
        public static readonly DependencyProperty DayOfMonthProperty = DependencyProperty.Register(
            nameof(DayOfMonth), typeof(string), typeof(ReportScheduleEditor),
            new FrameworkPropertyMetadata { PropertyChangedCallback = Field_PropertyChanged, BindsTwoWayByDefault = true });

        /// <summary>
        /// Dependency property for <see cref="Month"/>.
        /// </summary>
        public static readonly DependencyProperty MonthProperty = DependencyProperty.Register(
            nameof(Month), typeof(string), typeof(ReportScheduleEditor),
            new FrameworkPropertyMetadata { PropertyChangedCallback = Field_PropertyChanged, BindsTwoWayByDefault = true });

        /// <summary>
        /// Dependency property for <see cref="DayOfWeek"/>.
        /// </summary>
        public static readonly DependencyProperty DayOfWeekProperty = DependencyProperty.Register(
            nameof(DayOfWeek), typeof(string), typeof(ReportScheduleEditor),
            new FrameworkPropertyMetadata { PropertyChangedCallback = Field_PropertyChanged, BindsTwoWayByDefault = true });

        /// <summary>
        /// Dependency property for <see cref="Year"/>.
        /// </summary>
        public static readonly DependencyProperty YearProperty = DependencyProperty.Register(
            nameof(Year), typeof(string), typeof(ReportScheduleEditor),
            new FrameworkPropertyMetadata { PropertyChangedCallback = Field_PropertyChanged, BindsTwoWayByDefault = true });

        private const int UPDATING = 1;
        private const int NOT_UPDATING = 0;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Accessed atomically to determine if this instance is currently
        /// updating the schedule or its fields.
        /// </summary>
        private int _isUpdatingFlag = NOT_UPDATING;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the report name for this instance.
        /// </summary>
        public string ReportName
        {
            get => GetValue(ReportNameProperty) as string;
            set => SetValue(ReportNameProperty, value);
        }

        /// <summary>
        /// Gets or sets the schedule for this instance.
        /// </summary>
        public string Schedule
        {
            get => GetValue(ScheduleProperty) as string;
            set => SetValue(ScheduleProperty, value);
        }

        /// <summary>
        /// Gets or sets the schedule description for this instance.
        /// </summary>
        public string ScheduleDescription
        {
            get => GetValue(ScheduleDescriptionProperty) as string;
            set => SetValue(ScheduleDescriptionProperty, value);
        }

        /// <summary>
        /// Gets or sets the seconds value for this instance.
        /// </summary>
        /// <remarks>
        /// The initial value for this property comes from <see cref="Schedule"/>.
        /// </remarks>
        public string Seconds
        {
            get => GetValue(SecondsProperty) as string;
            set => SetValue(SecondsProperty, value);
        }

        /// <summary>
        /// Gets or sets the minutes value for this instance.
        /// </summary>
        /// <remarks>
        /// The initial value for this property comes from <see cref="Schedule"/>.
        /// </remarks>
        public string Minutes
        {
            get => GetValue(MinutesProperty) as string;
            set => SetValue(MinutesProperty, value);
        }

        /// <summary>
        /// Gets or sets the hours value for this instance.
        /// </summary>
        /// <remarks>
        /// The initial value for this property comes from <see cref="Schedule"/>.
        /// </remarks>
        public string Hours
        {
            get => GetValue(HoursProperty) as string;
            set => SetValue(HoursProperty, value);
        }

        /// <summary>
        /// Gets or sets the day of month value for this instance.
        /// </summary>
        /// <remarks>
        /// The initial value for this property comes from <see cref="Schedule"/>.
        /// </remarks>
        public string DayOfMonth
        {
            get => GetValue(DayOfMonthProperty) as string;
            set => SetValue(DayOfMonthProperty, value);
        }

        /// <summary>
        /// Gets or sets the month value for this instance.
        /// </summary>
        /// <remarks>
        /// The initial value for this property comes from <see cref="Schedule"/>.
        /// </remarks>
        public string Month
        {
            get => GetValue(MonthProperty) as string;
            set => SetValue(MonthProperty, value);
        }

        /// <summary>
        /// Gets or sets the day of week value for this instance.
        /// </summary>
        /// <remarks>
        /// The initial value for this property comes from <see cref="Schedule"/>.
        /// </remarks>
        public string DayOfWeek
        {
            get => GetValue(DayOfWeekProperty) as string;
            set => SetValue(DayOfWeekProperty, value);
        }

        /// <summary>
        /// Gets or sets the Year value for this instance.
        /// </summary>
        /// <remarks>
        /// The initial value for this property comes from <see cref="Schedule"/>.
        /// </remarks>
        public string Year
        {
            get => GetValue(YearProperty) as string;
            set => SetValue(YearProperty, value);
        }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ReportScheduleEditor"/> class.
        /// </summary>
        public ReportScheduleEditor()
        {
            InitializeComponent();
        }

        private void ApplySchedule(string schedule)
        {
            const string error = "ERROR";

            if (!IsLoaded)
            {
                return;
            }

            if (string.IsNullOrEmpty(schedule) || !IsScheduleValid(schedule))
            {
                ScheduleDescription = error;
                Seconds = string.Empty;
                Minutes = string.Empty;
                Hours = string.Empty;
                DayOfMonth = string.Empty;
                Month = string.Empty;
                DayOfWeek = string.Empty;
                Year = string.Empty;
                return;
            }

            try
            {
                var scheduleParts = schedule.Split(" ");
                Seconds = scheduleParts[0];
                Minutes = scheduleParts[1];
                Hours = scheduleParts[2];
                DayOfMonth = scheduleParts[3];
                Month = scheduleParts[4];
                DayOfWeek = scheduleParts[5];
                Year = scheduleParts.Length >= 7 ? scheduleParts[6] : "";

                ScheduleDescription = ExpressionDescriptor.GetDescription(schedule);
            }
            catch (Exception scheduleExc)
            {
               _logger.Info(scheduleExc, $"Invalid schedule - {schedule}");

                ScheduleDescription = error;
                try
                {
                    ScheduleDescription = error;
                    Seconds = string.Empty;
                    Minutes = string.Empty;
                    Hours = string.Empty;
                    DayOfMonth = string.Empty;
                    Month = string.Empty;
                    DayOfWeek = string.Empty;
                    Year = string.Empty;
                }
                catch (Exception recoveryExc)
                {
                   _logger.Info(recoveryExc, "Error while recovering from previous exception.");
                }
            }
        }

        private void RefreshSchedule()
        {
            const string error = "ERROR";

            var scheduleParts = new List<string>(6)
            {
                Seconds,
                Minutes,
                Hours,
                DayOfMonth,
                Month,
                DayOfWeek
            };

            // Year is an optional field
            if (!string.IsNullOrWhiteSpace(Year))
            {
                scheduleParts.Add(Year);
            }

            if (scheduleParts.Any(p => string.IsNullOrWhiteSpace(p)))
            {
                Schedule = string.Empty;
                ScheduleDescription = error;
                return;
            }

            var schedule = string.Join(" ", scheduleParts);

            if (!IsScheduleValid(schedule))
            {
                Schedule = string.Empty;
                ScheduleDescription = error;
                return;
            }

            Schedule = schedule;

            try
            {
                ScheduleDescription = ExpressionDescriptor.GetDescription(Schedule);
            }
            catch (Exception exc)
            {
                _logger.Info(exc, $"Invalid schedule - {Schedule}");

                try
                {
                    Schedule = string.Empty;
                    ScheduleDescription = error;
                }
                catch (Exception recoveryExc)
                {
                   _logger.Info(recoveryExc, "Error while recovering from previous exception.");
                }

            }
        }

        private bool IsScheduleValid(string schedule) =>
            CronExpression.IsValidExpression(schedule);

        #endregion

        #region Events

        private static void Schedule_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ReportScheduleEditor instance))
            {
                return;
            }

            var update = false;

            try
            {
                if (Interlocked.Exchange(ref instance._isUpdatingFlag, UPDATING) == NOT_UPDATING)
                {
                    update = true;
                    instance.ApplySchedule(e.NewValue as string);
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error changing Schedule");
            }
            finally
            {
                if (update)
                {
                    instance._isUpdatingFlag = NOT_UPDATING;
                }
            }
        }

        private static void Field_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ReportScheduleEditor instance))
            {
                return;
            }

            var update = false;

            try
            {
                if (Interlocked.Exchange(ref instance._isUpdatingFlag, UPDATING) == NOT_UPDATING)
                {
                    update = true;
                    instance.RefreshSchedule();
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error changing Schedule");
            }
            finally
            {
                if (update)
                {
                    instance._isUpdatingFlag = NOT_UPDATING;
                }
            }
        }

        #endregion

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            var update = false;

            try
            {
                if (!string.IsNullOrEmpty(Schedule) && Interlocked.Exchange(ref _isUpdatingFlag, UPDATING) == NOT_UPDATING)
                {
                    update = true;
                    ApplySchedule(Schedule);
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error changing Schedule");
            }
            finally
            {
                if (update)
                {
                    _isUpdatingFlag = NOT_UPDATING;
                }
            }
        }
    }
}
