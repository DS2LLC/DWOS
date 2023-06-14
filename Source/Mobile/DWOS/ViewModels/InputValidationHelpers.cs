using System;

namespace DWOS.ViewModels
{
    /// <summary>
    /// Defines validation-related utility methods.
    /// </summary>
    internal sealed class InputValidationHelpers
    {
        /// <summary>
        /// Validates the answer.
        /// </summary>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="listID">The list identifier.</param>
        /// <param name="answer">The answer.</param>
        /// <returns></returns>
        internal static bool ValidateAnswer(InputTypes inputType, string minValue, string maxValue, int listID, string answer)
        {
            if (string.IsNullOrEmpty(answer))
            {
                return false;
            }

            switch (inputType)
            {
                case InputTypes.Decimal:
                case InputTypes.DecimalBefore:
                case InputTypes.DecimalAfter:
                case InputTypes.DecimalDifference:
                case InputTypes.PreProcessWeight:
                case InputTypes.PostProcessWeight:
                    Decimal decMaxValue, decMinValue;
                    Decimal decValue;
                    //ensure min value is set
                    if (String.IsNullOrWhiteSpace(minValue))
                        minValue = "0.0";
                    if (String.IsNullOrWhiteSpace(maxValue))
                        maxValue = "99999.0";
                    return Decimal.TryParse(answer, out decValue) && Decimal.TryParse(maxValue, out decMaxValue) && Decimal.TryParse(minValue, out decMinValue) && decValue <= decMaxValue && decValue >= decMinValue;
                case InputTypes.PartQty:
                    int partQty = 0;
                    return int.TryParse(answer, out partQty);
                case InputTypes.Integer:
                case InputTypes.TimeDuration:
                case InputTypes.RampTime:
                    Int32 intMaxValue, intMinValue;
                    Int32 intValue;
                    //ensure min value is set
                    if (String.IsNullOrWhiteSpace(minValue))
                        minValue = "0";
                    if (String.IsNullOrWhiteSpace(maxValue))
                        maxValue = "999";
                    return Int32.TryParse(answer, out intValue) && Int32.TryParse(maxValue, out intMaxValue) && Int32.TryParse(minValue, out intMinValue) && intValue <= intMaxValue && intValue >= intMinValue;
                case InputTypes.Date:
                case InputTypes.Time:
                case InputTypes.TimeIn:
                case InputTypes.TimeOut:
                case InputTypes.DateTimeIn:
                case InputTypes.DateTimeOut:
                    DateTime dt;
                    return DateTime.TryParse(answer, out dt);
                case InputTypes.None:
                case InputTypes.String:
                case InputTypes.List:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets text to use for an invalid answer.
        /// </summary>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="numericUnits">The numeric units.</param>
        /// <returns></returns>
        internal static string GetInvalidAnswerText(InputTypes inputType, string minValue, string maxValue, string numericUnits)
        {
            switch (inputType)
            {
                case InputTypes.PartQty:
                    return "Set the correct quantity of parts.";
                case InputTypes.Decimal:
                case InputTypes.DecimalBefore:
                case InputTypes.DecimalAfter:
                case InputTypes.DecimalDifference:
                case InputTypes.PreProcessWeight:
                case InputTypes.PostProcessWeight:
                case InputTypes.Integer:
                case InputTypes.TimeDuration:
                case InputTypes.RampTime:
                    return String.Format("Set the [{0}] value between {1} and {2}", numericUnits, minValue, maxValue);
                case InputTypes.List:
                    return "Select correct value from the list.";
                case InputTypes.Date:
                    return "Set the correct date.";
                case InputTypes.Time:
                    return "Set the correct time.";
                case InputTypes.TimeIn:
                case InputTypes.DateTimeIn:
                    return "Set the correct start time.";
                case InputTypes.TimeOut:
                case InputTypes.DateTimeOut:
                    return "Set the correct end time.";
                case InputTypes.None:
                case InputTypes.String:
                default:
                    return "Set the text as required.";
            }
        }

        /// <summary>
        /// Checks if an answer is considered to be blank.
        /// </summary>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="answer">The answer.</param>
        /// <returns>
        /// <c>true</c>if the answer is blank; otherwise, <c>false</c>
        /// </returns>
        internal static bool IsAnswerBlank(InputTypes inputType, string answer)
        {
            switch (inputType)
            {
                case InputTypes.Decimal:
                case InputTypes.DecimalBefore:
                case InputTypes.DecimalAfter:
                case InputTypes.DecimalDifference:
                case InputTypes.PreProcessWeight:
                case InputTypes.PostProcessWeight:
                    decimal decimalResult;
                    decimal.TryParse(answer, out decimalResult);
                    return decimalResult == 0M;
                case InputTypes.PartQty:
                case InputTypes.Integer:
                case InputTypes.RampTime:
                case InputTypes.TimeDuration:
                    int integerResult;
                    int.TryParse(answer, out integerResult);
                    return integerResult == 0;
                case InputTypes.List:
                case InputTypes.None:
                case InputTypes.String:
                case InputTypes.Date:
                case InputTypes.Time:
                case InputTypes.TimeIn:
                case InputTypes.TimeOut:
                case InputTypes.DateTimeIn:
                case InputTypes.DateTimeOut:
                    return string.IsNullOrEmpty(answer);
                default:
                    return false;
            }
        }
    }
}
