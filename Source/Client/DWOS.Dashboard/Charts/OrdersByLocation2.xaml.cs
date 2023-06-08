using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using NLog;
using DWOS.Data;

namespace DWOS.Dashboard.Charts
{
    /// <summary>
    /// Interaction logic for OrdersByLocation2.xaml
    /// </summary>
    /// <remarks>
    /// Changing the name of this class may break persistence of dashboard layouts.
    /// </remarks>
    public partial class OrdersByLocation2 : UserControl, IDashboardWidget2, INotifyPropertyChanged
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private string _displayName;
        private BackgroundWorker _worker = null;
        private DispatcherTimer _timer;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        private OrdersByLocationSettings Settings { get; set; }

        #endregion

        #region Methods

        public OrdersByLocation2()
        {
            InitializeComponent();
           
            this.Settings = new OrdersByLocationSettings()
            {
                CountByType = OrdersByLocationSettings.CountBy.Orders,
                PendingInspectionDepartment = ApplicationSettings.Current.DepartmentQA,
                PartMarkingDepartment = ApplicationSettings.Current.DepartmentPartMarking 
            };

            this.DisplayName = "Orders By Department";

            settingsPanel.Visibility = System.Windows.Visibility.Collapsed;
            settingsPanel.DataContext = this.Settings;
            cboType.ItemsSource = Enum.GetValues(typeof(OrdersByLocationSettings.CountBy)).Cast<OrdersByLocationSettings.CountBy>();
        }
        
        public void Start()
        {
            try
            {
                this.DisplayName = this.Settings.CountByType == OrdersByLocationSettings.CountBy.Orders ? "Orders By Department" : "Parts By Department";

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

            var qaDept = String.IsNullOrWhiteSpace(this.Settings.PendingInspectionDepartment) ? null : this.Settings.PendingInspectionDepartment;
            var partMarkingDept = String.IsNullOrWhiteSpace(this.Settings.PartMarkingDepartment) ? null : this.Settings.PartMarkingDepartment;

            using (var ta = new Data.Reports.DashboardTableAdapters.OrderSummaryTableAdapter())
            {
                var lateOrders = ta.GetData();

                foreach (var lateOrder in lateOrders)
                {
                    string department;

                    if (qaDept != null && (lateOrder.WorkStatus == "Pending Inspection" || lateOrder.WorkStatus == "Final Inspection" || lateOrder.WorkStatus == "Pending Join" || lateOrder.WorkStatus == "Pending Rework Planning"))
                        department = qaDept;
                    else if (partMarkingDept != null && lateOrder.WorkStatus == "Part Marking")
                        department = partMarkingDept;
                    else
                        department = lateOrder.CurrentLocation; //In Process, Changing Departments, Pending Order Review

                    var row = lateOrdersByDept.Rows.Find(department);

                    if (this.Settings.CountByType == OrdersByLocationSettings.CountBy.Orders)
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
            set { this.Settings = value as OrdersByLocationSettings; }
        }

        #endregion

        #region OrdersByLocationSettings

        [DataContract]
        public class OrdersByLocationSettings : WidgetSettings
        {
            private CountBy _countByType;
            private string _pendingInspectionDepartment;
            private string _partMarkingDepartment;

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

            [DataMember]
            public string PendingInspectionDepartment
            {
                get { return this._pendingInspectionDepartment; }
                set
                {
                    this._pendingInspectionDepartment = value;
                    OnPropertyChanged("PendingInspectionDepartment");
                }
            }

            [DataMember]
            public string PartMarkingDepartment
            {
                get { return this._partMarkingDepartment; }
                set
                {
                    this._partMarkingDepartment = value;
                    OnPropertyChanged("PartMarkingDepartment");
                }
            }
        }

        #endregion
    }
}
