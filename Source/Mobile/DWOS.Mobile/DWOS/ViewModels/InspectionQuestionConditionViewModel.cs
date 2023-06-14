using DWOS.Services.Messages;
using System;

namespace DWOS.ViewModels
{
    public sealed class InspectionQuestionConditionViewModel : ViewModelBase
    {
        private bool _passed;

        public bool Passed
        {
            get => _passed;
            set
            {
                if (_passed != value)
                {
                    _passed = value;
                    OnPropertyChanged(nameof(Passed));
                }
            }
        }

        /// <summary>
        /// Gets or sets the ID of the question to check.
        /// </summary>
        public int CheckInspectionQuestionId { get; }

        /// <summary>
        /// Gets or sets the operator for this instance.
        /// </summary>
        public EqualityOperator Operator { get; }

        /// <summary>
        /// Gets or sets the value for this instance.
        /// </summary>
        public string Value { get; }

        private InspectionQuestionConditionViewModel(int checkInspectionQuestionId, EqualityOperator op, string value)
        {
            CheckInspectionQuestionId = checkInspectionQuestionId;
            Operator = op;
            Value = value;
        }

        public static InspectionQuestionConditionViewModel From(InspectionQuestionCondition condition)
        {
            if (condition == null)
            {
                return null;
            }

            Enum.TryParse<EqualityOperator>(condition.Operator, out var op);

            return new InspectionQuestionConditionViewModel(condition.CheckInspectionQuestionId,
                op,
                condition.Value);
        }

        public void Check(InspectionQuestionViewModel question)
        {
            if (question == null || question.InspectionQuestionID != CheckInspectionQuestionId)
            {
                return;
            }

            Passed = ConditionHelpers.MeetsCondition(question.InputType,
                Value,
                Operator,
                question.Answer);
        }
    }
}
