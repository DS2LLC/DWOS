using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Defines extension methods for <see cref="string"/> instances.
    /// </summary>
    /// <remarks>
    /// This differs from <see cref="StringExtensions"/> because these
    /// extensions methods are internally-developed.
    /// </remarks>
    public static class DwosStringExtensions
    {
        private const string UNKNOWN_INITIALS = "?";

        /// <summary>
        /// Returns the initials of a name.
        /// </summary>
        /// <param name="name">Name to extract initials from</param>
        /// <param name="option">Option for generating initials</param>
        /// <returns>String representation of the name's initials.</returns>
        public static string ToInitials(this string name, StringInitialOption option)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            else if (string.IsNullOrWhiteSpace(name))
            {
                return name.Trim();
            }

            var partsOfName = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            switch (option)
            {
                case StringInitialOption.FirstTwoInitials:
                    return FirstTwoInitials(partsOfName).ToUpper();
                case StringInitialOption.AllInitials:
                    return AllInitials(partsOfName).ToUpper();
                default:
                    return null;
            }
        }

        private static string FirstTwoInitials(string[] partsOfName)
        {
            if (partsOfName == null)
            {
                throw new ArgumentNullException(nameof(partsOfName));
            }

            string returnString;
            if (partsOfName.Length > 1)
            {
                var firstName = partsOfName[0];
                var secondName = partsOfName[1];

                returnString = firstName.First().ToString() + secondName.First().ToString();
            }
            else if (partsOfName.Length == 1)
            {
                var firstName = partsOfName[0];
                returnString = firstName.First().ToString();
            }
            else
            {
                returnString = UNKNOWN_INITIALS;
            }

            return returnString;
        }

        private static string AllInitials(string[] partsOfName)
        {
            if (partsOfName == null)
            {
                throw new ArgumentNullException(nameof(partsOfName));
            }
            else if (partsOfName.Length == 0)
            {
                return UNKNOWN_INITIALS;
            }

            return string.Join(string.Empty,
                partsOfName.SelectMany(i => i.FirstOrDefault().ToString()));
        }
    }
}
