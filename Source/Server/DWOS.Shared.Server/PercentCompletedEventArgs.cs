using System;

namespace DWOS.Shared.Server
{
    /// <summary>
    /// Represents data for a percent completed event.
    /// </summary>
    public class PercentCompletedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the percent, out of 100, for this instance.
        /// </summary>
        public int Percent { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="PercentCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="percent"></param>
        public PercentCompletedEventArgs(int percent)
        {
            Percent = percent;
        }

        #endregion
    }
}
