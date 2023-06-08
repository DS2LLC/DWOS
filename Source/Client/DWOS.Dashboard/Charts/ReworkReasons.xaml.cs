using DWOS.Dashboard.Charts.Controls;
using DWOS.Data.Reports.DashboardTableAdapters;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace DWOS.Dashboard.Charts
{
    /// <summary>
    /// Interaction logic for ReworkReasons.xaml
    /// </summary>
    /// <remarks>
    /// Changing the name of this class may break persistence of dashboard layouts.
    /// </remarks>
    public partial class ReworkReasons : UserControl, IDashboardWidget2
    {
        #region Fields

        private static Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Default amount of time to wait between loading data and
        /// refreshing it.
        /// </summary>
        private static readonly TimeSpan TIMER_INTERVAL = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Gets a read-only list of property names for properties that can be
        /// directly set by the user.
        /// </summary>
        private static readonly ReadOnlyCollection<string> USER_SETTINGS = new ReadOnlyCollection<string>(new List<string>()
        {
            "Days",
            "DepartmentID",
            "GroupBy",
            "ShowParetoChart",
        });

        private ReworkReasonsContext _context;
        private string _displayName;

        /// <summary>
        /// A 'just in case' lock used to ensure that only one thread can
        /// update the UI.
        /// </summary>
        private readonly object _uiLock;

        private DispatcherTimer _timer;
        private EventHandler _timerEventHandler;
        private CancellationTokenSource _backendTaskTokenSource;
        private bool _initialLoad;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ReworkReasons"/>
        /// class.
        /// </summary>
        public ReworkReasons()
        {
            InitializeComponent();
            _context = new ReworkReasonsContext();
            _uiLock = new object();
            DataContext = _context;
            _initialLoad = true;
            Dispatcher.ShutdownFinished += Dispatcher_ShutdownFinished;
        }

        /// <summary>
        /// Retrieves rework entries from the database.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<DWOS.Data.Reports.Dashboard.ReworkEntryRow> GetReworkEntries()
        {
            if (string.IsNullOrEmpty(_context.Settings.DepartmentID))
            {
                // Refresh automatically set DepartmentID  to null.
                return Enumerable.Empty<Data.Reports.Dashboard.ReworkEntryRow>();
            }

            DWOS.Data.Reports.Dashboard.ReworkEntryDataTable dtRework = null;

            switch (_context.Settings.DepartmentID)
            {
                case ReworkReasonsSettings.DEPARTMENT_ALL:
                    _log.Debug("Getting info for all departments.");
                    using (var taReworkEntry = new ReworkEntryTableAdapter())
                    {
                        dtRework = taReworkEntry.GetData(_context.Settings.FromDate, _context.Settings.ToDate);
                    }

                    break;
                default:
                    // Get by department
                    _log.Debug("Getting info for department \"{0}\".", _context.Settings.DepartmentID);
                    using (var taReworkEntry = new ReworkEntryTableAdapter())
                    {
                        dtRework = taReworkEntry.GetDataByDepartment(_context.Settings.FromDate, _context.Settings.ToDate, _context.Settings.DepartmentID);
                    }

                    break;
            }

            var returnValue = dtRework.ToList();
            var first = dtRework.FirstOrDefault();
            var last = dtRework.LastOrDefault();

            _log.Debug("Rework Item Info\n\tCount: {0}\n\tFirst InternalReworkID:{1}\n\tLast InternalReworkID:{2}",
                returnValue.Count,
                first == null ? "N/A" : first.InternalReworkID.ToString(),
                last == null ? "N/A" : last.InternalReworkID.ToString());

            return returnValue;
        }

        /// <summary>
        /// Task continuation method that actually shows rework results in
        /// the chart.
        /// </summary>
        /// <remarks>
        /// This method must run on a UI thread.
        /// </remarks>
        /// <param name="originalTask">task to continue from</param>
        private void ReworkTaskContinuation(Task<IEnumerable<DWOS.Data.Reports.Dashboard.ReworkEntryRow>> originalTask)
        {
            try
            {
                if (originalTask.IsFaulted)
                {
                    if (originalTask.Exception != null)
                    {
                        foreach (var innerException in originalTask.Exception.InnerExceptions)
                        {
                            _log.Error(innerException, "Error retrieving rework data.");
                        }
                    }

                    return;
                }

                var results = originalTask.Result;

                lock (_uiLock)
                {
                    if (_context.Settings.DepartmentID == ReworkReasonsSettings.DEPARTMENT_ALL)
                    {
                        DisplayName = "Rework Reasons";
                    }
                    else
                    {
                        DisplayName = string.Format("{0} Rework Reasons", _context.Settings.DepartmentID);
                    }

                    chartGrid.Show(results, _context.Settings);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "There was a problem with updating the UI.");
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// This handler performs object cleanup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Dispatcher_ShutdownFinished(object sender, EventArgs e)
        {
            if (_backendTaskTokenSource != null)
            {
                _backendTaskTokenSource.Dispose();
            }
        }

        void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "GroupBy")
            {
                if (_context.Settings.GroupBy == ReworkReasonsSettings.GroupByType.Reason)
                {
                    showParetoCheckbox.Visibility = Visibility.Visible;
                }
                else
                {
                    showParetoCheckbox.Visibility = Visibility.Collapsed;
                }
            }

            if (USER_SETTINGS.Contains(e.PropertyName))
            {
                Start();
            }
        }

        /// <summary>
        /// Toggles display of settings grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (chartGrid.Visibility == Visibility.Visible)
            {
                chartGrid.Visibility = Visibility.Collapsed;
                settingsGrid.Visibility = Visibility.Visible;
            }
            else
            {
                chartGrid.Visibility = Visibility.Visible;
                settingsGrid.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region IDashboardWidget2 Members

        public string DisplayName
        {
            get
            {
                return _displayName;
            }

            set
            {
                if (_displayName != value)
                {
                    _displayName = value;

                    var handler = PropertyChanged;

                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("DisplayName"));
                    }
                }
            }
        }

        public Control Control
        {
            get
            {
                return this;
            }
        }

        public WidgetSettings Settings
        {
            get
            {
                WidgetSettings returnValue;
                if (_context == null)
                {
                    returnValue = null;
                }
                else
                {
                    returnValue = _context.Settings;
                }

                return returnValue;
            }
            set
            {
                if (_context != null)
                {
                    var valueAsTypedSetting = value as ReworkReasonsSettings;
                    if (_context.Settings != valueAsTypedSetting)
                    {
                        _context.Settings = valueAsTypedSetting;
                    }
                }
            }
        }

        public void Start()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TIMER_INTERVAL;
            }
            else
            {
                _timer.Stop();
            }

            if (_backendTaskTokenSource == null)
            {
                _backendTaskTokenSource = new CancellationTokenSource();
            }

            if (_timerEventHandler == null)
            {
                _timerEventHandler = (sender, eventArgs) =>
                    {
                        _timer.Stop();

                        var task = new Task<IEnumerable<DWOS.Data.Reports.Dashboard.ReworkEntryRow>>(GetReworkEntries, _backendTaskTokenSource.Token);
                        var taskContinuation = task.ContinueWith(ReworkTaskContinuation, _backendTaskTokenSource.Token, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
                        taskContinuation.ContinueWith((reworkEntries) =>
                            {
                                if (!_backendTaskTokenSource.Token.IsCancellationRequested)
                                {
                                    _timer.Start();
                                }
                            }, _backendTaskTokenSource.Token);

                        task.Start();
                    };

                _timer.Tick += _timerEventHandler;
            }

            _timerEventHandler(_timer, new EventArgs());

            if (_initialLoad)
            {
                Settings.PropertyChanged += Settings_PropertyChanged;
                _initialLoad = false;

                if (_context.Settings.GroupBy == ReworkReasonsSettings.GroupByType.Reason)
                {
                    showParetoCheckbox.Visibility = Visibility.Visible;
                }
                else
                {
                    showParetoCheckbox.Visibility = Visibility.Collapsed;
                }
            }
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }

            if (_backendTaskTokenSource != null)
            {
                _backendTaskTokenSource.Cancel();
                _backendTaskTokenSource = null;
            }
        }

        public void OnDepartmentsChanged()
        {
            _context.RefreshDepartments();
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region ReworkReasonsContext

        /// <summary>
        /// DataContext for the <see cref="ReworkReasons"/> widget.
        /// </summary>
        public sealed class ReworkReasonsContext : INotifyPropertyChanged
        {
            #region Fields

            private ReworkReasonsSettings _settings;
            private readonly IEnumerable<ReworkReasonsSettings.GroupByType> _groupByItems;

            #endregion

            #region Properties

            /// <summary>
            /// Gets widget settings.
            /// </summary>
            /// <remarks>
            /// This is a data context property (instead of a property of the 
            /// widget) for ease of data binding.
            /// </remarks>
            public ReworkReasonsSettings Settings
            {
                get
                {
                    return _settings;
                }
                set
                {
                    if (_settings != value)
                    {
                        _settings = value;
                        OnPropertyChanged("Settings");
                    }
                }
            }

            /// <summary>
            /// Gets a list of 'group by' items for users to select from.
            /// </summary>
            public IEnumerable<ReworkReasonsSettings.GroupByType> GroupByItems
            {
                get { return _groupByItems; }
            }

            /// <summary>
            /// Gets a list of departments for users to select from.
            /// </summary>
            /// <remarks>
            /// This includes an option equal to
            /// <see cref="ReworkReasonsSettings.DEPARTMENT_ALL"/> that means
            /// "all departments."
            /// </remarks>
            public ObservableCollection<string> Departments { get; } =
                new ObservableCollection<string>();

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="ReworkReasonsContext"/> class.
            /// </summary>
            public ReworkReasonsContext()
            {
                _settings = new ReworkReasonsSettings();
                _groupByItems = Enum.GetValues(typeof(ReworkReasonsSettings.GroupByType)).OfType<ReworkReasonsSettings.GroupByType>();

                RefreshDepartments();
            }

            public void RefreshDepartments()
            {
                string currentDepartmentID = _settings.DepartmentID;
                int selectedIndex = 0;

                if (Departments.Count > 0)
                {
                    selectedIndex = Math.Max(Departments.IndexOf(_settings.DepartmentID), 0);
                }

                Departments.Clear();
                Departments.Add(ReworkReasonsSettings.DEPARTMENT_ALL);

                using (var taDept = new d_DepartmentTableAdapter())
                {
                    foreach (var departmentID in taDept.GetDataTable().GetDistinctColumnValues("DepartmentID"))
                    {
                        Departments.Add(departmentID);
                    }
                }

                if (Departments.Contains(currentDepartmentID))
                {
                    // Clearing Departments sets _settings.DepartmentID to null
                    _settings.DepartmentID = currentDepartmentID;
                }
                else if (Departments.Count > selectedIndex)
                {
                    _settings.DepartmentID = Departments[selectedIndex];
                }
                else
                {
                    _settings.DepartmentID = ReworkReasonsSettings.DEPARTMENT_ALL;
                }
            }

            private void OnPropertyChanged(string name)
            {
                var handler = PropertyChanged;

                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

        }

        #endregion
    }
}
