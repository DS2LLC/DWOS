using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ProcessesDatasetTableAdapters;
using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using NLog;

namespace DWOS.UI.Reports
{
    /// <summary>
    /// Interaction logic for ProcessGroupDialog.xaml
    /// </summary>
    public partial class ProcessGroupDialog
    {
        #region Properties

        private DialogContext ViewModel => DataContext as DialogContext;

        public ProcessQuestionGroup SelectedProcessQuestionGroup =>
            ViewModel?.CreateProcessQuestionGroup();

        #endregion

        #region Methods

        public ProcessGroupDialog()
        {
            InitializeComponent();
            DataContext = new DialogContext();
            Icon = Properties.Resources.Report32.ToWpfImage();
        }

        public void LoadData(int processId, ICollection<ProcessQuestionGroup> existingGroups)
        {
            ViewModel?.LoadProcessInfo(processId, existingGroups);
        }


        #endregion

        #region Events

        private void ProcessGroupDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if (vm == null)
                {
                    return;
                }

                vm.Accepted += VmOnAccepted;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error loading process group dialog.");
            }
        }

        private void ProcessGroupDialog_OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if (vm == null)
                {
                    return;
                }

                vm.Accepted -= VmOnAccepted;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error unloading process group dialog.");
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

        #endregion

        #region DialogContext

        private class DialogContext : INotifyPropertyChanged
        {
            #region Fields

            public event EventHandler Accepted;
            private ProcessStep _selectedStep;
            private ProcessQuestion _selectedQuestion;
            private int _processId;
            private string _processName;

            #endregion

            #region Properties

            public string ProcessName
            {
                get => _processName;
                set
                {
                    if (_processName != value)
                    {
                        _processName = value;
                        OnPropertyChanged(nameof(ProcessName));
                    }
                }
            }

            public ObservableCollection<ProcessStep> Steps { get; } =
                new ObservableCollection<ProcessStep>();

            public ProcessStep SelectedStep
            {
                get => _selectedStep;
                set
                {
                    if (_selectedStep != value)
                    {
                        _selectedStep = value;
                        OnPropertyChanged(nameof(SelectedStep));

                        StepQuestions.Clear();
                        if (_selectedStep?.Questions != null)
                        {
                            foreach (var question in _selectedStep.Questions)
                            {
                                StepQuestions.Add(question);
                            }
                        }
                    }
                }
            }

            public ObservableCollection<ProcessQuestion> StepQuestions { get; } =
                new ObservableCollection<ProcessQuestion>();

            public ProcessQuestion SelectedQuestion
            {
                get => _selectedQuestion;
                set
                {
                    if (_selectedQuestion != value)
                    {
                        _selectedQuestion = value;
                        OnPropertyChanged(nameof(SelectedQuestion));
                    }
                }
            }

            public ICommand Accept { get; }

            public ICommand ClearIdentifyBy { get; }

            #endregion

            #region Methods

            public DialogContext()
            {
                Accept = new RelayCommand(
                    () =>
                    {
                        Accepted?.Invoke(this, EventArgs.Empty);
                    },
                    () => _selectedStep != null && (_selectedQuestion != null || StepQuestions.Any(q => q.Include)));

                ClearIdentifyBy = new RelayCommand(
                    () =>
                    {
                        SelectedQuestion = null;
                    });
            }

            public void LoadProcessInfo(int processId, ICollection<ProcessQuestionGroup> existingGroups)
            {
                Steps.Clear();

                var dtProcessQuestion = new ProcessesDataset.ProcessQuestionDataTable();
                var dtSteps = new ProcessesDataset.ProcessStepsDataTable();

                _processId = processId;
                using (var dtProcess = new ProcessesDataset.ProcessDataTable())
                {
                    using (var taProcess = new ProcessTableAdapter())
                    {
                        taProcess.FillByProcess(dtProcess, processId);
                    }

                    if (dtProcess.Count == 1)
                    {
                        var revision = dtProcess[0].IsRevisionNull()
                            ? string.Empty
                            : " Rev. " + dtProcess[0].Revision;

                        _processName = dtProcess[0].Name +
                                      revision;
                    }
                }

                if (string.IsNullOrEmpty(_processName))
                {
                    _processName = processId.ToString();
                }

                using (var taProcessQuestion = new ProcessQuestionTableAdapter())
                {
                    taProcessQuestion.FillBy(dtProcessQuestion, processId);
                }

                using (var taProcessSteps = new ProcessStepsTableAdapter())
                {
                    taProcessSteps.FillBy(dtSteps, processId);
                }

                var stepsToSkip = (existingGroups ?? Enumerable.Empty<ProcessQuestionGroup>())
                    .Select(p => p.ProcessStepId)
                    .ToList();


                foreach (var stepRow in dtSteps)
                {
                    if (stepsToSkip.Contains(stepRow.ProcessStepID))
                    {
                        continue;
                    }

                    var questions = new List<ProcessQuestion>();

                    foreach (var questionRow in dtProcessQuestion.Where(q => q.ProcessStepID == stepRow.ProcessStepID))
                    {
                        questions.Add(new ProcessQuestion
                        {
                            ProcessQuestionId = questionRow.ProcessQuestionID,
                            Order = questionRow.StepOrder,
                            Name = questionRow.Name
                        });
                    }

                    if (questions.Count > 0)
                    {
                        Steps.Add(new ProcessStep
                        {
                            ProcessStepId = stepRow.ProcessStepID,
                            Name = stepRow.Name,
                            Order = stepRow.StepOrder,
                            Questions = questions
                        });
                    }
                }
            }

            public ProcessQuestionGroup CreateProcessQuestionGroup()
            {
                if (_selectedStep == null)
                {
                    return null;
                }

                return new ProcessQuestionGroup
                {
                    ProcessId = _processId,
                    ProcessName = _processName,
                    ProcessStepId = _selectedStep.ProcessStepId,
                    ProcessStepOrder = _selectedStep.Order,
                    ProcessStepName = _selectedStep.Name,
                    ProcessQuestionId = _selectedQuestion?.ProcessQuestionId,
                    ProcessQuestionOrder = _selectedQuestion?.Order,
                    ProcessQuestionName = _selectedQuestion?.Name,
                    Questions = StepQuestions
                        .Where(q => q != null)
                        .Select(q => q.ToGroupQuestion())
                        .ToList()
                };
            }

            private void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion

        #region ProcessStep

        public class ProcessStep
        {
            public List<ProcessQuestion> Questions { get; set; }

            public int ProcessStepId { get; set; }

            public decimal Order { get; set; }

            public string Name { get; set; }

            public string DisplayString => $"{Order} - {Name}";
        }

        #endregion

        #region ProcessQuestion

        public class ProcessQuestion : INotifyPropertyChanged
        {
            #region Fields

            private int _processQuestionId;
            private decimal _order;
            private string _name;
            private bool _include;

            #endregion

            #region Properties

            public int ProcessQuestionId
            {
                get => _processQuestionId;
                set
                {
                    if (_processQuestionId != value)
                    {
                        _processQuestionId = value;
                        OnPropertyChanged(nameof(ProcessQuestionId));
                    }
                }
            }

            public decimal Order
            {
                get => _order;
                set
                {
                    if (_order != value)
                    {
                        _order = value;
                        OnPropertyChanged(nameof(Order));
                    }
                }
            }

            public string Name
            {
                get => _name;
                set
                {
                    if (_name != value)
                    {
                        _name = value;
                        OnPropertyChanged(nameof(Name));
                    }
                }
            }

            public bool Include
            {
                get => _include;
                set
                {
                    if (_include != value)
                    {
                        _include = value;
                        OnPropertyChanged(nameof(Include));
                    }
                }
            }

            public string DisplayString => $"{Order} - {Name}";

            #endregion

            #region Methods

            private void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            public ProcessQuestionGroup.GroupQuestion ToGroupQuestion()
            {
                return new ProcessQuestionGroup.GroupQuestion
                {
                    QuestionId = _processQuestionId,
                    Include =  _include,
                    Name = _name
                };
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion
    }
}
