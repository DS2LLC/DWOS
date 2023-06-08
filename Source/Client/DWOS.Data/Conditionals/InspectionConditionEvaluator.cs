using DWOS.Data.Datasets;

namespace DWOS.Data.Conditionals
{
    /// <summary>
    /// Evaluates inspection conditions.
    /// </summary>
    public class InspectionConditionEvaluator
    {
        #region Properties

        public string Answer { get; }

        public InputType InputType { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="InspectionConditionEvaluator"/> class.
        /// </summary>
        /// <param name="answer">Answer to evaluate conditions against.</param>
        /// <param name="inputType">The input type of the answer.</param>
        public InspectionConditionEvaluator(string answer, InputType inputType)
        {
            Answer = answer;
            InputType = inputType;
        }

        /// <summary>
        /// Evaluates a condition.
        /// </summary>
        /// <param name="conditionRow"></param>
        /// <returns>
        /// <c>true</c> if the condition is met, <c>false</c> if the condition
        /// is not met, and <c>null</c> if data is invalid.
        /// </returns>
        public bool? Evaluate(PartInspectionDataSet.PartInspectionQuestionConditionRow conditionRow)
        {
            if (conditionRow == null)
            {
                return null;
            }

            var conditionOperator = conditionRow.Operator.ConvertToEnum<EqualityOperator>();
            return ConditionEvaluator.MeetsCondition(InputType, conditionRow.Value, conditionOperator, Answer);
        }

        #endregion
    }
}
