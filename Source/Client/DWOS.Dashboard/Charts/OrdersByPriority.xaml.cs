using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using NLog;

namespace DWOS.Dashboard.Charts
{
    /// <summary>
    /// Interaction logic for OrdersByPriority.xaml
    /// </summary>
    /// <remarks>
    /// Changing the name of this class may break persistence of dashboard layouts.
    /// </remarks>
    public partial class OrdersByPriority : UserControl, IDashboardWidget2
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        
        private string _displayName;
        public event PropertyChangedEventHandler PropertyChanged;
        private BackgroundWorker _worker = null;
        private DispatcherTimer _timer;
        
        #endregion

        #region Properties

        private OrderByPrioritySettings Settings { get; set; }

        #endregion

        #region Methods

        public OrdersByPriority()
        {
            InitializeComponent();

            this.Settings = new OrderByPrioritySettings() { CountByType = OrderByPrioritySettings.CountBy.Orders };
            this.DisplayName = "Orders By Priority";
            
            settingsPanel.Visibility = System.Windows.Visibility.Collapsed;
            settingsPanel.DataContext = this.Settings;
            cboType.ItemsSource = Enum.GetValues(typeof(OrderByPrioritySettings.CountBy)).Cast<OrderByPrioritySettings.CountBy>();
        }
        
        public void Start()
        {
            try
            {
                this.DisplayName = this.Settings.CountByType == OrderByPrioritySettings.CountBy.Orders ? "Orders By Priority" : "Parts By Priority";

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
                                                          var table = e.Result as DataView;

                                                            //update chart
                                                            chart.ValueMemberPath = "Count";
                                                            chart.LabelMemberPath = "Priority";
                                                            chart.ItemsSource = table;
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
            using (var ta = new Data.Reports.DashboardTableAdapters.OrderPrioritiesTableAdapter())
            {
                var table = ta.GetData();
                
                if(this.Settings.CountByType == OrderByPrioritySettings.CountBy.Orders)
                    table.OrderCountColumn.ColumnName = "Count";
                else
                    table.PartCountColumn.ColumnName = "Count";

                return new DataView(table) { Sort = "Count DESC" };
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
                legend.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                settingsPanel.Visibility = System.Windows.Visibility.Collapsed;
                chart.Visibility = System.Windows.Visibility.Visible;
                legend.Visibility = System.Windows.Visibility.Visible;
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
            set { this.Settings = value as OrderByPrioritySettings; }
        }

        #endregion

        #region LateByLocationSettings

        [DataContract]
        public class OrderByPrioritySettings : WidgetSettings, INotifyPropertyChanged
        {
            private CountBy _countByType;

            public enum CountBy { Orders, Parts }

            [DataMember]
            public CountBy CountByType
            {
                get { return this._countByType; }
                set
                {
                    this._countByType = value;
                    OnPropertyChanged("CountBy");
                }
            }
        }

        #endregion
    }
}
