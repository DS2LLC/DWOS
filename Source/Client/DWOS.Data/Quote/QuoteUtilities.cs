using DWOS.Data.Datasets;
using System;

namespace DWOS.Data.Quote
{
    /// <summary>
    /// Contains utility methods related to quotes.
    /// </summary>
    public static class QuoteUtilities
    {
        /// <summary>
        /// Gets a display string for the quote using a formatted string.
        /// </summary>
        /// <param name="quote"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetDisplayString(QuoteDataSet.QuoteRow quote, string format)
        {
            if (quote == null)
            {
                throw new ArgumentNullException(nameof(quote));
            }

            if (string.IsNullOrEmpty(format))
            {
                return quote.QuoteID.ToString();
            }

            var custName = quote.CustomerRow?.Name ?? string.Empty;

            var quoteId = quote.QuoteID;

            return ApplyFormat(format, quoteId, custName);
        }

        private static string ApplyFormat(string format, int id, string custName)
        {
            var adjustedFormat = format;

            if (format.Contains("<REQUIREDDATE>") || format.Contains("%REQUIREDDATE%"))
            {
                // Remove RequiredDate tokens with space at the end.
                // Otherwise, it makes output look incorrect.
                adjustedFormat = format
                    .Replace("<REQUIREDDATE> ", string.Empty)
                    .Replace("%REQUIREDDATE% ", string.Empty);
            }

            // Format string with '%' is preferred, but the format used angle brackets in 16.3.0.
            // Quotes do not have a required date.
            var formattedString = adjustedFormat
                .Replace("%ID%", id.ToString())
                .Replace("%REQUIREDDATE%", string.Empty)
                .Replace("%CUSTOMERNAME%", custName)
                .Replace("<ID>", id.ToString())
                .Replace("<REQUIREDDATE>", string.Empty)
                .Replace("<CUSTOMERNAME>", custName)
                .TrimEnd();

            var includesId = format.Contains("<ID>") || format.Contains("%ID%");

            return includesId ? formattedString : $"{id} {formattedString}";
        }
    }
}
