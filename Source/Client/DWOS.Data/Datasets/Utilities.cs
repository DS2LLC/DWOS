using System.Data;
using System;
using System.Text.RegularExpressions;

namespace DWOS.Data.Datasets
{
    /// <summary>
    /// Defines general utility methods.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Converts a string to an enumerated value of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static T ConvertToEnum<T>(this string enumValue)
        {
            if(String.IsNullOrEmpty(enumValue))
                enumValue = Enum.GetNames(typeof(T))[0];
            
            return (T)Enum.Parse(typeof(T), enumValue);
        }

        /// <summary>
        /// Escape <paramref name="value"/> for use in SQL queries.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SqlBless(string value)
        {
            const string REPLACEWITH = "*";

            if(!String.IsNullOrEmpty(value))
            {
                value = value.ReplaceWith(@"USER_NAME\(\)", string.Empty, RegexOptions.IgnoreCase)
                    .Replace("'", REPLACEWITH)
                    .Replace("--", REPLACEWITH) //Remove sql comments
                    .Replace(";", REPLACEWITH); //Remove multiple queries
            }


            return value;
        }

        /// <summary>
        /// Escapes a string for use in the pattern part of a LIKE operation.
        /// </summary>
        /// <remarks>
        /// Does not escape all SQL-unfriendly input; use in a parameter or
        /// use <see cref="SqlBless"/>.
        /// </remarks>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string PrepareForLike(string input)
        {
            const string charactersToEscape = @"\[\]*%";

            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input
                .ReplaceWith(@"([" + charactersToEscape + "])", @"[${0}]");
        }

        /// <summary>
        /// Is <paramref name="row"/> in a valid state?
        /// </summary>
        /// <param name="row"></param>
        /// <returns>
        /// <c>true</c> if the row has not been deleted or detached;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidState(this DataRow row)
        {
            return row != null && row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached;
        }
    }
}
