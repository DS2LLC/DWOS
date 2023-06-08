using DWOS.Data.Datasets;
using System;
using System.Collections.Generic;

namespace DWOS.Data.Conditionals
{
    /// <summary>
    /// Defines utility methods related to conditionals.
    /// </summary>
    public static class ConditionEvaluator
    {
        /// <summary>
        /// Returns a display string for the given
        /// <see cref="EqualityOperator"/> instance.
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static string EqualityToString(EqualityOperator op)
        {
            switch (op)
            {
                case EqualityOperator.GreaterThan:
                    return ">";
                case EqualityOperator.LessThan:
                    return "<";
                case EqualityOperator.Equal:
                    return "=";
                case EqualityOperator.NotEqual:
                    return "<>";
                case EqualityOperator.None:
                    return "None";
                default:
                    return "N/A";
            }
        }

        /// <summary>
        /// Returns a display string for the given
        /// <see cref="ProcessesDataset.ProcessStepConditionRow"/> instance.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string ConditionToString(ProcessesDataset.ProcessStepConditionRow condition)
        {
            var text = "None";

            // Question condition
            if (condition.InputType == nameof(ConditionInputType.ProcessQuestion) && condition.ProcessQuestionRow != null)
            {
                var processStepsRow = condition.ProcessQuestionRow.ProcessStepsRow;
                var processQuestionRow = condition.ProcessQuestionRow;

                text = $"Show this step if '{condition.Value}' is '{condition.Operator}' " +
                    $"'{processStepsRow.StepOrder.ToString()}: {processStepsRow.Name} - {processQuestionRow.Name}'";
            }

            // Part Tag condition
            if (condition.InputType == nameof(ConditionInputType.PartTag))
            {
                text = $"Show this step if Part has tag '{condition.Value}'";
            }

            return text;
        }

        /// <summary>
        /// Returns a display string for the given
        /// <see cref="OrderProcessingDataSet.ProcessStepConditionRow"/>
        /// instance.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string ConditionToString(OrderProcessingDataSet.ProcessStepConditionRow condition)
        {
            var text = "None";

            // Question condition
            if (condition.InputType == nameof(ConditionInputType.ProcessQuestion) && !condition.IsProcessQuestionIdNull())
            {
                using(var taProcessQuestion = new Datasets.ProcessesDatasetTableAdapters.ProcessQuestionTableAdapter())
                {
                    var questionName = taProcessQuestion.GetQuestionName(condition.ProcessQuestionId);

                    text = $"Show this step if '{condition.Value}' is " +
                        $"{EqualityToString(condition.Operator.ConvertToEnum<EqualityOperator>())} " +
                        $"the answer to Question '{questionName}'.";
                }
            }

            // Part Tag condition
            if (condition.InputType == nameof(ConditionInputType.PartTag))
            {
                text = $"Show this step if Part has tag '{condition.Value}'";
            }

            return text;
        }

        /// <summary>
        /// Returns a display string for the given
        /// <see cref="PartInspectionDataSet.PartInspectionQuestionConditionRow"/>
        /// instance.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string ConditionToString(PartInspectionDataSet.PartInspectionQuestionConditionRow condition)
        {
            var text = "None";

            if (condition == null)
            {
                return text;
            }

            // Question condition
            var checkQuestionRow = condition.CheckPartInspectionQuestionRow;
            if (checkQuestionRow != null)
            {
                var conditionOperator = EqualityToString(condition.Operator.ConvertToEnum<EqualityOperator>());

                text = $"Show this question if '{condition.Value}' is '{conditionOperator}' " +
                    $"'{checkQuestionRow.StepOrder}: {checkQuestionRow.Name}'";
            }

            return text;
        }

