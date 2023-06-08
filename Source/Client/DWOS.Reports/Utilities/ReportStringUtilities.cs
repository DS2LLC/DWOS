using AngleSharp.Parser.Html;
using System;

namespace DWOS.Reports.Utilities
{
    public static class ReportStringUtilities
    {
        /// <summary>
        /// Strips HTML, Infragistics-specific formatting, and surrounding
        /// whitespace from an input string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripHtml(this string input)
        {
            const string infragisticsSpace = "&edsp;";

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            var cleanedInput = input
                .Trim()
                .Replace(infragisticsSpace, " ");

            var parser = new HtmlParser();
            var doc = parser.Parse(cleanedInput);
            return doc.DocumentElement.TextContent;
        }
    }
}
