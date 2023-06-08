using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.Shared;
using DWOS.UI.Utilities;
using Infragistics.Windows.Ribbon;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace DWOS.UI.Admin.Time
{
    /// <summary>
    /// Interaction logic for TimeManager.xaml
    /// </summary>
    /// <remarks>
    /// This is the primary Time Manager dialog.
    /// </remarks>
    public partial class TimeManager : XamRibbonWindow, ITimeManagerView
    {
        #region Fields

        private static readonly TimeSpan TIMER_INTERVAL = TimeSpan.FromSeconds(15);
        private readonly object _timerStopLock = new object();
        private DispatcherTimer _timer;
        private readonly GridSettingsPersistence<XamDataGridSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<XamDataGridSettings>("TimeManager", new XamDataGridSettings());

        #endregion

        #region Properties

        private TimeManagerContext ViewModel
        {
            get
            {
                return DataContext as TimeManagerContext;
            }
        }

        #endregion

        #region Methods

        public TimeManager()
        {
            InitializeComponent();
            btnStart.LargeImage = Properties.Resources.TimeStart32.ToWpfImage();
            btnPause.LargeImage = Properties.Resources.TimePause32.ToWpfImage();
            btnStop.LargeImage = Properties.Resources.TimeStop32.ToWpfImage();
            btnMove.LargeImage = Properties.Resources.TimeMove32.ToWpfImage();
            btnProcessing.LargeImage = Properties.Resources.Process_32.ToWpfImage();
            Icon = Properties.Resources.Clock_32.ToWpfImage();
        }

        public void LoadData(int userId)
        {
            DataContext = TimeManagerContext.ForUser(this, userId);
        }

        public void LoadAdminData()
        {
            DataContext = TimeManagerContext.ForManager(this);
        }

        #endregion

        #region Events

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void XamRibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _timer = new DispatcherTimer()
            {
                Interval = TIMER_INTERVAL
            };

            _timer.Tick += Timer_Tick;

            _timer.Start();

            // Load settings
            _gridSettingsPersistence.LoadSettings().ApplyTo(TimeGrid);
        }

        private void XamRibbonWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            lock (_timerStopLock)
            {
                _timer?.Stop();
            }

            // Save settings
            var settings = new XamDataGridSettings();
            settings.RetrieveSettingsFrom(TimeGrid);
            _gridSettingsPersistence.SaveSettings(settings);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            lock (_timerStopLock)
            {
                if (!_timer.IsEnabled)
                {
                    return;
                }

                _timer.Stop();
                ViewModel?.Refresh();
                _timer.Start();
            }
        }

        #endregion

        #region ITimeManagerView Members

        public void MoveOrdersDialog(IEnumerable<IOperatorEntry> entriesToMove)
        {
            try
            {
                var dialog = new MoveTimersDialog();
                dialog.LoadData(entriesToMove);
                dialog.ShowDialog();
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error showing 'move orders' dialog.", exc, true);
            }
        }

        public void ManageProcessingTimeDialog()
        {
            try
            {
                var dialog = new ProcessingTimeManager();
                dialog.LoadData();
                dialog.ShowDialog();
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error showing 'manage processing timers' dialog.", exc, true);
            }
        }

        #endregion

        #region TimeManagerContext

        /// <summary>
        /// Context for <see cref="TimeManager"/>.
        /// </summary>
        private sealed class TimeManagerContext : INotifyPropertyChanged
        {
            #region Fields

            private object[] _selectedItems;
            private readonly object _uiUpdateLock = new object();

            #endregion

            #region Properties

            /// <summary>
            /// Gets the User ID of the context.
            /// </summary>
            /// <remarks>
            /// A null value indicates an administrator.
            /// </remarks>
            public int? UserId
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the view of the context.
            /// </summary>
            public ITimeManagerView View
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the operator entries of the context.
            /// </summary>
            public IEnumerable<IOperatorEntry> OperatorEntries
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets or sets selected items.
            /// </summary>
            public object[] SelectedItems
            {
                get
                {
                    return _selectedItems;
                }
                set
                {
                    if (_selectedItems != value)
                    {
                        _selectedItems = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItems)));
                    }
                }
            }

            /// <summary>
            /// Gets a collection of selected, paused items.
            /// </summary>
            public IEnumerable<IOperatorEntry> SelectedPaused
            {
                get
                {
                    if (SelectedItems == null)
                    {
                        return null;
                    }

                    return SelectedItems
                        .OfType<IOperatorEntry>()
                        .Where(i => !i.HasActiveTimer);
                }
            }

            /// <summary>
            /// Gets a collection of selected, active items.
            /// </summary>
            public IEnumerable<IOperatorEntry> SelectedActive
            {
                get
                {
                    if (SelectedItems == null)
                    {
                        return null;
                    }

                    return SelectedItems
                        .OfType<IOperatorEntry>()
                        .Where(i => i.HasActiveTimer);
                }
            }

            /// <summary>
            /// Gets the 'Start Timer' command instance.
            /// </summary>
            public ICommand StartTimer
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the 'Pause Timer' command instance.
            /// </summary>
            public ICommand PauseTimer
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the 'Stop Timer' command instance.
            /// </summary>
            public ICommand StopTimer
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the 'Move Timer' command instance.
            /// </summary>
            public ICommand MoveTimer
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the 'Manage Processing' command instance.
            /// </summary>
            public ICommand ManageProcessing
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the <see cref="TimeManagerContext"/> class.
            /// </summary>
            /// <remarks>
            /// Use <see cref="ForUser(ITimeManagerView, int)"/> or <see cref="ForManager(ITimeManagerView)"/>
            /// instead.
            /// </remarks>
            /// <param name="view"></param>
            /// <param name="userId"></param>
            private TimeManagerContext(ITimeManagerView view, int? userId)
            {
                View = view;
                UserId = userId;
                StartTimer = new StartTimerCommand(this);
                PauseTimer = new PauseTimerCommand(this);
                StopTimer = new StopTimerCommand(this);
                MoveTimer = new MoveTimerCommand(this);
                ManageProcessing = new ManageProcessingCommand(this);
            }

            /// <summary>
            /// Refreshes data for this context.
            /// </summary>
            public void Refresh()
            {
                lock (_uiUpdateLock)
                {
                    var previousSelection = SelectedItems?.OfType<IOperatorEntry>()
                        ?? Enumerable.Empty<IOperatorEntry>();

                    if (UserId.HasValue)
                    {
                        OperatorEntries = GetOperatorEntries(UserId.Value);
                    }
                    else
                    {
                        OperatorEntries = GetAllOperatorEntries();
                    }

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OperatorEntries)));

                    var newSelection = new List<IOperatorEntry>();
                    foreach (var item in previousSelection)
                    {
                        var matchingEntry = OperatorEntries.FirstOrDefault(i => i.Id == item.Id);

                        if (matchingEntry != null)
                        {
                            newSelection.Add(matchingEntry);
                        }
                    }

                    SelectedItems = newSelection.Count > 0 ?
                        newSelection.ToArray() :
                        null;
                }
            }

            public void ShowMoveDialog(List<IOperatorEntry> selection)
            {
                View.MoveOrdersDialog(selection);
                Refresh();
            }

            public void ShowProcessingDialog()
            {
                View.ManageProcessingTimeDialog();
                Refresh();
            }

            /// <summary>
            /// Initializes a new context for a specific user.
            /// </summary>
            /// <param name="view">Time Manager view instance.</param>
            /// <param name="userId">ID of the user to show the dialog for.</param>
            /// <returns>
            /// A new <see cref="TimeManagerContext"/> instance configured to
            /// show only a single user's timers.
            /// </returns>
            public static TimeManagerContext ForUser(ITimeManagerView view, int userId)
            {
                return new TimeManagerContext(view, userId)
                {
                    OperatorEntries = GetOperatorEntries(userId)
                };
            }

            /// <summary>
            /// Initializes a new context for a manager.
            /// </summary>
            /// <param name="view">Time Manager view instance.</param>
            /// <returns>
            /// A new <see cref="TimeManagerContext"/> instance configured to
            /// show every user's timers.
            /// </returns>
            public static TimeManagerContext ForManager(ITimeManagerView view)
            {
                return new TimeManagerContext(view, null)
                {
                    OperatorEntries = GetAllOperatorEntries()
                };
            }

            private static IEnumerable<IOperatorEntry> GetOperatorEntries(int userId)
            {
                OrderProcessingDataSet.OrderSummaryDataTable orders = null;
                OrderProcessingDataSet.BatchDataTable batches = null;
                OrderProcessingDataSet.UsersDataTable users = null;
                LaborTimeTableAdapter taLaborTime = null;

                try
                {
                    taLaborTime = new LaborTimeTableAdapter();

                    var operatorEntries = new List<IOperatorEntry>();
                    using (var taOrderSummary = new OrderSummaryTableAdapter())
                    {
                        orders = taOrderSummary.GetByActiveUser(userId);
                    }

                    using (var taBatch = new BatchTableAdapter())
                    {
                        batches = taBatch.GetByActiveUser(userId);
                    }

                    using (var taUsers = new UsersTableAdapter())
                    {
                        users = taUsers.GetActiveOperators();
                    }

                    foreach (var order in orders)
                    {
                        var timerCount = taLaborTime.GetOrderUserActiveTimerCount(order.OrderID, userId) ?? 0;
                        var durationMinutes = taLaborTime.GetOrderUserDuration(order.OrderID, userId) ?? 0;

                        operatorEntries.Add(new OrderOperatorEntry()
                        {
                            OrderId = order.OrderID,
                            HasActiveTimer = timerCount > 0,
                            DurationMinutes = durationMinutes,
                            UserId = userId,
                            UserName = users.FindByUserID(userId)?.Name ?? "N/A"
                        });
                    }

                    foreach (var batch in batches)
                    {
                        var timerCount = taLaborTime.GetBatchUserActiveTimerCount(batch.BatchID, userId) ?? 0;
                        var durationMinutes = taLaborTime.GetBatchUserDuration(batch.BatchID, userId) ?? 0;

                        operatorEntries.Add(new BatchOperatorEntry()
                        {
                            BatchId = batch.BatchID,
                            HasActiveTimer = timerCount > 0,
                            DurationMinutes = durationMinutes,
                            UserId = userId,
                            UserName = users.FindByUserID(userId)?.Name ?? "N/A"
                        });
                    }

                    return operatorEntries;
                }
                finally
                {
                    orders?.Dispose();
                    batches?.Dispose();
                    users?.Dispose();
                    taLaborTime?.Dispose();
                }
            }

            private static IEnumerable<IOperatorEntry> GetAllOperatorEntries()
            {
                OrderProcessingDataSet.OrderSummaryDataTable orders = null;
                OrderProcessingDataSet.BatchDataTable batches = null;
                OrderProcessingDataSet.UsersDataTable users = null;

                // Process operators
                LaborTimeTableAdapter taLaborTime = null;
                OrderProcessesOperatorTableAdapter taProcessOrderOperator = null;
                BatchProcessesOperatorTableAdapter taBatchProcessOperator = null;

                // Out-of-process operators
                OrderOperatorTableAdapter taOrderOperator = null;
                OrderOperatorTimeTableAdapter taOrderOperatorTime = null;
                BatchOperatorTableAdapter taBatchOperator = null;
                BatchOperatorTimeTableAdapter taBatchOperatorTime = null;

                try
                {
                    taLaborTime = new LaborTimeTableAdapter();
                    taProcessOrderOperator = new OrderProcessesOperatorTableAdapter();
                    taBatchProcessOperator = new BatchProcessesOperatorTableAdapter();
                    taOrderOperator = new OrderOperatorTableAdapter();
                    taOrderOperatorTime = new OrderOperatorTimeTableAdapter();
                    taBatchOperator = new BatchOperatorTableAdapter();
                    taBatchOperatorTime = new BatchOperatorTimeTableAdapter();

                    var operatorEntries = new List<IOperatorEntry>();
                    using (var taOrderSummary = new OrderSummaryTableAdapter())
                    {
                        orders = taOrderSummary.GetForActiveOperators();
                    }

                    using (var taBatch = new BatchTableAdapter())
                    {
                        batches = taBatch.GetForActiveOperators();
                    }

                    using (var taUsers = new UsersTableAdapter())
                    {
                        users = taUsers.GetActiveOperators();
                    }

                    foreach (var order in orders)
                    {
                        // In-process
                        var dtProcessOperator = taProcessOrderOperator
                            .GetOperatorsForOrder(nameof(OperatorStatus.Active), order.OrderID);

                        foreach (var processOperator in dtProcessOperator)
                        {
                            var timerCount = taLaborTime.GetOrderUserActiveTimerCount(order.OrderID, processOperator.UserID) ?? 0;
                            var durationMinutes = taLaborTime.GetOrderUserDuration(order.OrderID, processOperator.UserID) ?? 0;

                            operatorEntries.Add(new OrderOperatorEntry()
                            {
                                OrderId = order.OrderID,
                                HasActiveTimer = timerCount > 0,
                                DurationMinutes = durationMinutes,
                                UserId = processOperator.UserID,
                                UserName = users.FindByUserID(processOperator.UserID)?.Name ?? "N/A"
                            });
                        }

                        // Out-of-process
                        var dtOperator = taOrderOperator
                            .GetOperatorsForOrder(nameof(OperatorStatus.Active), order.OrderID);

                        foreach (var orderOperator in dtOperator)
                        {
                            var timerCount = taOrderOperatorTime.GetActiveTimerCount(orderOperator.OrderOperatorID) ?? 0;
                            var durationMinutes = taOrderOperatorTime.GetDuration(orderOperator.OrderOperatorID) ?? 0;

                            operatorEntries.Add(new OrderOperatorEntry()
                            {
                                OrderId = order.OrderID,
                                HasActiveTimer = timerCount > 0,
                                DurationMinutes = durationMinutes,
                                UserId = orderOperator.UserID,
                                UserName = users.FindByUserID(orderOperator.UserID)?.Name ?? "N/A"
                            });
                        }
                    }

                    foreach (var batch in batches)
                    {
                        // In-Process
                        var dtProcessOperator = taBatchProcessOperator
                            .GetOperatorsForBatch(nameof(OperatorStatus.Active), batch.BatchID);

                        foreach (var processOperator in dtProcessOperator)
                        {
                            var timerCount = taLaborTime.GetBatchUserActiveTimerCount(batch.BatchID, processOperator.UserID) ?? 0;
                            var durationMinutes = taLaborTime.GetBatchUserDuration(batch.BatchID, processOperator.UserID) ?? 0;

                            operatorEntries.Add(new BatchOperatorEntry()
                            {
                                BatchId = batch.BatchID,
                                HasActiveTimer = timerCount > 0,
                                DurationMinutes = durationMinutes,
                                UserId = processOperator.UserID,
                                UserName = users.FindByUserID(processOperator.UserID)?.Name ?? "N/A"
                            });
                        }

                        // Out-of-process
                        var dtOperator = taBatchOperator
                            .GetOperatorsForBatch(nameof(OperatorStatus.Active), batch.BatchID);

                        foreach (var batchOperator in dtOperator)
                        {
                            var timerCount = taBatchOperatorTime.GetActiveTimerCount(batchOperator.BatchOperatorID) ?? 0;
                            var durationMinutes = taBatchOperatorTime.GetDuration(batchOperator.BatchOperatorID) ?? 0;

                            operatorEntries.Add(new BatchOperatorEntry()
                            {
                                BatchId = batch.BatchID,
                                HasActiveTimer = timerCount > 0,
                                DurationMinutes = durationMinutes,
                                UserId = batchOperator.UserID,
                                UserName = users.FindByUserID(batchOperator.UserID)?.Name ?? "N/A"
                            });
                        }
                    }

                    return operatorEntries;
                }
                finally
                {
                    orders?.Dispose();
                    batches?.Dispose();
                    users?.Dispose();
                    taLaborTime?.Dispose();
                    taProcessOrderOperator?.Dispose();
                    taBatchProcessOperator?.Dispose();
                    taOrderOperator?.Dispose();
                    taOrderOperatorTime?.Dispose();
                    taBatchOperator?.Dispose();
                    taBatchOperatorTime?.Dispose();
                }
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        #region StartTimerCommand

        private sealed class StartTimerCommand : ICommand
        {
            #region Properties

            public TimeManagerContext Context
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public StartTimerCommand(TimeManagerContext context)
            {
                Context = context;
            }

            #endregion

            #region ICommand Members

            public event EventHandler CanExecuteChanged
            {
                add
                {
                    CommandManager.RequerySuggested += value;
                }
                remove
                {
                    CommandManager.RequerySuggested -= value;
                }
            }

            public bool CanExecute(object parameter)
            {
                try
                {
                    var activeCount = Context.SelectedActive?.Count() ?? 0;
                    var pausedCount = Context.SelectedPaused?.Count() ?? 0;

                    return activeCount == 0 && pausedCount > 0;
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error determining status for StartTimerCommand");
                    return false;
                }
            }

            public void Execute(object parameter)
            {
                try
                {
                    if (!CanExecute(parameter))
                    {
                        return;
                    }

                    foreach (var pausedItem in Context.SelectedPaused)
                    {
                        pausedItem.StartLaborTimer();
                    }

                    Context.Refresh();
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error running StartTimerCommand");
                }
            }

            #endregion
        }

        #endregion

        #region PauseTimerCommand

        private sealed class PauseTimerCommand : ICommand
        {
            #region Properties

            public TimeManagerContext Context
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public PauseTimerCommand(TimeManagerContext context)
            {
                Context = context;
            }

            #endregion

            #region ICommand Members

            public event EventHandler CanExecuteChanged
            {
                add
                {
                    CommandManager.RequerySuggested += value;
                }
                remove
                {
                    CommandManager.RequerySuggested -= value;
                }
            }

            public bool CanExecute(object parameter)
            {
                try
                {
                    var activeCount = Context.SelectedActive?.Count() ?? 0;
                    var pausedCount = Context.SelectedPaused?.Count() ?? 0;

                    return activeCount > 0 && pausedCount == 0;
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error determining status for PauseTimerCommand");
                    return false;
                }
            }

            public void Execute(object parameter)
            {
                try
                {
                    if (!CanExecute(parameter))
                    {
                        return;
                    }

                    foreach (var activeItem in Context.SelectedActive)
                    {
                        activeItem.PauseLaborTimer();
                    }

                    Context.Refresh();
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error running PauseTimerCommand");
                }
            }

            #endregion
        }

        #endregion

        #region StopTimerCommand

        private sealed class StopTimerCommand : ICommand
        {
            #region Properties

            public TimeManagerContext Context
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public StopTimerCommand(TimeManagerContext context)
            {
                Context = context;
            }

            #endregion

            #region ICommand Members

            public event EventHandler CanExecuteChanged
            {
                add
                {
                    CommandManager.RequerySuggested += value;
                }
                remove
                {
                    CommandManager.RequerySuggested -= value;
                }
            }

            public bool CanExecute(object parameter)
            {
                try
                {
                    var activeCount = Context.SelectedActive?.Count() ?? 0;
                    var pausedCount = Context.SelectedPaused?.Count() ?? 0;

                    return activeCount > 0 || pausedCount > 0;
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error determining status for StopTimerCommand");
                    return false;
                }
            }

            public void Execute(object parameter)
            {
                try
                {
                    if (!CanExecute(parameter))
                    {
                        return;
                    }

                    var selection = new List<IOperatorEntry>();
                    var active = Context.SelectedActive;
                    var paused = Context.SelectedPaused;

                    if (active != null)
                    {
                        selection.AddRange(active);
                    }

                    if (paused != null)
                    {
                        selection.AddRange(paused);
                    }

                    foreach (var selectedItem in selection)
                    {
                        selectedItem.StopLaborTimer();
                    }

                    Context.Refresh();
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error running StopTimerCommand");
                }
            }

            #endregion
        }

        #endregion

        #region MoveTimerCommand

        private sealed class MoveTimerCommand : ICommand
        {
            #region Properties

            public TimeManagerContext Context
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public MoveTimerCommand(TimeManagerContext context)
            {
                Context = context;
            }

            #endregion

            #region ICommand Members

            public event EventHandler CanExecuteChanged
            {
                add
                {
                    CommandManager.RequerySuggested += value;
                }
                remove
                {
                    CommandManager.RequerySuggested -= value;
                }
            }

            public bool CanExecute(object parameter)
            {
                try
                {
                    var activeCount = Context.SelectedActive?.Count() ?? 0;
                    var pausedCount = Context.SelectedPaused?.Count() ?? 0;

                    return activeCount > 0 || pausedCount > 0;
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error determining status for MoveTimerCommand");
                    return false;
                }
            }

            public void Execute(object parameter)
            {
                try
                {
                    if (!CanExecute(parameter))
                    {
                        return;
                    }

                    var selection = new List<IOperatorEntry>();
                    var active = Context.SelectedActive;
                    var paused = Context.SelectedPaused;

                    if (active != null)
                    {
                        selection.AddRange(active);
                    }

                    if (paused != null)
                    {
                        selection.AddRange(paused);
                    }

                    Context.ShowMoveDialog(selection);
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error running MoveTimerCommand");
                }
            }

            #endregion
        }

        #endregion

        #region ManageProcessingCommand

        private sealed class ManageProcessingCommand : ICommand
        {
            #region Properties

            public TimeManagerContext Context
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public ManageProcessingCommand(TimeManagerContext context)
            {
                Context = context;
            }

            #endregion

            #region ICommand Members

            public event EventHandler CanExecuteChanged
            {
                add
                {
                    CommandManager.RequerySuggested += value;
                }
                remove
                {
                    CommandManager.RequerySuggested -= value;
                }
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                try
                {
                    if (!CanExecute(parameter))
                    {
                        return;
                    }

                    Context.ShowProcessingDialog();
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Error(exc, "Error running ManageProcessingCommand");
                }
            }

            #endregion
        }

        #endregion
    }
}