        /// <summary>
        /// Evaluates a condition.
        /// </summary>
        /// <param name="inputType">The data type to use for comparisons.</param>
        /// <param name="left">
        /// The value on the left-hand side of the condition.
        /// </param>
        /// <param name="equalityOperator">The equality operator.</param>
        /// <param name="right">
        /// The value on the right-hand side of the condition.
        /// </param>
        /// <returns>
        /// <c>true</c> if the condition is met; otherwise, <c>false</c>.
        /// </returns>
        public static bool MeetsCondition(InputType inputType, string left, EqualityOperator equalityOperator, string right)
        {
            switch (inputType)
            {
                case InputType.Date:
                case InputType.Time:
                case InputType.TimeIn:
                case InputType.TimeOut:
                case InputType.DateTimeIn:
                case InputType.DateTimeOut:
                case InputType.TimeDuration:
                case InputType.RampTime:
                    return MeetsDateTimeCondition(left, equalityOperator, right);
                case InputType.Decimal:
                case InputType.DecimalBefore:
                case InputType.DecimalAfter:
                case InputType.DecimalDifference:
                case InputType.Integer:
                case InputType.PartQty:
                case InputType.PreProcessWeight:
                case InputType.PostProcessWeight:
                    return MeetsNumberCondition(left, equalityOperator, right);
                case InputType.List:
                case InputType.String:
                    return MeetsStringCondition(left, equalityOperator, right);
            }

            return false;
        }

        private static bool MeetsNumberCondition(string left, EqualityOperator equalityOperator, string right)
        {
            double.TryParse(left, out double input);
            double.TryParse(right, out double output);

            switch (equalityOperator)
            {
                case EqualityOperator.Equal:
                    return input.CompareTo(output) == 0;
                case EqualityOperator.NotEqual:
                    return input.CompareTo(output) != 0;
                case EqualityOperator.LessThan:
                    return input.CompareTo(output) < 0;
                case EqualityOperator.GreaterThan:
                    return input.CompareTo(output) > 0;
            }

            return false;
        }

        private static bool MeetsStringCondition(string left, EqualityOperator equalityOperator, string right)
        {
            switch (equalityOperator)
            {
                case EqualityOperator.Equal:
                    return string.Compare(left, right, true) == 0;
                case EqualityOperator.NotEqual:
                    return string.Compare(left, right, true) != 0;
                case EqualityOperator.LessThan:
                    return string.Compare(left, right, true) < 0;
                case EqualityOperator.GreaterThan:
                    return string.Compare(left, right, true) > 0;
            }

            return false;
        }

        private static bool MeetsDateTimeCondition(string left, EqualityOperator equalityOperator, string right)
        {
            DateTime.TryParse(left, out DateTime leftDate);
            DateTime.TryParse(right, out DateTime rightDate);

            switch (equalityOperator)
            {
                case EqualityOperator.Equal:
                    return DateTime.Compare(leftDate, rightDate) == 0;
                case EqualityOperator.NotEqual:
                    return DateTime.Compare(leftDate, rightDate) != 0;
                case EqualityOperator.LessThan:
                    return DateTime.Compare(leftDate, rightDate) < 0;
                case EqualityOperator.GreaterThan:
                    return DateTime.Compare(leftDate, rightDate) > 0;
            }

            return false;
        }

        /// <summary>
        /// Evaluates a part tag (mark) condition.
        /// </summary>
        /// <param name="value">
        /// The value to check part tags against.
        /// </param>
        /// <param name="orderId">
        /// The ID of the order to check.
        /// </param>
        /// <returns>
        /// <c>true</c> if the condition is met; otherwise, <c>false</c>.
        /// </returns>
        public static bool MeetsPartTagConditional(string value, int orderId)
        {
            if (orderId <= 0)
            {
                return false;
            }

            // Check part mark lines for an exact value
            // Comparison does not skip blank lines because value can also be blank.
            var lines = new List<string>();

            using (var dtPartMarking = new OrderProcessingDataSet.OrderPartMarkDataTable())
            {
                using (var taPartMarking = new Datasets.OrderProcessingDataSetTableAdapters.OrderPartMarkTableAdapter())
                {
                    taPartMarking.Fill(dtPartMarking, orderId);

                    foreach (var orderPartMark in dtPartMarking)
                    {
                        lines.Add(orderPartMark.IsLine1Null() ? string.Empty : orderPartMark.Line1);
                        lines.Add(orderPartMark.IsLine2Null() ? string.Empty : orderPartMark.Line2);
                        lines.Add(orderPartMark.IsLine3Null() ? string.Empty : orderPartMark.Line3);
                        lines.Add(orderPartMark.IsLine4Null() ? string.Empty : orderPartMark.Line4);
                    }
                }
            }

            return lines.Contains(value);
        }
    }
}
