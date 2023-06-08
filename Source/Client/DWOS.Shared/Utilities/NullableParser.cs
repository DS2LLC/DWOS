namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Parses strings to nullable value types.
    /// </summary>
    public static class NullableParser
    {
        /// <summary>
        /// Converts the string representation of a number to its nullable integer equivalent.
        /// </summary>
        /// <param name="s">A string containing a number to convert.</param>
        /// <param name="result">
        /// When this method returns, contains an integer value equivalent to
        /// <code>s</code> if conversion succeeded. Otherwise, contains null.
        /// </param>
        /// <returns>true if s was converted successfully; otherwise, false</returns>
        public static bool TryParse(string s, out int? result)
        {
            int nonNullableResult;

            if (int.TryParse(s, out nonNullableResult))
            {
                result = nonNullableResult;
            }
            else
            {
                result = null;
            }

            return result.HasValue;
        }

        /// <summary>
        /// Converts the string representation of a number to its nullable decimal equivalent.
        /// </summary>
        /// <param name="s">A string containing a number to convert.</param>
        /// <param name="result">
        /// When this method returns, contains a decimal value equivalent to
        /// <code>s</code> if conversion succeeded. Otherwise, contains null.
        /// </param>
        /// <returns>true if s was converted successfully; otherwise, false</returns>
        public static bool TryParse(string s, out decimal? result)
        {
            decimal nonNullableResult;

            if (decimal.TryParse(s, out nonNullableResult))
            {
                result = nonNullableResult;
            }
            else
            {
                result = null;
            }

            return result.HasValue;
        }
    }
}
