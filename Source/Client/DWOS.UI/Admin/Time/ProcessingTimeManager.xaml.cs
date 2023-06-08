using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.UI.Utilities;
using Infragistics.Windows.Ribbon;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DWOS.UI.Admin.Time
{
    /// <summary>
    /// Interaction logic for ProcessingTimeManager.xaml
    /// </summary>
    public partial class ProcessingTimeManager : XamRibbonWindow
    {
        #region Fields

        private static readonly TimeSpan TIMER_INTERVAL = TimeSpan.FromSeconds(15);
        private DispatcherTimer _timer;
        private readonly object _timerStopLock = new object();
        private ProcessingTimeContext _viewModel;
        private readonly GridSettingsPersistence<XamDataGridSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<XamDataGridSettings>("ProcessingTimeManager", new XamDataGridSettings());

        #endregion

        #region Methods

        public ProcessingTimeManager()
        {
            InitializeComponent();
            btnStop.LargeImage = Properties.Resources.TimeStop32.ToWpfImage();
            Icon = Properties.Resources.Clock_32.ToWpfImage();
        }

        public void LoadData()
        {
            _viewModel = new ProcessingTimeContext();
            DataContext = _viewModel;
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
                _viewModel?.Refresh();
                _timer.Start();
            }
        }

        #endregion

        #region ProcessingTimeContext

        /// <summary>
        /// Context for <see cref="ProcessingTimeManager"/>.
        /// </summary>
        private sealed class ProcessingTimeContext : INotifyPropertyChanged
        {
            #region Fields

            private object[] _selectedItems;
            private readonly object _uiUpdateLock = new object();

            #endregion

            #region Properties

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

            public IEnumerable<IProcessTimerEntry> SelectedEntries
            {
                get
                {
                    if (SelectedItems == null)
                    {
                        return null;
                    }

                    return SelectedItems.OfType<IProcessTimerEntry>();
                }
            }

            public IEnumerable<IProcessTimerEntry> ProcessingEntries
            {
                get;
                private set;
            }

            public ICommand StopTimer
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public ProcessingTimeContext()
            {
                var timerEntries = new List<IProcessTimerEntry>();
                ProcessingEntries = GetProcessingEntries();
                StopTimer = new StopTimerCommand(this);
            }

            /// <summary>
            /// Refreshes data for this context.
            /// </summary>
            public void Refresh()
            {
                lock (_uiUpdateLock)
                {
                    var previousSelection = SelectedEntries ?? Enumerable.Empty<IProcessTimerEntry>();

                    ProcessingEntries = GetProcessingEntries();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessingEntries)));

                    var newSelection = new List<IProcessTimerEntry>();
                    foreach (var item in previousSelection)
                    {
                        var matchingEntry = ProcessingEntries.FirstOrDefault(i => i.Id == item.Id);

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

            private static IEnumerable<IProcessTimerEntry> GetProcessingEntries()
            {
                OrderProcessingDataSet.OrderProcessesTimeDataTable orderTimers = null;
                OrderProcessingDataSet.BatchProcessesTimeDataTable batchTimers = null;

                OrderProcessesTableAdapter taOrderProcesses = null;
                OrderProcessesTimeTableAdapter taOrderProcessesTime = null;
                BatchProcessesTableAdapter taBatchProcesses = null;
                BatchProcessesTimeTableAdapter taBatchProcessesTime = null;

                try
                {
                    taOrderProcesses = new OrderProcessesTableAdapter();
                    taOrderProcessesTime = new OrderProcessesTimeTableAdapter();

                    taBatchProcesses = new BatchProcessesTableAdapter();
                    taBatchProcessesTime = new BatchProcessesTimeTableAdapter();

                    var timerEntries = new List<IProcessTimerEntry>();

                    orderTimers = taOrderProcessesTime.GetActiveTimers();

                    foreach (var timer in orderTimers)
                    {
                        var orderProcess = taOrderProcesses
                            .GetByID(timer.OrderProcessesID)
                            .FirstOrDefault();

                        if (orderProcess != null)
                        {
                            var durationMinutes = taOrderProcessesTime.GetTotalDuration(timer.OrderProcessesID, timer.WorkStatus) ?? 0;

                            timerEntries.Add(new OrderProcessTimerEntry()
                            {
                                OrderId = orderProcess.OrderID,
                                WorkStatus = timer.WorkStatus,
                                DurationMinutes = durationMinutes
                            });
                        }
                    }

                    batchTimers = taBatchProcessesTime.GetActiveTimers();

                    foreach (var timer in batchTimers)
                    {
                        var batchProcess = taBatchProcesses
                            .GetByBatchProcess(timer.BatchProcessID)
                            .FirstOrDefault();

                        if (batchProcess != null)
                        {
                            var durationMinutes = taBatchProcessesTime.GetTotalDuration(timer.BatchProcessID, timer.WorkStatus) ?? 0;

                            timerEntries.Add(new BatchProcessTimerEntry()
                            {
                                BatchId = batchProcess.BatchID,
                                WorkStatus = timer.WorkStatus,
                                DurationMinutes = durationMinutes
                            });
                        }
                    }
                    return timerEntries;
                }
                finally
                {
                    orderTimers?.Dispose();
                    batchTimers?.Dispose();

                    taOrderProcesses?.Dispose();
                    taOrderProcessesTime?.Dispose();

                    taBatchProcesses?.Dispose();
                    taBatchProcessesTime?.Dispose();
                }
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        #region StopProcessingCommand

        private sealed class StopTimerCommand : ICommand
        {
            #region Properties

            public ProcessingTimeContext Context
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public StopTimerCommand(ProcessingTimeContext context)
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
                var entryCount = Context.SelectedEntries?.Count();
                return entryCount > 0;
            }

            public void Execute(object parameter)
            {
                try
                {
                    if (!CanExecute(parameter))
                    {
                        return;
                    }

                    foreach (var item in Context.SelectedEntries)
                    {
                        item.StopTimer();
                    }

                    Context.Refresh();
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
