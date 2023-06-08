using DWOS.Data;
using DWOS.UI.Utilities;
using Infragistics.Windows.DataPresenter;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace DWOS.UI.Admin.Schedule.Manual
{
    /// <summary>
    /// Interaction logic for SchedulingDepartmentTab.xaml
    /// </summary>
    public partial class SchedulingDepartmentTab
    {
        #region Fields

        private static string UnscheduledDragDropData = "Unscheduled";
        private static string ScheduledDragDropData = "Scheduled";
        private bool _isLoaded;
        private bool _currentlyDragging;

        #endregion

        #region Properties

        private DepartmentDataContext ViewModel => DataContext as DepartmentDataContext;

        /// <summary>
        /// Gets or sets the settings persistence for this instance.
        /// </summary>
        public GridSettingsPersistence<SchedulingDepartmentTabSettings> SettingsPersistence { get; set; }

        #endregion

        #region Methods

        public SchedulingDepartmentTab()
        {
            InitializeComponent();
        }

        public void RefreshFields()
        {
            var visibility = ApplicationSettings.Current.MultipleLinesEnabled
                ? Visibility.Visible
                : Visibility.Collapsed;

            var currentLineScheduled =
                Scheduled.DefaultFieldLayout.FieldItems.FirstOrDefault(f => f.Name == "CurrentLine");
            if (currentLineScheduled != null)
            {
                currentLineScheduled.Visibility = visibility;
            }

            var currentLineUnscheduled =
                Unscheduled.DefaultFieldLayout.FieldItems.FirstOrDefault(f => f.Name == "CurrentLine");
            if (currentLineUnscheduled != null)
            {
                currentLineUnscheduled.Visibility = visibility;
            }
        }

        private void SaveSettings()
        {
            // Save settings
            var settings = new SchedulingDepartmentTabSettings
            {
                Unscheduled = new XamDataGridSettings(),
                Scheduled = new XamDataGridSettings()
            };

            settings.Unscheduled.RetrieveSettingsFrom(Unscheduled);
            settings.Scheduled.RetrieveSettingsFrom(Scheduled);
            SettingsPersistence?.SaveSettings(settings);
        }
        private static void CleanupGridDropAdorners(DataRecordPresenter presenter)
        {
            var layer = AdornerLayer.GetAdornerLayer(presenter);
            var adorners = layer.GetAdorners(presenter);
            if (adorners != null)
            {
                foreach (var adorner in adorners.OfType<GridDropAdorner>())
                {
                    layer.Remove(adorner);
                }
            }
        }

        #endregion

        #region Events

        private void SchedulingDepartmentTab_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RefreshFields();

                var vm = ViewModel;

                if (vm != null && !_isLoaded)
                {
                    vm.FieldsRefreshed += OnFieldsRefreshed;
                    vm.ScheduleOrderChanged += OnScheduleChanged;
                    vm.DataRefreshed += OnDataRefreshed;
                    vm.PropertyChanged += ViewModelPropertyChanged;
                    _isLoaded = true;
                }

                // Load Settings
                SettingsPersistence?.LoadSettings().Unscheduled?.ApplyTo(Unscheduled);
                SettingsPersistence?.LoadSettings().Scheduled?.ApplyTo(Scheduled);

                // Detect column width changes
                foreach (var field in Unscheduled.FieldLayouts[0].Fields.Concat(Scheduled.FieldLayouts[0].Fields))
                {
                    field.PropertyChanged += Field_PropertyChanged;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error loading tab.");
            }
        }

        private void Field_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(Infragistics.Windows.DataPresenter.Field.LabelWidthResolved))
                {
                    // Width changed - save layout changes
                    SaveSettings();
                }

                // Could handle property change for ActualPosition to handle position changes,
                // but it always fires for multiple fields.
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error handling property changed");
            }
        }

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if (vm == null)
                {
                    return;
                }

                if (e.PropertyName == nameof(DepartmentDataContext.UnscheduledSelection))
                {
                    // Clear appearance that unscheduled has selection
                    var unscheduledSelection = ViewModel?.UnscheduledSelection;

                    if (unscheduledSelection == null || unscheduledSelection.Length == 0)
                    {
                        Unscheduled.ActiveRecord = null;
                    }
                }
                else if (e.PropertyName == nameof(DepartmentDataContext.ScheduledSelection))
                {
                    // Clear appearance that scheduled has selection
                    var scheduledSelection = ViewModel?.ScheduledSelection;

                    if (scheduledSelection == null || scheduledSelection.Length == 0)
                    {
                        Scheduled.ActiveRecord = null;
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling property change.");
            }
        }

        private void SchedulingDepartmentTab_OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if (vm == null)
                {
                    return;
                }

                vm.FieldsRefreshed -= OnFieldsRefreshed;
                vm.ScheduleOrderChanged -= OnScheduleChanged;
                vm.DataRefreshed -= OnDataRefreshed;
                vm.PropertyChanged += ViewModelPropertyChanged;
                _isLoaded = false;

                // Detect column width changes
                foreach (var field in Unscheduled.FieldLayouts[0].Fields.Concat(Scheduled.FieldLayouts[0].Fields))
                {
                    field.PropertyChanged -= Field_PropertyChanged;
                }

                SaveSettings();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error unloading tab.");
            }
        }

        private void OnScheduleChanged(object sender, EventArgs eventArgs)
        {
            try
            {
                Scheduled.Records.RefreshSort();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error refreshing sort.");
            }
        }

        private void OnDataRefreshed(object sender, EventArgs eventArgs)
        {
            try
            {
                Unscheduled.Records.RefreshSort();
                Scheduled.Records.RefreshSort();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error refreshing sort.");
            }
        }

        private void OnFieldsRefreshed(object sender, EventArgs e)
        {
            try
            {
                if (IsVisible)
                {
                    RefreshFields();
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error refreshing fields.");
            }
        }

        private void Grid_FieldPositionChanged(object sender, Infragistics.Windows.DataPresenter.Events.FieldPositionChangedEventArgs e)
        {
            try
            {
                SaveSettings();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error handing column move event");
            }
        }

        private void RecordSelector_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_currentlyDragging)
                {
                    e.Handled = true;
                    return;
                }

                if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
                {
                    _currentlyDragging = true;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling mouse down event.");
            }
        }

        private void RecordSelector_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    return;
                }

                _currentlyDragging = false;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling mouse leave.");
            }
        }

        private void RecordSelector_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton != MouseButton.Left)
                {
                    return;
                }

                _currentlyDragging = false;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling mouse up.");
            }
        }

        private void UnscheduledRecordSelector_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    return;
                }

                var vm = ViewModel;

                if (vm == null)
                {
                    return;
                }

                var unscheduledOrders = vm.UnscheduledSelection
                    ?.OfType<DepartmentDataContext.ScheduleData>()
                    ?.ToList();

                if (unscheduledOrders != null)
                {
                    var unscheduledData = new DataObject(UnscheduledDragDropData,
                        unscheduledOrders);

                    DragDrop.DoDragDrop(Unscheduled,
                        unscheduledData,
                        DragDropEffects.Move);
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling mouse move.");
            }
        }

        private void Scheduled_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (!e.Data.GetDataPresent(UnscheduledDragDropData))
                {
                    return;
                }

                var vm = ViewModel;

                var unscheduledOrders = e.Data.GetData(UnscheduledDragDropData)
                    as IEnumerable<DepartmentDataContext.ScheduleData>;

                if (vm == null || unscheduledOrders == null)
                {
                    return;
                }

                vm.AddToSchedule(unscheduledOrders.ToList());
                e.Handled = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling drop.");
            }
        }

        private void Scheduled_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(UnscheduledDragDropData) || e.Data.GetDataPresent(ScheduledDragDropData))
                {
                    e.Effects = DragDropEffects.Move;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling drag over.");
            }
        }

        private void ScheduledRecordSelector_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    return;
                }

                var vm = ViewModel;

                if (vm == null)
                {
                    return;
                }

                var scheduledOrders = vm.ScheduledSelection
                    ?.OfType<DepartmentDataContext.ScheduleData>()
                    ?.ToList();

                if (scheduledOrders != null)
                {
                    var scheduledData = new DataObject(ScheduledDragDropData,
                        scheduledOrders);

                    DragDrop.DoDragDrop(Scheduled,
                        scheduledData,
                        DragDropEffects.Move);
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling mouse move.");
            }
        }

        private void Unscheduled_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(ScheduledDragDropData))
                {
                    e.Effects = DragDropEffects.Move;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling drag over.");
            }
        }

        private void Unscheduled_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (!e.Data.GetDataPresent(ScheduledDragDropData))
                {
                    return;
                }

                var vm = ViewModel;

                var scheduledOrders = e.Data.GetData(ScheduledDragDropData)
                    as IEnumerable<DepartmentDataContext.ScheduleData>;

                if (vm == null || scheduledOrders == null)
                {
                    return;
                }

                vm.RemoveFromSchedule(scheduledOrders.ToList());
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling drop.");
            }
        }

        private void DataRecordPresenter_Drop(object sender, DragEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if (vm == null)
                {
                    return;
                }

                // If targetItem is null, it is assumed that the drop
                // occurred on the header.
                var targetItem = (sender as DataRecordPresenter)
                    ?.DataRecord
                    ?.DataItem as DepartmentDataContext.ScheduleData;

                if (e.Data.GetDataPresent(ScheduledDragDropData))
                {
                    var scheduledOrders = e.Data.GetData(ScheduledDragDropData)
                        as IEnumerable<DepartmentDataContext.ScheduleData>;

                    if (scheduledOrders != null)
                    {
                        if (targetItem == null)
                        {
                            vm.MoveToTop(scheduledOrders.ToList());
                        }
                        else
                        {
                            vm.MoveBelow(scheduledOrders.ToList(), targetItem);
                        }

                        e.Handled = true;
                    }
                }

                if (e.Data.GetDataPresent(UnscheduledDragDropData))
                {
                    var unscheduledOrders = e.Data.GetData(UnscheduledDragDropData)
                        as IEnumerable<DepartmentDataContext.ScheduleData>;

                    if (unscheduledOrders != null)
                    {
                        var orderList = unscheduledOrders.ToList();
                        vm.AddToSchedule(orderList);

                        if (targetItem == null)
                        {
                            vm.MoveToTop(orderList);
                        }
                        else
                        {
                            vm.MoveBelow(orderList, targetItem);
                        }

                        e.Handled = true;
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling drop.");
            }

            try
            {

                if (sender is DataRecordPresenter presenter)
                {
                    CleanupGridDropAdorners(presenter);
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error cleaning up adorners.");
            }
        }

        private void DataRecordPresenter_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(ScheduledDragDropData) || e.Data.GetDataPresent(UnscheduledDragDropData))
                {
                    e.Effects = DragDropEffects.Move;

                    if (sender is DataRecordPresenter presenter)
                    {
                        AdornerLayer.GetAdornerLayer(presenter)?.Add(new GridDropAdorner(presenter));
                    }
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling drag over.");
            }
        }

        private void DataRecordPresenter_DragLeave(object sender, DragEventArgs e)
        {
            try
            {
                if (sender is DataRecordPresenter presenter)
                {
                    CleanupGridDropAdorners(presenter);
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling drag leave.");
            }
        }

        #endregion
    }
}
