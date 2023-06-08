using System;
using System.Linq;
using System.Windows;
using DWOS.Reports;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Reports
{
    /// <summary>
    /// Interaction logic for ProcessAnswerReportOptions.xaml
    /// </summary>
    public partial class ProcessAnswerReportOptions
    {
        #region Fields

        private readonly PersistWpfWindowState _persistState;

        #endregion

        #region Properties

        private ProcessAnswerContext ViewModel => DataContext as ProcessAnswerContext;

        #endregion

        #region Methods

        public ProcessAnswerReportOptions()
        {
            _persistState = new PersistWpfWindowState(this);
            InitializeComponent();
            DataContext = new ProcessAnswerContext();
            Icon = Properties.Resources.Report32.ToWpfImage();
        }

        public ProcessAnswerReport GenerateReport()
        {
            return ViewModel?.GenerateReport();
        }

        #endregion

        #region Events

        private void ProcessAnswerReportOptions_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if (vm == null)
                {
                    return;
                }

                vm.LoadData();
                vm.Accepted += VmOnAccepted;
                vm.ShowQuestionGroupDialog += VmOnShowQuestionGroupDialog;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading report dialog.");
            }
        }

        private void ProcessAnswerReportOptions_OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if (vm == null)
                {
                    return;
                }

                vm.Accepted -= VmOnAccepted;
                vm.ShowQuestionGroupDialog -= VmOnShowQuestionGroupDialog;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error unloading report dialog.");
            }
        }

        private void VmOnAccepted(object sender, EventArgs eventArgs)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting DialogResult");
            }
        }

        private void VmOnShowQuestionGroupDialog(object sender, EventArgs eventArgs)
        {
            try
            {
                var vm = ViewModel;

                if (vm == null)
                {
                    return;
                }

                // Select process
                var vmSelectedProcesses = vm.CastedSelectedProcesses.ToList();
                var selectProcessDialog = new SelectProcessDialog
                {
                    Owner = this,
                    Processes = vmSelectedProcesses.SelectMany(p => p.AllRevisions),
                    SelectedProcess = vmSelectedProcesses.FirstOrDefault()
                };

                if ((selectProcessDialog.ShowDialog() ?? false) && selectProcessDialog.SelectedProcess != null)
                {
                    var selectedProcessId = selectProcessDialog.SelectedProcess.ProcessId;

                    // Add question
                    var groupDialog = new ProcessGroupDialog
                    {
                        Owner = this
                    };

                    groupDialog.LoadData(selectedProcessId, vm.QuestionGroups);

                    if (groupDialog.ShowDialog() ?? false)
                    {
                        var processQuestionGroup = groupDialog.SelectedProcessQuestionGroup;

                        if (processQuestionGroup != null)
                        {
                            vm.QuestionGroups.Add(processQuestionGroup);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error showing question group dialog.");
            }
        }

        #endregion
    }
}
