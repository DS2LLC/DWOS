using System;

namespace DWOS.Shared.Server
{
    /// <summary>
    /// Represents data for a shrink completion event.
    /// </summary>
    public class ShrinkCompletedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the original size of the log files in kilobytes.
        /// </summary>
        public double OriginalSizeKilobytes { get; }

        /// <summary>
        /// Gets the new size of the log files in kilobytes.
        /// </summary>
        public double NewSizeKilobytes { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ShrinkCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="originalSizeKilobytes"></param>
        /// <param name="newSizeKilobytes"></param>
        public ShrinkCompletedEventArgs(double originalSizeKilobytes, double newSizeKilobytes)
        {
            OriginalSizeKilobytes = originalSizeKilobytes;
            NewSizeKilobytes = newSizeKilobytes;
        }

        #endregion
    }
}
