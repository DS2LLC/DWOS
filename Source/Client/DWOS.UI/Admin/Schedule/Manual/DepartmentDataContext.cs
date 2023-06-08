using System;
using System.Collections.Generic;
using System.ComponentModel;
using DWOS.Data.Datasets;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DWOS.UI.Utilities;
using NLog;
using GalaSoft.MvvmLight.CommandWpf;

namespace DWOS.UI.Admin.Schedule.Manual
{
    public class DepartmentDataContext : INotifyPropertyChanged
    {
        public event EventHandler ScheduleOrderChanged;
        public event EventHandler FieldsRefreshed;
        public event EventHandler DataRefreshed;

        private object[] _unscheduledSelection;
        private object[] _scheduledSelection;

        public string Name
        {
            get;
        }

        public object[] UnscheduledSelection
        {
            get { return _unscheduledSelection; }
            set
            {
                if (_unscheduledSelection != value)
                {
                    _unscheduledSelection = value;
                    OnPropertyChanged(nameof(UnscheduledSelection));
                }
            }
        }

        public object[] ScheduledSelection
        {
            get { return _scheduledSelection; }
            set
            {
                if (_scheduledSelection != value)
                {
                    _scheduledSelection = value;
                    OnPropertyChanged(nameof(ScheduledSelection));
                }
            }
        }

        public ObservableCollection<ScheduleData> Unscheduled { get; } = new ObservableCollection<ScheduleData>();

        public ObservableCollection<ScheduleData> Scheduled { get; } = new ObservableCollection<ScheduleData>();

        public ICommand AddSelectedToSchedule { get; }

        public ICommand RemoveSelectedFromSchedule { get; }

        public ICommand MoveSelectedUp { get; }

        public ICommand MoveSelectedDown { get; }

        public DepartmentDataContext(string name)
        {
            Name = name;
            AddSelectedToSchedule = new RelayCommand(
                () =>
                {
                    var selection = _unscheduledSelection;

                    if (selection == null)
                    {
                        return;
                    }

                    AddToSchedule(selection.OfType<ScheduleData>().ToList());
                },
                () => _unscheduledSelection != null && _unscheduledSelection.Length > 0);

            RemoveSelectedFromSchedule = new RelayCommand(
                () =>
                {
                    var selection = _scheduledSelection;

                    if (selection == null)
                    {
                        return;
                    }

                    var scheduledItems = selection.OfType<ScheduleData>().ToList();
                    RemoveFromSchedule(scheduledItems);
                },
                () => _scheduledSelection != null && _scheduledSelection.Length > 0);

            MoveSelectedUp = new RelayCommand(
                DoMoveUp,
                () => _scheduledSelection != null && _scheduledSelection.Length > 0);

            MoveSelectedDown  = new RelayCommand(
                DoMoveDown,
                () => _scheduledSelection != null && _scheduledSelection.Length > 0);
        }

        public void AddToSchedule(ICollection<ScheduleData> selection)
        {
            if (selection == null)
            {
                return;
            }

            foreach (var item in selection)
            {
                Unscheduled.Remove(item);
            }

            UnscheduledSelection = null;

            var nextSchedulePriority = Scheduled.Any() ?
                Scheduled.Max(o => o.SchedulePriority) + 1 :
                1;

            foreach (var item in selection)
            {
                item.SchedulePriority = nextSchedulePriority;
                Scheduled.Add(item);
            }

            OnScheduleOrderChanged();
        }

        public void RemoveFromSchedule(ICollection<ScheduleData> selection)
        {
            if (selection == null)
            {
                return;
            }

            foreach (var item in selection)
            {
                Scheduled.Remove(item);
            }

            ScheduledSelection = null;

            foreach (var item in selection)
            {
                item.SchedulePriority = 0;
                Unscheduled.Add(item);
            }

            OnScheduleOrderChanged();
        }

        public void MoveToTop(ICollection<ScheduleData> selection)
        {
            if (selection == null)
            {
                return;
            }

            var nonSelectedItems = Scheduled
                .Where(s => !selection.Contains(s))
                .OrderBy(s => s.SchedulePriority)
                .ToList();

            var schedulePriority = 1;

            foreach (var item in selection.Concat(nonSelectedItems))
            {
                item.SchedulePriority = schedulePriority;
                schedulePriority++;
            }

            OnScheduleOrderChanged();
        }

        public void MoveBelow(ICollection<ScheduleData> selection, ScheduleData targetItem)
        {
            if (selection == null || targetItem == null)
            {
                return;
            }

            var schedulePriority = targetItem.SchedulePriority + 1;

            var nonSelectedItemsBelow = Scheduled
                .Where(s => !selection.Contains(s) && s.SchedulePriority >= schedulePriority)
                .OrderBy(s => s.SchedulePriority)
                .ToList();

            foreach (var item in selection.Concat(nonSelectedItemsBelow))
            {
                item.SchedulePriority = schedulePriority;
                schedulePriority++;
            }

            OnScheduleOrderChanged();
        }

        private void DoMoveUp()
        {
            try
            {
                var selection = _scheduledSelection
                    ?.OfType<ScheduleData>()
                    .OrderBy(o => o.SchedulePriority)
                    .ToList();

                var selectionStart = selection?.FirstOrDefault();

                if (selectionStart == null)
                {
                    return;
                }

                var previousOrder = Scheduled.OrderBy(o => o.SchedulePriority)
                    .LastOrDefault(o => o.SchedulePriority < selectionStart.SchedulePriority);

                if (previousOrder != null)
                {
                    var startingPriority = previousOrder.SchedulePriority;
                    var lastPriority = selection.Last().SchedulePriority;

                    for (var i = 0; i < selection.Count; ++i)
                    {
                        selection[i].SchedulePriority = startingPriority + i;
                    }

                    previousOrder.SchedulePriority = lastPriority;
                }

                OnScheduleOrderChanged();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error moving orders up in schedule.");
            }
        }

