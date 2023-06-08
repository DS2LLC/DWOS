using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using DWOS.Data;
using DWOS.Data.Datasets;
using System.Windows.Input;
using DWOS.UI.Utilities;
using DWOS.Shared;
using GalaSoft.MvvmLight.CommandWpf;

namespace DWOS.UI.Admin.Schedule.Manual
{
    public class SchedulingTabDataContext : INotifyPropertyChanged
    {
        #region Fields

        public event EventHandler<DepartmentAddedEventArgs> DepartmentAdded;
        public event EventHandler DataSaved;
        private bool _hasChanges;
        private readonly IDwosApplicationSettingsProvider _settingsProvider;
        private readonly ISchedulingPersistence _schedulingPersistence;

        /// <summary>
        /// Previously retrieved data from the database; used to refresh data
        /// after canceling changes.
        /// </summary>
        private IDictionary<string, List<ScheduleData>> _currentDatabaseData;

        #endregion

        #region Properties

        public ObservableCollection<DepartmentDataContext> Departments { get; } =
            new ObservableCollection<DepartmentDataContext>();

        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged(nameof(HasChanges));
                    OnPropertyChanged(nameof(RefreshWarningVisibility));
                }
            }
        }

        public Visibility RefreshWarningVisibility => _hasChanges ? Visibility.Visible : Visibility.Collapsed;

        public bool IsEnabled => _settingsProvider.Settings.UsingManualScheduling
            && _schedulingPersistence.SecurityManager.IsInRole("OrderSchedule");

        public ICommand Save { get; }

        public ICommand Cancel { get; }

        #endregion

        #region Methods

        public SchedulingTabDataContext(ISchedulingPersistence schedulingPersistence, IDwosApplicationSettingsProvider settingsProvider)
        {
            if (schedulingPersistence == null)
            {
                throw new ArgumentNullException(nameof(schedulingPersistence));
            }

            Save = new RelayCommand(DoSave, () => HasChanges);

            Cancel = new RelayCommand(DoRefresh, () => HasChanges);

            _schedulingPersistence = schedulingPersistence;
            _settingsProvider = settingsProvider;
        }

        private void DoSave()
        {
            try
            {
                _schedulingPersistence.SaveChanges(this);

                HasChanges = false;
                DataSaved?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error saving schedule data.", exc);
            }
        }

        public void Initialize(WipData wipData)
        {
            if (wipData == null)
            {
                throw new ArgumentNullException(nameof(wipData));
            }

            var _schedulerType = _settingsProvider.Settings.SchedulerType;

            Departments.Clear();

            if (_schedulerType == SchedulerType.Manual)
            {
                _currentDatabaseData = GetOrdersByDepartment(wipData.OrderDataSet);
            }
            else
            {
                var allOrders = GetAllOrders(wipData.OrderDataSet);

                _currentDatabaseData = new Dictionary<string, List<ScheduleData>>
                {
                    { "All", allOrders }
                };
            }

            if (_settingsProvider.Settings.UsingManualScheduling)
            {
                foreach (var deptOrderPair in _currentDatabaseData)
                {
                    var deptContext = new DepartmentDataContext(deptOrderPair.Key);
                    deptContext.Load(deptOrderPair.Value);
                    Departments.Add(deptContext);
                }
            }
        }

        public void RefreshSchedulerType()
        {
            var schedulerType = _settingsProvider.Settings.SchedulerType;

            Departments.Clear();
            HasChanges = false; // Changing scheduler type resets all in-progress changes.
            var settings = _settingsProvider.Settings;

            if (schedulerType == SchedulerType.Manual)
            {
                var itemsByDepartment = new Dictionary<string, List<ScheduleData>>();

                foreach (var scheduleItem in _currentDatabaseData.Values.SelectMany(dept => dept))
                {
                    var dept = string.Empty;
                    if (scheduleItem.WorkStatus == settings.WorkStatusChangingDepartment)
                    {
                        dept = scheduleItem.NextDept;
                    }
                    else if (scheduleItem.WorkStatus == settings.WorkStatusInProcess)
                    {
                        dept = scheduleItem.CurrentLocation;
                    }

                    if (string.IsNullOrEmpty(dept))
                    {
                        continue;
                    }

                    if (itemsByDepartment.ContainsKey(dept))
                    {
                        itemsByDepartment[dept].Add(scheduleItem);
                    }
                    else
                    {
                        itemsByDepartment[dept] = new List<ScheduleData> { scheduleItem };
                    }
                }

                _currentDatabaseData = itemsByDepartment;
            }
            else
            {
                var allOrders = _currentDatabaseData.Values.SelectMany(dept => dept).ToList();

                _currentDatabaseData = new Dictionary<string, List<ScheduleData>>
                {
                    { "All", allOrders }
                };
            }

            if (settings.UsingManualScheduling)
            {
                foreach (var deptOrderPair in _currentDatabaseData)
                {
                    var deptContext = new DepartmentDataContext(deptOrderPair.Key);
                    deptContext.Load(deptOrderPair.Value);
                    deptContext.ScheduleOrderChanged += DeptContextOnScheduleOrderChanged;
                    Departments.Add(deptContext);
                }
            }
        }

        private void DoRefresh()
        {
            try
            {
                if (_settingsProvider.Settings.UsingManualScheduling)
                {
                    foreach (var deptOrderPair in _currentDatabaseData)
                    {
                        var matchingDept = Departments.FirstOrDefault(d => d.Name == deptOrderPair.Key);

                        if (matchingDept == null)
                        {
                            matchingDept = new DepartmentDataContext(deptOrderPair.Key);
                            Departments.Add(matchingDept);
                            DepartmentAdded?.Invoke(this, new DepartmentAddedEventArgs(matchingDept));
                            matchingDept.ScheduleOrderChanged += DeptContextOnScheduleOrderChanged;
                        }

                        matchingDept.Load(deptOrderPair.Value);
                    }

                    foreach (var dept in Departments)
                    {
                        if (!_currentDatabaseData.Keys.Contains(dept.Name))
                        {
                            dept.Load(Enumerable.Empty<ScheduleData>());
                        }
                    }
                }

                HasChanges = false;
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error refreshing schedule data.", exc);
            }
        }

        private void DoFieldRefresh()
        {
            foreach (var dept in Departments)
            {
                dept.RefreshFields();
            }
        }

        private IDictionary<string, List<ScheduleData>> GetOrdersByDepartment(OrderStatusDataSet orders)
        {
            var settings = _settingsProvider.Settings;

            var itemsByDepartment = new Dictionary<string, List<ScheduleData>>();

            foreach (var orderRow in orders.OrderStatus.Where(o => o.IsValidState() && !o.InBatch))
            {
                var dept = string.Empty;
                if (orderRow.WorkStatus == settings.WorkStatusChangingDepartment)
                {
                    dept = orderRow.IsNextDeptNull() ? string.Empty : orderRow.NextDept;
                }
                else if (orderRow.WorkStatus == settings.WorkStatusInProcess)
                {
                    dept = orderRow.CurrentLocation;
                }

                if (string.IsNullOrEmpty(dept))
                {
                    continue;
                }

                var order = ScheduleData.From(orderRow);

                if (itemsByDepartment.ContainsKey(dept))
                {
                    itemsByDepartment[dept].Add(order);
                }
                else
                {
                    itemsByDepartment[dept] = new List<ScheduleData> { order };
                }
            }

            foreach (var batchRow in orders.BatchStatus.Where(o => o.IsValidState()))
            {
                var dept = string.Empty;

                if (batchRow.WorkStatus == settings.WorkStatusChangingDepartment)
                {
                    dept = batchRow.IsNextDeptNull() ? string.Empty : batchRow.NextDept;
                }
                else if (batchRow.WorkStatus == settings.WorkStatusInProcess)
                {
                    dept = batchRow.CurrentLocation;
                }

                if (string.IsNullOrEmpty(dept))
                {
                    continue;
                }

                var batch = ScheduleData.From(batchRow);
                if (itemsByDepartment.ContainsKey(dept))
                {
                    itemsByDepartment[dept].Add(batch);
                }
                else
                {
                    itemsByDepartment[dept] = new List<ScheduleData> { batch };
                }
            }

            return itemsByDepartment;
        }

        private List<ScheduleData> GetAllOrders(OrderStatusDataSet orders)
        {
            var settings = _settingsProvider.Settings;

            var items = new List<ScheduleData>();
            foreach (var orderRow in orders.OrderStatus.Where(o => o.IsValidState() && !o.InBatch))
            {
                var dept = string.Empty;
                if (orderRow.WorkStatus == settings.WorkStatusChangingDepartment || orderRow.WorkStatus == settings.WorkStatusInProcess)
                {
                    dept = orderRow.IsNextDeptNull() ? string.Empty : orderRow.NextDept;
                }
                else if (orderRow.WorkStatus == settings.WorkStatusInProcess)
                {
                    dept = orderRow.CurrentLocation;
                }

                if (string.IsNullOrEmpty(dept))
                {
                    continue;
                }

                items.Add(ScheduleData.From(orderRow));
            }

            foreach (var batchRow in orders.BatchStatus.Where(o => o.IsValidState()))
            {
                var dept = string.Empty;

                if (batchRow.WorkStatus == settings.WorkStatusChangingDepartment)
                {
                    dept = batchRow.IsNextDeptNull() ? string.Empty : batchRow.NextDept;
                }
                else if (batchRow.WorkStatus == settings.WorkStatusInProcess)
                {
                    dept = batchRow.CurrentLocation;
                }

                if (string.IsNullOrEmpty(dept))
                {
                    continue;
                }

                items.Add(ScheduleData.From(batchRow));
            }

            return items;
        }

        public void UpdateSettings()
        {
            OnPropertyChanged(nameof(IsEnabled)); // Could have changed configuration settings

            DoFieldRefresh();
        }

        public void UpdateData(WipData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!HasChanges)
            {
                var schedulerType = _settingsProvider.Settings.SchedulerType;
                if (schedulerType == SchedulerType.Manual)
                {
                    _currentDatabaseData = GetOrdersByDepartment(data.OrderDataSet);
                }
                else
                {
                    _currentDatabaseData = new Dictionary<string, List<ScheduleData>>
                    {
                        { "All", GetAllOrders(data.OrderDataSet) }
                    };
                }

                DoRefresh();
            }
        }

        public void OnLoaded()
        {
            foreach (var dept in Departments)
            {
                dept.ScheduleOrderChanged += DeptContextOnScheduleOrderChanged;
            }

            OnPropertyChanged(nameof(IsEnabled)); // Could have logged-in before load
            _schedulingPersistence.SecurityManager.UserUpdated += SecurityManagerOnUserUpdated;
        }

        public void OnUnloaded()
        {
            foreach (var dept in Departments)
            {
                dept.ScheduleOrderChanged -= DeptContextOnScheduleOrderChanged;
            }

            _schedulingPersistence.SecurityManager.UserUpdated -= SecurityManagerOnUserUpdated;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Events

        private void DeptContextOnScheduleOrderChanged(object sender, EventArgs eventArgs)
        {
            HasChanges = true;
        }

        private void SecurityManagerOnUserUpdated(object sender, UserUpdatedEventArgs e)
        {
            OnPropertyChanged(nameof(IsEnabled));
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
