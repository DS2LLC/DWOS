using System.Collections.Generic;

namespace DWOS.UI
{
    /// <summary>
    /// Represents accepted/rejected/total count data.
    /// </summary>
    internal sealed class CocCountData
    {
        #region Properties

        public long AcceptedCount { get; set; }

        public long RejectedCount { get; set; }

        public long TotalCount { get; set; }

        public List<string> SerialNumbers { get; set; }

        public string ImportValue { get; set; }

        public string AcceptedHtml =>
            $"&nbsp;Accepted:&nbsp;{AcceptedCount}";

        public string RejectedHtml =>
            $"&nbsp;Rejected:&nbsp;{RejectedCount}";

        public string TotalHtml =>
            $"&nbsp;Total:&nbsp;<B>{TotalCount}</B>";

        public string SerialNumbersHtml
        {
            get
            {
                var prefix = SerialNumbers.Count == 1
                    ? "&nbsp;Serial Number:&nbsp;"
                    : "&nbsp;Serial Numbers:&nbsp;";

                return prefix + string.Join(", ", SerialNumbers);
            }
        }

        public string ImportValueHtml =>
            $"&nbsp;Import Value:&nbsp;{ImportValue}";

        #endregion
    }

}
