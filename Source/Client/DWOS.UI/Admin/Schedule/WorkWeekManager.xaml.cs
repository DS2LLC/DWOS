using System;
using System.Windows;
using System.Windows.Input;
using DWOS.Data.Datasets.ScheduleDatasetTableAdapters;
using DWOS.UI.Utilities;
using NLog;
using System.Linq;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.CommandWpf;

namespace DWOS.UI.Admin.Schedule
{
    /// <summary>
    /// Interaction logic for WorkWeekManager.xaml
    /// </summary>
    public partial class WorkWeekManager
    {
        #region Properties

        private WorkWeekDataContext ViewModel =>
            DataContext as WorkWeekDataContext;

        #endregion

        #region Methods

        public WorkWeekManager()
        {
            InitializeComponent();
            DataContext = new WorkWeekDataContext();
            Icon = Properties.Resources.Calendar2_32.ToWpfImage();
        }

        public void LoadData()
        {
            ViewModel?.LoadData();
        }

        #endregion

        #region Events

        private void WorkWeekManager_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;

            if (vm != null)
            {
                vm.Saved += ViewModelOnSaved;
            }
        }

        private void WorkWeekManager_OnUnloaded(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;

            if (vm != null)
            {
                vm.Saved -= ViewModelOnSaved;
            }
        }

        private void ViewModelOnSaved(object sender, EventArgs e)
        {
            DialogResult = true;
        }

        #endregion

        #region WorkWeekDataContext

        private class WorkWeekDataContext
        {
            #region Fields

            public event EventHandler Saved;
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

            #endregion

            #region Properties

            public ObservableCollection<WorkWeekDay> WorkWeekDays { get; } =
                new ObservableCollection<WorkWeekDay>();

            public ICommand Save { get; }

            private bool CanSave => WorkWeekDays.Any(w => w.IsWorkday)
                    && WorkWeekDays.All(w => string.IsNullOrEmpty(w.ValidateAll()));

            #endregion

            #region Methods

            public WorkWeekDataContext()
            {
                Save = new RelayCommand(DoSave, () => CanSave);
            }

            public void LoadData()
            {
                using (var taDay = new DayOfWeekTableAdapter())
                {
                    using (var dtDay = taDay.GetData())
                    {
                        foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                        {
                            var dayRow = dtDay.FindByDayOfWeekID((int)day);

                            if (dayRow != null)
                            {
                                WorkWeekDays.Add(new WorkWeekDay
                                {
                                    DayOfWeek = (DayOfWeek)dayRow.DayOfWeekID,
                                    IsWorkday = dayRow.IsWorkday,
                                    WorkdayStart = dayRow.WorkdayStart,
                                    WorkdayEnd = dayRow.WorkdayEnd
                                });
                            }
                            else
                            {
                                _logger.Warn($"Did not find existing row for {day}.");
                                WorkWeekDays.Add(new WorkWeekDay
                                {
                                    DayOfWeek = day,
                                    IsWorkday = false,
                                    WorkdayStart = new TimeSpan(9, 0, 0),
                                    WorkdayEnd = new TimeSpan(17, 0, 0)
                                });
                            }
                        }
                    }
                }
            }

            private void DoSave()
            {
                using (var taDay = new DayOfWeekTableAdapter())
                {
                    using (var dtDay = taDay.GetData())
                    {
                        foreach (var dayEntry in WorkWeekDays)
                        {
                            var row = dtDay.FindByDayOfWeekID((int)dayEntry.DayOfWeek);

                            if (row != null)
                            {
                                row.IsWorkday = dayEntry.IsWorkday;
                                row.WorkdayStart = dayEntry.WorkdayStart;
                                row.WorkdayEnd = dayEntry.WorkdayEnd;
                            }
                            else
                            {
                                _logger.Error($"Did not find existing row for {dayEntry.DayOfWeek}.");
                                dtDay.AddDayOfWeekRow((int)dayEntry.DayOfWeek,
                                    dayEntry.IsWorkday,
                                    dayEntry.WorkdayStart,
                                    dayEntry.WorkdayEnd);
                            }
                        }

                        taDay.Update(dtDay);
                    }
                }

                Saved?.Invoke(this, EventArgs.Empty);
            }

            #endregion
        }

        #endregion

        #region WorkWeekDay

        internal class WorkWeekDay : ViewModelBase
        {
            #region Fields

            private DayOfWeek _dayOfWeek;
            private bool _isWorkday;
            private TimeSpan _workdayStart;
            private TimeSpan _workdayEnd;

            #endregion

            #region Properties

            public DayOfWeek DayOfWeek
            {
                get => _dayOfWeek;
                set
                {
                    if (_dayOfWeek != value)
                    {
                        _dayOfWeek = value;
                        RaisePropertyChanged(nameof(DayOfWeek));
                    }
                }
            }

            public bool IsWorkday
            {
                get => _isWorkday;
                set
                {
                    if (_isWorkday != value)
                    {
                        _isWorkday = value;
                        RaisePropertyChanged(nameof(IsWorkday));
                    }
                }
            }

            public TimeSpan WorkdayStart
            {
                get => _workdayStart;
                set
                {
                    if (_workdayStart != value)
                    {
                        _workdayStart = value;
                        RaisePropertyChanged(nameof(WorkdayStart));
                    }
                }
            }

            public TimeSpan WorkdayEnd
            {
                get => _workdayEnd;
                set
                {
                    if (_workdayEnd != value)
                    {
                        _workdayEnd = value;
                        RaisePropertyChanged(nameof(WorkdayEnd));
                    }
                }
            }

            #endregion

            #region Methods

            public override string Validate(string propertyName)
            {
                if (propertyName == nameof(WorkdayStart) && _workdayStart >= _workdayEnd)
                {
                    return "Start time needs to come before end time.";
                }

                return null;
            }

            public override string ValidateAll()
            {
                return Validate(nameof(WorkdayStart));
            }

            #endregion
        }

        #endregion
    }
}
