using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.Shared.Utilities
{
    public static class StringUtilities
    {
        #region Fields

        private static Dictionary<string, string> PLURALIZATION_LOOKUP = CreatePluralizationLookup();

        #endregion

        #region Methods

        /// <summary>
        /// Joins strings using commas and the word "and".
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static string ToDisplayText(IEnumerable<string> strings)
        {
            if (strings == null)
            {
                throw new ArgumentNullException(nameof(strings));
            }

            return ToDisplayText(new List<string>(strings));
        }

        /// <summary>
        /// Joins strings using commas and the word "and".
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static string ToDisplayText(List<string> strings)
        {
            if (strings == null)
            {
                throw new ArgumentNullException(nameof(strings));
            }

            if (strings.Count < 2)
            {
                return strings.FirstOrDefault() ?? string.Empty;
            }

            if (strings.Count == 2)
            {
                return $"{strings[0]} and {strings[1]}";
            }

            var lastIndex = strings.Count - 1;
            return $"{string.Join(", ", strings.ToArray(), 0, strings.Count - 1)}, and {strings[lastIndex]}";
        }

        /// <summary>
        /// Quickly pluralizes words that commonly need to be pluralized
        /// in DWOS.
        /// </summary>
        /// <param name="singular"></param>
        /// <returns></returns>
        public static string ToPlural(string singular)
        {
            if (string.IsNullOrEmpty(singular))
            {
                return string.Empty;
            }

            if (PLURALIZATION_LOOKUP.ContainsKey(singular))
            {
                return PLURALIZATION_LOOKUP[singular];
            }

            return singular + "s";
        }

        private static Dictionary<string, string> CreatePluralizationLookup() => new Dictionary<string, string>
        {
            { "box", "boxes" },
            { "Box", "Boxes" },
            { "package", "packages" },
            { "Package", "Packages" }
        };

        #endregion
    }
}
