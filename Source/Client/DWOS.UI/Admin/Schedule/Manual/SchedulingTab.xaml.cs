using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DWOS.Data;
using DWOS.Reports;
using DWOS.Reports.Reports;
using DWOS.Shared;
using DWOS.Shared.Utilities;
using DWOS.UI.Tools;
using DWOS.UI.Utilities;
using Infragistics.Windows.DockManager;
using Infragistics.Windows.DockManager.Events;
using NLog;

namespace DWOS.UI.Admin.Schedule.Manual
{
    /// <summary>
    /// Interaction logic for SchedulingTab.xaml
    /// </summary>
    public partial class SchedulingTab : ISchedulingTab, IReportTab
    {
        #region Fields

        public const string DATA_TYPE = "scheduler";
        private bool _initialLoad = true;
        private GridSettingsPersistence<SchedulingDepartmentTabSettings> _tabSettingsPersistence =
            new GridSettingsPersistence<SchedulingDepartmentTabSettings>("SchedulingTab", new SchedulingDepartmentTabSettings());

        private SchedulerType _currentSchedulerType;

        #endregion

        #region Properties

        private SchedulingTabDataContext ViewModel => DataContext as SchedulingTabDataContext;

        #endregion

        #region Methods

        public SchedulingTab()
        {
            InitializeComponent();
            DataContext = new SchedulingTabDataContext(new SchedulingPersistence(SecurityManager.Current),
                DependencyContainer.Resolve<IDwosApplicationSettingsProvider>());
        }

        public void Initialize(WipData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            _currentSchedulerType = ApplicationSettings.Current.SchedulerType;
            ViewModel?.Initialize(data);
        }

        public void RefreshSettings()
        {
            if (ViewModel == null)
            {
                return;
            }

            ViewModel.UpdateSettings();

            if (ApplicationSettings.Current.SchedulerType != _currentSchedulerType)
            {
                ResetTabs();

                // Initialize data
                _currentSchedulerType = ApplicationSettings.Current.SchedulerType;
                ViewModel.RefreshSchedulerType();

                // Add tabs for each department
                foreach (var dept in ViewModel.Departments.OrderByDescending(d => d.Name))
                {
                    AddNewTab(dept);
                }
            }
        }

        public void RefreshData(WipData data)
        {
            ViewModel?.UpdateData(data);
        }

        private void ResetTabs()
        {
            LogManager.GetCurrentClassLogger().Info($"Resetting tabs for scheduler tab {Key}");

            var documents = DockManager
                .GetPanes(PaneNavigationOrder.VisibleOrder)
                .ToList(); // ToList() call is needed - won't close tabs otherwise

            foreach (var doc in documents)
            {
                doc.AllowClose = true;
                doc.ExecuteCommand(ContentPaneCommands.Close);
            }
        }

        private static void DoExport(SchedulingTabDataContext viewModel)
        {
            if (viewModel == null)
            {
                return;
            }

            // Construct table
            var tables = new List<ExcelReport.ReportTable>();
            foreach (var dept in viewModel.Departments.Where(d => d.Scheduled.Any()).OrderBy(d => d.Name))
            {
                var reportTable = new ExcelReport.ReportTable
                {
                    Name = dept.Name,
                    Columns = new List<ExcelReport.Column>
                    {
                        new ExcelReport.Column("Priority") { Width = 10 },
                        new ExcelReport.Column("ID") { Width = 6 },
                        new ExcelReport.Column("Type") { Width = 6 },
                        new ExcelReport.Column("Customer") {Width = 50}
                    }
                };

                var rows = new List<ExcelReport.Row>();

                foreach (var scheduledOrder in dept.Scheduled.OrderBy(o => o.SchedulePriority))
                {
                    var row = new ExcelReport.Row
                    {
                        Cells = new object[] {scheduledOrder.SchedulePriority, scheduledOrder.Id, scheduledOrder.Type, scheduledOrder.Customer }
                    };

                    rows.Add(row);
                }

                reportTable.Rows = rows;
                tables.Add(reportTable);
            }

            // Print
            using (var report = new ExcelReport("Schedule", tables))
            {
                report.DisplayReport();
            }
        }

        private void AddNewTab(DepartmentDataContext dept)
        {
            var tab = new SchedulingDepartmentTab { DataContext = dept, SettingsPersistence = _tabSettingsPersistence };
            ContentPane contentPane = DockManager.AddDocument(dept.Name, tab);
            contentPane.AllowClose = false;
            contentPane.CloseAction = PaneCloseAction.RemovePane;
            contentPane.Closed += SchedulingTab_Closed;
        }

        #endregion

        #region Events

        private void SchedulingTab_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!_initialLoad || ViewModel == null)
            {
                return;
            }

            foreach (var dept in ViewModel.Departments.OrderByDescending(d => d.Name))
            {
                AddNewTab(dept);
            }

            ViewModel.OnLoaded();
            ViewModel.DepartmentAdded += VmOnDepartmentAdded;
            ViewModel.DataSaved += VmOnDataSaved;

            _initialLoad = false;
        }

        private void VmOnDepartmentAdded(object sender, DepartmentAddedEventArgs departmentAddedEventArgs)
        {
            var dept = departmentAddedEventArgs?.Context;

            if (dept == null)
                return;

            AddNewTab(dept);
        }

        private void VmOnDataSaved(object sender, EventArgs e)
        {
            DWOSApp.MainForm.RefreshData(RefreshType.Order);
        }

        private void SchedulingTab_Closed(object sender, PaneClosedEventArgs e)
        {
            // Prevent closed event from bubbling up to main WIP screen.
            e.Handled = true;
        }

        #endregion

        #region ISchedulingTab Members

#pragma warning disable 67
        public event EventHandler LayoutError;
#pragma warning restore 67

        public string DataType => DATA_TYPE;

        public string Key
        {
            get;
            set;
        }

        public UserControl TabControl => this;

        public string TabName
        {
            get; set;
        }

        public void LoadLayout()
        {
            // No layout to load
        }

        public void LoadLayout(string content)
        {
            // No layout to load
        }

        public void SaveLayout()
        {
            // No layout to save
        }

        public DwosTabData Export()
        {
            return new DwosTabData
            {
                Name = TabName,
                DataType = DataType,
                Key = Key
            };
        }

        #endregion

        #region IReportTab Members

        public void DisplayReport()
        {
            using (new UsingWaitCursorWpf(this))
            {
                try
                {
                    var vm = ViewModel;

                    if (vm == null)
                    {
                        return;
                    }

                    if (vm.HasChanges)
                    {
                        MessageBoxUtilities.ShowMessageBoxWarn("Please save your changes before printing the schedule.",
                            "Order Scheduling");
                    }
                    else
                    {
                        DoExport(ViewModel);
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessageBox.ShowDialog("Error printing schedule", exc);
                }
            }
        }

        public void OnClose()
        {
            var vm = ViewModel;

            if (vm == null)
            {
                return;
            }

            vm.OnUnloaded();
            vm.DepartmentAdded -= VmOnDepartmentAdded;
            vm.DataSaved -= VmOnDataSaved;
        }

        #endregion
    }
}
