using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using DWOS.Data;
using Infragistics.Controls.Charts;
using NLog;

namespace DWOS.Dashboard.Charts
{
    /// <summary>
    /// Interaction logic for WIPHistory.xaml
    /// </summary>
    /// <remarks>
    /// Changing the name of this class may break persistence of dashboard layouts.
    /// </remarks>
    public partial class WIPHistory : UserControl, IDashboardWidget2
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        
        private string _displayName;
        public event PropertyChangedEventHandler PropertyChanged;
        private BackgroundWorker _worker = null;
        private DispatcherTimer _timer;
        
        #endregion

        #region Properties

        private WIPHistorySettings Settings { get; set; }

        #endregion

        #region Methods

        public WIPHistory()
        {
            InitializeComponent();

            this.Settings = new WIPHistorySettings() { HistoryDays = 10 };
            this.DisplayName = "WIP History";
            
            settingsPanel.Visibility = System.Windows.Visibility.Collapsed;
            settingsPanel.DataContext = this.Settings;
        }
        
        public void Start()
        {
            try
            {
                this.DisplayName = "WIP History";

                if(_timer == null)
                {
                    _timer = new DispatcherTimer(TimeSpan.FromSeconds(30), DispatcherPriority.Normal, (s, e) => Start(), System.Windows.Application.Current.Dispatcher) { IsEnabled = false };
                    Settings.PropertyChanged += Settings_PropertyChanged;
                }
                
                _timer.IsEnabled = false;

                if(_worker == null)
                {
                    _worker = new BackgroundWorker() {WorkerSupportsCancellation = true};
                    _worker.DoWork += (s, e) =>
                                      {
                                          e.Result = this.GetData();
                                      };
                    _worker.RunWorkerCompleted += (s, e) =>
                                                  {
                                                      if (e.Cancelled || e.Error != null)
                                                          return;

                                                      if (e.Result is List<WIPCount>)
                                                      {
                                                          var view = e.Result as List<WIPCount>;
                                                         
                                                          var xAxis = chart.Axes.OfType<CategoryXAxis>().First();
                                                          xAxis.ItemsSource = view;
                                                          chart.Series[0].ItemsSource = view;
                                                      }

                                                      _timer.IsEnabled = true;
                                                  };
                }

                if(_worker.IsBusy)
                    return;

                _worker.RunWorkerAsync();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error loading current summary data.");
            }
        }
        
        public void Stop()
        {
            if(_timer != null)
            {
                _timer.IsEnabled = false;
                _timer = null;
            }

            if(_worker != null)
            {
                if(_worker.IsBusy)
                    _worker.CancelAsync();

                _worker.Dispose();
                _worker = null;
            }
        }

        public void OnDepartmentsChanged()
        {
        }

        private List<WIPCount> GetData()
        {
            try
            {
                using (var ta = new Data.Reports.DashboardTableAdapters.WIPCountsTableAdapter() {ClearBeforeFill = true})
                {
                    var currentWeek = DateTime.Now.AddDays(-1).StartOfWeek(DayOfWeek.Monday);
                    var wipCounts = new List <WIPCount>();

                    for(int index = 0; index < Settings.HistoryDays; index++)
                    {
                        var fromDate = currentWeek.StartOfDay();
                        var toDate = currentWeek.AddDays(6).EndOfDay();
                        var data = ta.GetData(toDate, fromDate);

                        var row = data.FirstOrDefault();

                        if(row != null)
                            wipCounts.Add(new WIPCount() { Date = fromDate, OrderCount = row.IsOrdersNull() ? 0 : row.Orders, PartCount = row.IsPartsNull() ? 0 : row.Parts });

                        currentWeek = currentWeek.AddDays(-7);
                    }

                    wipCounts.Sort((f1, f2) => f1.Date.CompareTo(f2.Date));

                    return wipCounts;
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error getting data.");
                return null;
            }
        }

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }


        public class WIPCount
        {
            public DateTime Date { get; set; }
            public int OrderCount { get; set; }
            public int PartCount { get; set; }
        }

        #endregion

        #region Events

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(settingsPanel.Visibility == System.Windows.Visibility.Collapsed)
            {
                settingsPanel.DataContext = this.Settings;
                settingsPanel.Visibility = System.Windows.Visibility.Visible;
                chart.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                settingsPanel.Visibility = System.Windows.Visibility.Collapsed;
                chart.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Start();
        }
        
        #endregion

        #region IDashboardWidget

        public string DisplayName
        {
            get { return this._displayName; }
            set
            {
                this._displayName = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("DisplayName"));
            }
        }

        public System.Windows.Controls.Control Control
        {
            get { return this; }
        }

        WidgetSettings IDashboardWidget2.Settings
        {
            get { return this.Settings; }
            set { this.Settings = value as WIPHistorySettings; }
        }

        #endregion

        #region PriorityHistorySettings

        [DataContract]
        public class WIPHistorySettings : WidgetSettings, INotifyPropertyChanged
        {
            private int _historyDays;

            [DataMember]
            public int HistoryDays
            {
                get { return this._historyDays; }
                set
                {
                    this._historyDays = value;
                    OnPropertyChanged("HistoryDays");
                }
            }
        }

        #endregion
    }
}
