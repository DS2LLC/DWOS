using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Infragistics.Controls.Charts;
using NLog;

namespace DWOS.Dashboard.Charts
{
    /// <summary>
    /// Interaction logic for PriorityHistory.xaml
    /// </summary>
    /// <remarks>
    /// Changing the name of this class may break persistence of dashboard layouts.
    /// </remarks>
    public partial class PriorityHistory : UserControl, IDashboardWidget2
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        
        private string _displayName;
        public event PropertyChangedEventHandler PropertyChanged;
        private BackgroundWorker _worker = null;
        private DispatcherTimer _timer;
        
        #endregion

        #region Properties

        private PriorityHistorySettings Settings { get; set; }

        #endregion

        #region Methods

        public PriorityHistory()
        {
            InitializeComponent();

            this.Settings = new PriorityHistorySettings() { HistoryDays = 120 };
            this.DisplayName = "Order Priority History";
            
            settingsPanel.Visibility = System.Windows.Visibility.Collapsed;
            settingsPanel.DataContext = this.Settings;
        }
        
        public void Start()
        {
            try
            {
                this.DisplayName = "Order Priority History";

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

                                                      if(e.Result is DataView)
                                                      {
                                                          var view = e.Result as DataView;
                                                         
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

        private DataView GetData()
        {
            try
            {
                using (var ta = new Data.Reports.DashboardTableAdapters.OrderPriorityHistoryTableAdapter())
                {
                    var table = ta.GetData(Settings.NormalPriority, Settings.HistoryDays);
                    var summedValues = new Dictionary<DateTime, int>();

                    foreach (var row in table)
                    {
                        var date = Convert.ToDateTime(row.OrderDate).Date;
                        if (!summedValues.ContainsKey(date))
                            summedValues.Add(date, row.OrderCount);
                        else
                            summedValues[date] += row.OrderCount;
                    }

                    var combinedTable = new DataTable();
                    combinedTable.Columns.Add("OrderDate", typeof(DateTime));
                    combinedTable.Columns.Add("OrderCount", typeof(int));

                    foreach (var summedValue in summedValues)
                    {
                        combinedTable.Rows.Add(summedValue.Key, summedValue.Value);
                    }

                    var view = combinedTable.DefaultView;
                    view.Sort = "OrderDate";
                    return view;
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error getting data.");
                return null;
            }
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
            set { this.Settings = value as PriorityHistorySettings; }
        }

        #endregion

        #region PriorityHistorySettings

        [DataContract]
        public class PriorityHistorySettings : WidgetSettings, INotifyPropertyChanged
        {
            private int _historyDays;
            private string _normalPriority = "Normal";

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

            [DataMember]
            public string NormalPriority
            {
                get { return this._normalPriority; }
                set
                {
                    this._normalPriority = value;
                    OnPropertyChanged("NormalPriority");
                }
            }
        }

        #endregion
    }
}
