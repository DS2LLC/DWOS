using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace DWOS.Dashboard.Charts
{
    /// <summary>
    /// Interaction logic for OrdersDue.xaml
    /// </summary>
    /// <remarks>
    /// Changing the name of this class may break persistence of dashboard layouts.
    /// </remarks>
    public partial class OrdersDue : UserControl, IDashboardWidget2
    {
        #region Fields

        /// <summary>
        /// Default amount of time to wait between loading data and
        /// refreshing it.
        /// </summary>
        private static readonly TimeSpan TIMER_INTERVAL = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Date format for orders due counts.
        /// </summary>
        private const string ORDERS_DUE_DATE_FORMAT = "MM/dd";

        private DispatcherTimer _timer;
        private EventHandler _timerEventHandler;
        private CancellationTokenSource _backendTaskTokenSource;
        private OrdersDueContext _context;

        /// <summary>
        /// Lock used to ensure that only one thread can update the
        /// list of orders due counts.
        /// </summary>
        private object _ordersDueLock;

        private bool _initialLoad;

        #endregion

        #region Methods

        /// <summary>
        /// Default constructor.
        /// </summary>
        public OrdersDue()
        {
            InitializeComponent();
            _context = new OrdersDueContext();
            DataContext = _context;
            Dispatcher.ShutdownFinished += Dispatcher_ShutdownFinished;

            _ordersDueLock = new object();
            _initialLoad = true;
        }

        /// <summary>
        /// Gets a list of 'orders due count' items.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<OrdersDueCount> GetOrderDueCounts()
        {
            var ordersDue = new List<OrdersDueCount>();
            var lateOrdersDueCount = new OrdersDueCount()
            {
                Day = "Late"
            };

            ordersDue.Add(lateOrdersDueCount);

            var toDate = DateTime.Today.AddDays(_context.Settings.Days);
            var fromDate = DateTime.Today;

            var daysSpan = toDate.Subtract(fromDate).Days;

            for (int x = 0; x < daysSpan; x++)
            {
                ordersDue.Add(new OrdersDueCount()
                {
                    Day = fromDate.AddDays(x).ToString(ORDERS_DUE_DATE_FORMAT)
                });
            }

            using (var taOrderSummary = new DWOS.Data.Reports.DashboardTableAdapters.OrderSummaryTableAdapter())
            {
                foreach (var order in taOrderSummary.GetData().Where(summary => !summary.IsEstShipDateNull()))
                {
                    OrdersDueCount match = null;
                    if (order.EstShipDate >= fromDate && order.EstShipDate <= toDate)
                    {
                        match = ordersDue.FirstOrDefault(r => r.Day == order.EstShipDate.ToString(ORDERS_DUE_DATE_FORMAT));
                        if (match != null)
                        {
                            match.Count++;
                        }
                    }
                    else if (order.EstShipDate < fromDate)
                    {
                        lateOrdersDueCount.Count++;
                    }
                }
            }

            return ordersDue;
        }

        /// <summary>
        /// Task continuation method that actually shows 'order due count'
        /// results in the UI.
        /// </summary>
        /// <remarks>
        /// This method must run on the UI thread.
        /// </remarks>
        /// <param name="originalTask">task to continue from</param>
        /// <returns>same list of due counts</returns>
        private IEnumerable<OrdersDueCount> OrderTaskContinuation(Task<IEnumerable<OrdersDueCount>> originalTask)
        {
            IEnumerable<OrdersDueCount> ordersDueCounts = originalTask.Result;

            if (!_backendTaskTokenSource.Token.IsCancellationRequested)
            {
                lock (_ordersDueLock)
                {
                    _context.OrdersDue.Clear();

                    foreach (var ordersDueCount in ordersDueCounts)
                    {
                        _context.OrdersDue.Add(ordersDueCount);
                    }
                }
            }

            return ordersDueCounts;
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

        #endregion

        #region IDashboardWidget2 Members

        public string DisplayName
        {
            get
            {
                return "Orders Due";
            }
        }

        public Control Control
        {
            get { return this; }
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
                    var valueAsTypedSetting = value as OrdersDueSettings;
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
                        var task = new Task<IEnumerable<OrdersDueCount>>(GetOrderDueCounts, _backendTaskTokenSource.Token);
                        var taskContinuation = task.ContinueWith<IEnumerable<OrdersDueCount>>(OrderTaskContinuation, _backendTaskTokenSource.Token, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
                        taskContinuation.ContinueWith((ordersDueCounts) =>
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
        }

#pragma warning disable 67
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 67

        #endregion

        #region Events

        void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Start();
        }

        /// <summary>
        /// Toggles display of settings grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (orderChart.Visibility == Visibility.Visible)
            {
                orderChart.Visibility = Visibility.Collapsed;
                settingsGrid.Visibility = Visibility.Visible;
            }
            else
            {
                orderChart.Visibility = Visibility.Visible;
                settingsGrid.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region OrdersDueContext

        /// <summary>
        /// DataContext for the OrdersDue widget.
        /// </summary>
        public sealed class OrdersDueContext : INotifyPropertyChanged
        {
            #region Fields

            private ObservableCollection<OrdersDueCount> _ordersDue = new ObservableCollection<OrdersDueCount>();
            private OrdersDueSettings _settings = new OrdersDueSettings();

            #endregion

            #region Properties

            /// <summary>
            /// Gets a list of orders due counts.
            /// </summary>
            public ObservableCollection<OrdersDueCount> OrdersDue
            {
                get
                {
                    return _ordersDue;
                }
            }


            /// <summary>
            /// Gets widget settings.
            /// </summary>
            /// <remarks>
            /// This is a data context property (instead of a property of the 
            /// widget) for ease of data binding.
            /// </remarks>
            public OrdersDueSettings Settings
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

            #endregion

            #region Methods

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

        #region OrdersDueCount

        /// <summary>
        /// Represents the number of orders due on a specific day.
        /// </summary>
        public sealed class OrdersDueCount
        {
            /// <summary>
            /// Date that this count covers.
            /// </summary>
            /// <remarks>
            /// The type of this property was changed from DateTime to string
            /// to support having 'Late' as a value.
            /// </remarks>
            public string Day { get; set; }

            public int Count { get; set; }
        }

        #endregion

        #region OrdersDueSettings

        /// <summary>
        /// Represents settings for the OrdersDue widget.
        /// </summary>
        [DataContract]
        public class OrdersDueSettings : WidgetSettings
        {
            #region Fields

            private const int DEFAULT_DAYS = 10;
            private int _days;

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets the number of days to span.
            /// </summary>
            [DataMember]
            public int Days
            {
                get
                {
                    return _days;
                }
                set
                {
                    if (_days != value)
                    {
                        _days = value;
                        OnPropertyChanged("Days");
                    }
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Default constructor.
            /// </summary>
            public OrdersDueSettings()
            {
                Days = DEFAULT_DAYS;
            }

            #endregion
        }

        #endregion
    }
}
