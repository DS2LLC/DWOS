using System;

namespace DWOS.ViewModels
{
    static class ConditionHelpers
    {
        public static bool MeetsCondition(InputTypes inputType, string left, EqualityOperator equalityOperator, string right)
        {
            switch (inputType)
            {
                case InputTypes.Date:
                case InputTypes.Time:
                case InputTypes.TimeIn:
                case InputTypes.TimeOut:
                case InputTypes.TimeDuration:
                case InputTypes.RampTime:
                case InputTypes.DateTimeIn:
                case InputTypes.DateTimeOut:
                    return MeetsDateTimeCondition(left, equalityOperator, right);
                case InputTypes.Decimal:
                case InputTypes.DecimalBefore:
                case InputTypes.DecimalAfter:
                case InputTypes.DecimalDifference:
                case InputTypes.Integer:
                case InputTypes.PartQty:
                case InputTypes.PreProcessWeight:
                case InputTypes.PostProcessWeight:
                    return MeetsNumberCondition(left, equalityOperator, right);
                case InputTypes.List:
                case InputTypes.String:
                    return MeetsStringCondition(left, equalityOperator, right);
            }

            return false;
        }

        private static bool MeetsNumberCondition(string left, EqualityOperator equalityOperator, string right)
        {
            double input;
            double output;

            double.TryParse(left, out input);
            double.TryParse(right, out output);

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
                    return string.Compare(left, right, StringComparison.CurrentCultureIgnoreCase) == 0;
                case EqualityOperator.NotEqual:
                    return string.Compare(left, right, StringComparison.CurrentCultureIgnoreCase) != 0;
                case EqualityOperator.LessThan:
                    return string.Compare(left, right, StringComparison.CurrentCultureIgnoreCase) < 0;
                case EqualityOperator.GreaterThan:
                    return string.Compare(left, right, StringComparison.CurrentCultureIgnoreCase) > 0;
            }

            return false;
        }

        private static bool MeetsDateTimeCondition(string left, EqualityOperator equalityOperator, string right)
        {
            DateTime leftDate;
            DateTime rightDate;

            DateTime.TryParse(left, out leftDate);
            DateTime.TryParse(right, out rightDate);

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
    }
}
