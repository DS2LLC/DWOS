using DWOS.Data;
using NLog;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace DWOS.Dashboard.Charts
{
    /// <summary>
    /// Interaction logic for QAByLocation2.xaml
    /// </summary>
    /// <remarks>
    /// Changing the name of this class may break persistence of dashboard layouts.
    /// </remarks>
    public partial class QAByLocation2 : UserControl, IDashboardWidget2
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private string _displayName;
        private BackgroundWorker _worker = null;
        private DispatcherTimer _timer;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        private QAByLocationSettings Settings { get; set; }

        #endregion

        #region Methods

        public QAByLocation2()
        {
            InitializeComponent();

            this.Settings = new QAByLocationSettings()
            {
                CountByType = QAByLocationSettings.CountBy.Orders
            };

            this.DisplayName = "Inspection By Department";

            settingsPanel.Visibility = System.Windows.Visibility.Collapsed;
            settingsPanel.DataContext = this.Settings;
            cboType.ItemsSource = Enum.GetValues(typeof(QAByLocationSettings.CountBy)).Cast<QAByLocationSettings.CountBy>();
        }

        public void Start()
        {
            try
            {
                this.DisplayName = this.Settings.CountByType == QAByLocationSettings.CountBy.Orders ? "Inspection Orders By Department" : "Inspection Parts By Department";

                if(_timer == null)
                {
                    _timer = new DispatcherTimer(TimeSpan.FromSeconds(30), DispatcherPriority.Normal, (s, e) => Start(), System.Windows.Application.Current.Dispatcher) { IsEnabled = false };
                    Settings.PropertyChanged += Settings_PropertyChanged;
                }

                _timer.IsEnabled = false;

                if (_worker == null)
                {
                    _worker = new BackgroundWorker() { WorkerSupportsCancellation = true };
                    _worker.DoWork += (s, e) =>
                    {
                        e.Result = this.GetData();
                    };
                    _worker.RunWorkerCompleted += (s, e) =>
                    {
                        if (e.Cancelled || e.Error != null)
                            return;

                        if (e.Result is DataTable)
                        {
                            var lateOrdersByDept = e.Result as DataTable;

                            //update chart
                            chart.ValueMemberPath = "Count";
                            chart.LabelMemberPath = "Department";
                            chart.ItemsSource = lateOrdersByDept.DefaultView;
                        }

                        _timer.IsEnabled = true;
                    };
                }

                if (_worker.IsBusy)
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
            if (_timer != null)
            {
                _timer.IsEnabled = false;
                _timer = null;
            }

            if (_worker != null)
            {
                if (_worker.IsBusy)
                    _worker.CancelAsync();

                _worker.Dispose();
                _worker = null;
            }
        }

        public void OnDepartmentsChanged()
        {
        }
        
        private DataTable GetData()
        {
            var lateOrdersByDept = new DataTable();
            var deptCol = lateOrdersByDept.Columns.Add("Department", typeof(string));
            lateOrdersByDept.Columns.Add("Count", typeof(int));
            lateOrdersByDept.PrimaryKey = new DataColumn[] { deptCol };

            using (var ta = new Data.Reports.DashboardTableAdapters.OrderSummaryTableAdapter())
            {
                var lateOrders = ta.GetData();

                foreach (var lateOrder in lateOrders)
                {
                    if (lateOrder.WorkStatus == "Pending Inspection")
                    {
                        var department = lateOrder.CurrentLocation;
                        var row = lateOrdersByDept.Rows.Find(department);

                        if (this.Settings.CountByType == QAByLocationSettings.CountBy.Orders)
                        {
                            if (row == null)
                                lateOrdersByDept.Rows.Add(department, 1);
                            else
                                row["Count"] = Convert.ToInt32(row["Count"]) + 1;
                        }
                        else if (!lateOrder.IsPartQuantityNull())
                        {
                            if (row == null)
                                lateOrdersByDept.Rows.Add(department, lateOrder.PartQuantity);
                            else
                                row["Count"] = Convert.ToInt32(row["Count"]) + lateOrder.PartQuantity;
                        }
                    }
                }


                return SortTable(lateOrdersByDept, "Department", "ASC");
            }
        }
        
        private static DataTable SortTable(DataTable input, string columnName, string direction)
        {
            input.DefaultView.Sort = columnName + " " + direction;
            return input.DefaultView.ToTable();
        }

        #endregion

        #region Events

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (settingsPanel.Visibility == System.Windows.Visibility.Collapsed)
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

                if(PropertyChanged != null)
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
            set { this.Settings = value as QAByLocationSettings; }
        }

        #endregion

        #region QAByLocationSettings

        [DataContract]
        public class QAByLocationSettings : WidgetSettings
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
                    OnPropertyChanged("CountByType");
                }
            }
        }

        #endregion
    }
}
