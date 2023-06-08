using System;
using System.Collections.Generic;

namespace DWOS.Data.Date
{
    /// <summary>
    /// Calculator utility class for days late calculations.
    /// </summary>
    public class DaysLateCalculator
    {
        #region Fields

        /// <summary>
        /// The default number of days late to use if one is not found in lookup.
        /// </summary>
        /// <remarks>
        /// This number represents a 'very late' date in business logic.
        /// </remarks>
        private const int DEFAULT_DAYS_LATE = 99;

        private DateTime _dayLookUpCreatedOn = DateTime.MinValue;
        private readonly Dictionary <DateTime, int> _dayLateLookup = new Dictionary <DateTime, int>();
        private readonly object _initializeLock = new object();

        #endregion

        #region Properties

        public IDateTimeNowProvider NowProvider { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="DaysLateCalculator"/
        /// class.
        /// </summary>
        /// <param name="nowProvider"></param>
        public DaysLateCalculator(IDateTimeNowProvider nowProvider)
        {
            NowProvider = nowProvider;
        }

        private void Initialize()
        {
            lock (_initializeLock)
            {
                var now = NowProvider.Now;
                if (_dayLookUpCreatedOn != now.Date)
                {
                    _dayLookUpCreatedOn = now.Date;
                    _dayLateLookup.Clear();
                    _dayLateLookup.Add(now.StartOfDay(), 0);
                    _dayLateLookup.Add(now.Date.AddBusinessDays(1), 1);
                    _dayLateLookup.Add(now.Date.AddBusinessDays(2), 2);
                    _dayLateLookup.Add(now.Date.AddBusinessDays(3), 3);
                }
            }
        }

        /// <summary>
        /// Using an input date, calculates the number of days late using
        /// business rules.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int GetDaysLate(DateTime date)
        {
            var now = NowProvider.Now;

            // Reset daily - necessary for long-lived instances of this class.
            if (_dayLookUpCreatedOn != now.Date)
            {
                Initialize();
            }

            if (date < now.Date)
            {
                // Order is not late
                return -1;
            }

            return _dayLateLookup.ContainsKey(date) ? _dayLateLookup[date] : DEFAULT_DAYS_LATE;
        }

        #endregion
    }
}
