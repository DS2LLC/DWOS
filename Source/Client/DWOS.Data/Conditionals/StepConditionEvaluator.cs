using DWOS.Data.Datasets;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.Data.Conditionals
{
    /// <summary>
    /// Implements step condition evaluation functionality.
    /// </summary>
    public class StepConditionEvaluator
    {
        #region Properties

        /// <summary>
        /// Gets or sets the order ID.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the collection of order process answers.
        /// </summary>
        /// <remarks>
        /// All answers in this question should (but are not required to)
        /// belong to the same order process.
        /// </remarks>
        public IEnumerable<OrderProcessingDataSet.OrderProcessAnswerRow> OrderProcessAnswers { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Evaluates a condition.
        /// </summary>
        /// <param name="condition">Condition to evaluate</param>
        /// <returns>
        /// true if the condition is met; false if the condition is unmet;
        /// null if the condition cannot be evaluated.
        /// </returns>
        public bool? Evaluate(StepCondition condition)
        {
            if (OrderProcessAnswers == null)
            {
                throw new InvalidOperationException("Must set OrderProcessAnswers.");
            }

            if (OrderProcessAnswers.Select(opa => opa.OrderProcessesID).Distinct().Count() != 1)
            {
                const string msgFormat = "{0} has answers from multiple order processes.";
                LogManager.GetCurrentClassLogger().Warn(msgFormat, nameof(OrderProcessAnswers));
            }

            switch (condition.InputType)
            {
                case ConditionInputType.ProcessQuestion:
                    var questionAnswer = OrderProcessAnswers.FirstOrDefault(opa => opa.ProcessQuestionID == condition.ProcessQuestionId);
                    if (questionAnswer != null && questionAnswer.Completed)
                    {
                        return ConditionEvaluator.MeetsCondition(questionAnswer.ProcessQuestionRow.InputType.ConvertToEnum<InputType>(), condition.Value, condition.Operator, questionAnswer.IsAnswerNull() ? null : questionAnswer.Answer);
                    }
                    else
                    {
                        return null;
                    }
                case ConditionInputType.PartTag:
                    return ConditionEvaluator.MeetsPartTagConditional(condition.Value, OrderId);
            }

            return false;
        }

        #endregion
    }
}
