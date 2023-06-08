using System;
using System.Globalization;
using System.Linq;
using DWOS.Data.Datasets;
using NLog;

namespace DWOS.Data.Process
{
    /// <summary>
    /// Defines utility methods related to processing.
    /// </summary>
    /// <remarks>
    /// For order processing functionality, all methods assume that
    /// parameters have field-related children loaded.
    /// </remarks>
    public static class ProcessUtilities
    {
        #region Fields

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        /// <summary>
        /// Gets the default value for a process question.
        /// </summary>
        /// <param name="question"></param>
        /// <param name="order"></param>
        /// <returns>
        /// <see cref="String.Empty"/> if the question has a tolerance defined for it;
        /// otherwise, the default value for the question.
        /// </returns>
        public static string DefaultValue(OrderProcessingDataSet.ProcessQuestionRow question,
            OrderProcessingDataSet.OrderSummaryRow order)
        {
            var questionData = QuestionData.From(question, order);

            if (questionData == null)
            {
                return null;
            }

            return string.IsNullOrEmpty(questionData.Tolerance) ? questionData.DefaultValue : string.Empty;
        }

        /// <summary>
        /// Retrieves the minimum value from a question.
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public static string MinValue(OrderProcessingDataSet.ProcessQuestionRow question)
        {
            return MinValueString(QuestionData.From(question));
        }

        /// <summary>
        /// Retrieves the minimum value from a question.
        /// </summary>
        /// <param name="question"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static string MinValue(OrderProcessingDataSet.ProcessQuestionRow question,
            OrderProcessingDataSet.OrderSummaryRow order)
        {
            return MinValueString(QuestionData.From(question, order));
        }

        /// <summary>
        /// Retrieves the minimum value from a question.
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public static string MinValue(ProcessesDataset.ProcessQuestionRow question)
        {
            return MinValueString(QuestionData.From(question));
        }

        /// <summary>
        /// Retrieves the maximum value for a question.
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public static string MaxValue(OrderProcessingDataSet.ProcessQuestionRow question)
        {
            return MaxValueString(QuestionData.From(question));
        }

