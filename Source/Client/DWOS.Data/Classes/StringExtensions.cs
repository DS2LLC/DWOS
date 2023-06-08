using System;
using System.Linq;
using System.Text.RegularExpressions;


namespace DWOS.Data
{
    /// <summary>
    /// Defines extension methods for <see cref="string"/> instances.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Regex pattern to use for checking email address.
        /// </summary>
        public const string EMAIL_REGEX = @"[a-z0-9!#$%&'*+/=?^_‘{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_‘{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

        /// <summary>
        /// Determines if the string value contains a valid email address.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if value is a valid email address; otherwise, false</returns>
        public static bool IsValidEmail(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "value cannot be null");
            }
            
            return Regex.IsMatch(value, EMAIL_REGEX);
        }

        /// <summary>
        /// Formats the given string for use in part marking.
        /// </summary>
        /// <param name="value">The string to format.</param>
        /// <returns>A formatted representation of the input value.</returns>
        public static string ToPartMarkingString(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return value.Remove('<', '>', ' ');
        }

        /// <summary>
        /// Increments a <see cref="string"/> instance.
        /// </summary>
        /// <param name="input">Input to increment.</param>
        /// <returns></returns>
        public static string Increment(this string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var trimmedInput = input.TrimEnd();

            if (string.IsNullOrEmpty(trimmedInput))
            {
                return trimmedInput;
            }

            // Basic case - ends with digit
            var digitRegex = new Regex(@"\d+$");
            var digitMatch = digitRegex.Match(trimmedInput);

            if (digitMatch.Success)
            {
                if (long.TryParse(digitMatch.Value, out var originalNumber))
                {
                    if (originalNumber == long.MaxValue)
                    {
                        // Would overflow - just return the current serial number
                        return trimmedInput;
                    }

                    var replacement = (originalNumber + 1).ToString().PadLeft(digitMatch.Length, '0');
                    return digitRegex.Replace(trimmedInput, replacement);
                }
            }

            // Ends with letter
            var regex = new Regex(@"0*([a-zA-Z]+$)");
            var match = regex.Match(trimmedInput);
            if (match.Success)
            {
                var replacement =  IncrementCharacters(match.Groups[1].Value).PadLeft(match.Length, '0');
                return regex.Replace(trimmedInput, replacement);
            }

            return trimmedInput;
        }

        private static string IncrementCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var lastCharacter = input.Last();

            if ((lastCharacter >= 'a' && lastCharacter < 'z') || (lastCharacter >= 'A' && lastCharacter < 'Z'))
            {
                lastCharacter += (char)1;
                return input.Substring(0, input.Length - 1) + lastCharacter;
            }

            if (lastCharacter == 'z' || lastCharacter == 'Z')
            {
                var nextSection = input.Substring(0, input.Length - 1);

                if (string.IsNullOrEmpty(nextSection))
                {
                    return ((char)(lastCharacter - 25)).ToString() + (char)(lastCharacter - 25);
                }

                var incrementedSection = IncrementCharacters(nextSection);

                if (incrementedSection != nextSection)
                {
                    return incrementedSection + (char)(lastCharacter - 25);
                }
            }

            return input;
        }
    }
}
