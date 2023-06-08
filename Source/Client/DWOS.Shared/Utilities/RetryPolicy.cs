using System;
using NLog;

namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Refresh time calculator that uses a Max Failure count with an exponential back off algroithm.
    /// </summary>
    public class RetryPolicy
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the refresh time span before adjustments for this instance.
        /// </summary>
        public TimeSpan BaseRefreshTime { get; private set; }

        /// <summary>
        /// Gets the current refresh time span for this instance.
        /// </summary>
        public TimeSpan RefreshTime { get; private set; }

        /// <summary>
        /// Gets the total milliseconds covered by <see cref="RefreshTime"/>
        /// for this instance.
        /// </summary>
        public int RefreshTimeInMilliSeconds
        {
            get { return Convert.ToInt32(RefreshTime.TotalMilliseconds); }
        }

        /// <summary>
        /// Gets the failures count for this instance.
        /// </summary>
        /// <remarks>
        /// On success, this value is reset.
        /// </remarks>
        public int FailureCount { get; private set; }

        /// <summary>
        /// Gets a value indicating if retry should occur.
        /// </summary>
        public bool ShouldContinue
        {
            get { return FailureCount < MaxFailureCount; }
        }

        /// <summary>
        /// Gets the global maximum for failures.
        /// </summary>
        public static int MaxFailureCount { get; private set; }

        #endregion

        #region Methods

        static RetryPolicy() { MaxFailureCount = 5; }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RetryPolicy"/> class.
        /// </summary>
        /// <param name="refreshTime"></param>
        public RetryPolicy(TimeSpan refreshTime)
        {
            BaseRefreshTime = refreshTime;
            RefreshTime = refreshTime;
        }

        /// <summary>
        /// Call when failure occurs.
        /// </summary>
        /// <returns><c>true</c> if should keep trying, <c>false</c> otherwise</returns>
        public bool OnFailure()
        {
            FailureCount += 1;
            _log.Info("RetryPolicy failed {0} attempt.".FormatWith(FailureCount));

            if(FailureCount >= MaxFailureCount)
            {
                RefreshTime = TimeSpan.MaxValue;
                return false;
            }
            //double refresh time
            this.RefreshTime = this.RefreshTime.Add(this.RefreshTime);
            return true;
        }

        /// <summary>
        /// Called when [success] occurs and resets failure counters.
        /// </summary>
        public void OnSuccess()
        {
            _log.Debug("RetryPolicy success.");

            //Reset
            RefreshTime = BaseRefreshTime;
            FailureCount = 0;
        }

        #endregion
    }
}