        /// <summary>
        /// Retrieves the maximum value for a question.
        /// </summary>
        /// <param name="question"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static string MaxValue(OrderProcessingDataSet.ProcessQuestionRow question,
            OrderProcessingDataSet.OrderSummaryRow order)
        {
            return MaxValueString(QuestionData.From(question, order));
        }

        /// <summary>
        /// Retrieves the maximum value for a question.
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public static string MaxValue(ProcessesDataset.ProcessQuestionRow question)
        {
            return MaxValueString(QuestionData.From(question));
        }

        /// <summary>
        /// Retrieves the minimum value for a question.
        /// </summary>
        /// <param name="question"></param>
        /// <returns>
        /// Minimum value parsed as a <see cref="decimal"/>.
        /// </returns>
        public static decimal? MinValueDecimal(OrderProcessingDataSet.ProcessQuestionRow question)
        {
            return MinValueDecimal(QuestionData.From(question));
        }

        /// <summary>
        /// Retrieves the minimum value for a question.
        /// </summary>
        /// <param name="question"></param>
        /// <param name="order"></param>
        /// <returns>
        /// Minimum value parsed as a <see cref="decimal"/>.
        /// </returns>
        public static decimal? MinValueDecimal(OrderProcessingDataSet.ProcessQuestionRow question,
            OrderProcessingDataSet.OrderSummaryRow order)
        {
            return MinValueDecimal(QuestionData.From(question, order));
        }

        /// <summary>
        /// Retrieves the minimum value for a question.
        /// </summary>
        /// <param name="question"></param>
        /// <returns>
        /// Minimum value parsed as a <see cref="decimal"/>.
        /// </returns>
        public static decimal? MinValueDecimal(ProcessesDataset.ProcessQuestionRow question)
        {
            return MinValueDecimal(QuestionData.From(question));
        }

        /// <summary>
        /// Retrieves the maximum value for a question.
        /// </summary>
        /// <param name="question"></param>
        /// <returns>
        /// Maximum value parsed as a <see cref="decimal"/>.
        /// </returns>
        public static decimal? MaxValueDecimal(OrderProcessingDataSet.ProcessQuestionRow question)
        {
            return MaxValueDecimal(QuestionData.From(question));
        }

        /// <summary>
        /// Retrieves the maximum value for a question.
        /// </summary>
        /// <param name="question"></param>
        /// <param name="order"></param>
        /// <returns>
        /// Maximum value parsed as a <see cref="decimal"/>.
        /// </returns>
        public static decimal? MaxValueDecimal(OrderProcessingDataSet.ProcessQuestionRow question,
            OrderProcessingDataSet.OrderSummaryRow order)
        {
            return MaxValueDecimal(QuestionData.From(question, order));
        }

        /// <summary>
        /// Retrieves the maximum value for a question.
        /// </summary>
        /// <param name="question"></param>
        /// <returns>
        /// Maximum value parsed as a <see cref="decimal"/>.
        /// </returns>
        public static decimal? MaxValueDecimal(ProcessesDataset.ProcessQuestionRow question)
        {
            return MaxValueDecimal(QuestionData.From(question));
        }

        /// <summary>
        /// Retrieves the minimum value for a question.
        /// </summary>
        /// <param name="question"></param>
        /// <returns>
        /// Minimum value parsed as a <see cref="int"/>.
        /// </returns>
        public static int? MinValueInt32(OrderProcessingDataSet.ProcessQuestionRow question)
        {
            var minValueDec = MinValueDecimal(QuestionData.From(question));
            return ConvertToNullableInt32(minValueDec);
        }

        /// <summary>
        /// Retrieves the minimum value for a question.
        /// </summary>
        /// <param name="question"></param>
        /// <param name="order"></param>
        /// <returns>
        /// Minimum value parsed as a <see cref="int"/>.
        /// </returns>
        public static int? MinValueInt32(OrderProcessingDataSet.ProcessQuestionRow question,
            OrderProcessingDataSet.OrderSummaryRow order)
        {
            var minValueDec = MinValueDecimal(QuestionData.From(question, order));
            return ConvertToNullableInt32(minValueDec);
        }

        /// <summary>
        /// Retrieves the minimum value for a question.
        /// </summary>
        /// <param name="question"></param>
        /// <returns>
        /// Minimum value parsed as a <see cref="int"/>.
        /// </returns>
        public static int? MinValueInt32(ProcessesDataset.ProcessQuestionRow question)
        {
            var minValueDec = MinValueDecimal(QuestionData.From(question));
            return ConvertToNullableInt32(minValueDec);
        }

        /// <summary>
        /// Retrieves the maximum value for a question.
        /// </summary>
        /// <param name="question"></param>
        /// <returns>
        /// Maximum value parsed as a <see cref="int"/>.
        /// </returns>
        public static int? MaxValueInt32(OrderProcessingDataSet.ProcessQuestionRow question)
        {
            var maxValueDec = MaxValueDecimal(QuestionData.From(question));
            return ConvertToNullableInt32(maxValueDec);
        }

        /// <summary>
        /// Retrieves the maximum value for a question.
        /// </summary>
        /// <param name="question"></param>
        /// <param name="order"></param>
        /// <returns>
        /// Maximum value parsed as a <see cref="int"/>.
        /// </returns>
        public static int? MaxValueInt32(OrderProcessingDataSet.ProcessQuestionRow question,
            OrderProcessingDataSet.OrderSummaryRow order)
        {
            var maxValueDec = MaxValueDecimal(QuestionData.From(question, order));
            return ConvertToNullableInt32(maxValueDec);
        }

        /// <summary>
        /// Retrieves the maximum value for a question.
        /// </summary>
        /// <param name="question"></param>
        /// <returns>
        /// Maximum value parsed as a <see cref="int"/>.
        /// </returns>
        public static int? MaxValueInt32(ProcessesDataset.ProcessQuestionRow question)
        {
            var maxValueDec = MaxValueDecimal(QuestionData.From(question));
            return ConvertToNullableInt32(maxValueDec);
        }

        /// <summary>
        /// Ensure that all expected <see cref="QuestionField.AnswerOut"/>
        /// fields exist on <paramref name="order"/>.
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="order"></param>
        /// <param name="dsOrderProcessing"></param>
        public static void EnsureFieldsExist(OrderProcessingDataSet.OrderProcessAnswerRow answer,
            OrderProcessingDataSet.OrderSummaryRow order, OrderProcessingDataSet dsOrderProcessing)
        {
            if (answer == null || order == null || answer.ProcessQuestionRow == null || order.IsCustomerIDNull() ||
                dsOrderProcessing == null)
            {
                return;
            }

            var orderCustomFields = order.GetOrderCustomFieldsRows();

            var answerOutFields = answer.ProcessQuestionRow.GetProcessQuestionFieldRows()
                .Where(f => f.IsValidState() && f.FieldName == QuestionField.AnswerOut.ToString());

            foreach (var answerOutField in answerOutFields)
            {
                var outOrderField = orderCustomFields
                    .FirstOrDefault(f => f.IsValidState() && TokenName(f) == answerOutField.TokenName);

                if (outOrderField != null)
                {
                    // Field exists
                    continue;
                }

                var customField = dsOrderProcessing.CustomField
                    .FirstOrDefault(
                        i =>
                            i.IsValidState() && !i.IsTokenNameNull() && i.CustomerID == order.CustomerID &&
                            i.TokenName == answerOutField.TokenName);

                if (customField != null)
                {
                    Logger.Info("Creating new order custom field row for custom field {0}", answerOutField.TokenName);

                    var newOrderField = dsOrderProcessing.OrderCustomFields.NewOrderCustomFieldsRow();
                    newOrderField.OrderSummaryRow = order;
                    newOrderField.CustomFieldRow = customField;
                    dsOrderProcessing.OrderCustomFields.AddOrderCustomFieldsRow(newOrderField);
                }
            }
        }

        /// <summary>
        /// Saves an answer's value to a custom field if the question is
        /// setup to do so.
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="order"></param>
        public static void SetFieldValue(OrderProcessingDataSet.OrderProcessAnswerRow answer, OrderProcessingDataSet.OrderSummaryRow order)
        {
            if (answer == null || order == null || answer.ProcessQuestionRow == null)
            {
                return;
            }

            var processFields = answer.ProcessQuestionRow.GetProcessQuestionFieldRows();
            var customFields = order.GetOrderCustomFieldsRows();

            var answerOutFields = processFields
                .Where(f => f.IsValidState() && f.FieldName == QuestionField.AnswerOut.ToString());

            foreach (var answerOutField in answerOutFields)
            {
                var customField = customFields
                    .FirstOrDefault(f => f.IsValidState() && TokenName(f) == answerOutField.TokenName);

                if (customField != null)
                {
                    Logger.Info("Setting value for custom field {0}", answerOutField.TokenName);
                    customField.Value = answer.Answer;
                }
            }
        }

        /// <summary>
        /// Clears any field that the question is setup to sync to.
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="order"></param>
        public static void ClearFieldValue(OrderProcessingDataSet.OrderProcessAnswerRow answer, OrderProcessingDataSet.OrderSummaryRow order)
        {
            if (answer == null || order == null || answer.ProcessQuestionRow == null)
            {
                return;
            }

            var processFields = answer.ProcessQuestionRow.GetProcessQuestionFieldRows();
            var customFields = order.GetOrderCustomFieldsRows();

            var answerOutFields = processFields
                .Where(f => f.IsValidState() && f.FieldName == QuestionField.AnswerOut.ToString());

            foreach (var answerOutField in answerOutFields)
            {
                Logger.Info("Clearing value for custom field {0}", answerOutField.TokenName);

                var customField = customFields
                    .FirstOrDefault(f => f.IsValidState() && TokenName(f) == answerOutField.TokenName);

                customField?.SetValueNull();
            }
        }

        private static string GetDefaultValue(OrderProcessingDataSet.ProcessQuestionRow question,
            OrderProcessingDataSet.OrderSummaryRow order)
        {
            return GetValue(question, order, QuestionField.Answer);
        }

        private static string GetMinValue(OrderProcessingDataSet.ProcessQuestionRow question,
            OrderProcessingDataSet.OrderSummaryRow order)
        {
            return GetValue(question, order, QuestionField.MinValue);
        }

        private static string GetMaxValue(OrderProcessingDataSet.ProcessQuestionRow question,
            OrderProcessingDataSet.OrderSummaryRow order)
        {
            return GetValue(question, order, QuestionField.MaxValue);
        }

        private static string GetTolerance(OrderProcessingDataSet.ProcessQuestionRow question,
            OrderProcessingDataSet.OrderSummaryRow order)
        {
            return GetValue(question, order, QuestionField.Tolerance);
        }

        private static string GetValue(OrderProcessingDataSet.ProcessQuestionRow question,
            OrderProcessingDataSet.OrderSummaryRow order, QuestionField field)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }

            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var fieldString = field.ToString();

            var questionField = question.GetProcessQuestionFieldRows()
                .FirstOrDefault(i => i.IsValidState() && i.FieldName == fieldString);

            if (questionField != null)
            {
                var customField = order
                    .GetOrderCustomFieldsRows()
                    .FirstOrDefault(f => f.IsValidState() && TokenName(f) == questionField.TokenName);

                var hasValue = customField != null && !customField.IsValueNull() &&
                               !string.IsNullOrEmpty(customField.Value);

                var isProcessUnique = customField?.CustomFieldRow?.ProcessUnique ?? false;

                if (hasValue && isProcessUnique)
                {
                    return customField.Value;
                }
            }

            switch(field)
            {
                case QuestionField.Answer:
                    return question.IsDefaultValueNull() ? null : question.DefaultValue;
                case QuestionField.MinValue:
                    return question.IsMinValueNull() ? null : question.MinValue;
                case QuestionField.MaxValue:
                    return question.IsMaxValueNull() ? null : question.MaxValue;
                case QuestionField.Tolerance:
                    return question.IsToleranceNull() ? null : question.Tolerance;
                default:
                    return null;
            }
        }

        private static string TokenName(OrderProcessingDataSet.OrderCustomFieldsRow orderCustomFieldsRow)
        {
            if (orderCustomFieldsRow == null || !orderCustomFieldsRow.IsValidState())
            {
                return null;
            }

            var customFieldRow = orderCustomFieldsRow.CustomFieldRow;

            if (customFieldRow == null || !customFieldRow.IsValidState() || customFieldRow.IsTokenNameNull())
            {
                return null;
            }

            return customFieldRow.TokenName;
        }

        private static string MinValueString(QuestionData question)
        {
            if (question == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(question.DefaultValue) && !string.IsNullOrEmpty(question.Tolerance))
            {
                decimal defaultValue;
                decimal tolerance;

                if (decimal.TryParse(question.DefaultValue, out defaultValue) &&
                    decimal.TryParse(question.Tolerance, out tolerance))
                {
                    return (defaultValue - tolerance).ToString(CultureInfo.InvariantCulture);
                }
            }

            return question.MinValue;
        }

        private static string MaxValueString(QuestionData question)
        {
            if (question == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(question.DefaultValue) && !string.IsNullOrEmpty(question.Tolerance))
            {
                decimal defaultValue;
                decimal tolerance;

                if (decimal.TryParse(question.DefaultValue, out defaultValue) &&
                    decimal.TryParse(question.Tolerance, out tolerance))
                {
                    return (defaultValue + tolerance).ToString(CultureInfo.InvariantCulture);
                }
            }

            return question.MaxValue;
        }

        private static decimal? MinValueDecimal(QuestionData question)
        {
            if (question == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(question.DefaultValue) && !string.IsNullOrEmpty(question.Tolerance))
            {
                decimal defaultValue;
                decimal tolerance;

                if (decimal.TryParse(question.DefaultValue, out defaultValue) &&
                    decimal.TryParse(question.Tolerance, out tolerance))
                {
                    return defaultValue - tolerance;
                }
            }

            decimal minValue;

            if (decimal.TryParse(question.MinValue, out minValue))
            {
                return minValue;
            }

            return null;
        }

        private static decimal? MaxValueDecimal(QuestionData question)
        {
            if (question == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(question.DefaultValue) && !string.IsNullOrEmpty(question.Tolerance))
            {
                decimal defaultValue;
                decimal tolerance;

                if (decimal.TryParse(question.DefaultValue, out defaultValue) &&
                    decimal.TryParse(question.Tolerance, out tolerance))
                {
                    return defaultValue + tolerance;
                }
            }

            decimal maxValue;

            if (decimal.TryParse(question.MaxValue, out maxValue))
            {
                return maxValue;
            }

            return null;
        }

        private static int? ConvertToNullableInt32(decimal? value)
        {
            if (value.HasValue)
            {
                return Convert.ToInt32(value.Value);
            }

            return null;
        }

        #endregion

        #region QuestionData

        private class QuestionData
        {
            #region Properties

            public string DefaultValue { get; private set; }

            public string Tolerance { get; private set; }

            public string MinValue { get; private set; }

            public string MaxValue { get; private set; }

            #endregion

            #region Methods

            public static QuestionData From(OrderProcessingDataSet.ProcessQuestionRow question)
            {
                if (question == null)
                {
                    return null;
                }

                return new QuestionData
                {
                    DefaultValue = question.IsDefaultValueNull() ? null : question.DefaultValue,
                    Tolerance = question.IsToleranceNull() ? null : question.Tolerance,
                    MinValue = question.IsMinValueNull() ? null : question.MinValue,
                    MaxValue = question.IsMaxValueNull() ? null : question.MaxValue
                };
            }

            public static QuestionData From(OrderProcessingDataSet.ProcessQuestionRow question, OrderProcessingDataSet.OrderSummaryRow order)
            {
                if (question == null)
                {
                    return null;
                }

                if (order == null)
                {
                    LogManager.GetCurrentClassLogger().Warn("Order was not loaded");

                    return new QuestionData
                    {
                        DefaultValue = question.IsDefaultValueNull() ? null : question.DefaultValue,
                        Tolerance = question.IsToleranceNull() ? null : question.Tolerance,
                        MinValue = question.IsMinValueNull() ? null : question.MinValue,
                        MaxValue = question.IsMaxValueNull() ? null : question.MaxValue
                    };
                }

                return new QuestionData
                {
                    DefaultValue = GetDefaultValue(question, order),
                    Tolerance = GetTolerance(question, order),
                    MinValue = GetMinValue(question, order),
                    MaxValue = GetMaxValue(question, order)
                };
            }

            public static QuestionData From(ProcessesDataset.ProcessQuestionRow question)
            {
                if (question == null)
                {
                    return null;
                }

                return new QuestionData
                {
                    DefaultValue = question.IsDefaultValueNull() ? null : question.DefaultValue,
                    Tolerance = question.IsToleranceNull() ? null : question.Tolerance,
                    MinValue = question.IsMinValueNull() ? null : question.MinValue,
                    MaxValue = question.IsMaxValueNull() ? null : question.MaxValue
                };
            }

            #endregion
        }

        #endregion
    }
}