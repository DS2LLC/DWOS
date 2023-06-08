using System;

namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Implements some <see cref="Math"/> methods for use with nullable
    /// values.
    /// </summary>
    public static class NullableMath
    {
        /// <summary>
        /// Rounds a nullable decimal value to a specified number of fractional
        /// digits.
        /// </summary>
        /// <param name="d">The nullable decimal number to be rounded.</param>
        /// <param name="decimals">
        /// The number of decimal places in the return value.
        /// </param>
        /// <returns>
        /// The number nearest to d that contains a number of fractional
        /// digits equal to decimals.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// decimals is less than 0 or greater than 28. 
        /// </exception>
        /// <exception cref="OverflowException">
        /// The result is outside the range of a Decimal.
        /// </exception>
        public static decimal? Round(decimal? d, int decimals)
        {
            if (!d.HasValue)
            {
                return null;
            }

            return Math.Round(d.Value, decimals);
        }
    }
}
