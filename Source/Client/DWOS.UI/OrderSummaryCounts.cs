using System;

namespace DWOS.UI
{
    /// <summary>
    /// Encapsulates counts for the order summary screen.
    /// </summary>
    public class OrderSummaryCounts
    {
        #region Properties

        /// <summary>
        /// Gets or sets the total count for this instance.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the late count for this instance.
        /// </summary>
        public int LateCount { get; set; }

        /// <summary>
        /// Gets or sets the 'one day before late' count for this instance.
        /// </summary>
        public int Day1Count { get; set; }

        /// <summary>
        /// Gets or sets the 'two days before late' count for this instance.
        /// </summary>
        public int Day2Count { get; set; }

        /// <summary>
        /// Gets or sets the 'three days before late' count for this instance.
        /// </summary>
        public int Day3Count { get; set; }

        /// <summary>
        /// Gets or sets the 'due today' count for this instance.
        /// </summary>
        public int DueTodayCount { get; set; }

        /// <summary>
        /// Gets or sets the quarantine count for this instance.
        /// </summary>
        public int QuarantineCount { get; set; }

        /// <summary>
        /// Gets or sets the external rework count for this instance.
        /// </summary>
        public int ExtReworkCount { get; set; }

        /// <summary>
        /// Gets or sets the internal rework count for this instance.
        /// </summary>
        public int IntReworkCount { get; set; }

        /// <summary>
        /// Gets or sets the hold count for this instance.
        /// </summary>
        public int HoldCount { get; set; }

        /// <summary>
        /// Gets or sets the percentage of late orders for this instance.
        /// </summary>
        public double LateCountPercent => GetPercentageOfTotal(LateCount);

        /// <summary>
        /// Gets or sets the percentage of 'one day before late' orders for this instance.
        /// </summary>
        public double Day1CountPercent => GetPercentageOfTotal(Day1Count);

        /// <summary>
        /// Gets or sets the percentage of 'two days before late' orders for this instance.
        /// </summary>

        public double Day2CountPercent => GetPercentageOfTotal(Day2Count);

        /// <summary>
        /// Gets or sets the percentage of 'three days before late' orders for this instance.
        /// </summary>

        public double Day3CountPercent => GetPercentageOfTotal(Day3Count);

        /// <summary>
        /// Gets or sets the percentage of quarantine orders for this instance.
        /// </summary>
        public double QuarantineCountPercent => GetPercentageOfTotal(QuarantineCount);

        /// <summary>
        /// Gets or sets the percentage of external rework orders for this instance.
        /// </summary>
        public double ExtReworkCountPercent => GetPercentageOfTotal(ExtReworkCount);

        /// <summary>
        /// Gets or sets the percentage of internal rework orders for this instance.
        /// </summary>
        public double IntReworkCountPercent => GetPercentageOfTotal(IntReworkCount);

        /// <summary>
        /// Gets or sets the percentage of hold orders for this instance.
        /// </summary>
        public double HoldCountPercent => GetPercentageOfTotal(HoldCount);

        #endregion

        #region Methods

        private double GetPercentageOfTotal(int count)
        {
            return TotalCount > 0 ? Math.Round(Convert.ToDouble(count)/Convert.ToDouble(TotalCount), 2) : 0D;
        }

        #endregion
    }
}