        private void DoMoveDown()
        {
            try
            {
                var selection = _scheduledSelection
                    ?.OfType<ScheduleData>()
                    .OrderBy(o => o.SchedulePriority)
                    .ToList();

                var selectionStart = selection?.FirstOrDefault();
                var selectionEnd = selection?.LastOrDefault();

                if (selectionStart == null || selectionEnd == null)
                {
                    return;
                }

                var nextOrder = Scheduled.OrderBy(o => o.SchedulePriority)
                    .FirstOrDefault(o => o.SchedulePriority > selectionEnd.SchedulePriority);

                if (nextOrder != null)
                {
                    var firstPriority = selectionStart.SchedulePriority;
                    var startingPriority = nextOrder.SchedulePriority;

                    for (var i = 0; i < selection.Count; ++i)
                    {
                        selection[i].SchedulePriority = startingPriority + i;
                    }

                    nextOrder.SchedulePriority = firstPriority;
                }

                OnScheduleOrderChanged();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error moving orders down in schedule.");
            }
        }

        private void EnsureScheduleOrder()
        {
            var expectedSchedulePriority = 1;
            foreach (var scheduledOrder in Scheduled.OrderBy(o => o.SchedulePriority))
            {
                scheduledOrder.SchedulePriority = expectedSchedulePriority;
                ++expectedSchedulePriority;
            }
        }

        public void RefreshFields()
        {
            FieldsRefreshed?.Invoke(this, EventArgs.Empty);
        }

        public void Load(IEnumerable<UI.ScheduleData> departmentOrders)
        {
            if (departmentOrders == null)
            {
                throw new ArgumentNullException(nameof(departmentOrders));
            }

            Unscheduled.Clear();
            Scheduled.Clear();

            foreach (var data in departmentOrders.Select(s => new ScheduleData(s)).OrderBy(s => s.SchedulePriority))
            {
                if (data.SchedulePriority == 0)
                {
                    Unscheduled.Add(data);
                }
                else
                {
                    Scheduled.Add(data);
                }
            }

            if (_unscheduledSelection != null)
            {
                var unscheduledOrderIds = _unscheduledSelection.OfType<ScheduleData>().Select(o => o.Id);
                UnscheduledSelection = Unscheduled.Where(o => unscheduledOrderIds.Contains(o.Id)).Cast<object>().ToArray();

            }

            if (_scheduledSelection != null)
            {
                var scheduledOrderIds = _scheduledSelection.OfType<ScheduleData>().Select(o => o.Id);
                ScheduledSelection = Scheduled.Where(o => scheduledOrderIds.Contains(o.Id)).Cast<object>().ToArray();
            }

            DataRefreshed?.Invoke(this, EventArgs.Empty);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnScheduleOrderChanged()
        {
            EnsureScheduleOrder();
            ScheduleOrderChanged?.Invoke(this, EventArgs.Empty);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region OrderData

        public class ScheduleData : INotifyPropertyChanged
        {
            #region Fields

            private bool _updated;
            private int _schedulePriority;

            #endregion

            #region Properties

            public int SchedulePriority
            {
                get {  return _schedulePriority; }
                set
                {
                    if (_schedulePriority != value)
                    {
                        _schedulePriority = value;
                        OnPropertyChanged(nameof(SchedulePriority));
                        Updated = true;
                    }
                }
            }

            public int OriginalSchedulePriority { get; }

            public bool Updated
            {
                get {  return _updated; }
                set
                {
                    if (_updated != value)
                    {
                        _updated = value;
                        OnPropertyChanged(nameof(Updated));
                    }
                }
            }

            public int Id { get; }

            public ScheduleDataType Type { get; }

            public string Customer { get; }

            public string Priority { get; }

            public string Part { get; }

            public int Quantity { get; }

            public double TotalSurfaceArea { get; }

            public DateTime? RequiredDate { get; }

            public string CurrentLocation { get; }

            public string NextDept { get; }

            public string CurrentLine { get; }

            public string Process { get; }

            #endregion

            #region Methods

            public ScheduleData(UI.ScheduleData order)
            {
                if (order == null)
                {
                    throw new ArgumentNullException(nameof(order));
                }

                _schedulePriority = order.SchedulePriority;
                OriginalSchedulePriority = _schedulePriority;
                Id = order.Id;
                Type = order.Type == UI.ScheduleData.ScheduleDataType.Batch ? ScheduleDataType.Batch : ScheduleDataType.Order;
                Customer = order.Customer ?? string.Empty;
                Priority = order.Priority ?? string.Empty;
                Part = order.Part ?? string.Empty;
                Quantity = order.Quantity ?? 0;
                TotalSurfaceArea = order.SurfaceArea ?? 0d;
                RequiredDate = order.RequiredDate;
                CurrentLine = order.CurrentLine;
                CurrentLocation = order.CurrentLocation;
                NextDept = order.NextDept;
                Process = order.Process;
               // foreach (ProcessActionsConverter in OrderProcessingDataSet.o)


            }

            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        #region ScheduleDataType

        public enum ScheduleDataType
        {
            Order,
            Batch
        }

        #endregion
    }
}
