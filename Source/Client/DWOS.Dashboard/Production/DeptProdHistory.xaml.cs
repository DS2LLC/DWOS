using MathNet.Numerics.Statistics;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace DWOS.Dashboard.Production
{
    /// <summary>
    /// Interaction logic for DeptProdHistory.xaml
    /// </summary>
    public partial class DeptProdHistory : UserControl, IDashboardWidget2
    {
        #region Fields

        private const string TABLE_COLUMN_DAY = "Day";
        private const string TABLE_COLUMN_COUNT = "Count";

        private string _displayName;
        public event PropertyChangedEventHandler PropertyChanged;
        private BackgroundWorker _worker = null;
        private DispatcherTimer _timer;
        private DeptProdHistoryContext _viewModel;

        #endregion

        #region Methods

        public DeptProdHistory()
        {
            InitializeComponent();

            this._viewModel = new DeptProdHistoryContext();
            this.DisplayName = "Production Details";
            this.DataContext = this._viewModel;
            settingsPanel.Visibility = Visibility.Collapsed;
        }

        public void Start()
        {
            try
            {
                if (string.IsNullOrEmpty(this._viewModel.Settings.Department))
                {
                    // Ignore - Department gets set to null after
                    // Departments update.
                    return;
                }

                this.DisplayName = this._viewModel.Settings.Department + " Production";

                goalLine.Value = _viewModel.Settings.GoalCount.GetValueOrDefault(0);
                goalLine.Visibility = _viewModel.Settings.GoalCount.GetValueOrDefault(0) > 0 ?
                    Visibility.Visible :
                    Visibility.Collapsed;

                if(_timer == null)
                {
                    _timer = new DispatcherTimer(TimeSpan.FromSeconds(30), DispatcherPriority.Normal, (s, e) => Start(), Application.Current.Dispatcher) { IsEnabled = false };
                    _viewModel.Settings.PropertyChanged += Settings_PropertyChanged;
                }
                else
                {
                    _timer.IsEnabled = false;
                }

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

                                                      var data = e.Result as ProductionHistoryData;
                                                      if (data != null)
                                                      {
                                                          var view = data.DataTable.DefaultView;
                                                          view.Sort = TABLE_COLUMN_DAY;

                                                          stripAverage.StartY = data.StartY;
                                                          stripAverage.EndY = data.EndY;

                                                          _viewModel.ChartView = view;
                                                      }

                                                      _timer.IsEnabled = true;
                                                  };
                }

                if (!_worker.IsBusy)
                {
                    _worker.RunWorkerAsync();
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading current summary data.");
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
            this._viewModel.RefreshDepartments();
        }

        private ProductionHistoryData GetData()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add(TABLE_COLUMN_DAY, typeof(DateTime));
            dataTable.Columns.Add(TABLE_COLUMN_COUNT, typeof(int));

            var samplePoints = new List <double>();

            using (var ta = new Data.Reports.DashboardTableAdapters.ProcessedPartQtyTableAdapter())
            {
                var processData = ta.GetDataBy(this._viewModel.Settings.HistoryDays);
                var rows = processData
                    .Select("Department = '{0}'".FormatWith(this._viewModel.Settings.Department))
                    .OfType<Data.Reports.Dashboard.ProcessedPartQtyRow>();

                foreach (var processRow in rows)
                {
                    var date = Convert.ToDateTime(processRow.Day);
                    dataTable.Rows.Add(date, processRow.ProcessedPartQty);
                    samplePoints.Add(Convert.ToDouble(processRow.ProcessedPartQty));
                }
            }

            int calcMin = 0;
            int calcMax = 0;

            if (samplePoints.Count > 1)
            {
                //don't trim the sampling if it is too small to trim
                if (samplePoints.Count > 10)
                {
                    var sd = samplePoints.StandardDeviation();
                    var mean = samplePoints.Mean();

                    //get sampling of datasets only within first standard deviation
                    var sd2Sample = samplePoints
                        .Where(d => d >= mean - sd && d <= mean + sd)
                        .ToList();

                    //if we didn't trim too much then use this
                    if (sd2Sample.Count > 4)
                        samplePoints = sd2Sample;
                }

                calcMin = Convert.ToInt32(samplePoints.Minimum());
                calcMax = Convert.ToInt32(samplePoints.Maximum());
            }

            return new ProductionHistoryData()
            {
                DataTable = dataTable,
                StartY = calcMax,
                EndY = calcMin
            };
        }

        #endregion

        #region Events

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            bool enableSettingsPanel = settingsPanel.Visibility == Visibility.Collapsed;

            settingsPanel.Visibility = enableSettingsPanel ? Visibility.Visible : Visibility.Collapsed;
            chart.Visibility = !enableSettingsPanel ? Visibility.Visible : Visibility.Collapsed;
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

        public Control Control
        {
            get { return this; }
        }

        public WidgetSettings Settings
        {
            get { return this._viewModel.Settings; }
            set { this._viewModel.Settings = value as DeptProdHistorySettings; }
        }

        #endregion

        #region ProductionByDepartmentSettings

        [DataContract]
        public class DeptProdHistorySettings : WidgetSettings
        {
            private int _historyDays;
            private string _department;
            private int? _goalCount;

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
            public string Department
            {
                get { return this._department; }
                set
                {
                    this._department = value;
                    OnPropertyChanged("Department");
                }
            }

            [DataMember]
            public int? GoalCount
            {
                get { return this._goalCount; }
                set
                {
                    this._goalCount = value;
                    OnPropertyChanged("GoalCount");
                }
            }

            public DeptProdHistorySettings()
            {
                this.HistoryDays = 7;
            }
        }

        #endregion

        #region ProductionHistoryData

        private sealed class ProductionHistoryData
        {
            public DataTable DataTable { get; set; }

            public int StartY { get; set; }

            public int EndY { get; set; }
        }

        #endregion

        #region DeptProdHistoryContext

        private sealed class DeptProdHistoryContext : INotifyPropertyChanged
        {
            #region Fields

            private DeptProdHistorySettings _settings;
            private DataView _chartView;

            public DeptProdHistorySettings Settings
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

            public ObservableCollection<string> Departments { get; } =
                new ObservableCollection<string>();

            public DataView ChartView
            {
                get
                {
                    return _chartView;
                }

                set
                {
                    if (_chartView != value)
                    {
                        _chartView = value;
                        OnPropertyChanged("ChartView");
                    }
                }
            }

            #endregion

            #region Methods

            public DeptProdHistoryContext()
            {
                _settings = new DeptProdHistorySettings() { Department = "Paint", HistoryDays = 14 };
                using (var taDept = new Data.Reports.DashboardTableAdapters.d_DepartmentTableAdapter())
                {
                    var departmentList = taDept.GetDataTable().GetDistinctColumnValues("DepartmentID");
                    foreach (var departmentID in departmentList)
                    {
                        Departments.Add(departmentID);
                    }
                }
            }

            public void RefreshDepartments()
            {
                string currentDepartmentID = _settings.Department;
                int selectedIndex = Math.Max(Departments.IndexOf(_settings.Department), 0);

                Departments.Clear();
                using (var taDept = new Data.Reports.DashboardTableAdapters.d_DepartmentTableAdapter())
                {
                    var departmentList = taDept.GetDataTable().GetDistinctColumnValues("DepartmentID");
                    foreach (var departmentID in departmentList)
                    {
                        Departments.Add(departmentID);
                    }
                }

                if (Departments.Contains(currentDepartmentID))
                {
                    // Clearing Departments sets _settings.Department to null
                    _settings.Department = currentDepartmentID;
                }
                else if (Departments.Count > selectedIndex)
                {
                    _settings.Department = Departments[selectedIndex];
                }
                else
                {
                    _settings.Department = Departments.FirstOrDefault();
                }

            }

            private void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;

                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
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
