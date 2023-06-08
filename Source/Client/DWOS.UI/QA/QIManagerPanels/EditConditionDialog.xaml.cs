using DWOS.Data.Conditionals;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DWOS.UI.QA.QIManagerPanels
{
    /// <summary>
    /// Interaction logic for EditConditionDialog.xaml
    /// </summary>
    public partial class EditConditionDialog
    {
        #region Properties

        private DialogContext ViewModel =>
            DataContext as DialogContext;

        #endregion

        #region Methods

        public EditConditionDialog()
        {
            InitializeComponent();
            Icon = Properties.Resources.Equal_32.ToWpfImage();
            DataContext = new DialogContext();
        }

        public void LoadNewCondition(PartInspectionDataSet dsPartInspection,
            PartInspectionDataSet.PartInspectionQuestionRow mainQuestion)
        {
            if (dsPartInspection == null)
            {
                throw new ArgumentNullException(nameof(dsPartInspection));
            }

            if (mainQuestion == null)
            {
                throw new ArgumentNullException(nameof(mainQuestion));
            }

            ViewModel?.LoadForNewCondition(dsPartInspection, mainQuestion);
        }

        public void LoadForExistingCondition(PartInspectionDataSet dsPartInspection,
            PartInspectionDataSet.PartInspectionQuestionConditionRow conditionRow)
        {
            if (dsPartInspection == null)
            {
                throw new ArgumentNullException(nameof(dsPartInspection));
            }

            if (conditionRow == null)
            {
                throw new ArgumentNullException(nameof(conditionRow));
            }

            ViewModel?.LoadForExistingCondition(dsPartInspection, conditionRow);
        }

        public PartInspectionDataSet.PartInspectionQuestionConditionRow ApplyChanges()
        {
            var vm = ViewModel;

            if (vm == null)
            {
                return null;
            }

            vm.ApplyChanges();
            return vm.ConditionRow;
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;

            if (vm != null)
            {
                vm.Accepted += ViewModel_Accepted;
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;

            if (vm != null)
            {
                vm.Accepted -= ViewModel_Accepted;
            }
        }

        private void ViewModel_Accepted(object sender, EventArgs e)
        {
            DialogResult = true;
        }

        #endregion

        #region DialogContext

        private class DialogContext : ViewModelBase
        {
            #region Fields

            public event EventHandler Accepted;
            private InspectionQuestion _selectedQuestion;
            private InspectionQuestion _mainQuestion;
            private ConditionOperator _selectedOperator;
            private string _conditionValue;
            private PartInspectionDataSet _dsPartInspection;

            #endregion

            #region Properties

            public PartInspectionDataSet.PartInspectionQuestionConditionRow ConditionRow { get; set; }

            public ObservableCollection<InspectionQuestion> Questions { get; } =
                new ObservableCollection<InspectionQuestion>();

            public InspectionQuestion SelectedQuestion
            {
                get => _selectedQuestion;

                set
                {
                    if (_selectedQuestion != value)
                    {
                        _selectedQuestion = value;
                        RaisePropertyChanged(nameof(SelectedQuestion));
                        RaisePropertyChanged(nameof(ConditionDescription));
                        RaisePropertyChanged(nameof(InputTypeImage));
                    }
                }
            }

            public InspectionQuestion MainQuestion
            {
                get => _mainQuestion;
                set
                {
                    if (_mainQuestion != value)
                    {
                        _mainQuestion = value;
                        RaisePropertyChanged(nameof(MainQuestion));
                        RaisePropertyChanged(nameof(ConditionDescription));
                    }
                }
            }

            public IEnumerable<ConditionOperator> Operators { get; } = new List<ConditionOperator>
            {
                new ConditionOperator(EqualityOperator.GreaterThan, ">"),
                new ConditionOperator(EqualityOperator.LessThan, "<"),
                new ConditionOperator(EqualityOperator.Equal, "="),
                new ConditionOperator(EqualityOperator.NotEqual, "<>")
            };

            public ConditionOperator SelectedOperator
            {
                get => _selectedOperator;
                set
                {
                    if (_selectedOperator != value)
                    {
                        _selectedOperator = value;
                        RaisePropertyChanged(nameof(SelectedOperator));
                        RaisePropertyChanged(nameof(ConditionDescription));
                    }
                }
            }

            public string ConditionValue
            {
                get => _conditionValue;

                set
                {
                    if (_conditionValue != value)
                    {
                        _conditionValue = value;
                        RaisePropertyChanged(nameof(ConditionValue));
                        RaisePropertyChanged(nameof(ConditionDescription));
                    }
                }
            }

            public string ConditionDescription
            {
                get
                {
                    return $"Show this question if '{_conditionValue}' is '{_selectedOperator?.DisplayText}' " +
                        $"'{_selectedQuestion?.DisplayText}'";
                }
            }

            public BitmapImage InputTypeImage =>
                ControlUtilities.GetInputTypeImage(_selectedQuestion?.InputType ?? Data.Datasets.InputType.None).ToWpfImage();

            public ICommand Accept { get; }

            #endregion

            #region Methods

            public DialogContext()
            {
                Accept = new RelayCommand(
                    () => Accepted?.Invoke(this, EventArgs.Empty),
                    () => _selectedQuestion != null && _selectedOperator != null);
            }

            public void LoadForNewCondition(PartInspectionDataSet dsPartInspection,
                PartInspectionDataSet.PartInspectionQuestionRow mainQuestion)
            {
                if (mainQuestion == null)
                {
                    throw new ArgumentNullException(nameof(mainQuestion));
                }

                _dsPartInspection = dsPartInspection ?? throw new ArgumentNullException(nameof(dsPartInspection));
                ConditionRow = null;
                MainQuestion = new InspectionQuestion(mainQuestion);
                Questions.Clear();

                var allQuestions = mainQuestion.PartInspectionTypeRow
                    .GetPartInspectionQuestionRows()
                    .OrderBy(question => question.StepOrder);

                foreach (var question in allQuestions)
                {
                    if (question.PartInspectionQuestionID == mainQuestion.PartInspectionQuestionID)
                    {
                        // Only add questions that come before mainQuestion
                        break;
                    }

                    Questions.Add(new InspectionQuestion(question));
                }

                SelectedQuestion = Questions.FirstOrDefault();

                SelectedOperator = Operators.FirstOrDefault();
                ConditionValue = string.Empty;
            }

            public void LoadForExistingCondition(PartInspectionDataSet dsPartInspection,
                PartInspectionDataSet.PartInspectionQuestionConditionRow conditionRow)
            {
                _dsPartInspection = dsPartInspection ?? throw new ArgumentNullException(nameof(dsPartInspection));
                ConditionRow = conditionRow ?? throw new ArgumentNullException(nameof(conditionRow));

                var mainQuestion = conditionRow.MainPartInspectionQuestionRow;
                MainQuestion = new InspectionQuestion(mainQuestion);

                Questions.Clear();

                var allQuestions = mainQuestion
                    .PartInspectionTypeRow
                    .GetPartInspectionQuestionRows()
                    .OrderBy(question => question.StepOrder);

                foreach (var question in allQuestions)
                {
                    if (question.PartInspectionQuestionID == mainQuestion.PartInspectionQuestionID)
                    {
                        // Only add questions that come before mainQuestion
                        break;
                    }

                    Questions.Add(new InspectionQuestion(question));
                }

                SelectedQuestion = Questions.FirstOrDefault(q => q.QuestionRow.PartInspectionQuestionID == conditionRow.CheckPartInspectionQuestionID);

                SelectedOperator = Operators.FirstOrDefault(o => o.Value.ToString() == conditionRow.Operator);
                ConditionValue = conditionRow.Value;
            }

            public void ApplyChanges()
            {
                if (ConditionRow == null)
                {
                    // Create new row
                    ConditionRow = _dsPartInspection.PartInspectionQuestionCondition.AddPartInspectionQuestionConditionRow(
                        _mainQuestion.QuestionRow,
                        _selectedQuestion.QuestionRow,
                        _selectedOperator.Value.ToString(),
                        _conditionValue);
                }
                else
                {
                    ConditionRow.CheckPartInspectionQuestionRow = _selectedQuestion.QuestionRow;
                    ConditionRow.Operator = _selectedOperator.Value.ToString();
                    ConditionRow.Value = _conditionValue;
                }
            }

            #endregion
        }

        #endregion

        #region InspectionQuestion

        private class InspectionQuestion
        {
            #region Fields

            private readonly Data.Datasets.InputType _inputType;

            #endregion
            #region Properties

            public string DisplayText =>
                $"{QuestionRow.StepOrder} - {QuestionRow.Name}";

            public PartInspectionDataSet.PartInspectionQuestionRow QuestionRow { get; }

            public Data.Datasets.InputType InputType => _inputType;

            #endregion

            #region Methods

            public InspectionQuestion (PartInspectionDataSet.PartInspectionQuestionRow questionRow)
            {
                QuestionRow = questionRow ?? throw new ArgumentNullException(nameof(questionRow));
                Enum.TryParse(questionRow.InputType, out _inputType);
            }

            #endregion
        }

        #endregion

        #region ConditionOperator

        private class ConditionOperator
        {
            #region Properties

            public EqualityOperator Value { get; }

            public string DisplayText { get; }

            #endregion

            #region Methods

            public ConditionOperator(EqualityOperator value, string displayText)
            {
                Value = value;
                DisplayText = displayText;
            }

            #endregion
        }

        #endregion
    }
}
